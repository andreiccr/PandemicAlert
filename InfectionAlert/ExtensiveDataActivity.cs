using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Xamarin.Android;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PandemicAlert
{
   
    [Activity(Label = "ExtensiveDataActivity")]
    public class ExtensiveDataActivity : Activity
    { 

        PlotView chart;
        IDataService dataService;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            dataService = new DataService();
        }

        private PlotModel ModelInfections()
        {
            if (dataService.Infections == null || dataService.Infections.Count == 0)
            {
                return null;
            }

            var plotModel = new PlotModel { Title = "Infecții" };

            plotModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, StringFormat = "dd.MM.yyyy" });
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = 0 });

            var series1 = new RectangleBarSeries
            {
                FillColor = OxyColors.Red,
                StrokeColor = OxyColors.Red
            };

            foreach (var entry in dataService.Infections)
            {
                //series1.Items.Add(new RectangleBarItem(DateTimeAxis.ToDouble(entry.Key), 0, DateTimeAxis.ToDouble(entry.Key.AddHours(12)), compute.GetInfectedDay(entry.Key)));
                //series1.Points.Add(new DataPoint(DateTimeAxis.ToDouble(entry.Key), compute.GetInfectedDay(entry.Key)));
            }

            plotModel.Series.Add(series1);

            return plotModel;
        }
    }

    
}