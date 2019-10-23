using ServiceDll.Realization;
using System;
using System.Collections.Generic;
using System.Windows;
using WpfClientCursova.Windows;

namespace WpfClientCursova
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string userEmail;
        public MainWindow(Dictionary<string, string> responseObj)
        {
            InitializeComponent();

            userEmail = responseObj["Email"];

            lblUser.Content = responseObj["Role"] + "\n";
            lblUser.Content += responseObj["FirstName"] + " " + responseObj["LastName"] + "\n";
            lblUser.Content += responseObj["Phone"];
        }

        private async void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            AccountApiService service = new AccountApiService();
            bool Logout = await service.LogoutAsync(userEmail);
            if (Logout)
            {
                LoginWindow window = new LoginWindow();
                window.Show();
                this.Close();
            }
        }
    }
}
