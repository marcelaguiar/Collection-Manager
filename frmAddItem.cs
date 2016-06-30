using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CollectionsManager
{
    public partial class frmAddItem : Form
    {
        SqlCommand command;
        SqlConnection connection;


        public frmAddItem()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Maybe create a connectionString and set it to the variable in Settings
            using (connection = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\Marcel\Desktop\CollectionsManager\CollectionsManager\Collections.mdf;Integrated Security=True"))
            {
                connection.Open();
                insertTextFieldsAndImage();
            }

            this.Close();
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

                // Create a image processing function
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

            command = new SqlCommand("INSERT INTO Bottlecaps (Maker,Variant,Drink,Method_Acquired,Spares_Available,Text,Icon,Color,Date_Acquired,Underside_Text,Imgpath,Img) VALUES (@Maker, @Variant, @Drink, @MethodAcquired, @SparesAvailable, @Text, @Icon, @Color, @DateAcquired, @Underside, @Imgpath, @Img)", connection);
            command.Parameters.AddWithValue("@Maker", txtMaker.Text);
            command.Parameters.AddWithValue("@Variant", txtVariant.Text);
            command.Parameters.AddWithValue("@Drink", txtDrink.Text);
            command.Parameters.AddWithValue("@MethodAcquired", txtMethodAcquired.Text);
            command.Parameters.AddWithValue("@SparesAvailable", sparesAvailable.Checked);
            command.Parameters.AddWithValue("@Text", txtText.Text);
            command.Parameters.AddWithValue("@Icon", txtIcon.Text);
            command.Parameters.AddWithValue("@Color", txtColor.Text);
            command.Parameters.AddWithValue("@DateAcquired", datePicker.Value);
            command.Parameters.AddWithValue("@Underside", txtUnderside.Text);
            command.Parameters.AddWithValue("@Imgpath", imgPath.Text);
            command.Parameters.AddWithValue("@Img",
                (pictureBox1.Image == null) ? (object)DBNull.Value : byteArray).SqlDbType = SqlDbType.Image;
            command.ExecuteNonQuery();
        }

    }
}
