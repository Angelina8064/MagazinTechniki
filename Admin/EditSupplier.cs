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
    public partial class EditSupplier : Form
    {
        string connect = Connect.conn;
        private Supplier selectedSupplier;

        public EditSupplier(Supplier supplier)
        {
            InitializeComponent();

            selectedSupplier = supplier;
            EditSupplier_Load();
        }

        private void EditSupplier_Load()
        {
            textboxNameSupplier.Text = selectedSupplier.SupplierName;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textboxNameSupplier.Text))
            {
                MessageBox.Show("Пожалуйста, заполните поле названия поставщика.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult result = MessageBox.Show("Вы уверены, что хотите сохранить изменения?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                selectedSupplier.SupplierName = textboxNameSupplier.Text;

                UpdateSupplierInDatabase(selectedSupplier);

                this.Close();
            }      
        }

        private void UpdateSupplierInDatabase(Supplier supplier)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connect))
                {
                    connection.Open();

                    string updateQuery = @"
                                    UPDATE supplier
                                    SET 
                                        SupplierName = @SupplierName
                                    WHERE SupplierID = @SupplierID";

                    MySqlCommand command = new MySqlCommand(updateQuery, connection);        
                    
                    command.Parameters.AddWithValue("@SupplierID", supplier.SupplierID);
                    command.Parameters.AddWithValue("@SupplierName", supplier.SupplierName);


                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Поставщик успешно обновлен в базе данных.");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при обновлении поставщика в базе данных.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении поставщика в базе данных: " + ex.Message);
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
