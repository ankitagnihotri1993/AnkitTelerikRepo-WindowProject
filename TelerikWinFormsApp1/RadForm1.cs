using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using Telerik.WinControls.UI;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace TelerikWinFormsApp1
{
    public partial class RadForm1 : Telerik.WinControls.UI.RadForm
    {
        SqlConnection con = new SqlConnection("data source=DESKTOP-FVRIAA6; database=telerikapp; integrated security=true");

        public  RadForm1()
        {
            InitializeComponent();
            BindGridDataSource();
            Bindcountry();
        }
        
        public void clear()
        {
            txtname.Text = string.Empty;
            txtaddress.Text = string.Empty;
            txtage.Text = string.Empty;
            ddlcountry.SelectedValue = 0;
            rdoMale.IsChecked = false;
            rdoFemale.IsChecked = false;
            chklistbox.CheckedItems.Clear();
            empdtp.Value = DateTime.UtcNow;
            pictureBox1.Image = null;
            txtname.Focus();
        }
        private void BindGridDataSource()
        {
            Dal d = new Dal();
            List<Employee> emp = d.GetEmployees();
            radGridView2.DataSource = emp;
        }
        private void Bindcountry()
        {
            Dal d = new Dal();
            List<country> country = d.Getcountry();
            ddlcountry.DisplayMember = "cname";
            ddlcountry.ValueMember = "cid";
            ddlcountry.DataSource = country;
            ddlcountry.DropDownListElement.Text = "<please select>";
        }

        List<Employee> addedEmployees = new List<Employee>();
        List<Employee> deletedEmployees = new List<Employee>();
        List<Employee> editedemployees = new List<Employee>();
        List<Employee> updatedemplouees = new List<Employee>();

        private void radGridView2_CommandCellClick(object sender, Telerik.WinControls.UI.GridViewCellEventArgs e)
        {
            

            if (e.Column.Name.ToLower() == "cmddelete")
            {
                var confirmResult = MessageBox.Show("Are you sure to delete this item ??",
                                        "Confirm Delete!!",
                                        MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    Employee employee = e.Row.DataBoundItem as Employee;
                    deletedEmployees.Add(employee);
                    foreach (Employee item in deletedEmployees)
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand("usp_emp_delete", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", item.id);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        MessageBox.Show("record deleted");
                        BindGridDataSource();
                    }
                }
            }
            else if (e.Column.Name == "cmdedit")
            { 
                Employee employee = e.Row.DataBoundItem as Employee;
                editedemployees.Add(employee);
                txtname.Text = employee.empName;
                txtaddress.Text = employee.address;
                txtage.Text = employee.age.ToString();
                ddlcountry.Text = employee.empcountry;
                empdtp.Text = employee.empdob;
                if (employee.gender == "Male")
                {
                    rdoMale.IsChecked = true;
                }
                else
                {
                    rdoFemale.IsChecked = true;
                }
                string hobby = employee.emphobbies;
                string[] hobbylist = hobby.Split(',');
                chklistbox.UncheckAllItems();
                foreach (ListViewDataItem lv in chklistbox.Items)
                {

                    foreach (string s in hobbylist)
                    {
                        if (lv.Text == s)
                        {
                            lv.CheckState = Telerik.WinControls.Enumerations.ToggleState.On;
                        }
                    }
                }
                empdtp.Value = Convert.ToDateTime( employee.empdob);
                MemoryStream ms = new MemoryStream((byte[])employee.empimages);
                pictureBox1.Image = new Bitmap(ms);
                BindGridDataSource();
            }
            else if (e.Column.Name == "cmdupdate")
            {

                Employee employee = e.Row.DataBoundItem as Employee;
                updatedemplouees.Add(employee);
             
                string HOB = string.Empty;
                for (int i = 0; i < chklistbox.CheckedItems.Count; i++)
                {
                    HOB += chklistbox.Items[i].Text + ",";
                }
                HOB = HOB.TrimEnd(',');
                string gender = rdoMale.IsChecked ? "Male" : "Female";
                MemoryStream ms = new MemoryStream();
                try
                {
                    pictureBox1.Image.Save(ms, ImageFormat.Jpeg);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                byte[] photo_aray = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(photo_aray, 0, photo_aray.Length);
                if (txtname.Text == "" || txtaddress.Text == "" || ddlcountry.SelectedValue == null || Convert.ToInt32(txtage.Text)  == 0
               || (rdoMale.IsChecked == false && rdoFemale.IsChecked == false) || empdtp.Value == null || chklistbox.CheckedItems.Count == 0 || pictureBox1.Image == null)


                {
                    MessageBox.Show("Can't update plese select values to update!!");
                }
                else
                {
                    foreach (Employee update in updatedemplouees)
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand("usp_emp_update", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id",  update.id);
                        cmd.Parameters.AddWithValue("@name", txtname.Text);
                        cmd.Parameters.AddWithValue("@address", txtaddress.Text);
                        cmd.Parameters.AddWithValue("age", txtage.Text);
                        cmd.Parameters.AddWithValue("gender", gender);
                        cmd.Parameters.AddWithValue("@contry", ddlcountry.SelectedValue);
                        cmd.Parameters.AddWithValue("@hobbies", HOB);
                        cmd.Parameters.AddWithValue("@dob", Convert.ToDateTime(empdtp.Text));
                        cmd.Parameters.AddWithValue("@images", photo_aray);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        MessageBox.Show("Details updated");
                        clear();
                        BindGridDataSource();
                    }
                }
            }
            
        }
        private void btnsave_Click(object sender, EventArgs e)
        {
            
            if (txtname.Text == "" || txtaddress.Text == "" || ddlcountry.SelectedValue == null || txtage.Text == "" 
                || (rdoMale.IsChecked == false && rdoFemale.IsChecked == false)||empdtp.Value==null|| chklistbox.CheckedItems.Count == 0|| pictureBox1.Image==null)
            {
                MessageBox.Show("Please fill in all fields","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                txtname.Focus();
                return;
            }
            if (!Regex.Match(txtname.Text, "^[a-zA-Z][A-Za-z s]+$").Success)
            {
                MessageBox.Show("Invalid name!! Please enter alphabets only", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtname.Focus();
                return;
            }
            if (!Regex.Match(txtaddress.Text, @"^[A-Za-z0-9]+(?:\s[A-Za-z0-9'_-]+)+$").Success)
            {
                MessageBox.Show("Invalid address!! Please enter correct address", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtaddress.Focus();
                return;
            }
            if (!Regex.Match(txtage.Text, "^([1-9]?[1-9]|100)$").Success)
            {
                MessageBox.Show("Invalid age!! Please enter age between 1 to 100", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtage.Focus();
                return;
            }

            else
            {
                MemoryStream ms = new MemoryStream();
                pictureBox1.Image.Save(ms, ImageFormat.Jpeg);
                byte[] photo_aray = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(photo_aray, 0, photo_aray.Length);
                //cmd.Parameters.AddWithValue("@photo", photo_aray);
                string HOB = string.Empty;
                for (int i = 0; i < chklistbox.CheckedItems.Count; i++)
                {
                    HOB += chklistbox.Items[i].Text + ",";
                }
                HOB = HOB.TrimEnd(',');
                string gender = rdoMale.IsChecked ? "Male" : "Female";
                con.Open();
                SqlCommand cmd = new SqlCommand("usp_emp_insert", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@name", txtname.Text);
                cmd.Parameters.AddWithValue("@address", txtaddress.Text);
                cmd.Parameters.AddWithValue("@age", txtage.Text);
                cmd.Parameters.AddWithValue("@gender", gender);
                cmd.Parameters.AddWithValue("@contry", ddlcountry.SelectedValue);
                cmd.Parameters.AddWithValue("@hobbies", HOB);
                cmd.Parameters.AddWithValue("@dob", empdtp.Value);
                cmd.Parameters.AddWithValue("@images", photo_aray);
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("data inserted");
                clear();
                BindGridDataSource();
            }


        }

        private void btnbrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog opfd = new OpenFileDialog();
            opfd.Filter = "jpeg|*.jpg|bmp|*.bmp|all files|*.*";
            DialogResult res = opfd.ShowDialog();
            if (res == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(opfd.FileName);
            }
        }

        private void RadForm1_Load(object sender, EventArgs e)
        {
            //DataTable dt = new DataTable();
            //SqlDataAdapter da = new SqlDataAdapter("usp_student_get", con);
            //da.Fill(dt);

            //DataTable dtt = new DataTable();
            //SqlDataAdapter daa = new SqlDataAdapter("usp_emp_get", con);
            //daa.Fill(dtt);


            //radGridView2.DataSource = dt;
            //radGridView2.AutoGenerateHierarchy = true;
            //GridViewTemplate gvt = new GridViewTemplate();
            //gvt.DataSource = dtt;
            //radGridView2.MasterTemplate.Templates.Add(gvt);


            //GridViewRelation gvr = new GridViewRelation(this.radGridView2.MasterTemplate);
            //gvr.ChildTemplate = gvt;
            //gvr.RelationName = "studentmarksanddetails";
            //gvr.ParentColumnNames.Add("sid");
            //gvr.ChildColumnNames.Add("stid");
            //radGridView2.Relations.Add(gvr);

        }
    }
}


