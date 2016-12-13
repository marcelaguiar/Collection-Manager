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

        bool listFull = false;
         
        public frmMain()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["CollectionsManager.Properties.Settings.CollectionsConnectionString"].ConnectionString;
            Shown += frmMain_Shown;
        }

        private void frmMain_Shown(Object sender, EventArgs e)
        {
            populateCollectionListView();
            if (currentItems.Items.Count >= 1)
            {
                currentItems.Items[0].Selected = true;
                displaySelection();
            }
            else displayNothing();
        }

        private void populateCollectionListView()
        {
            currentItems.Items.Clear();

            using (connection = new SqlConnection(connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Bottlecaps ORDER BY Product ASC", connection))
            {
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                foreach (DataRow dr in dataTable.Rows)
                {
                    ListViewItem item = new ListViewItem(dr["Id"].ToString().Trim());
                    item.SubItems.Add(dr["Product"].ToString().Trim());
                    item.SubItems.Add(dr["Variant"].ToString().Trim());
                    item.SubItems.Add(dr["Manufacturer"].ToString().Trim());
                    currentItems.Items.Add(item);
                }
            }
            listFull = true;
        }

        private void displaySelection()
        {
            if (this.currentItems.SelectedItems.Count == 0)
                return;

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
            labelID.Text = record[0].ToString().Trim();
            labelProduct.Text = record[1].ToString().Trim();
            labelVariant.Text = record[2].ToString().Trim();
            labelManufacturer.Text = record[3].ToString().Trim();
            labelDrink.Text = record[4].ToString().Trim();
            labelMethodAcquired.Text = record[5].ToString().Trim();
            labelSpares.Text = record[6].ToString().Trim();
            labelText.Text = record[7].ToString().Trim();
            labelIcon.Text = record[8].ToString().Trim();
            labelColor.Text = record[9].ToString().Trim();
            labelDateAcquired.Text = record[10].ToString().Trim();
            labelUnderside.Text = record[11].ToString().Trim();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmAddItem itemAddPage = new frmAddItem();
            if (itemAddPage.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine("PopulateCollectionListView called");
                populateCollectionListView();
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
            if (!textBox1.Text.Equals("") && !textBox1.Text.Equals("Search..."))
            {
                if (comboBox1.SelectedIndex >= 0)
                {
                    repopulateList();
                }
                else
                {
                    //Warn user to select a drop box search option
                    comboBox1.DroppedDown = true;
                    Cursor.Current = Cursors.Default;
                }
            }
            else if (!textBox1.Text.Equals("Search..."))
            {
                if (!listFull)
                    populateCollectionListView();
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
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if( !textBox1.Text.Equals("Search...") )
            {
                repopulateList();
            }
        }

        private void repopulateList()
        {
            currentItems.Items.Clear();

            int index = comboBox1.SelectedIndex;

            string category = "";
            switch(index)
            {
                case 0: //ID
                    category = "Id";
                    break;
                case 1: //Product
                    category = "Product";
                    break;
                case 2: //Variant
                    category = "Variant";
                    break;
                case 3: //Manufacturer
                    category = "Manufacturer";
                    break;
                case 4: //Drink
                    category = "Drink";
                    break;
                case 5: //Color
                    category = "Color";
                    break;
                default:
                    Console.WriteLine("Category for query not Assigned!");
                    break;
            }

            var queryString = "SELECT * FROM Bottlecaps WHERE "+category+" LIKE '%"+textBox1.Text+"%' ORDER BY Product ASC";

            // I probably shouldn't need to use a SQLDataAdapter here
            using (connection = new SqlConnection(connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter(queryString, connection))
            {
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                foreach (DataRow dr in dataTable.Rows)
                {
                    ListViewItem item = new ListViewItem(dr["Id"].ToString().Trim());
                    item.SubItems.Add(dr["Product"].ToString().Trim());
                    item.SubItems.Add(dr["Variant"].ToString().Trim());
                    item.SubItems.Add(dr["Manufacturer"].ToString().Trim());
                    currentItems.Items.Add(item);
                }
            }

            listFull = false;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Delete this item permanently?", "Deletion Warning", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                deleteSelectedItem();
                populateCollectionListView();
                if (currentItems.Items.Count >= 1)
                {
                    currentItems.Items[0].Selected = true;
                    displaySelection();
                }
                else displayNothing();
            }
            else if (dialogResult == DialogResult.No)
            {
                //don't delete
            }
        }

        private void deleteSelectedItem()
        {
            string deleteQuery = "DELETE FROM Bottlecaps WHERE Id="+ labelID.Text;

            try
            {
                //Do I even need to start a new connection here?
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("DELETE FROM Bottlecaps WHERE Id='"+labelID.Text+"'", connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch (SystemException ex)
            {
                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
            }
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            frmEditItem itemEditPage = new frmEditItem(
                labelID.Text,
                labelProduct.Text,
                labelVariant.Text,
                labelManufacturer.Text,
                labelDrink.Text,
                labelMethodAcquired.Text,
                labelSpares.Text,
                labelText.Text,
                labelIcon.Text,
                labelColor.Text,
                labelDateAcquired.Text,
                labelUnderside.Text,
                pictureBox1.Image
                );

            if (itemEditPage.ShowDialog() == DialogResult.OK)
            {
                populateCollectionListView();
                //Figure out how to re-select edited item here
                displaySelection();
            }

            itemEditPage.Dispose();
        }

        private void displayNothing()
        {
            labelID.Text = "";
            labelProduct.Text = "";
            labelVariant.Text = "";
            labelManufacturer.Text = "";
            labelDrink.Text = "";
            labelMethodAcquired.Text = "";
            labelSpares.Text = "";
            labelText.Text = "";
            labelIcon.Text = "";
            labelColor.Text = "";
            labelDateAcquired.Text = "";
            labelUnderside.Text = "";
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }
        }

    }
}
