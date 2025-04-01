using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MagazinTechniki.AdminUsers;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TextBox = System.Windows.Forms.TextBox;

namespace MagazinTechniki
{
    public partial class AddUsers : Form
    {
        public AddUsers()
        {
            InitializeComponent();
        }

        private void AddUsers_Load(object sender, EventArgs e)
        {
            comboBoxRole.DropDownStyle = ComboBoxStyle.DropDownList; // Блок ручного ввода в комбобокс

            // Очистка полей ввода
            textboxSurname.Text = "";
            textBoxName.Text = "";
            textBoxPatronymic.Text = "";
            textBoxLogin.Text = "";
            textBoxPassword.Text = "";

            // Загрузка списка ролей
            LoadRole();
        }

        // Обработчик кнопки добавления сотрудника
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Создаем объект сотрудника
                Users newUsers = new Users
                {
                    Surname = textboxSurname.Text,
                    Name = textBoxName.Text,
                    Patronymic = textBoxPatronymic.Text,
                    Login = textBoxLogin.Text,
                    Password = textBoxPassword.Text,
                    RoleName = comboBoxRole.SelectedItem?.ToString()
                };

                // Проверка на пустые поля
                if (string.IsNullOrEmpty(newUsers.Surname) ||
                    string.IsNullOrEmpty(newUsers.Name) ||
                    string.IsNullOrEmpty(newUsers.Patronymic) ||
                    string.IsNullOrEmpty(newUsers.Login) ||
                    string.IsNullOrEmpty(newUsers.Password) ||
                    string.IsNullOrEmpty(newUsers.RoleName))
                {
                    MessageBox.Show("Ошибка заполнения данных!", "Заполните все поля!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Подтверждение добавления сотрудника
                DialogResult result = MessageBox.Show(
                    "Вы уверены, что хотите добавить сотрудника?",
                    "Подтверждение добавления",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                // Если пользователь подтвердил добавление
                if (result == DialogResult.Yes)
                {
                    AddUsersToDatabase(newUsers); // Добавляем сотрудника в базу данных
                    this.Close(); // Закрываем форму
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка!", "Ошибка при добавлении сотрудника: " + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Хеширование пароля с SHA256
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2")); // Преобразование в hex-строку
                }
                return builder.ToString();
            }
        }

        // Добавление пользователя в базу данных
        private void AddUsersToDatabase(Users users)
        {
            try
            {
                string connect = Connect.conn;

                string hashedPassword = HashPassword(users.Password);

                // SQL-запрос с подзапросом для получения RoleID
                string insertQuery = @"
                        INSERT INTO user (
                            UserSurname, 
                            UserName, 
                            UserPatronymic, 
                            UserLogin, 
                            UserPassword, 
                            UserRole                          
                        ) VALUES (
                            @UserSurname, 
                            @UserName, 
                            @UserPatronymic, 
                            @UserLogin, 
                            @UserPassword, 
                            (SELECT RoleID FROM role WHERE RoleName = @UserRoleName)
                        )";

                using (MySqlConnection connection = new MySqlConnection(connect))
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand(insertQuery, connection);

                    // Добавление параметров
                    command.Parameters.AddWithValue("@UserSurname", users.Surname);
                    command.Parameters.AddWithValue("@UserName", users.Name);
                    command.Parameters.AddWithValue("@UserPatronymic", users.Patronymic);
                    command.Parameters.AddWithValue("@UserLogin", users.Login); 
                    command.Parameters.AddWithValue("@UserPassword", hashedPassword); // Используем хешированный пароль
                    command.Parameters.AddWithValue("@UserRoleName", users.RoleName);

                    // Выполнение запроса
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Сотрудник успешно добавлен в базу данных.");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при добавлении сотрудника в базу данных.");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка при добавлении сотрудника в базу данных: " + e.Message);
            }
        }

        // Загрузка списка ролей из базы данных
        private void LoadRole()
        {
            using (MySqlConnection connection = new MySqlConnection(Connect.conn))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand("SELECT RoleName FROM role", connection);
                MySqlDataReader reader = command.ExecuteReader();

                comboBoxRole.Items.Clear();

                while (reader.Read())
                {
                    comboBoxRole.Items.Add(reader["RoleName"].ToString());
                }

                reader.Close();
            }

            if (comboBoxRole.Items.Count > 0)
            {
                comboBoxRole.SelectedIndex = 0; // Выбираем первую роль по умолчанию
            }
        }

        // Автоматическая капитализация первой буквы
        private void CapitalizeFirstLetter(TextBox textBox)
        {
            if (!string.IsNullOrEmpty(textBox.Text))
            {
                string text = textBox.Text;
                text = char.ToUpper(text[0]) + text.Substring(1);
                textBox.Text = text;
                textBox.SelectionStart = textBox.Text.Length;
            }
        }
        private void textboxSurname_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешаем только русские буквы и управляющие клавиши (например, Backspace)
            if (!(e.KeyChar >= 'А' && e.KeyChar <= 'Я') && !(e.KeyChar >= 'а' && e.KeyChar <= 'я') && e.KeyChar != 'Ё' && e.KeyChar != 'ё' && e.KeyChar != ' ' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Игнорируем ввод
            }
        }

        private void textboxSurname_TextChanged(object sender, EventArgs e)
        {
            CapitalizeFirstLetter(textboxSurname);
        }

        private void textBoxName_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешаем только русские буквы и управляющие клавиши (например, Backspace)
            if (!(e.KeyChar >= 'А' && e.KeyChar <= 'Я') && !(e.KeyChar >= 'а' && e.KeyChar <= 'я') && e.KeyChar != 'Ё' && e.KeyChar != 'ё' && e.KeyChar != ' ' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Игнорируем ввод
            }
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            CapitalizeFirstLetter(textBoxName);
        }

        private void textBoxPatronymic_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешаем только русские буквы и управляющие клавиши (например, Backspace)
            if (!(e.KeyChar >= 'А' && e.KeyChar <= 'Я') && !(e.KeyChar >= 'а' && e.KeyChar <= 'я') && e.KeyChar != 'Ё' && e.KeyChar != 'ё' && e.KeyChar != ' ' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Игнорируем ввод
            }
        }

        private void textBoxPatronymic_TextChanged(object sender, EventArgs e)
        {
            CapitalizeFirstLetter(textBoxPatronymic);
        }

        private void login_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetterOrDigit(e.KeyChar) || e.KeyChar == '_' || e.KeyChar == '-' || char.IsControl(e.KeyChar)))
            {
                e.Handled = true;
            }

            if (e.KeyChar >= 'А' && e.KeyChar <= 'я' || e.KeyChar == 'ё' || e.KeyChar == 'Ё')
            {
                e.Handled = true;
            }

        }

        private void pass_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= 'а' && e.KeyChar <= 'я') || (e.KeyChar >= 'А' && e.KeyChar <= 'Я') || e.KeyChar == ' ')
            {
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
