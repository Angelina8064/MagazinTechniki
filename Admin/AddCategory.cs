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
    public partial class AddCategory : Form
    {
        public AddCategory()
        {
            InitializeComponent();
        }
       private void AddCategory_Load(object sender, EventArgs e)
        {
            textboxNameCategory.Text = "";
        }
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Создаем объект категории
                Category newCategory = new Category
                {
                    CategoryName = textboxNameCategory.Text
                };

                // Проверка на пустые поля
                if (string.IsNullOrEmpty(newCategory.CategoryName))
                {
                    MessageBox.Show("Ошибка заполнения данных!", "Заполните все поля!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Подтверждение добавления категории
                DialogResult result = MessageBox.Show(
                    "Вы уверены, что хотите добавить категорию?",
                    "Подтверждение добавления",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                // Если пользователь подтвердил добавление
                if (result == DialogResult.Yes)
                {
                    AddCategoryToDatabase(newCategory); // Добавляем категорию в базу данных
                    this.Close(); // Закрываем форму
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка!", "Ошибка при добавлении категории: " + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddCategoryToDatabase(Category category)
        {
            try
            {
                string connect = Connect.conn;

                string insertQuery = @"
                        INSERT INTO category (CategoryName) 
                        VALUES (@CategoryName)";

                using (MySqlConnection connection = new MySqlConnection(connect))
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand(insertQuery, connection);

                    command.Parameters.AddWithValue("@CategoryName", category.CategoryName);
 

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Категория успешно добавлена в базу данных.");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при добавлении категории в базу данных.");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка при добавлении категории в базу данных: " + e.Message);
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