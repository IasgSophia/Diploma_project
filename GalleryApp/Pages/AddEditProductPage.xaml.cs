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
        private string selectedImageFileName;

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
        }

        private void FillFields()
        {
            IdTextBox.Text = currentLamp.Id.ToString();
            ModelNameTextBox.Text = currentLamp.ModelName;
            ManufacturerComboBox.SelectedValue = currentLamp.ManufacturerTypeId;
            LampTypeComboBox.SelectedValue = currentLamp.LampTypeId;
            MountingTypeComboBox.SelectedValue = currentLamp.MountingTypeId;
            PowerTextBox.Text = currentLamp.PowerWatts.ToString();
            ColorTempTextBox.Text = currentLamp.ColorTemperature.ToString();
            CRITextBox.Text = currentLamp.CRI.ToString();
            LumenTextBox.Text = currentLamp.LuminousFlux.ToString();
            BeamAngleTextBox.Text = currentLamp.BeamAngle.ToString();
            VoltageTextBox.Text = currentLamp.Voltage.ToString();
            PriceTextBox.Text = currentLamp.Price.ToString("F2");
            QuantityTextBox.Text = currentLamp.StockQuantity.ToString();
            DescriptionTextBox.Text = currentLamp.Description;
            UVCheckBox.IsChecked = currentLamp.HasUVProtection;
            IRCheckBox.IsChecked = currentLamp.HasIRProtection;
            DimmableCheckBox.IsChecked = currentLamp.Dimmable;

            if (!string.IsNullOrEmpty(currentLamp.PhotoName))
            {
                string path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "img", currentLamp.PhotoName);
                if (File.Exists(path))
                {
                    BitmapImage bitmap = new BitmapImage(new System.Uri(path));
                    PhotoImage.Source = bitmap;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var context = gallerydatabaseEntities.GetContext();

            if (string.IsNullOrWhiteSpace(ModelNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(ManufacturerComboBox.Text))
            {
                MessageBox.Show("Заполните все обязательные поля.");
                return;
            }

            if (!float.TryParse(PowerTextBox.Text, out float power) ||
                !int.TryParse(ColorTempTextBox.Text, out int colorTemp) ||
                !float.TryParse(CRITextBox.Text, out float cri) ||
                !int.TryParse(LumenTextBox.Text, out int lumen) ||
                !int.TryParse(BeamAngleTextBox.Text, out int beam) ||
                !float.TryParse(VoltageTextBox.Text, out float voltage) ||
                !decimal.TryParse(PriceTextBox.Text, out decimal price) ||
                !int.TryParse(QuantityTextBox.Text, out int quantity))
            {
                MessageBox.Show("Некорректные числовые значения.");
                return;
            }

            currentLamp.ModelName = ModelNameTextBox.Text;
            currentLamp.ManufacturerTypeId = (int)(ManufacturerComboBox.SelectedValue ?? 0);
            currentLamp.LampTypeId = (int)(LampTypeComboBox.SelectedValue ?? 0);
            currentLamp.MountingTypeId = (int)(MountingTypeComboBox.SelectedValue ?? 0);
            currentLamp.PowerWatts = power;
            currentLamp.ColorTemperature = colorTemp;
            currentLamp.CRI = cri;
            currentLamp.LuminousFlux = lumen;
            currentLamp.BeamAngle = beam;
            currentLamp.Voltage = voltage;
            currentLamp.Price = price;
            currentLamp.StockQuantity = quantity;
            currentLamp.Description = DescriptionTextBox.Text;
            currentLamp.HasUVProtection = UVCheckBox.IsChecked == true;
            currentLamp.HasIRProtection = IRCheckBox.IsChecked == true;
            currentLamp.Dimmable = DimmableCheckBox.IsChecked == true;

            if (!string.IsNullOrEmpty(selectedImageFileName))
            {
                string fileName = System.IO.Path.GetFileName(selectedImageFileName);
                currentLamp.PhotoName = fileName;

                string destination = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "img", fileName);
                File.Copy(selectedImageFileName, destination, true);
            }

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
                selectedImageFileName = dlg.FileName;
                BitmapImage bitmap = new BitmapImage(new System.Uri(selectedImageFileName));
                PhotoImage.Source = bitmap;
            }
        }
    }
}
