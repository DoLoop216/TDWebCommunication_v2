using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TDWebCommunication
{
    class Debug
    {
        public static List<string> ChangeLog = new List<string>();

        public static void Log(string Message)
        {
            string output = string.Format("[ {0} ] {1} {2}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"), Message, Environment.NewLine);
            ChangeLog.Add(output);

            if(TDWebCommunication.LogChanges)
                File.AppendAllText(TDWebCommunication.LogFile, output);
        }
    }
}
