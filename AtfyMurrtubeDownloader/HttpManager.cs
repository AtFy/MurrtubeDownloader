using System;
using System.IO;
using System.Net;
using System.Text;
namespace AtfyMurrtubeDownloader
{
    class HttpManager
    {
        private string htmlCode;
        private string videoLink;

        private string videoKeyPrefix;
        private string videoKey;

        readonly string SATATIC_STORAGE_LINK = @"https://storage.murrtube.net/murrtube/";

        public StatusCode SetVideoLink(string videoLink)
        {
            if (Validator.CheckVideoLink(videoLink) == StatusCode.NOT_OK)
            {
                return StatusCode.NOT_OK;
            }

            this.videoLink = videoLink;

            return StatusCode.OK;
        }

        public void SetHtmlCode()
        {
            var request = (HttpWebRequest)WebRequest.Create(this.videoLink);

            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {


                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                htmlCode = readStream.ReadToEnd();

                response.Close();
                readStream.Close();
            }
        }

        public string GetHtmlCode()
        {
            return this.htmlCode;
        }

        public void SetVideoKey(string rawVideoKey)
        {
            string[] videoKey = rawVideoKey.Split('.');
            this.videoKey = this.videoKeyPrefix + '/' + videoKey[0];
        }

        public void SetVideoKeyPrefix(string videoKeyPrefix)
        {
            this.videoKeyPrefix = videoKeyPrefix;
        }

        public int DownloadVideoParts(FileManager file, System.Windows.Forms.ProgressBar progressBar)
        {
            int amount = 0;
            try
            {
                
                while (true)
                {
                    if(file.Download(this.SATATIC_STORAGE_LINK + this.videoKey + amount.ToString(), amount.ToString() + ".mp4", FileType.TS) == StatusCode.NOT_OK)
                    {
                        throw new Exception();
                    }
                    ++amount;
                    if (progressBar.Value < 250)
                    {
                        progressBar.PerformStep();
                    }
                }
            }
            catch
            {
                return amount;
            }
        }
    }
    
}
