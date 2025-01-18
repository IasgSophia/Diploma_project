using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GalleryApp.Classes
{
    internal class Manager
    {
        public static Frame MainFrame { get; set; }
        public static Data.Workers CurrentUser { get; set; }
        public static void GetImageDate()
        {
            var list = Data.gallerydatabaseEntities.GetContext().Art.ToList();
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
    }
}
