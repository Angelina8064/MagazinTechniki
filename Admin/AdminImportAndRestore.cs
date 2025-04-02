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
            LoadTablesList();
        }

        string connect = Connect.conn;

        private void LoadTablesList()
        {
            comboBoxTable.Items.Clear();
            comboBoxTable.Items.AddRange(new string[]
            {   "category",
                "client",
                "`order`",
                "orderproduct",
                "product",
                "role",
                "supplier",
                "user"
            });
        }

        private void buttonDirectory_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Выберите файл для импорта",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textboxDirectory.Text = openFileDialog.FileName;
            }
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            if (comboBoxTable.SelectedItem == null)
            {
                MessageBox.Show("Выберите таблицу для импорта", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(textboxDirectory.Text))
            {
                MessageBox.Show("Сначала выберите файл для импорта", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string tableName = comboBoxTable.SelectedItem.ToString();
                int importedRows = ImportCsvToTable(textboxDirectory.Text, tableName);

                MessageBox.Show($"Успешно импортировано {importedRows} записей", "Успех",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка импорта: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int ImportCsvToTable(string filePath, string tableName)
        {
            using (var connection = new MySqlConnection(connect))
            {
                connection.Open();

                // Чтение файла построчно
                var lines = new List<string>();
                using (var reader = new StreamReader(filePath, Encoding.UTF8))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("!"))
                            lines.Add(line.Trim());
                    }
                }

                if (lines.Count < 2) return 0;

                // Определяем разделитель
                char separator = lines[0].Contains(';') ? ';' : ',';

                // Получаем заголовки
                var headers = SplitCsvLine(lines[0], separator);

                // Получаем столбцы таблицы
                var columns = GetTableColumnsSimple(connection, tableName);

                // Проверка соответствия
                if (headers.Length != columns.Count)
                {
                    throw new Exception($"Несоответствие столбцов. Ожидалось {columns.Count}, получено {headers.Length}");
                }

                // Создаем команду для вставки
                var cmdText = $"INSERT INTO {tableName} ({string.Join(", ", columns)}) VALUES ({string.Join(", ", columns.Select(c => $"@{c}"))})";
                var cmd = connection.CreateCommand();
                cmd.CommandText = cmdText;

                int importedRows = 0;
                for (int i = 1; i < lines.Count; i++)
                {
                    var values = SplitCsvLine(lines[i], separator);
                    if (values.Length != columns.Count) continue;

                    cmd.Parameters.Clear();
                    for (int j = 0; j < columns.Count; j++)
                    {
                        var param = cmd.CreateParameter();
                        param.ParameterName = $"@{columns[j]}";
                        param.Value = string.IsNullOrWhiteSpace(values[j]) ? DBNull.Value : (object)values[j].Trim('"');
                        cmd.Parameters.Add(param);
                    }

                    try
                    {
                        cmd.ExecuteNonQuery();
                        importedRows++;
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine($"Ошибка при вставке строки {i}: {ex.Message}");
                    }
                }

                return importedRows;
            }
        }

        private string[] SplitCsvLine(string line, char separator)
        {
            var result = new List<string>();
            var inQuotes = false;
            var current = new StringBuilder();

            foreach (char c in line)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == separator && !inQuotes)
                {
                    result.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            result.Add(current.ToString());
            return result.ToArray();
        }

        private List<string> GetTableColumnsSimple(MySqlConnection connection, string tableName)
        {
            var columns = new List<string>();
            using (var cmd = new MySqlCommand($"SHOW COLUMNS FROM {tableName}", connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    columns.Add(reader.GetString(0));
                }
            }
            return columns;
        }


        private void buttonRestore_Click(object sender, EventArgs e)
        {
            try
            {
                // Извлекаем параметры подключения
                var builder = new MySqlConnectionStringBuilder(connect);
                string databaseName = builder.Database;

                if (string.IsNullOrEmpty(databaseName))
                {
                    MessageBox.Show("Имя базы данных не указано в строке подключения", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Подключение к серверу без указания базы данных
                builder.Database = "";
                string serverConnectionString = builder.ConnectionString;

                // 1. Удаляем и создаем заново базу данных
                using (var serverConnection = new MySqlConnection(serverConnectionString))
                {
                    serverConnection.Open();

                    // Удаляем базу, если существует
                    using (var cmd = new MySqlCommand($"DROP DATABASE IF EXISTS `{databaseName}`", serverConnection))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Создаем новую базу
                    using (var cmd = new MySqlCommand($"CREATE DATABASE `{databaseName}` CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci", serverConnection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                // 2. Восстанавливаем структуру таблиц
                builder.Database = databaseName;
                string dbConnectionString = builder.ConnectionString;

                // SQL-запросы для создания таблиц в правильном порядке (с учетом зависимостей)
                string[] createTablesScript = {
            // Отключаем проверки для упрощения создания таблиц с внешними ключами
            "SET FOREIGN_KEY_CHECKS = 0;",
            "SET UNIQUE_CHECKS = 0;",
            "SET SQL_MODE = 'NO_AUTO_VALUE_ON_ZERO';",

            // Создание таблицы role (должна быть первой из-за зависимостей)
            @"CREATE TABLE IF NOT EXISTS `role` (
                `RoleID` int NOT NULL AUTO_INCREMENT,
                `RoleName` varchar(100) NOT NULL,
                PRIMARY KEY (`RoleID`)
            ) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;",

            // Создание таблицы category
            @"CREATE TABLE IF NOT EXISTS `category` (
                `CategoryID` int NOT NULL AUTO_INCREMENT,
                `CategoryName` varchar(100) NOT NULL,
                PRIMARY KEY (`CategoryID`)
            ) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;",

            // Создание таблицы supplier
            @"CREATE TABLE IF NOT EXISTS `supplier` (
                `SupplierID` int NOT NULL AUTO_INCREMENT,
                `SupplierName` varchar(100) NOT NULL,
                PRIMARY KEY (`SupplierID`)
            ) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;",

            // Создание таблицы client
            @"CREATE TABLE IF NOT EXISTS `client` (
                `ClientID` int NOT NULL AUTO_INCREMENT,
                `ClientName` varchar(100) NOT NULL,
                `ClientSurname` varchar(100) NOT NULL,
                `ClientTelephone` varchar(18) NOT NULL,
                `ClientEmail` varchar(100) NOT NULL,
                PRIMARY KEY (`ClientID`)
            ) ENGINE=InnoDB AUTO_INCREMENT=82 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;",

            // Создание таблицы user
            @"CREATE TABLE IF NOT EXISTS `user` (
                `UserID` int NOT NULL AUTO_INCREMENT,
                `UserSurname` varchar(100) NOT NULL,
                `UserName` varchar(100) NOT NULL,
                `UserPatronymic` varchar(100) NOT NULL,
                `UserLogin` text NOT NULL,
                `UserPassword` text NOT NULL,
                `UserRole` int NOT NULL,
                PRIMARY KEY (`UserID`),
                KEY `UserRole` (`UserRole`),
                CONSTRAINT `user_ibfk_1` FOREIGN KEY (`UserRole`) REFERENCES `role` (`RoleID`)
            ) ENGINE=InnoDB AUTO_INCREMENT=77 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;",

            // Создание таблицы product
            @"CREATE TABLE IF NOT EXISTS `product` (
                `ProductArticleNumber` varchar(6) NOT NULL,
                `ProductName` varchar(100) NOT NULL,
                `ProductCost` float NOT NULL,
                `ProductDiscount` tinyint DEFAULT NULL,
                `ProductManufacturer` varchar(100) NOT NULL,
                `ProductSupplierID` int NOT NULL,
                `ProductCategoryID` int NOT NULL,
                `ProductQuantityInStock` int NOT NULL,
                `ProductDescription` varchar(100) NOT NULL,
                `ProductPhoto` varchar(100) DEFAULT NULL,
                PRIMARY KEY (`ProductArticleNumber`),
                KEY `ProductSupplierID` (`ProductSupplierID`),
                KEY `ProductCategoryID` (`ProductCategoryID`),
                CONSTRAINT `product_ibfk_1` FOREIGN KEY (`ProductSupplierID`) REFERENCES `supplier` (`SupplierID`),
                CONSTRAINT `product_ibfk_2` FOREIGN KEY (`ProductCategoryID`) REFERENCES `category` (`CategoryID`)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;",

            // Создание таблицы order
            @"CREATE TABLE IF NOT EXISTS `order` (
                `OrderID` int NOT NULL AUTO_INCREMENT,
                `OrderDate` datetime NOT NULL,
                `OrderUserID` int NOT NULL,
                `OrderClientID` int NOT NULL,
                `OrderStatus` varchar(45) DEFAULT NULL,
                PRIMARY KEY (`OrderID`),
                KEY `OrderUserID` (`OrderUserID`),
                KEY `OrderClientID` (`OrderClientID`),
                CONSTRAINT `order_ibfk_1` FOREIGN KEY (`OrderUserID`) REFERENCES `user` (`UserID`),
                CONSTRAINT `order_ibfk_2` FOREIGN KEY (`OrderClientID`) REFERENCES `client` (`ClientID`)
            ) ENGINE=InnoDB AUTO_INCREMENT=101 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;",

            // Создание таблицы orderproduct
            @"CREATE TABLE IF NOT EXISTS `orderproduct` (
                `OrderID` int NOT NULL,
                `ProductArticleNumber` varchar(6) NOT NULL,
                `ProductCount` int NOT NULL,
                `ProoductTotalCost` int NOT NULL,
                PRIMARY KEY (`OrderID`,`ProductArticleNumber`),
                KEY `ProductArticleNumber` (`ProductArticleNumber`),
                CONSTRAINT `orderproduct_ibfk_1` FOREIGN KEY (`OrderID`) REFERENCES `order` (`OrderID`),
                CONSTRAINT `orderproduct_ibfk_2` FOREIGN KEY (`ProductArticleNumber`) REFERENCES `product` (`ProductArticleNumber`)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;",

            // Включаем проверки обратно
            "SET FOREIGN_KEY_CHECKS = 1;",
            "SET UNIQUE_CHECKS = 1;"
        };

                // Выполняем SQL-запросы
                ExecuteSqlScript(dbConnectionString, createTablesScript);

                MessageBox.Show("База данных и таблицы успешно восстановлены", "Успех",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка восстановления: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExecuteSqlScript(string connectionString, string[] scripts)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                foreach (var script in scripts)
                {
                    if (string.IsNullOrWhiteSpace(script)) continue;

                    try
                    {
                        using (var cmd = new MySqlCommand(script, connection))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка выполнения команды: {script}\n{ex.Message}");
                        throw;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
