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
    public partial class EditCategory : Form
    {
        string connect = Connect.conn;
        private Category selectedCategory;

        public EditCategory(Category category)
        {
            InitializeComponent();

            selectedCategory = category;
            EditCategory_Load();
        }

       private void EditCategory_Load()
        {
            textboxNameCategory.Text = selectedCategory.CategoryName;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // Проверка, заполнено ли поле с названием категории
            if (string.IsNullOrWhiteSpace(textboxNameCategory.Text))
            {
                MessageBox.Show("Пожалуйста, заполните поле названия категории.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Запрос подтверждения у пользователя
            DialogResult result = MessageBox.Show("Вы уверены, что хотите сохранить изменения?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Обновляем название категории
                selectedCategory.CategoryName = textboxNameCategory.Text;

                // Обновляем категорию в базе данных
                UpdateCategoryInDatabase(selectedCategory);

                // Закрываем форму
                this.Close();
            }
        }

        private void UpdateCategoryInDatabase(Category category)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connect))
                {
                    connection.Open();

                    string updateQuery = @"
                                    UPDATE category
                                    SET 
                                        CategoryName = @CategoryName
                                    WHERE CategoryID = @CategoryID";

                    MySqlCommand command = new MySqlCommand(updateQuery, connection);
                    command.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                    command.Parameters.AddWithValue("@CategoryID", category.CategoryID);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Категория успешно обновлена в базе данных.");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при обновлении категории в базе данных.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении категории в базе данных: " + ex.Message);
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

        private void textboxNameCategory_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(e.KeyChar >= 'А' && e.KeyChar <= 'Я') && !(e.KeyChar >= 'а' && e.KeyChar <= 'я') && e.KeyChar != 'Ё' && e.KeyChar != 'ё' && e.KeyChar != ' ' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }
       
        private void textboxNameCategory_TextChanged(object sender, EventArgs e)
        {
            CapitalizeFirstLetter(textboxNameCategory);
        } 

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
