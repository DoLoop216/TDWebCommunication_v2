using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TDWebCommunication_v2
{
    public class Settings
    {
        public static bool IsRunning = true;
        public const int RefreshRate = 500; //1000 = 1 second
        public static int WEB_ZAPID = 107;

        public static string AbsolutePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TDWebCommunication_v2");
        public static string Path_ConnectionStringsSettings = Path.Combine(AbsolutePath, "Settings.ar");

        public static void FirstTimeInitialization()
        {
            if (!Directory.Exists(AbsolutePath))
                Directory.CreateDirectory(AbsolutePath);

            if(!File.Exists(Path_ConnectionStringsSettings))
                File.WriteAllText(Path_ConnectionStringsSettings, JsonConvert.SerializeObject(new ConnectionStrings()));

            MessageBox.Show("Putanje do baze nisu definisate. Idite u %appdata%/TDWebCommunication_v2/Settings.ar i podesite parametre!");
        }

    }
}
