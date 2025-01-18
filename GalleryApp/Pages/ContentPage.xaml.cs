using GalleryApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GalleryApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для ContentPage.xaml
    /// </summary>
    public partial class ContentPage : Page
    {
        public ContentPage()
        {
            InitializeComponent();
            Init();
        }
        public void Init()
        {
            try
            {
                ProductsListView.ItemsSource = Data.gallerydatabaseEntities.GetContext().Art.ToList();
                var sizeTypeList = Data.gallerydatabaseEntities.GetContext().TypeSize.ToList();
                sizeTypeList.Insert(0, new Data.TypeSize { Size = "Все размеры" });
                SizeTypeComboBox.ItemsSource = sizeTypeList;
                SizeTypeComboBox.SelectedIndex = 0;

                if (Classes.Manager.CurrentUser != null)
                {
                    FIOLabel.Visibility = Visibility.Visible;
                    FIOLabel.Content = $"{Classes.Manager.CurrentUser.LastName} " +
                         $"{Classes.Manager.CurrentUser.FirstName} " +
                         $"{Classes.Manager.CurrentUser.MiddleName} ";                    
                }
                else
                {
                    FIOLabel.Visibility = Visibility.Hidden;                    
                    ProductsListView.Visibility = Visibility.Hidden;
                }

                CountOfLabel.Content = $"{Data.gallerydatabaseEntities.GetContext().Art.Count()}/" +
                     $"{Data.gallerydatabaseEntities.GetContext().Art.Count()}";
            }
            catch { }
        }

        public List<Data.Art> _products = Data.gallerydatabaseEntities.GetContext().Art.ToList();

        public void Update()
        {
            try
            {
                _products = Data.gallerydatabaseEntities.GetContext().Art.ToList();

                if (!string.IsNullOrEmpty(SearchTextBox.Text))
                {
                    _products = (from item in Data.gallerydatabaseEntities.GetContext().Art.ToList()
                                 where item.title.ToLower().Contains(SearchTextBox.Text.ToLower()) ||
                                 item.author.ToLower().Contains(SearchTextBox.Text.ToLower()) ||
                                 item.genre.ToLower().Contains(SearchTextBox.Text.ToLower()) ||
                                 item.Exibition.Name.ToString().ToLower().Contains(SearchTextBox.Text.ToLower())
                                 select item).ToList();

                }
                if (SortUpRadioButton.IsChecked == true)
                {
                    _products = _products.OrderBy(d => d.price).ToList();
                }

                if (SortDownRadioButton.IsChecked == true)
                {
                    _products = _products.OrderByDescending(d => d.price).ToList();
                }

                var selected = SizeTypeComboBox.SelectedItem as Data.TypeSize;
                if (selected != null && selected.Size != "Все размеры")
                {
                    _products = _products.Where(d => d.id == selected.Id).ToList();
                }

                CountOfLabel.Content = $"{_products.Count}/" +
                     $"{Data.gallerydatabaseEntities.GetContext().Art.Count()}";

                ProductsListView.ItemsSource = _products;                
            }
            catch
            {

            }
        }


        private void AuthorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
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

        private void SizeTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }
    }
}