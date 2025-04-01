using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MagazinTechniki
{
    public partial class FormImportAndRestore : Form
    {
        public FormImportAndRestore()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Autorisation form = new Autorisation();
            form.Show();
            Hide();
        }
    }
}
