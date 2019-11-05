using Microsoft.Win32;
using ServiceDll.Helpers;
using ServiceDll.Models;
using ServiceDll.Realization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfClientCursova.Windows
{
    /// <summary>
    /// Interaction logic for EditProduct.xaml
    /// </summary>
    public partial class EditProductWindow : Window
    {
        public int IdProduct { get; set; }
        ProductApiService service = new ProductApiService();
        string base64Image = "";
        public EditProductWindow()
        {
            InitializeComponent();
        }

        private async void BtnLoadPhoto_Click(object sender, RoutedEventArgs e)
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
                    LogoWindow logo = new LogoWindow(this.Left, this.Top, this.Height, this.Width);
                    logo.Show();

                    await Task.Run(() =>
                    {
                        string filePath = dlg.FileName;
                        var image = Image.FromFile(filePath);
                        base64Image = image.ConvertToBase64String();
                        this.Dispatcher.BeginInvoke((Action)(() => imgPhoto.Source = new BitmapImage(new Uri(filePath))));
                    });

                    logo.Close();
                }
                catch (Exception)
                {
                    MessageBox.Show("There was a problem downloading the file");
                }
            }
        }
        private async void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            tbWarningName.Text = "";
            tbWarningPrice.Text = "";

            Dictionary<string, string> errorList = new Dictionary<string, string>();

            try
            {

                LogoWindow logo = new LogoWindow(this.Left, this.Top, this.Height, this.Width);
                logo.Show();

                errorList = await service.EditSaveAsync(new ProductEditModel
                {
                    Id = IdProduct,
                    Name = tbName.Text,
                    Price = Convert.ToDecimal(tbPrice.Text),
                    PhotoBase64 = base64Image
                });

                logo.Close();

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var product = service.EditGetById(this.IdProduct);

            tbName.Text = product.Name;
            tbPrice.Text = product.Price.ToString();

            string hostUrl = ConfigurationManager.AppSettings["HostUrl"];
            string uri = $"{hostUrl}images/{product.PhotoName}";
            imgPhoto.Source = new BitmapImage(new Uri(uri));
        }
    }
}
