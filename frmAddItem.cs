using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CollectionsManager
{
    public partial class frmAddItem : Form
    {
        SqlCommand command;
        SqlConnection connection;
        String connectionString;

        public frmAddItem()
        {
            InitializeComponent();
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

            command = new SqlCommand("INSERT INTO Bottlecaps (Product,Variant,Manufacturer,Drink,Method_Acquired,Spares_Available,Text,Icon,Color,Date_Acquired,Underside_Text,Imgpath,Img) VALUES (@Product, @Variant, @Manufacturer, @Drink, @MethodAcquired, @SparesAvailable, @Text, @Icon, @Color, @DateAcquired, @Underside, @Imgpath, @Img)", connection);
            command.Parameters.AddWithValue("@Product", txtProduct.Text);
            command.Parameters.AddWithValue("@Variant", txtVariant.Text);
            command.Parameters.AddWithValue("@Manufacturer", txtManufacturer.Text);
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


        //private Image compressImage()
        //{
        //    return ;
        //}

    }
}
