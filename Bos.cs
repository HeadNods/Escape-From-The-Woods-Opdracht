using System;
using System.Collections.Generic;

namespace EscapeFromTheWoods
{
    public class Bos
    {
        public static int WoodRecordId = 1;
        public static int MonkeyRecordId = 1;

        private static int idTeller = 0;
        public int Id { get; set; }
        public Bos(int xMax, int yMin)
        {
            this.Id = idTeller;
            idTeller++;
            this.Xmaximum = xMax;
            this.Yminimum = yMin;
        }
        //Aangezien we links in de bovenhoek moeten beginnen zet ik al 2 properties vast
        public int Xminimum { get; } = 0;
        public int Ymaximum { get; } = 0;
        public int Xmaximum { get; set; }
        public int Yminimum { get; set; }
        public Dictionary<int, Boom> Bomen = new Dictionary<int, Boom>(); //Kan perfect met Id van boom werken voor de key
        public Dictionary<int, Aap> Apen = new Dictionary<int, Aap>();
        public Dictionary<string, Boom> BestaandeBoomPosities = new Dictionary<string, Boom>();
        public void CreateBos(int aantalBomen, int aantalApen)
        {
            int maxAantalBomen = (Math.Abs(this.Yminimum) - 2) * (this.Xmaximum - 2);
            //-1 zodat de border niet meegerekent wordt.
            //deze if zodat het enkel aangemaakt wordt als dit mogelijk is.
            if (aantalBomen < maxAantalBomen && aantalBomen > aantalApen)
            {
                for (int i = 0; i < aantalBomen; i++)
                {
                    this.Bomen.Add(i, new Boom(this, i+1, WoodRecordId));
                    WoodRecordId++;
                    this.Bomen.TryGetValue(i, out Boom boom);
                    boom.BoomXenYWaardeGenereren();
                    //op deze manier kunnen alle bomen voor het bos gegenereert worden.
                }
                for (int i = 0; i < aantalApen; i++)
                {
                    int lengte = i;
                    if(lengte < 2) { lengte = 4;}
                    if(lengte > 10) { lengte = 5; }
                    this.Apen.Add(i, new Aap(GenerateName(lengte), this));
                    this.Apen.TryGetValue(i, out Aap aap);
                    aap.StartBoomInstellen();
                    //op deze manier alle apen genereren met een random naam en in een startboom.
                }
            }
            else
            {
                Console.WriteLine("Het bos kan of niet zo'n hoog aantal bomen bevatten of heeft meer apen dan bomen.");
            }
        }
        //Voor het creëren van de namen voor alle apen:
        public static string GenerateName(int len)
        {
            Random r = new Random();
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
            string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
            string Name = "";
            Name += consonants[r.Next(consonants.Length)].ToUpper();
            Name += vowels[r.Next(vowels.Length)];
            int b = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
            while (b < len)
            {
                Name += consonants[r.Next(consonants.Length)];
                b++;
                Name += vowels[r.Next(vowels.Length)];
                b++;
            }
            return Name;
        }
    }
}
