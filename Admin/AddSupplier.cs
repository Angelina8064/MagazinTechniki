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
    public partial class AddSupplier : Form
    {
        public AddSupplier()
        {
            InitializeComponent();
        }
        private void AddSupplier_Load(object sender, EventArgs e)
        {
            textboxNameSupplier.Text = "";
        }
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Создаем объект поставщика
                Supplier newSupplier = new Supplier
                {
                    SupplierName = textboxNameSupplier.Text
                };

                // Проверка на пустые поля
                if (string.IsNullOrEmpty(newSupplier.SupplierName))
                {
                    MessageBox.Show("Ошибка заполнения данных!", "Заполните все поля!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Подтверждение добавления поставщика
                DialogResult result = MessageBox.Show(
                    "Вы уверены, что хотите добавить поставщика?",
                    "Подтверждение добавления",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                // Если пользователь подтвердил добавление
                if (result == DialogResult.Yes)
                {
                    AddSupplierToDatabase(newSupplier); // Добавляем поставщика в базу данных
                    this.Close(); // Закрываем форму
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка!", "Ошибка при добавлении поставщика: " + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddSupplierToDatabase(Supplier supplier)
        {
            try
            {
                string connect = Connect.conn;

                string insertQuery = @"
                        INSERT INTO supplier (SupplierName) 
                        VALUES (@SupplierName)";

                using (MySqlConnection connection = new MySqlConnection(connect))
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand(insertQuery, connection);

                    command.Parameters.AddWithValue("@SupplierName", supplier.SupplierName);


                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Поставщик успешно добавлен в базу данных.");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при добавлении поставщика в базу данных.");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка при добавлении поставщика в базу данных: " + e.Message);
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

        private void textboxNameSupplier_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(e.KeyChar >= 'А' && e.KeyChar <= 'Я') && !(e.KeyChar >= 'а' && e.KeyChar <= 'я') && e.KeyChar != 'Ё' && e.KeyChar != 'ё' && e.KeyChar != ' ' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textboxNameSupplier_TextChanged(object sender, EventArgs e)
        {
            CapitalizeFirstLetter(textboxNameSupplier);
        }        

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
