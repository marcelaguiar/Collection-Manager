using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CollectionsManager
{
    public partial class frmMain : Form
    {
        String connectionString;
        SqlConnection connection;

        List<CollectionEntry> items = new List<CollectionEntry>();
         
        public frmMain()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["CollectionsManager.Properties.Settings.CollectionsConnectionString"].ConnectionString;
            Shown += frmMain_Shown;
        }

        private void frmMain_Shown(Object sender, EventArgs e)
        {
            populateCollectionListView();
            initializeCollectionList();
        }

        private void populateCollectionListView()
        {
            currentItems.Items.Clear();

            using (connection = new SqlConnection(connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Bottlecaps ORDER BY Maker ASC", connection))
            {
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                foreach (DataRow dr in dataTable.Rows)
                {
                    ListViewItem item = new ListViewItem(dr["Id"].ToString().Trim());
                    item.SubItems.Add(dr["Maker"].ToString().Trim());
                    item.SubItems.Add(dr["Variant"].ToString().Trim());
                    currentItems.Items.Add(item);
                }
            }
        }

        private void initializeCollectionList()
        {
            using (connection = new SqlConnection(connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Bottlecaps ORDER BY Maker ASC", connection))
            {
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                foreach (DataRow dr in dataTable.Rows)
                {
                    string id = dr["Id"].ToString().Trim();
                    string maker = dr["Maker"].ToString().Trim();
                    string variant = dr["Variant"].ToString().Trim();

                    CollectionEntry collectionEntry = new CollectionEntry(id, maker, variant);

                    items.Add(collectionEntry);
                }
            }
        }

        private void displaySelection()
        {
            if (this.currentItems.SelectedItems.Count == 0)
                return; //Maybe select first element in list instead?

            string id = this.currentItems.SelectedItems[0].SubItems[0].Text;

            string queryString = string.Format("SELECT * FROM Bottlecaps WHERE Id='{0}';", id);

            using (connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(queryString, connection);
                SqlCommand command = new SqlCommand(queryString, connection);
                SqlDataReader reader = command.ExecuteReader();

                /* Display Labels */
                while (reader.Read())
                {
                    SetLabels((IDataRecord)reader);
                }

                reader.Close();

                /* Display Image */
                var ds = new DataSet();
                adapter.Fill(ds, "Images");
                int count = ds.Tables["Images"].Rows.Count;

                if (count == 1)
                {
                    var data = (ds.Tables["Images"].Rows[count - 1]["Img"]);

                    if (data.GetType() == typeof(System.DBNull))
                    {
                        pictureBox1.Image = null;
                    }
                    else
                    {
                        var stream = new MemoryStream((Byte[])data);
                        pictureBox1.Image = Image.FromStream(stream);
                    }
                }
                else
                    Console.Out.WriteLine("Multiple instances of an Id found!: " + count);

                //Should I close/dispose adapter and command?
            }
        }

        private void SetLabels(IDataRecord record)
        {
            ID.Text = record[0].ToString().Trim();
            Maker.Text = record[1].ToString().Trim();
            Variant.Text = record[2].ToString().Trim();
            label11.Text = record[3].ToString().Trim();
            label12.Text = record[9].ToString().Trim();
            label13.Text = record[4].ToString().Trim();
            label14.Text = record[5].ToString().Trim();
            label15.Text = record[8].ToString().Trim();
            label16.Text = record[7].ToString().Trim();
            label17.Text = record[10].ToString().Trim();
            label18.Text = record[6].ToString().Trim();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmAddItem itemAddPage = new frmAddItem();
            if (itemAddPage.ShowDialog() == DialogResult.OK)
            {
                populateCollectionListView(); //Does this not do anything
            }

            itemAddPage.Dispose();
        }

        private void currentItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            displaySelection();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Search...")
            {
                textBox1.Text = "";
                textBox1.ForeColor = SystemColors.WindowText;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
            {
                textBox1.Text = "Search...";
                textBox1.ForeColor = SystemColors.GrayText;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox1.Text != "Search..." && comboBox1.SelectedIndex > 0)
            {
                currentItems.Items.Clear();

                int category = comboBox1.SelectedIndex;

                Console.WriteLine("Selected category: " + category);

                // repopulate the listview with search results corresponding to selected category
                if (category == 1) //ID
                {
                    currentItems.Items.AddRange(items.Where(i => string.IsNullOrEmpty(textBox1.Text) || i.Id.StartsWith(textBox1.Text))
                        .Select(c => new ListViewItem(c.Id)).ToArray());
                }
                else if (category == 2) //Maker
                {
                    currentItems.Items.AddRange(items.Where(i => string.IsNullOrEmpty(textBox1.Text) || i.Maker.StartsWith(textBox1.Text))
                        .Select(c => new ListViewItem(c.Id)).ToArray());
                }
                else if (category == 3) //Variant
                {
                    currentItems.Items.AddRange(items.Where(i => string.IsNullOrEmpty(textBox1.Text) || i.Variant.StartsWith(textBox1.Text))
                        .Select(c => new ListViewItem(c.Id)).ToArray());
                }
                else
                { }
            }
            else
            {
                //if everything is already displayed, do nothing

                currentItems.Items.Clear();

                //display everything
                currentItems.Items.AddRange(items.Where(i => string.IsNullOrEmpty(textBox1.Text))
                        .Select(c => new ListViewItem(c.Id)).ToArray());
            }
        }

        private void panel3_Resize(object sender, EventArgs e)
        {
            //Maintain square proportion on image
            if(panel3.Width < panel3.Height)
            {
                pictureBox1.Width = panel3.Width;
                pictureBox1.Height = pictureBox1.Width;
            }
            else if(panel3.Width > panel3.Height)
            {
                pictureBox1.Height = panel3.Height;
                pictureBox1.Width = pictureBox1.Height;
            }
            else
            { }
        }

    }
}
