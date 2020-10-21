using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace Finam
{
    /// <summary>
    /// Interaction logic for Chart.xaml
    /// </summary>
    public partial class Chart : Window
    {
        private string[] _labels;

        public Chart(DataTable chartTable)
        {
            InitializeComponent();

            ChartValues<OhlcPoint> ohlcPoints = new ChartValues<OhlcPoint>();
            foreach (DataRow row in chartTable.Rows)
            {
                ohlcPoints.Add(new OhlcPoint(Convert.ToDouble((decimal)row[4]), Convert.ToDouble((decimal)row[5]), Convert.ToDouble((decimal)row[6]), Convert.ToDouble((decimal)row[7])));
            }

            SeriesCollection = new SeriesCollection
            {
                new CandleSeries()
                {
                    Values = ohlcPoints
                }
            };

            //based on https://github.com/beto-rodriguez/Live-Charts/issues/166 
            //The Ohcl point X property is zero based indexed.
            //this means the first point is 0, second 1, third 2.... and so on
            //then you can use the Axis.Labels properties to map the chart X with a label in the array.
            //for more info see (mapped labels section) 
            //http://lvcharts.net/#/examples/v1/labels-wpf?path=WPF-Components-Labels

            List<string> dates = new List<string>();
            foreach (DataRow row in chartTable.Rows)
            {
                DateTime dateTime = (DateTime)row[2];
                TimeSpan timeSpan = (TimeSpan)row[3];
                string s = dateTime.AddHours(timeSpan.Hours).ToString("dd.MM.yyyy HH");
                dates.Add(s);
            }
            Labels = dates.ToArray();

            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; set; }

        public string[] Labels
        {
            get { return _labels; }
            set
            {
                _labels = value;
                OnPropertyChanged("Labels");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
