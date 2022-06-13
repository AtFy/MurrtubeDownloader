
namespace AtfyMurrtubeDownloader
{
    class Validator
    {
        public static StatusCode CheckVideoLink(string videoLink)
        {
            if (!videoLink.Contains(@"murrtube.net/videos/"))
            {
                return StatusCode.NOT_OK;
            }
            return StatusCode.OK;
           
        }
    }
}
