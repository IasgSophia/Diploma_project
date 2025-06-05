using GalleryApp.Data;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Word = Microsoft.Office.Interop.Word;

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

            var rawOrders = context.Order
                .Join(context.Lamp,
                    order => order.IdLamp,
                    lamp => lamp.Id,
                    (order, lamp) => new { order, lamp })
                .Join(context.Users,
                    combined => combined.order.IdUser,
                    user => user.Id,
                    (combined, user) => new { combined.order, combined.lamp, user })
                .Join(context.ShippingType,
                    combined => combined.order.IdShippingType,
                    shipping => shipping.Id,
                    (combined, shipping) => new { combined.order, combined.lamp, combined.user, shipping })
                .ToList(); 

            var orders = rawOrders.Select(item => new
            {
                FullName = $"{item.user.LastName} {GetInitial(item.user.FirstName)}{GetInitial(item.user.MiddleName)}",
                Comment = string.IsNullOrWhiteSpace(item.order.Comment) ? "Комментариев к заказу нет" : item.order.Comment,
                Address = item.order.Adress,
                ShippingTypeName = item.shipping.Name,
                LampModel = item.lamp.ModelName,
                OrderEntity = item.order
            }).ToList();

            dataGridOrders.ItemsSource = orders;
            CountLabel.Content = $"Всего заказов: {orders.Count}";
        }

        private string GetInitial(string name)
        {
            return string.IsNullOrWhiteSpace(name) ? "" : $" {name[0]}.";
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

        private void DeleteOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedOrder = button?.CommandParameter as Order;

            if (selectedOrder == null)
            {
                MessageBox.Show("Не удалось определить заказ для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить этот заказ?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                var context = gallerydatabaseEntities.GetContext();
                context.Order.Remove(selectedOrder);
                context.SaveChanges();
                MessageBox.Show("Заказ успешно удалён.", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
