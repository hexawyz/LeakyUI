using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using LeakyUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LeakyUI
{
	/// <summary>
	/// An empty window that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainWindow : Window
	{
		private sealed class ViewModel : INotifyPropertyChanged
		{
			public TimeSeries Series { get; }

			public event PropertyChangedEventHandler PropertyChanged;

			public ViewModel(TimeSeries timeSeries)
			{
				Series = timeSeries;
			}
		}

		private sealed class TimeSeries : ITimeSeries
		{
			public DateTime StartTime => DateTime.UtcNow;
			public TimeSpan Interval => TimeSpan.FromTicks(100 * TimeSpan.TicksPerMillisecond);
			public int Length => 60;

			public double this[int index] => Math.Sin(index * (2 * Math.PI / 60));

			public double? MaximumReachedValue => 1;
			public double? MinimumReachedValue => -1;

			public event EventHandler Changed;

			public TimeSeries()
			{
				_ = RunAsync();
			}

			private async Task RunAsync()
			{
				var random = new Random();
				while (true)
				{
					await Task.Delay(500);
					Changed?.Invoke(this, EventArgs.Empty);
				}
			}
		}

		public MainWindow()
		{
			InitializeComponent();
			const int Size = 10;
			var ts = new TimeSeries();
			for (int i = 0; i < Size; i++)
			{
				Root.ColumnDefinitions.Add(new());
				Root.RowDefinitions.Add(new());
				for (int j = 0; j < Size; j++)
				{
					var chart = new LineChart();
					chart.SetBinding(LineChart.SeriesProperty, new Binding() { Path = new PropertyPath("Series"), Mode = BindingMode.OneWay });
					chart.SetValue(Grid.RowProperty, i);
					chart.SetValue(Grid.ColumnProperty, j);
					chart.DataContext = new ViewModel(ts);
					Root.Children.Add(chart);
				}
			}
		}
	}
}
