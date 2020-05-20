using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TDWebCommunication
{
    public class User
    {
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Pw { get; set; }
        public int Tip { get; set; }
        public string DisplayName { get; set; }
        public int Aktivan { get; set; }
        public int PPID { get; set; }
        public DateTime DatumRodjenja { get; set; }
        public string Opstina { get; set; }
        public string Adresa { get; set; }
        public string Mobilni { get; set; }
        public int PrimarnoZanimanje { get; set; }
        public int SekundardnoZanimanje { get; set; }
        public int OmiljeniMagacinID { get; set; }
        public string Komentar { get; set; }

        public static string Hash(string value)
        {
            HashAlgorithm algorithm = SHA256.Create();
            byte[] res = algorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in res)
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}
