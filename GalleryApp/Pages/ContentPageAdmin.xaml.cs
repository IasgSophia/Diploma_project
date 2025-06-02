using GalleryApp.Classes;
using GalleryApp.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GalleryApp.Pages
{
    public partial class ContentPageAdmin : Page
    {
        private byte[] _defaultImage;

        public ContentPageAdmin()
        {
            InitializeComponent();
            InitializeComboBoxes();
            LoadDefaultImage();
            InitializePage();
        }

        private void LoadDefaultImage()
        {
            try
            {
                var uri = new Uri("pack://application:,,,/Resources/default_lamp.png", UriKind.Absolute);
                var resourceStream = Application.GetResourceStream(uri);
                if (resourceStream != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        resourceStream.Stream.CopyTo(memoryStream);
                        _defaultImage = memoryStream.ToArray();
                    }
                }
            }
            catch
            {
                _defaultImage = null;
            }
        }

        private List<Lamp> SortLamps(List<Lamp> lamps)
        {
            if (SortUpRadioButton.IsChecked == true)
                return lamps.OrderBy(l => l.Price).ToList();

            if (SortDownRadioButton.IsChecked == true)
                return lamps.OrderByDescending(l => l.Price).ToList();

            return lamps;
        }

        private void InitializeComboBoxes()
        {
            var context = gallerydatabaseEntities.GetContext();

            // Подгружаем список из БД
            var lampTypes = context.LampType.OrderBy(x => x.Name).ToList();
            var mountingTypes = context.MountingType.OrderBy(x => x.Name).ToList();
            var manufacturers = context.Manufacturer.OrderBy(x => x.Name).ToList();

            // Инициализируем ComboBox-ы
            InitializeComboBox(LampTypeComboBox, lampTypes, "Все типы ламп");
            InitializeComboBox(MountingTypeComboBox, mountingTypes, "Все типы креплений");
            InitializeComboBox(ManufacturerComboBox, manufacturers, "Все производители");
        }

        private void InitializeComboBox<T>(ComboBox comboBox, List<T> items, string defaultName) where T : class, new()
        {
            var idProp = typeof(T).GetProperty("Id");
            var nameProp = typeof(T).GetProperty("Name");

            if (idProp == null || nameProp == null)
            {
                throw new InvalidOperationException($"Класс {typeof(T).Name} должен содержать свойства Id и Name.");
            }

            // Создаём элемент "Все ..."
            var defaultItem = new T();
            idProp.SetValue(defaultItem, 0); // Id = 0 — значит "не выбран фильтр"
            nameProp.SetValue(defaultItem, defaultName);

            // Добавляем "все" и остальные
            var itemList = new List<T> { defaultItem };
            itemList.AddRange(items);

            comboBox.ItemsSource = itemList;
            comboBox.DisplayMemberPath = "Name";
            comboBox.SelectedValuePath = "Id";
            comboBox.SelectedValue = 0; // по умолчанию — "все"
        }



        private void InitializePage()
        {
            try
            {
                var currentUser = Manager.CurrentUser;
                if (currentUser != null)
                {
                    FIOLabel.Visibility = Visibility.Visible;
                    FIOLabel.Content = $"{currentUser.LastName} {currentUser.FirstName} {currentUser.MiddleName}";
                }
                else
                {
                    FIOLabel.Visibility = Visibility.Hidden;
                    FIOLabel.Content = "ФИО не найдено";
                }

                var context = gallerydatabaseEntities.GetContext();

                var lamps = context.Lamp.ToList();

                SetDefaultImages(lamps);
                ProductListView.ItemsSource = lamps;

                // ComboBox уже инициализированы в InitializeComboBoxes, просто обновим метки количества
                CountOfLabel.Content = $"{lamps.Count}/{context.Lamp.Count()}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SetDefaultImages(List<Lamp> lamps)
        {
            foreach (var lamp in lamps)
            {
                if (lamp.ProductPhoto == null || lamp.ProductPhoto.Length == 0)
                    lamp.ProductPhoto = _defaultImage;
            }
        }

        public void Update()
        {
            try
            {
                var context = gallerydatabaseEntities.GetContext();
                var lamps = context.Lamp.ToList();
                SetDefaultImages(lamps);

                FilterLamps(ref lamps);
                lamps = SortLamps(lamps);

                CountOfLabel.Content = $"{lamps.Count}/{context.Lamp.Count()}";
                ProductListView.ItemsSource = lamps;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FilterLamps(ref List<Lamp> lamps)
        {
            string search = SearchTextBox.Text?.ToLower() ?? "";

            if (!string.IsNullOrWhiteSpace(search))
            {
                lamps = lamps.Where(l =>
                    (!string.IsNullOrEmpty(l.ModelName) && l.ModelName.
                    ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(l.Description) && l.Description.
                    ToLower().Contains(search))
                ).ToList();
            }
            if (LampTypeComboBox.SelectedValue is int lampTypeId && lampTypeId != 0)
                lamps = lamps.Where(l => l.LampTypeId == lampTypeId).ToList();


            if (MountingTypeComboBox.SelectedValue is int mountingTypeId && mountingTypeId != 0)
                lamps = lamps.Where(l => l.MountingTypeId == mountingTypeId).ToList();

            if (ManufacturerComboBox.SelectedValue is int manufacturerId && manufacturerId != 0)
                lamps = lamps.Where(l => l.ManufacturerTypeId == manufacturerId).ToList();

            if (UVCheckBox?.IsChecked == true)
                lamps = lamps.Where(l => l.HasUVProtection).ToList();

            if (IRCheckBox?.IsChecked == true)
                lamps = lamps.Where(l => l.HasIRProtection).ToList();

            if (DimmableCheckBox?.IsChecked == true)
                lamps = lamps.Where(l => l.Dimmable).ToList();
           
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => Update();
        private void SortUpRadioButton_Checked(object sender, RoutedEventArgs e) => Update();
        private void SortDownRadioButton_Checked(object sender, RoutedEventArgs e) => Update();
        private void LampTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => Update();
        private void MountingTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => Update();
        private void ManufacturerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => Update();

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedLamp = (sender as Button)?.DataContext as Lamp;
            if (selectedLamp != null)
                Manager.MainFrame.Navigate(new Pages.AddEditProductPage(selectedLamp));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedLamp = (sender as Button)?.DataContext as Lamp;
                if (selectedLamp != null)
                {
                    var result = MessageBox.Show("Вы уверены, что хотите удалить эту лампу?",
                                                  "Подтверждение удаления",
                                                  MessageBoxButton.YesNo,
                                                  MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        DeleteLamp(selectedLamp);
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите лампу для удаления.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}");
            }
        }

        private void DeleteLamp(Lamp selectedLamp)
        {
            var context = gallerydatabaseEntities.GetContext();
            var lampToDelete = context.Lamp.FirstOrDefault(l => l.Id == selectedLamp.Id);
            if (lampToDelete != null)
            {
                context.Lamp.Remove(lampToDelete);
                context.SaveChanges();
                Update();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new Pages.AddEditProductPage(null));
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new Pages.AddEditAccountsPage());
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Manager.MainFrame.CanGoBack)
            {
                Manager.CurrentUser = null;
                Manager.MainFrame.GoBack();
            }
        }

        private void ViewOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new Pages.ViewOrdersPage());
        }
        private void UVCheckBox_Checked(object sender, RoutedEventArgs e) => Update();
        private void IRCheckBox_Checked(object sender, RoutedEventArgs e) => Update();
        private void DimmableCheckBox_Checked(object sender, RoutedEventArgs e) => Update();
        private void InStockCheckBox_Checked(object sender, RoutedEventArgs e) => Update();
        private void UVCheckBox_Unchecked(object sender, RoutedEventArgs e) => Update();
        private void IRCheckBox_Unchecked(object sender, RoutedEventArgs e) => Update();
        private void DimmableCheckBox_Unchecked(object sender, RoutedEventArgs e) => Update();
        private void InStockCheckBox_Unchecked(object sender, RoutedEventArgs e) => Update();
        private void LuminousFluxComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => Update();
        private void VoltageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => Update();

    }
}
