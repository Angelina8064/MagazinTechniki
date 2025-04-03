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

        private CaptchaGenerator _captchaGenerator = new CaptchaGenerator();
        private string _currentCaptcha;
        private DateTime _lastFailedAttempt = DateTime.MinValue;
        private int _failedAttempts = 0;
        private int _blockDurationSeconds = 10;
        private System.Windows.Forms.Timer blockTimer;

        public Autorisation()
        {
            InitializeComponent();

            InitializeBlockTimer();
            HideCaptchaAndControls();
        }

        // Обработчик кнопки входа
        private void buttonEntry_Click(object sender, EventArgs e)
        {
            if (DateTime.Now - _lastFailedAttempt < TimeSpan.FromSeconds(_blockDurationSeconds))
            {
                MessageBox.Show($"Вы были заблокированы на {_blockDurationSeconds} секунд из-за слишком большого количества неудачных попыток входа.");
                return;
            }

            string UserLogin = login.Text;
            string UserPass = pass.Text;
            string hashedPass = HashPassword(UserPass);

            bool isAuthenticated = false;

            if (UserLogin.Length != 0)
            {
                // Проверка капчи только после первой неудачной попытки
                if (_failedAttempts > 0)
                {
                    if (inputcaptcha.Text != _currentCaptcha)
                    {
                        MessageBox.Show("Неверная CAPTCHA. Вы заблокированы на 10 секунд.");
                        _lastFailedAttempt = DateTime.Now;

                        login.Clear();
                        pass.Clear();
                        inputcaptcha.Clear();

                        DisableControls();
                        
                        blockTimer.Start();
                        
                        UpdateCaptcha();
                        
                        return;
                    }
                }

                using (MySqlConnection con = new MySqlConnection(conn))
                {
                    try
                    {
                        con.Open();

                        using (MySqlCommand cmd = new MySqlCommand("SELECT UserRole, UserSurname FROM user WHERE UserLogin = @UserLogin AND UserPassword = @UserPassword", con))
                        {
                            cmd.Parameters.AddWithValue("@UserLogin", UserLogin);
                            cmd.Parameters.AddWithValue("@UserPassword", hashedPass);

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
                                else if (Properties.Settings.Default.userlogin == login.Text &&
                                         Properties.Settings.Default.userpwd == pass.Text)
                                {
                                    AdminImportAndRestore form = new AdminImportAndRestore();
                                    form.Show();
                                    Hide();
                                    return;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}");
                    }
                }

                if (!isAuthenticated)
                {
                    _failedAttempts++;

                    if (_failedAttempts == 1)
                    {
                        ShowCaptchaAndControls();
                        UpdateCaptcha();
                    }

                    login.Clear();
                    pass.Clear();
                    login.Focus();
                    MessageBox.Show("Введен неправильный логин или пароль.");
                }
                else
                {
                    _failedAttempts = 0;
                    HideCaptchaAndControls();
                }
            }
            else
            {
                MessageBox.Show("Логин не может быть пустым.");
            }
        }

        #region Captcha
        private void InitializeBlockTimer()
        {
            blockTimer = new System.Windows.Forms.Timer();
            blockTimer.Interval = 1000; // 1 секунда
            blockTimer.Tick += BlockTimer_Tick;
        }

        private void BlockTimer_Tick(object sender, EventArgs e)
        {
            var timeLeft = _blockDurationSeconds - (int)(DateTime.Now - _lastFailedAttempt).TotalSeconds;
            
            if (timeLeft <= 0)
            {
                blockTimer.Stop();
                EnableControls();
                buttonEntry.Text = "Войти";
            }
            else
            {
                buttonEntry.Text = $"({timeLeft} сек)";
            }
        }

        private void DisableControls()
        {
            login.Enabled = false;
            pass.Enabled = false;
            buttonEntry.Enabled = false;
            inputcaptcha.Enabled = false;
            updatecaptcha.Enabled = false;
            btnShowPass.Enabled = false;
        }

        private void EnableControls()
        {
            login.Enabled = true;
            pass.Enabled = true;
            buttonEntry.Enabled = true;
            inputcaptcha.Enabled = true;
            updatecaptcha.Enabled = true;
            btnShowPass.Enabled = true;
        }

        private void UpdateCaptcha()
        {
            _currentCaptcha = _captchaGenerator.GenerateCaptcha();
            captchaImage.Image = _captchaGenerator.RenderCaptcha(_currentCaptcha);
        }

        private void ShowCaptchaAndControls()
        {
            captchaImage.Visible = true;
            inputcaptcha.Visible = true;
            updatecaptcha.Visible = true;
        }

        private void HideCaptchaAndControls()
        {
            captchaImage.Visible = false;
            inputcaptcha.Visible = false;
            updatecaptcha.Visible = false;
        }
        private void updatecaptcha_Click(object sender, EventArgs e)
        {
            inputcaptcha.Clear();
            UpdateCaptcha();
        }
        #endregion

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
