using GalleryApp.Data;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Word = Microsoft.Office.Interop.Word;
using System.Runtime.InteropServices;

namespace GalleryApp.Pages
{
    public partial class ViewOrdersPage : Page
    {
        public ViewOrdersPage()
        {
            InitializeComponent();
            InitializePage();
        }

        private void InitializePage()
        {
            try
            {
                LoadOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке заказов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadOrders()
        {
            var context = gallerydatabaseEntities.GetContext();

            var orders = context.Order
                .Join(context.Lamp,
                    o => o.Id,
                    a => a.Id,
                    (o, a) => new { o, a })
                .Join(context.Users,
                    orderLamp => orderLamp.o.IdUser,
                    u => u.Id,
                    (orderLamp, u) => new
                    {
                        orderLamp.o.Id,
                        orderLamp.o.Adress,
                        orderLamp.o.Comment,
                        ArtTitle = orderLamp.a.ModelName,
                        ArtAuthor = orderLamp.a.Manufacturer,
                        UserName = u.FirstName + " " + u.LastName,
                        OrderEntity = orderLamp.o
                    })
                .ToList();

            dataGridOrders.ItemsSource = orders;

            CountLabel.Content = $"Всего заказов: {orders.Count}";
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        private void GenerateDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedItem = button?.CommandParameter as Order;

            if (selectedItem == null)
            {
                MessageBox.Show("Выберите заказ для генерации накладной.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int orderId = selectedItem.Id;

            try
            {
                Classes.WordDocumentGenerator.CreateInvoice(orderId);
                MessageBox.Show("Накладная успешно сгенерирована.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании накладной: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateAllInvoicesButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrders = dataGridOrders.ItemsSource.Cast<dynamic>().ToList();
            if (selectedOrders == null || !selectedOrders.Any())
            {
                MessageBox.Show("Нет заказов для генерации накладных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                foreach (var selectedItem in selectedOrders)
                {
                    int orderId = selectedItem.OrderEntity.Id;
                    Classes.WordDocumentGenerator.CreateInvoice(orderId);
                }

                MessageBox.Show("Накладные успешно сгенерированы для всех заказов.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании накладных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
