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
        private Users CurrentUser;
        private bool IsEditMode = false;

        public AddEditAccountsPage(Users selectedUser = null)
        {
            InitializeComponent();

            if (selectedUser != null)
            {
                CurrentUser = selectedUser;
                IsEditMode = true;
            }
            else
            {
                CurrentUser = new Users();
                IsEditMode = false;
            }

            DataContext = CurrentUser;
            Init();
        }

        private void Init()
        {
            var context = gallerydatabaseEntities.GetContext();

            RoleComboBox.ItemsSource = context.Role.ToList();
            RoleComboBox.DisplayMemberPath = "Name";
            RoleComboBox.SelectedValuePath = "Id";

            PositionComboBox.ItemsSource = context.Position.ToList();
            PositionComboBox.DisplayMemberPath = "Name";
            PositionComboBox.SelectedValuePath = "Id";

            if (IsEditMode)
            {
                var worker = context.WorkerInfo.FirstOrDefault(w => w.Id == CurrentUser.UserType);
                if (worker != null)
                {
                    RoleComboBox.SelectedValue = worker.IdRole;
                    PositionComboBox.SelectedValue = worker.IdPosition;
                }

                FirstNameTextBox.Text = CurrentUser.FirstName;
                MiddleNameTextBox.Text = CurrentUser.MiddleName;
                LastNameTextBox.Text = CurrentUser.LastName;
                MailTextBox.Text = CurrentUser.Mail;
                PhoneTextBox.Text = CurrentUser.Phone;
                BirthDatePicker.SelectedDate = CurrentUser.Birth;
                LoginTextBox.Text = CurrentUser.Login;
            }
        }

        private void SaveUserButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder errors = new StringBuilder();

                if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
                    errors.AppendLine("Введите имя");
                if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
                    errors.AppendLine("Введите фамилию");
                if (string.IsNullOrWhiteSpace(MailTextBox.Text))
                    errors.AppendLine("Введите email");
                if (string.IsNullOrWhiteSpace(PhoneTextBox.Text))
                    errors.AppendLine("Введите телефон");
                if (string.IsNullOrWhiteSpace(LoginTextBox.Text))
                    errors.AppendLine("Введите логин");
                if (!IsEditMode && string.IsNullOrWhiteSpace(PasswordBox.Password))
                    errors.AppendLine("Введите пароль");
                if (RoleComboBox.SelectedValue == null)
                    errors.AppendLine("Выберите роль");
                if (PositionComboBox.SelectedValue == null)
                    errors.AppendLine("Выберите позицию");
                if (BirthDatePicker.SelectedDate == null)
                    errors.AppendLine("Выберите дату рождения");

                var context = gallerydatabaseEntities.GetContext();
                if (!IsEditMode && context.Users.Any(u => u.Login == LoginTextBox.Text))
                    errors.AppendLine("Пользователь с таким логином уже существует");

                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var roleId = (int)RoleComboBox.SelectedValue;
                var positionId = (int)PositionComboBox.SelectedValue;
                WorkerInfo worker;

                if (!IsEditMode)
                {
                    byte[] passwordBytes = Encoding.UTF8.GetBytes(PasswordBox.Password);
                    byte[] salt = PasswordHelper.GenerateSalt();
                    byte[] hashBytes = PasswordHelper.HashPassword(passwordBytes, salt);

                    worker = new WorkerInfo
                    {
                        IdRole = roleId,
                        IdPosition = positionId
                    };
                    context.WorkerInfo.Add(worker);
                    context.SaveChanges();

                    CurrentUser = new Users
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
                        UserType = worker.Id
                    };

                    context.Users.Add(CurrentUser);
                }
                else
                {
                    worker = context.WorkerInfo.FirstOrDefault(w => w.Id == CurrentUser.UserType);
                    if (worker != null)
                    {
                        worker.IdRole = roleId;
                        worker.IdPosition = positionId;
                    }

                    CurrentUser.FirstName = FirstNameTextBox.Text;
                    CurrentUser.MiddleName = MiddleNameTextBox.Text;
                    CurrentUser.LastName = LastNameTextBox.Text;
                    CurrentUser.Mail = MailTextBox.Text;
                    CurrentUser.Phone = PhoneTextBox.Text;
                    CurrentUser.Birth = BirthDatePicker.SelectedDate;
                    CurrentUser.Login = LoginTextBox.Text;
                }

                context.SaveChanges();
                MessageBox.Show("Пользователь успешно сохранён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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

