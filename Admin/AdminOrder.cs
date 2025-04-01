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

namespace MagazinTechniki
{
    public partial class AdminOrder : Form
    {     
        public AdminOrder()
        {
            InitializeComponent();
            labelSurname.Text = CurrentUser.Surname; // отображение фамилии текущего пользователя
        }

        // Класс для хранения информации о заказе
        public class Orders
        {
            public int OrderID { get; set; }
            public DateTime Date { get; set; }
            public string UserSurname { get; set; } 
            public string ClientSurname { get; set; } 
            public string Status { get; set; }

        }
        
        string connect = Connect.conn;
        private List<Orders> orders = new List<Orders>(); // Список заказов
        private BindingSource bindingSource = new BindingSource(); // Источник данных для гридвью

        private void AdminOrder_Load(object sender, EventArgs e)
        {
            // Настройка текста поиска
            textBoxPoisk.Text = "Поиск";
            textBoxPoisk.ForeColor = SystemColors.GrayText;

            // Настройка привязки данных
            bindingSource.DataSource = orders;
            dataGridView1.DataSource = bindingSource;

            // Загрузка заказов из БД
            ListOrdersFromDatabase();

            // Настройка DataGridView
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            // Добавление колонок
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn{HeaderText = "ID", DataPropertyName = "OrderID", Visible = false});
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Дата", DataPropertyName = "Date"});
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Сотрудник", DataPropertyName = "UserSurname" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Клиент", DataPropertyName = "ClientSurname" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Статус", DataPropertyName = "Status" });

            // Подписка на события
            textBoxPoisk.TextChanged += textBoxPoisk_TextChanged;
            textBoxPoisk.Enter += textBoxPoisk_Enter;
            textBoxPoisk.Leave += textBoxPoisk_Leave;

            // Настройка фильтра по статусам
            comboBoxFilter.Items.Add("Все статусы");
            comboBoxFilter.Items.Add("Оформлен");
            comboBoxFilter.Items.Add("Завершен");
            comboBoxFilter.SelectedIndex = 0;   
            comboBoxFilter.SelectedIndexChanged += comboBoxFilter_SelectedIndexChanged;

            // Блокировка кнопок
            buttonDelete.Enabled = false;
            buttonRedact.Enabled = false;
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;

            // Подписка на события кнопок
            buttonDelete.Click += buttonDelete_Click;

            // Применение фильтров
            FilterData();
        }

        // Загрузка заказов из базы данных
        private void ListOrdersFromDatabase()
        {
            orders.Clear();
            using (MySqlConnection connection = new MySqlConnection(connect))
            {
                connection.Open();

                // SQL-запрос с JOIN для получения данных о заказах
                string query = @"
                    SELECT 
                        o.OrderID,
                        o.OrderDate AS Date,
                        CONCAT(u.UserSurname, ' ', u.UserName) AS UserFullName, -- Полное имя сотрудника
                        CONCAT(c.ClientSurname, ' ', c.ClientName) AS ClientFullName, -- Полное имя клиента
                        o.OrderStatus AS Status
                    FROM `order` o
                    LEFT JOIN `user` u ON o.OrderUserID = u.UserID
                    LEFT JOIN `client` c ON o.OrderClientID = c.ClientID";

                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    // Проверка на NULL значения
                    if (reader["OrderID"] != DBNull.Value &&
                        reader["Date"] != DBNull.Value &&
                        reader["UserFullName"] != DBNull.Value &&
                        reader["ClientFullName"] != DBNull.Value &&
                        reader["Status"] != DBNull.Value)
                    {
                        orders.Add(new Orders
                        {
                            OrderID = Convert.ToInt32(reader["OrderID"]),
                            Date = Convert.ToDateTime(reader["Date"]),
                            UserSurname = reader["UserFullName"].ToString(),
                            ClientSurname = reader["ClientFullName"].ToString(), 
                            Status = reader["Status"].ToString()
                        });
                    }
                }
                reader.Close();
            }
            bindingSource.ResetBindings(false);
            FilterData();

            dataGridView1.ClearSelection();
        }

        // Обработчик изменения выбора в DataGridView
        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            bool isRowSelected = dataGridView1.SelectedRows.Count > 0;
            buttonDelete.Enabled = isRowSelected;

            // Проверяем статус выбранного заказа для кнопки редактирования
            if (isRowSelected)
            {
                Orders selectedOrder = GetSelectedOrder();
                buttonRedact.Enabled = selectedOrder != null && selectedOrder.Status == "Оформлен";
            }
            else
            {
                buttonRedact.Enabled = false;
            }
        }

        // Фильтрация данных по статусу и поисковому запросу
        private void FilterData()
        {
            string searchText = textBoxPoisk.Text.Trim().ToLower();
            string selectedStatus = comboBoxFilter.SelectedItem?.ToString();

            var filteredOrders = orders;

            // Фильтрация по статусу
            if (selectedStatus != "Все статусы")
            {
                filteredOrders = filteredOrders
                    .Where(o => o.Status == selectedStatus)
                    .ToList();
            }

            // Фильтрация по поисковому запросу
            if (!string.IsNullOrEmpty(searchText) && searchText != "поиск")
            {
                filteredOrders = filteredOrders
                    .Where(o => 
                                o.UserSurname.ToLower().Contains(searchText) ||
                                o.ClientSurname.ToLower().Contains(searchText))
                    .ToList();
            }

            bindingSource.DataSource = filteredOrders;
            bindingSource.ResetBindings(false);
            dataGridView1.Refresh();
        }    
        
        private void comboBoxFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterData();
        }

        private void textBoxPoisk_TextChanged(object sender, EventArgs e)
        {
            FilterData();
        }

        private void textBoxPoisk_Enter(object sender, EventArgs e)
        {
            if (textBoxPoisk.Text == "Поиск")
            {
                textBoxPoisk.Text = "";
                textBoxPoisk.ForeColor = Color.Black;
            }
        }

        private void textBoxPoisk_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxPoisk.Text))
            {
                textBoxPoisk.Text = "Поиск";
                textBoxPoisk.ForeColor = Color.Gray;

                FilterData();
            }
        }        
        
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            AdminAddOrder form = new AdminAddOrder();
            form.ShowDialog();

            ListOrdersFromDatabase(); 
            FilterData();
        }

        // Получение выбранного заказа
        private Orders GetSelectedOrder()
        {
            if (dataGridView1.CurrentRow == null || dataGridView1.CurrentRow.DataBoundItem == null)
                return null;

            return dataGridView1.CurrentRow.DataBoundItem as Orders;
        }

        // Обработчик кнопки редактирования
        private void buttonRedact_Click(object sender, EventArgs e)
        {
            // Получаем выбранный заказ
            Orders selectedOrder = GetSelectedOrder();

            if (selectedOrder == null)
            {
                MessageBox.Show("Пожалуйста, выберите заказ для редактирования.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Проверяем статус заказа
            if (selectedOrder.Status != "Оформлен")
            {
                MessageBox.Show("Редактирование возможно только для заказов со статусом 'Оформлен'.\n" +
                              "Этот заказ уже завершен и не может быть отредактирован.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Открываем форму EditOrder и передаем выбранный заказ
            EditOrder editForm = new EditOrder(selectedOrder);
            editForm.ShowDialog();

            // Обновляем список заказов после редактирования
            ListOrdersFromDatabase();
            FilterData();
        }

        // Обработчик кнопки удаления
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            Orders selectedOrder = GetSelectedOrder();

            if (selectedOrder == null)
            {
                MessageBox.Show("Пожалуйста, выберите заказ для удаления.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Подтверждение удаления
            DialogResult result = MessageBox.Show(
                $"Вы уверены, что хотите удалить заказ №{selectedOrder.OrderID} от {selectedOrder.Date:d}?",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            try
            {
                bool deletionSuccess = false;

                using (MySqlConnection connection = new MySqlConnection(connect))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction()) // Транзакция
                    {
                        try
                        {
                            // Получаем список товаров в заказе для возврата на склад
                            string selectProductsQuery = @"
                                        SELECT ProductArticleNumber, ProductCount 
                                        FROM orderproduct 
                                        WHERE OrderID = @OrderID";

                            Dictionary<string, int> productRestock = new Dictionary<string, int>();

                            using (MySqlCommand selectCmd = new MySqlCommand(selectProductsQuery, connection))
                            {
                                selectCmd.Parameters.AddWithValue("@OrderID", selectedOrder.OrderID);
                                using (MySqlDataReader reader = selectCmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        string articleNumber = reader["ProductArticleNumber"].ToString();
                                        int quantity = Convert.ToInt32(reader["ProductCount"]);
                                        productRestock[articleNumber] = quantity;
                                    }
                                }
                            }

                            // Возвращаем товары на склад
                            foreach (var item in productRestock)
                            {
                                string updateStockQuery = @"
                                    UPDATE product 
                                    SET ProductQuantityInStock = ProductQuantityInStock + @Quantity 
                                    WHERE ProductArticleNumber = @ProductArticleNumber";

                                using (MySqlCommand updateCmd = new MySqlCommand(updateStockQuery, connection))
                                {
                                    updateCmd.Parameters.AddWithValue("@Quantity", item.Value);
                                    updateCmd.Parameters.AddWithValue("@ProductArticleNumber", item.Key);
                                    updateCmd.ExecuteNonQuery();
                                }
                            }

                            // Удаляем товары из заказа
                            string deleteProductsQuery = @"DELETE FROM orderproduct WHERE OrderID = @OrderID";
                            using (MySqlCommand cmd = new MySqlCommand(deleteProductsQuery, connection))
                            {
                                cmd.Parameters.AddWithValue("@OrderID", selectedOrder.OrderID);
                                cmd.ExecuteNonQuery();
                            }

                            // Удаляем сам заказ
                            string deleteOrderQuery = @"DELETE FROM `order` WHERE OrderID = @OrderID";
                            using (MySqlCommand cmd = new MySqlCommand(deleteOrderQuery, connection))
                            {
                                cmd.Parameters.AddWithValue("@OrderID", selectedOrder.OrderID);
                                int rowsAffected = cmd.ExecuteNonQuery();
                                deletionSuccess = rowsAffected > 0;
                            }

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show($"Ошибка при удалении заказа: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                if (deletionSuccess)
                {
                    MessageBox.Show("Заказ успешно удален, товары возвращены на склад.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    ListOrdersFromDatabase();
                    dataGridView1.ClearSelection();
                }
                else
                {
                    MessageBox.Show("Не удалось найти выбранный заказ.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении заказа: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчики переходов между формами
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AdminClient form = new AdminClient();
            form.Show();
            Hide();
        }       
        
        private void button2_Click(object sender, EventArgs e)
        {
            Admin form = new Admin();
            form.Show();
            Hide();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Autorisation form = new Autorisation();
            form.Show();
            Hide();
        }

        private void linkLabelProducts_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AdminProducts form = new AdminProducts();
            form.Show();
            Hide();
        }

        private void linkLabelUser_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AdminUsers form = new AdminUsers();
            form.Show();
            Hide();
        }

        private void linkLabelCategory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AdminSpravochnik form = new AdminSpravochnik();
            form.Show();
            Hide();
        }

        private void linkLabelOtchet_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AdminOtchet form = new AdminOtchet();
            form.ShowDialog();
        }

    }
}