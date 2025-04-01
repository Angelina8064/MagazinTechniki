
namespace MagazinTechniki
{
    partial class FormImportAndRestore
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormImportAndRestore));
            this.label3 = new System.Windows.Forms.Label();
            this.buttonRestore = new System.Windows.Forms.Button();
            this.buttonDirectory = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.textboxDirectory = new System.Windows.Forms.TextBox();
            this.comboBoxTable = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonImport = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Book Antiqua", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(115, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(387, 58);
            this.label3.TabIndex = 21;
            this.label3.Text = "Администратор";
            // 
            // buttonRestore
            // 
            this.buttonRestore.BackColor = System.Drawing.Color.SlateGray;
            this.buttonRestore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRestore.Font = new System.Drawing.Font("Book Antiqua", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonRestore.ForeColor = System.Drawing.Color.Black;
            this.buttonRestore.Location = new System.Drawing.Point(184, 115);
            this.buttonRestore.Name = "buttonRestore";
            this.buttonRestore.Size = new System.Drawing.Size(244, 43);
            this.buttonRestore.TabIndex = 78;
            this.buttonRestore.Text = "Восстановление БД";
            this.buttonRestore.UseVisualStyleBackColor = false;
            // 
            // buttonDirectory
            // 
            this.buttonDirectory.BackColor = System.Drawing.Color.SlateGray;
            this.buttonDirectory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonDirectory.Font = new System.Drawing.Font("Book Antiqua", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonDirectory.ForeColor = System.Drawing.Color.Black;
            this.buttonDirectory.Location = new System.Drawing.Point(450, 268);
            this.buttonDirectory.Name = "buttonDirectory";
            this.buttonDirectory.Size = new System.Drawing.Size(77, 31);
            this.buttonDirectory.TabIndex = 77;
            this.buttonDirectory.Text = "Обзор";
            this.buttonDirectory.UseVisualStyleBackColor = false;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.SlateGray;
            this.button1.BackgroundImage = global::MagazinTechniki.Properties.Resources.Выход;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Book Antiqua", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button1.ForeColor = System.Drawing.Color.Black;
            this.button1.Location = new System.Drawing.Point(93, 314);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(43, 43);
            this.button1.TabIndex = 79;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textboxDirectory
            // 
            this.textboxDirectory.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.textboxDirectory.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textboxDirectory.Location = new System.Drawing.Point(93, 268);
            this.textboxDirectory.MaxLength = 50;
            this.textboxDirectory.Name = "textboxDirectory";
            this.textboxDirectory.Size = new System.Drawing.Size(351, 31);
            this.textboxDirectory.TabIndex = 84;
            // 
            // comboBoxTable
            // 
            this.comboBoxTable.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.comboBoxTable.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.25F);
            this.comboBoxTable.FormattingEnabled = true;
            this.comboBoxTable.Location = new System.Drawing.Point(93, 230);
            this.comboBoxTable.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxTable.Name = "comboBoxTable";
            this.comboBoxTable.Size = new System.Drawing.Size(434, 33);
            this.comboBoxTable.TabIndex = 85;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Book Antiqua", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(237, 177);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(149, 41);
            this.label1.TabIndex = 86;
            this.label1.Text = "Импорт";
            // 
            // buttonImport
            // 
            this.buttonImport.BackColor = System.Drawing.Color.SlateGray;
            this.buttonImport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonImport.Font = new System.Drawing.Font("Book Antiqua", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonImport.ForeColor = System.Drawing.Color.Black;
            this.buttonImport.Location = new System.Drawing.Point(371, 313);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(156, 43);
            this.buttonImport.TabIndex = 87;
            this.buttonImport.Text = "Импорт";
            this.buttonImport.UseVisualStyleBackColor = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Book Antiqua", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(86, 136);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(450, 41);
            this.label2.TabIndex = 88;
            this.label2.Text = "________________________";
            // 
            // FormImportAndRestore
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::MagazinTechniki.Properties.Resources.c71ba709ae020a84c7d33900bde09b41;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(619, 405);
            this.Controls.Add(this.buttonImport);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxTable);
            this.Controls.Add(this.textboxDirectory);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonRestore);
            this.Controls.Add(this.buttonDirectory);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormImportAndRestore";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormImportAndRestore";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonRestore;
        private System.Windows.Forms.Button buttonDirectory;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textboxDirectory;
        private System.Windows.Forms.ComboBox comboBoxTable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.Label label2;
    }
}