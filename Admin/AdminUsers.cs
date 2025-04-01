using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MagazinTechniki.AdminProducts;

namespace MagazinTechniki
{
    public partial class AdminUsers : Form
    {
        public AdminUsers()
        {
            InitializeComponent();
            labelSurname.Text = CurrentUser.Surname; // Отображение фамилии текущего пользователя
        }

        // Класс для хранения инф-ции о пользователе
        public class Users
        {
            public int UserID { get; set; }
            public string Surname { get; set; }
            public string Name { get; set; }
            public string Patronymic { get; set; }
            public string Login { get; set; }
            public string Password { get; set; }
            public string RoleName { get; set; }
        }

        string connect = Connect.conn;
        private List<Users> users = new List<Users>(); // Спиосок пользователей

        private void AdminUsers_Load(object sender, EventArgs e)
        {
            // Настройка текста поиска
            textBoxPoisk.Text = "Поиск";
            textBoxPoisk.ForeColor = SystemColors.GrayText;

            // Загрузка пользователей из БД
            ListUsersFromDatabase();

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

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Фамилия", DataPropertyName = "Surname" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Имя", DataPropertyName = "Name" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Отчество", DataPropertyName = "Patronymic" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Логин", DataPropertyName = "Login" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Пароль", DataPropertyName = "Password"});
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Роль", DataPropertyName = "RoleName" });

            // Подписка на события
            textBoxPoisk.TextChanged += textBoxPoisk_TextChanged;
            textBoxPoisk.Enter += textBoxPoisk_Enter;
            textBoxPoisk.Leave += textBoxPoisk_Leave;

            // Блокировка кнопки редактирования до выбора строки
            buttonRedact.Enabled = false;
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
        }

        // Загрузка пользователей из базы данных
        private void ListUsersFromDatabase()
        {
            users.Clear();
            using (MySqlConnection connection = new MySqlConnection(connect))
            {
                connection.Open();

                // SQL запрос с JOIN для получения данных о пользователях и их ролях
                string query = @"
                                SELECT 
                                    u.UserID AS UserID,
                                    u.UserSurname AS Surname,
                                    u.UserName AS Name,
                                    u.UserPatronymic AS Patronymic,
                                    u.UserLogin AS Login,
                                    u.UserPassword AS Password,  
                                    r.RoleName AS RoleName
                                FROM user u
                                LEFT JOIN role r ON u.UserRole = r.RoleID";

                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    // Проверка на NULL значения
                    if (
                        reader["UserID"] != DBNull.Value &&
                        reader["Surname"] != DBNull.Value &&
                        reader["Name"] != DBNull.Value &&
                        reader["Patronymic"] != DBNull.Value &&
                        reader["Login"] != DBNull.Value &&
                        reader["Password"] != DBNull.Value &&
                        reader["RoleName"] != DBNull.Value)
                    {
                        int id = Convert.ToInt32(reader["UserID"]);
                        string surname = reader["Surname"].ToString();
                        string name = reader["Name"].ToString();
                        string patronymic = reader["Patronymic"].ToString();
                        string login = reader["Login"].ToString();
                        string password = reader["Password"].ToString();
                        string roleName = reader["RoleName"].ToString();

                        // Добавление пользователя в список
                        users.Add(new Users
                        {
                            UserID = id,
                            Surname = surname,
                            Name = name,
                            Patronymic = patronymic,
                            Login = login,
                            Password = password,
                            RoleName = roleName
                    
                        });
                    }
                }
                reader.Close();
            }
            // Обновление DataGridView
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = users;
        }

        // Блокировка/разблокировка кнопки редактирования
        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            buttonRedact.Enabled = dataGridView1.SelectedRows.Count > 0;
        }

        // Добавление нового пользователя
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            AddUsers form = new AddUsers();
            form.ShowDialog();

            ListUsersFromDatabase();
        }

        // Редактирование пользователя
        private void buttonRedact_Click(object sender, EventArgs e)
        {
            Users selectedUsers = GetSelectedUser();

            if (selectedUsers != null)
            {
                EditUsers form = new EditUsers(selectedUsers);
                form.ShowDialog();
               
                ListUsersFromDatabase();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите продукт для редактирования.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Получение выбранного пользователя
        private Users GetSelectedUser()
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                return selectedRow.DataBoundItem as Users;
            }
            return null;
        }

        // Поиск 
        private void textBoxPoisk_TextChanged(object sender, EventArgs e)
        {
            string searchText = textBoxPoisk.Text.Trim().ToLower();

            if (searchText == "Поиск" || string.IsNullOrEmpty(searchText))
            {
                // Показать всех пользователей, если строка поиска пуста
                dataGridView1.DataSource = users;
            }
            else
            {
                // Фильтрация пользователей по ФИО
                var filteredUsers = users.Where(u =>
                    u.Surname.ToLower().Contains(searchText) ||
                    u.Name.ToLower().Contains(searchText) ||
                    u.Patronymic.ToLower().Contains(searchText)
                ).ToList();

                dataGridView1.DataSource = filteredUsers;
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

                dataGridView1.DataSource = users;
            }
        }

        // Переходы на формы
        private void linkLabelProducts_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AdminProducts form = new AdminProducts();
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

        private void linkLabelOrder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AdminOrder form = new AdminOrder();
            form.Show();
            Hide();
        }

        private void linkLabelCategory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AdminSpravochnik form = new AdminSpravochnik();
            form.Show();
            Hide();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AdminClient form = new AdminClient();
            form.Show();
            Hide();
        }

    }
}