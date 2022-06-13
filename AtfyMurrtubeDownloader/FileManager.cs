using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
using NReco.VideoConverter;
using System.Collections.Generic;

namespace AtfyMurrtubeDownloader
{
    class FileManager
    {
        private string savingPath = "";
        private string videoName = null;

        private readonly int MERGING_RANGE = 35;
        private readonly int NAMER_START = 30000;

        public StatusCode Download(string link, string name, FileType fileType)
        {
            switch(fileType)
            {
                case FileType.TS:
                    {
                        return DownloadTs(link, name);
                    }
                case FileType.INDEX:
                    {
                        return DownloadIndex(link, name);
                    }
                default:
                    {
                        return StatusCode.NOT_OK;
                    }
            }
                
        }

        public StatusCode DownloadIndex(string link, string name)
        {
            try
            {
                var webClient = new WebClient();
                
                webClient.DownloadFile(new Uri(link + '/' + name), this.savingPath + name);

                return StatusCode.OK;
            }
            catch
            {
                return StatusCode.NOT_OK;
            }
        }

        public StatusCode DownloadTs(string link, string name)
        {
            try
            {
                var webClient = new WebClient();

                webClient.DownloadFile(new Uri(link + ".ts"), this.savingPath + name);

                return StatusCode.OK;
            }
            catch
            {
                return StatusCode.NOT_OK;
            }
        }

        public StatusCode Delete(string name)
        {
            try
            {
                if(!File.Exists(this.savingPath + name))
                {
                    throw new Exception();
                }

                File.Delete(this.savingPath + name);

                return StatusCode.OK;
            }
            catch
            {
                return StatusCode.NOT_OK;
            }

        }

        public void SetSavingPath()
        {
            var saveFileDialog = new SaveFileDialog();

            saveFileDialog.FileName = "yiff";
            saveFileDialog.DefaultExt = "mp4";
            saveFileDialog.Filter = "Video files (*.mp4)|*.mp4|All files (*.*)|*.*";

            saveFileDialog.ShowDialog();


            string[] temp = saveFileDialog.FileName.Split('\\');

            this.videoName = temp[temp.Length - 1]; //video title set by user (e.g. video.mp4)

            for (int i = 0; i <= temp.Length - 2; ++i) //video path set by user (e.g. C:\Documents\)
            {
                this.savingPath += temp[i] + '\\';
            }
            Console.WriteLine();
        }

        public string GetSavingPath()
        {
            return this.savingPath;
        }

        public void Merge(int amount, System.Windows.Forms.ProgressBar progressBar)
        {
            var filesList = new List<string>();
            for (int i = 0; i < amount; ++i)
            {
                filesList.Add(this.savingPath + i.ToString() + ".mp4");
            }

            progressBar.PerformStep();
            progressBar.Step = 30;

            int leftBorder = 0;

            var ffMpeg = new FFMpegConverter();
            var set = new ConcatSettings();
            int namer = NAMER_START;
            int newAmount = 0;

            string[] filesArray;

            bool isEndOfStream = false;
            while (!isEndOfStream)
            {
                if (filesList.Count - 1 >= leftBorder + MERGING_RANGE + 1)
                {
                    filesArray = filesList.GetRange(leftBorder, MERGING_RANGE).ToArray();
                }
                else
                {
                    filesArray = filesList.GetRange(leftBorder, filesList.Count - leftBorder).ToArray();
                    isEndOfStream = true;
                }


                if (isEndOfStream && leftBorder == 0)
                {
                    ffMpeg.ConcatMedia(filesArray, this.savingPath + this.videoName, "mp4", set);
                }
                else
                {
                    
                    ffMpeg.ConcatMedia(filesArray, this.savingPath + namer.ToString() + ".mp4", "mp4", set);
                }
                progressBar.PerformStep();

                if (leftBorder == 0)
                {
                    for (int i = leftBorder; i < leftBorder + MERGING_RANGE; ++i)
                    {
                        if (Delete(i.ToString() + ".mp4") == StatusCode.NOT_OK)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = leftBorder - 1; i < leftBorder + MERGING_RANGE; ++i)
                    {
                        if (Delete(i.ToString() + ".mp4") == StatusCode.NOT_OK)
                        {
                            break;
                        }
                    }
                }

                
                ++namer;
                ++newAmount;
                leftBorder += MERGING_RANGE + 1;
                filesArray = null;
            }
            if (File.Exists(this.savingPath + "30001.mp4"))
            {
                filesList = new List<string>();


                for (int i = NAMER_START; i - NAMER_START < newAmount; ++i)
                {
                    filesList.Add(this.savingPath + i.ToString() + ".mp4");
                }


                filesArray = filesList.GetRange(0, filesList.Count).ToArray();

                ffMpeg.ConcatMedia(filesArray, this.savingPath + this.videoName, "mp4", set);
                progressBar.PerformStep();

                for (int i = NAMER_START; i - NAMER_START < newAmount; ++i)
                {
                    Delete(i.ToString() + ".mp4");
                }
            }

        }

    }
}
