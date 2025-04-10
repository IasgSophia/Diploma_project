using GalleryApp.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace GalleryApp.Pages
{
    public partial class ContentPageAdmin : Page
    {
        private byte[] _defaultImage;

        public ContentPageAdmin()
        {
            InitializeComponent();
            LoadDefaultImage();
            Init();
        }

        private void LoadDefaultImage()
        {
            try
            {
                var uri = new Uri("pack://application:,,,/Resources/smallcrow.png", UriKind.Absolute);
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

        public List<Data.Art> _product = new List<Data.Art>();

        private void Init()
        {
            try
            {
                var products = Data.gallerydatabaseEntities.GetContext().Art.ToList();

                foreach (var art in products)
                {
                    if (art.ProductPhoto == null || art.ProductPhoto.Length == 0)
                        art.ProductPhoto = _defaultImage;
                }

                ProductsListView.ItemsSource = products;
                _product = products;

                var sizeTypeList = Data.gallerydatabaseEntities.GetContext().TypeSize.ToList();
                sizeTypeList.Insert(0, new Data.TypeSize { Size = "Все размеры" });
                SizeTypeComboBox.ItemsSource = sizeTypeList;
                SizeTypeComboBox.SelectedIndex = 0;

                var exhibitionList = Data.gallerydatabaseEntities.GetContext().Exibition.ToList();
                exhibitionList.Insert(0, new Data.Exibition { Name = "Все выставки" });
                ExhibitionFilterComboBox.ItemsSource = exhibitionList;
                ExhibitionFilterComboBox.SelectedIndex = 0;

                Console.WriteLine($"Количество выставок: {exhibitionList.Count}");

                if (Manager.CurrentUser != null)
                {
                    FIOLabel.Visibility = Visibility.Visible;
                    FIOLabel.Content = $"{Manager.CurrentUser.LastName} {Manager.CurrentUser.FirstName} {Manager.CurrentUser.MiddleName}";
                    ProductsListView.Visibility = Visibility.Visible; 
                    Update(); 
                }
                else
                {
                    FIOLabel.Visibility = Visibility.Hidden;
                    ProductsListView.Visibility = Visibility.Hidden;
                    CountOfLabel.Content = "0/0"; 
                }

                CountOfLabel.Content = $"{products.Count}/{products.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }




        public void Update()
{
    try
    {
        var context = Data.gallerydatabaseEntities.GetContext();

        var products = context.Art.ToList();

        if (!string.IsNullOrEmpty(SearchTextBox.Text))
        {
            string search = SearchTextBox.Text.ToLower();
            products = products.Where(item =>
                item.title.ToLower().Contains(search) ||
                item.author.ToLower().Contains(search) ||
                item.genre.ToLower().Contains(search) ||
                context.Exibition
                    .Where(e => e.Id == item.idExibition)
                    .Select(e => e.Name)
                    .FirstOrDefault()
                    .ToLower()
                    .Contains(search)).ToList();
        }

        var selectedExhibition = ExhibitionFilterComboBox.SelectedItem as Data.Exibition;

        if (selectedExhibition != null)
        {
            if (selectedExhibition.Name == "Все выставки")
            {
                products = context.Art.ToList();
            }
            else
            {
                products = products.Where(p => p.idExibition == selectedExhibition.Id).ToList();
            }
        }

        var selectedSizeType = SizeTypeComboBox.SelectedItem as Data.TypeSize;
        if (selectedSizeType != null && selectedSizeType.Size != "Все размеры")
        {
            products = products.Where(d => d.idTypeSize == selectedSizeType.Id).ToList();
        }

        if (SortUpRadioButton.IsChecked == true)
        {
            products = products.OrderBy(d => d.price).ToList();
        }
        else if (SortDownRadioButton.IsChecked == true)
        {
            products = products.OrderByDescending(d => d.price).ToList();
        }

        CountOfLabel.Content = $"{products.Count}/{context.Art.Count()}";

        ProductsListView.ItemsSource = products;
        ProductsListView.Items.Refresh(); 

    }
    catch (Exception ex)
    {
        MessageBox.Show(ex.Message);
    }
}



        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Update();
        }

        private void SortUpRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void SortDownRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Update();
        }   

        private void SizeTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }

        private void ExhibitionFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedArt = (sender as Button)?.DataContext as Data.Art;
            if (selectedArt != null)
                Classes.Manager.MainFrame.Navigate(new Pages.AddEditProductPage(selectedArt));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedArt = (sender as Button)?.DataContext as Data.Art;
                if (selectedArt != null)
                {
                    var result = MessageBox.Show("Вы уверены, что хотите удалить это произведение?",
                                                  "Подтверждение удаления",
                                                  MessageBoxButton.YesNo,
                                                  MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        var context = Data.gallerydatabaseEntities.GetContext();
                        var artToDelete = context.Art.FirstOrDefault(a => a.id == selectedArt.id);
                        if (artToDelete != null)
                        {
                            context.Art.Remove(artToDelete);
                            context.SaveChanges();

                            Update();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите произведение для удаления.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}");
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Classes.Manager.MainFrame.Navigate(new Pages.AddEditProductPage(null));
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            Classes.Manager.MainFrame.Navigate(new Pages.AddEditAccountsPage());
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Classes.Manager.MainFrame.CanGoBack)
            {
                if (Classes.Manager.CurrentUser != null)
                {
                    Classes.Manager.CurrentUser = null;
                }
                Classes.Manager.MainFrame.GoBack();
            }
        }
    }
}
