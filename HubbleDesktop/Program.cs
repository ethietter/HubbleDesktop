using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Xml;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace HubbleDesktop {
    class Program {
        public object LocalEncoding { get; private set; }

        static void Main(string[] args) {
            Program program = new Program();
        }

        public Program() {
            string url = "http://apod.nasa.gov/apod/astropix.html";
            string file_contents;
            using (WebClient wc = new WebClient()) {
                Console.Write("Preparing to download html file...");
                file_contents = wc.DownloadString(url);
                Console.Write("Done!\n");
            }

            string url_base = "http://apod.nasa.gov/apod/";
            try {
                string img_url = getImageUrl(file_contents);
                string full_path = url_base + img_url;
                setImageAsBackground(full_path);
                Environment.Exit(0);
            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message);
                Console.ReadLine(); //Prevents the console from closing in case of an error
            }
            
        }

        //See https://msdn.microsoft.com/en-us/library/ms724947(VS.85).aspx
        //And http://stackoverflow.com/questions/2886786/change-windows-wallpaper-using-net-4-0
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private void setImageAsBackground(string img_url) {
            using (WebClient client = new WebClient()) {
                DateTime dt = DateTime.Now;
                string date_string = dt.Year.ToString() + "_" + dt.Month.ToString("D2") + "_" + dt.Day.ToString("D2");
                string file_path = AppDomain.CurrentDomain.BaseDirectory + @"wallpapers\" + date_string + ".jpg";
                Console.Write("Preparing to download today's image..");
                client.DownloadFile(img_url, file_path);
                Console.Write("Done!\n");

                //Values retrieved from: https://msdn.microsoft.com/en-us/library/ms724947(VS.85).aspx
                const int SPI_SETDESKTOPWALLPAPER = 0x14;
                const int SPIF_UPDATEINIFILE = 1; //0b01
                const int SPIF_SENDCHANGE = 2; //0b10

                int result = SystemParametersInfo(SPI_SETDESKTOPWALLPAPER, 0, file_path, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
            }
        }

        private string getImageUrl(string webpage) {
            /* There's only one img tag on the webpage and it looks like this (contained within a link tag):
            <a href="image/1601/lorand_fenyes_dipper_big.jpg"
                onMouseOver="if (document.images)
                document.imagename1.src='image/1601/lf_dipper_subt.jpg';"
                onMouseOut="if (document.images)
                document.imagename1.src='image/1601/lf_dipper_messier.jpg';">
                <IMG SRC="image/1601/lf_dipper_messier.jpg" name=imagename1 
                alt="See Explanation.
                Moving the cursor over the image will bring up an annotated version.
                Clicking on the image will bring up the highest resolution version
                available.">
            </a>
            */
            string unescaped = WebUtility.HtmlDecode(webpage);
            Regex pattern = new Regex(@"(?is)<a href=""(image/.*?\.jpg)"".*?<IMG SRC="); //(?is) = Single Line, Ignore Case
            Match match = pattern.Match(unescaped);
            string url = match.Groups[1].Value;
            if(url == "") {
                //The format of the feed changed, so alert me
                throw new Exception("Couldn't download image");
            }
            return url;
        }
    }
}
