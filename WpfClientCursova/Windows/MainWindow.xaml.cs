using ServiceDll.Realization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfClientCursova.MVVM;
using WpfClientCursova.Windows;

namespace WpfClientCursova
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string userEmail;
        public ObservableCollection<ProductVM> products = new ObservableCollection<ProductVM>();

        public MainWindow(Dictionary<string, string> responseObj)
        {
            InitializeComponent();

            userEmail = responseObj["Email"];

            lblUser.Content = responseObj["Role"] + "\n";
            lblUser.Content += responseObj["FirstName"] + " " + responseObj["LastName"] + "\n";
            lblUser.Content += responseObj["Phone"];

            UpdateDatabase();
        }

        private async void UpdateDatabase()
        {
            products.Clear();
            ProductApiService service = new ProductApiService();
            var list = await service.GetProductsAsync();
            foreach (var item in list)
            {
                ProductVM newProduct = new ProductVM
                {
                    Id = item.Id,
                    Name = item.Name,
                    Price = item.Price,
                    PhotoPath = item.PhotoPath
                };
                products.Add(newProduct);
            }

            lbxProducts.Items.Clear();

            foreach (var item in products)
            {
                StackPanel sp = new StackPanel();

                TextBlock tbName = new TextBlock();
                tbName.Text = item.Name;
                TextBlock tbPrice = new TextBlock();
                tbPrice.Text = item.Price.ToString();

                System.Windows.Controls.Image imgPhoto = new Image();
                if (item.PhotoPath != null)
                {
                    Uri uri = new Uri(item.PhotoPath);
                    imgPhoto.Source = new BitmapImage(uri);
                }

                sp.Children.Add(imgPhoto);
                sp.Children.Add(tbName);
                sp.Children.Add(tbPrice);

                ListBoxItem lbxItem = new ListBoxItem();
                lbxItem.Content = sp;

                lbxProducts.Items.Add(lbxItem);
            }
        }


        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            AddProductWindow dlg = new AddProductWindow();
            dlg.ShowDialog();

            UpdateDatabase();
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
