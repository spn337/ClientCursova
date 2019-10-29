using Microsoft.Win32;
using ServiceDll.Helpers;
using ServiceDll.Realization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfClientCursova.Windows
{
    /// <summary>
    /// Interaction logic for AddProductWindow.xaml
    /// </summary>
    public partial class AddProductWindow : Window
    {
        string base64Image = "";
        public AddProductWindow()
        {
            InitializeComponent();
        }

        private void BtnLoadPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) " +
               "| *.jpg; *.jpeg; *.jpe; *.jfif; *.png",
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    string filePath = dlg.FileName;
                    var image = Image.FromFile(filePath);
                    imgPhoto.Source = new BitmapImage(new Uri(filePath));
                    base64Image = image.ConvertToBase64String();
                }
                catch (Exception)
                {
                    MessageBox.Show("There was a problem downloading the file");
                }
            }
        }
        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            tbWarningName.Text = "";
            tbWarningPrice.Text = "";

            Dictionary<string, string> errorList = new Dictionary<string, string>();

            ProductApiService service = new ProductApiService();

            try
            {
                errorList = await service.CreateAsync(new ServiceDll.Models.ProductAddModel
                {
                    Name = tbName.Text,
                    Price = Convert.ToDecimal(tbPrice.Text),
                    Photo = base64Image
                });

                // витягуємо помилки, якщо поля невалідні
                if (errorList != null)
                {
                    foreach (var item in errorList)
                    {
                        if ("name" == item.Key)
                            tbWarningName.Text = item.Value;

                        if ("price" == item.Key)
                            tbWarningPrice.Text = item.Value;
                    }
                }
                // в іншому випадку - успішно
                else
                {
                    this.Close();
                }
            }
            catch
            {
                tbWarningPrice.Text = "Неправильне число";
            }
        }
    }
}
