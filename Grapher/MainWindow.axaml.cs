using System;
using System.Linq;
using Avalonia.Controls;
using ScottPlot.Avalonia;
using Avalonia.Interactivity;
using System.IO;
using System.Text;
using Avalonia.Platform.Storage;

namespace Grapher;

public partial class MainWindow : Window
{
    private double[] _lastXs = [];
    private double[] _lastYs = [];

    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnGenerateClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            double xFrom = double.Parse(XFromInput.Text ?? "-10");
            double xTo = double.Parse(XToInput.Text ?? "10");

            if (xFrom >= xTo) return;

            int points = 500;

            _lastXs = Enumerable.Range(0, points)
                .Select(i => xFrom + i * (xTo - xFrom) / (points - 1))
                .ToArray();

            _lastYs = (FunctionPicker.SelectedIndex switch
            {
                0 => _lastXs.Select(x => x),
                1 => _lastXs.Select(x => x * x),
                2 => _lastXs.Select(x => Math.Sin(x)),
                3 => _lastXs.Select(x => Math.Cos(x)),
                _ => _lastXs.Select(x => x)
            }).ToArray();

            var plt = MyPlot.Plot;
            plt.Clear();
            plt.Add.Scatter(_lastXs, _lastYs);
            MyPlot.Refresh();
        }
        catch (FormatException) { }
    }

    private async void OnExportCsvClick(object? sender, RoutedEventArgs e)
    {
        if (_lastXs.Length == 0) return;

        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Exportovat CSV",
            SuggestedFileName = "graf.csv",
            FileTypeChoices = [new FilePickerFileType("CSV soubory") { Patterns = ["*.csv"] }]
        });

        if (file is null) return;

        var sb = new StringBuilder();
        sb.AppendLine("x,y");
        for (int i = 0; i < _lastXs.Length; i++)
            sb.AppendLine($"{_lastXs[i]},{_lastYs[i]}");

        await using var stream = await file.OpenWriteAsync();
        await using var writer = new StreamWriter(stream);
        await writer.WriteAsync(sb.ToString());
    }
}
