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
    public partial class Manager : Form
    {
        public Manager()
        {
            InitializeComponent();
            labelSurname.Text = CurrentUser.Surname;
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

        private void linkLabelProducts_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ManagerProducts form = new ManagerProducts();
            form.Show();
            Hide();
        }

        private void linkLabelUser_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ManagerClients form = new ManagerClients();
            form.Show();
            Hide();
        }

        private void linkLabelOtchet_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AdminOtchet form = new AdminOtchet();
            form.ShowDialog();
        }
    }
}
