using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MagazinTechniki
{
    public partial class AdminImportAndRestore : Form
    {
        public AdminImportAndRestore()
        {
            InitializeComponent();
        }

        string connect = Connect.conn;
        private string selectedFilePath = "";

        private void buttonDirectory_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileD = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv",
                Title = "Выберите CSV файл для импорта"
            };

            if (openFileD.ShowDialog() == DialogResult.OK)
            {
                selectedFilePath = openFileD.FileName;
                textboxDirectory.Text = selectedFilePath;
            }
        }
        
        private void buttonImport_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBoxTable.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, выберите таблицу для импорта.");
                    return;
                }

                if (string.IsNullOrEmpty(selectedFilePath))
                {
                    MessageBox.Show("Пожалуйста, выберите файл для импорта.");
                    return;
                }

                using (MySqlConnection connection = new MySqlConnection(connect))
                {
                    connection.Open();
                    string tableName = comboBoxTable.SelectedValue.ToString();

                    int importRows = ImportDataFromCSV(selectedFilePath, tableName, connection);
                    MessageBox.Show($"Успешно импортировано {importRows} записей в таблицу {tableName}.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при импорте данных: {ex.Message}");
            }
        }

        private int ImportDataFromCSV(string filePath, string tableName, MySqlConnection connection)
        {
            int importRows = 0;
            using (StreamReader reader = new StreamReader(filePath))
            {
                string header = reader.ReadLine();
                string[] columns = header.Split(',');

                // Получаем список колонок из БД, исключая автоинкрементное поле
                var dbColumns = GetDbColumns(tableName, connection);

                if (columns.Length != dbColumns.Count)
                {
                    throw new Exception($"Количество столбцов в CSV файле ({columns.Length}) не соответствует структуре таблицы ({dbColumns.Count})");
                }

                // Создаем SQL запрос с явным указанием колонок (исключая автоинкрементное поле)
                string columnNames = string.Join(", ", dbColumns);
                string parameters = string.Join(", ", dbColumns.Select(c => "@" + c));
                string insertQuery = $"INSERT INTO {tableName} ({columnNames}) VALUES ({parameters})";

                MySqlCommand command = new MySqlCommand(insertQuery, connection);

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');

                    if (values.Length != dbColumns.Count)
                    {
                        continue; // или выбросить исключение
                    }

                    command.Parameters.Clear();
                    for (int i = 0; i < dbColumns.Count; i++)
                    {
                        command.Parameters.AddWithValue($"@{dbColumns[i]}", values[i]);
                    }
                    importRows += command.ExecuteNonQuery();
                }
            }
            return importRows;
        }

        private List<string> GetDbColumns(string tableName, MySqlConnection connection)
        {
            List<string> columns = new List<string>();
            string query = $@"SELECT COLUMN_NAME 
                             FROM INFORMATION_SCHEMA.COLUMNS 
                             WHERE TABLE_NAME = '{tableName}' 
                             AND EXTRA NOT LIKE '%auto_increment%' 
                             ORDER BY ORDINAL_POSITION";

            MySqlCommand command = new MySqlCommand(query, connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    columns.Add(reader.GetString(0));
                }
            }
            return columns;
        }

      

        private void comboBoxTable_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connect))
                {
                    connection.Open();

                    DataTable tables = connection.GetSchema("Tables");
                    comboBoxTable.DataSource = tables;
                    comboBoxTable.DisplayMember = "Table_NAME";
                    comboBoxTable.ValueMember = "Table_NAME";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при подключении списка таблиц: {ex.Message}");
            }
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }       
               
    }
}
