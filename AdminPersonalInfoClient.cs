using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MagazinTechniki.AdminClient;

namespace MagazinTechniki
{
    public partial class AdminPersonalInfoClient : Form
    {
        public AdminPersonalInfoClient(Client client)
        {
            InitializeComponent();

            selectedClient = client; // Сохраняем выбранного клиента 
            AdminPersonalInfoClient_Load(); // Загружаем данные клиента

        }

        string connect = Connect.conn;
        private Client selectedClient;

        private void AdminPersonalInfoClient_Load()
        {
            // Заполнение данными 
            textBoxName.Text = selectedClient.ClientName;
            textBoxSurname.Text = selectedClient.ClientSurname;
            maskedTextBoxTelephone.Text = selectedClient.ClientTelephone;
            maskedTextBoxTelephone.Mask = "+7 (000) 000-00-00"; // Установка маски для телефона
            textBoxEmail.Text = selectedClient.ClientEmail;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
