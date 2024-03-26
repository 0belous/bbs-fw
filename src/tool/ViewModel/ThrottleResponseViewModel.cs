using BBSFW.ViewModel.Base;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System.Collections.Generic;
using System;
using System.ComponentModel;

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

		private void ConfigVm_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(ConfigurationViewModel.QuadraticFactor))
			{
				UpdateGraphData();
			}
		}

		public ThrottleResponseViewModel(ConfigurationViewModel config)
		{
			_configVm = config;

			_configVm.PropertyChanged += ConfigVm_PropertyChanged;

			// Initialize the PlotModel with default values
			IsLinearChecked = true;
			PlotModel = new PlotModel { Title = "Throttle Response" };
			PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Throttle Position", IsPanEnabled = false, IsZoomEnabled = false });
			PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Power Output", IsPanEnabled = false, IsZoomEnabled = false });

			// Render graph initially
			UpdateGraphData();

			PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == nameof(IsLinearChecked) || e.PropertyName == nameof(IsQuadraticChecked) || e.PropertyName == nameof(_configVm.QuadraticFactor))
				{
					if (IsLinearChecked) {
						_configVm.QuadraticFactor = 1;
					}
					UpdateGraphData();
				}
			};
		}

		private void UpdateGraphData()
		{
			PlotModel.Series.Clear();

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
				else if (IsQuadraticChecked)
				{
					double quadraticFactor = double.Parse(_configVm.QuadraticFactor.ToString());
					powerOutput = Math.Pow(i / 100.0, quadraticFactor) * 100; // quadratic relationship
				}
				series.Points.Add(new DataPoint(i, powerOutput));
			}

			PlotModel.Series.Add(series);
			PlotModel.InvalidatePlot(true);
		}
	}
}
