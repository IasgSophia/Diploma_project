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

            // Загрузка данных для комбобоксов
            var context = gallerydatabaseEntities.GetContext();

            // Загружаем роли
            RoleComboBox.ItemsSource = context.Role.ToList();
            RoleComboBox.DisplayMemberPath = "Name"; // Название роли будет отображаться
            RoleComboBox.SelectedValuePath = "Id"; // ID роли будет использован для сохранения

            // Загружаем позиции
            PositionComboBox.ItemsSource = context.Position.ToList();
            PositionComboBox.DisplayMemberPath = "Name"; // Название позиции
            PositionComboBox.SelectedValuePath = "Id"; // ID позиции
        }

        private void SaveUserButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка на пустые логин/пароль
                if (string.IsNullOrWhiteSpace(LoginTextBox.Text) || string.IsNullOrWhiteSpace(PasswordBox.Password))
                {
                    MessageBox.Show("Введите логин и пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var context = gallerydatabaseEntities.GetContext();

                // Проверка на уникальность логина
                if (context.Users.Any(u => u.Login == LoginTextBox.Text))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Генерация соли и хэша
                byte[] passwordBytes = Encoding.UTF8.GetBytes(PasswordBox.Password);
                byte[] salt = PasswordHelper.GenerateSalt();
                byte[] hashBytes = PasswordHelper.HashPassword(passwordBytes, salt);

                // Шаг 1. Сначала создаём запись в WorkerInfo
                var workerInfo = new WorkerInfo
                {
                    IdPosition = (int?)PositionComboBox.SelectedValue,
                    IdRole = (int?)RoleComboBox.SelectedValue
                };

                context.WorkerInfo.Add(workerInfo);
                context.SaveChanges(); // Сохраняем, чтобы получить Id

                // Шаг 2. Создаём запись пользователя с UserType = workerInfo.Id
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
                // Диагностика полной цепочки ошибок
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
