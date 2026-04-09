using System;
using System.Linq;
using Avalonia.Controls;
using ScottPlot.Avalonia;
using Avalonia.Interactivity;

namespace Grapher;

public partial class MainWindow : Window
{
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

            if (xFrom >= xTo)
            {
                return;
            }

            int points = 500;

            double[] xs = Enumerable.Range(0, points)
                .Select(i => xFrom + i * (xTo - xFrom) / (points - 1))
                .ToArray();

            double[] ys = (FunctionPicker.SelectedIndex switch
            {
                0 => xs.Select(x => x),
                1 => xs.Select(x => x * x),
                2 => xs.Select(x => Math.Sin(x)),
                3 => xs.Select(x => Math.Cos(x)),
                _ => xs.Select(x => x)
            }).ToArray();

            var plt = MyPlot.Plot;
            plt.Clear();
            plt.Add.Scatter(xs, ys);
            MyPlot.Refresh();
        }
        catch (FormatException){}
    }
}