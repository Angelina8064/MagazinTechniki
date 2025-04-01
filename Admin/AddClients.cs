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
using static MagazinTechniki.AdminClient;

namespace MagazinTechniki
{
    public partial class AddClients : Form
    {
        public AddClients()
        {
            InitializeComponent();

            // Подписка на события проверки ввода
            textBoxName.KeyPress += textBoxName_KeyPress;
            textBoxSurname.KeyPress += textBoxSurname_KeyPress;
            textBoxEmail.KeyPress += textBoxEmail_KeyPress;
        }

        // Инициализация формы при загрузке
        private void AddClients_Load(object sender, EventArgs e)
        {
            // Очистка полей ввода
            textBoxName.Text = "";
            textBoxSurname.Text = "";
            maskedTextBoxTelephone.Mask = "+7 (000) 000-00-00"; // Установка маски для телефона
            textBoxEmail.Text = "";
        }

        // Обработчик кнопки добавления клиента
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Создаем объект клиента
                Client newClient = new Client
                {
                    ClientName = textBoxName.Text,
                    ClientSurname = textBoxSurname.Text,
                    ClientTelephone = maskedTextBoxTelephone.Text, // Используем MaskedTextBox
                    ClientEmail = textBoxEmail.Text
                };

                if (string.IsNullOrEmpty(newClient.ClientName) ||
                    string.IsNullOrEmpty(newClient.ClientSurname) ||
                    string.IsNullOrEmpty(newClient.ClientTelephone) ||
                    string.IsNullOrEmpty(newClient.ClientEmail))
                {
                    MessageBox.Show("Ошибка заполнения данных!", "Заполните все поля!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Проверка корректности номера телефона
                if (!maskedTextBoxTelephone.MaskCompleted)
                {
                    MessageBox.Show("Номер телефона введен не полностью!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Проверка формата email
                if (!IsValidEmail(newClient.ClientEmail))
                {
                    MessageBox.Show("Некорректный формат email!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Подтверждение добавления клиента
                DialogResult result = MessageBox.Show(
                    "Вы уверены, что хотите добавить клиента?",
                    "Подтверждение добавления",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                // Если пользователь подтвердил добавление
                if (result == DialogResult.Yes)
                {
                    AddClientToDatabase(newClient); // Добавляем клиента в базу данных
                    this.Close(); // Закрываем форму
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка!", "Ошибка при добавлении клиента: " + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Добавление клиента в базу данных
        private void AddClientToDatabase(Client client)
        {
            try
            {
                string connect = Connect.conn;

                // SQL-запрос на добавление
                string insertQuery = @"
                    INSERT INTO client (
                        ClientName, 
                        ClientSurname, 
                        ClientTelephone, 
                        ClientEmail
                    ) VALUES (
                        @ClientName, 
                        @ClientSurname, 
                        @ClientTelephone, 
                        @ClientEmail
                    )";

                using (MySqlConnection connection = new MySqlConnection(connect))
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand(insertQuery, connection);

                    // Добавление параметров
                    command.Parameters.AddWithValue("@ClientName", client.ClientName);
                    command.Parameters.AddWithValue("@ClientSurname", client.ClientSurname);
                    command.Parameters.AddWithValue("@ClientTelephone", client.ClientTelephone);
                    command.Parameters.AddWithValue("@ClientEmail", client.ClientEmail);

                    // Выполнение запроса
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Клиент успешно добавлен в базу данных.");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при добавлении клиента в базу данных.");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка при добавлении клиента в базу данных: " + e.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void textBoxSurname_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешаем только русские буквы и управляющие клавиши (например, Backspace)
            if (!char.IsControl(e.KeyChar) && !(e.KeyChar >= 'А' && e.KeyChar <= 'я') && e.KeyChar != 'Ё' && e.KeyChar != 'ё')
            {
                e.Handled = true; // Игнорируем ввод
            }
        }
        private void textBoxSurname_TextChanged(object sender, EventArgs e)
        {
            CapitalizeFirstLetter(textBoxSurname);
        }

        private void textBoxName_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешаем только русские буквы и управляющие клавиши (например, Backspace)
            if (!char.IsControl(e.KeyChar) && !(e.KeyChar >= 'А' && e.KeyChar <= 'я') && e.KeyChar != 'Ё' && e.KeyChar != 'ё')
            {
                e.Handled = true; // Игнорируем ввод
            }
        }
        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            CapitalizeFirstLetter(textBoxName);
        }

        // Проверка валидности email адреса
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);

                // Дополнительные проверки:
                // 1. Email должен содержать ровно один символ '@'
                // 2. Должна быть точка после '@'
                // 3. Домен должен быть не короче 2 символов
                return addr.Address == email &&
                       email.Count(c => c == '@') == 1 &&
                       email.Split('@')[1].Contains(".") &&
                       email.Split('@')[1].Split('.')[0].Length >= 1 &&
                       email.Split('@')[1].Split('.')[1].Length >= 2;
            }
            catch
            {
                return false;
            }
        }

        private void textBoxEmail_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Проверяем на русские буквы
            bool isRussianLetter = (e.KeyChar >= 'а' && e.KeyChar <= 'я') ||
                                 (e.KeyChar >= 'А' && e.KeyChar <= 'Я') ||
                                 e.KeyChar == 'ё' || e.KeyChar == 'Ё';

            // Разрешенные символы для email
            bool isEnglishLetter = (e.KeyChar >= 'a' && e.KeyChar <= 'z') ||
                                 (e.KeyChar >= 'A' && e.KeyChar <= 'Z');
            bool isDigit = char.IsDigit(e.KeyChar);
            bool isAllowedSymbol = e.KeyChar == '.' || e.KeyChar == '@' ||
                                 e.KeyChar == '_' || e.KeyChar == '-';
            bool isControl = char.IsControl(e.KeyChar);

            // Блокируем русские буквы и другие неразрешенные символы
            if (isRussianLetter || (!isEnglishLetter && !isDigit &&
                                   !isAllowedSymbol && !isControl))
            {
                e.Handled = true;
            }
        }

        // Дополнительная фильтрация email при изменении текста
        private void textBoxEmail_TextChanged(object sender, EventArgs e)
        {
            string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@.";
            string text = textBoxEmail.Text;
            string newText = "";

            foreach (char c in text)
            {
                if (validChars.Contains(c))
                {
                    newText += c;
                }
            }

            if (newText != text)
            {
                textBoxEmail.Text = newText;
                textBoxEmail.SelectionStart = textBoxEmail.Text.Length;
            }
        }
    }
}
