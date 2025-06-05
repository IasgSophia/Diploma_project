using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using GalleryApp.Data;
using GalleryApp.Classes;

namespace GalleryApp.Pages
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder errors = new StringBuilder();

                if (string.IsNullOrWhiteSpace(LoginTextBox.Text))
                    errors.AppendLine("Заполните логин");
                if (string.IsNullOrWhiteSpace(PasswordBox.Password))
                    errors.AppendLine("Заполните пароль");

                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var context = gallerydatabaseEntities.GetContext();

                var user = context.Users
                    .FirstOrDefault(u => u.Login.ToLower() == LoginTextBox.Text.ToLower());

                if (user == null)
                {
                    MessageBox.Show("Неверный логин.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (user.PasswordSalt == null || user.PasswordHash == null)
                {
                    MessageBox.Show("Ошибка в данных пользователя.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                byte[] hashBytes = PasswordHelper.HashPassword(Encoding.UTF8.GetBytes(PasswordBox.Password), user.PasswordSalt);
                if (!hashBytes.SequenceEqual(user.PasswordHash))
                {
                    MessageBox.Show("Неверный пароль.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Manager.CurrentUser = user;

                // Получаем информацию о роли и должности через WorkInfo (UserType -> WorkInfo.Id)
                var workerInfo = context.WorkerInfo
                    .FirstOrDefault(w => w.Id == user.UserType);

                if (workerInfo == null)
                {
                    MessageBox.Show("Не удалось определить роль пользователя.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var role = context.Role
                    .FirstOrDefault(r => r.Id == workerInfo.IdRole);

                if (role == null)
                {
                    MessageBox.Show("Роль пользователя не найдена.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Можно при желании получить должность
                var position = context.Position
                    .FirstOrDefault(p => p.Id == workerInfo.IdPosition)?.Name ?? "Должность не указана";

                MessageBox.Show($"Вы вошли как {role.Name} ({position})", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);

                NavigateUser(role.Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavigateUser(string roleName)
        {
            switch (roleName)
            {
                case "Сотрудник":
                    Manager.MainFrame.Navigate(new ContentPageManager());
                    break;
                case "Пользователь":
                    Manager.MainFrame.Navigate(new ContentPageUser());
                    break;
                case "Администратор":
                    Manager.MainFrame.Navigate(new ContentPageAdmin());
                    break;
                default:
                    MessageBox.Show("Неизвестный тип пользователя.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }
    }
}
