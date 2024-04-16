using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Data;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataProcess pr = new DataProcess();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DataTable dataTable = new DataTable();
            string myResult = pr.checkDB(ref dataTable);
            if (myResult == "success")
            {
                API_Import.ItemsSource = dataTable.DefaultView;
                Populate.Visibility = Visibility.Hidden;
                Cont.Visibility = Visibility.Visible;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            mainWindow.Visibility = Visibility.Hidden;
            NotificationForm nf = new NotificationForm();
            nf.Owner = this;
            nf.Show();

        }
    }


    public class Manager
    {
        public string? id { get; set; }
        public string? phone { get; set; }
        public string? jurisdiction { get; set; }
        public string? identificationNumber { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }

    }
    
    public class DataProcess
    {
        public string checkDB(ref DataTable myDT)
        {
            string dbStatus = string.Empty;
            string appDir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            appDir += "\\Database2.mdf";
            string conStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + appDir + ";Integrated Security=True";
            string cmdText = @"Select jurisdiction, lastName, firstName from Managers order by jurisdiction, lastName, firstName";
            string cmdText1 = @"Select COUNT(*) as Count from Managers";
            int count = 0;

            const string subkey = "ConValue";
            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(subkey))
            {
                Registry.SetValue(Registry.CurrentUser + "\\" + subkey, "ConnectionStr", conStr);
            }

            using (SqlConnection con = new SqlConnection(conStr))
            {
                using(SqlCommand cmd = new SqlCommand(cmdText1, con))
                {
                    con.Open();
                    count = (int)cmd.ExecuteScalar();
                    con.Close();
                }
            }

            if (count == 0)
            {
                //Get all the records from the API
                dbStatus = dbPopulate();
            }
            else
            {
                //database already populated from API
                dbStatus = "success";
            }

            if (dbStatus == "success")
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    using (SqlCommand cmd = new SqlCommand(cmdText, con))
                    {
                        DataTable dt = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                        myDT = dt;
                    }

                }
            }

            return dbStatus;
        }




        public string dbPopulate()
        {
            string dbstatus = "success";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://o3m5qixdng.execute-api.us-east-1.amazonaws.com/api/");
            HttpResponseMessage response = client.GetAsync("managers").Result;
            
           if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                var myList = System.Text.Json.JsonSerializer.Deserialize<List<Manager>>(result);

                string appDir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
                appDir += "\\Database2.mdf";
                string conStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + appDir + ";Integrated Security=True";

                using (SqlConnection con = new SqlConnection(conStr))
                {
                    string cmdText = @"insert into dbo.Managers (Id, phone, jurisdiction, identificationNumber, firstName, lastName)
                                        values (@Id, @phone, @jurisdiction, @identificationNumber, @firstName, @lastName)";

                    foreach (var manager in myList)
                    {
                        var cmd = new SqlCommand(cmdText, con);
                        cmd.Parameters.AddWithValue("@Id", manager.id);
                        cmd.Parameters.AddWithValue("@phone", manager.phone);
                        cmd.Parameters.AddWithValue("@jurisdiction", manager.jurisdiction);
                        cmd.Parameters.AddWithValue("@identificationNumber", manager.identificationNumber);
                        cmd.Parameters.AddWithValue("@firstName", manager.firstName);
                        cmd.Parameters.AddWithValue("@lastName", manager.lastName);

                        if (con != null && con.State == ConnectionState.Closed)
                        {
                            con.Open();
                        }
                        cmd.ExecuteNonQuery();
                    }
                    if (con != null && con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }

            }
            else
            {
                dbstatus = "error";
            }

            return dbstatus;
        }
    }
    


}