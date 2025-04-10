using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using GalleryApp.Data;
using System.Windows;

namespace GalleryApp.Classes
{
    public static class PasswordHelper
    {
        public static byte[] GenerateSalt(int size = 16)
        {
            var rng = RandomNumberGenerator.Create();
            var saltBytes = new byte[size];
            rng.GetBytes(saltBytes);
            return saltBytes;
        }

        public static byte[] HashPassword(byte[] passwordBytes, byte[] salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var combined = passwordBytes.Concat(salt).ToArray();
                return sha256.ComputeHash(combined);
            }
        }

        public static void AddUser(string login, string password, int positionId, int roleId)
        {
            try
            {
                using (var context = gallerydatabaseEntities.GetContext())
                {
                    if (context.Users.Any(u => u.Login == login))
                    {
                        MessageBox.Show("Логин уже существует.");
                        return;
                    }

                    byte[] salt = GenerateSalt();  
                    byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                    byte[] hashBytes = HashPassword(passwordBytes, salt);  

                    var newWorkerInfo = new WorkerInfo
                    {
                        IdPosition = positionId,
                        IdRole = roleId
                    };
                    context.WorkerInfo.Add(newWorkerInfo);
                    context.SaveChanges(); 

                    var newUser = new Users
                    {
                        Login = login,
                        PasswordHash = hashBytes,
                        PasswordSalt = salt,
                        FirstName = "First",
                        MiddleName = "Middle",
                        LastName = "Last",
                        Mail = "example@mail.com",
                        Phone = "123456789",
                        Birth = DateTime.Now,
                        UserType = newWorkerInfo.Id  
                    };
                    context.Users.Add(newUser);
                    context.SaveChanges(); 

                    MessageBox.Show("Пользователь успешно добавлен.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении пользователя: " + ex.Message);
            }
        }


    }
}
