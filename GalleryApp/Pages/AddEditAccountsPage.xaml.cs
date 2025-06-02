using GalleryApp.Classes;
using GalleryApp.Data;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GalleryApp.Pages
{
    public partial class AddEditAccountsPage : Page
    {
        public AddEditAccountsPage()
        {
            InitializeComponent();
            var context = gallerydatabaseEntities.GetContext();
            RoleComboBox.ItemsSource = context.Role.ToList();
            RoleComboBox.DisplayMemberPath = "Name";
            RoleComboBox.SelectedValuePath = "Id";

            PositionComboBox.ItemsSource = context.Position.ToList();
            PositionComboBox.DisplayMemberPath = "Name";
            PositionComboBox.SelectedValuePath = "Id";
        }

        private void SaveUserButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка обязательных текстовых полей
                if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(MailTextBox.Text) ||
                    string.IsNullOrWhiteSpace(PhoneTextBox.Text) ||
                    string.IsNullOrWhiteSpace(LoginTextBox.Text) ||
                    string.IsNullOrWhiteSpace(PasswordBox.Password))
                {
                    MessageBox.Show("Пожалуйста, заполните все обязательные поля (имя, фамилия, email, телефон, логин, пароль).",
                                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверка выбора роли
                if (RoleComboBox.SelectedValue == null)
                {
                    MessageBox.Show("Выберите роль пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверка выбора позиции
                if (PositionComboBox.SelectedValue == null)
                {
                    MessageBox.Show("Выберите позицию пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверка выбора даты рождения
                if (BirthDatePicker.SelectedDate == null)
                {
                    MessageBox.Show("Выберите дату рождения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var context = gallerydatabaseEntities.GetContext();

                if (context.Users.Any(u => u.Login == LoginTextBox.Text))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                byte[] passwordBytes = Encoding.UTF8.GetBytes(PasswordBox.Password);
                byte[] salt = PasswordHelper.GenerateSalt();
                byte[] hashBytes = PasswordHelper.HashPassword(passwordBytes, salt);

                var workerInfo = new WorkerInfo
                {
                    IdPosition = (int?)PositionComboBox.SelectedValue,
                    IdRole = (int?)RoleComboBox.SelectedValue
                };

                context.WorkerInfo.Add(workerInfo);
                context.SaveChanges();

                var user = new Users
                {
                    FirstName = FirstNameTextBox.Text,
                    MiddleName = MiddleNameTextBox.Text,
                    LastName = LastNameTextBox.Text,
                    Mail = MailTextBox.Text,
                    Phone = PhoneTextBox.Text,
                    Birth = BirthDatePicker.SelectedDate,
                    Login = LoginTextBox.Text,
                    PasswordHash = hashBytes,
                    PasswordSalt = salt,
                    UserType = workerInfo.Id
                };

                context.Users.Add(user);
                context.SaveChanges();

                MessageBox.Show("Пользователь успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message;
                MessageBox.Show("Ошибка при сохранении: " + message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}
