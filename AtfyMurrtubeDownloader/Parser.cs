using System;
using System.IO;

namespace AtfyMurrtubeDownloader
{
    class Parser
    {
        private string IndexFileLink = null;
        public void FindIndexFileLink(HttpManager httpManager)
        {
            this.IndexFileLink = httpManager.GetHtmlCode();

            string[] separators = { "murrtube/", "/thumbnail" };
            string[] temp = this.IndexFileLink.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            this.IndexFileLink = "https://storage.murrtube.net/murrtube/" + temp[1];

            httpManager.SetVideoKeyPrefix(temp[1]);
        }

        public string FindVideoKey(FileManager file)
        {
            
            var streamReader = new StreamReader(file.GetSavingPath() + "index.m3u8");
            for (int i = 0; i < 2; ++i)
            {
                streamReader.ReadLine();
            }
            string key = streamReader.ReadLine();

            streamReader.Close();
            return key;
        }

        public string GetIndexFileLink()
        {
            return this.IndexFileLink;
        }
    }
}
