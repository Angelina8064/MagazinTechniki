using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static MagazinTechniki.AdminProducts;
using Microsoft.Office.Interop.Word;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace MagazinTechniki
{
    public partial class AdminAddOrderKorzina : Form
    {
        // Конструктор формы, принимает список товаров и родительскую фрому
        public AdminAddOrderKorzina(List<Products> items, AdminAddOrder parent)
        {
            InitializeComponent();

            dateTimePicker1.MaxDate = DateTime.Now;

            cartItems = new BindingList<Products>(items); // Инициализация корзины
            parentForm = parent; // Сохранение ссылки на родительскую форму
           
            DisplayCartItems(); // Настройка отображения корзины
            UpdateTotalCost(); // Расчет общей стоимости

            comboBoxStatus.SelectedItem = "Оформлен";  // Установка статуса по умолчанию
        }
        
        string connect = Connect.conn;
        private BindingList<Products> cartItems; // Список товаров в корзине
        private AdminAddOrder parentForm; // Ссылка на родительскую форму

        // Настройка отображения DataGridView
        private void DisplayCartItems()
        {
            // Настройка внешнего вида таблицы
            dataGridView1.Dock = DockStyle.None;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; ;

            dataGridView1.Columns.Clear();

            // Добавление колонок
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Артикул", DataPropertyName = "Id" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Название", DataPropertyName = "Name", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells});
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Базовая цена", DataPropertyName = "Cost" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Количество", DataPropertyName = "Quantity" });         
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Цена со скидкой", DataPropertyName = "DiscountedCost" });
           
            LoadUsers();
            LoadClient();

            // Кнопки для управления количеством
            DataGridViewButtonColumn addToCartButton = new DataGridViewButtonColumn
            {
                HeaderText = "Добавить",
                Text = "+",
                Name = "AddButton",
                UseColumnTextForButtonValue = true
            };
            dataGridView1.Columns.Add(addToCartButton);

            DataGridViewButtonColumn delToCartButton = new DataGridViewButtonColumn
            {
                HeaderText = "Удалить",
                Text = "-",
                Name = "DeleteButton",
                UseColumnTextForButtonValue = true
            };
            dataGridView1.Columns.Add(delToCartButton);
            dataGridView1.CellPainting += DataGridView1_CellPainting;


            comboBoxStatus.Items.Add("Оформлен");
            comboBoxStatus.Items.Add("Завершен");

            dataGridView1.DataSource = cartItems; // Привязка данных
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
        }

        private void DataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && (e.ColumnIndex == dataGridView1.Columns["AddButton"].Index || e.ColumnIndex == dataGridView1.Columns["DeleteButton"].Index))
            {
                e.PaintBackground(e.CellBounds, true);

                using (Button btn = new Button())
                {
                    btn.Text = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.BackColor = Color.SlateGray;
                    btn.ForeColor = Color.Black;
                    btn.Font = new System.Drawing.Font("Book Antiqua", 10f, FontStyle.Regular, GraphicsUnit.Point);
                    btn.Size = e.CellBounds.Size;

                    Bitmap bmp = new Bitmap(btn.Width, btn.Height);
                    btn.DrawToBitmap(bmp, new System.Drawing.Rectangle(0, 0, btn.Width, btn.Height));
                    e.Graphics.DrawImage(bmp, e.CellBounds.Location);
                }
                e.Handled = true;
            }
        }

        // Обработчик кликов по кнопкам в таблице
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var column = dataGridView1.Columns[e.ColumnIndex];

                if (column is DataGridViewButtonColumn)
                {
                    // Обработка кнопки "+"
                    if (column.HeaderText == "Добавить")
                    {
                        Products selectedProduct = cartItems[e.RowIndex];

                        int availableQuantity = GetAvailableQuantity(selectedProduct.Id);
                        if (selectedProduct.Quantity < availableQuantity)
                        {
                            selectedProduct.Quantity++; // Увеличение количества

                            dataGridView1.Refresh();

                            UpdateTotalCost();// Пересчет суммы
                        }
                        else
                        {
                            MessageBox.Show($"Не хватает товаров на складе. Доступное количество: {availableQuantity}");
                        }
                    }
                    // Обработка кнопки "-"
                    else if (column.HeaderText == "Удалить")
                    {
                        Products selectedProduct = cartItems[e.RowIndex];
                        if (selectedProduct.Quantity > 1)
                        {
                            selectedProduct.Quantity--; // Уменьшение количества
                        }
                        else
                        {
                            cartItems.RemoveAt(e.RowIndex);  // Удаление товара
                        }

                        dataGridView1.Refresh();
                        UpdateTotalCost();  // Пересчет суммы
                    }
                }
            }
        }

        // Расчет и отображение общей стоимости
        private void UpdateTotalCost()
        {
            decimal totalCost = cartItems.Sum(p => p.DiscountedCost * p.Quantity);
            labelTotalPrice.Text = $"Общая стоимость: {totalCost:C}";
        }

        // Загрузка списка пользователей и клиентов
        private void LoadUsers()
        {
            using (MySqlConnection connection = new MySqlConnection(connect))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand("SELECT UserSurname, UserName FROM user", connection);
                MySqlDataReader reader = command.ExecuteReader();

                comboBoxUsers.Items.Clear();

                while (reader.Read())
                {
                    string fullName = $"{reader["UserSurname"]} {reader["UserName"]}";
                    comboBoxUsers.Items.Add(fullName);
                }

                reader.Close();
            }

            comboBoxUsers.SelectedIndex = 0;
        }
        private void LoadClient()
        {
            using (MySqlConnection connection = new MySqlConnection(connect))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand("SELECT ClientSurname, ClientName FROM client", connection);
                MySqlDataReader reader = command.ExecuteReader();

                comboBoxClient.Items.Clear();

                while (reader.Read())
                {
                    string fullName = $"{reader["ClientSurname"]} {reader["ClientName"]}";
                    comboBoxClient.Items.Add(fullName);
                }

                reader.Close();
            }

            comboBoxClient.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        // Сохранение заказа в БД
        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Проверка заполнения всех полей
            StringBuilder errors = new StringBuilder();

            if (comboBoxUsers.SelectedItem == null)
                errors.AppendLine("Не выбран пользователь!");
            if (comboBoxClient.SelectedItem == null)
                errors.AppendLine("Не выбран клиент!");
            if (comboBoxStatus.SelectedItem == null)
                errors.AppendLine("Не выбран статус заказа!");
            if (cartItems.Count == 0)
                errors.AppendLine("Корзина пуста - добавьте товары!");

            if (errors.Length > 0)
            {
                MessageBox.Show($"Ошибка оформления заказа:\n{errors.ToString()}",
                              "Ошибка",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
                return;
            }

            // Проверка доступного количества товаров
            foreach (var product in cartItems)
            {
                int available = GetAvailableQuantity(product.Id);
                if (product.Quantity > available)
                {
                    MessageBox.Show($"Товар {product.Name} (арт. {product.Id})\n" +
                                  $"Доступно: {available}, заказано: {product.Quantity}",
                                  "Ошибка количества",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error);
                    return;
                }
            }

            // Подтверждение перед сохранением заказа
            DialogResult confirmResult = MessageBox.Show("Вы уверены, что хотите оформить заказ?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Подтверждение перед сохранением заказа
            if (confirmResult == DialogResult.No)
            {
                return;
            }

            using (MySqlConnection connection = new MySqlConnection(connect))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction()) // Добавляем транзакцию
                {
                    try
                    {
                        string selectedUser = comboBoxUsers.SelectedItem.ToString();
                        string selectedClient = comboBoxClient.SelectedItem.ToString();
                        string selectedStatus = comboBoxStatus.SelectedItem.ToString();
                        DateTime orderDate = dateTimePicker1.Value;

                        int userId = GetUserId(selectedUser, connection);
                        int clientId = GetClientId(selectedClient, connection);

                        string insertOrderQuery = @"
                                            INSERT INTO `order` (OrderDate, OrderUserID, OrderClientID, OrderStatus)
                                            VALUES (@OrderDate, @OrderUserID, @OrderClientID, @OrderStatus);
                                            SELECT LAST_INSERT_ID();";

                        MySqlCommand orderCommand = new MySqlCommand(insertOrderQuery, connection);
                        orderCommand.Parameters.AddWithValue("@OrderDate", orderDate);
                        orderCommand.Parameters.AddWithValue("@OrderUserID", userId);
                        orderCommand.Parameters.AddWithValue("@OrderClientID", clientId);
                        orderCommand.Parameters.AddWithValue("@OrderStatus", selectedStatus);

                        int orderId = Convert.ToInt32(orderCommand.ExecuteScalar());

                        foreach (var product in cartItems)
                        {
                            string insertOrderProductQuery = @"
                                            INSERT INTO orderproduct (OrderID, ProductArticleNumber, 
                                            ProductCount, ProoductTotalCost)
                                            VALUES (@OrderID, @ProductArticleNumber, 
                                            @ProductCount, @ProoductTotalCost);";

                            MySqlCommand orderProductCommand = new MySqlCommand(insertOrderProductQuery, connection);
                            orderProductCommand.Parameters.AddWithValue("@OrderID", orderId);
                            orderProductCommand.Parameters.AddWithValue("@ProductArticleNumber", product.Id);
                            orderProductCommand.Parameters.AddWithValue("@ProductCount", product.Quantity);
                            orderProductCommand.Parameters.AddWithValue("@ProoductTotalCost",
                                product.DiscountedCost * product.Quantity);

                            orderProductCommand.ExecuteNonQuery();

                            // Обновляем количество товаров в таблице product
                            string updateProductQuantityQuery = @"
                                            UPDATE product 
                                            SET ProductQuantityInStock = ProductQuantityInStock - @ProductCount
                                            WHERE ProductArticleNumber = @ProductArticleNumber;";

                            MySqlCommand updateProductCommand = new MySqlCommand(updateProductQuantityQuery, connection);
                            updateProductCommand.Parameters.AddWithValue("@ProductCount", product.Quantity);
                            updateProductCommand.Parameters.AddWithValue("@ProductArticleNumber", product.Id);
                            updateProductCommand.ExecuteNonQuery();
                        }

                        transaction.Commit(); // Подтверждаем транзакцию
                        MessageBox.Show("Заказ успешно сохранен!");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback(); // Откатываем при ошибке
                        MessageBox.Show($"Ошибка: {ex.Message}");
                        return;
                    }
                }

            }

            // Подтверждение для печати чека
            DialogResult printResult = MessageBox.Show("Хотите распечатать чек заказа?", "Печать чека", MessageBoxButtons.YesNo);
            if (printResult == DialogResult.Yes)
            {
                PrintReceipt();
            }

            this.Close();

            // Закрываем родительскую форму
            if (parentForm != null && !parentForm.IsDisposed)
            {
                parentForm.Close();
            }
        }

        private void PrintReceipt()
        {
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            Document wordDoc = wordApp.Documents.Add();
            wordApp.Visible = true;

            // Настройка стилей документа
            wordDoc.Content.SetRange(0, 0);
            wordDoc.Content.Font.Name = "Times New Roman";
            wordDoc.Content.Font.Size = 12;

            // Заголовок чека
            Paragraph title = wordDoc.Content.Paragraphs.Add();
            title.Range.Text = "Чек заказа";
            title.Range.Font.Bold = 1;
            title.Range.Font.Size = 16;
            title.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            title.Range.InsertParagraphAfter();

            // Информация о заказе
            Paragraph orderInfo = wordDoc.Content.Paragraphs.Add();
            orderInfo.Range.Text = $"Дата и время заказа: {dateTimePicker1.Value.ToString("dd.MM.yyyy HH:mm")}\n" +
                                   $"Пользователь: {comboBoxUsers.SelectedItem}\n" +
                                   $"Клиент: {comboBoxClient.SelectedItem}\n" +
                                   $"Статус: {comboBoxStatus.SelectedItem}\n\n" +
                                   "Товары:\n";
            orderInfo.Range.InsertParagraphAfter();

            // Таблица для списка товаров
            Table productTable = wordDoc.Tables.Add(orderInfo.Range, cartItems.Count + 1, 5); 
            productTable.Borders.Enable = 1;

            // Заголовки таблицы 
            productTable.Cell(1, 1).Range.Text = "Название";
            productTable.Cell(1, 2).Range.Text = "Количество";
            productTable.Cell(1, 3).Range.Text = "Базовая цена";
            productTable.Cell(1, 4).Range.Text = "Скидка";
            productTable.Cell(1, 5).Range.Text = "Сумма";

            // Заполнение таблицы данными о товарах
            for (int i = 0; i < cartItems.Count; i++)
            {
                var product = cartItems[i];
                productTable.Cell(i + 2, 1).Range.Text = product.Name;
                productTable.Cell(i + 2, 2).Range.Text = product.Quantity.ToString();
                productTable.Cell(i + 2, 3).Range.Text = product.Cost.ToString("C");
                productTable.Cell(i + 2, 4).Range.Text = $"{product.Discount}%";
                productTable.Cell(i + 2, 5).Range.Text = (product.Quantity * product.DiscountedCost).ToString("C");
            }

            // Итоговая стоимость
            decimal totalCost = cartItems.Sum(p => p.DiscountedCost * p.Quantity);
            decimal totalDiscount = cartItems.Sum(p => (p.Cost * p.Discount / 100) * p.Quantity);

            Paragraph totals = wordDoc.Content.Paragraphs.Add();
            totals.Range.Text = $"\nОбщая сумма скидки: {totalDiscount:C}\n" +
                                $"Итого к оплате: {totalCost:C}";
            totals.Range.Font.Bold = 1;
            totals.Range.Font.Size = 14;
            totals.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
            totals.Range.InsertParagraphAfter();

            // Сохранение документа
            string debugFolder = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            string receiptsFolder = Path.Combine(debugFolder, "Чеки");

            if (!Directory.Exists(receiptsFolder))
            {
                Directory.CreateDirectory(receiptsFolder);
            }

            string fileName = $"Чек_заказа_{DateTime.Now:yyyyMMdd_HHmmss}.docx";
            string filePath = Path.Combine(receiptsFolder, fileName);
            wordDoc.SaveAs(filePath);
        }


        private int GetUserId(string fullName, MySqlConnection connection)
        {
            string[] names = fullName.Split(' ');
            string query = "SELECT UserID FROM user WHERE UserSurname = @Surname AND UserName = @Name;";
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Surname", names[0]);
            command.Parameters.AddWithValue("@Name", names[1]);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        private int GetClientId(string fullName, MySqlConnection connection)
        {
            string[] names = fullName.Split(' ');
            string query = "SELECT ClientID FROM client WHERE ClientSurname = @Surname AND ClientName = @Name;";
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Surname", names[0]);
            command.Parameters.AddWithValue("@Name", names[1]);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        private int GetAvailableQuantity(string productArticle)
        {
            using (MySqlConnection connection = new MySqlConnection(connect))
            {
                connection.Open();
                string query = @"
                        SELECT ProductQuantityInStock, ProductDiscount 
                        FROM product 
                        WHERE ProductArticleNumber = @ProductArticleNumber";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductArticleNumber", productArticle);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var product = cartItems.FirstOrDefault(p => p.Id == productArticle);
                        if (product != null)
                        {
                            // обновляем скидку
                            product.Discount = Convert.ToInt32(reader["ProductDiscount"]);
                        }
                        return Convert.ToInt32(reader["ProductQuantityInStock"]);
                    }
                }
            }
            return 0;
        }

    }
}