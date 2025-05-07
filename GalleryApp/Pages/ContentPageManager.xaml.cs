    using GalleryApp.Classes;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    namespace GalleryApp.Pages
    {
        public partial class ContentPageManager : Page
        {
            private byte[] _defaultImage;

            public ContentPageManager()
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
                    var products = Data.gallerydatabaseEntities.GetContext().Art.ToList();
                    SetDefaultImages(products);

                    ProductsListView.ItemsSource = products;

                    var sizeTypes = Data.gallerydatabaseEntities.GetContext().TypeSize.ToList();
                    sizeTypes.Insert(0, new Data.TypeSize { Size = "Все размеры" });
                    SizeTypeComboBox.ItemsSource = sizeTypes;
                    SizeTypeComboBox.SelectedIndex = 0;

                    var exhibitions = Data.gallerydatabaseEntities.GetContext().Exibition.ToList();
                    exhibitions.Insert(0, new Data.Exibition { Name = "Все выставки" });
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

            private void EditButton_Click(object sender, RoutedEventArgs e)
            {
                var selectedArt = (sender as Button)?.DataContext as Data.Art;
                if (selectedArt != null)
                    Manager.MainFrame.Navigate(new Pages.AddEditProductPage(selectedArt));
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
                            DeleteArt(selectedArt);
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

            private void DeleteArt(Data.Art selectedArt)
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

            private void AddButton_Click(object sender, RoutedEventArgs e)
            {
                Manager.MainFrame.Navigate(new Pages.AddEditProductPage(null));
            }              

            private void BackButton_Click(object sender, RoutedEventArgs e)
            {
                if (Manager.MainFrame.CanGoBack)
                {
                    if (Manager.CurrentUser != null)
                    {
                        Manager.CurrentUser = null;
                    }
                    Manager.MainFrame.GoBack();
                }
            }


            private void ViewOrdersButton_Click(object sender, RoutedEventArgs e)
            {
                Manager.MainFrame.Navigate(new Pages.ViewOrdersPage());
            }

        }
    }
