using GalleryApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Controls;

namespace GalleryApp.Classes
{
    internal class Manager
    {
        public static Frame MainFrame { get; set; }
        public static Users CurrentUser { get; set; }
        public static List<Art> CartItems { get; } = new List<Art>();

        public static List<Art> GetCartForCurrentUser()
        {
            var context = gallerydatabaseEntities.GetContext();
            var cartItems = context.Order
                .Where(o => o.IdUser == CurrentUser.Id)
                .Select(o => o.Art)
                .ToList();

            return cartItems;
        }

        public static void AddToCart(Art art)
        {
            if (art == null)
            {
                throw new ArgumentNullException(nameof(art), "Невозможно добавить пустой товар в корзину");
            }

            CartItems.Add(art);

            var context = gallerydatabaseEntities.GetContext();
            var order = new Order
            {
                IdUser = CurrentUser.Id,
                IdArt = art.id,
                Adress = "Не указано",
                IdShippingType = 1,
                Comment = "Комментарий по заказу"
            };
            context.Order.Add(order);
            context.SaveChanges();
        }

        public static void RemoveFromCart(Art art)
        {
            if (art == null)
            {
                throw new ArgumentNullException(nameof(art), "Невозможно удалить пустой товар");
            }

            CartItems.Remove(art);

            var context = gallerydatabaseEntities.GetContext();
            var orderItem = context.Order
                .FirstOrDefault(o => o.IdArt == art.id && o.IdUser == CurrentUser.Id);

            if (orderItem != null)
            {
                context.Order.Remove(orderItem);
                context.SaveChanges();
            }
        }

        public static Users GetAuthenticatedUser(string username, string password)
        {
            var context = gallerydatabaseEntities.GetContext();
            var user = context.Users.FirstOrDefault(u => u.Login == username);

            if (user == null || user.PasswordSalt == null || user.PasswordHash == null)
            {
                return null;
            }

            byte[] passwordHash = HashPassword(password, user.PasswordSalt);

            if (passwordHash.SequenceEqual(user.PasswordHash))
            {
                return user;
            }

            return null;
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

            if (user != null)
            {
                return $"{user.LastName} {user.FirstName} {user.MiddleName}";
            }
            return string.Empty;
        }

        public static string GetUserAddress(int userId)
        {
            var context = gallerydatabaseEntities.GetContext();
            var order = context.Order
                .Where(o => o.IdUser == userId)
                .FirstOrDefault();

            return order?.Adress ?? "Не указано";
        }

        public static void RemoveFromCart(int artId)
        {
            if (CurrentUser == null)
            {
                throw new InvalidOperationException("Пользователь не авторизован");
            }

            var context = gallerydatabaseEntities.GetContext();
            var artToRemove = context.Order.FirstOrDefault(o => o.IdArt == artId && o.IdUser == CurrentUser.Id);

            if (artToRemove != null)
            {
                context.Order.Remove(artToRemove);
                context.SaveChanges();

                var itemToRemove = CartItems.FirstOrDefault(a => a.id == artId);
                if (itemToRemove != null)
                {
                    CartItems.Remove(itemToRemove);
                }
            }
        }
    }
}
