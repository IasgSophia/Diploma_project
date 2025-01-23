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
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
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
                if (string.IsNullOrEmpty(LoginTextBox.Text))
                {
                    errors.AppendLine("Заполните логин");
                }
                if (string.IsNullOrEmpty(PasswordBox.Password))
                {
                    errors.AppendLine("Заполните пароль");
                }

                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

               
                if (Data.gallerydatabaseEntities.GetContext().Workers
                    .Any(d => d.Login.ToLower() == LoginTextBox.Text.ToLower() &&
                    d.Password == PasswordBox.Password))
                {
                    var user = Data.gallerydatabaseEntities.GetContext().Workers
                    .Where(d => d.Login.ToLower() == LoginTextBox.Text.ToLower() &&
                    d.Password == PasswordBox.Password).FirstOrDefault();

                    Classes.Manager.CurrentUser = user;

                    MessageBox.Show("вы вошли, как работник", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);

                    switch (user.Role.Name)
                    {
                        case "Администратор":
                            Classes.Manager.MainFrame.Navigate(new Pages.ContentPageAdmin());
                            break;
                        case "Сотрудник":
                            Classes.Manager.MainFrame.Navigate(new Pages.ContentPageManager());
                            break;
                    }
                }
                else if (Data.gallerydatabaseEntities.GetContext().Clients
                    .Any(d => d.Login == LoginTextBox.Text &&
                    d.Password == PasswordBox.Password))
                {
                    var user = Data.gallerydatabaseEntities.GetContext().Clients
                    .Where(d => d.Login == LoginTextBox.Text &&
                    d.Password == PasswordBox.Password).FirstOrDefault();

                    Classes.Manager.CurrentUser = user;

                    MessageBox.Show("вы вошли, как пользователь", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                    Classes.Manager.MainFrame.Navigate(new Pages.ContentPage());
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
