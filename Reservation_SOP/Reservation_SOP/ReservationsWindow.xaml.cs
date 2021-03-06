using RestSharp;
using System;
using System.Collections.Generic;
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
using System.Drawing;
using System.Configuration;
using System.Threading;
using RestSharp.Serialization.Json;
using Rectangle = System.Windows.Shapes.Rectangle;
using Brushes = System.Windows.Media.Brushes;

namespace Reservation_SOP
{
    /// <summary>
    /// Interaction logic for ReservationsWindow.xaml
    /// </summary>
    public partial class ReservationsWindow : Window
    {
        RestClient client = null;
        List<Reservation> reservations;
        private List<Reservation> pending = new List<Reservation>();
        private Reservation ClickedSeat;
        private const int size = 21;
        private const int space = 4;
        private bool[,] seats = new bool[20, 20];
        public ReservationsWindow(MainWindow Main)
        {
            string server = ConfigurationSettings.AppSettings["server"];
            int port = int.Parse(ConfigurationSettings.AppSettings["port"]);
            client = new RestClient(string.Format($"http://{server}:{port}/reservation"));
            InitializeComponent();
            ListReservations();
            ResetInputs();
            DrawTable(can_seats, null);
            txtblk_LoggedinUsername.Text += " " + MainWindow.LoggedInUser.Name;

        }
        public MainWindow Main { get; set; }
        private void ListReservations()
        {
            seats = new bool[20, 20];
            var request = new RestRequest(Method.GET);
            request.RequestFormat = RestSharp.DataFormat.Json;

            request.AddObject(new
            {
                username = MainWindow.LoggedInUser.Name,
                password = MainWindow.LoggedInUser.Password
            });

            var response = client.Execute(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                MessageBox.Show(response.Content);
                return;
            }

            reservations = new JsonSerializer().Deserialize<List<Reservation>>(response);

            foreach (Reservation r in reservations)
            {
                seats[r.SeatColumn, r.SeatRow] = true;
            }
        }
        public void DrawTable(Canvas Canvas, object sender)
        {
            Canvas.Children.Clear();
            for (int i = 0; i < seats.GetLength(1); i++)
            {
                for (int j = 0; j < seats.GetLength(0); j++)
                {
                    Rectangle rectangle = new Rectangle
                    {
                        Height = size,
                        Width = size,
                    };
                    if (MainWindow.LoggedInUser.Admin == 0)
                    {
                        
                        if (!seats[i, j])
                        {
                            rectangle.MouseDown += Reserve;
                            rectangle.Fill = Brushes.Green;
                        }
                        else if (reservations.Find(x => x.ReservedBy == MainWindow.LoggedInUser.Name && x.SeatColumn == i && x.SeatRow == j) != null)
                        {
                            rectangle.Fill = Brushes.Pink;
                        }
                        else
                        {
                            rectangle.Fill = Brushes.Red;
                        }
                       
                    }
                    else
                    {
                        Button clickedButton = (Button)sender;
                        if (seats[i, j])
                        {
                            if (clickedButton == null)
                            {
                                if (reservations.Find(x => x.ReservedBy == MainWindow.LoggedInUser.Name && x.SeatColumn == i && x.SeatRow == j) != null)
                                {
                                    rectangle.Fill = Brushes.Pink;
                                }
                                else rectangle.Fill = Brushes.Red;

                            }
                            else if (clickedButton.Name == "btn_Search")
                            {
                                if (reservations.Find(x => x.ReservedBy == tb_ReservationName.Text && x.SeatColumn == i && x.SeatRow == j) != null)
                                {
                                    rectangle.Fill = Brushes.Magenta;
                                }
                                else if (reservations.Find(x => x.ReservedBy == MainWindow.LoggedInUser.Name && x.SeatColumn == i && x.SeatRow == j) != null)
                                {
                                    rectangle.Fill = Brushes.Pink;
                                }
                                else
                                {
                                    rectangle.Fill = Brushes.Red;
                                }
                            }
                            rectangle.MouseDown += ReservationViewing;
                        }
                        else
                        {
                            rectangle.Fill = Brushes.Green;
                            rectangle.MouseDown += Reserve;
                        }
                    }

                    Canvas.Children.Add(rectangle);
                    Canvas.SetLeft(rectangle, i * (size + space));
                    Canvas.SetTop(rectangle, j * (size + space));
                }
            }
        }
        private void ReservationViewing(object sender, MouseButtonEventArgs e)
        {
            Rectangle rectangle = sender as Rectangle;

            int px = Convert.ToInt32(Canvas.GetLeft(rectangle)) / (size + space);
            int py = Convert.ToInt32(Canvas.GetTop(rectangle)) / (size + space);

            ClickedSeat = reservations.Find(x => x.SeatColumn == px && x.SeatRow == py);

            tb_ReservationName.Text = ClickedSeat.ReservedBy;
            tb_SeatColumn.Text = (ClickedSeat.SeatColumn + 1).ToString();
            tb_SeatRow.Text = (ClickedSeat.SeatRow + 1).ToString();
        }
        private void Reserve(object sender, MouseButtonEventArgs e)
        {
            ClickedSeat = null;
            Rectangle rectangle = sender as Rectangle;
            Reservation temp;
            rectangle.Fill = (rectangle.Fill == Brushes.Green) ? Brushes.LightBlue : Brushes.Green;

            int px = Convert.ToInt32(Canvas.GetLeft(rectangle)) / (size + space);
            int py = Convert.ToInt32(Canvas.GetTop(rectangle)) / (size + space);

            if (px < seats.GetLength(1) && py < seats.GetLength(0))
            {
                if (!seats[px, py])
                {
                    temp = new Reservation(py, px);
                    pending.Add(temp);
                    tb_SeatColumn.Text = (temp.SeatColumn + 1).ToString();
                    tb_SeatRow.Text = (temp.SeatRow + 1).ToString();
                }
                if (seats[px, py])
                {
                    temp = pending.Find(x => x.SeatColumn == px && x.SeatRow == py);
                    pending.Remove(temp);
                    tb_SeatRow.Clear();
                    tb_SeatColumn.Clear();
                }
                SwitchState(px, py);
            }
        }
        public void ResetInputs()
        {
            if (MainWindow.LoggedInUser.Admin == 0)
            {
                btn_DeleteByName.Visibility = Visibility.Hidden;
                btn_DeleteByName.IsEnabled = false;
                btn_DeleteSelected.Visibility = Visibility.Hidden;
                btn_DeleteSelected.IsEnabled = false;

                btn_EditSelected.Visibility = Visibility.Hidden;
                btn_EditSelected.IsEnabled = false;

                btn_Search.Visibility = Visibility.Hidden;
                btn_Search.IsEnabled = false;

                tb_SeatColumn.IsEnabled = false;
                tb_SeatRow.IsEnabled = false;
                tb_ReservationName.Visibility = Visibility.Hidden;
                lbl_reserveFor.Visibility = Visibility.Hidden;
                rec_HighlightedSeat.Visibility = Visibility.Hidden;
                lbl_Highlighted.Visibility = Visibility.Hidden;

                btn_ResetInputs.Visibility = Visibility.Hidden;
            }
            else
            {
                btn_DeleteByName.Visibility = Visibility.Visible;
                btn_DeleteByName.IsEnabled = true;
                btn_DeleteSelected.Visibility = Visibility.Visible;
                btn_DeleteSelected.IsEnabled = true;

                btn_EditSelected.Visibility = Visibility.Visible;
                btn_EditSelected.IsEnabled = true;

                btn_Search.Visibility = Visibility.Visible;
                btn_Search.IsEnabled = true;

                tb_ReservationName.IsEnabled = true;
                tb_ReservationName.IsReadOnly = false;
                tb_SeatColumn.IsEnabled = true;
                tb_SeatRow.IsEnabled = true;
                rec_HighlightedSeat.Visibility = Visibility.Visible;
                lbl_Highlighted.Visibility = Visibility.Visible;
            }
            tb_ReservationName.Clear();
            tb_SeatColumn.Clear();
            tb_SeatRow.Clear();
            ClickedSeat = null;
            ButtonCheck();
        }
        public void SwitchState(int x, int y)
        {
            ButtonCheck();
            seats[x, y] = !seats[x, y];
        }
        public void ButtonCheck()
        {
            if (pending.Count == 0)
            {
                btn_reserve.IsEnabled = false;
            }
            else
            {
                btn_reserve.IsEnabled = true;
            }
        }

        private void btn_Search_Click(object sender, RoutedEventArgs e)
        {
            ListReservations();
            if (tb_ReservationName.Text == "")
            {
                MessageBox.Show("Please input a name");
                return;
            }
            if (reservations.Find(x => x.ReservedBy == tb_ReservationName.Text) == null)
            {
                MessageBox.Show("The given reservation with this name does not exist!");
                return;
            }
            pending.Clear();
            ButtonCheck();
            ListReservations();
            DrawTable(can_seats, sender);
        }

        private void btn_reserve_Click(object sender, RoutedEventArgs e)
        {
            ListReservations();
            if (pending.Count == 0)
            {
                MessageBox.Show("Please select seat(s) to reserve");
                return;
            }
            bool successful = false;
            string message = "Failed, please try again";

            foreach (Reservation r in pending)
            {
                var request = new RestRequest(Method.POST);
                request.RequestFormat = RestSharp.DataFormat.Json;
                request.AddJsonBody(new
                {
                    reservator = MainWindow.LoggedInUser.Name,
                    password = MainWindow.LoggedInUser.Password,
                    rownum = r.SeatRow,
                    columnnum = r.SeatColumn
                });
                var response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    successful = true;
                    message = response.Content;
                }
                else
                {
                    message = response.Content;
                    pending.Clear();
                    break;
                }
            }
            if (!successful)
            {
                MessageBox.Show(message);
                pending.Clear();
                ListReservations();
                DrawTable(can_seats, null);
                return;
            }
            else
            {
                MessageBox.Show(message);
                pending.Clear();
                ListReservations();
                ResetInputs();
                DrawTable(can_seats, null);
            }
        }
        private void btn_EditSelected_Click(object sender, RoutedEventArgs e)
        {
            pending.Clear();
            if (ClickedSeat == null)
            {
                MessageBox.Show("Please select a seat!");
                return;
            }
            else
            {
                var request = new RestRequest(Method.PUT);
                request.RequestFormat = RestSharp.DataFormat.Json;

                request.AddJsonBody(new
                {
                    id = ClickedSeat.ID,
                    seatrow = tb_SeatRow.Text,
                    seatcolumn = tb_SeatColumn.Text,
                    username = MainWindow.LoggedInUser.Name,
                    password = MainWindow.LoggedInUser.Password
                });

                var response = client.Execute(request);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show(response.StatusDescription);
                    return;
                }
                MessageBox.Show(response.Content);
                ClickedSeat = null;
                ResetInputs();
                ListReservations();
                DrawTable(can_seats, null);
            }
        }

        private void btn_Update_Click(object sender, RoutedEventArgs e)
        {
            ListReservations();
            pending.Clear();
            ButtonCheck();
            DrawTable(can_seats, null);
        }

        private void btn_ResetInputs_Click(object sender, RoutedEventArgs e)
        {
            ResetInputs();
            ClickedSeat = null;
        }

        private void btn_DeleteByName_Click(object sender, RoutedEventArgs e)
        {
                pending.Clear();
                if (tb_ReservationName.Text == "")
                {
                    MessageBox.Show("Name is empty, please try again");
                    return;
                }
                else
                {
                    ListReservations();
                    Reservation selected = reservations.Find(x => x.ReservedBy == tb_ReservationName.Text);
                    if (selected == null)
                    {
                        MessageBox.Show("There is no reservation with this name");
                        pending.Clear();
                        return;
                    }
                    else
                    {
                    var request = new RestRequest(Method.DELETE);
                    request.RequestFormat = RestSharp.DataFormat.Json;

                    request.AddObject(new
                    {
                        reservedby = selected.ReservedBy,
                        username = MainWindow.LoggedInUser.Name,
                        password = MainWindow.LoggedInUser.Password
                    });

                    var response = client.Execute(request);
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        MessageBox.Show(response.Content);
                        DrawTable(can_seats, null);
                        return;
                    }
                    else
                    {
                        MessageBox.Show(response.Content);
                    }
                }
                }
                ListReservations();
                ResetInputs();
                DrawTable(can_seats, null);
            }


        private void btn_DeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            pending.Clear();
            if (ClickedSeat == null)
            {
                MessageBox.Show("Please select a seat!");
                return;
            }

            var request = new RestRequest(Method.DELETE);
            request.RequestFormat = RestSharp.DataFormat.Json;

            request.AddObject(new
            {
                id = ClickedSeat.ID,
                username = MainWindow.LoggedInUser.Name,
                password = MainWindow.LoggedInUser.Password
            });

            var response = client.Execute(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                MessageBox.Show(response.Content);
                DrawTable(can_seats, null);
                return;
            }
            MessageBox.Show(response.Content);
            ResetInputs();
            ListReservations();
            DrawTable(can_seats, null);
            ClickedSeat = null;
        }
        private void btn_LogOut_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.LoggedInUser = null;
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow.LoggedInUser = null;
        }
    }
}
