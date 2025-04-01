using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MagazinTechniki.AdminOrder;

namespace MagazinTechniki
{
    public partial class EditOrder : Form
    {
        private Orders order;
        private string connect = Connect.conn;

        // Словарь для хранения начальных количеств товаров
        private Dictionary<string, int> initialProductQuantities = new Dictionary<string, int>();

        public EditOrder(Orders selectedOrder)
        {
            InitializeComponent();

            dateTimePicker1.MaxDate = DateTime.Now;

            // Загружаем данные в ComboBox
            LoadUsers();
            LoadClients();

            // Загружаем статусы в ComboBox
            comboBoxStatus.Items.Add("Оформлен");
            comboBoxStatus.Items.Add("Завершен");

            // Устанавливаем значения из заказа
            this.order = selectedOrder;
            dateTimePicker1.Value = order.Date;

            // Устанавливаем выбранные значения в ComboBox
            if (comboBoxUsers.Items.Contains(order.UserSurname)) // Теперь order.UserSurname содержит полное имя
            {
                comboBoxUsers.SelectedItem = order.UserSurname;
            }
            else
            {
                MessageBox.Show("Сотрудник не найден в списке.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (comboBoxClient.Items.Contains(order.ClientSurname)) // Теперь order.ClientSurname содержит полное имя
            {
                comboBoxClient.SelectedItem = order.ClientSurname;
            }
            else
            {
                MessageBox.Show("Клиент не найден в списке.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Устанавливаем статус заказа
            if (comboBoxStatus.Items.Contains(order.Status))
            {
                comboBoxStatus.SelectedItem = order.Status;
            }
            else
            {
                // Если статус не найден, добавляем его в ComboBox
                comboBoxStatus.Items.Add(order.Status);
                comboBoxStatus.SelectedItem = order.Status;

                MessageBox.Show("Статус заказа был добавлен в список.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // Загружаем товары в заказе
            LoadOrderProducts();
        }

        // Метод для загрузки списка пользователей из БД
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
        }

        // Метод для загрузки списка клиентов из БД
        private void LoadClients()
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
        }

        // Метод для загрузки товаров в заказе
        private void LoadOrderProducts()
        {
            // Настройка DataGridView
            dataGridView1.Dock = DockStyle.None;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Очищаем DataGridView
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            // Добавляем столбцы в DataGridView
            dataGridView1.Columns.Add("Name", "Название товара");
            dataGridView1.Columns.Add("Quantity", "Количество");
            dataGridView1.Columns.Add("CostPerUnit", "Стоимость за единицу");
            dataGridView1.Columns.Add("Discount", "Скидка (%)");
            dataGridView1.Columns.Add("TotalCost", "Общая стоимость");

            dataGridView1.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            // Добавляем кнопки "+" и "-"
            DataGridViewButtonColumn addButton = new DataGridViewButtonColumn
            {
                HeaderText = "Добавить",
                Text = "+",
                Name = "AddButton",
                UseColumnTextForButtonValue = true
            };
            dataGridView1.Columns.Add(addButton);

            DataGridViewButtonColumn removeButton = new DataGridViewButtonColumn
            {
                HeaderText = "Удалить",
                Text = "-",
                Name = "DeleteButton",
                UseColumnTextForButtonValue = true
            };

            dataGridView1.Columns.Add(removeButton);
            dataGridView1.CellPainting += DataGridView1_CellPainting;

            // Загружаем данные из базы данных
            using (MySqlConnection connection = new MySqlConnection(connect))
            {
                connection.Open();
                string query = @"
                        SELECT 
                            p.ProductName AS Name,
                            op.ProductCount AS Quantity,
                            p.ProductCost AS CostPerUnit,
                            p.ProductDiscount AS Discount,
                            op.ProoductTotalCost AS TotalCost
                        FROM 
                            orderproduct op
                        JOIN 
                            product p ON op.ProductArticleNumber = p.ProductArticleNumber
                        WHERE 
                            op.OrderID = @OrderID";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderID", order.OrderID);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string productName = reader["Name"].ToString();
                        int quantity = Convert.ToInt32(reader["Quantity"]);
                        decimal costPerUnit = Convert.ToDecimal(reader["CostPerUnit"]);
                        int discount = Convert.ToInt32(reader["Discount"]);
                        decimal discountedCostPerUnit = CalculateDiscountedCost(costPerUnit, discount); // Стоимость за единицу с учетом скидки
                        decimal totalCost = discountedCostPerUnit * quantity; // Общая стоимость с учетом скидки

                        // Добавляем строки в DataGridView
                        dataGridView1.Rows.Add(
                            productName,
                            quantity,
                            costPerUnit,
                            discount,
                            totalCost
                        );

                        initialProductQuantities[productName] = quantity;
                    }
                }
            }
            UpdateTotalCost();
        }

        // Метод для отрисовки кнопок в DataGridView
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

        // Обработчик нажатия на кнопки в DataGridView
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var column = dataGridView1.Columns[e.ColumnIndex];

                if (column is DataGridViewButtonColumn)
                {
                    var row = dataGridView1.Rows[e.RowIndex];
                    string productName = row.Cells["Name"].Value.ToString();
                    int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                    decimal costPerUnit = GetCostPerUnit(productName);
                    int discount = GetDiscount(productName);
                    decimal discountedCostPerUnit = CalculateDiscountedCost(costPerUnit, discount);

                    if (column.HeaderText == "Добавить")
                    {
                        int availableQuantity = GetAvailableQuantity(productName);

                        // Определяем, сколько товара изначально было в заказе
                        int initialQuantity = initialProductQuantities.ContainsKey(productName) ? initialProductQuantities[productName] : 0;

                        // Максимальное возможное количество = доступное на складе + то, что уже в заказе
                        int maxPossibleQuantity = availableQuantity + initialQuantity;

                        if (quantity < maxPossibleQuantity)
                        {
                            quantity++;
                            row.Cells["Quantity"].Value = quantity;
                            row.Cells["TotalCost"].Value = discountedCostPerUnit * quantity;
                        }
                        else
                        {
                            MessageBox.Show($"Максимальное доступное количество товара: {maxPossibleQuantity}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    
                    if (column.HeaderText == "Удалить")
                    {
                        if (quantity > 1)
                        {
                            quantity--;
                            row.Cells["Quantity"].Value = quantity;
                            row.Cells["TotalCost"].Value = discountedCostPerUnit * quantity;
                        }
                        else
                        {
                            dataGridView1.Rows.Remove(row);
                        }
                    }

                    UpdateTotalCost(); // Обновляем общую стоимость после изменений
                }
            }
        }

        // Обработчик кнопки сохранения изменений
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // Проверка на наличие товаров в заказе
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Корзина пуста. Изменение невозможно.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Прерываем выполнение метода, если товаров нет
            }

            // Проверка заполнения всех полей
            StringBuilder errors = new StringBuilder();

            if (comboBoxUsers.SelectedItem == null)
                errors.AppendLine("Не выбран пользователь!");
            if (comboBoxClient.SelectedItem == null)
                errors.AppendLine("Не выбран клиент!");
            if (comboBoxStatus.SelectedItem == null)
                errors.AppendLine("Не выбран статус заказа!");

            if (errors.Length > 0)
            {
                MessageBox.Show($"Ошибка изменении заказа:\n{errors.ToString()}",
                              "Ошибка",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
                return;
            }

            // Подтверждение перед сохранением заказа
            DialogResult confirmResult = MessageBox.Show("Вы уверены, что хотите изменить заказ?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Подтверждение перед сохранением заказа
            if (confirmResult == DialogResult.No)
            {
                return;
            }

            using (MySqlConnection connection = new MySqlConnection(connect))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Обновляем данные заказа
                        string updateOrderQuery = @"
                                    UPDATE `order`
                                    SET 
                                        OrderDate = @OrderDate,
                                        OrderUserID = @OrderUserID,
                                        OrderClientID = @OrderClientID,
                                        OrderStatus = @OrderStatus
                                    WHERE 
                                        OrderID = @OrderID";

                        MySqlCommand command = new MySqlCommand(updateOrderQuery, connection);
                        command.Parameters.AddWithValue("@OrderDate", dateTimePicker1.Value);
                        command.Parameters.AddWithValue("@OrderUserID", GetUserId(comboBoxUsers.SelectedItem.ToString(), connection));
                        command.Parameters.AddWithValue("@OrderClientID", GetClientId(comboBoxClient.SelectedItem.ToString(), connection));
                        command.Parameters.AddWithValue("@OrderStatus", comboBoxStatus.SelectedItem.ToString());
                        command.Parameters.AddWithValue("@OrderID", order.OrderID);

                        command.ExecuteNonQuery();

                        // Удаляем старые товары из заказа
                        string deleteProductsQuery = @"DELETE FROM orderproduct WHERE OrderID = @OrderID";
                        command = new MySqlCommand(deleteProductsQuery, connection);
                        command.Parameters.AddWithValue("@OrderID", order.OrderID);
                        command.ExecuteNonQuery();

                        // Добавляем новые товары в заказ
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            string productName = row.Cells["Name"].Value?.ToString();
                            int newQuantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                            decimal totalCost = Convert.ToDecimal(row.Cells["TotalCost"].Value);

                            // Получаем исходное количество
                            int oldQuantity = initialProductQuantities.ContainsKey(productName) ? initialProductQuantities[productName] : 0;

                            int difference = newQuantity - oldQuantity; // Разница в количестве
                            string productArticleNumber = GetProductArticleNumber(productName, connection);

                            // Корректируем количество на складе
                            string updateStockQuery = @"
                                            UPDATE product
                                            SET ProductQuantityInStock = ProductQuantityInStock - @QuantityChange
                                            WHERE ProductArticleNumber = @ProductArticleNumber";

                            MySqlCommand stockCommand = new MySqlCommand(updateStockQuery, connection);
                            stockCommand.Parameters.AddWithValue("@QuantityChange", difference);
                            stockCommand.Parameters.AddWithValue("@ProductArticleNumber", productArticleNumber);
                            stockCommand.ExecuteNonQuery();

                            // Добавляем обновленные товары в заказ
                            string insertProductQuery = @"
                                            INSERT INTO orderproduct (OrderID, ProductArticleNumber, ProductCount, ProoductTotalCost)
                                            VALUES (@OrderID, @ProductArticleNumber, @ProductCount, @ProoductTotalCost)";

                            MySqlCommand comand = new MySqlCommand(insertProductQuery, connection);
                            comand.Parameters.AddWithValue("@OrderID", order.OrderID);
                            comand.Parameters.AddWithValue("@ProductArticleNumber", productArticleNumber);
                            comand.Parameters.AddWithValue("@ProductCount", newQuantity);
                            comand.Parameters.AddWithValue("@ProoductTotalCost", totalCost);

                            comand.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        MessageBox.Show("Заказ успешно обновлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Ошибка при обновлении заказа: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Получение ID пользователя по ФИО
        private int GetUserId(string fullName, MySqlConnection connection)
        {
            string[] names = fullName.Split(' ');
            string query = "SELECT UserID FROM user WHERE UserSurname = @Surname AND UserName = @Name;";
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Surname", names[0]);
            command.Parameters.AddWithValue("@Name", names[1]);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        // Получение ID клиента по ФИО
        private int GetClientId(string fullName, MySqlConnection connection)
        {
            string[] names = fullName.Split(' ');
            string query = "SELECT ClientID FROM client WHERE ClientSurname = @Surname AND ClientName = @Name;";
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Surname", names[0]);
            command.Parameters.AddWithValue("@Name", names[1]);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        // Получение артикула товара по названию
        private string GetProductArticleNumber(string productName, MySqlConnection connection)
        {
            string query = "SELECT ProductArticleNumber FROM product WHERE ProductName = @ProductName;";
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductName", productName);
            return command.ExecuteScalar()?.ToString();
        }

        // Получение стоимости товара за единицу
        private decimal GetCostPerUnit(string productName)
        {
            using (MySqlConnection connection = new MySqlConnection(connect))
            {
                connection.Open();
                string query = "SELECT ProductCost FROM product WHERE ProductName = @ProductName;";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductName", productName);
                return Convert.ToDecimal(command.ExecuteScalar());
            }
        }

        // Получение скидки на товар
        private int GetDiscount(string productName)
        {
            using (MySqlConnection connection = new MySqlConnection(connect))
            {
                connection.Open();
                string query = "SELECT ProductDiscount FROM product WHERE ProductName = @ProductName;";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductName", productName);
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        // Получение доступного количества товара на складе
        private int GetAvailableQuantity(string productName)
        {
            using (MySqlConnection connection = new MySqlConnection(connect))
            {
                connection.Open();
                string query = "SELECT ProductQuantityInStock FROM product WHERE ProductName = @ProductName;";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductName", productName);
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        // Обновление общей стоимости заказа
        private void UpdateTotalCost()
        {
            decimal totalCost = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["TotalCost"].Value != null)
                {
                    totalCost += Convert.ToDecimal(row.Cells["TotalCost"].Value);
                }
            }

            labelTotalPrice.Text = $"Общая стоимость: {totalCost:C}";
        }

        // Расчет стоимости с учетом скидки
        private decimal CalculateDiscountedCost(decimal cost, int discount)
        {
            return cost * (100 - discount) / 100;
        }      
        
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
