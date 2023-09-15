using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace TourSelectionRabbitMQ
{
    public partial class Form1 : Form
    {
        private Tour _tour;
        public Form1()
        {
            InitializeComponent();
            _tour = new Tour();
            comboBox1.Items.Add(TourType.fed);
            comboBox1.Items.Add(TourType.kedelig);
            comboBox1.SelectedIndex = 0;            
        }


        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox2.TextChanged += (sender, e) =>
            {
                _tour.Name = textBox2.Text;
            };
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            textBox1.TextChanged += (sender, e) =>
            {
                _tour.Email = textBox1.Text;
            };
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _tour.Type = (TourType)comboBox1.SelectedItem;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            _tour.Book = checkBox2.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            _tour.Cancel = checkBox1.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new RabbitEmit().Emit(_tour);
        }
    }
}
