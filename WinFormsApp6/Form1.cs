using Amazon.Rekognition;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;


namespace WinFormsApp6
{

    public partial class Form1 : Form
    {
        public HttpClient pictureClient { get; set; } = new HttpClient();
        public Root root { get; set; }
        int ID { get; set; } = 0;

        MemoryStream memoryStream = new MemoryStream();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("Human");
            comboBox1.Items.Add("Flower");
            comboBox1.Items.Add("Car");
            comboBox1.Items.Add("Tree");
            comboBox1.Items.Add("Grass");
            comboBox1.Items.Add("House");
            comboBox1.Items.Add("Sky");
            comboBox1.Items.Add("Cloud");
            comboBox1.SelectedIndex = 0;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            pictureBox1.LoadCompleted += PictureBox1_LoadCompleted;
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            LoadPhotos(textBox1.Text);
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetItems();
        }

        private void LoadPhotos(string search)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query.Add("key", "27439269-9d388cb8af1fe5a63ee6e1c4e");
            query.Add("q", search);
            query.Add("image_type", "photo");


            var res = pictureClient.GetAsync("https://pixabay.com/api/?" + query.ToString()).Result;

            res.EnsureSuccessStatusCode();
            res.Content.ReadAsStringAsync();

            root = JsonConvert.DeserializeObject<Root>(res.Content.ReadAsStringAsync().Result);

            LoadImg();
        }

        private  void LoadImg()
        {
            pictureBox1.LoadAsync(root.hits[ID].webformatURL);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ID = ID == 0 ? 19 : ID-1;
            LoadImg();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ID =  ID == 19 ? 0 : ID+1;
            LoadImg();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
                LoadPhotos(textBox1.Text);
        }

        public void GetItems()
        {
            var awsCredentials = new Amazon.Runtime.BasicAWSCredentials("AKIAU65SRJDXLD3MUKEF", "/jT4FxGDl4kWaBf06y3HD1AsiRHu6MEwmpleawRS");
            var client = new AmazonRekognitionClient(awsCreden‌​tials, Amazon.RegionEndpoint.EUWest2);

            
            pictureBox1.Image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            bool a = false;

            client.DetectLabelsAsync(new Amazon.Rekognition.Model.DetectLabelsRequest()
            {
                Image = new Amazon.Rekognition.Model.Image()
                {
                    Bytes = memoryStream
                }
            }).Result.Labels.ToList().ForEach(x =>
            {
                if (a == false)
                {
                    if (x.Name.ToString().Equals(comboBox1.SelectedItem.ToString()))
                    {
                        label1.ForeColor = Color.Green;
                        label1.Text = "НАЙДЕНО";
                        a = true;
                    }
                    else
                    {
                        label1.ForeColor = Color.Red;
                        label1.Text = "НЕ НАЙДЕНО";
                    }
                }
            });

            memoryStream = new MemoryStream();

        }

        private void PictureBox1_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            GetItems();
        }
    }
}
