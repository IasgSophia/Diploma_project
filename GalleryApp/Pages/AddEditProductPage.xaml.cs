using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalleryApp.Data;

namespace GalleryApp.Pages
{
    public partial class AddEditProductPage : Page
    {
        private Lamp CurrentLamp;
        private bool IsEditMode = false;
        private bool FlagPhoto = false;

        public AddEditProductPage(Lamp selectedLamp = null)
        {
            InitializeComponent();

            if (selectedLamp != null)
            {
                CurrentLamp = selectedLamp;
                IsEditMode = true;
            }
            else
            {
                CurrentLamp = new Lamp();
                IsEditMode = false;
            }

            DataContext = CurrentLamp;
            Init();
        }

        private void Init()
        {
            try
            {
                var context = gallerydatabaseEntities.GetContext();

                LampTypeComboBox.ItemsSource = context.LampType.ToList();
                MountingTypeComboBox.ItemsSource = context.MountingType.ToList();
                ManufacturerComboBox.ItemsSource = context.Manufacturer.ToList();

                if (!IsEditMode)
                {
                    ModelNameTextBox.Text = "";
                    ManufacturerComboBox.SelectedItem = null;
                    LampTypeComboBox.SelectedItem = null;
                    MountingTypeComboBox.SelectedItem = null;
                    PowerTextBox.Text = "";
                    ColorTempTextBox.Text = "";
                    CRITextBox.Text = "";
                    LumenTextBox.Text = "";
                    BeamAngleTextBox.Text = "";
                    VoltageTextBox.Text = "";
                    PriceTextBox.Text = "";
                    QuantityTextBox.Text = "";
                    DescriptionTextBox.Text = "";
                    UVCheckBox.IsChecked = false;
                    IRCheckBox.IsChecked = false;
                    DimmableCheckBox.IsChecked = false;

                    PhotoImage.Source = new BitmapImage(new Uri("pack://application:,,,/GalleryApp;component/Resources/smallcrow.png"));
                    FlagPhoto = false;
                }
                else
                {
                    ModelNameTextBox.Text = CurrentLamp.ModelName;
                    ManufacturerComboBox.SelectedValue = CurrentLamp.ManufacturerTypeId;
                    LampTypeComboBox.SelectedValue = CurrentLamp.LampTypeId;
                    MountingTypeComboBox.SelectedValue = CurrentLamp.MountingTypeId;
                    PowerTextBox.Text = CurrentLamp.PowerWatts.ToString();
                    ColorTempTextBox.Text = CurrentLamp.ColorTemperature.ToString();
                    CRITextBox.Text = CurrentLamp.CRI.ToString();
                    LumenTextBox.Text = CurrentLamp.LuminousFlux.ToString();
                    BeamAngleTextBox.Text = CurrentLamp.BeamAngle.ToString();
                    VoltageTextBox.Text = CurrentLamp.Voltage.ToString();
                    PriceTextBox.Text = CurrentLamp.Price.ToString("F2");
                    QuantityTextBox.Text = CurrentLamp.StockQuantity.ToString();
                    DescriptionTextBox.Text = CurrentLamp.Description;
                    UVCheckBox.IsChecked = CurrentLamp.HasUVProtection;
                    IRCheckBox.IsChecked = CurrentLamp.HasIRProtection;
                    DimmableCheckBox.IsChecked = CurrentLamp.Dimmable;

                    if (CurrentLamp.ProductPhoto != null && CurrentLamp.ProductPhoto.Length > 0)
                    {
                        using (var ms = new MemoryStream(CurrentLamp.ProductPhoto))
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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder errors = new StringBuilder();

                if (string.IsNullOrWhiteSpace(ModelNameTextBox.Text))
                    errors.AppendLine("Введите модель.");

                if (ManufacturerComboBox.SelectedItem == null)
                    errors.AppendLine("Выберите производителя.");

                if (LampTypeComboBox.SelectedItem == null)
                    errors.AppendLine("Выберите тип лампы.");

                if (MountingTypeComboBox.SelectedItem == null)
                    errors.AppendLine("Выберите тип монтажа.");

                if (!float.TryParse(PowerTextBox.Text, out float power))
                    errors.AppendLine("Некорректная мощность.");

                if (!int.TryParse(ColorTempTextBox.Text, out int colorTemp))
                    errors.AppendLine("Некорректная цветовая температура.");

                if (!float.TryParse(CRITextBox.Text, out float cri))
                    errors.AppendLine("Некорректный CRI.");

                if (!int.TryParse(LumenTextBox.Text, out int lumen))
                    errors.AppendLine("Некорректный световой поток.");

                if (!int.TryParse(BeamAngleTextBox.Text, out int beam))
                    errors.AppendLine("Некорректный угол.");

                if (!float.TryParse(VoltageTextBox.Text, out float voltage))
                    errors.AppendLine("Некорректное напряжение.");

                if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price < 0)
                    errors.AppendLine("Цена должна быть положительным числом.");

                if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity < 0)
                    errors.AppendLine("Некорректное количество.");

                if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
                    errors.AppendLine("Введите описание.");

                if (!FlagPhoto)
                    errors.AppendLine("Выберите изображение.");

                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                CurrentLamp.ModelName = ModelNameTextBox.Text;
                CurrentLamp.ManufacturerTypeId = (int)(ManufacturerComboBox.SelectedValue ?? 0);
                CurrentLamp.LampTypeId = (int)(LampTypeComboBox.SelectedValue ?? 0);
                CurrentLamp.MountingTypeId = (int)(MountingTypeComboBox.SelectedValue ?? 0);
                CurrentLamp.PowerWatts = power;
                CurrentLamp.ColorTemperature = colorTemp;
                CurrentLamp.CRI = cri;
                CurrentLamp.LuminousFlux = lumen;
                CurrentLamp.BeamAngle = beam;
                CurrentLamp.Voltage = voltage;
                CurrentLamp.Price = price;
                CurrentLamp.StockQuantity = quantity;
                CurrentLamp.Description = DescriptionTextBox.Text;
                CurrentLamp.HasUVProtection = UVCheckBox.IsChecked == true;
                CurrentLamp.HasIRProtection = IRCheckBox.IsChecked == true;
                CurrentLamp.Dimmable = DimmableCheckBox.IsChecked == true;

                if (!IsEditMode)
                    gallerydatabaseEntities.GetContext().Lamp.Add(CurrentLamp);

                gallerydatabaseEntities.GetContext().SaveChanges();

                MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
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

                    using (MemoryStream ms = new MemoryStream())
                    {
                        PngBitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                        encoder.Save(ms);

                        CurrentLamp.ProductPhoto = ms.ToArray();
                        CurrentLamp.PhotoName = System.IO.Path.GetFileName(openFileDialog.FileName);
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
