﻿using ServiceDll.Models;
using ServiceDll.Realization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfClientCursova.MVVM;
using WpfClientCursova.Windows;
using System.Windows.Media;

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

            UpdateDatabase();
        }

        private async void UpdateDatabase()
        {
            Products.Clear();
            ProductApiService service = new ProductApiService();
            var list = await service.GetProductsAsync();
            foreach (var item in list)
            {
                string hostUrl = ConfigurationManager.AppSettings["HostUrl"];
                ProductVM newProduct = new ProductVM
                {
                    Id = item.Id,
                    Name = item.Name,
                    Price = item.Price,
                    PhotoPath = $"{hostUrl}images/{item.PhotoName}"
                };
                Products.Add(newProduct);

            }
            ShowPage();
        }
        public void ShowPage(int currentPage = 1)
        {
            int countDataInPage = 3;

            // вибираємо товари на конкретній сторінці
            var productsInPage = Products
                .Skip(countDataInPage * (currentPage - 1))
                .Take(countDataInPage)
                .ToList();

            //відображаємо на екран
            lbxProducts.ItemsSource = productsInPage;

            // генеруємо кількість кнопок
            double countButtons = Products.Count / countDataInPage;
            if (Products.Count % countDataInPage != 0)
            {
                countButtons++;
            }

            // малюємо панель з кнопками
            ShowPaginationPanel(Convert.ToInt32(countButtons), countDataInPage, currentPage);
        }
        public void ShowPaginationPanel(int countButtons, int countDataInPage, int currentPage)
        {
            int lastPage = countButtons;
            int step = -4;

            //cut max counts of buttons
            if (countButtons > 15)
                countButtons = 15;

            //create container
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            gbPages.Content = sp;

            //create childrens(buttons)
            for (int i = 0; i < countButtons; i++)
            {
                Button dynamicButton = new Button();
                ///////////////////////////////////////////////////////////
                //if current page <= 1....7(in the left)
                ///////////////////////////////////////////////////////////
                if (currentPage <= countButtons / 2)
                {
                    //set(...) (is almost in the end)
                    if (i == countButtons - 2 && i != 0)
                    {
                        dynamicButton = CreateElipsisButton();
                    }
                    else
                    {
                        //button's numerable(1-8 ... lastPage)
                        int number = (i != countButtons - 1) ? i + 1 : lastPage;
                        dynamicButton = CreatePageButton(number, currentPage);
                    }
                }
                ///////////////////////////////////////////////////////////
                //if current page >= 8...11(in the middle)
                ///////////////////////////////////////////////////////////
                else if (currentPage > countButtons / 2 && currentPage <= lastPage - countButtons / 2)
                {
                    //set(...) (is between numbers in the middle)

                    if (i == 3 || i == countButtons - 2)
                    {
                        dynamicButton = CreateElipsisButton();
                    }
                    //set numbers
                    else
                    {
                        int number;

                        if (i < 3)
                        {
                            number = i + 1;
                        }
                        else if (i == countButtons - 1)
                        {
                            number = lastPage;
                        }
                        else
                        {
                            number = currentPage + step;
                            step++;
                        }

                        dynamicButton = CreatePageButton(number, currentPage);
                    }

                }

                ///////////////////////////////////////////////////////////
                //if current page >= 12...15(in the rigth)
                ///////////////////////////////////////////////////////////
                else if (currentPage >= countButtons - 3)
                {
                    //set(...) (is almost in the left)
                    if (i == 3)
                    {
                        dynamicButton = CreateElipsisButton();
                    }
                    //add button
                    else
                    {
                        //button's numerable(1 2 3 ... (lastPage - 12) - lastPage)
                        int number = (i < 3) ? (i + 1) : (i + lastPage - countButtons + 1);
                        dynamicButton = CreatePageButton(number, currentPage);
                    }
                }

                dynamicButton.Width = 35;
                dynamicButton.Margin = new Thickness(5, 5, 0, 0);
                dynamicButton.HorizontalAlignment = HorizontalAlignment.Left;
                dynamicButton.VerticalAlignment = VerticalAlignment.Stretch;

                sp.Children.Add(dynamicButton);
            }
        }
        public Button CreateElipsisButton()
        {
            Button btn = new Button();
            btn.Content = "";
            btn.IsEnabled = false;

            return btn;
        }
        public Button CreatePageButton(int number, int currentPage)
        {
            Brush selectColor = Brushes.DarkCyan;
            Brush defaultColor = Brushes.White;

            Button btn = new Button();
            btn.Content = number.ToString();
            btn.Background = (number == currentPage) ? selectColor : defaultColor;
            btn.Click += DynamicButton_Click;

            return btn;
        }
        private void DynamicButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int page = int.Parse(btn.Content.ToString());
            ShowPage(page);
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
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = lbxProducts.SelectedItem as ProductVM;
            if (selectedItem != null)
            {
                int id = selectedItem.Id;
                EditProductWindow dlg = new EditProductWindow();
                dlg.IdProduct = id;
                dlg.ShowDialog();

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
