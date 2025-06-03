using System;
using System.Collections.Generic;

namespace ArenaRPG2
{
    // Oyunculara özel yetenek tanımlamak için interface
    interface IOzelYetenek
    {
        void OzelSaldiri(Karakter hedef); // Sınıfın özel saldırı yapmasını zorunda bırakır
    }

    // Tüm karakterlerin ortak özelliklerini barındıran abstract sınıfı
    abstract class Karakter
    {
        public string Ad { get; set; } 

        // Alanlar encapsulation ile korunuyor
        private int _can;
        private int _guc;
        private int _mana;

        public int Can
        {
            get => _can;
            set => _can = value < 0 ? 0 : value;
        }
  
        public int Guc
        {
            get => _guc;
            protected set => _guc = value;
        }

        public int Mana
        {
            get => _mana;
            set => _mana = value;
        }

       
        public static int ToplamSaldiriSayisi = 0;

        // Constructor
        public Karakter(string ad, int can, int guc, int mana)
        {
            Ad = ad;
            Can = can;
            Guc = guc;
            Mana = mana;
        }
        public virtual void Saldir(Karakter hedef)
        {
            hedef.Can -= this.Guc; // Düşmanın canını azaltır
            ToplamSaldiriSayisi++; 
            Console.WriteLine($"{Ad}, {hedef.Ad} karakterine {Guc} hasar verdi!");
        }

        public void ManaYenile()
        {
            Mana += 10;
            Console.WriteLine($"{Ad} mana yeniledi. Yeni Mana: {Mana}");
        }
        public bool HayattaMi()
        {
            return Can > 0;
        }
    }
    class Oyuncu : Karakter, IOzelYetenek
    {
        // Oyuncu başlangıç değerleri atanır
        public Oyuncu(string ad) : base(ad, 100, 20, 30) { }

        public void OzelSaldiri(Karakter hedef)
        {
            if (Mana >= 15)
            {
                int hasar = Guc * 2;
                hedef.Can -= hasar;
                Mana -= 15;
                Console.WriteLine($"{Ad}, özel saldırı yaptı! {hedef.Ad} {hasar} hasar aldı.");
            }
            else
            {
                Console.WriteLine("Yetersiz mana! Özel saldırı yapılamadı.");
            }
        }
        public override void Saldir(Karakter hedef)
        {
            base.Saldir(hedef);
        }
    }
    class Dusman : Karakter
    {
        public Dusman(string ad, int can, int guc, int mana) : base(ad, can, guc, mana) { }
        public override void Saldir(Karakter hedef)
        {
            base.Saldir(hedef);
        }
    }
    class Program
    {
        static Random rnd = new Random(); // Rastgele düşman oluşturur
        static List<string> dusmanIsimleri = new List<string> { "Zombi", "Goblin", "Ejderha" };

        // Rastgele düşman üretir
        static Dusman YeniDusman()
        {
            string ad = dusmanIsimleri[rnd.Next(dusmanIsimleri.Count)];
            int can = 100;
            int guc = rnd.Next(10, 21);
            int mana = rnd.Next(10, 31);
            return new Dusman(ad, can, guc, mana);
        }

        //Oyun burada başlar
        static void Main(string[] args)
        {
            Console.Write("Karakter adınızı girin: ");
            string ad = Console.ReadLine();

            Oyuncu oyuncu = new Oyuncu(ad);
            Dusman dusman = YeniDusman();

            while (oyuncu.HayattaMi())
            {
                Console.WriteLine($"\n=== {oyuncu.Ad} vs {dusman.Ad} ===");
                Console.WriteLine($"{oyuncu.Ad} - Can: {oyuncu.Can}, Mana: {oyuncu.Mana}");
                Console.WriteLine($"{dusman.Ad} - Can: {dusman.Can}");

                Console.WriteLine("\n1. Saldır");
                Console.WriteLine("2. Özel Saldırı");
                Console.WriteLine("3. Mana Yenile");
                Console.Write("Seçiminiz: ");
                string secim = Console.ReadLine();

                switch (secim)
                {
                    case "1":
                        oyuncu.Saldir(dusman);
                        break;
                    case "2":
                        oyuncu.OzelSaldiri(dusman);
                        break;
                    case "3":
                        oyuncu.ManaYenile();
                        break;
                    default:
                        Console.WriteLine("Geçersiz seçim!");
                        continue;
                }

                // Düşman ölürse yeni düşman gelir
                if (!dusman.HayattaMi())
                {
                    Console.WriteLine($"\n{dusman.Ad} yok edildi! Yeni düşman geliyor...\n");
                    dusman = YeniDusman();
                    continue;
                }

                dusman.Saldir(oyuncu);

                // Oyuncu öldüyse oyun biter
                if (!oyuncu.HayattaMi())
                {
                    Console.WriteLine($"\n{oyuncu.Ad} öldü! Oyun sona erdi.");
                    Console.WriteLine($"Toplam saldırı sayısı: {Karakter.ToplamSaldiriSayisi}");
                    break;
                }
            }
            Console.WriteLine("\nOynamaya devam etmek için bir tuşa bas...");
            Console.ReadKey();
        }
    }
}
