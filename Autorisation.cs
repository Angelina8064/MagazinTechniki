using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MagazinTechniki
{
    public partial class Autorisation : Form
    {
        string conn = Connect.conn;
        private bool passwordVisible = false; // Флаг видимости пароля

        public Autorisation()
        {
            InitializeComponent();
        }

        // Обработчик кнопки входа
        private void buttonEntry_Click(object sender, EventArgs e)
        { 
            string UserLogin = login.Text;
            string UserPass = pass.Text;
            string hashedPass = HashPassword(UserPass); // Хеширование пароля

            bool isAuthenticated = false;

            // Вход под стандратным admin / admin
            if (Properties.Settings.Default.userlogin == login.Text && Properties.Settings.Default.userpwd == pass.Text)
            {
                FormImportAndRestore form = new FormImportAndRestore();
                form.Show();
                Hide();

                return;
            }

            if (UserLogin.Length != 0)
            {
                using (MySqlConnection con = new MySqlConnection(conn))
                {
                    try
                    {
                        con.Open();

                        // SQL-запрос для проверки учетных данных
                        using (MySqlCommand cmd = new MySqlCommand("SELECT UserRole, UserSurname FROM user WHERE UserLogin = @UserLogin AND UserPassword = @UserPassword", con))
                        {
                            cmd.Parameters.AddWithValue("@UserLogin", UserLogin);
                            cmd.Parameters.AddWithValue("@UserPassword", hashedPass);
                            try
                            {
                                using (MySqlDataReader reader = cmd.ExecuteReader())
                                {
                                    if (reader.HasRows)
                                    {
                                        reader.Read();

                                        CurrentUser.Surname = reader["UserSurname"].ToString();
                                        CurrentUser.Role = reader["UserRole"].ToString();

                                        isAuthenticated = true;

                                        if (CurrentUser.Role == "1")
                                        {
                                            Admin form = new Admin();
                                            form.Show();
                                            Hide();
                                        }
                                        else if (CurrentUser.Role == "2")
                                        {
                                            Manager form = new Manager();
                                            form.Show();
                                            Hide();
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Введен не правильный логин или пароль.");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Ошибка: {ex.Message}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка подключения: " + ex.Message);
                    }
                }
                // Сброс полей при неудачной аутентификации
                if (!isAuthenticated)
                {
                    login.Clear();
                    pass.Clear();
                    login.Focus(); 
                }
            }
            else
            {
                MessageBox.Show("Логин не может быть пустым.");
            }
        }

        // Хеширование пароля с использованием SHA256
        private string HashPassword(string UserPass)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(UserPass));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Фильтрация ввода для логина
        private void login_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешаем: буквы/цифры/спецсимволы, запрещаем кириллицу
            if (!(char.IsLetterOrDigit(e.KeyChar) || e.KeyChar == '_' || e.KeyChar == '-' || char.IsControl(e.KeyChar)))
            { 
                e.Handled = true; 
            }

            if (e.KeyChar >= 'А' && e.KeyChar <= 'я' || e.KeyChar == 'ё' || e.KeyChar == 'Ё')
            {
                e.Handled = true;
            }

        }

        // Фильтрация ввода для пароля
        private void pass_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Запрещаем русские буквы и пробелы
            if ((e.KeyChar >= 'а' && e.KeyChar <= 'я') || (e.KeyChar >= 'А' && e.KeyChar <= 'Я') || e.KeyChar == ' ')
            {
                e.Handled = true;
            }
        }

        // Обработчик кнопки выхода
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите выйти?", "Подтверждение выхода", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Application.Exit(); 
            }
        }

        private void btnShowPass_Click_1(object sender, EventArgs e)
        {
            passwordVisible = !passwordVisible;

            if (passwordVisible)
            {
                pass.PasswordChar = '\0'; // Показываем пароль
                btnShowPass.BackgroundImage = Properties.Resources.glaz; // Иконка открытого глаза
            }
            else
            {
                pass.PasswordChar = '*'; // Скрываем пароль
                btnShowPass.BackgroundImage = Properties.Resources.glaz2; // Иконка закрытого глаза
            }

            // Фокусируемся обратно на поле пароля
            pass.Focus();
            pass.SelectionStart = pass.Text.Length;
        }
    }
}
