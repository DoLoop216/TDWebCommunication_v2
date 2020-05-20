using FirebirdSql.Data.FirebirdClient;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using AR;
using System.IO;

namespace TDWebCommunication_v2
{
    static class Program
    {
        public static bool isServer = false;
        private static DateTime lastTimeRestarted = DateTime.Now;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AutoRestart();
            Application.Run(new Login());
            Settings.IsRunning = false;
        }

        private static void AutoRestart()
        {
            Thread t = new Thread(() =>
            {
                while (Settings.IsRunning)
                {
                    if (DateTime.Now.Hour > 4)
                    {
                        if (lastTimeRestarted.Date < DateTime.Now.Date)
                        {
                            Application.Restart();
                            Environment.Exit(0);
                        }
                    }

                    // Sleep for two hours. I do it like this because if i put Thread.Sleep(7200000) and exit application this thread will stay stuck and leave process on
                    for(int i = 0; i < 7200; i++)
                    {
                        Thread.Sleep(1000);
                        if (!Settings.IsRunning)
                            break;
                    }
                }
            });

            t.Start();
        }

        public static void Communicate()
        {
            while (Settings.IsRunning)
            {
                try
                {
                    Web.TDAction lastAction = Web.TDAction.Get();

                    if (lastAction != null)
                    {
                        string[] elements = lastAction.Action.Split('|');

                        switch (elements[0])
                        {
                            case "PovuciProizvodeUTDOffice":
                                Debug.Log(DateTime.Now.ToString("[ dd/MM/yyyy - HH:mm ]") + " WEB: Pokrenuta je akcija povlacenja proizvoda u TDOffice! Akciju pokrenuo " + AR.TDShop.User.GetDisplayName(lastAction.Sender));
                                if (!isServer)
                                    break;
                                PovuciProizvodeUTDOffice();

                                lastAction.Remove();
                                break;

                            case "PosaljiPartnereNaWeb":
                                Debug.Log(DateTime.Now.ToString("[ dd/MM/yyyy - HH:mm ]") + " WEB: Pokrenuta je akcija Slanja proizvoda na web! Akciju pokrenuo " + AR.TDShop.User.GetDisplayName(lastAction.Sender));
                                if (!isServer)
                                    break;

                                PosaljiPartnereNaWeb();

                                lastAction.Remove();
                                break;

                            case "PosaljiOvogPartneraNaWeb":
                                break;

                            case "PretvoriUProracun":
                                Debug.Log(DateTime.Now.ToString("[ dd/MM/yyyy - HH:mm ]") + " WEB: Pokrenuta je akcija prebacivanja porudzbine [" + elements[1].ToString() + "] u proracun!" + AR.TDShop.User.GetDisplayName(lastAction.Sender));
                                if (!isServer)
                                    break;

                                PrebaciUProracun(Convert.ToInt32(elements[1]));

                                lastAction.Remove();
                                break;

                            case "SENDSMS":
                                Debug.Log(DateTime.Now.ToString("[ dd/MM/yyyy - HH:mm ]") + " WEB: Pokrenuta je akcija slanja sms-a sa tekstom '" + elements[2] + "' na broj " + elements[1] + "!" + AR.TDShop.User.GetDisplayName(lastAction.Sender));
                                if (!isServer)
                                    break;

                                PosaljiSms(elements[1], elements[2], Convert.ToInt32(elements[3]));
                                lastAction.Remove();
                                break;

                            case "PROVERISTANJE":
                                Debug.Log(DateTime.Now.ToString("[ dd/MM/yyyy - HH:mm ]") + " WEB: Pokrenuta je akcija provere stanja po web porudzbini " + elements[1] + "! - " + AR.TDShop.User.GetDisplayName(lastAction.Sender));
                                ProveriStanje();
                                lastAction.Remove();
                                break;

                            default:
                                Debug.Log("Nepoznata akcija: " + lastAction.Action);
                                break;
                        }
                    }
                }
                catch(Exception ex)
                {
                    Debug.Log(ex.ToString());
                }
                Thread.Sleep(1000);
            }
        }

        public static void Communicate_DocumentValidation()
        {
            Thread.Sleep(10000);
            while(Settings.IsRunning)
            {
                try
                {
                    List<Web.Porudzbina> porudzbine = Web.Porudzbina.List();

                    foreach(Web.Porudzbina p in porudzbine.Where(x => x.Status == Web.Porudzbina.status.CekaUplatu))
                    {
                        if (p.BrDokKom < 1 && p.Datum.Year != 2020)
                            continue;

                        p.UcitajStavke();

                        Debug.Log("Porudzbina web id: " + p.PorudzbinaID);

                        foreach (Web.Porudzbina.Item i in p.Items)
                            Debug.Log("RobaID: " + i.RobaID + " - Kolicina: " + i.Kolicina);

                        Debug.Log("Proracun kom: " + p.BrDokKom);

                        foreach(Komercijalno.Stavka s in Komercijalno.Stavka.List(32, p.BrDokKom))
                            Debug.Log("RobaID: " + s.RobaID + " - Kolicina: " + s.Kolicina);

                        Debug.Log("Gotovo!");

                        break;
                    }
                }
                catch(Exception ex)
                {
                    throw new ARException(ex.ToString());
                }
                // Provera svakih 5 minuta
                Thread.Sleep(300000);
            }
        }

        private static void ProveriStanje()
        {

        }

        private static void PosaljiSms(string Mobilni, string Text, int Status)
        {
            try
            {
                using(FbConnection con = new FbConnection(Buffer.ConnectionStrings.Config))
                {
                    con.Open();
                    using (FbCommand cmd = new FbCommand(@"INSERT INTO SEND_SMS 
                        (SMSID, VREME, BROJ_TEL, SMS_TEXT, VRDOK, BRDOK, ALIASID, VRSTA, ZAPID, PRIMALAC, PPID, STATUS, VREME_SLANJA, CUST_ID)
                        VALUES (((SELECT COALESCE(MAX(SMSID), 1) FROM SEND_SMS) + 1), @CURRDATE, @BROJ, @TEXT, -1, -1, 37, 0, @ZAP, 'TDSHOP', 1, @STATUS, NULL, 0)", con))
                    {
                        cmd.Parameters.AddWithValue("@CURRDATE", DateTime.Now);
                        cmd.Parameters.AddWithValue("@BROJ", Mobilni);
                        cmd.Parameters.AddWithValue("@TEXT", Text);
                        cmd.Parameters.AddWithValue("@STATUS", Status);
                        cmd.Parameters.AddWithValue("@ZAP", Settings.WEB_ZAPID);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }
        private static void PosaljiPartnereNaWeb()
        {
            try
            {
                Debug.Log("Ucitavanje partnera...");
                List<Komercijalno.Partner> partneri = Komercijalno.Partner.List();
                Debug.Log("Partneri ucitani!");

                Debug.Log("Ubacujem partnere u web bazu...");
                int i = 0;
                using (MySqlConnection con = new MySqlConnection(Buffer.ConnectionStrings.Web))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"INSERT INTO POSLOVNI_PARTNER (ppid, naziv, adresa, pib)
                            values (@ppid, @n, @a, @pib) ON DUPLICATE KEY UPDATE NAZIV = @n, ADRESA = @a, PIB = @pib", con))
                    {
                        cmd.Parameters.Add("@ppid", MySqlDbType.Int32);
                        cmd.Parameters.Add("@n", MySqlDbType.VarChar);
                        cmd.Parameters.Add("@a", MySqlDbType.VarChar);
                        cmd.Parameters.Add("@pib", MySqlDbType.VarChar);

                        foreach(Komercijalno.Partner p in partneri)
                        {
                            Debug.Log(string.Format("{0} / {1}", i + 1, partneri.Count));
                            i++;
                            cmd.Parameters["@ppid"].Value = p.PPID;
                            cmd.Parameters["@n"].Value = p.Naziv;
                            cmd.Parameters["@a"].Value = p.Adresa;
                            cmd.Parameters["@pib"].Value = p.PIB;

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                Debug.Log("Proces prebacivanja partnera uspesno zavrsen!");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private static void PosaljiPartneraNaWeb(int PPID)
        {

            try
            {
                Debug.Log("Ucitavanje partnera...");
                Komercijalno.Partner partner = new Komercijalno.Partner(PPID);
                Debug.Log("Partner ucitan!");

                Debug.Log("Ubacujem partnere u web bazu...");
                using (MySqlConnection con = new MySqlConnection(Buffer.ConnectionStrings.Web))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"INSERT INTO POSLOVNI_PARTNER (ppid, naziv, adresa, pib)
                            values (@ppid, @n, @a, @pib) ON DUPLICATE KEY UPDATE NAZIV = @n, ADRESA = @a, PIB = @pib", con))
                    {
                        cmd.Parameters.AddWithValue("@ppid", partner.PPID);
                        cmd.Parameters.AddWithValue("@n", partner.Naziv);
                        cmd.Parameters.AddWithValue("@a", partner.Adresa);
                        cmd.Parameters.AddWithValue("@pib", partner.PIB);
                    }
                }
                Debug.Log("Proces prebacivanja partnera uspesno zavrsen!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private static void PovuciProizvodeUTDOffice()
        {
            try
            {
                List<int> list = new List<int>();
                using (MySqlConnection con = new MySqlConnection(Buffer.ConnectionStrings.Web))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT ROBAID FROM ROBA", con))
                    {
                        MySqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                            list.Add(Convert.ToInt32(dr[0]));
                    }
                }
                using (FbConnection con = new FbConnection(Buffer.ConnectionStrings.TDOffice))
                {
                    con.Open();
                    using (FbCommand cmd = new FbCommand(@"INSERT INTO WEB_CENA (WCID, ROBAID, WEB_CENA_BP) 
                        VALUES (@WCID, @R, 0)", con))
                    {
                        cmd.Parameters.Add("@R", FbDbType.Integer);
                        cmd.Parameters.Add("@WCID", FbDbType.Integer);

                        foreach (int i in list)
                        {
                            if (!TDOffice.WebCena.Exist(i))
                            {
                                cmd.Parameters["@R"].Value = i;
                                cmd.Parameters["@WCID"].Value = (TDOffice.WebCena.MaxID() + 1);

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private static void PrebaciUProracun(int PorudzbinaID)
        {
            try
            {
                Debug.Log("Ucitavam porudzbinu...");
                AR.TDShop.Porudzbina p = new AR.TDShop.Porudzbina(PorudzbinaID);
                Debug.Log("Ucitavam stavke porudzbine...");
                p.UcitajStavke();

                Debug.Log("Proveravam da li porudzbina ima stavke...");
                if (p.Items == null)
                {
                    Debug.Log("Porudzbina nema stavke!");
                    return;
                }

                int nu = 5;

                Debug.Log("Definisem nacin uplate...");
                if (p.NacinUplate == AR.TDShop.Porudzbina.PorudzbinaNacinUplate.Virmanom)
                    nu = 1;

                Debug.Log("Kreiram dokument proracuna u komercijalnom...");
                Debug.Log("VRDOK: 32");
                Debug.Log("MAGACINID: " + p.MagacinID.ToString());
                Debug.Log("PORUDZBINAID:" + p.PorudzbinaID);
                Debug.Log("PPID: " + p.PPID);
                Debug.Log("NUID: " + nu);

                if (p.PPID == null && p.PPID <= 0)
                    p.PPID = null;

                int newDok = Komercijalno.Dokument.Add(32, p.MagacinID, "WEB: " + p.PorudzbinaID, p.PPID, nu);
                Debug.Log("Kreiran novi proracun broj: " + newDok.ToString());


                Debug.Log("Azuriranje statusa porudzbine da ceka uplatu...");
                AR.TDShop.Porudzbina.SetStatus(p.PorudzbinaID, AR.TDShop.Porudzbina.PorudzbinaStatus.CekaUplatu);

                if (newDok <= 0)
                {
                    Debug.Log("Error creating new document!");
                    return;
                }

                Debug.Log("Azuriranje komentara u komercijalnom...");
                List<string> op = new List<string>();
                List<string> kom = new List<string>();

                op.Add("Web User ID: " + p.UserID);
                op.Add("Web Porudzbina: " + p.PorudzbinaID);

                if(p.Tag.Dostava)
                    op.Add("Dostaviti!!!!");

                op.Add("");
                op.Add("==============================");
                op.Add("==============================");
                op.Add("==============================");
                op.Add("");

                if (!string.IsNullOrWhiteSpace(p.Tag.KontaktMobilni))
                {
                    kom.Add("Kupac je ostavio kontakt: " + p.Tag.KontaktMobilni);
                    op.Add("Kupac je ostavio kontakt: " + p.Tag.KontaktMobilni);
                }

                if (!string.IsNullOrWhiteSpace(p.Tag.AdresaIsporuke))
                {
                    kom.Add("Adresa isporuke: " + p.Tag.AdresaIsporuke);
                    op.Add("Adresa isporuke: " + p.Tag.AdresaIsporuke);
                }

                if (!string.IsNullOrWhiteSpace(p.Tag.Napomena))
                {
                    kom.Add("Kupac je ostavio napomenu: " + p.Tag.Napomena);
                    op.Add("Kupac je ostavio napomenu: " + p.Tag.Napomena);
                }

                if (!string.IsNullOrWhiteSpace(p.Tag.KomercijalnoInterniKomentar))
                {
                    op.Add("");
                    op.Add("==============================");
                    op.Add("======= Admin interni komentar =======");
                    op.Add("==============================");
                    op.Add("");
                    op.Add(p.Tag.KomercijalnoInterniKomentar);
                }

                Komercijalno.Dokument.UpdateInterniKomentar(32, newDok, string.Join(Environment.NewLine, op));
                Komercijalno.Dokument.UpdateKomentar(32, newDok, string.Join(Environment.NewLine, kom));
                // p.InterniKomentar umesto u interni komentar staviti u komentar komercijalnog

                Debug.Log("Pocinjem unos stavki...");
                using (FbConnection con = new FbConnection(Buffer.ConnectionStrings.Komercijalno))
                {
                    con.Open();
                    using (FbCommand cmd = new FbCommand("NAPRAVISTAVKU", con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("VRDOK", 32);
                        cmd.Parameters.AddWithValue("BRDOK", newDok);
                        cmd.Parameters.Add("ROBAID", FbDbType.Integer);
                        cmd.Parameters.Add("CENA_BEZ_PDV", FbDbType.Double);
                        cmd.Parameters.Add("KOL", FbDbType.Double);
                        cmd.Parameters.AddWithValue("RABAT", 0);

                        foreach (AR.TDShop.Porudzbina.Item i in p.Items)
                        {
                            Debug.Log("Unosim stavku " + i.RobaID + "...");

                            cmd.Parameters["ROBAID"].Value = i.RobaID;
                            cmd.Parameters["CENA_BEZ_PDV"].Value = i.VPCena * (1 + (Komercijalno.Roba.GetPDV(i.RobaID) / 100));
                            cmd.Parameters["KOL"].Value = i.Kolicina;

                            cmd.ExecuteNonQuery();

                            Debug.Log("Unesena stavka: " + i.RobaID.ToString());
                        }

                        Debug.Log("Povezujem dokument komercijalnog sa porudzbinom...");

                        AR.TDShop.Porudzbina.SetBrDokKom(p.PorudzbinaID, newDok);
                    }
                }

                Debug.Log("Zakljucavam dokument...");
                Komercijalno.Dokument.Zakljucaj(32, newDok);

                Debug.Log("Zavrsen proces prebacivanja u proracun!" + Environment.NewLine);
                Thread.Sleep(2000);
            }
            catch(Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }
    }
    public class Cena
    {
        public int RobaID { get; set; }
        public double VPCena { get; set; }
    }
    public class Komercijalno
    {
        private static bool Initialized = false;
        public class Dokument
        {
            private string SQL_Select = @"SELECT VRDOK, BRDOK, INTBROJ, KODDOK, FLAG, DATUM, LINKED, MAGACINID, PPID, FAKTDOBIZV, PLACEN, DATROKA, NUID, NRID, VALUTA, KURS, ZAPID,
                UPLACENO, TROSKOVI, DUGUJE, POTRAZUJE, POPUST, RAZLIKA, DODPOREZ, POREZ, PRODVREDBP, KUPAC, OPISUPL, VRDOKIN, BRDOKIN, VRDOKOUT, BRDOKOUT, MAGID, POPUST1DANA, POPUST1PROCENAT,
                POPUST2DANA, POPUST2PROCENAT, POPUST3DANA, POZNABROJ, POPUST3PROCENAT, MTID, REFID, STATUS, PPO, PRENETI_POREZ, AKVRDOK, AKBRDOK, ALIASIZ, ALIASU, PREVOZROBE,
                DATUM_PDV, NDID, NABVREDNOST, SAT_START, SAT_END, KNJIZNAOZ, POVRATNICE, SINHRO, STORNO, SMENAID, POR_ODB, POPDVBROJ, PROMET_BEZ_NAKNADE FROM DOKUMENT";

            #region Properties
            public int VrDok { get; set; }
            public int BrDok { get; set; }
            public string IntBroj { get; set; }
            public int KodDok { get; set; }
            public int Flag { get; set; }
            public DateTime Datum { get; set; }
            public string Linked { get; set; }
            public int MagacinID { get; set; }
            public int? PPID { get; set; }
            public string FaktDobIzv { get; set; }
            public int Placen { get; set; }
            public DateTime? DatRoka { get; set; }
            public int? NUID { get; set; }
            public int? NRID { get; set; }
            public string Valuta { get; set; }
            public double Kurs { get; set; }
            public int ZapID { get; set; }
            public double Uplaceno { get; set; }
            public double Troskovi { get; set; }
            public double Duguje { get; set; }
            public double Potrazuje { get; set; }
            public double Popust { get; set; }
            public double Razlika { get; set; }
            public double DodPorez { get; set; }
            public double Porez { get; set; }
            public double ProdVredBP { get; set; }
            public string Kupac { get; set; }
            public string OpisUpl { get; set; }
            public int? VrDokIN { get; set; }
            public int? BrDokIN { get; set; }
            public int? VrDokOUT { get; set; }
            public int? BrDokOUT { get; set; }
            public int? MagID { get; set; }
            public int Popust1Dana { get; set; }
            public double Popust1Procenat { get; set; }
            public int Popust2Dana { get; set; }
            public double Popust2Procenat { get; set; }
            public int Popust3Dana { get; set; }
            public double Popust3Procenat { get; set; }
            public string PozNaBroj { get; set; }
            public string MTID { get; set; }
            public int? RefID { get; set; }
            public int? Status { get; set; }
            public int PPO { get; set; }
            public double PrenetiPorez { get; set; }
            public int AkVrDok { get; set; }
            public int AkBrDok { get; set; }
            public int? AliasIZ { get; set; }
            public int? AliasU { get; set; }
            public int PrevozRobe { get; set; }
            public DateTime? DatumPDV { get; set; }
            public int? NDID { get; set; }
            public double NabVrednost { get; set; }
            public string SatStart { get; set; }
            public string SatEnd { get; set; }
            public double KnjiznaOZ { get; set; }
            public double Povratnice { get; set; }
            public int Sinhro { get; set; }
            public double Storno { get; set; }
            public int SmenaID { get; set; }
            public double PorOdb { get; set; }
            public string PoPDVBroj { get; set; }
            public int PrometBezNaknade { get; set; }
            #endregion

            public Dokument(int VrDok, int BrDok)
            {
                try
                {
                    Debug.Log("Selektujem dokument iz komecijalnog - VRDOK: " + VrDok + ", BRDOK: " + BrDok);
                    using(FbConnection con = new FbConnection(Buffer.ConnectionStrings.Komercijalno))
                    {
                        con.Open();
                        Debug.Log("Ucitavam iz baze...");
                        using(FbCommand cmd = new FbCommand(SQL_Select + " WHERE VRDOK = @V AND BRDOK = @B", con))
                        {
                            cmd.Parameters.AddWithValue("@V", VrDok);
                            cmd.Parameters.AddWithValue("@B", BrDok);

                            using(FbDataReader dr = cmd.ExecuteReader())
                            {
                                if (dr.Read())
                                {
                                    this.VrDok = Convert.ToInt32(dr["VRDOK"]);
                                    this.BrDok = Convert.ToInt32(dr["BRDOK"]);
                                    this.IntBroj = dr["INTBROJ"] is DBNull ? null : dr["INTBROJ"].ToString();
                                    this.KodDok = Convert.ToInt32(dr["KODDOK"]);
                                    this.Flag = Convert.ToInt32(dr["FLAG"]);
                                    this.Datum = Convert.ToDateTime(dr["DATUM"]);
                                    this.Linked = dr["LINKED"].ToString();
                                    this.MagacinID = Convert.ToInt32(dr["MAGACINID"]);
                                    this.PPID = dr["PPID"] is DBNull ? null : (int?)Convert.ToInt32(dr["PPID"]);
                                    this.FaktDobIzv = dr["FAKTDOBIZV"].ToString();
                                    this.Placen = Convert.ToInt32(dr["PLACEN"]);
                                    this.DatRoka = dr["DATROKA"] is DBNull ? null : (DateTime?)Convert.ToDateTime(dr["DATROKA"]);
                                    this.NUID = dr["NUID"] is DBNull ? null : (int?) Convert.ToInt32(dr["NUID"]);
                                    this.NRID = dr["NRID"] is DBNull ? null : (int?)Convert.ToInt32(dr["NRID"]);
                                    this.Valuta = dr["VALUTA"].ToString();
                                    this.Kurs = Convert.ToDouble(dr["KURS"]);
                                    this.ZapID = Convert.ToInt32(dr["ZAPID"]);
                                    this.Uplaceno = Convert.ToDouble(dr["UPLACENO"]);
                                    this.Troskovi = Convert.ToDouble(dr["TROSKOVI"]);
                                    this.Duguje = Convert.ToDouble(dr["DUGUJE"]);
                                    this.Potrazuje = Convert.ToDouble(dr["POTRAZUJE"]);
                                    this.Popust = Convert.ToDouble(dr["POPUST"]);
                                    this.Razlika = Convert.ToDouble(dr["RAZLIKA"]);
                                    this.DodPorez = Convert.ToDouble(dr["DODPOREZ"]);
                                    this.Porez = Convert.ToDouble(dr["POREZ"]);
                                    this.ProdVredBP = Convert.ToDouble(dr["PRODVREDBP"]);
                                    this.Kupac = dr["KUPAC"] is DBNull ? null : dr["KUPAC"].ToString();
                                    this.OpisUpl = dr["OPISUPL"] is DBNull ? null : dr["OPISUPL"].ToString();
                                    this.VrDokIN = dr["VRDOKIN"] is DBNull ? null : (int?)Convert.ToInt32(dr["VRDOKIN"]);
                                    this.BrDokIN = dr["BRDOKIN"] is DBNull ? null : (int?)Convert.ToInt32(dr["BRDOKIN"]);
                                    this.VrDokOUT = dr["VRDOKOUT"] is DBNull ? null : (int?)Convert.ToInt32(dr["VRDOKOUT"]);
                                    this.BrDokOUT = dr["BRDOKOUT"] is DBNull ? null : (int?)Convert.ToInt32(dr["BRDOKOUT"]);
                                    this.MagID = dr["MAGID"] is DBNull ? null : (int?)Convert.ToInt32(dr["MAGID"]);
                                    this.Popust1Dana = Convert.ToInt32(dr["POPUST1DANA"]);
                                    this.Popust1Procenat = Convert.ToInt32(dr["POPUST1PROCENAT"]);
                                    this.Popust2Dana = Convert.ToInt32(dr["POPUST2DANA"]);
                                    this.Popust2Procenat = Convert.ToInt32(dr["POPUST2PROCENAT"]);
                                    this.Popust3Dana = Convert.ToInt32(dr["POPUST3DANA"]);
                                    this.Popust3Procenat = Convert.ToInt32(dr["POPUST3PROCENAT"]);
                                    this.PozNaBroj = dr["POZNABROJ"].ToString();
                                    this.MTID = dr["MTID"] is DBNull ? null : dr["MTID"].ToString();
                                    this.RefID = dr["REFID"] is DBNull ? null : (int?)Convert.ToInt32(dr["REFID"]);
                                    this.Status = dr["STATUS"] is DBNull ? null : (int?)Convert.ToInt32(dr["STATUS"]);
                                    this.PPO = Convert.ToInt32(dr["PPO"]);
                                    this.PrenetiPorez = Convert.ToDouble(dr["PRENETI_POREZ"]);
                                    this.AkVrDok = Convert.ToInt32(dr["AKVRDOK"]);
                                    this.AkBrDok = Convert.ToInt32(dr["AKBRDOK"]);
                                    this.AliasIZ = dr["ALIASIZ"] is DBNull ? null : (int?)Convert.ToInt32(dr["ALIASIZ"]);
                                    this.AliasU = dr["ALIASU"] is DBNull ? null : (int?)Convert.ToInt32(dr["ALIASU"]);
                                    this.PrevozRobe = Convert.ToInt32(dr["PREVOZROBE"]);
                                    this.DatumPDV = dr["DATUM_PDV"] is DBNull ? null : (DateTime?)Convert.ToDateTime(dr["DATUM_PDV"]);
                                    this.NDID = dr["NDID"] is DBNull ? null : (int?)Convert.ToInt32(dr["NDID"]);
                                    this.NabVrednost = Convert.ToDouble(dr["NABVREDNOST"]);
                                    this.SatStart = dr["SAT_START"] is DBNull ? null : dr["SAT_START"].ToString();
                                    this.SatEnd = dr["SAT_END"] is DBNull ? null : dr["SAT_END"].ToString();
                                    this.KnjiznaOZ = Convert.ToDouble(dr["KNJIZNAOZ"]);
                                    this.Povratnice = Convert.ToDouble(dr["POVRATNICE"]);
                                    this.Sinhro = Convert.ToInt32(dr["SINHRO"]);
                                    this.Storno = Convert.ToInt32(dr["STORNO"]);
                                    this.SmenaID = Convert.ToInt32(dr["SMENAID"]);
                                    this.PorOdb = Convert.ToDouble(dr["POR_ODB"]);
                                    this.PoPDVBroj = dr["POPDVBROJ"] is DBNull ? null : dr["POPDVBROJ"].ToString();
                                    this.PrometBezNaknade = Convert.ToInt32(dr["PROMET_BEZ_NAKNADE"]);
                                }
                                else
                                {
                                    throw new ARException("Dokument sa zadatim parametrima nije pronadjen");
                                }
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    throw new ARException(ex.ToString());
                }
            }
            public static int Add(int VrDok, int MagacinID, string InterniBroj, int? PPID, int NUID)
            {
                try
                {
                    Debug.Log("Uzimam poslednji broj dokumenta za VRDOK: " + VrDok + " i MAGACINID: " + MagacinID);
                    int pos = Poslednji(VrDok, MagacinID);

                    using (FbConnection con = new FbConnection(Buffer.ConnectionStrings.Komercijalno))
                    {
                        Debug.Log("Insertujem u bazu komercijalnog...");
                        con.Open();
                        using (FbCommand cmd = new FbCommand(@"INSERT INTO DOKUMENT
                            (VRDOK, BRDOK, INTBROJ, KODDOK, FLAG, DATUM, LINKED, MAGACINID, PPID, FAKTDOBIZV, PLACEN, DATROKA,
                            NUID, NRID, VALUTA, KURS, ZAPID, UPLACENO, TROSKOVI, DUGUJE, POTRAZUJE, POPUST, RAZLIKA, DODPOREZ,
                            POREZ, PRODVREDBP, KUPAC, OPISUPL, VRDOKIN, BRDOKIN, VRDOKOUT, BRDOKOUT, MAGID, POPUST1DANA,
                            POPUST1PROCENAT, POPUST2DANA, POPUST2PROCENAT, POPUST3DANA, POZNABROJ, POPUST3PROCENAT, KONTRBROJ,
                            MTID, REFID, STATUS, PPO, PRENETI_POREZ, AKVRDOK, AKBRDOK, ALIASIZ, ALIASU, PREVOZROBE, DATUM_PDV,
                            NDID, NABVREDNOST, SAT_START, SAT_END, KNJIZNAOZ, POVRATNICE, SINHRO, STORNO, SMENAID, POR_ODB,
                            POPDVBROJ, PROMET_BEZ_NAKNADE)

                            VALUES

                            (@VRDOK, @BRDOK, @INTBROJ, 0, 0, @DATUM, 0, @MAGACINID, @PPID, NULL, 0, @DATUM, @NUID, 1, 'DIN', 1, 0,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, 0, 0, 0,
                            05, 0, NULL, (SELECT MTID FROM MAGACIN WHERE MAGACINID = @MAGACINID), -1, 0, 1, 0, 0, 0, NULL, NULL,
                            0, @DATUM, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)", con))
                        {
                            cmd.Parameters.AddWithValue("@VRDOK", VrDok);
                            cmd.Parameters.AddWithValue("@MAGACINID", MagacinID);
                            cmd.Parameters.AddWithValue("@BRDOK", pos + 1);
                            cmd.Parameters.AddWithValue("@INTBROJ", InterniBroj);
                            cmd.Parameters.AddWithValue("@DATUM", DateTime.Now);
                            cmd.Parameters.AddWithValue("@PPID", (PPID == null) ? (object)DBNull.Value : PPID.ToString());
                            cmd.Parameters.AddWithValue("@NUID", NUID);

                            cmd.ExecuteNonQuery();
                            Debug.Log("Vracam novi broj dokumenta: " + (pos + 1).ToString());
                            return (pos + 1);
                        }
                    }
                }
                catch(Exception ex)
                {
                    throw new ArgumentException(ex.ToString());
                }
            }


            public static void UpdateInterniKomentar(int VrDok, int BrDok, string InterniKomentar)
            {
                using(FbConnection con = new FbConnection(Buffer.ConnectionStrings.Komercijalno))
                {
                    con.Open();

                    bool exist = false;
                    Debug.Log("Proveravam da li interni komentar postoji...");
                    using(FbCommand cmd = new FbCommand("SELECT COUNT(VRDOK) FROM KOMENTARI WHERE VRDOK = @VRDOK AND BRDOK = @BRDOK", con))
                    {
                        cmd.Parameters.AddWithValue("@VRDOK", VrDok);
                        cmd.Parameters.AddWithValue("@BRDOK", BrDok);

                        using (FbDataReader dr = cmd.ExecuteReader())
                            if (dr.Read())
                                if (Convert.ToInt32(dr[0]) > 0)
                                    exist = true;
                    }


                    Debug.Log("Exist: " + exist.ToString());
                    if(exist)
                    {
                        Debug.Log("Updating interni komentar...");
                        using(FbCommand cmd = new FbCommand("UPDATE KOMENTARI SET INTKOMENTAR = @KOM WHERE VRDOK = @VRDOK AND BRDOK = @BRDOK", con))
                        {
                            cmd.Parameters.AddWithValue("@VRDOK", VrDok);
                            cmd.Parameters.AddWithValue("@BRDOK", BrDok);
                            cmd.Parameters.AddWithValue("@KOM", InterniKomentar);

                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        Debug.Log("Inserting interni komentar...");
                        using (FbCommand cmd = new FbCommand("INSERT INTO KOMENTARI (VRDOK, BRDOK, INTKOMENTAR) VALUES (@VRDOK, @BRDOK, @KOM)", con))
                        {
                            cmd.Parameters.AddWithValue("@VRDOK", VrDok);
                            cmd.Parameters.AddWithValue("@BRDOK", BrDok);
                            cmd.Parameters.AddWithValue("@KOM", InterniKomentar);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            public static void UpdateKomentar(int VrDok, int BrDok, string Komentar)
            {
                using (FbConnection con = new FbConnection(Buffer.ConnectionStrings.Komercijalno))
                {
                    con.Open();

                    bool exist = false;
                    Debug.Log("Proveravam da li komentar postoji...");
                    using (FbCommand cmd = new FbCommand("SELECT COUNT(VRDOK) FROM KOMENTARI WHERE VRDOK = @VRDOK AND BRDOK = @BRDOK", con))
                    {
                        cmd.Parameters.AddWithValue("@VRDOK", VrDok);
                        cmd.Parameters.AddWithValue("@BRDOK", BrDok);

                        using (FbDataReader dr = cmd.ExecuteReader())
                            if (dr.Read())
                                if (Convert.ToInt32(dr[0]) > 0)
                                    exist = true;
                    }


                    Debug.Log("Exist: " + exist.ToString());
                    if (exist)
                    {
                        Debug.Log("Updating interni komentar...");
                        using (FbCommand cmd = new FbCommand("UPDATE KOMENTARI SET KOMENTAR = @KOM WHERE VRDOK = @VRDOK AND BRDOK = @BRDOK", con))
                        {
                            cmd.Parameters.AddWithValue("@VRDOK", VrDok);
                            cmd.Parameters.AddWithValue("@BRDOK", BrDok);
                            cmd.Parameters.AddWithValue("@KOM", Komentar);

                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        Debug.Log("Inserting interni komentar...");
                        using (FbCommand cmd = new FbCommand("INSERT INTO KOMENTARI (VRDOK, BRDOK, KOMENTAR) VALUES (@VRDOK, @BRDOK, @KOM)", con))
                        {
                            cmd.Parameters.AddWithValue("@VRDOK", VrDok);
                            cmd.Parameters.AddWithValue("@BRDOK", BrDok);
                            cmd.Parameters.AddWithValue("@KOM", Komentar);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }

            public static int Poslednji(int VrDok, int MagacinID)
            {
                try
                {
                    using (FbConnection con = new FbConnection(Buffer.ConnectionStrings.Komercijalno))
                    {
                        con.Open();
                        using (FbCommand cmd = new FbCommand("SELECT POSLEDNJI FROM VRSTADOKMAG WHERE MAGACINID = @M AND VRDOK = @V", con))
                        {
                            cmd.Parameters.AddWithValue("@M", MagacinID);
                            cmd.Parameters.AddWithValue("@V", VrDok);

                            using (FbDataReader dr = cmd.ExecuteReader())
                                if (dr.Read())
                                    return Convert.ToInt32(dr[0]);
                        }

                        using (FbCommand cmd = new FbCommand("SELECT POSLEDNJI FROM VRSTADOK WHERE VRDOK = @V", con))
                        {
                            cmd.Parameters.AddWithValue("@V", VrDok);

                            using (FbDataReader dr = cmd.ExecuteReader())
                                if (dr.Read())
                                    return Convert.ToInt32(dr[0]);
                        }
                    }
                    return 0;
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
            public static void Zakljucaj(int VrDok, int BrDok)
            {
                try
                {
                    using (FbConnection con = new FbConnection(Buffer.ConnectionStrings.Komercijalno))
                    {
                        con.Open();
                        using (FbCommand cmd = new FbCommand("UPDATE DOKUMENT SET STATUS = 1 WHERE VRDOK = @V AND BRDOK = @B", con))
                        {
                            cmd.Parameters.AddWithValue("@V", VrDok);
                            cmd.Parameters.AddWithValue("@B", BrDok);


                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public class Stavka
        {
            private static string SQL_Select = @"SELECT STAVKAID, VRDOK, BRDOK, MAGACINID, ROBAID, VRSTA, NAZIV, NABCENSAPOR, FAKTURNACENA, NABCENABT, TROSKOVI,
                NABAVNACENA, PRODCENABP, KOREKCIJA, PRODAJNACENA, DEVIZNACENA, DEVPRODCENA, KOLICINA, NIVKOL, TARIFAID, IMAPOREZ, POREZ, RABAT, MARZA, TAKSA,
                AKCIZA, PROSNAB, PRECENA, PRENAB, PROSPROD, MTID, PT, ZVEZDICA, TREN_STANJE, POREZ_ULAZ, SDATUM, DEVNABCENA, POREZ_IZ, X4, Y4, Z4, CENAPOAJM,
                KGID, SAKCIZA FROM STAVKA";
            public int StavkaID { get; set; }
            public int VrDok { get; set; }
            public int BrDok { get; set; }
            public int MagacinID { get; set; }
            public int RobaID { get; set; }
            public int? Vrsta { get; set; }
            public string Naziv { get; set; }
            public double? NabCenaSaPor { get; set; }
            public double? FakturaNaCena { get; set; }
            public double? NabCenaBT { get; set; }
            public double? Troskovi { get; set; }
            public double NabavnaCena { get; set; }
            public double ProdCenaBP { get; set; }
            public double? Korekcija { get; set; }
            public double ProdajnaCena { get; set; }
            public double DeviznaCena { get; set; }
            public double? DevProdCena { get; set; }
            public double Kolicina { get; set; }
            public double NivKol { get; set; }
            public string TarifaID { get; set; }
            public int? ImaPorez { get; set; }
            public double Porez { get; set; }
            public double Rabat { get; set; }
            public double Marza { get; set; }
            public double? Taksa { get; set; }
            public double? Akciza { get; set; }
            public double ProsNab { get; set; }
            public double PreCena { get; set; }
            public double PreNab { get; set; }
            public double ProsProd { get; set; }
            public string MTID { get; set; }
            public string PT { get; set; }
            public string Zvezdica { get; set; }
            public double TrenStanje { get; set; }
            public double PorezUlaz { get; set; }
            public DateTime? SDatum { get; set; }
            public double? DevNabCena { get; set; }
            public double PorezIz { get; set; }
            public double? X4 { get; set; }
            public double? Y4 { get; set; }
            public double? Z4 { get; set; }
            public double CenaPOAJM { get; set; }
            public int? KGID { get; set; }
            public double SAkciza { get; set; }

            public static List<Stavka> List(int VrDok, int BrDok)
            {
                List<Stavka> list = new List<Stavka>();
                try
                {
                    using(FbConnection con = new FbConnection(Buffer.ConnectionStrings.Komercijalno))
                    {
                        con.Open();
                        using(FbCommand cmd = new FbCommand(SQL_Select + " WHERE VRDOK = @V AND BRDOK = @B", con))
                        {
                            cmd.Parameters.AddWithValue("@V", VrDok);
                            cmd.Parameters.AddWithValue("@B", BrDok);

                            using (FbDataReader dr = cmd.ExecuteReader())
                                while (dr.Read())
                                    list.Add(new Stavka()
                                    {
                                        StavkaID = Convert.ToInt32(dr["STAVKAID"]),
                                        VrDok = Convert.ToInt32(dr["VRDOK"]),
                                        BrDok = Convert.ToInt32(dr["BRDOK"]),
                                        MagacinID = Convert.ToInt32(dr["MAGACINID"]),
                                        RobaID = Convert.ToInt32(dr["ROBAID"]),
                                        Vrsta = dr["VRSTA"] is DBNull ? null : (int?)Convert.ToInt32(dr["VRSTA"]),
                                        Naziv = dr["NAZIV"] is DBNull ? null : dr["NAZIV"].ToString(),
                                        NabCenaSaPor = dr["NABCENSAPOR"] is DBNull ? null : (double?)Convert.ToDouble(dr["NABCENSAPOR"]),
                                        FakturaNaCena = dr["FAKTURNACENA"] is DBNull ? null : (double?)Convert.ToDouble(dr["FAKTURNACENA"]),
                                        NabCenaBT = dr["NABCENABT"] is DBNull ? null : (double?)Convert.ToDouble(dr["NABCENABT"]),
                                        Troskovi = dr["TROSKOVI"] is DBNull ? null : (double?)Convert.ToDouble(dr["TROSKOVI"]),
                                        NabavnaCena = Convert.ToDouble(dr["NABAVNACENA"]),
                                        ProdCenaBP = Convert.ToDouble(dr["PRODCENABP"]),
                                        Korekcija = dr["KOREKCIJA"] is DBNull ? null : (double?)Convert.ToDouble(dr["KOREKCIJA"]),
                                        ProdajnaCena = Convert.ToDouble(dr["PRODAJNACENA"]),
                                        DeviznaCena = Convert.ToDouble(dr["DEVIZNACENA"]),
                                        DevProdCena = dr["DEVPRODCENA"] is DBNull ? null : (double?)Convert.ToDouble(dr["DEVPRODCENA"]),
                                        Kolicina = Convert.ToDouble(dr["KOLICINA"]),
                                        NivKol = Convert.ToDouble(dr["NIVKOL"]),
                                        TarifaID = dr["TARIFAID"].ToString(),
                                        ImaPorez = dr["IMAPOREZ"] is DBNull ? null : (int?)Convert.ToInt32(dr["IMAPOREZ"]),
                                        Porez = Convert.ToDouble(dr["POREZ"]),
                                        Rabat = Convert.ToDouble(dr["RABAT"]),
                                        Marza = Convert.ToDouble(dr["MARZA"]),
                                        Taksa = dr["TAKSA"] is DBNull ? null : (double?)Convert.ToDouble(dr["TAKSA"]),
                                        Akciza = dr["AKCIZA"] is DBNull ? null : (double?)Convert.ToDouble(dr["AKCIZA"]),
                                        ProsNab = Convert.ToDouble(dr["PROSNAB"]),
                                        PreCena = Convert.ToDouble(dr["PRECENA"]),
                                        PreNab = Convert.ToDouble(dr["PRENAB"]),
                                        ProsProd = Convert.ToDouble(dr["PROSPROD"]),
                                        MTID = dr["MTID"] is DBNull ? null : dr["MTID"].ToString(),
                                        PT = dr["PT"] is DBNull ? null : dr["PT"].ToString(),
                                        Zvezdica = dr["ZVEZDUCA"] is DBNull ? null : dr["ZVEZDICA"].ToString(),
                                        TrenStanje = Convert.ToDouble(dr["TRENSTANJE"]),
                                        PorezUlaz = Convert.ToDouble(dr["POREZ_ULAZ"]),
                                        SDatum = dr["SDATUM"] is DBNull ? null : (DateTime?)Convert.ToDateTime(dr["SDATUM"]),
                                        DevNabCena = dr["DEVNABCENA"] is DBNull ? null : (double?)Convert.ToDouble(dr["DEVNABCENA"]),
                                        PorezIz = Convert.ToDouble(dr["POREZ_IZ"]),
                                        X4 = dr["X4"] is DBNull ? null : (double?)Convert.ToDouble(dr["X4"]),
                                        Y4 = dr["Y4"] is DBNull ? null : (double?)Convert.ToDouble(dr["Y4"]),
                                        Z4 = dr["Z4"] is DBNull ? null : (double?)Convert.ToDouble(dr["Z4"]),
                                        CenaPOAJM = Convert.ToDouble(dr["CENAPOAJM"]),
                                        KGID = dr["KGID"] is DBNull ? null : (int?)Convert.ToInt32(dr["KGID"]),
                                        SAkciza = Convert.ToDouble(dr["SAKCIZA"])
                                    });
                        }
                    }
                }
                catch(Exception ex)
                {
                    Debug.Log(ex.ToString());
                }
                return list;
            }
        }
        public class Roba
        {
            public int ROBAID { get; set; }
            public string KatBr { get; set; }
            public string Naziv { get; set; }
            public string JM { get; set; }

            public static List<Roba> BufferedLista = new List<Roba>();
            public static List<Roba> List()
            {
                List<Roba> list = new List<Roba>();

                try
                {
                    using (FbConnection con = new FbConnection(Buffer.ConnectionStrings.Komercijalno))
                    {
                        con.Open();
                        using (FbCommand cmd = new FbCommand("SELECT ROBAID, KATBR, NAZIV, JM FROM ROBA", con))
                        {
                            FbDataReader dr = cmd.ExecuteReader();

                            while (dr.Read())
                                list.Add(new Roba() { ROBAID = Convert.ToInt32(dr[0]), KatBr = dr[1].ToString(), Naziv = dr[2].ToString(), JM = dr[3].ToString() });
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return null;
                }
                return list;
            }

            public static double GetPDV(int RobaID)
            {
                try
                {
                    using (FbConnection con = new FbConnection(Buffer.ConnectionStrings.Komercijalno))
                    {
                        con.Open();
                        using (FbCommand cmd = new FbCommand("SELECT STOPA FROM TARIFE WHERE TARIFAID = (SELECT TARIFAID FROM ROBA WHERE ROBAID = @R)", con))
                        {
                            cmd.Parameters.AddWithValue("@R", RobaID);

                            using (FbDataReader dr = cmd.ExecuteReader())
                                if (dr.Read())
                                    return Convert.ToInt32(dr[0]);
                        }
                    }
                    throw new Exception("Error getting PDV!");
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Preracunava nabavne cene po odredjenom algoritmu
            /// Ukoliko algoritam ne ispunjava uslove vraca poslednju nabavnu cenu
            /// </summary>
            /// <returns>Item1 = ROBAID, Item2 = NabavnaCena, Item3 = True ako je ispunjen algoritam, False ako je uzeta poslednja nabavna cena</returns>
            public static List<Tuple<int, double, bool>> GetNabavneCene()
            {

                List<Tuple<int, double, bool>> list = new List<Tuple<int, double, bool>>();

                try
                {
                    using (FbConnection con = new FbConnection(Buffer.ConnectionStrings.Komercijalno))
                    {
                        con.Open();
                        using (FbCommand cmd = new FbCommand("SELECT ROBAID, KATBR, NAZIV, JM FROM ROBA", con))
                        {
                            FbDataReader dr = cmd.ExecuteReader();

                            while (dr.Read())
                                list.Add(new Tuple<int, double, bool>(1, 1, true));
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return null;
                }
                return list;
            }

            public class Promet
            {
                /// <summary>
                /// Preracunava prodaju sve robe svih magacina za zadati period
                /// </summary>
                /// <param name="OdDatuma"></param>
                /// <param name="DoDatuma"></param>
                /// <returns>Zbir prodaje BEZ PDV</returns>
                public static double Prodaja(DateTime OdDatuma, DateTime DoDatuma)
                {
                    try
                    {
                        using (FbConnection con = new FbConnection(Buffer.ConnectionStrings.Komercijalno))
                        {
                            con.Open();
                            using (FbCommand cmd = new FbCommand(@"SELECT SUM(PRODCENABP * KOLICINA) FROM 
                                    STAVKA WHERE VRDOK = 15 OR VRDOK = 13", con))
                            {
                                FbDataReader dr = cmd.ExecuteReader();

                                if (dr.Read())
                                    return Convert.ToDouble(dr[0]);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Debug.Log(ex.ToString());
                    }
                    return 0;
                }
            }
        }
        public class Magacin
        {
            public int MagacinID { get; set; }
            public string Naziv { get; set; }

            public static List<Magacin> List()
            {
                List<Magacin> list = new List<Magacin>();

                using (FbConnection con = new FbConnection(Buffer.ConnectionStrings.Komercijalno))
                {
                    con.Open();
                    using (FbCommand cmd = new FbCommand("SELECT MAGACINID, NAZIV FROM MAGACIN", con))
                    {
                        FbDataReader dr = cmd.ExecuteReader();

                        while (dr.Read())
                            list.Add(new Magacin() { MagacinID = Convert.ToInt32(dr[0]), Naziv = dr[1].ToString() });
                    }
                }

                return list;
            }
        }
        public class Partner
        {
            public int PPID { get; set; }
            public string Naziv { get; set; }
            public string Adresa { get; set; }
            public string PIB { get; set; }

            public Partner()
            {

            }
            public Partner(int ID)
            {
                using (FbConnection con = new FbConnection(Buffer.ConnectionStrings.Komercijalno))
                {
                    con.Open();
                    using (FbCommand cmd = new FbCommand("SELECT PPID, NAZIV, ADRESA, PIB FROM PARTNER WHERE PPID = @P", con))
                    {
                        cmd.Parameters.AddWithValue("@P", ID);

                        using (FbDataReader dr = cmd.ExecuteReader())
                        {
                            if(dr.Read())
                            {
                                this.PPID = ID;
                                this.Naziv = dr[1].ToString();
                                this.Adresa = dr[2].ToString();
                                this.PIB = dr[3].ToString();
                            }
                        }
                    }
                }
            }
            public static string GetNaziv(int PPID)
            {
                try
                {
                    using (FbConnection con = new FbConnection(Buffer.ConnectionStrings.Komercijalno))
                    {
                        con.Open();
                        using (FbCommand cmd = new FbCommand("SELECT NAZIV FROM PARTNER WHERE PPID = @P", con))
                        {
                            cmd.Parameters.AddWithValue("@P", PPID);

                            FbDataReader dr = cmd.ExecuteReader();

                            if (dr.Read())
                                return (dr[0].ToString());
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                return null;
            }
            public static List<Partner> List()
            {
                List<Partner> list = new List<Partner>();
                using (FbConnection con = new FbConnection(Buffer.ConnectionStrings.Komercijalno))
                {
                    con.Open();
                    using (FbCommand cmd = new FbCommand("SELECT PPID, NAZIV, ADRESA, PIB FROM PARTNER", con))
                        using (FbDataReader dr = cmd.ExecuteReader())
                            while(dr.Read())
                                list.Add(new Partner() { PPID = Convert.ToInt32(dr[0]), Naziv = dr[1].ToString(), Adresa = dr[2].ToString(), PIB = dr[3].ToString() });
                }
                return list;
            }
        }
        public static void Initialize()
        {
            if (Initialized)
                throw new Exception("Parametri komercijalnog su vec inicijalizovano!");

            Roba.BufferedLista = Roba.List();

            Initialized = true;
        }
    }
    public class TDOffice
    {
        public class WebCena
        {
            public static string SQLSelect = @"SELECT WCID, ROBAID, ZADOKMAGID, KRITERIJUM, CENAIZMAGID, WEB_CENA_BP, PROCENAT, ROKPLACANJA, DATOD, DATDO, POCSTANJE, PLANIRANIRABAT, STANDARDNAVPCENA, POSLEDNJANABAVNACENA, PLANIRANAMARZA, NIV, REF_ROBAID, NAUPIT FROM WEB_CENA";

            #region Properties
            public int WCID { get; set; }
            public int RobaID { get; set; }
            public int? ZaDokMagID { get; set; }
            public int? Kriterijum { get; set; }
            public int? CenIzMagID { get; set; }
            public double? WebCenaBP { get; set; }
            public double? Procenat { get; set; }
            public  int? RokPlacanja { get; set; }
            public DateTime? DatOD { get; set; }
            public DateTime? DatDO { get; set; }
            public int? PocStanje { get; set; }
            public double? PlaniraniRabat { get; set; }
            public double? StandardnaVPCena { get; set; }
            public double? PoslednjaNabavnaCena { get; set; }
            public double? PlaniranaMarza { get; set; }
            public int? Niv { get; set; }
            public int? RefRobaID { get; set; }
            public int? NaUpit { get; set; }
            #endregion


            public static List<WebCena> List()
            {
                List<WebCena> list = new List<WebCena>();

                try
                {
                    using(FbConnection con = new FbConnection(Buffer.ConnectionStrings.TDOffice))
                    {
                        con.Open();
                        using (FbCommand cmd = new FbCommand(SQLSelect, con))
                            using (FbDataReader dr = cmd.ExecuteReader())
                                while (dr.Read())
                                    list.Add(new WebCena()
                                    {
                                        WCID = Convert.ToInt32(dr[0]),
                                        RobaID = Convert.ToInt32(dr[1]),
                                        ZaDokMagID = (dr[2] is DBNull) ? null : (int?)Convert.ToInt32(dr[2]),
                                        Kriterijum = (dr[3] is DBNull) ? null : (int?)Convert.ToInt32(dr[3]),
                                        CenIzMagID = (dr[4] is DBNull) ? null : (int?)Convert.ToInt32(dr[4]),
                                        WebCenaBP = (dr[5] is DBNull) ? null : (double?)Convert.ToDouble(dr[5]),
                                        Procenat = (dr[6] is DBNull) ? null : (double?)Convert.ToDouble(dr[6]),
                                        RokPlacanja = (dr[7] is DBNull) ? null : (int?)Convert.ToInt32(dr[7]),
                                        DatOD = (dr[8] is DBNull) ? null : (DateTime?)Convert.ToDateTime(dr[8]),
                                        DatDO = (dr[9] is DBNull) ? null : (DateTime?)Convert.ToDateTime(dr[9]),
                                        PocStanje = (dr[10] is DBNull) ? null : (int?)Convert.ToInt32(dr[10]),
                                        PlaniraniRabat = (dr[1] is DBNull) ? null : (double?)Convert.ToDouble(dr[11]),
                                        StandardnaVPCena = (dr[12] is DBNull) ? null : (double?)Convert.ToDouble(dr[12]),
                                        PoslednjaNabavnaCena = (dr[13] is DBNull) ? null : (double?)Convert.ToDouble(dr[13]),
                                        PlaniranaMarza = (dr[14] is DBNull) ? null : (double?)Convert.ToDouble(dr[14]),
                                        Niv = (dr[15] is DBNull) ? null : (int?)Convert.ToInt32(dr[15]),
                                        RefRobaID = (dr[16] is DBNull) ? null : (int?)Convert.ToInt32(dr[16]),
                                        NaUpit = (dr[17] is DBNull) ? null : (int?)Convert.ToInt32(dr[17])
                                    });
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                return list;
            }

            public static int MaxID()
            {
                try
                {

                    using (FbConnection con = new FbConnection(Buffer.ConnectionStrings.TDOffice))
                    {
                        con.Open();
                        using (FbCommand cmd = new FbCommand("SELECT MAX(WCID) FROM WEB_CENA", con))
                        {
                            FbDataReader dr = cmd.ExecuteReader();

                            if (dr.Read())
                                return Convert.ToInt32(dr[0]);
                            else
                                return 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                return 0;
            }
            public static bool Exist(int WCID)
            {
                try
                {
                    using (FbConnection con = new FbConnection(Buffer.ConnectionStrings.TDOffice))
                    {
                        con.Open();
                        using (FbCommand cmd = new FbCommand("SELECT WCID FROM WEB_CENA WHERE WCID = @ID", con))
                        {
                            cmd.Parameters.AddWithValue("@ID", WCID);

                            FbDataReader dr = cmd.ExecuteReader();

                            if (dr.Read())
                                return true;
                        }
                    }
                }
                catch(Exception ex)
                {
                    throw ex;
                }
                return false;
            }
        }
    }
}