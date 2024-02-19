using BBSFW.ViewModel.Base;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System.Collections.Generic;
using System;

namespace BBSFW.ViewModel
{
	public class ThrottleResponseViewModel : ObservableObject
	{

		private bool _isLinearChecked;
		public bool IsLinearChecked
		{
			get { return _isLinearChecked; }
			set
			{
				if (_isLinearChecked != value)
				{
					_isLinearChecked = value;
					OnPropertyChanged(nameof(IsLinearChecked));
				}
			}
		}

		private ConfigurationViewModel _configVm;
		public ConfigurationViewModel ConfigVm
		{
			get { return _configVm; }
		}

		private bool _isQuadraticChecked;
		public bool IsQuadraticChecked
		{
			get { return _isQuadraticChecked; }
			set
			{
				if (_isQuadraticChecked != value)
				{
					_isQuadraticChecked = value;
					OnPropertyChanged(nameof(IsQuadraticChecked));
				}
			}
		}

		private string _quadraticFactor;
		public string QuadraticFactor
		{
			get { return _quadraticFactor; }
			set
			{
				if (_quadraticFactor != value)
				{
					_quadraticFactor = value;
					OnPropertyChanged(nameof(QuadraticFactor));
				}
			}
		}

		private PlotModel _plotModel;
		public PlotModel PlotModel
		{
			get { return _plotModel; }
			set
			{
				if (_plotModel != value)
				{
					_plotModel = value;
					OnPropertyChanged(nameof(PlotModel));
				}
			}
		}
		public ThrottleResponseViewModel(ConfigurationViewModel config)
		{
			_configVm = config;

			// Initialize the PlotModel with default values
			IsLinearChecked = true;
			QuadraticFactor = "1.5";
			PlotModel = new PlotModel { Title = "Throttle Response" };
			PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Throttle Position", IsPanEnabled = false });
			PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Power Output", IsPanEnabled = false  });

			// Render graph initially
			UpdateGraphData();

			// Update the graph data whenever IsLinearChecked, IsQuadraticChecked, or QuadraticFactor changes
			PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == nameof(IsLinearChecked) || e.PropertyName == nameof(IsQuadraticChecked) || e.PropertyName == nameof(QuadraticFactor))
				{
					UpdateGraphData();
				}
			};
		}

		private void UpdateGraphData()
		{
			PlotModel.Series.Clear();

			_configVm.QuadraticFactor = float.Parse(QuadraticFactor);

			var series = new LineSeries
			{
				Title = "Power Output",
				TrackerFormatString = "{0}\n{1}: {2:0}\n{3}: {4:0}"
			};
			for (int i = 0; i <= 100; i++)
			{
				double powerOutput = 0;
				if (IsLinearChecked)
				{
					powerOutput = i; // linear relationship
				}
				else if (IsQuadraticChecked && QuadraticFactor != null)
				{
					double quadraticFactor = double.Parse(QuadraticFactor);
					powerOutput = Math.Pow(i / 100.0, quadraticFactor) * 100; // quadratic relationship
				}
				series.Points.Add(new DataPoint(i, powerOutput));
			}

			PlotModel.Series.Add(series);
			PlotModel.InvalidatePlot(true);
		}
	}
}
