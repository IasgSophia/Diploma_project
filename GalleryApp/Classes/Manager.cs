using GalleryApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Controls;
using System.IO;

namespace GalleryApp.Classes
{
    internal class Manager
    {
        public static Frame MainFrame { get; set; }
        public static Users CurrentUser { get; set; }
        public static List<Lamp> CartItems { get; } = new List<Lamp>();
        public static void GetImageDate()
        {
            var list = Data.gallerydatabaseEntities.GetContext().Lamp.ToList();
            foreach (var item in list)
            {
                string path = Directory.GetCurrentDirectory() + @"\img\" + item.PhotoName;
                if (File.Exists(path))
                {
                    item.ProductPhoto = File.ReadAllBytes(path);
                }
            }
            Data.gallerydatabaseEntities.GetContext().SaveChanges();
        }
        public static List<Lamp> GetCartForCurrentUser()
{
    if (CurrentUser == null)
        throw new InvalidOperationException("Пользователь не авторизован.");

    var context = gallerydatabaseEntities.GetContext();

    var lampIds = context.Order
        .Where(o => o.IdUser == CurrentUser.Id)
        .Select(o => o.IdLamp)
        .Distinct()
        .ToList();

    var cartItems = context.Lamp
        .Where(l => lampIds.Contains(l.Id))
        .ToList();

    CartItems.Clear();
    CartItems.AddRange(cartItems);

    return CartItems;
}

        public static void AddToCart(Lamp lamp)
        {
            if (lamp == null)
                throw new ArgumentNullException(nameof(lamp));

            if (CurrentUser == null)
                throw new InvalidOperationException("Пользователь не авторизован.");

            var context = gallerydatabaseEntities.GetContext();

            // Создаём новую запись в заказе
            var order = new Order
            {
                IdUser = CurrentUser.Id,
                IdLamp = lamp.Id,
                Adress = "Не указано", // можно будет потом изменить
                IdShippingType = 1,    // можно изменить, если есть UI
                Comment = "Добавлено в корзину"
            };

            context.Order.Add(order);
            context.SaveChanges();

            CartItems.Add(lamp);
        }

        public static void RemoveFromCart(int lampId)
        {
            if (CurrentUser == null)
                throw new InvalidOperationException("Пользователь не авторизован.");

            var context = gallerydatabaseEntities.GetContext();
            var orderToRemove = context.Order
                .FirstOrDefault(o => o.IdLamp == lampId && o.IdUser == CurrentUser.Id);

            if (orderToRemove != null)
            {
                context.Order.Remove(orderToRemove);
                context.SaveChanges();
            }

            var itemToRemove = CartItems.FirstOrDefault(a => a.Id == lampId);
            if (itemToRemove != null)
            {
                CartItems.Remove(itemToRemove);
            }
        }

        public static Users GetAuthenticatedUser(string username, string password)
        {
            var context = gallerydatabaseEntities.GetContext();
            var user = context.Users.FirstOrDefault(u => u.Login == username);

            if (user == null || user.PasswordSalt == null || user.PasswordHash == null)
                return null;

            byte[] passwordHash = HashPassword(password, user.PasswordSalt);

            return passwordHash.SequenceEqual(user.PasswordHash) ? user : null;
        }

        private static byte[] HashPassword(string password, byte[] salt)
        {
            using (var hmac = new HMACSHA512(salt))
            {
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public static string GetUserFullName(int userId)
        {
            var context = gallerydatabaseEntities.GetContext();
            var user = context.Users.FirstOrDefault(u => u.Id == userId);

            return user != null ? $"{user.LastName} {user.FirstName} {user.MiddleName}" : string.Empty;
        }
        public static string GetUserAddress(int userId)
        {
            var context = gallerydatabaseEntities.GetContext();
            var order = context.Order
                .Where(o => o.IdUser == userId)
                .OrderByDescending(o => o.Id)
                .FirstOrDefault();

            return order?.Adress ?? "Не указано";
        }

    }
}
