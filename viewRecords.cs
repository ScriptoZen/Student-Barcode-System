using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KimToo;
using MySql.Data.MySqlClient;

namespace SAMS_v3._0
{
    public partial class viewRecords : Form
    {
        MySqlConnection con = new MySqlConnection("Server=localhost; Database=nibm_att; user=root; Pwd=;  SslMode=none");
        public viewRecords()
        {
            InitializeComponent();
            MySqlDataAdapter adp = new MySqlDataAdapter("select * from std_att", con);
            DataTable dt = new DataTable();
            con.Open();
            adp.Fill(dt);
            con.Close();
            guna2DataGridView1.DataSource = dt;
        }

        public void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
            
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {

            EasyHTMLReports frm = new EasyHTMLReports();
           // frm.AddImage(pbLogo.Image, "width=254");

            frm.AddLineBreak();
            frm.AddString("<h3>Attendence Report</h3>");

            frm.AddHorizontalRule();
            frm.AddDatagridView(guna2DataGridView1);
            frm.ShowPrintPreviewDialog();
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
         
        }
    }
}
