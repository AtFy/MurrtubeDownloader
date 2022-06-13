using System;
using System.ComponentModel;
using System.Windows.Forms;


namespace AtfyMurrtubeDownloader
{
    public partial class DownloaderForm : Form
    {
        public DownloaderForm()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            var httpManager = new HttpManager();
            if (httpManager.SetVideoLink(textBox1.Text) == StatusCode.NOT_OK) //Trying to validate and set a video link
            {
                textBox1.Text = "incorrect link"; //!!!!!!MAKE AN ERROR POP-UP FORM
                return;
            }

            progressBar1.Step = 10;

            httpManager.SetHtmlCode();

            var parser = new Parser();
            parser.FindIndexFileLink(httpManager); //looking for the key-link in the HTML file to access .m3u8 file

            progressBar1.PerformStep();

            var file = new FileManager();
            file.SetSavingPath(); //setting up the saving path for the video

            string name = "index.m3u8";
            if (file.Download(parser.GetIndexFileLink(), name, FileType.INDEX) == StatusCode.NOT_OK) //downloading Index.m3u8 file
            {
                textBox1.Text = "incorrect link or saving path"; //!!!!!!MAKE AN ERROR POP-UP FORM
                return;
            }

            progressBar1.PerformStep();

            string rawVideoKey = parser.FindVideoKey(file);
            httpManager.SetVideoKey(rawVideoKey);

            if(file.Delete(name) == StatusCode.NOT_OK)
            {
                textBox1.Text = "incorrect saving path"; //!!!!!!MAKE AN ERROR POP-UP FORM
                return;
            }

            progressBar1.Step = 2;

            int amount = httpManager.DownloadVideoParts(file, this.progressBar1);

            file.Merge(amount, this.progressBar1);

            progressBar1.Value = 300;
            textBox1.Text = null;

        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if((textBox1.Text == "incorrect lin" || textBox1.Text == "incorrect link or saving pat" || textBox1.Text == "incorrect saving pat") && textBox1.ContainsFocus)
            {
                textBox1.Text = null; //!!!!!!MAKE AN ERROR POP-UP FORM
            }
        }

        private void DownloaderForm_Load(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://paypal.me/archiethefox");
        }
    }
}
