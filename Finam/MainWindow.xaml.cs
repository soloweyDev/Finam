using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

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
                connection = new SqlConnection("Data Source=DESKTOP-MY\\SQLEXPRESS;Initial Catalog=Finam;Integrated Security=True");
                SqlCommand command = new SqlCommand(sql, connection);
                adapter = new SqlDataAdapter(command);

                // установка команды на добавление для вызова хранимой процедуры
                adapter.InsertCommand = new SqlCommand("spGetChart", connection);
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

        struct ChartStruct
        {
            decimal Open;
            decimal High;
            decimal Low;
            decimal Close;
        }
    }
}
