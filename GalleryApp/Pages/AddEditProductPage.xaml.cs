using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace GalleryApp.Pages
{
    public partial class AddEditProductPage : Page
    {
        private Data.Art CurrentArt;
        private bool IsEditMode = false;
        private bool FlagPhoto = false;

        public AddEditProductPage(Data.Art selectedArt = null)
        {
            InitializeComponent();

            if (selectedArt != null)
            {
                CurrentArt = selectedArt;
                IsEditMode = true;
            }
            else
            {
                CurrentArt = new Data.Art();
                IsEditMode = false;
            }

            DataContext = CurrentArt;
            Init();
        }

        private void Init()
        {
            try
            {
                SizeTypeComboBox.ItemsSource = Data.gallerydatabaseEntities.GetContext().TypeSize.ToList();
                SizeTypeComboBox.DisplayMemberPath = "Size";

                ExhibitionComboBox.ItemsSource = Data.gallerydatabaseEntities.GetContext().Exibition.ToList();

                if (!IsEditMode)
                {
                    IdTextBox.Text = (Data.gallerydatabaseEntities.GetContext().Art.Max(d => (int?)d.id) + 1 ?? 1).ToString();

                    TitleTextBox.Text = "";
                    AuthorTextBox.Text = "";
                    GenreTextBox.Text = "";
                    SizeTextBox.Text = "";
                    SizeTypeComboBox.SelectedItem = null;
                    PriceTextBox.Text = "";
                    ExhibitionComboBox.SelectedItem = null;  
                    DescriptionTextBox.Text = "";

                    FlagPhoto = false;
                    PhotoImage.Source = new BitmapImage(new Uri("pack://application:,,,/GalleryApp;component/Resources/smallcrow.png"));

                    IdLabel.Visibility = Visibility.Hidden;
                }
                else
                {
                    IdTextBox.Visibility = Visibility.Visible;
                    IdLabel.Visibility = Visibility.Visible;

                    IdTextBox.Text = CurrentArt.id.ToString();
                    TitleTextBox.Text = CurrentArt.title;
                    AuthorTextBox.Text = CurrentArt.author;
                    GenreTextBox.Text = CurrentArt.genre;
                    SizeTextBox.Text = CurrentArt.size.ToString();
                    SizeTypeComboBox.SelectedItem = Data.gallerydatabaseEntities.GetContext().TypeSize
                        .FirstOrDefault(d => d.Id == CurrentArt.idTypeSize);
                    PriceTextBox.Text = CurrentArt.price.ToString("F2");

                    var exhibition = Data.gallerydatabaseEntities.GetContext().Exibition
                        .FirstOrDefault(e => e.Id == CurrentArt.idExibition);
                    ExhibitionComboBox.SelectedItem = exhibition;

                    DescriptionTextBox.Text = CurrentArt.Decription;

                    if (CurrentArt.ProductPhoto != null && CurrentArt.ProductPhoto.Length > 0)
                    {
                        using (var ms = new MemoryStream(CurrentArt.ProductPhoto))
                        {
                            BitmapImage bitmapImage = new BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.StreamSource = ms;
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImage.EndInit();

                            PhotoImage.Source = bitmapImage;
                            FlagPhoto = true;
                        }
                    }
                    else
                    {
                        PhotoImage.Source = new BitmapImage(new Uri("pack://application:,,,/GalleryApp;component/Resources/smallcrow.png"));
                        FlagPhoto = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder errors = new StringBuilder();

                if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
                    errors.AppendLine("Заполните название");

                if (string.IsNullOrWhiteSpace(AuthorTextBox.Text))
                    errors.AppendLine("Заполните автора");

                if (string.IsNullOrWhiteSpace(GenreTextBox.Text))
                    errors.AppendLine("Заполните жанр");

                if (SizeTypeComboBox.SelectedItem == null)
                    errors.AppendLine("Выберите тип размера");

                if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price < 0)
                    errors.AppendLine("Цена должна быть положительным числом");

                if (ExhibitionComboBox.SelectedItem == null)
                    errors.AppendLine("Выберите выставку");

                if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
                    errors.AppendLine("Заполните описание");

                if (!FlagPhoto)
                    errors.AppendLine("Выберите изображение");

                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                CurrentArt.title = TitleTextBox.Text;
                CurrentArt.author = AuthorTextBox.Text;
                CurrentArt.genre = GenreTextBox.Text;
                CurrentArt.size = SizeTextBox.Text;
                CurrentArt.price = price;
                CurrentArt.Decription = DescriptionTextBox.Text;

                var selectedSize = (Data.TypeSize)SizeTypeComboBox.SelectedItem;
                CurrentArt.idTypeSize = selectedSize.Id;

                var selectedExhibition = (Data.Exibition)ExhibitionComboBox.SelectedItem;
                if (selectedExhibition != null)
                {
                    CurrentArt.idExibition = selectedExhibition.Id;
                }

                if (!IsEditMode)
                {
                    Data.gallerydatabaseEntities.GetContext().Art.Add(CurrentArt);
                }
                else
                {
                    var existingArt = Data.gallerydatabaseEntities.GetContext().Art
                        .FirstOrDefault(a => a.id == CurrentArt.id);
                    if (existingArt != null)
                    {
                        existingArt.title = CurrentArt.title;
                        existingArt.author = CurrentArt.author;
                        existingArt.genre = CurrentArt.genre;
                        existingArt.size = CurrentArt.size;
                        existingArt.price = CurrentArt.price;
                        existingArt.Decription = CurrentArt.Decription;
                        existingArt.idTypeSize = CurrentArt.idTypeSize;
                        existingArt.idExibition = CurrentArt.idExibition;
                        existingArt.PhotoName = CurrentArt.PhotoName;
                        existingArt.ProductPhoto = CurrentArt.ProductPhoto;
                    }
                }

                Data.gallerydatabaseEntities.GetContext().SaveChanges();
                MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void PhotoImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Изображения (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg"
                };

                if (openFileDialog.ShowDialog() == true && File.Exists(openFileDialog.FileName))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(openFileDialog.FileName, UriKind.Absolute);
                    bitmapImage.EndInit();

                    CurrentArt.PhotoName = openFileDialog.FileName;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        PngBitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                        encoder.Save(ms);

                        CurrentArt.ProductPhoto = ms.ToArray();
                    }

                    PhotoImage.Source = bitmapImage;
                    FlagPhoto = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}