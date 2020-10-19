using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Finam
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly SqlDataAdapter adapter;
        readonly DataTable chartTable;

        public MainWindow()
        {
            InitializeComponent();

            string sql = "SELECT * FROM Chart";
            chartTable = new DataTable();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection("Data Source=localhost\\SQLEXPRESS;Initial Catalog=Finam;Integrated Security=True");
                SqlCommand command = new SqlCommand(sql, connection);
                adapter = new SqlDataAdapter(command)
                {

                    // установка команды на добавление для вызова хранимой процедуры
                    InsertCommand = new SqlCommand("spGetChart", connection)
                };
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;

                connection.Open();
                adapter.Fill(chartTable);
                dg.ItemsSource = chartTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
        }

        private void dg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            var arrayObjects = (dg.SelectedItem as DataRowView).Row.ItemArray;

            double step = 200.0;
            double open = Convert.ToDouble((decimal)arrayObjects[4]);
            double high = Convert.ToDouble((decimal)arrayObjects[5]);
            double low = Convert.ToDouble((decimal)arrayObjects[6]);
            double close = Convert.ToDouble((decimal)arrayObjects[7]);

            Open.Content = arrayObjects[4].ToString();
            High.Content = arrayObjects[5].ToString();
            Low.Content = arrayObjects[6].ToString();
            Close.Content = arrayObjects[7].ToString();

            if (open > close)
            {
                Color color =  Colors.Red;
                Line.Stroke = new SolidColorBrush(color);

                double delta = (open - close) * step / (high - low);
                Rectangle.Height = delta != 0 ? delta : 1;

                Rectangle.RenderTransform = new TranslateTransform(0, (high - open) * step / (high - low));
                Rectangle.Fill = new SolidColorBrush(color);
            }
            else
            {
                Color color =  Colors.Green;
                Line.Stroke = new SolidColorBrush(color);

                double delta = (close - open) * step / (high - low);
                Rectangle.Height = delta != 0 ? delta : 1;

                Rectangle.RenderTransform = new TranslateTransform(0, (high - close) * step / (high - low));
                Rectangle.Fill = new SolidColorBrush(color);
            }
        }
    }
}
