namespace MagazinTechniki
{
    partial class AddProduct
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddProduct));
            this.label2 = new System.Windows.Forms.Label();
            this.textboxNameProduct = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textboxPrice = new System.Windows.Forms.TextBox();
            this.textboxDiscount = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxProductQuantityInStock = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxProductManufacter = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxProductDescription = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.textBoxArticle = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.pictureBoxImage = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Book Antiqua", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(469, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(182, 28);
            this.label2.TabIndex = 58;
            this.label2.Text = "Наименование";
            // 
            // textboxNameProduct
            // 
            this.textboxNameProduct.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.textboxNameProduct.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textboxNameProduct.Location = new System.Drawing.Point(474, 95);
            this.textboxNameProduct.MaxLength = 50;
            this.textboxNameProduct.Name = "textboxNameProduct";
            this.textboxNameProduct.Size = new System.Drawing.Size(260, 31);
            this.textboxNameProduct.TabIndex = 59;
            this.textboxNameProduct.TextChanged += new System.EventHandler(this.textboxNameProduct_TextChanged);
            this.textboxNameProduct.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textboxNameProduct_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Book Antiqua", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(352, 139);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 28);
            this.label3.TabIndex = 60;
            this.label3.Text = "Цена";
            // 
            // textboxPrice
            // 
            this.textboxPrice.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.textboxPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textboxPrice.Location = new System.Drawing.Point(357, 170);
            this.textboxPrice.MaxLength = 6;
            this.textboxPrice.Name = "textboxPrice";
            this.textboxPrice.Size = new System.Drawing.Size(103, 31);
            this.textboxPrice.TabIndex = 63;
            this.textboxPrice.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxProductCost_KeyPress);
            // 
            // textboxDiscount
            // 
            this.textboxDiscount.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.textboxDiscount.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textboxDiscount.Location = new System.Drawing.Point(474, 170);
            this.textboxDiscount.MaxLength = 2;
            this.textboxDiscount.Name = "textboxDiscount";
            this.textboxDiscount.Size = new System.Drawing.Size(111, 31);
            this.textboxDiscount.TabIndex = 64;
            this.textboxDiscount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textboxDiscount_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Book Antiqua", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(469, 139);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 28);
            this.label4.TabIndex = 65;
            this.label4.Text = "Скидка";
            // 
            // textBoxProductQuantityInStock
            // 
            this.textBoxProductQuantityInStock.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.textBoxProductQuantityInStock.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxProductQuantityInStock.Location = new System.Drawing.Point(601, 170);
            this.textBoxProductQuantityInStock.MaxLength = 2;
            this.textBoxProductQuantityInStock.Name = "textBoxProductQuantityInStock";
            this.textBoxProductQuantityInStock.Size = new System.Drawing.Size(133, 31);
            this.textBoxProductQuantityInStock.TabIndex = 67;
            this.textBoxProductQuantityInStock.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxProductQuantityInStock_KeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Book Antiqua", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(596, 139);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(104, 28);
            this.label5.TabIndex = 68;
            this.label5.Text = "Остаток";
            // 
            // textBoxProductManufacter
            // 
            this.textBoxProductManufacter.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.textBoxProductManufacter.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxProductManufacter.Location = new System.Drawing.Point(357, 248);
            this.textBoxProductManufacter.MaxLength = 20;
            this.textBoxProductManufacter.Name = "textBoxProductManufacter";
            this.textBoxProductManufacter.Size = new System.Drawing.Size(182, 31);
            this.textBoxProductManufacter.TabIndex = 69;
            this.textBoxProductManufacter.TextChanged += new System.EventHandler(this.textBoxProductManufacter_TextChanged);
            this.textBoxProductManufacter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxProductManufacter_KeyPress);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Book Antiqua", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(352, 217);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(187, 28);
            this.label7.TabIndex = 70;
            this.label7.Text = "Производитель";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("Book Antiqua", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.Location = new System.Drawing.Point(559, 217);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(141, 28);
            this.label8.TabIndex = 71;
            this.label8.Text = "Поставщик";
            // 
            // comboBox3
            // 
            this.comboBox3.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.comboBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.25F);
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Location = new System.Drawing.Point(564, 248);
            this.comboBox3.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(170, 33);
            this.comboBox3.TabIndex = 75;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Font = new System.Drawing.Font("Book Antiqua", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label9.Location = new System.Drawing.Point(36, 302);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(212, 28);
            this.label9.TabIndex = 76;
            this.label9.Text = "Категория товара";
            // 
            // comboBox1
            // 
            this.comboBox1.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.comboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.25F);
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(41, 332);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(207, 33);
            this.comboBox1.TabIndex = 77;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.Transparent;
            this.label10.Font = new System.Drawing.Font("Book Antiqua", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label10.Location = new System.Drawing.Point(256, 303);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(207, 28);
            this.label10.TabIndex = 78;
            this.label10.Text = "Описание товара";
            // 
            // textBoxProductDescription
            // 
            this.textBoxProductDescription.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.textBoxProductDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxProductDescription.Location = new System.Drawing.Point(261, 332);
            this.textBoxProductDescription.MaxLength = 50;
            this.textBoxProductDescription.Name = "textBoxProductDescription";
            this.textBoxProductDescription.Size = new System.Drawing.Size(304, 31);
            this.textBoxProductDescription.TabIndex = 79;
            this.textBoxProductDescription.TextChanged += new System.EventHandler(this.textBoxProductDescription_TextChanged);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.SlateGray;
            this.button1.BackgroundImage = global::MagazinTechniki.Properties.Resources.Выход;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Book Antiqua", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button1.ForeColor = System.Drawing.Color.Black;
            this.button1.Location = new System.Drawing.Point(41, 17);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(43, 43);
            this.button1.TabIndex = 80;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonAdd
            // 
            this.buttonAdd.BackColor = System.Drawing.Color.SlateGray;
            this.buttonAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAdd.Font = new System.Drawing.Font("Book Antiqua", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonAdd.ForeColor = System.Drawing.Color.Black;
            this.buttonAdd.Location = new System.Drawing.Point(586, 303);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(148, 62);
            this.buttonAdd.TabIndex = 81;
            this.buttonAdd.Text = "Добавить";
            this.buttonAdd.UseVisualStyleBackColor = false;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // textBoxArticle
            // 
            this.textBoxArticle.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.textBoxArticle.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxArticle.Location = new System.Drawing.Point(357, 96);
            this.textBoxArticle.MaxLength = 6;
            this.textBoxArticle.Name = "textBoxArticle";
            this.textBoxArticle.Size = new System.Drawing.Size(103, 31);
            this.textBoxArticle.TabIndex = 57;
            this.textBoxArticle.TextChanged += new System.EventHandler(this.textBoxArticle_TextChanged);
            this.textBoxArticle.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxArticle_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Book Antiqua", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(352, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 28);
            this.label1.TabIndex = 56;
            this.label1.Text = "Артикул";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Book Antiqua", 20.25F);
            this.label6.Location = new System.Drawing.Point(142, 31);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 32);
            this.label6.TabIndex = 55;
            this.label6.Text = "Фото";
            // 
            // pictureBoxImage
            // 
            this.pictureBoxImage.BackColor = System.Drawing.Color.SlateGray;
            this.pictureBoxImage.Location = new System.Drawing.Point(41, 65);
            this.pictureBoxImage.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBoxImage.Name = "pictureBoxImage";
            this.pictureBoxImage.Size = new System.Drawing.Size(306, 225);
            this.pictureBoxImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxImage.TabIndex = 54;
            this.pictureBoxImage.TabStop = false;
            this.pictureBoxImage.Click += new System.EventHandler(this.pictureBoxImage_Click);
            // 
            // AddProduct
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::MagazinTechniki.Properties.Resources.ФонДляОкон;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(778, 390);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBoxProductDescription);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.comboBox3);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBoxProductManufacter);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxProductQuantityInStock);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textboxDiscount);
            this.Controls.Add(this.textboxPrice);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textboxNameProduct);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxArticle);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.pictureBoxImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddProduct";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Добавление продукта";
            this.Load += new System.EventHandler(this.AddProduct_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textboxNameProduct;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textboxPrice;
        private System.Windows.Forms.TextBox textboxDiscount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxProductQuantityInStock;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxProductManufacter;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxProductDescription;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.TextBox textBoxArticle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox pictureBoxImage;
    }
}