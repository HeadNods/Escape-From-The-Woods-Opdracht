using System;
using System.Collections.Generic;
using System.Linq;

namespace EscapeFromTheWoods
{
    public class Aap
    {
        private static int aapId = 1;
        public Aap(string naam, Bos bos)
        {
            this.Bos = bos;
            this.Id = aapId;
            this.Naam = naam;
            aapId++;
        }
        public List<string> LogsPerAap { get; set; } = new List<string>();
        public int Id { get; set; }
        public string Naam { get; set; }
        public Boom StartBoom { get; set; }
        public Boom HuidigeBoom { get; set; }
        public Bos Bos { get; set; }
        public List<Boom> BezochteBomen { get; set; } = new List<Boom>();
        public bool Ontsnapt { get; set; } = false;
        public void StartBoomInstellen()
        {
            foreach (Boom b in Bos.Bomen.Values)
            {
                if (this.StartBoom == null)
                {
                    if (b.Bezet == false)
                    {
                        this.StartBoom = b;
                        this.HuidigeBoom = b;
                        b.Bezet = true;
                        break;
                    }
                }
            }
            this.LogsPerAap.Add($"{this.Naam} is in tree {this.HuidigeBoom.Id} at ({this.HuidigeBoom.X},{this.HuidigeBoom.Y})");
            this.BezochteBomen.Add(this.StartBoom);
        }
        public void Sprong()
        {
            double minimumAfstand = 10000;
            double minimumAfstandSafe;
            double afstandTotRand = (new List<double>() { this.Bos.Ymaximum - this.HuidigeBoom.Y, this.Bos.Xmaximum - this.HuidigeBoom.X, this.HuidigeBoom.Y - this.Bos.Yminimum, this.HuidigeBoom.X - this.Bos.Xminimum }).Min();

            int dichtsteBoomId = this.HuidigeBoom.Id;
            foreach (KeyValuePair<int, Boom> b in Bos.Bomen)
            {
                if (!this.BezochteBomen.Contains(b.Value))
                {
                    minimumAfstandSafe = Math.Sqrt(Math.Pow(this.HuidigeBoom.X - b.Value.X, 2) + Math.Pow(this.HuidigeBoom.Y - b.Value.Y, 2));
                    if (minimumAfstandSafe < minimumAfstand)
                    {
                        minimumAfstand = minimumAfstandSafe;
                        dichtsteBoomId = b.Key;
                    }
                }
            }
            if (minimumAfstand < afstandTotRand)
            {
                this.HuidigeBoom = this.Bos.Bomen[dichtsteBoomId];
                this.BezochteBomen.Add(this.HuidigeBoom);
                this.LogsPerAap.Add($"{this.Naam} is in tree {this.HuidigeBoom.Id} at ({this.HuidigeBoom.X},{this.HuidigeBoom.Y})");
            }
            else { this.Ontsnapt = true; HuidigeBoom = null; }
        }
    }
}
