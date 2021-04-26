using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;

namespace SAMS_v3._0
{
    public partial class addstudent : UserControl
    {
        MySqlConnection con;
        MySqlCommand cmd;
        MySqlDataReader dr;
        MySqlDataAdapter da;
        private static addstudent _instance;
        public static addstudent Instance
        {
            get
            {

                if (_instance == null)
                    _instance = new addstudent();
                return _instance;
            }
        }
        public addstudent()
        {
            InitializeComponent();
            con = new MySqlConnection("Server=localhost; Database=nibm_att; user=root; Pwd=;  SslMode=none");
        }

        private void addstudent_Load(object sender, EventArgs e)
        {

        }

        private void btnAddData_Click(object sender, EventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MemoryStream ms = new MemoryStream();
            pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat);
            byte[] img = ms.ToArray();

            String insertQuery = "INSERT INTO std_info  (nibm_id, nic, name, address, number, batch,image) VALUES(@nibm_id, @nic, @name, @address, @number, @batch, @image)";

            con.Open();

            cmd = new MySqlCommand(insertQuery, con);

            cmd.Parameters.Add("@nibm_id", MySqlDbType.VarChar, 20);
            cmd.Parameters.Add("@nic", MySqlDbType.VarChar, 20);
            cmd.Parameters.Add("@name", MySqlDbType.VarChar, 250);
            cmd.Parameters.Add("@address", MySqlDbType.VarChar, 250);
            cmd.Parameters.Add("@number", MySqlDbType.Int32);
            cmd.Parameters.Add("@batch", MySqlDbType.VarChar,15);
            cmd.Parameters.Add("@image", MySqlDbType.Blob);

            cmd.Parameters["@nibm_id"].Value = textBox1.Text;
            cmd.Parameters["@nic"].Value = textBox2.Text;
            cmd.Parameters["@name"].Value = textBox3.Text;
            cmd.Parameters["@address"].Value = textBox4.Text;
            cmd.Parameters["@number"].Value = textBox5.Text;
            cmd.Parameters["@batch"].Value = textBox6.Text;
            cmd.Parameters["@image"].Value = img;

            if (cmd.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Data Inserted");
            }

            con.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Filter = "Choose Image(*.jpg; *.png; *.gif)|*.jpg; *.png; *.gif";
            if (opf.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(opf.FileName);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
