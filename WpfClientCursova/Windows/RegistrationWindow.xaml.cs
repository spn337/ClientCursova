using ServiceDll.Models;
using ServiceDll.Realization;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;
using System.Windows.Media;

namespace WpfClientCursova.Windows
{
    /// <summary>
    /// Interaction logic for RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private async void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            tbWarning.Text = "";

            if (tbPassword.Password != tbConfirmPassword.Password)
            {
                tbWarning.Text = "Пароль не співпадає";
                return;
            }
            if (tbFirstName.Text == tbLastName.Text && tbFirstName.Text != "")
            {
                tbWarning.Text = "Поле FirstName та LastName не можуть співпадати";
                return;
            }

            // відправляємо модель на сервер
            AccountApiService service = new AccountApiService();

            var response = await service.RegistrationAsync(new UserModel
            {
                FirstName = tbFirstName.Text,
                LastName = tbLastName.Text,
                Password = tbPassword.Password,
                Email = tbEmail.Text,
                Phone = tbPhone.Text
            });

            // витягуємо помилки, якщо поля невалідні
            if (response != null)
            {
                string message = "";
                for (int i = 0; i < response.Count; i++)
                {
                    message += response[i] += "\n";
                }
                tbWarning.Text = message;
            }
            // в іншому випадку реєстрація успішна
            else
            {
                tbWarning.Foreground = Brushes.Blue;
                tbWarning.Text = "Реєстрація успішна";

                btnSend.IsEnabled = false;
            }
        }

        private void TbPhone_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (tbPhone.Text.Length <= 1)
            {
                tbPhone.Text = "+3";
                tbPhone.SelectionStart = 2;
                tbPhone.SelectionLength = 0;
            }
        }
    }
}
