using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace Reservation_SOP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static User LoggedInUser = null;
        RestClient client = null;
        public MainWindow()
        {
            string server = ConfigurationSettings.AppSettings["server"];
            int port = int.Parse(ConfigurationSettings.AppSettings["port"]);
            client = new RestClient(string.Format($"http://{server}:{port}/login"));
            InitializeComponent();
        }

        private void btn_Login_Click(object sender, RoutedEventArgs e)
        {

            if (tb_LoginUsername.Text == "" || tb_LoginPassword.Password == "")
            {
                MessageBox.Show("Username and password is required!");
                return;
            }
            var request = new RestRequest(Method.GET);
            request.RequestFormat = RestSharp.DataFormat.Json;

            request.AddObject(new
            {
                username = tb_LoginUsername.Text,
                password = tb_LoginPassword.Password.ToString()

            });

            var response = client.Execute(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                MessageBox.Show(response.StatusDescription);
                return;
            }

            try
            {
                LoggedInUser = new JsonSerializer().Deserialize<User>(response);
            }
            catch (InvalidCastException)
            {
                MessageBox.Show("Incorrect login credentials!");
                tb_LoginPassword.Clear();
                tb_LoginUsername.Clear();
                return;
            }
            catch (System.Runtime.Serialization.SerializationException)
            {
                MessageBox.Show("Incorrect login credentials!");
                tb_LoginPassword.Clear();
                tb_LoginUsername.Clear();
                return;
            }
            if (LoggedInUser != null)
            {
                MessageBox.Show($"{LoggedInUser.Name} logged in successfully!");
                this.Hide();
                tb_LoginPassword.Clear();
                tb_LoginUsername.Clear();
                ReservationsWindow resWindow = new ReservationsWindow(this);
                resWindow.ShowDialog();
                this.Show();
            }
        }
    }
}
