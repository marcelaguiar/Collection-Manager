using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CollectionsManager
{
    public partial class frmEditItem : Form
    {
        SqlCommand command;
        SqlConnection connection;
        String connectionString;

        int id;

        public frmEditItem(
            string idLabel,
            string makerLabel,
            string variantLabel,
            string drinkLabel,
            string methodAcquiredLabel,
            string sparesLabel,
            string textLabel,
            string iconLabel,
            string colorLabel,
            string dateAcquired,
            string undersideLabel,
            Image picture)
        {
            InitializeComponent(idLabel);
            try
            {
                id = Int32.Parse(idLabel);
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }
            this.txtMaker.Text = makerLabel;
            this.txtVariant.Text = variantLabel;
            this.txtDrink.Text = drinkLabel;
            this.txtMethodAcquired.Text = methodAcquiredLabel;
            if (sparesLabel.Equals("True"))
                this.sparesAvailable.Checked = true;
            else if (sparesLabel.Equals("False"))
                this.sparesAvailable.Checked = false;
            this.txtText.Text = textLabel;
            this.txtIcon.Text = iconLabel;
            this.txtColor.Text = colorLabel;
            this.datePicker.Value = DateTime.Parse(dateAcquired);
            this.txtUnderside.Text = undersideLabel;
            this.pictureBox1.Image = picture;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            connectionString = ConfigurationManager.ConnectionStrings["CollectionsManager.Properties.Settings.CollectionsConnectionString"].ConnectionString;

            using (connection = new SqlConnection(connectionString))
            {
                connection.Open();
                insertTextFieldsAndImage();

                //add new item to fromMain.items
                Console.Out.WriteLine("Item inserted into Bottlecaps database. Proceed to next step.");
            }

            this.Close();
            Console.Out.WriteLine("Add Item Window closed. Proceed to next step.");
        }

        private void insertTextFieldsAndImage()
        {
            byte[] byteArray;

            if (pictureBox1.Image != null)
            {
                MemoryStream ms = new MemoryStream();
                pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat);
                byteArray = ms.GetBuffer();
                ms.Close();
            }
            else
            {
                Console.Out.WriteLine("No image selected.");
                byteArray = null;
            }

            string updateQueryString = "UPDATE Bottlecaps SET Maker=@Maker,Variant=@Variant,Drink=@Drink,Method_Acquired=@MethodAcquired,Spares_Available=@SparesAvailable,Text=@Text,Icon=@Icon,Color=@Color,Date_Acquired=@DateAcquired,Underside_Text=@Underside,Imgpath=@Imgpath,Img=@Img WHERE Id=" + id;
            command = new SqlCommand(updateQueryString, connection);
            command.Parameters.AddWithValue("@Maker", txtMaker.Text);
            command.Parameters.AddWithValue("@Variant", txtVariant.Text);
            command.Parameters.AddWithValue("@Drink", txtDrink.Text);
            command.Parameters.AddWithValue("@MethodAcquired", txtMethodAcquired.Text);
            command.Parameters.AddWithValue("@SparesAvailable", sparesAvailable.Checked);
            command.Parameters.AddWithValue("@Text", txtText.Text);
            command.Parameters.AddWithValue("@Icon", txtIcon.Text);
            command.Parameters.AddWithValue("@Color", txtColor.Text);
            command.Parameters.AddWithValue("@DateAcquired", datePicker.Value.Date);
            command.Parameters.AddWithValue("@Underside", txtUnderside.Text);
            command.Parameters.AddWithValue("@Imgpath", imgPath.Text);
            command.Parameters.AddWithValue("@Img",
                (pictureBox1.Image == null) ? (object)DBNull.Value : byteArray).SqlDbType = SqlDbType.Image;
            command.ExecuteNonQuery();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFd = new OpenFileDialog())
            {
                openFd.Title = "Open Image";
                openFd.Filter = "Images Only. |*.jpg; *.jpeg; *.png; *.gif;";

                DialogResult dr = openFd.ShowDialog();

                if (dr == DialogResult.Cancel)
                {
                    return;
                }

                //Image selectedImage = Image.FromFile(openFd.FileName);
                //pictureBox1.Image = compressImage(selectedImage);

                pictureBox1.Image = Image.FromFile(openFd.FileName);
                imgPath.Text = openFd.FileName;
            }
        }
    }
}
