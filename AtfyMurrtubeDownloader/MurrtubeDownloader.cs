using System;
using System.Windows.Forms;

namespace AtfyMurrtubeDownloader
{
    enum StatusCode
    {
        NOT_OK,
        OK
    }

    enum FileType
    {
        TS,
        INDEX
    }
    static class MurrtubeDownloader
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DownloaderForm());

        }
    }
}
