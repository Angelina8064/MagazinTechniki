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
using static MagazinTechniki.AdminProducts;

namespace MagazinTechniki
{
    public partial class EditProduct : Form
    {
        // Конструктор формы, принимает продукт для редактирования
        public EditProduct(Products product)
        {
            InitializeComponent();
            selectedProduct = product;
            EditProduct_Load();
        }

        string connect = Connect.conn;
        private Products selectedProduct;

        // Загрузка данных продукта в элементы формы
        private void EditProduct_Load()
        {
            pictureBoxImage.SizeMode = PictureBoxSizeMode.Zoom; // Режим отображения изображения

            // Заполнение полей данными продукта
            textboxNameProduct.Text = selectedProduct.Name;
            textboxPrice.Text = selectedProduct.Cost.ToString();
            textboxDiscount.Text = selectedProduct.Discount.ToString();
            textBoxProductQuantityInStock.Text = selectedProduct.QuantityInStock.ToString();
            textBoxProductDescription.Text = selectedProduct.Description;
            pictureBoxImage.Image = selectedProduct.Image;
        }

        // Обработчик кнопки сохранения изменений
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // Проверка, заполнены ли все поля
            if (string.IsNullOrWhiteSpace(textboxNameProduct.Text) ||
                string.IsNullOrWhiteSpace(textboxPrice.Text) ||
                string.IsNullOrWhiteSpace(textboxDiscount.Text) ||
                string.IsNullOrWhiteSpace(textBoxProductQuantityInStock.Text) ||
                string.IsNullOrWhiteSpace(textBoxProductDescription.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Запрос подтверждения у пользователя
            DialogResult result = MessageBox.Show("Вы уверены, что хотите сохранить изменения?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Обновляем данные продукта
                selectedProduct.Name = textboxNameProduct.Text;
                selectedProduct.Cost = decimal.Parse(textboxPrice.Text);
                selectedProduct.Discount = int.Parse(textboxDiscount.Text);
                selectedProduct.QuantityInStock = int.Parse(textBoxProductQuantityInStock.Text);
                selectedProduct.Description = textBoxProductDescription.Text;

                // Обновляем данные продукта в базе данных
                UpdateProductInDatabase(selectedProduct);

                // Закрываем форму
                this.Close();
            }
        }

        // Обновление продукта в базе данных
        private void UpdateProductInDatabase(Products product)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connect))
                {
                    connection.Open();

                    // SQL-запрос на обновление
                    string updateQuery = @"
                                    UPDATE product
                                    SET 
                                        ProductName = @ProductName, 
                                        ProductCost = @ProductCost,
                                        ProductDiscount = @ProductDiscount,
                                        ProductQuantityInStock = @ProductQuantityInStock,
                                        ProductDescription = @ProductDescription,
                                        ProductPhoto = @ProductPhoto
                                    WHERE ProductArticleNumber = @ProductArticleNumber";

                    MySqlCommand command = new MySqlCommand(updateQuery, connection);

                    // Добавление параметров
                    command.Parameters.AddWithValue("@ProductName", product.Name);
                    command.Parameters.AddWithValue("@ProductCost", product.Cost);
                    command.Parameters.AddWithValue("@ProductDiscount", product.Discount);
                    command.Parameters.AddWithValue("@ProductDescription", product.Description);
                    command.Parameters.AddWithValue("@ProductQuantityInStock", product.QuantityInStock);
                    command.Parameters.AddWithValue("@ProductArticleNumber", product.Id);


                    // Обработка изображения
                    if (product.Image != null)
                    {
                        string imageFileName = SaveImageToFile(product.Image, product.Id);
                        command.Parameters.AddWithValue("@ProductPhoto", imageFileName);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@ProductPhoto", DBNull.Value);
                    }

                    // Выполнение запроса
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Продукт успешно обновлен в базе данных.");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при обновлении продукта в базе данных.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении продукта в базе данных: " + ex.Message);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Проверка формата изображения
        private bool IsValidImage(string imagePath)
        {
            string extension = Path.GetExtension(imagePath).ToLower();
            return extension == ".jpg" || extension == ".png";
        }

        // Обработчик клика по изображению для загрузки нового
        private void pictureBoxImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения (*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedImagePath = openFileDialog.FileName;

                if (IsValidImage(selectedImagePath))
                {
                    // Загрузка нового изображения
                    selectedProduct.Image = Image.FromFile(selectedImagePath);
                    pictureBoxImage.Image = selectedProduct.Image;
                }
                else
                {
                    MessageBox.Show("Выбранное изображение не соответствует требованиям. Допустимые типы: JPG, PNG.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Сохранение изображения
        private string SaveImageToFile(Image image, string productId)
        {
            if (image == null)
            {
                return null;
            }

            // Генерация уникального имени файла
            string imageFileName = $"{selectedProduct.Id}_{DateTime.Now.ToString("yyyyMMddHHmmss")}_{selectedProduct.Name}.jpg"; 
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", imageFileName);

            Directory.CreateDirectory(Path.GetDirectoryName(imagePath));

            image.Save(imagePath, System.Drawing.Imaging.ImageFormat.Jpeg);

            return imageFileName;
        }


        // Ограничение ввода
        private void textboxPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void textboxDiscount_KeyPress(object sender, KeyPressEventArgs e)
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

        // Автоматическое приведение первой буквы к верхнему регистру
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

        private void textboxNameProduct_TextChanged(object sender, EventArgs e)
        {
            CapitalizeFirstLetter(textboxNameProduct);
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

        private void textBoxProductDescription_TextChanged(object sender, EventArgs e)
        {
            CapitalizeFirstLetter(textBoxProductDescription);
        }
    }
}
