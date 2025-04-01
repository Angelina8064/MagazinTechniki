using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MagazinTechniki.AdminClient;

namespace MagazinTechniki
{
    public partial class EditClients : Form
    {
        public EditClients(Client client)
        {
            InitializeComponent();

            selectedClient = client; // Сохраняем выбранного клиента 
            EditClients_Load(); // Загружаем данные клиента

            // Подписка на события
            textBoxName.KeyPress += textBoxName_KeyPress;
            textBoxSurname.KeyPress += textBoxSurname_KeyPress;
            textBoxEmail.KeyPress += textBoxEmail_KeyPress;
        }
        
        string connect = Connect.conn;
        private Client selectedClient;

        private void EditClients_Load()
        {
            // Заполнение данными 
            textBoxName.Text = selectedClient.ClientName;
            textBoxSurname.Text = selectedClient.ClientSurname;
            maskedTextBoxTelephone.Text = selectedClient.ClientTelephone; 
            maskedTextBoxTelephone.Mask = "+7 (000) 000-00-00"; // Установка маски для телефона
            textBoxEmail.Text = selectedClient.ClientEmail;
        }

        // Обработчик кнопки сохранения изменений
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // Проверка, заполнены ли все поля
            if (string.IsNullOrWhiteSpace(textBoxName.Text) ||
                string.IsNullOrWhiteSpace(textBoxSurname.Text) ||
                string.IsNullOrWhiteSpace(maskedTextBoxTelephone.Text) || // Используем MaskedTextBox
                string.IsNullOrWhiteSpace(textBoxEmail.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Проверка корректности номера телефона
            if (!maskedTextBoxTelephone.MaskCompleted)
            {
                MessageBox.Show("Номер телефона введен не полностью!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Проверка формата email
            if (!IsValidEmail(textBoxEmail.Text.Trim()))
            {
                MessageBox.Show("Некорректный формат email! Пример: example@mail.com", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Запрос подтверждения у пользователя
            DialogResult result = MessageBox.Show("Вы уверены, что хотите сохранить изменения?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Обновляем данные клиента
                selectedClient.ClientName = textBoxName.Text;
                selectedClient.ClientSurname = textBoxSurname.Text;
                selectedClient.ClientTelephone = maskedTextBoxTelephone.Text; // Используем MaskedTextBox
                selectedClient.ClientEmail = textBoxEmail.Text;

                // Обновляем данные клиента в базе данных
                UpdateClientInDatabase(selectedClient);

                // Закрываем форму
                this.Close();
            }
        }

        // Обновление данных клиента в базе данных
        private void UpdateClientInDatabase(Client client)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connect))
                {
                    connection.Open();

                    // SQL-запрос на обновление
                    string updateQuery = @"
                    UPDATE client
                    SET 
                        ClientName = @ClientName, 
                        ClientSurname = @ClientSurname,
                        ClientTelephone = @ClientTelephone,
                        ClientEmail = @ClientEmail
                    WHERE ClientID = @ClientID";

                    MySqlCommand command = new MySqlCommand(updateQuery, connection);

                    // Добавление параметров
                    command.Parameters.AddWithValue("@ClientName", client.ClientName);
                    command.Parameters.AddWithValue("@ClientSurname", client.ClientSurname);
                    command.Parameters.AddWithValue("@ClientTelephone", client.ClientTelephone);
                    command.Parameters.AddWithValue("@ClientEmail", client.ClientEmail);
                    command.Parameters.AddWithValue("@ClientID", client.ClientID);

                    // Выполнение запроса
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Данные клиента успешно обновлены.");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при обновлении данных клиента.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении данных клиента: " + ex.Message);
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
            if (!(e.KeyChar >= 'А' && e.KeyChar <= 'Я') && !(e.KeyChar >= 'а' && e.KeyChar <= 'я') && e.KeyChar != 'Ё' && e.KeyChar != 'ё' && e.KeyChar != ' ' && !char.IsControl(e.KeyChar))
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
            if (!(e.KeyChar >= 'А' && e.KeyChar <= 'Я') && !(e.KeyChar >= 'а' && e.KeyChar <= 'я') && e.KeyChar != 'Ё' && e.KeyChar != 'ё' && e.KeyChar != ' ' && !char.IsControl(e.KeyChar))
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
