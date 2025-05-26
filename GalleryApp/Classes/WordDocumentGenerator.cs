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
                var art = context.Art.FirstOrDefault(a => a.id == order.Id);
                var shippingType = context.ShippingType.FirstOrDefault(s => s.Id == order.IdShippingType);

                if (user == null || art == null || shippingType == null)
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

                document.Content.Font.Name = "Times New Roman";
                document.Content.Font.Size = 12;

                Word.Paragraph para;

                para = document.Content.Paragraphs.Add();
                string formattedDate = DateTime.Now.ToString("d MMMM yyyy г.", new System.Globalization.CultureInfo("ru-RU"));
                para.Range.Text = $"Товарная накладная № {order.Id} от {formattedDate}";
                para.Range.Font.Size = 16;
                para.Range.Font.Bold = 1;
                para.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                para.Range.InsertParagraphAfter();

                para = document.Content.Paragraphs.Add();
                para.Range.Text = "Поставщик:\nООО «Глобал Текс», ИНН 7701234567, г. Москва, ул. Арбат, д. 15, оф. 23\nТел.: +7 (495) 123-45-67, e-mail: info@galleryart.ru";
                para.Range.Font.Size = 12;
                para.Range.Font.Bold = 0;
                para.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para.Format.LeftIndent = wordApp.CentimetersToPoints(1.25f);
                para.Range.InsertParagraphAfter();

                para = document.Content.Paragraphs.Add();
                string clientName = $"{user.LastName} {user.FirstName} {user.MiddleName}";
                para.Range.Text = $"Покупатель:\n{clientName}, Адрес: {order.Adress}\nТелефон: {user.Phone}";
                para.Range.InsertParagraphAfter();

                Word.Table table = document.Tables.Add(para.Range, 2, 5);
                table.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                table.Borders.Enable = 1;
                table.Rows.Alignment = Word.WdRowAlignment.wdAlignRowCenter;

                table.Cell(1, 1).Range.Text = "№";
                table.Cell(1, 2).Range.Text = "Наименование произведения";
                table.Cell(1, 3).Range.Text = "Количество";
                table.Cell(1, 4).Range.Text = "Цена за ед., руб.";
                table.Cell(1, 5).Range.Text = "Сумма, руб.";

                table.Cell(2, 1).Range.Text = "1";
                table.Cell(2, 2).Range.Text = art.title;
                table.Cell(2, 3).Range.Text = "1";
                table.Cell(2, 4).Range.Text = art.price.ToString("N2");
                table.Cell(2, 5).Range.Text = art.price.ToString("N2");

                para = document.Content.Paragraphs.Add();
                para.Range.InsertParagraphAfter();

                para = document.Content.Paragraphs.Add();
                para.Range.Text = $"Комментарий к заказу:\n{order.Comment}";
                para.Format.LeftIndent = wordApp.CentimetersToPoints(1.25f);
                para.Range.InsertParagraphAfter();

                para = document.Content.Paragraphs.Add();
                para.Range.Text = $"Тип доставки:\n{shippingType.Name}";
                para.Format.LeftIndent = wordApp.CentimetersToPoints(1.25f);
                para.Range.InsertParagraphAfter();

                para = document.Content.Paragraphs.Add();
                para.Range.Text = $"Адрес доставки:\n{order.Adress}";
                para.Format.LeftIndent = wordApp.CentimetersToPoints(1.25f);
                para.Range.InsertParagraphAfter();

                para = document.Content.Paragraphs.Add();
                para.Range.Text = "\n\nПодписи сторон:\n\n" +
                                  "От поставщика: _____________________ /Иванов И.И./\n\n" +
                                  $"От покупателя: _____________________ /{user.LastName} {user.FirstName}/";
                para.Format.LeftIndent = wordApp.CentimetersToPoints(1.25f);

                string directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Накладные");
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                string filePath = Path.Combine(directoryPath, $"Накладная_{clientName}_{DateTime.Now:yyyyMMddHHmmss}.docx");
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
