using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using GalleryApp.Data;
using System.IO;
using System.Linq;

namespace GalleryApp.Pages
{
    public partial class AddEditProductPage : Page
    {
        private Lamp currentLamp;
        private byte[] imageBytes;

        public AddEditProductPage(Lamp lamp = null)
        {
            InitializeComponent();
            LoadComboBoxes();

            if (lamp == null)
            {
                currentLamp = new Lamp();
                IdTextBox.Text = "Новый";
            }
            else
            {
                currentLamp = lamp;
                FillFields();
            }
        }

        private void LoadComboBoxes()
        {
            var context = gallerydatabaseEntities.GetContext();

            LampTypeComboBox.ItemsSource = context.LampType.ToList();
            MountingTypeComboBox.ItemsSource = context.MountingType.ToList();
            SizeTypeComboBox.ItemsSource = context.TypeSize.ToList(); // исправлено
        }

        private void FillFields()
        {
            IdTextBox.Text = currentLamp.Id.ToString();
            TitleTextBox.Text = currentLamp.Title;
            LampTypeComboBox.SelectedValue = currentLamp.IdLampType;
            MountingTypeComboBox.SelectedValue = currentLamp.IdMountingType;
            SizeTextBox.Text = currentLamp.Size;
            SizeTypeComboBox.SelectedValue = currentLamp.IdSizeType;
            PriceTextBox.Text = currentLamp.Price.ToString("F2");
            DescriptionTextBox.Text = currentLamp.Description;

            if (currentLamp.Photo != null && currentLamp.Photo.Length > 0)
            {
                using (var ms = new MemoryStream(currentLamp.Photo))
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                    PhotoImage.Source = image;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Введите название лампы.");
                return;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Введите корректную цену.");
                return;
            }

            currentLamp.Title = TitleTextBox.Text;
            currentLamp.IdLampType = (int)(LampTypeComboBox.SelectedValue ?? 0);
            currentLamp.IdMountingType = (int)(MountingTypeComboBox.SelectedValue ?? 0);
            currentLamp.Size = SizeTextBox.Text;
            currentLamp.IdSizeType = (int)(SizeTypeComboBox.SelectedValue ?? 0);
            currentLamp.Price = price;
            currentLamp.Description = DescriptionTextBox.Text;

            if (imageBytes != null)
                currentLamp.Photo = imageBytes;

            var context = gallerydatabaseEntities.GetContext();

            if (currentLamp.Id == 0)
                context.Lamp.Add(currentLamp);

            try
            {
                context.SaveChanges();
                MessageBox.Show("Сохранено успешно");
                NavigationService.GoBack();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        private void PhotoImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Изображения (*.jpg;*.png)|*.jpg;*.png";

            if (dlg.ShowDialog() == true)
            {
                imageBytes = File.ReadAllBytes(dlg.FileName);
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new System.Uri(dlg.FileName);
                bitmap.EndInit();
                PhotoImage.Source = bitmap;
            }
        }
    }
}
