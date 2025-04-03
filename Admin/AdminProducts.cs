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
    public partial class AdminProducts : Form
    {
        public AdminProducts()
        {
            InitializeComponent();
            labelSurname.Text = CurrentUser.Surname; // Отображение фамилии текущего пользователя
        }

        // Класс для хранения инф-ции о продукте
        public class Products
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public decimal Cost { get; set; }
            public int Discount { get; set; }

            // Рассчет стоимости со скидкой
            public decimal DiscountedCost
            {
                get => Discount > 0 ? Cost * (1 - Discount / 100m) : Cost;
            }

            public string Manufacterer { get; set; }
            public string SupplierName { get; set; }  
            public string CategoryName { get; set; }
            public int QuantityInStock { get; set; }
            public string Description { get; set; }
            public int Quantity { get; set; }

            // Изображение товара
            public Image Image { get; set; }
        }

        string connect = Connect.conn;

        private int currentPage = 1;
        private int pageSize = 5;
        private int totalPages = 1;
        private int totalViewedProducts = 0;
        private List<Products> allProducts = new List<Products>();
        private List<Products> filteredProducts = new List<Products>();

        private void AdminProducts_Load(object sender, EventArgs e)
        {
            // Настройка текста поиска
            textBoxPoisk.Text = "Поиск";
            textBoxPoisk.ForeColor = SystemColors.GrayText;

            // Настройка обработчиков для кнопок пагинации
            btnPrev.Click += (s, e1) => GoToPage(currentPage - 1);
            btnNext.Click += (s, e1) => GoToPage(currentPage + 1);


            // Загрузка товаров из БД
            ListProductsFromDatabase();

            // Настройка DataGridView
            dataGridView1.Dock = DockStyle.None;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ReadOnly = true;

            // Добавление колонок
            dataGridView1.Columns.Clear();

            // Колонка с изображением
            DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
            imageColumn.HeaderText = "Изображение";
            imageColumn.ImageLayout = DataGridViewImageCellLayout.Stretch;
            imageColumn.DataPropertyName = "Image";
            dataGridView1.Columns.Add(imageColumn);

            // Текстовые колонки
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Артикул", DataPropertyName = "Id", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Название", DataPropertyName = "Name", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Цена", DataPropertyName = "Cost", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Скидка", DataPropertyName = "Discount", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Производитель", DataPropertyName = "Manufacterer", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Поставщик", DataPropertyName = "SupplierName", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });  
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Категория", DataPropertyName = "CategoryName", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });  
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Количество", DataPropertyName = "QuantityInStock", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Описание", DataPropertyName = "Description", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });

            // Подписка на события
            textBoxPoisk.TextChanged += textBoxPoisk_TextChanged;
            textBoxPoisk.Enter += textBoxPoisk_Enter;
            textBoxPoisk.Leave += textBoxPoisk_Leave;

            // Настройка сортировки
            comboBoxSort.Items.Add("Все");
            comboBoxSort.Items.Add("По возрастанию");
            comboBoxSort.Items.Add("По убыванию");
            comboBoxSort.SelectedIndex = 0;
            comboBoxSort.SelectedIndexChanged += comboBoxSort_SelectedIndexChanged;

            // Загрузка категорий для фильтра
            LoadCategories();
            comboBoxFilter.SelectedIndexChanged += comboBoxFilter_SelectedIndexChanged;

            //блокировка кнопок удалить и редактирвать
            buttonRedact.Enabled = false;
            button1.Enabled = false;
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
        }

        // Загрузка продуктов из базы данных
        private void ListProductsFromDatabase()
        {
            allProducts.Clear();
            using (MySqlConnection connection = new MySqlConnection(connect))
            {
                connection.Open();

                string query = @"
                    SELECT 
                        p.ProductArticleNumber AS Id,
                        p.ProductName AS Name,
                        p.ProductCost AS Cost,
                        p.ProductDiscount AS Discount,
                        p.ProductManufacturer AS Manufacturer,
                        s.SupplierName AS SupplierName,  
                        c.CategoryName AS CategoryName, 
                        p.ProductQuantityInStock AS QuantityInStock,
                        p.ProductDescription AS Description,
                        p.ProductPhoto AS ProductPhoto
                    FROM product p
                    LEFT JOIN supplier s ON p.ProductSupplierID = s.SupplierID
                    LEFT JOIN category c ON p.ProductCategoryID = c.CategoryID";

                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string imageFileName = reader["ProductPhoto"].ToString();
                    Image image = null;

                    if (!string.IsNullOrEmpty(imageFileName))
                    {
                        string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", imageFileName);
                        image = File.Exists(imagePath) ?
                            Image.FromFile(imagePath) :
                            Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "zaglushka.jpg"));
                    }
                    else
                    {
                        image = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "zaglushka.jpg"));
                    }

                    if (reader["Id"] != DBNull.Value &&
                        reader["Name"] != DBNull.Value &&
                        reader["Cost"] != DBNull.Value &&
                        reader["Discount"] != DBNull.Value &&
                        reader["Manufacturer"] != DBNull.Value &&
                        reader["SupplierName"] != DBNull.Value &&
                        reader["CategoryName"] != DBNull.Value &&
                        reader["QuantityInStock"] != DBNull.Value &&
                        reader["Description"] != DBNull.Value)
                    {
                        allProducts.Add(new Products
                        {
                            Id = reader["Id"].ToString(),
                            Name = reader["Name"].ToString(),
                            Cost = Convert.ToDecimal(reader["Cost"]),
                            Discount = Convert.ToInt32(reader["Discount"]),
                            Manufacterer = reader["Manufacturer"].ToString(),
                            SupplierName = reader["SupplierName"].ToString(),
                            CategoryName = reader["CategoryName"].ToString(),
                            QuantityInStock = Convert.ToInt32(reader["QuantityInStock"]),
                            Description = reader["Description"].ToString(),
                            Image = image
                        });
                    }
                }
                reader.Close();
            }
            ApplyFilterAndSort();
            UpdateRecordsCount();
        }

        private void UpdateRecordsCount()
        {
            int filteredRecords = filteredProducts.Count;

            lblRecordsInfo.Text = $"Просмотрено: {totalViewedProducts} из {filteredRecords}";
        }


        private void UpdatePagination()
        {
            totalPages = (int)Math.Ceiling((double)filteredProducts.Count / pageSize);
            if (totalPages == 0) totalPages = 1;

            if (currentPage > totalPages)
                currentPage = totalPages;

            lblPageInfo.Text = $"Страница {currentPage} из {totalPages}";

            // Обновляем состояние кнопок
            btnPrev.Enabled = currentPage > 1;
            btnNext.Enabled = currentPage < totalPages;

            UpdateDataGridView();
        }

        private void UpdateDataGridView()
        {
            var pagedProducts = filteredProducts
               .Skip((currentPage - 1) * pageSize)
               .Take(pageSize)
               .ToList();

            dataGridView1.DataSource = pagedProducts;

            // Обновляем счетчик просмотренных товаров
            if (currentPage == 1)
            {
                totalViewedProducts = pagedProducts.Count;
            }
            else if (currentPage > 1 && pagedProducts.Count > 0)
            {
                // При переходе вперед увеличиваем счетчик
                if (currentPage * pageSize > totalViewedProducts)
                {
                    totalViewedProducts += pagedProducts.Count;
                }
                // При переходе назад уменьшаем счетчик
                else if (currentPage * pageSize < totalViewedProducts)
                {
                    totalViewedProducts -= pageSize;
                }
            }

            UpdateRecordsCount();
        }

        private void GoToPage(int pageNumber)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageNumber > totalPages) pageNumber = totalPages;

            currentPage = pageNumber;
            UpdatePagination();
        }

        //блокировка кнопок удалить и редактирвать
        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            buttonRedact.Enabled = dataGridView1.SelectedRows.Count > 0;
            button1.Enabled = dataGridView1.SelectedRows.Count > 0;
        }

        // для фильтрации, поиска и сортировки
        private void comboBoxSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilterAndSort();
        }

        private void comboBoxFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilterAndSort();
        }

        private void textBoxPoisk_TextChanged(object sender, EventArgs e)
        {
            ApplyFilterAndSort();
        }

        private void ApplyFilterAndSort()
        {
            string searchText = textBoxPoisk.Text.ToLower();

            if (searchText == "поиск")
            {
                searchText = "";
            }

            string selectedCategory = comboBoxFilter.SelectedItem?.ToString();
            string selectedSort = comboBoxSort.SelectedItem?.ToString();

            filteredProducts = allProducts;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filteredProducts = filteredProducts
                    .Where(p => p.Name.ToLower().Contains(searchText) ||
                               p.Id.ToLower().Contains(searchText) ||
                               p.Manufacterer.ToLower().Contains(searchText))
                    .ToList();
            }

            if (selectedCategory != "Все категории" && selectedCategory != null)
            {
                filteredProducts = filteredProducts
                    .Where(p => p.CategoryName == selectedCategory)
                    .ToList();
            }

            switch (selectedSort)
            {
                case "По возрастанию":
                    filteredProducts = filteredProducts
                        .OrderBy(p => p.Cost)
                        .ToList();
                    break;

                case "По убыванию":
                    filteredProducts = filteredProducts
                        .OrderByDescending(p => p.Cost)
                        .ToList();
                    break;

                default:
                    filteredProducts = filteredProducts
                        .OrderBy(p => p.Id)
                        .ToList();
                    break;
            }

            totalViewedProducts = 0;
            currentPage = 1;
            UpdatePagination();
        }

        // Загрузка категорий для фильтра
        private void LoadCategories()
        {
            using (MySqlConnection connection = new MySqlConnection(connect))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand("SELECT CategoryName FROM category", connection);
                MySqlDataReader reader = command.ExecuteReader();

                comboBoxFilter.Items.Clear();
                comboBoxFilter.Items.Add("Все категории");

                while (reader.Read())
                {
                    comboBoxFilter.Items.Add(reader["CategoryName"].ToString());
                }

                reader.Close();
            }

            comboBoxFilter.SelectedIndex = 0;  
        }

        // Обработчики текстового поля поиска
        private void textBoxPoisk_Enter(object sender, EventArgs e)
        {
            if (textBoxPoisk.Text == "Поиск")
            {
                textBoxPoisk.Text = "";
                textBoxPoisk.ForeColor = Color.Black;
            }
        }

        private void textBoxPoisk_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxPoisk.Text))
            {
                textBoxPoisk.Text = "Поиск";
                textBoxPoisk.ForeColor = Color.Gray;
            }
        }

        // Добавление нового продукта
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            AddProduct form = new AddProduct();
            form.ShowDialog();

            ListProductsFromDatabase();
        }

        // Редактирование продукта
        private void buttonRedact_Click(object sender, EventArgs e)
        {
            Products selectedProduct = GetSelectedProduct();

            if (selectedProduct != null)
            {

                EditProduct form = new EditProduct(selectedProduct);
                form.ShowDialog();

                ListProductsFromDatabase();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите продукт для редактирования.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Получение выбранного продукта
        private Products GetSelectedProduct()
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView1.SelectedRows[0];
                return selectedRow.DataBoundItem as Products;
            }
            return null;

        }

        //удаление продукта
       private void button1_Click(object sender, EventArgs e)
        {
            Products selectedProduct = GetSelectedProduct();

            if (selectedProduct != null)
            {
                DialogResult result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить продукт '{selectedProduct.Name}'?",
                    "Подтверждение удаления",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        using (MySqlConnection connection = new MySqlConnection(connect))
                        {
                            connection.Open();

                            string deleteQuery = "DELETE FROM product WHERE ProductArticleNumber = @ProductArticleNumber";

                            MySqlCommand command = new MySqlCommand(deleteQuery, connection);
                            command.Parameters.AddWithValue("@ProductArticleNumber", selectedProduct.Id);

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Продукт успешно удален.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                               
                                ListProductsFromDatabase();
                            }
                            else
                            {
                                MessageBox.Show("Ошибка при удалении продукта.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show($"Ошибка при удалении продукта: Этот продукт есть в заказах.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите продукт для удаления.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Обработчики переходов между формами
        private void buttonExit_Click(object sender, EventArgs e)
        {
            Autorisation form = new Autorisation();
            form.Show();
            Hide();
        }

        private void linkLabelOtchet_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AdminOtchet form = new AdminOtchet();
            form.ShowDialog();
        }

        private void linkLabelUser_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AdminUsers form = new AdminUsers();
            form.Show();
            Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Admin form = new Admin();
            form.Show();
            Hide();
        }

        private void linkLabelOrder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AdminOrder form = new AdminOrder();
            form.Show();
            Hide();
        }

        private void linkLabelCategory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AdminSpravochnik form = new AdminSpravochnik();
            form.Show();
            Hide();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AdminClient form = new AdminClient();
            form.Show();
            Hide();
        }

    }
}