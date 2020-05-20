using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TDWebCommunication_v2
{
    public class ConnectionStrings
    {
        private string _Komercijalno = null;
        private string _TDOffice = null;
        private string _Config = null;
        private string _Web = null;

        public string Komercijalno { get => _Komercijalno; set => _Komercijalno = value; }
        public string TDOffice { get => _TDOffice; set => _TDOffice = value; }
        public string Config { get => _Config; set => _Config = value; }
        public string Web { get => _Web; set => _Web = value; }

        //{"Komercijalno":"data source=4monitor; initial catalog = C:\\Poslovanje\\Baze\\2020\\FIRMA2020.FDB; user=SYSDBA; password=m","TDOffice":"data source=4monitor; initial catalog = C:\\ORGTASK\\Programi\\Proces5\\ORG\\ORGANIZATOR.FDB; user=SYSDBA; password=m","Config":"data source=4monitor; initial catalog = C:\\Poslovanje\\Baze\\ConfigPDV\\CONFIG.FDB; user=SYSDBA; password=m","Web":"Server=MYSQL6001.site4now.net;Database=db_a45ae3_td;Uid=a45ae3_td;Pwd=Adanijela1;Pooling=false;"}

        public static ConnectionStrings Load()
        {
            if (!Directory.Exists(Settings.AbsolutePath) || !File.Exists(Settings.Path_ConnectionStringsSettings))
                Settings.FirstTimeInitialization();

            string text = File.ReadAllText(Settings.Path_ConnectionStringsSettings);

            return JsonConvert.DeserializeObject<ConnectionStrings>(text);
        }
    }
}
