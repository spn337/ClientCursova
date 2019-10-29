using ServiceDll.Models;
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
        public ObservableCollection<ProductVM> Products = new ObservableCollection<ProductVM>();

        public MainWindow(Dictionary<string, string> responseObj)
        {
            InitializeComponent();

            userEmail = responseObj["Email"];

            lblUser.Content = responseObj["Role"] + "\n";
            lblUser.Content += responseObj["FirstName"] + " " + responseObj["LastName"] + "\n";
            lblUser.Content += responseObj["Phone"];

            lbxProducts.ItemsSource = Products;

            UpdateDatabase();
        }

        private async void UpdateDatabase()
        {
            Products.Clear();
            ProductApiService service = new ProductApiService();
            var list = await service.GetProductsAsync();
            foreach (var item in list)
            {
                ProductVM newProduct = new ProductVM
                {
                    Id = item.Id,
                    Name = item.Name,
                    Price = item.Price,
                    PhotoPath = "https://localhost:44329/images/" + item.PhotoName
                };
                Products.Add(newProduct);
            }
        }


        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            AddProductWindow dlg = new AddProductWindow();
            dlg.ShowDialog();

            UpdateDatabase();
        }
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = lbxProducts.SelectedItem as ProductVM;
            if (selectedItem != null)
            {
                if (MessageBox.Show("Видалити продукт " + selectedItem.Name + "?", "Видалення",
                    MessageBoxButton.YesNo)
                    == MessageBoxResult.Yes)
                {
                    ProductApiService service = new ProductApiService();
                    service.Delete(new ProductDeleteModel
                    {
                        Id = selectedItem.Id
                    });
                }

                UpdateDatabase();
            }        
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
