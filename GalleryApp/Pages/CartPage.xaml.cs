using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GalleryApp.Classes;
using GalleryApp.Data;

namespace GalleryApp.Pages
{
    public partial class CartPage : Page
    {
        public ObservableCollection<Art> CartItems { get; set; }

        public CartPage()
        {
            InitializeComponent();
            CartItems = new ObservableCollection<Art>(Manager.GetCartForCurrentUser());
            this.DataContext = this;
            LoadShippingTypes();
            DisplayUserInfo();
        }

        private void DisplayUserInfo()
        {
            if (Manager.CurrentUser != null)
            {
                UserInfoLabel.Visibility = Visibility.Visible;
                UserInfoLabel.Content = $"{Manager.CurrentUser.LastName} {Manager.CurrentUser.FirstName} {Manager.CurrentUser.MiddleName}";
            }
            else
            {
                UserInfoLabel.Visibility = Visibility.Hidden;
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                var item = button.CommandParameter as Art;
                if (item != null)
                {
                    Manager.RemoveFromCart(item);
                    CartItems.Remove(item);
                }
            }
        }

        private void SubmitOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (CartItems == null || CartItems.Count == 0)
            {
                MessageBox.Show("Корзина пуста.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Manager.CurrentUser == null)
            {
                MessageBox.Show("Пользователь не авторизован.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (ShippingTypeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите тип доставки.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(CommentTextBox.Text) || string.IsNullOrWhiteSpace(AddressTextBox.Text))
            {
                MessageBox.Show("Заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var context = gallerydatabaseEntities.GetContext();

                foreach (var art in CartItems)
                {
                    var selectedShippingType = ShippingTypeComboBox.SelectedItem as ShippingType;

                    var newOrder = new Order
                    {
                        IdUser = Manager.CurrentUser.Id,
                        Comment = CommentTextBox.Text,
                        Adress = AddressTextBox.Text,
                        IdShippingType = selectedShippingType.Id
                    };

                    context.Order.Add(newOrder);
                }

                context.SaveChanges();
                MessageBox.Show("Заказ оформлен успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                Manager.CartItems.Clear();
                CartItems.Clear();
                CommentTextBox.Text = string.Empty;
                AddressTextBox.Text = string.Empty;
                ShippingTypeComboBox.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при оформлении заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadShippingTypes()
        {
            var context = gallerydatabaseEntities.GetContext();
            ShippingTypeComboBox.ItemsSource = context.ShippingType.ToList();
            ShippingTypeComboBox.SelectedIndex = 0;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}
