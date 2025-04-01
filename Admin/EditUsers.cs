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
using static MagazinTechniki.AdminUsers;

namespace MagazinTechniki
{
    public partial class EditUsers : Form
    {
        public EditUsers(Users user)
        {
            InitializeComponent();

            selectedUsers = user; // сохраняем выбранного пользователя
            EditUsers_Load(); // загружаем данные пользователя в форму
        }

        string connect = Connect.conn;
        private Users selectedUsers; // текущий редактируемый пользователь

        // Загрузка данных пользователя в форму
        private void EditUsers_Load()
        {
            // Заполнение полей данными пользователя
            textboxSurname.Text = selectedUsers.Surname;
            textBoxName.Text = selectedUsers.Name;
            textBoxPatronymic.Text = selectedUsers.Patronymic;

            LoadRole(); // Загрузка списка ролей

            comboBoxRole.Text = selectedUsers.RoleName; // Установка текущей роли
        }

        // Обработчик кнопки сохранения изменений
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // Проверка, заполнены ли все поля
            if (string.IsNullOrWhiteSpace(textboxSurname.Text) ||
                string.IsNullOrWhiteSpace(textBoxName.Text) ||
                string.IsNullOrWhiteSpace(textBoxPatronymic.Text) ||
                string.IsNullOrWhiteSpace(comboBoxRole.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Запрос подтверждения у пользователя
            DialogResult result = MessageBox.Show("Вы уверены, что хотите сохранить изменения?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Обновляем данные пользователя
                selectedUsers.Surname = textboxSurname.Text;
                selectedUsers.Name = textBoxName.Text;
                selectedUsers.Patronymic = textBoxPatronymic.Text;
                selectedUsers.RoleName = comboBoxRole.Text;

                // Обновляем данные пользователя в базе данных
                UpdateUsersInDatabase(selectedUsers);

                // Закрываем форму
                this.Close();
            }
        }

        // Обновление данных пользователя в базе данных
        private void UpdateUsersInDatabase(Users user)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connect))
                {
                    connection.Open();

                    // SQL-запрос на обновление с подзапросом для RoleID
                    string updateQuery = @"
                    UPDATE user
                    SET 
                        UserSurname = @Surname, 
                        UserName = @Name,
                        UserPatronymic = @Patronymic,

                        UserRole = (SELECT RoleID FROM role WHERE RoleName = @RoleName)
                    WHERE UserID = @UserID";

                    MySqlCommand command = new MySqlCommand(updateQuery, connection);

                    // Добавление параметров
                    command.Parameters.AddWithValue("@Surname", user.Surname);
                    command.Parameters.AddWithValue("@Name", user.Name);
                    command.Parameters.AddWithValue("@Patronymic", user.Patronymic);
                    command.Parameters.AddWithValue("@RoleName", user.RoleName);
                    command.Parameters.AddWithValue("@UserID", user.UserID);

                    // Выполнение запроса
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Данные пользователя успешно обновлены.");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при обновлении данных пользователя.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении данных пользователя: " + ex.Message);
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
        }

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

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
