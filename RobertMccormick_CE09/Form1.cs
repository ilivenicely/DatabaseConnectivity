 
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Net;

//Final Edit - Version 2  
// May 25 2017

namespace FirstnameLastname_CE09
{
    public partial class Form1 : Form
    {
        private int CurrentIndex = 0;
        private List<Obj> ListData;

        public Form1()
        {
            InitializeComponent();
            ListData = new List<Obj>();
            LoadData();
        }



        //mySQL query 
        private void LoadData()
        {



            //if DB cant be accessed then alert otherwise the connection passes to line 61
            var dbConn = DBConnection.Instance();
            if (!dbConn.IsConnect())
            {
                MessageBox.Show("Can not connect to mysql.");
                return;
            }

            //mySQL query 
            string query = "SELECT dvd_title, studio, price, createddate, publicRating FROM exampledatabase.dvd limit 25";

            //instantiate mysql and send the query with connection 
            var cmd = new MySqlCommand(query, dbConn.Connection);
            var reader = cmd.ExecuteReader();
            //convert to Obj for store to list
            while (reader.Read())
            {
                ListData.Add(
                    new Obj()
                    {
                        Title = reader.GetString(0),
                        Studio = reader.GetString(1),
                        Price = reader.GetDouble(2),
                        CreatedDate = reader.GetDateTime(3),
                        Rating = reader.GetDouble(4)
                    });
            }

            //generate controls by first obj
            var index = 0;
            foreach (var prop in ListData[0].GetType().GetProperties())
            {
                AutoGenerateControl(prop, index);
                index++;
            }

            //display first row
            DisplayData();

            //set paging
            SetPaging();

            reader.Close();
        }

        private void AutoGenerateControl(PropertyInfo prop, int index)
        {
            //label 
            Label lbl = new Label();
            lbl.Name = "label" + index.ToString();
            lbl.Location = new Point(12, 43 + 26 * index);
            lbl.Text = prop.Name;
            lbl.AutoSize = true;
            this.Controls.Add(lbl);

            //if String is generate textbox
            if (prop.PropertyType.Name.Equals("String"))
            {
                TextBox txt = new TextBox();
                txt.Name = prop.Name;
                txt.Location = new Point(95, 43 + 26 * index);
                txt.Size = new Size(100, 20);
                this.Controls.Add(txt);
            }
            //double is NumericUpDown
            else if (prop.PropertyType.Name.Equals("Double"))
            {
                NumericUpDown num = new NumericUpDown();
                num.Name = prop.Name;
                num.Location = new Point(95, 43 + 26 * index);
                num.Size = new Size(100, 20);
                num.ThousandsSeparator = true;
                num.DecimalPlaces = 2;
                this.Controls.Add(num);
            }
            //datetime  is Datetimepicker
            else if (prop.PropertyType.Name.Equals("DateTime"))
            {
                DateTimePicker dtp = new DateTimePicker();
                dtp.Format = DateTimePickerFormat.Short;
                dtp.Name = prop.Name;
                dtp.Location = new Point(95, 43 + 26 * index);
                dtp.Size = new Size(100, 20);
                this.Controls.Add(dtp);
            }
        }

        private void DisplayData() {
            toolStripStatusLabel1.Text = String.Format("Record {0} of {1}", (CurrentIndex + 1), ListData.Count);

            foreach (var prop in ListData[CurrentIndex].GetType().GetProperties())
            {
                var control = this.Controls.Find(prop.Name, true)[0];

                switch (prop.PropertyType.Name)
                {
                    case "String":
                        (control as TextBox).Text = (string)prop.GetValue(ListData[CurrentIndex]);
                        break;
                    case "Double":
                        (control as NumericUpDown).Value = Convert.ToDecimal(prop.GetValue(ListData[CurrentIndex]));
                        break;
                    case "DateTime":
                        (control as DateTimePicker).Value = (DateTime)prop.GetValue(ListData[CurrentIndex]);
                        break;
                }
            }
        }
         
        // Navigability controls
        private void SetPaging()
        {
            btnFirst.Enabled = true;
            btnLast.Enabled = true;
            btnNext.Enabled = true;
            btnPrevious.Enabled = true;
            if(CurrentIndex == 0)
            {
                btnFirst.Enabled = false;
                btnPrevious.Enabled = false;
            }else if(CurrentIndex == ListData.Count - 1)
            {
                btnLast.Enabled = false;
                btnNext.Enabled = false;
            }
        }

        //setting for xml
        private string GetExt(string filename)
        {
            var temp = filename.Split('.');
            return temp[temp.Length - 1];
        }


        private void btnFirst_Click(object sender, EventArgs e)
        {
            CurrentIndex = 0;

            DisplayData();

            SetPaging();
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            CurrentIndex --;

            DisplayData();

            SetPaging();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            CurrentIndex++;

            DisplayData();

            SetPaging();

        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            CurrentIndex = ListData.Count-1;

            DisplayData();

            SetPaging();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //ApplyModel();
                //deleted 274

                //radio selected to result in print type
             if (GetExt(saveFileDialog1.FileName).ToLower().Equals("xml"))
                    {

                    //  Stream myStream;

                    saveFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.RestoreDirectory = true;

                    var serializer = new XmlSerializer(ListData.GetType());
                    var xmlString = "";
                    using (StringWriter textWriter = new StringWriter())
                    {
                        serializer.Serialize(textWriter, ListData);
                        xmlString = textWriter.ToString();

                        File.WriteAllText(saveFileDialog1.FileName, xmlString);
                        MessageBox.Show("Saved!");
                    }
                }

                //radio selected to result in print type
                else  

                {

                    saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                    saveFileDialog1.FilterIndex = 1;
                    saveFileDialog1.RestoreDirectory = true;

                    File.WriteAllText(saveFileDialog1.FileName, JsonConvert.SerializeObject(ListData));
                    MessageBox.Show("Saved!");


                }
            }
        }

        //Exit
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
