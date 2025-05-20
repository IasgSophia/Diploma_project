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

                var user = gallerydatabaseEntities.GetContext().Users
                    .FirstOrDefault(u => u.Login.ToLower() == LoginTextBox.Text.ToLower());

                if (user == null)
                {
                    MessageBox.Show("Неверный логин.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                byte[] hashBytes = PasswordHelper.HashPassword(Encoding.UTF8.GetBytes(PasswordBox.Password), user.PasswordSalt);
                if (!hashBytes.SequenceEqual(user.PasswordHash))
                {
                    MessageBox.Show("Неверный пароль.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Manager.CurrentUser = user; 

                var workerInfo = gallerydatabaseEntities.GetContext().WorkerInfo
                    .FirstOrDefault(w => w.Id == user.UserType);
                if (workerInfo == null)
                {
                    MessageBox.Show("Не удалось определить роль пользователя.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var role = gallerydatabaseEntities.GetContext().Role
                    .FirstOrDefault(r => r.Id == workerInfo.IdRole);
                if (role == null)
                {
                    MessageBox.Show("Роль пользователя не найдена.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show($"Вы вошли как {role.Name}", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigateUser(role);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void NavigateUser(Role userrole)
        {
            switch (userrole.Name)
            {
                case "Сотрудник":
                    Classes.Manager.MainFrame.Navigate(new Pages.ContentPageManager());
                    break;
                case "Пользователь":
                    Classes.Manager.MainFrame.Navigate(new Pages.ContentPageUser());
                    break;
                case "Администратор":
                    Classes.Manager.MainFrame.Navigate(new Pages.ContentPageAdmin());
                    break;
                default:
                    throw new Exception("Неизвестный тип пользователя.");
            }
        }
    }
}
