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
    public partial class ManagerProducts : Form
    {
        public ManagerProducts()
        {
            InitializeComponent();
            labelSurname.Text = CurrentUser.Surname; // Отображение фамилии текущего пользователя
        }

        string connect = Connect.conn;
        private List<Products> products = new List<Products>(); // Список продуктов

        private void ManagerProducts_Load(object sender, EventArgs e)
        {
            //Настройка поиска
            textBoxPoisk.Text = "Поиск";
            textBoxPoisk.ForeColor = SystemColors.GrayText;

            // Обновление 
            ListProductsFromDatabase();

            // Настройка гридвью
            dataGridView1.Dock = DockStyle.None;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ReadOnly = true;

            dataGridView1.Columns.Clear();

            // Колонка с изображением
            DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
            imageColumn.HeaderText = "Изображение";
            imageColumn.ImageLayout = DataGridViewImageCellLayout.Stretch;
            imageColumn.DataPropertyName = "Image";
            dataGridView1.Columns.Add(imageColumn);

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Артикул", DataPropertyName = "Id", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Название", DataPropertyName = "Name", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Цена", DataPropertyName = "Cost", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Скидка", DataPropertyName = "Discount", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Производитель", DataPropertyName = "Manufacterer", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Поставщик", DataPropertyName = "SupplierName", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Категория", DataPropertyName = "CategoryName", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Количество", DataPropertyName = "QuantityInStock", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Описание", DataPropertyName = "Description", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });

            // привязка данных
            textBoxPoisk.TextChanged += textBoxPoisk_TextChanged;
            textBoxPoisk.Enter += textBoxPoisk_Enter;
            textBoxPoisk.Leave += textBoxPoisk_Leave;


            // Сортировка
            comboBoxSort.Items.Add("Все");
            comboBoxSort.Items.Add("По возрастанию");
            comboBoxSort.Items.Add("По убыванию");
            comboBoxSort.SelectedIndex = 0;
            comboBoxSort.SelectedIndexChanged += comboBoxSort_SelectedIndexChanged;

            // Загрузка категорий
            LoadCategories();
            comboBoxFilter.SelectedIndexChanged += comboBoxFilter_SelectedIndexChanged;

            buttonExit.Click += buttonExit_Click;
        }

        // Загрузка товаров из БД
        private void ListProductsFromDatabase()
        {
            products.Clear();
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
                        string id = reader["Id"].ToString();
                        string name = reader["Name"].ToString();
                        decimal cost = Convert.ToDecimal(reader["Cost"]);
                        int discount = Convert.ToInt32(reader["Discount"]);
                        string manufacturer = reader["Manufacturer"].ToString();
                        string supplierName = reader["SupplierName"].ToString();
                        string categoryName = reader["CategoryName"].ToString();
                        int quantityInStock = Convert.ToInt32(reader["QuantityInStock"]);
                        string description = reader["Description"].ToString();

                        products.Add(new Products
                        {
                            Id = id,
                            Name = name,
                            Cost = cost,
                            Discount = discount,
                            Manufacterer = manufacturer,
                            SupplierName = supplierName,
                            CategoryName = categoryName,
                            QuantityInStock = quantityInStock,
                            Description = description,
                            Image = image
                        });
                    }
                }
                reader.Close();
            }
            ApplyFilterAndSort();
        }

        // Применение фильтров и сортировки
        private void comboBoxFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilterAndSort();
        } 
        private void comboBoxSort_SelectedIndexChanged(object sender, EventArgs e)
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

            // Применяем фильтрацию и поиск
            var filteredProducts = products;

            // Фильтрация по поиску
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filteredProducts = filteredProducts
                    .Where(p => p.Name.ToLower().Contains(searchText))
                    .ToList();
            }

            // Фильтрация по категории
            if (selectedCategory != "Все категории")
            {
                filteredProducts = filteredProducts
                    .Where(p => p.CategoryName == selectedCategory)
                    .ToList();
            }

            // Применяем сортировку к уже отфильтрованным данным
            string selectedSort = comboBoxSort.SelectedItem?.ToString();
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

                case "Все":
                    // Сортировка по артикулу (по умолчанию)
                    filteredProducts = filteredProducts
                        .OrderBy(p => p.Id)
                        .ToList();
                    break;
            }

            // Обновляем DataGridView
            dataGridView1.DataSource = filteredProducts;
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

        #region переходы на формы
        private void button2_Click(object sender, EventArgs e)
        {
            Manager form = new Manager();
            form.Show();
            Hide();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Autorisation form = new Autorisation();
            form.Show();
            Hide();
        }

        private void linkLabelOrder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ManagerOrder form = new ManagerOrder();
            form.Show();
            Hide();
        }

        private void linkLabelOtchet_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AdminOtchet form = new AdminOtchet();
            form.ShowDialog();
        }
        #endregion

    }
}
