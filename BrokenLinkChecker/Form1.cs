using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IBC.Database;
using System.Net;

namespace BrokenLinkChecker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (typeOfCheck.SelectedItem == null)
            {
                MessageBox.Show("Please choose the type of resources to check");
                return;
            }
            this.Enabled = false;
            db _db = new db("GLOBALEDGE_MVCConnectionString");
            var manageURL = "";
            switch (typeOfCheck.SelectedItem.ToString())
            {
                case "Resources":
                    _db.ExecuteSqlReader("SELECT uniqueid, url FROM v2_resources WHERE URL IS NOT NULL");
                    manageURL = "https://globaledge.msu.edu/Management/Resources/Edit/";
                    break;
                case "International Internships":
                    _db.ExecuteSqlReader("SELECT internshipid as uniqueid, url FROM Internships");
                    manageURL = "https://globaledge.msu.edu/Management/InternationalInternships/InternshipEdit/";
                    break;
                default:
                    this.Enabled = true;
                    return;
                    break;
            }
            Dictionary<int, string> urls = new Dictionary<int, string>();
            while (_db.Reader.Read())
            {
                urls.Add((int)_db.Reader["uniqueid"], (string)_db.Reader["url"]);
            }
            var counter = urls.Count;
            var width = (100.0 / counter);
            var current = 1;
            System.Threading.Tasks.Parallel.ForEach(urls, i =>
            //foreach(var i in urls)
            {
                progressBar1.Value = Convert.ToInt16(width * current);
                current++;
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(i.Value);
                    request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/536.3 (KHTML, like Gecko) Chrome/19.0.1061.1 Safari/536.3";
                    request.Method = "GET";
                    using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
                    {
                    }
                }
                catch (WebException ex)
                {
                    dataGridView1.Rows.Add(new string[] { i.Value, ex.Message, manageURL + i.Key + "?tag=0&section=0" });
                }
            });
            this.Enabled = true;
            progressBar1.Value = 100;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
