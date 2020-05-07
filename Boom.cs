using System;

namespace EscapeFromTheWoods
{
    public class Boom
    {
        public Boom(Bos bos, int id, int woodrecordId)
        {
            this.Id = id;
            this.Bos = bos;
            this.woodRecordId = woodrecordId;
        }
        public int Id { get; set; }
        public int woodRecordId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public Bos Bos { get; set; }
        public bool Bezet { get; set; } = false;
        public void BoomXenYWaardeGenereren()
        {
            int xmin = Bos.Xminimum;
            int xmax = Bos.Xmaximum;
            int ymin = Bos.Yminimum;
            int ymax = Bos.Ymaximum;
            Random rnd = new Random();
            int x = rnd.Next(xmin, xmax);
            int y = rnd.Next(ymin, ymax);
            string key = String.Concat(x, " ", y);
            while (Bos.BestaandeBoomPosities.ContainsKey(key))
            {
                x = rnd.Next(xmin, xmax);
                y = rnd.Next(ymin, ymax);
                key = String.Concat(x, " ", y);
            }
            Bos.BestaandeBoomPosities.Add(key, this);
            this.X = x;
            this.Y = y;
        }
    }
}
