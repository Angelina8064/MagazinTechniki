using Microsoft.Office.Interop.Word;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; 
using Application = Microsoft.Office.Interop.Word.Application;

namespace MagazinTechniki
{
    public partial class AdminOtchet : Form
    {
        public AdminOtchet()
        {
            InitializeComponent();

            dateTimePicker1.MaxDate = DateTime.Now;
            dateTimePicker2.MaxDate = DateTime.Now;
        }
        
        string connect = Connect.conn;

        public class ProductSale
        {
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public decimal TotalCost { get; set; }
        }

        private void buttonGenerationOtchet_Click(object sender, EventArgs e)
        {
            DateTime startDate = dateTimePicker1.Value;
            DateTime endDate = dateTimePicker2.Value;


            // Получаем данные о выручке
            decimal totalRevenue = GetTotalRevenue(startDate, endDate);

            // Создаем документ Word
            CreateWordReport(startDate, endDate, totalRevenue);
        }

        // Общая сумма выручки за период
        private decimal GetTotalRevenue(DateTime startDate, DateTime endDate)
        {
            // Инициализируем переменную для хранения общей выручки
            decimal totalRevenue = 0;

            // Создаем подключение к базе данных
            using (MySqlConnection connection = new MySqlConnection(connect))
            {
                // Открываем соединение с БД
                connection.Open();

                // SQL-запрос для получения суммы всех завершенных заказов за период
                string query = @"
                    SELECT 
                        SUM(op.ProoductTotalCost) AS TotalRevenue
                    FROM 
                        `order` o
                    JOIN 
                        `orderproduct` op ON o.OrderID = op.OrderID
                    WHERE 
                        o.OrderDate BETWEEN @StartDate AND @EndDate
                        AND o.OrderStatus = 'Завершен';";

                // Создаем команду для выполнения запроса
                MySqlCommand command = new MySqlCommand(query, connection);

                // Добавляем параметры для дат периода
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);

                // Выполняем запрос и получаем результат
                object result = command.ExecuteScalar();

                // Проверяем, что результат не пустой
                if (result != null && result != DBNull.Value)
                {
                    totalRevenue = Convert.ToDecimal(result);
                }
            }

            // Возвращаем общую выручку
            return totalRevenue;
        }

        //Список объектов ProductSale с информацией о продажах
        private List<ProductSale> GetSoldProducts(DateTime startDate, DateTime endDate)
        {
            // Создаем список для хранения информации о проданных товарах
            List<ProductSale> soldProducts = new List<ProductSale>();

            using (MySqlConnection connection = new MySqlConnection(connect))
            {
                connection.Open();

                string query = @"
                    SELECT 
                        p.ProductName AS ProductName,
                        op.ProductCount AS Quantity,
                        op.ProoductTotalCost AS TotalCost
                    FROM 
                        `order` o
                    JOIN 
                        `orderproduct` op ON o.OrderID = op.OrderID
                    JOIN 
                        `product` p ON op.ProductArticleNumber = p.ProductArticleNumber
                    WHERE 
                        o.OrderDate BETWEEN @StartDate AND @EndDate
                        AND o.OrderStatus = 'Завершен';";

                MySqlCommand command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Создаем объект ProductSale для каждой строки результата
                        soldProducts.Add(new ProductSale
                        {
                            ProductName = reader["ProductName"].ToString(),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            TotalCost = Convert.ToDecimal(reader["TotalCost"])
                        });
                    }
                }
            }

            // Возвращаем список проданных товаров
            return soldProducts;
        }

        private void CreateWordReport(DateTime startDate, DateTime endDate, decimal totalRevenue)
        {
            // Создаем экземпляр Word
            Application wordApp = new Application();
            Document wordDoc = wordApp.Documents.Add();

            try
            {
                // Настройка документа
                wordApp.Visible = true; // Показываем документ пользователю

                // Добавляем заголовок отчета
                Paragraph title = wordDoc.Content.Paragraphs.Add();
                title.Range.Text = "Отчет о выручке";
                title.Range.Font.Bold = 1;
                title.Range.Font.Size = 16;
                title.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                title.Range.InsertParagraphAfter();

                // Добавляем информацию о периоде
                Paragraph periodInfo = wordDoc.Content.Paragraphs.Add();
                periodInfo.Range.Text = $"Период: с {startDate.ToShortDateString()} по {endDate.ToShortDateString()}";
                periodInfo.Range.Font.Size = 14;
                periodInfo.Range.InsertParagraphAfter();

                // Добавляем общую выручку
                Paragraph revenueInfo = wordDoc.Content.Paragraphs.Add();
                revenueInfo.Range.Text = $"Общая выручка: {totalRevenue:C}";
                revenueInfo.Range.Font.Size = 14;
                revenueInfo.Range.InsertParagraphAfter();

                // Получаем список проданной продукции
                List<ProductSale> soldProducts = GetSoldProducts(startDate, endDate);

                // Добавляем таблицу с проданной продукцией
                if (soldProducts.Count > 0)
                {
                    // Добавляем заголовок для таблицы
                    Paragraph tableTitle = wordDoc.Content.Paragraphs.Add();
                    tableTitle.Range.Text = "Список проданной продукции:";
                    tableTitle.Range.Font.Size = 14;
                    tableTitle.Range.InsertParagraphAfter();

                    // Определяем диапазон для вставки таблицы
                    Range tableRange = wordDoc.Content;
                    tableRange.Collapse(WdCollapseDirection.wdCollapseEnd); // Перемещаем курсор в конец документа

                    // Создаем таблицу
                    Table productTable = wordDoc.Tables.Add(
                        tableRange, // Место для вставки таблицы
                        soldProducts.Count + 1, // Количество строк (заголовок + данные)
                        3 // Количество колонок
                    );
                    productTable.Borders.Enable = 1; // Включаем границы таблицы

                    // Заголовки таблицы
                    productTable.Cell(1, 1).Range.Text = "Название товара";
                    productTable.Cell(1, 2).Range.Text = "Количество";
                    productTable.Cell(1, 3).Range.Text = "Стоимость";

                    // Заполнение таблицы данными
                    for (int i = 0; i < soldProducts.Count; i++)
                    {
                        productTable.Cell(i + 2, 1).Range.Text = soldProducts[i].ProductName;
                        productTable.Cell(i + 2, 2).Range.Text = soldProducts[i].Quantity.ToString();
                        productTable.Cell(i + 2, 3).Range.Text = soldProducts[i].TotalCost.ToString("C");
                    }
                }
                else
                {
                    // Если нет данных о проданной продукции
                    Paragraph noData = wordDoc.Content.Paragraphs.Add();
                    noData.Range.Text = "Нет данных о проданной продукции за выбранный период.";
                    noData.Range.Font.Size = 14;
                    noData.Range.InsertParagraphAfter();
                }

                // Сохранение документа
                string debugFolder = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
                string reportsFolder = Path.Combine(debugFolder, "Отчеты");

                // Создаем папку "Отчеты", если она не существует
                if (!Directory.Exists(reportsFolder))
                {
                    Directory.CreateDirectory(reportsFolder);
                }

                // Генерация имени файла
                string fileName = $"Отчет_выручка_{DateTime.Now:yyyyMMdd_HHmmss}.docx";
                string filePath = Path.Combine(reportsFolder, fileName);

                // Сохраняем документ
                wordDoc.SaveAs(filePath);
                MessageBox.Show($"Отчет успешно сохранен: {filePath}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании отчета: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value > dateTimePicker2.Value)
            {
                dateTimePicker2.Value = dateTimePicker1.Value;
            }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker2.Value < dateTimePicker1.Value)
            {
                dateTimePicker1.Value = dateTimePicker2.Value;
            }
        }
    }
}

