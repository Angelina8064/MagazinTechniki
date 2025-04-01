using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MagazinTechniki
{
    public partial class AdminSpravochnik : Form
    {
        public AdminSpravochnik()
        {
            InitializeComponent();
            labelSurname.Text = CurrentUser.Surname; // Отображение фамилии текущего пользователя
        }

        // Класс для хранения категорий
        public class Category
        {
            public int CategoryID { get; set; }
            public string CategoryName { get; set; }
        }

        // Класс для хранения ролей
        public class Role
        {
            public int RoleID { get; set; }
            public string RoleName { get; set; }
        }

        // Класс для хранения поставщиков
        public class Supplier
        {
            public int SupplierID { get; set; }
            public string SupplierName { get; set; }
        }

        string connect = Connect.conn;

        // Коллекции для данных
        private List<Category> category = new List<Category>();
        private List<Role> role = new List<Role>();
        private List<Supplier> supplier = new List<Supplier>();

        private void AdminSpravochnik_Load(object sender, EventArgs e)
        {
            // Загрузка данных
            ListCategoryFromDatabase();
            ListRoleFromDatabase();
            ListSupplierFromDatabase();

            // Настройка таблицы категорий
            dataGridView1.Dock = DockStyle.None;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Название категории", DataPropertyName = "CategoryName" });


            dataGridView2.Dock = DockStyle.None;
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.AllowUserToDeleteRows = false;
            dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView2.AutoGenerateColumns = false;
            dataGridView2.ReadOnly = true;
            dataGridView2.RowHeadersVisible = false;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.Columns.Clear();
                  
            dataGridView2.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Название роли", DataPropertyName = "RoleName" });


            dataGridView3.Dock = DockStyle.None;
            dataGridView3.AllowUserToAddRows = false;
            dataGridView3.AllowUserToDeleteRows = false;
            dataGridView3.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView3.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView3.AutoGenerateColumns = false;
            dataGridView3.ReadOnly = true;
            dataGridView3.RowHeadersVisible = false;
            dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView3.Columns.Clear();

            dataGridView3.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Название поставщика", DataPropertyName = "SupplierName" });

            //блокировка кнопок удалить и редактирвать
            buttonRedact.Enabled = false;
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;

            button4.Enabled = false;
            dataGridView2.SelectionChanged += DataGridView2_SelectionChanged;

            button7.Enabled = false;
            dataGridView3.SelectionChanged += DataGridView3_SelectionChanged;
        }

        // Загрузка категорий из БД
        #region Category
        private void ListCategoryFromDatabase()
        {
            category.Clear();
            using (MySqlConnection connection = new MySqlConnection(connect))
            {
                connection.Open();

                string query = "SELECT CategoryID, CategoryName FROM category";

                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (reader["CategoryID"] != DBNull.Value &&
                        reader["CategoryName"] != DBNull.Value)
                    {
                        int categoryID = Convert.ToInt32(reader["CategoryID"]);
                        string categoryName = reader["CategoryName"].ToString();

                        category.Add(new Category
                        {
                            CategoryID = categoryID,
                            CategoryName = categoryName
                        }); ;
                    }
                }
                reader.Close();
            }
            dataGridView1.DataSource = null; 
            dataGridView1.DataSource = category; 
        }

        // Добавление новой категории
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            AddCategory form = new AddCategory();
            form.ShowDialog();

            ListCategoryFromDatabase();
        }

        // Редактирование категории
        private void buttonRedact_Click(object sender, EventArgs e)
        {

            Category selectedCategory = GetSelectedCategory();

            if (selectedCategory != null)
            {

                EditCategory form = new EditCategory(selectedCategory);
                form.ShowDialog();

                ListCategoryFromDatabase();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите категорию для редактирования.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private Category GetSelectedCategory()
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView1.SelectedRows[0];

                return selectedRow.DataBoundItem as Category;
            }
            return null;

        }
        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            buttonRedact.Enabled = dataGridView1.SelectedRows.Count > 0;
        }
        #endregion


        // Загрузка ролей из БД
        #region Role
        private void ListRoleFromDatabase()
        {
            role.Clear();
            using (MySqlConnection connection = new MySqlConnection(connect))
            {
                connection.Open();

                string query = "SELECT RoleID, RoleName FROM role";

                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (reader["RoleID"] != DBNull.Value &&
                        reader["RoleName"] != DBNull.Value)
                    {
                        int roleID = Convert.ToInt32(reader["RoleID"]);
                        string roleName = reader["RoleName"].ToString();

                        role.Add(new Role
                        {
                            RoleID = roleID,
                            RoleName = roleName
                        });
                    }
                }
                reader.Close();
            }
            dataGridView2.DataSource = null;
            dataGridView2.DataSource = role;
        }

        // Добавление ролей
        private void button5_Click(object sender, EventArgs e)
        {
            AddRole form = new AddRole();
            form.ShowDialog();

            ListRoleFromDatabase();
        }

        // Редактирование роли
        private void button4_Click(object sender, EventArgs e)
        {
            Role selectedRole = GetSelectedRole();

            if (selectedRole != null)
            {

                EditRole form = new EditRole(selectedRole);
                form.ShowDialog();

                ListRoleFromDatabase();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите роль для редактирования.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Выбранная строка
        private Role GetSelectedRole()
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView2.SelectedRows[0];

                return selectedRow.DataBoundItem as Role;
            }
            return null;

        }

        private void DataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            button4.Enabled = dataGridView2.SelectedRows.Count > 0;
        }
        #endregion


        // Загрузка поставщиков из БД
        #region Supplierr
        private void ListSupplierFromDatabase()
        {
            supplier.Clear();
            using (MySqlConnection connection = new MySqlConnection(connect))
            {
                connection.Open();

                string query = "SELECT SupplierID, SupplierName FROM supplier";

                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (reader["SupplierID"] != DBNull.Value &&
                        reader["SupplierName"] != DBNull.Value)
                    {
                        int supplierID = Convert.ToInt32(reader["SupplierID"]);
                        string supplierName = reader["SupplierName"].ToString();

                        supplier.Add(new Supplier
                        {
                            SupplierID = supplierID,
                            SupplierName = supplierName
                        });
                    }
                }
                reader.Close();
            }
            dataGridView3.DataSource = null;
            dataGridView3.DataSource = supplier;
        }        
        
        // Добавление поставщиков
        private void button6_Click(object sender, EventArgs e)
        {
            AddSupplier form = new AddSupplier();
            form.ShowDialog();

            ListSupplierFromDatabase();
        }

        // Редактирование поставщиков
        private void button7_Click(object sender, EventArgs e)
        {
            Supplier selectedSupplier = GetSelectedSupplier();

            if (selectedSupplier != null)
            {

                EditSupplier form = new EditSupplier(selectedSupplier);
                form.ShowDialog();

                ListSupplierFromDatabase();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите категорию для редактирования.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private Supplier GetSelectedSupplier()
        {
            if (dataGridView3.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView3.SelectedRows[0];

                return selectedRow.DataBoundItem as Supplier;
            }
            return null;
        }

        private void DataGridView3_SelectionChanged(object sender, EventArgs e)
        {
            button7.Enabled = dataGridView3.SelectedRows.Count > 0;
        }
        #endregion

        #region Переходы по формам
        private void buttonExit_Click(object sender, EventArgs e)
        {
            Autorisation form = new Autorisation();
            form.Show();
            Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Admin form = new Admin();
            form.Show();
            Hide();
        }

        private void linkLabelProducts_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AdminProducts form = new AdminProducts();
            form.Show();
            Hide();
        }

        private void linkLabelOrder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AdminOrder form = new AdminOrder();
            form.Show();
            Hide();
        }

        private void linkLabelUser_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AdminUsers form = new AdminUsers();
            form.Show();
            Hide();
        }

        private void linkLabelOtchet_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AdminOtchet form = new AdminOtchet(); 
            form.ShowDialog();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AdminClient form = new AdminClient();
            form.Show();
            Hide();
        }
        #endregion

    }
}
