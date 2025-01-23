using GalleryApp.Classes;
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
    /// Логика взаимодействия для AddEditAccountsPage.xaml
    /// </summary>
    public partial class AddEditAccountsPage : Page
    {
        public AddEditAccountsPage()
        {
            InitializeComponent();
        }

        private void SaveUserButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(LoginTextBox.Text) || string.IsNullOrWhiteSpace(PasswordBox.Password))
                {
                    MessageBox.Show("Заполните обязательные поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                string salt;
                string hash = PasswordHelper.HashPassword(PasswordBox.Password, out salt);

                if (string.IsNullOrWhiteSpace(PositionTextBox.Text) && string.IsNullOrWhiteSpace(RoleTextBox.Text))
                {
                    var client = new Clients
                    {
                        FirstName = FirstNameTextBox.Text,
                        MiddleName = MiddleNameTextBox.Text,
                        LastName = LastNameTextBox.Text,
                        Mail = MailTextBox.Text,
                        Phone = PhoneTextBox.Text,
                        Login = LoginTextBox.Text,
                        PasswordHash = hash,
                        PasswordSalt = salt,
                        Birth = BirthDatePicker.SelectedDate
                    };

                    Data.gallerydatabaseEntities.GetContext().Clients.Add(client);
                }
                else
                {
                    var worker = new Workers
                    {
                        FirstName = FirstNameTextBox.Text,
                        MiddleName = MiddleNameTextBox.Text,
                        LastName = LastNameTextBox.Text,
                        Mail = MailTextBox.Text,
                        Phone = PhoneTextBox.Text,
                        Login = LoginTextBox.Text,
                        PasswordHash = hash,
                        PasswordSalt = salt,
                        Birth = BirthDatePicker.SelectedDate,
                        idPosition = int.Parse(PositionTextBox.Text),
                        idRole = int.Parse(RoleTextBox.Text)
                    };

                    Data.gallerydatabaseEntities.GetContext().Workers.Add(worker);
                }

                Data.gallerydatabaseEntities.GetContext().SaveChanges();
                MessageBox.Show("Пользователь сохранён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
