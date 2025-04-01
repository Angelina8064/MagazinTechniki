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
using static MagazinTechniki.AdminSpravochnik;

namespace MagazinTechniki
{
    public partial class AddRole : Form
    {
        public AddRole()
        {
            InitializeComponent();
        }

        private void AddRole_Load(object sender, EventArgs e)
        {
            textboxNameRole.Text = "";
        }
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Создаем объект роли
                Role newRole = new Role
                {
                    RoleName = textboxNameRole.Text
                };

                // Проверка на пустые поля
                if (string.IsNullOrEmpty(newRole.RoleName))
                {
                    MessageBox.Show("Ошибка заполнения данных!", "Заполните все поля!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Подтверждение добавления роли
                DialogResult result = MessageBox.Show(
                    "Вы уверены, что хотите добавить роль?",
                    "Подтверждение добавления",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                // Если пользователь подтвердил добавление
                if (result == DialogResult.Yes)
                {
                    AddRoleToDatabase(newRole); // Добавляем роль в базу данных
                    this.Close(); // Закрываем форму
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка!", "Ошибка при добавлении роли: " + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddRoleToDatabase(Role role)
        {
            try
            {
                string connect = Connect.conn;

                string insertQuery = @"
                        INSERT INTO role (RoleName) 
                        VALUES (@RoleName)";

                using (MySqlConnection connection = new MySqlConnection(connect))
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand(insertQuery, connection);

                    command.Parameters.AddWithValue("@RoleName", role.RoleName);


                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Роль успешно добавлена в базу данных.");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при добавлении роли в базу данных.");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка при добавлении роли в базу данных: " + e.Message);
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

        private void textboxNameRole_KeyPress(object sender, KeyPressEventArgs e)
        {
           if (!(e.KeyChar >= 'А' && e.KeyChar <= 'Я') && !(e.KeyChar >= 'а' && e.KeyChar <= 'я') && e.KeyChar != 'Ё' && e.KeyChar != 'ё' && e.KeyChar != ' ' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }
       
        private void textboxNameRole_TextChanged(object sender, EventArgs e)
        {
            CapitalizeFirstLetter(textboxNameRole);
        } 

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
