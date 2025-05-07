using GalleryApp.Classes;
using GalleryApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static GalleryApp.Classes.Manager;

namespace GalleryApp.Pages
{
    public partial class ContentPageUser : Page
    {
        private byte[] _defaultImage;

        public ContentPageUser()
        {
            InitializeComponent();
            LoadDefaultImage();
            InitializePage();
        }

        private void LoadDefaultImage()
        {
            try
            {
                var uri = new Uri("pack://application:,,,/Resources/smallcrow.png", UriKind.Absolute);
                var resourceStream = Application.GetResourceStream(uri);
                if (resourceStream != null)
                {
                    using (var memoryStream = new System.IO.MemoryStream())
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

        private List<Data.Art> SortProducts(List<Data.Art> products)
        {
            if (SortUpRadioButton.IsChecked == true)
                return products.OrderBy(d => d.price).ToList();

            if (SortDownRadioButton.IsChecked == true)
                return products.OrderByDescending(d => d.price).ToList();

            return products;
        }

        private void InitializePage()
        {
            try
            {
                var currentUser = Manager.CurrentUser;
                if (currentUser != null)
                {
                    FIOLabel.Visibility = Visibility.Visible;
                    FIOLabel.Content = $"{Manager.CurrentUser.LastName} {Manager.CurrentUser.FirstName} {Manager.CurrentUser.MiddleName}";

                }
                else
                {
                    FIOLabel.Visibility = Visibility.Hidden;
                    FIOLabel.Content = "ФИО не найдено";
                }
                var products = gallerydatabaseEntities.GetContext().Art.ToList();
                SetDefaultImages(products);

                ProductsListView.ItemsSource = products;

                var sizeTypes = gallerydatabaseEntities.GetContext().TypeSize.ToList();
                sizeTypes.Insert(0, new TypeSize { Size = "Все размеры" });
                SizeTypeComboBox.ItemsSource = sizeTypes;
                SizeTypeComboBox.SelectedIndex = 0;

                var exhibitions = gallerydatabaseEntities.GetContext().Exibition.ToList();
                exhibitions.Insert(0, new Exibition { Name = "Все выставки" });
                ExhibitionFilterComboBox.ItemsSource = exhibitions;
                ExhibitionFilterComboBox.SelectedIndex = 0;
               

                CountOfLabel.Content = $"{products.Count}/{products.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SetDefaultImages(List<Data.Art> products)
        {
            foreach (var art in products)
            {
                if (art.ProductPhoto == null || art.ProductPhoto.Length == 0)
                    art.ProductPhoto = _defaultImage;
            }
        }

        public void Update()
        {
            try
            {
                var products = Data.gallerydatabaseEntities.GetContext().Art.ToList();
                SetDefaultImages(products);

                FilterProducts(ref products);
                products = SortProducts(products);

                CountOfLabel.Content = $"{products.Count}/{Data.gallerydatabaseEntities.GetContext().Art.Count()}";
                ProductsListView.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FilterProducts(ref List<Data.Art> products)
        {
            var search = SearchTextBox.Text.ToLower();
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(item =>
                    item.title.ToLower().Contains(search) ||
                    item.author.ToLower().Contains(search) ||
                    item.genre.ToLower().Contains(search) ||
                    Data.gallerydatabaseEntities.GetContext().Exibition
                        .Where(e => e.Id == item.idExibition)
                        .Select(e => e.Name)
                        .FirstOrDefault()
                        .ToLower()
                        .Contains(search)).ToList();
            }

            var selectedSizeType = SizeTypeComboBox.SelectedItem as Data.TypeSize;
            if (selectedSizeType != null && selectedSizeType.Size != "Все размеры")
            {
                products = products.Where(d => d.idTypeSize == selectedSizeType.Id).ToList();
            }

            var selectedExhibition = ExhibitionFilterComboBox.SelectedItem as Data.Exibition;
            if (selectedExhibition != null && selectedExhibition.Name != "Все выставки")
            {
                products = products.Where(p => p.idExibition == selectedExhibition.Id).ToList();
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => Update();
        private void SortUpRadioButton_Checked(object sender, RoutedEventArgs e) => Update();
        private void SortDownRadioButton_Checked(object sender, RoutedEventArgs e) => Update();
        private void SizeTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => Update();
        private void ExhibitionFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => Update();

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedArt = (sender as Button)?.DataContext as Data.Art;
            if (selectedArt != null)
            {
                Manager.CartItems.Add(selectedArt);
                MessageBox.Show("Товар добавлен в корзину!");
            }
        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new CartPage());
        }


















        
    }
}
