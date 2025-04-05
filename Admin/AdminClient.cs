using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MagazinTechniki
{
    public partial class AdminClient : Form
    {
        public AdminClient()
        {
            InitializeComponent();
            labelSurname.Text = CurrentUser.Surname; // отображение фамилии текущего пользователя
        }

        // класс для хранения инф-ции о клиенте
        public class Client
        {
            public int ClientID { get; set; }
            public string ClientName { get; set; }
            public string ClientSurname { get; set; }
            public string ClientTelephone { get; set; }
            public string ClientEmail { get; set; }

             // Свойства для отображения (только для чтения)
            
            public string DisplayName => !string.IsNullOrEmpty(ClientName) ? $"{ClientName[0]}." : "";
            public string DisplayTelephone => !string.IsNullOrEmpty(ClientTelephone) ? 
                $"+7 (***) ***-{ClientTelephone.Substring(Math.Max(0, ClientTelephone.Length - 5))}" : "";
}


        string connect = Connect.conn;
        private List<Client> clients = new List<Client>(); // Список клиентов

        private void AdminClient_Load(object sender, EventArgs e)
        {
            // Настройка текста поиска
            textBoxPoisk.Text = "Поиск";
            textBoxPoisk.ForeColor = SystemColors.GrayText;

            // Загрузка клиентов из БД
            ListClientsFromDatabase();

            // Настройка DataGridView
            dataGridView1.Dock = DockStyle.None;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Добавление колонок
            dataGridView1.Columns.Clear();
            
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Фамилия", DataPropertyName = "ClientSurname" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Имя", DataPropertyName = "DisplayName" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Телефон", DataPropertyName = "DisplayTelephone" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Email", DataPropertyName = "ClientEmail" });

            // Подписка на события
            textBoxPoisk.TextChanged += textBoxPoisk_TextChanged;
            textBoxPoisk.Enter += textBoxPoisk_Enter;
            textBoxPoisk.Leave += textBoxPoisk_Leave;

            // Блокировка кнопок до выбора строки
            buttonRedact.Enabled = false;
            buttonDelete.Enabled = false;
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;

            dataGridView1.DoubleClick += DataGridView1_DoubleClick;
        }

        // Загрузка клиентов из базы данных
        private void ListClientsFromDatabase()
        {
            clients.Clear();
            using (MySqlConnection connection = new MySqlConnection(connect))
            {
                connection.Open();

                // SQL запрос для получения данных о клиентах
                string query = @"
                    SELECT 
                        ClientID,
                        ClientName,
                        ClientSurname,
                        ClientTelephone,
                        ClientEmail
                    FROM client";

                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    // Проверка на NULL значения
                    if (
                        reader["ClientID"] != DBNull.Value &&
                        reader["ClientName"] != DBNull.Value &&
                        reader["ClientSurname"] != DBNull.Value &&
                        reader["ClientTelephone"] != DBNull.Value &&
                        reader["ClientEmail"] != DBNull.Value)
                    {
                        int id = Convert.ToInt32(reader["ClientID"]);
                        string name = reader["ClientName"].ToString();
                        string surname = reader["ClientSurname"].ToString();
                        string telephone = reader["ClientTelephone"].ToString();
                        string email = reader["ClientEmail"].ToString();

                        // Добавление клиента в список
                        clients.Add(new Client
                        {
                            ClientID = id,
                            ClientName = name,
                            ClientSurname = surname,
                            ClientTelephone = telephone,
                            ClientEmail = email
                        });
                    }
                }
                reader.Close();
            }
            // Обновление DataGridView
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = clients;
        }

        // Блокировка/разблокировка кнопок при выборе строки
        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            buttonRedact.Enabled = dataGridView1.SelectedRows.Count > 0;
            buttonDelete.Enabled = dataGridView1.SelectedRows.Count > 0;
        }

        // Получение выбранного клиента
        private Client GetSelectedClient()
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                return selectedRow.DataBoundItem as Client;
            }
            return null;
        }

        // Поиск 
        private void textBoxPoisk_TextChanged(object sender, EventArgs e)
        {
            string searchText = textBoxPoisk.Text.Trim().ToLower();

            if (searchText == "Поиск" || string.IsNullOrEmpty(searchText))
            {
                // Показать всех клиентов, если строка поиска пуста
                dataGridView1.DataSource = clients;
            }
            else
            {
                // Фильтрация клиентов по всем полям
                var filteredClients = clients.Where(c =>
                    c.ClientName.ToLower().Contains(searchText) ||
                    c.ClientSurname.ToLower().Contains(searchText) ||
                    c.ClientTelephone.ToLower().Contains(searchText) ||
                    c.ClientEmail.ToLower().Contains(searchText)
                ).ToList();

                dataGridView1.DataSource = filteredClients;
            }
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

                dataGridView1.DataSource = clients;
            }
        }

        // Добавление нового клиента
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            AddClients form = new AddClients();
            form.ShowDialog();

            ListClientsFromDatabase();
        }

        // Редактирование клиента
        private void buttonRedact_Click(object sender, EventArgs e)
        {
            Client selectedClient = GetSelectedClient();

            if (selectedClient != null)
            {
                EditClients form = new EditClients(selectedClient);
                form.ShowDialog();

                ListClientsFromDatabase();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите клиента для редактирования.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Удаление клиента
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            Client selectedClient = GetSelectedClient();

            if (selectedClient != null)
            {
                // Запрос подтверждения удаления
                DialogResult result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить клиента '{selectedClient.ClientName} {selectedClient.ClientSurname}'?",
                    "Подтверждение удаления",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        using (MySqlConnection connection = new MySqlConnection(connect))
                        {
                            connection.Open();

                            // SQL-запрос для удаления клиента
                            string deleteQuery = "DELETE FROM client WHERE ClientID = @ClientID";

                            MySqlCommand command = new MySqlCommand(deleteQuery, connection);
                            command.Parameters.AddWithValue("@ClientID", selectedClient.ClientID);

                            // Выполняем запрос
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Клиент успешно удален.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Обновляем список клиентов после удаления
                                ListClientsFromDatabase();
                            }
                            else
                            {
                                MessageBox.Show("Ошибка при удалении клиента.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show($"Ошибка при удалении клиента: Этот клиент есть в заказе.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите клиента для удаления.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DataGridView1_DoubleClick(object sender, EventArgs e)
        {
            // Получаем выбранного клиента
            Client selectedClient = GetSelectedClient();

            if (selectedClient != null)
            {
                // Создаем и показываем форму с подробной информацией
                AdminPersonalInfoClient infoForm = new AdminPersonalInfoClient(selectedClient);
                infoForm.ShowDialog();
            }
        }

        // переходы на формы
        private void linkLabelOrder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AdminOrder form = new AdminOrder();
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

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Autorisation form = new Autorisation();
            form.Show();
            Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Admin form = new Admin();
            form.Show();
            Hide();
        }

    }
}