using GalleryApp.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace GalleryApp.Pages
{
    public partial class ContentPageAdmin : Page
    {
        private byte[] _defaultImage;

        public ContentPageAdmin()
        {
            InitializeComponent();
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

        private List<Data.Lamp> SortLamps(List<Data.Lamp> lamps)
        {
            if (SortUpRadioButton.IsChecked == true)
                return lamps.OrderBy(l => l.Price).ToList();

            if (SortDownRadioButton.IsChecked == true)
                return lamps.OrderByDescending(l => l.Price).ToList();

            return lamps;
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

                var lamps = Data.gallerydatabaseEntities.GetContext().Lamp.ToList();
                SetDefaultImages(lamps);
                ProductsListView.ItemsSource = lamps;

                var lampTypes = Data.gallerydatabaseEntities.GetContext().LampType.ToList();
                lampTypes.Insert(0, new Data.LampType { Name = "Все типы" });
                LampTypeComboBox.ItemsSource = lampTypes;
                LampTypeComboBox.SelectedIndex = 0;

                var mountingTypes = Data.gallerydatabaseEntities.GetContext().MountingType.ToList();
                mountingTypes.Insert(0, new Data.MountingType { Name = "Все крепления" });
                MountingTypeComboBox.ItemsSource = mountingTypes;
                MountingTypeComboBox.SelectedIndex = 0;
                CountOfLabel.Content = $"{lamps.Count}/{lamps.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SetDefaultImages(List<Data.Lamp> lamps)
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
                var lamps = Data.gallerydatabaseEntities.GetContext().Lamp.ToList();
                SetDefaultImages(lamps);

                FilterLamps(ref lamps);
                lamps = SortLamps(lamps);

                CountOfLabel.Content = $"{lamps.Count}/{Data.gallerydatabaseEntities.GetContext().Lamp.Count()}";
                ProductsListView.ItemsSource = lamps;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FilterLamps(ref List<Data.Lamp> lamps)
        {
            var search = SearchTextBox.Text.ToLower();
            if (!string.IsNullOrEmpty(search))
            {
                lamps = lamps.Where(l =>
                    (!string.IsNullOrEmpty(l.ModelName) && l.ModelName.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(l.Description) && l.Description.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(l.Manufacturer) && l.Manufacturer.ToLower().Contains(search))
                ).ToList();
            }

            var selectedLampType = LampTypeComboBox.SelectedItem as Data.LampType;
            if (selectedLampType != null && selectedLampType.Name != "Все типы")
            {
                lamps = lamps.Where(l => l.LampTypeId == selectedLampType.Id).ToList();
            }

            var selectedMountingType = MountingTypeComboBox.SelectedItem as Data.MountingType;
            if (selectedMountingType != null && selectedMountingType.Name != "Все крепления")
            {
                lamps = lamps.Where(l => l.MountingTypeId == selectedMountingType.Id).ToList();
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => Update();
        private void SortUpRadioButton_Checked(object sender, RoutedEventArgs e) => Update();
        private void SortDownRadioButton_Checked(object sender, RoutedEventArgs e) => Update();
        private void LampTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => Update();
        private void MountingTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => Update();
        private void ManufacturerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => Update();

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedLamp = (sender as Button)?.DataContext as Data.Lamp;
            if (selectedLamp != null)
                Manager.MainFrame.Navigate(new Pages.AddEditProductPage(selectedLamp));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedLamp = (sender as Button)?.DataContext as Data.Lamp;
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

        private void DeleteLamp(Data.Lamp selectedLamp)
        {
            var context = Data.gallerydatabaseEntities.GetContext();
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
                if (Manager.CurrentUser != null)
                    Manager.CurrentUser = null;

                Manager.MainFrame.GoBack();
            }
        }

        private void ViewOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new Pages.ViewOrdersPage());
        }
    }
}
