using Microsoft.Office.Interop.Word;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using Word = Microsoft.Office.Interop.Word;

namespace GalleryApp.Classes
{
    public static class WordDocumentGenerator
    {
        public static void CreateInvoice(int orderId)
        {
            try
            {
                var context = Data.gallerydatabaseEntities.GetContext();

                var order = context.Order.FirstOrDefault(o => o.Id == orderId);
                if (order == null)
                {
                    MessageBox.Show("Заказ не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var user = context.Users.FirstOrDefault(u => u.Id == order.IdUser);
                var lamp = context.Lamp.FirstOrDefault(l => l.Id == order.IdLamp);
                var shippingType = context.ShippingType.FirstOrDefault(s => s.Id == order.IdShippingType);

                if (user == null || lamp == null || shippingType == null)
                {
                    MessageBox.Show("Не удалось загрузить все данные для накладной!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var wordApp = new Word.Application();
                wordApp.Visible = false;

                var document = wordApp.Documents.Add();

                document.PageSetup.TopMargin = wordApp.CentimetersToPoints(2);
                document.PageSetup.BottomMargin = wordApp.CentimetersToPoints(2);
                document.PageSetup.LeftMargin = wordApp.CentimetersToPoints(2);
                document.PageSetup.RightMargin = wordApp.CentimetersToPoints(2);

                Word.Paragraph para;

                string formattedDate = DateTime.Now.ToString("d MMMM yyyy г.", new System.Globalization.CultureInfo("ru-RU"));

                para = document.Content.Paragraphs.Add();
                para.Range.Text = $"Товарная накладная № {order.Id} от {formattedDate}";
                para.Range.Font.Name = "Times New Roman";
                para.Range.Font.Size = 16;
                para.Range.Font.Bold = 1;
                para.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                para.Range.InsertParagraphAfter();

                para = document.Content.Paragraphs.Add();
                para.Range.Text = "Поставщик:\nООО «Глобал Текс», ИНН 7701234567, г. Москва, ул. Арбат, д. 15, оф. 23\nТел.: +7 (495) 123-45-67, e-mail: info@galleryart.ru";
                para.Range.Font.Name = "Times New Roman";
                para.Range.Font.Size = 12;
                para.Range.Font.Bold = 0;
                para.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                para.Range.InsertParagraphAfter();

                string clientName = $"{user.LastName} {user.FirstName} {user.MiddleName}";
                para = document.Content.Paragraphs.Add();
                para.Range.Text = $"Покупатель:\n{clientName}, Адрес: {order.Adress}\nТелефон: {user.Phone}";
                para.Range.Font.Name = "Times New Roman";
                para.Alignment = Word.WdParagraphAlignment.wdAlignParagraphJustify;
                para.Range.InsertParagraphAfter();

                para = document.Content.Paragraphs.Add();
                Word.Table table = document.Tables.Add(para.Range, 13, 2);
                table.Borders.Enable = 1;
                table.Range.Font.Name = "Times New Roman";
                table.Range.Font.Size = 12;

                table.Cell(1, 1).Range.Text = "Модель";
                table.Cell(1, 2).Range.Text = lamp.ModelName;

                table.Cell(2, 1).Range.Text = "Производитель";
                table.Cell(2, 2).Range.Text = lamp.Manufacturer?.Name ?? "—";

                table.Cell(3, 1).Range.Text = "Тип лампы";
                table.Cell(3, 2).Range.Text = lamp.LampType?.Name ?? "—";

                table.Cell(4, 1).Range.Text = "Мощность, Вт";
                table.Cell(4, 2).Range.Text = lamp.PowerWatts.ToString();

                table.Cell(5, 1).Range.Text = "Цветовая температура, К";
                table.Cell(5, 2).Range.Text = lamp.ColorTemperature.ToString();

                table.Cell(6, 1).Range.Text = "Индекс цветопередачи (CRI)";
                table.Cell(6, 2).Range.Text = lamp.CRI.ToString();

                table.Cell(7, 1).Range.Text = "Световой поток, лм";
                table.Cell(7, 2).Range.Text = lamp.LuminousFlux.ToString();

                table.Cell(8, 1).Range.Text = "Угол свечения, °";
                table.Cell(8, 2).Range.Text = lamp.BeamAngle.ToString();

                table.Cell(9, 1).Range.Text = "Защита от УФ";
                table.Cell(9, 2).Range.Text = lamp.HasUVProtection ? "Да" : "Нет";

                table.Cell(10, 1).Range.Text = "Защита от ИК";
                table.Cell(10, 2).Range.Text = lamp.HasIRProtection ? "Да" : "Нет";

                table.Cell(11, 1).Range.Text = "Диммируемость";
                table.Cell(11, 2).Range.Text = lamp.Dimmable ? "Да" : "Нет";

                table.Cell(12, 1).Range.Text = "Тип крепления";
                table.Cell(12, 2).Range.Text = lamp.MountingType?.Name ?? "—";

                table.Cell(13, 1).Range.Text = "Цена, руб.";
                table.Cell(13, 2).Range.Text = lamp.Price.ToString("N2");

                para = document.Content.Paragraphs.Add();
                string commentText = string.IsNullOrWhiteSpace(order.Comment) ? "Комментария к заказу нет." : order.Comment;
                para.Range.Text = $"Комментарий к заказу:\n{commentText}";
                para.Range.Font.Name = "Times New Roman";
                para.Alignment = Word.WdParagraphAlignment.wdAlignParagraphJustify;
                para.Range.InsertParagraphAfter();

                para = document.Content.Paragraphs.Add();
                para.Range.Text = $"Тип доставки:\n{shippingType.Name}";
                para.Range.Font.Name = "Times New Roman";
                para.Alignment = Word.WdParagraphAlignment.wdAlignParagraphJustify;
                para.Range.InsertParagraphAfter();

                para = document.Content.Paragraphs.Add();
                para.Range.Text = $"Адрес доставки:\n{order.Adress}";
                para.Range.Font.Name = "Times New Roman";
                para.Alignment = Word.WdParagraphAlignment.wdAlignParagraphJustify;
                para.Range.InsertParagraphAfter();

                para = document.Content.Paragraphs.Add();
                para.Range.Text = "\n\nПодписи сторон:\n\n" +
                                  "От поставщика: _____________________ /Иванов И.И./\n\n" +
                                  $"От покупателя: _____________________ /{user.LastName} {user.FirstName}/";
                para.Range.Font.Name = "Times New Roman";
                para.Alignment = Word.WdParagraphAlignment.wdAlignParagraphJustify;
                para.Range.InsertParagraphAfter();

                
                string directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Накладные");
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                string fileNameSafeClient = string.Join("_", clientName.Split(Path.GetInvalidFileNameChars()));
                string filePath = Path.Combine(directoryPath, $"Накладная_{fileNameSafeClient}_{DateTime.Now:yyyyMMddHHmmss}.docx");

                document.SaveAs2(filePath);
                document.Close();
                wordApp.Quit();

                MessageBox.Show($"Накладная успешно создана:\n{filePath}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания документа:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
