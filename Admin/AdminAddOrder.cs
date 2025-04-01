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
    public partial class AdminAddOrder : Form
    {
     
        public AdminAddOrder()
        {
            InitializeComponent();
        }

        // Класс для хранения выбранных товаров
        public class Cart
        {
            string connect = Connect.conn;
            private Dictionary<Products, int> items = new Dictionary<Products, int>();

            // Добавление товара в корзину
            public void AddProduct(string productId, List<Products> allProducts)
            {
                var product = allProducts.FirstOrDefault(p => p.Id == productId);
                if (product != null)
                {
                    // Проверяем, есть ли товар на складе
                    if (product.QuantityInStock <= 0)
                    {
                        MessageBox.Show($"Товар '{product.Name}' отсутствует на складе.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return; // Прекращаем выполнение метода, если товара нет в наличии
                    }

                    // Загружаем актуальную скидку из БД
                    product.Discount = GetDiscountFromDatabase(product.Id);

                    if (items.ContainsKey(product))
                    {
                        // Проверяем, не превышает ли текущее количество в корзине доступное на складе
                        if (items[product] >= product.QuantityInStock)
                        {
                            MessageBox.Show($"Невозможно добавить больше товара '{product.Name}'. На складе осталось только {product.QuantityInStock} шт.",
                                          "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        items[product]++;
                        product.Quantity++;
                    }
                    else
                    {
                        items[product] = 1;
                        product.Quantity = 1;
                    }
                }
            }

            // Получение текущей скидки из БД
            private int GetDiscountFromDatabase(string productId)
            {
                using (MySqlConnection connection = new MySqlConnection(connect))
                {
                    connection.Open();
                   
                    string query = "SELECT ProductDiscount FROM product WHERE ProductArticleNumber = @ProductArticleNumber";
                    
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ProductArticleNumber", productId);
                   
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }

            // Получение списка товаров в корзине
            public List<Products> GetItems()
            {
                return items.Select(i => new Products
                {
                    Id = i.Key.Id,
                    Name = i.Key.Name,
                    Cost = i.Key.Cost,
                    Discount = i.Key.Discount,
                    Manufacterer = i.Key.Manufacterer,
                    SupplierName = i.Key.SupplierName,
                    CategoryName = i.Key.CategoryName,
                    QuantityInStock = i.Key.QuantityInStock,
                    Description = i.Key.Description,
                    Image = i.Key.Image,
                    Quantity = i.Value
                }).ToList();
            }

            // Получение общего количества товаров
            public int GetTotalItemsCount()
            {
                return items.Sum(item => item.Value);
            }
        }

        string connect = Connect.conn;
        private List<Products> products = new List<Products>();  // Список всех товаров
        private Cart cart = new Cart(); // Корзина заказа

        private void AdminAddOrder_Load(object sender, EventArgs e)
        {
            // Настройка интерфейса
            textBoxPoisk.Text = "Поиск";
            textBoxPoisk.ForeColor = SystemColors.GrayText;

            // Загрузка товаров из БД
            ListProductsFromDatabase();

            // Настройка DataGridView
            dataGridView1.Dock = DockStyle.None;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Добавление колонок
            dataGridView1.Columns.Clear();

            // Колонка с изображением
            DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
            imageColumn.HeaderText = "Изображение";
            imageColumn.ImageLayout = DataGridViewImageCellLayout.Stretch;
            imageColumn.DataPropertyName = "Image";
            imageColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            imageColumn.Width = 110;
            dataGridView1.Columns.Add(imageColumn);

            // Текстовые колонки
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Артикул", DataPropertyName = "Id", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Название", DataPropertyName = "Name", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Цена", DataPropertyName = "Cost", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Сикдка", DataPropertyName = "Discount", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Остаток", DataPropertyName = "QuantityInStock", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Производитель", DataPropertyName = "Manufacterer", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Категория", DataPropertyName = "CategoryName", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });

            // Кнопка "В корзину"
            DataGridViewButtonColumn addToCartButton = new DataGridViewButtonColumn
            {
                Name = " ",
                Text = "В корзину",
                UseColumnTextForButtonValue = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = 100
            };

            dataGridView1.Columns.Add(addToCartButton);
            dataGridView1.CellPainting += DataGridView1_CellPainting;

            Label labelTotalPrice = new Label
            {
                AutoSize = true,
                Location = new Point(10, dataGridView1.Bottom + 10)
            };
            this.Controls.Add(labelTotalPrice);

            UpdateTotalPriceDisplay();

            textBoxPoisk.TextChanged += textBoxPoisk_TextChanged;
            textBoxPoisk.Enter += textBoxPoisk_Enter;
            textBoxPoisk.Leave += textBoxPoisk_Leave;

            // Настройка фильтров и сортировки
            comboBoxSort.Items.Add("По возрастанию");
            comboBoxSort.Items.Add("По убыванию");
            comboBoxSort.SelectedIndex = 0;
            comboBoxSort.SelectedIndexChanged += comboBoxSort_SelectedIndexChanged;

            LoadCategories(); // Загрузка категорий для фильтра
            comboBoxFilter.SelectedIndexChanged += comboBoxFilter_SelectedIndexChanged;


            button2.Click += button2_Click;
            ApplyFilterAndSort(); // Применение фильтров
        }

        private void DataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dataGridView1.Columns[" "].Index)
            {
                e.PaintBackground(e.CellBounds, true);

                using (Button btn = new Button())
                {
                    btn.Text = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.BackColor = Color.SlateGray; 
                    btn.ForeColor = Color.Black;     
                    btn.Font = new Font("Book Antiqua", 15f, FontStyle.Regular, GraphicsUnit.Point);
                    btn.Size = e.CellBounds.Size;   

                    Bitmap bmp = new Bitmap(btn.Width, btn.Height);
                    btn.DrawToBitmap(bmp, new Rectangle(0, 0, btn.Width, btn.Height));
                    e.Graphics.DrawImage(bmp, e.CellBounds.Location);
                }
                e.Handled = true;
            }
        }

        // Загрузка товаров из базы данных
        private void ListProductsFromDatabase()
        {
            products.Clear();
            using (MySqlConnection connection = new MySqlConnection(connect))
            {
                connection.Open();

                // SQL-запрос с JOIN для получения данных о товарах
                string query = @"
                                SELECT 
                                    p.ProductArticleNumber AS Id,
                                    p.ProductName AS Name,
                                    p.ProductCost AS Cost,
                                    p.ProductDiscount AS Discount,
                                    p.ProductManufacturer AS Manufacturer, 
                                    p.ProductQuantityInStock AS QuantityInStock, 
                                    c.CategoryName AS CategoryName,
                                    p.ProductPhoto AS ProductPhoto
                                FROM product p
                                LEFT JOIN category c ON p.ProductCategoryID = c.CategoryID";

                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    // Загрузка изображения товара
                    string imageFileName = reader["ProductPhoto"].ToString();

                    Image image = null;

                    if (!string.IsNullOrEmpty(imageFileName))
                    {
                        string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", imageFileName);

                        if (File.Exists(imagePath))
                        {
                            image = Image.FromFile(imagePath);
                        }
                        else
                        {
                            image = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "zaglushka.jpg"));
                        }
                    }
                    else
                    {
                        image = Image.FromFile("zaglushka.jpg");
                    }

                    if (reader["Id"] != DBNull.Value &&
                        reader["Name"] != DBNull.Value &&
                        reader["Cost"] != DBNull.Value &&
                        reader["Discount"] != DBNull.Value &&
                        reader["QuantityInStock"] != DBNull.Value &&
                        reader["Manufacturer"] != DBNull.Value &&
                        reader["CategoryName"] != DBNull.Value)
                    {
                        string id = reader["Id"].ToString();
                        string name = reader["Name"].ToString();
                        decimal cost = Convert.ToDecimal(reader["Cost"]);
                        int discount = Convert.ToInt32(reader["Discount"]);
                        int quantityInStock = Convert.ToInt32(reader["QuantityInStock"]);
                        string manufacturer = reader["Manufacturer"].ToString();
                        string categoryName = reader["CategoryName"].ToString();

                        // Создание объекта товара
                        products.Add(new Products
                        {
                            Id = id,
                            Name = name,
                            Cost = cost,
                            Discount = discount,
                            QuantityInStock = quantityInStock,
                            Manufacterer = manufacturer,
                            CategoryName = categoryName,
                            Image = image
                        });
                    }
                }
                reader.Close();
            }
            ApplyFilterAndSort();
        }

        // Обработчик нажатия кнопки "В корзину"
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                var column = dataGridView1.Columns[e.ColumnIndex];
                if (column is DataGridViewButtonColumn && column.HeaderText == " ")
                {
                    // Получаем товар из отфильтрованного списка
                    var selectedProduct = (Products)dataGridView1.Rows[e.RowIndex].DataBoundItem;
                    cart.AddProduct(selectedProduct.Id, products);

                    UpdateTotalPriceDisplay(); // Обновление информации о корзине
                }
            }
        }

        // Обновление отображения информации о корзине
        private void UpdateTotalPriceDisplay()
        {
            int totalItems = cart.GetTotalItemsCount();
            labelTotalQuantity.Text = $"Количество товаров в корзине: {totalItems}";
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

        // Применение фильтров и сортировки
        private void ApplyFilterAndSort()
        {
            string searchText = textBoxPoisk.Text.ToLower();

            if (searchText == "поиск")
            {
                searchText = "";
            }

            string selectedCategory = comboBoxFilter.SelectedItem?.ToString();

            var filteredProducts = products;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filteredProducts = filteredProducts
                    .Where(p => p.Name.ToLower().Contains(searchText))
                    .ToList();
            }

            if (selectedCategory != "Все категории")
            {
                filteredProducts = filteredProducts
                    .Where(p => p.CategoryName == selectedCategory)
                    .ToList();
            }

            // Сортировка
            if (comboBoxSort.SelectedItem?.ToString() == "По возрастанию")
            {
                filteredProducts = filteredProducts
                    .OrderBy(p => p.Id)
                    .ToList();
            }
            else if (comboBoxSort.SelectedItem?.ToString() == "По убыванию")
            {
                filteredProducts = filteredProducts
                    .OrderByDescending(p => p.Id)
                    .ToList();
            }

            dataGridView1.DataSource = filteredProducts;
        }
                
        // Загрузка категорий из БД
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

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Открытие формы корзины
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // Получаем список товаров в корзине
            var itemsInCart = cart.GetItems();

            // Проверяем, есть ли товары в корзине
            if (itemsInCart.Count == 0)
            {
                MessageBox.Show("Корзина пуста! Добавьте товары перед оформлением заказа.",
                              "Пустая корзина",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
                return;
            }

            // Если товары есть - открываем форму корзины
            AdminAddOrderKorzina form = new AdminAddOrderKorzina(itemsInCart, this);
            form.Show();
        }

        private void textBoxPoisk_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true; // Блокируем неразрешенные символы
            }
        }
    }
}
