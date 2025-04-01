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
using System.Xml.Linq;
using static MagazinTechniki.AdminProducts;

namespace MagazinTechniki
{
    public partial class AddProduct : Form
    {
        public AddProduct()
        {
            InitializeComponent();
        }

        // Загрузка формы
        private void AddProduct_Load(object sender, EventArgs e)
        {
            // Выпадающие списки
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;  
            comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;  

            // очистка полей для ввода
            textBoxArticle.Text = "";
            textboxNameProduct.Text = "";
            textboxPrice.Text = "";
            textboxDiscount.Text = "";
            textBoxProductManufacter.Text = "";  
            textBoxProductQuantityInStock.Text = "";     
            textBoxProductDescription.Text = "";

            // Загрзука заглушки
            pictureBoxImage.Image = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "zaglushka.jpg"));


            // Загрузка категорий и поставщиков
            LoadCategories();
            LoadSuppliers();
        }

        // Обработчик кнопки добавления товара
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверка длины артикула (должен быть 6 символов)
                if (textBoxArticle.Text.Length != 6)
                {
                    MessageBox.Show("Артикул должен содержать ровно 6 символов!",
                                  "Ошибка ввода",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning);
                    return;
                }


                if (pictureBoxImage.Image == null)
                {
                    MessageBox.Show("Добавьте изображение товара!");
                    return;
                }

                // Проверка на уникальность артикула
                if (!IsArticleNumberUnique(textBoxArticle.Text))
                {
                    MessageBox.Show("Артикул уже существует! Введите уникальный артикул.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                // Создаем объект продукта
                Products newProduct = new Products
                {
                    Id = textBoxArticle.Text,
                    Name = textboxNameProduct.Text,
                    Cost = decimal.Parse(textboxPrice.Text),
                    Discount = int.Parse(textboxDiscount.Text),
                    Manufacterer = textBoxProductManufacter.Text,
                    SupplierName = comboBox3.SelectedItem?.ToString(),
                    CategoryName = comboBox1.SelectedItem?.ToString(),
                    QuantityInStock = int.Parse(textBoxProductQuantityInStock.Text),
                    Description = textBoxProductDescription.Text
                };

                // Проверка на пустые поля и корректность данных
                if (string.IsNullOrEmpty(newProduct.Id) ||
                    string.IsNullOrEmpty(newProduct.Name) ||
                    newProduct.Cost <= 0 ||
                    newProduct.Discount < 0 ||
                    string.IsNullOrEmpty(newProduct.Manufacterer) ||
                    string.IsNullOrEmpty(newProduct.SupplierName) ||
                    string.IsNullOrEmpty(newProduct.CategoryName) ||
                    newProduct.QuantityInStock < 0 ||
                    string.IsNullOrEmpty(newProduct.Description))
                {
                    MessageBox.Show("Ошибка заполнения данных!", "Заполните все поля!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Подтверждение добавления продукта
                DialogResult result = MessageBox.Show(
                    "Вы уверены, что хотите добавить продукт?",
                    "Подтверждение добавления",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                // Если пользователь подтвердил добавление
                if (result == DialogResult.Yes)
                {
                    byte[] image = GetImageBytes(pictureBoxImage.Image); // Получаем изображение
                    AddProductToDatabase(newProduct); // Добавляем продукт в базу данных
                    this.Close(); // Закрываем форму
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка!", "Ошибка при добавлении продукта: " + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Конвертация изображения в массив байтов
        private byte[] GetImageBytes(Image image)
        {
            if (image == null) return null;

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    // Создаем копию для избежания GDI+ ошибок
                    using (Bitmap temp = new Bitmap(image))
                    {
                        temp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка конвертации изображения: " + ex.Message);
                return null;
            }
        }

        // Добавление продукта в базу данных
        private void AddProductToDatabase(Products product)
        {
            try
            {
                string connect = Connect.conn;

                // Сохранение изображения в файл
                string imageFileName = SaveImageToFile(pictureBoxImage.Image, product.Id);

                // SQL-запрос с подзапросами для получения ID категории и поставщика
                string insertQuery = @"
                        INSERT INTO product (
                            ProductArticleNumber, 
                            ProductName, 
                            ProductCost, 
                            ProductDiscount, 
                            ProductManufacturer, 
                            ProductSupplierID, 
                            ProductCategoryID, 
                            ProductQuantityInStock, 
                            ProductDescription, 
                            ProductPhoto
                        ) VALUES (
                            @ProductArticleNumber, 
                            @ProductName, 
                            @ProductCost, 
                            @ProductDiscount, 
                            @ProductManufacturer, 
                            (SELECT SupplierID FROM supplier WHERE SupplierName = @ProductSupplierName), 
                            (SELECT CategoryID FROM category WHERE CategoryName = @ProductCategoryName), 
                            @ProductQuantityInStock, 
                            @ProductDescription, 
                            @ProductPhoto
                        )";

                using (MySqlConnection connection = new MySqlConnection(connect))
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand(insertQuery, connection);

                    // Добавление параметров
                    command.Parameters.AddWithValue("@ProductArticleNumber", product.Id);
                    command.Parameters.AddWithValue("@ProductName", product.Name);
                    command.Parameters.AddWithValue("@ProductCost", product.Cost);
                    command.Parameters.AddWithValue("@ProductDiscount", product.Discount);
                    command.Parameters.AddWithValue("@ProductManufacturer", product.Manufacterer);
                    command.Parameters.AddWithValue("@ProductSupplierName", product.SupplierName); 
                    command.Parameters.AddWithValue("@ProductCategoryName", product.CategoryName);
                    command.Parameters.AddWithValue("@ProductQuantityInStock", product.QuantityInStock);
                    command.Parameters.AddWithValue("@ProductDescription", product.Description);
                    command.Parameters.AddWithValue("@ProductPhoto", imageFileName);

                    // Выполнение запроса
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Продукт успешно добавлен в базу данных.");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при добавлении продукта в базу данных.");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка при добавлении продукта в базу данных: " + e.Message);
            }
        }

        // Сохранение изображения
        private string SaveImageToFile(Image image, string productId)
        {
            if (image == null) return null;

            // Очищаем недопустимые символы в имени файла
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                productId = productId.Replace(c, '_');
            }

            string imageFileName = $"{productId}.jpg";
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", imageFileName);

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(imagePath));

                // Удаляем существующий файл, если есть
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }

                // Сохраняем через копию изображения
                using (Bitmap tempImage = new Bitmap(image))
                {
                    tempImage.Save(imagePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                }

                return imageFileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения изображения: {ex.Message}");
                return null;
            }
        }

        // Загрузка изображения по клику
        private void pictureBoxImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения (*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Используем using для освобождения ресурсов
                using (FileStream stream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                {
                    pictureBoxImage.Image = Image.FromStream(stream);
                }
            }
        }

        // Загрузка категорий из БД
        private void LoadCategories()
        {
            using (MySqlConnection connection = new MySqlConnection(Connect.conn))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand("SELECT CategoryName FROM category", connection);
                MySqlDataReader reader = command.ExecuteReader();

                comboBox1.Items.Clear();

                while (reader.Read())
                {
                    comboBox1.Items.Add(reader["CategoryName"].ToString());
                }

                reader.Close();
            }

            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0; 
            }
        }

        private void LoadSuppliers()
        {
            using (MySqlConnection connection = new MySqlConnection(Connect.conn))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand("SELECT SupplierName FROM supplier", connection);
                MySqlDataReader reader = command.ExecuteReader();

                comboBox3.Items.Clear();

                while (reader.Read())
                {
                    comboBox3.Items.Add(reader["SupplierName"].ToString());
                }

                reader.Close();
            }

            if (comboBox3.Items.Count > 0)
            {
                comboBox3.SelectedIndex = 0; 
            }
        }

        // Проверка уникальности артикула
        private bool IsArticleNumberUnique(string articleNumber)
        {
            try
            {
                string connect = Connect.conn;

                using (MySqlConnection connection = new MySqlConnection(connect))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM product WHERE ProductArticleNumber = @ProductArticleNumber";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ProductArticleNumber", articleNumber);

                        int count = Convert.ToInt32(command.ExecuteScalar());

                        // Если count > 0, значит, артикул уже существует
                        return count == 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при проверке уникальности артикула: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Контроль ввода данных
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

        private void textBoxArticle_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= 'а' && e.KeyChar <= 'я') || (e.KeyChar >= 'А' && e.KeyChar <= 'Я'))
            {
                e.Handled = true; 
            }
            else if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBoxArticle_TextChanged(object sender, EventArgs e)
        {
            textBoxArticle.Text = textBoxArticle.Text.ToUpper();
            textBoxArticle.SelectionStart = textBoxArticle.Text.Length;
        }

        private void textboxNameProduct_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешаем:
            // - буквы (char.IsLetter)
            // - цифры (char.IsDigit)
            // - управляющие символы (Backspace, Delete и т.д.)
            // - пробел
            if (!char.IsLetter(e.KeyChar) && !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true; // Блокируем неразрешенные символы
            }
        }

        private void textboxNameProduct_TextChanged(object sender, EventArgs e)
        {
            CapitalizeFirstLetter(textboxNameProduct);
        }

        private void textboxDiscount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; 
            }
        }
        private void textBoxProductCost_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBoxProductQuantityInStock_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; 
            }
        }

        private void textBoxProductManufacter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true; 
            }
        }
        private void textBoxProductManufacter_TextChanged(object sender, EventArgs e)
        {
            CapitalizeFirstLetter(textBoxProductManufacter);
        }

        private void textBoxProductDescription_TextChanged(object sender, EventArgs e)
        {
            CapitalizeFirstLetter(textBoxProductDescription);
        }
    }
}