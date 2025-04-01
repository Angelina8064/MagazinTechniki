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
    public partial class EditRole : Form
    {
        string connect = Connect.conn;
        private Role selectedRole;

        public EditRole(Role role)
        {
            InitializeComponent();

            selectedRole = role;
            EditRole_Load();
        }

        private void EditRole_Load()
        {
            textboxNameRole.Text = selectedRole.RoleName;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // Проверка, заполнено ли поле с названием категории
            if (string.IsNullOrWhiteSpace(textboxNameRole.Text))
            {
                MessageBox.Show("Пожалуйста, заполните поле названия роли.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Запрос подтверждения у пользователя
            DialogResult result = MessageBox.Show("Вы уверены, что хотите сохранить изменения?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                selectedRole.RoleName = textboxNameRole.Text;

                UpdateRoleInDatabase(selectedRole);

                this.Close();
            }
        }

        private void UpdateRoleInDatabase(Role role)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connect))
                {
                    connection.Open();

                    string updateQuery = @"
                                    UPDATE role
                                    SET 
                                        RoleName = @RoleName
                                    WHERE RoleID = @RoleID";

                    MySqlCommand command = new MySqlCommand(updateQuery, connection);
                    command.Parameters.AddWithValue("@RoleName", role.RoleName);
                    command.Parameters.AddWithValue("@RoleID", role.RoleID);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Роль успешно обновлена в базе данных.");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при обновлении роли в базе данных.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении роли в базе данных: " + ex.Message);
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
