using System;
using System.Windows.Forms;
using ShedLab.Models;


namespace AzureQueueClientTest {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {

            string connectionString = "[AZURE STORAGE ACCOUNT CONNECTION STRING]";
            string queueName = "[QUEUE NAME]";

            var queue = new ShedLab.AzureQueueClient(connectionString, queueName, ShedLab.AzureQueueClient.QueueMode.Peek, 10);

            var list = queue.GetItems<Customer>();

        }
    }
}
