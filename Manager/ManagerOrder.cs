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
    public partial class ManagerOrder : Form
    {
        public ManagerOrder()
        {
            InitializeComponent();
            labelSurname.Text = CurrentUser.Surname;  // Отображение фамилии текущего пользователя
        }

        string connect = Connect.conn;
        private List<Orders> orders = new List<Orders>(); // Список заказов
        private BindingSource bindingSource = new BindingSource(); // Источник данных для DataGridView

        private void ManagerOrder_Load(object sender, EventArgs e)
        {
            // Настройка поиска
            textBoxPoisk.Text = "Поиск";
            textBoxPoisk.ForeColor = SystemColors.GrayText;

            bindingSource.DataSource = orders;
            dataGridView1.DataSource = bindingSource;

            ListOrdersFromDatabase();

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

            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID", DataPropertyName = "OrderID", Visible = false });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Дата", DataPropertyName = "Date" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Сотрудник", DataPropertyName = "UserSurname" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Клиент", DataPropertyName = "ClientSurname" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Статус", DataPropertyName = "Status" });

            textBoxPoisk.TextChanged += textBoxPoisk_TextChanged;
            textBoxPoisk.Enter += textBoxPoisk_Enter;
            textBoxPoisk.Leave += textBoxPoisk_Leave;

            // Настройка фильтра по статусу
            comboBoxFilter.Items.Add("Все статусы");
            comboBoxFilter.Items.Add("Оформлен");
            comboBoxFilter.Items.Add("Завершен");
            comboBoxFilter.SelectedIndex = 0;
            comboBoxFilter.SelectedIndexChanged += comboBoxFilter_SelectedIndexChanged;

            buttonDelete.Enabled = false;
            buttonRedact.Enabled = false;
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;

            buttonDelete.Click += buttonDelete_Click;

            FilterData();
        }

        // Загрузка заказов из БД
        private void ListOrdersFromDatabase()
        {
            orders.Clear();
            using (MySqlConnection connection = new MySqlConnection(connect))
            {
                connection.Open();

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
                            UserSurname = reader["UserFullName"].ToString(), // Теперь это полное имя
                            ClientSurname = reader["ClientFullName"].ToString(), // Теперь это полное имя
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

        // Фильтрация данных
        private void FilterData()
        {
            string searchText = textBoxPoisk.Text.Trim().ToLower();
            string selectedStatus = comboBoxFilter.SelectedItem?.ToString();

            var filteredOrders = orders;

            // Фильтр по статусу
            if (selectedStatus != "Все статусы")
            {
                filteredOrders = filteredOrders
                    .Where(o => o.Status == selectedStatus)
                    .ToList();
            }

            // Фильтр по поиску
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

        // Добавление нового заказа
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

        // Редактирование заказа
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

        // Удаление заказа с возвратом товаров на склад
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            Orders selectedOrder = GetSelectedOrder();

            if (selectedOrder == null)
            {
                MessageBox.Show("Пожалуйста, выберите заказ для удаления.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Получаем список товаров в заказе и их количество
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

        #region переходы на формы
        private void button2_Click(object sender, EventArgs e)
        {
            Manager form = new Manager();
            form.Show();
            Hide();
        }
        private void buttonExit_Click(object sender, EventArgs e)
        {
            Autorisation form = new Autorisation();
            form.Show();
            Hide();
        }
        private void linkLabelUser_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ManagerClients form = new ManagerClients();
            form.Show();
            Hide();
        }
        private void linkLabelProducts_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ManagerProducts form = new ManagerProducts();
            form.Show();
            Hide();
        }
        private void linkLabelOtchet_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AdminOtchet form = new AdminOtchet();
            form.ShowDialog();
        }
        #endregion

    }
}
