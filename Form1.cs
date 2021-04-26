using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using MySql.Data.MySqlClient;
using System.IO;
using ZXing;

namespace SAMS_v3._0
{
    public partial class SAMS : Form
    {
        MySqlConnection con;
        MySqlCommand cmd;
        MySqlDataReader dr;
        MySqlDataAdapter da;

        private static SAMS _instance;
        public static SAMS Instance
        {
            get
            {

                if (_instance == null)
                    _instance = new SAMS();
                return _instance;
            }
        }
        public SAMS()
        {
            InitializeComponent();
            con = new MySqlConnection("Server=localhost; Database=nibm_att; user=root; Pwd=;  SslMode=none");
            btnStop.Hide();
            btnDarlMode.Hide();
            pnlAddStudents.Hide();

            //hello


        }
        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice videoCaptureDevice;


        private void btnClose_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure You want to Close? ", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                this.Close();
            }
            
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            if (pnlAddStudents.Visible)
            {
                pnlAddStudents.Hide();
                pnlDashboard.Show();

            }
            else
            {
                pnlDashboard.Show();
            }
            pnlDashboard.Show();
           

        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

       
        

        private void pnlDashboard_Paint(object sender, PaintEventArgs e)
        {


            if (this.btnDashboard.FillColor == System.Drawing.Color.White)
            {
                if (pnlDashboard.Visible)
                {
                    this.btnDashboard.ForeColor = System.Drawing.Color.White;
                }

                else
                {
                    this.btnDashboard.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
                }

            }

            else if(this.pnlDashboard.BackColor == System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31))))))
            {
                if (pnlDashboard.Visible)
                {
                    this.btnDashboard.ForeColor = System.Drawing.Color.White;
                }

                else
                {
                    this.btnDashboard.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                }
            }

           
        }


        private void pnlAddStudents_Paint(object sender, PaintEventArgs e)
        {
            if (this.btnAddStudent.FillColor == System.Drawing.Color.White)
            {
                if (pnlAddStudents.Visible)
                {
                    this.btnAddStudent.ForeColor = System.Drawing.Color.White;
                }

                else
                {
                    this.btnAddStudent.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
                }

            }

            else if (this.pnlAddStudents.BackColor == System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31))))))
            {
                if (pnlAddStudents.Visible)
                {
                    this.btnAddStudent.ForeColor = System.Drawing.Color.White;
                }

                else
                {
                    this.btnAddStudent.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                }
            }
        }


        private void linkViewAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            viewRecords ob = new viewRecords();
            ob.Show();
           

        }

        
        private void btnStart_Click_1(object sender, EventArgs e)
        {
            btnStart.Hide();
            videoCaptureDevice = new VideoCaptureDevice(filterInfoCollection[CmbCamera.SelectedIndex].MonikerString);
            videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;
            videoCaptureDevice.Start();
            btnStop.Show();
            this.CmbCamera.Enabled = false;
        }

        private void VideoCaptureDevice_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {

            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            BarcodeReader reader = new BarcodeReader();
            var result = reader.Decode(bitmap);
            if (result != null)
            {
                txtid.Invoke(new MethodInvoker(delegate ()
                {

                    cmd = new MySqlCommand();
                    cmd.CommandText = " SELECT * FROM `std_info` where nibm_id like '" + txtid.Text + "%'";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    con.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        txtid.Text = result.ToString();
                        txtname.Text = dr.GetValue(2).ToString();
                        txtnic.Text = dr.GetValue(1).ToString();
                        txtbatch.Text = dr.GetValue(5).ToString();


                    }

                    con.Close();
                    String qry = " SELECT * FROM `std_info` where nibm_id like '" + txtid.Text + "%'";
                    cmd = new MySqlCommand(qry, con);
                    da = new MySqlDataAdapter(cmd);
                    DataTable table = new DataTable();
                    da.Fill(table);
                    txtid.Text = table.Rows[0][0].ToString();
                    byte[] img = (byte[])table.Rows[0][6];
                    MemoryStream ms = new MemoryStream(img);
                    StudentImage.Image = Image.FromStream(ms);
                    da.Dispose();

                }));


            }
            ShowdeviceInputBox.Image = bitmap;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure You want to Stop the current Operation? ", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                btnStop.Hide();
                btnStart.Show();
                this.CmbCamera.Enabled = true;
            }
            
        }

        private void btnAddData_Click_1(object sender, EventArgs e)
        {
            cmd = new MySqlCommand();
            cmd.CommandText = "insert into std_att (nibm_id, nic, name, address, number, batch,image) SELECT * FROM `std_info` where nibm_id like '" + txtid.Text + "%'";
            if (txtid.Text == "")
            {
                MessageBox.Show("Please provide all data");
            }
            else
            {
                con.Open();
                cmd.Connection = con;
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Data Inserted");

                string Query = "select * from std_att ;";
                MySqlCommand MyCommand2 = new MySqlCommand(Query, con);
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = MyCommand2;
                DataTable dTable = new DataTable();
                MyAdapter.Fill(dTable);
                ShowRecords.DataSource = dTable;
            }
        }
        private void btnCancelAddData_Click_1(object sender, EventArgs e)
        {
            if (txtname.Text == "" || txtnic.Text == "" || txtbatch.Text == "" || txtid.Text == "")
            {

                MessageBox.Show("Fields are already empty.");

            }
            else
            {

                if (MessageBox.Show("Are you sure You want to Clear? ", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    txtname.Text = "";
                    txtnic.Text = "";
                    txtbatch.Text = "";
                    txtid.Text = "";
                }
            }
        }

        
        private void btnDarlMode_Click(object sender, EventArgs e)
        {
            darkMode();
            
        }

        private void btnLightMode_Click(object sender, EventArgs e)
        {
            lightMod();
            
        }

        private void btnAddStudent_Click(object sender, EventArgs e)
        {
            if (pnlDashboard.Visible)
            {
                pnlDashboard.Hide();
                pnlAddStudents.Show();
                

            }
            else
            {
                pnlAddStudents.Show();
            }
            
        }


        private void btnAddStudntData_Click(object sender, EventArgs e)
        {
            MemoryStream ms = new MemoryStream();
            picStude.Image.Save(ms, picStude.Image.RawFormat);
            byte[] img = ms.ToArray();

            String insertQuery = "INSERT INTO std_info  (nibm_id, nic, name, address, number, batch,image) VALUES(@nibm_id, @nic, @name, @address, @number, @batch, @image)";

            con.Open();

            cmd = new MySqlCommand(insertQuery, con);

            cmd.Parameters.Add("@nibm_id", MySqlDbType.VarChar, 20);
            cmd.Parameters.Add("@nic", MySqlDbType.VarChar, 20);
            cmd.Parameters.Add("@name", MySqlDbType.VarChar, 250);
            cmd.Parameters.Add("@address", MySqlDbType.VarChar, 250);
            cmd.Parameters.Add("@number", MySqlDbType.Int32);
            cmd.Parameters.Add("@batch", MySqlDbType.VarChar, 15);
            cmd.Parameters.Add("@image", MySqlDbType.Blob);

            cmd.Parameters["@nibm_id"].Value = txtStudentID.Text;
            cmd.Parameters["@nic"].Value = txtStudentNIC.Text;
            cmd.Parameters["@name"].Value = txtStudentName.Text;
            cmd.Parameters["@address"].Value = txtStudentAddress.Text;
            cmd.Parameters["@number"].Value = txtStudentPhone.Text;
            cmd.Parameters["@batch"].Value = txtStudentBatch.Text;
            cmd.Parameters["@image"].Value = img;

            if (cmd.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Data Inserted");
            }

            con.Close();
        }

        private void btnCamcelStudData_Click(object sender, EventArgs e)
        {
            txtStudentID.Text=("");
            txtStudentNIC.Text =("");
            txtStudentName.Text = ("");
            txtStudentAddress.Text = ("");
            txtStudentPhone.Text = ("");
            txtStudentBatch.Text = ("");

        }

        private void btnChooseImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Filter = "Choose Image(*.jpg; *.png; *.gif)|*.jpg; *.png; *.gif";
            if (opf.ShowDialog() == DialogResult.OK)
            {
                picStude.Image = Image.FromFile(opf.FileName);
            }
        }


        
        

        ////////////////////////////////////////////////////////////////-----------------------------------UI MODS----------------------------------////////////////////////////////////////////////////////////////////////

        private void lightMod()
        {

            // pnlMain
            
            this.pnlMain.BackColor = System.Drawing.Color.White;


            // pnlButtons

            this.pnlButtons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            
            
            // btnAdmin
           
            this.btnAdmin.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.btnAdmin.ForeColor = System.Drawing.Color.Black;
            this.btnAdmin.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(65)))), ((int)(((byte)(137)))));
            this.btnAdmin.HoverState.ForeColor = System.Drawing.Color.White;
            this.btnAdmin.Image = global::SAMS_v3._0.Properties.Resources.userIcon;
            



            // btnReports


            this.btnReports.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.btnReports.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnReports.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(65)))), ((int)(((byte)(137)))));
            this.btnReports.HoverState.Font = new System.Drawing.Font("Roboto Condensed", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReports.HoverState.ForeColor = System.Drawing.Color.White;
            this.btnReports.HoverState.Image = global::SAMS_v3._0.Properties.Resources.repotslogo_Dark;
            this.btnReports.Image = global::SAMS_v3._0.Properties.Resources.repotslogo_light;
            
            
            // btnDashboard
            
            
            this.btnDashboard.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.btnDashboard.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnDashboard.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(65)))), ((int)(((byte)(137)))));
            this.btnDashboard.HoverState.Font = new System.Drawing.Font("Roboto Condensed", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDashboard.HoverState.ForeColor = System.Drawing.Color.White;
            this.btnDashboard.HoverState.Image = global::SAMS_v3._0.Properties.Resources.Dashboad_logo_dark;
            this.btnDashboard.Image = global::SAMS_v3._0.Properties.Resources.Dashboad_logo_light;


            // btnAddStudent

            this.btnAddStudent.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.btnAddStudent.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnAddStudent.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(65)))), ((int)(((byte)(137)))));
            this.btnAddStudent.HoverState.Font = new System.Drawing.Font("Roboto Condensed", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddStudent.HoverState.ForeColor = System.Drawing.Color.White;
            this.btnAddStudent.HoverState.Image = global::SAMS_v3._0.Properties.Resources.Add_student_dark;
            this.btnAddStudent.Image = global::SAMS_v3._0.Properties.Resources.Add_student_light;

            // pnlBar

            this.pnlBar.BackColor = System.Drawing.Color.White;

            // pnlDashboard
            
            this.pnlDashboard.BackColor = System.Drawing.Color.White;


            // linkViewAll
            
            this.linkViewAll.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(148)))), ((int)(((byte)(226)))));
            this.linkViewAll.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(65)))), ((int)(((byte)(137)))));


            // lblAtList

            this.lblAtList.ForeColor = System.Drawing.Color.Black;


            // ShowdeviceInputBox
            
            this.ShowdeviceInputBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(235)))), ((int)(((byte)(242)))));
            
            // comboDropDevice
            
            this.CmbCamera.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(235)))), ((int)(((byte)(242)))));
            this.CmbCamera.ForeColor = System.Drawing.Color.Black;
            
            
            // pnlShowDetails
            
            this.pnlShowDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(235)))), ((int)(((byte)(242)))));
            this.pnlShowDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // txtStudIndex
            
            this.txtid.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(243)))), ((int)(((byte)(247)))));
            this.txtid.ForeColor = System.Drawing.Color.Black;
            
            
            // txtStudBatch
            
            this.txtbatch.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(243)))), ((int)(((byte)(247)))));
            this.txtbatch.ForeColor = System.Drawing.Color.Black;
            
            // txtStudNID
            
            this.txtnic.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(243)))), ((int)(((byte)(247)))));
            this.txtnic.ForeColor = System.Drawing.Color.Black;
            
            
            // txtname

            this.txtname.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(235)))), ((int)(((byte)(242)))));
            this.txtname.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(243)))), ((int)(((byte)(247)))));
            this.txtname.ForeColor = System.Drawing.Color.Black;


            // StudentImage

            this.StudentImage.BackColor = System.Drawing.Color.WhiteSmoke;

            // gridViewShowRecords

            this.ShowRecords.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(235)))), ((int)(((byte)(242)))));


            // btnClose
            
            this.btnClose.FillColor = System.Drawing.Color.White;
            this.btnClose.Image = global::SAMS_v3._0.Properties.Resources.close;

            // btnMinimize


            this.btnMinimize.FillColor = System.Drawing.Color.White;
            this.btnMinimize.Image = global::SAMS_v3._0.Properties.Resources.minimize;


            // pnlAddStudents

            this.pnlAddStudents.BackColor = System.Drawing.Color.White;


            // lblsAddNewStud


            this.lblAddNewStudTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(22)))), ((int)(((byte)(20)))));
            this.lblStudPic.ForeColor = System.Drawing.Color.Black;
            this.lblStudPhone.ForeColor = System.Drawing.Color.Black;
            this.lblStudAddress.ForeColor = System.Drawing.Color.Black;
            this.lblStudName.ForeColor = System.Drawing.Color.Black;
            this.lblStudNIC.ForeColor = System.Drawing.Color.Black;
            this.lblStudID.ForeColor = System.Drawing.Color.Black;
            this.lblStudBatch.ForeColor = System.Drawing.Color.Black;

            // txtStudentNIC

            this.txtStudentNIC.FillColor = System.Drawing.Color.White;
            this.txtStudentNIC.ForeColor = System.Drawing.Color.Black;

            this.txtStudentName.FillColor = System.Drawing.Color.White;
            this.txtStudentName.ForeColor = System.Drawing.Color.Black;

            this.txtStudentAddress.FillColor = System.Drawing.Color.White;
            this.txtStudentAddress.ForeColor = System.Drawing.Color.Black;

            this.txtStudentPhone.FillColor = System.Drawing.Color.White;
            this.txtStudentPhone.ForeColor = System.Drawing.Color.Black;

            this.txtStudentBatch.FillColor = System.Drawing.Color.White;
            this.txtStudentBatch.ForeColor = System.Drawing.Color.Black;

            this.txtStudentID.FillColor = System.Drawing.Color.White;
            this.txtStudentID.ForeColor = System.Drawing.Color.Black;

            // txtpanels
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel2.BackColor = System.Drawing.Color.Black;
            this.panel3.BackColor = System.Drawing.Color.Black;
            this.panel4.BackColor = System.Drawing.Color.Black;
            this.panel5.BackColor = System.Drawing.Color.Black;
            this.panel6.BackColor = System.Drawing.Color.Black;


            // picStude

            this.picStude.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));

            // btnChooseImage
            
            this.btnChooseImage.FillColor = System.Drawing.Color.Silver;
            this.btnChooseImage.ForeColor = System.Drawing.Color.Black;




            btnLightMode.Hide();
            btnDarlMode.Show();


        }

//-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
        private void darkMode()



        {
            // pnlMain

            this.pnlMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));

            // pnlButtons

            this.pnlButtons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));


            // btnAdmin

            
            this.btnAdmin.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.btnAdmin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnAdmin.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(65)))), ((int)(((byte)(137)))));
            this.btnAdmin.HoverState.ForeColor = System.Drawing.Color.White;


            // btnReports


            this.btnReports.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.btnReports.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnReports.Image = global::SAMS_v3._0.Properties.Resources.repotslogo_Dark;

            // btnDashboard


            this.btnDashboard.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.btnDashboard.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnDashboard.Image = global::SAMS_v3._0.Properties.Resources.Dashboad_logo_dark;


            // btnAddStudent

            this.btnAddStudent.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.btnAddStudent.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnAddStudent.Image = global::SAMS_v3._0.Properties.Resources.Add_student_dark;

            // pnlBar

            this.pnlBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));

            // pnlDashboard

            this.pnlDashboard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));


            // linkViewAll

            this.linkViewAll.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(148)))), ((int)(((byte)(226)))));
            this.linkViewAll.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));


            // lblAtList

            this.lblAtList.ForeColor = System.Drawing.Color.White;


            // ShowdeviceInputBox

            this.ShowdeviceInputBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));

            // comboDropDevice

            this.CmbCamera.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.CmbCamera.ForeColor = System.Drawing.Color.White;


            // pnlShowDetails

            this.pnlShowDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));


            // txtStudIndex

            this.txtid.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.txtid.ForeColor = System.Drawing.Color.White;


            // txtStudBatch

            this.txtbatch.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.txtbatch.ForeColor = System.Drawing.Color.White;

            // txtStudNID

            this.txtnic.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.txtnic.ForeColor = System.Drawing.Color.White;

            // txtname
             
            this.txtname.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.txtname.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.txtname.ForeColor = System.Drawing.Color.White;
            


            // StudentImage

            this.StudentImage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));

            // gridViewShowRecords

            this.ShowRecords.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));


            // btnClose

            this.btnClose.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.btnClose.Image = global::SAMS_v3._0.Properties.Resources.closeHover;

            // btnMinimize


            this.btnMinimize.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.btnMinimize.Image = global::SAMS_v3._0.Properties.Resources.minimizelight;


            // pnlAddStudents

            this.pnlAddStudents.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));


            // lblsAddNewStud


            this.lblAddNewStudTitle.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblStudPic.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblStudPhone.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblStudAddress.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblStudName.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblStudNIC.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblStudID.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblStudBatch.ForeColor = System.Drawing.Color.Gainsboro;

            // txtStudentNIC

            this.txtStudentNIC.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.txtStudentNIC.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));

            this.txtStudentName.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.txtStudentName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));

            this.txtStudentAddress.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.txtStudentAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));

            this.txtStudentPhone.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.txtStudentPhone.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));

            this.txtStudentBatch.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.txtStudentBatch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));

            this.txtStudentID.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.txtStudentID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));

            // txtpanels
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));


            // picStude

            this.picStude.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));

            // btnChooseImage

            this.btnChooseImage.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.btnChooseImage.ForeColor = System.Drawing.Color.WhiteSmoke;


            btnDarlMode.Hide();
            btnLightMode.Show();

        }

        private void SAMS_Load(object sender, EventArgs e)
        {
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in filterInfoCollection)
            CmbCamera.Items.Add(device.Name);
            CmbCamera.SelectedIndex = 0;
        }

        
    }
}
