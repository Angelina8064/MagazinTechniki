using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MagazinTechniki
{
    public partial class Admin : Form
    {
        public Admin()
        {
            InitializeComponent();

            labelSurname.Text = CurrentUser.Surname;
        }

        // Переходы по формам
        private void linkLabelProducts_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AdminProducts form = new AdminProducts();
            form.Show();
            Hide();
        }

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

        private void button2_Click(object sender, EventArgs e)
        {
            AdminImportAndRestore form = new AdminImportAndRestore();
            form.ShowDialog();
        }
    }
}
