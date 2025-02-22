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

                if (!IsEditMode)
                {
                    IdTextBox.Text = (Data.gallerydatabaseEntities.GetContext().Art.Max(d => (int?)d.id) + 1 ?? 1).ToString();

                    TitleTextBox.Text = "";
                    AuthorTextBox.Text = "";
                    GenreTextBox.Text = "";
                    SizeTextBox.Text = "";
                    SizeTypeComboBox.SelectedItem = null;
                    PriceTextBox.Text = "";
                    ExibitionTextBox.Text = "";
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
                    ExibitionTextBox.Text = exhibition?.Name ?? "Неизвестно";

                    DescriptionTextBox.Text = CurrentArt.Decription;

                    if (!string.IsNullOrEmpty(CurrentArt.PhotoName))
                    {
                        PhotoImage.Source = new BitmapImage(new Uri(CurrentArt.PhotoName, UriKind.RelativeOrAbsolute));
                        FlagPhoto = true;
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
                Classes.Manager.MainFrame.GoBack();
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

                if (!int.TryParse(SizeTextBox.Text, out int size) || size <= 0)
                    errors.AppendLine("Размер должен быть положительным числом");

                if (SizeTypeComboBox.SelectedItem == null)
                    errors.AppendLine("Выберите тип размера");

                if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price < 0)
                    errors.AppendLine("Цена должна быть положительным числом");

                if (string.IsNullOrWhiteSpace(ExibitionTextBox.Text))
                    errors.AppendLine("Заполните выставку");

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
                CurrentArt.size = size.ToString();
                CurrentArt.price = price;
                CurrentArt.Decription = DescriptionTextBox.Text;

                var selectedSize = (Data.TypeSize)SizeTypeComboBox.SelectedItem;
                CurrentArt.idTypeSize = selectedSize.Id;

                var exhibition = Data.gallerydatabaseEntities.GetContext().Exibition
                    .FirstOrDefault(ex => ex.Name == ExibitionTextBox.Text);
                if (exhibition != null)
                {
                    CurrentArt.idExibition = exhibition.Id;
                }

                if (!IsEditMode)
                {
                    Data.gallerydatabaseEntities.GetContext().Art.Add(CurrentArt);
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
