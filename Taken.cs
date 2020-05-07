using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace EscapeFromTheWoods
{
    class Taken
    {
        private static int MonkeyRecordId = 0;
        private static int LogsId = 0;
        private readonly string ConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
        public async Task AllAsyncStuff(Bos bos)
        {
            await CreateWoodRecordsDataTable(bos);
            await CreateBitmapAndLogsFile(bos);
        }
        public async Task CreateWoodRecordsDataTable(Bos bos)
        {
            DataTable WoodRecords = new DataTable();
            WoodRecords.Clear();
            WoodRecords.Columns.Add(new DataColumn("recordId", typeof(int)));
            WoodRecords.Columns.Add(new DataColumn("woodID", typeof(int)));
            WoodRecords.Columns.Add(new DataColumn("treeID", typeof(int)));
            WoodRecords.Columns.Add(new DataColumn("x", typeof(int)));
            WoodRecords.Columns.Add(new DataColumn("y", typeof(int)));
            Console.WriteLine($"write wood {bos.Id} to database -start");
            foreach (Boom boom in bos.Bomen.Values)
            {
                DataRow dr = WoodRecords.NewRow();
                dr[0] = boom.woodRecordId;
                dr[1] = bos.Id;
                dr[2] = boom.Id;
                dr[3] = boom.X;
                dr[4] = boom.Y;
                WoodRecords.Rows.Add(dr);
            }
            await WriteWoodRecordsToDatabase(bos, WoodRecords);
        }
        public async Task WriteWoodRecordsToDatabase(Bos bos, DataTable WoodRecords)
        {
            using (SqlConnection connection = new SqlConnection(ConnString))
            {
                connection.Open();// Transaction not allowed!
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "WoodRecords";
                    bulkCopy.ColumnMappings.Clear();
                    bulkCopy.ColumnMappings.Add("recordId", "recordId");
                    bulkCopy.ColumnMappings.Add("woodID", "woodID");
                    bulkCopy.ColumnMappings.Add("treeID", "treeID");
                    bulkCopy.ColumnMappings.Add("x", "x");
                    bulkCopy.ColumnMappings.Add("y", "y");
                    bulkCopy.WriteToServer(WoodRecords);
                }
            }
            Console.WriteLine($"write wood {bos.Id} to database -end");
            await StartCalculatingEscapeRoutes(bos);
        }
        public async Task StartCalculatingEscapeRoutes(Bos bos)
        {
            foreach(Aap aap in bos.Apen.Values)
            {
                await OntsnappingsRoutePerAap(aap);
            }
        }
        public async Task OntsnappingsRoutePerAap(Aap aap)
        {
            Console.WriteLine($"Start calculating escape route for wood: {aap.Bos.Id}, monkey: {aap.Naam}");
            while (aap.Ontsnapt != true)
            {
                aap.Sprong();
            }
            await CreateRoutesDatatables(aap);
            Console.WriteLine($"End calculating escape route for wood: {aap.Bos.Id}, monkey: {aap.Naam}");
        }
        public async Task CreateRoutesDatatables(Aap aap)
        {
            Console.WriteLine($"write route to database wood: {aap.Bos.Id}, monkey: {aap.Naam} -start");
            DataTable MonkeyRecords = new DataTable();
            MonkeyRecords.Clear();
            DataTable Logs = new DataTable();
            Logs.Clear();
            MonkeyRecords.Columns.Add(new DataColumn("recordID", typeof(int)));
            MonkeyRecords.Columns.Add(new DataColumn("monkeyID", typeof(int)));
            MonkeyRecords.Columns.Add(new DataColumn("monkeyName", typeof(string)));
            MonkeyRecords.Columns.Add(new DataColumn("woodID", typeof(int)));
            MonkeyRecords.Columns.Add(new DataColumn("seqnr", typeof(int)));
            MonkeyRecords.Columns.Add(new DataColumn("treeID", typeof(int)));
            MonkeyRecords.Columns.Add(new DataColumn("x", typeof(int)));
            MonkeyRecords.Columns.Add(new DataColumn("y", typeof(int)));
            Logs.Columns.Add(new DataColumn("Id", typeof(int)));
            Logs.Columns.Add(new DataColumn("woodID", typeof(int)));
            Logs.Columns.Add(new DataColumn("monkeyID", typeof(int)));
            Logs.Columns.Add(new DataColumn("message", typeof(string)));
            int seqnr = 1;
            foreach (Boom boom in aap.BezochteBomen)
            {
                DataRow dr = MonkeyRecords.NewRow();
                MonkeyRecordId++;
                dr[0] = MonkeyRecordId;
                dr[1] = aap.Id;
                dr[2] = aap.Naam;
                dr[3] = boom.Bos.Id;
                dr[4] = seqnr;
                dr[5] = boom.Id;
                dr[6] = boom.X;
                dr[7] = boom.Y;
                MonkeyRecords.Rows.Add(dr);
                seqnr++;
            }
            foreach (string logstring in aap.LogsPerAap)
            {
                DataRow dr = Logs.NewRow();
                LogsId++;
                dr[0] = LogsId;
                dr[1] = aap.Bos.Id;
                dr[2] = aap.Id;
                dr[3] = logstring;
            }
            await WriteRouteToDatabase(aap, MonkeyRecords, Logs);
        }
        public async Task WriteRouteToDatabase(Aap aap, DataTable monkeyrecords, DataTable logs)
        {
            using (SqlConnection connection = new SqlConnection(ConnString))
            {
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "Logs";
                    bulkCopy.ColumnMappings.Add("Id", "Id");
                    bulkCopy.ColumnMappings.Add("woodID", "woodID");
                    bulkCopy.ColumnMappings.Add("monkeyID", "monkeyID");
                    bulkCopy.ColumnMappings.Add("message", "message");
                    bulkCopy.WriteToServer(logs);
                    bulkCopy.DestinationTableName = "MonkeyRecords";
                    bulkCopy.ColumnMappings.Clear();
                    bulkCopy.ColumnMappings.Add("recordID", "recordID");
                    bulkCopy.ColumnMappings.Add("monkeyID", "monkeyID");
                    bulkCopy.ColumnMappings.Add("monkeyName", "monkeyName");
                    bulkCopy.ColumnMappings.Add("woodID", "woodID");
                    bulkCopy.ColumnMappings.Add("seqnr", "seqnr");
                    bulkCopy.ColumnMappings.Add("treeID", "treeID");
                    bulkCopy.ColumnMappings.Add("x", "x");
                    bulkCopy.ColumnMappings.Add("y", "y");
                    bulkCopy.WriteToServer(monkeyrecords);
                }
            }
            Console.WriteLine($"write route to database wood: {aap.Bos.Id}, monkey: {aap.Naam} -end");
        }
        public async Task CreateBitmapAndLogsFile(Bos bos)
        {
            string pth = @"D:\HoGent Progammeren\Programmeren 4\AsynchroonProgrammeren Oefening\EscapeFromTheWoods\bin\Debug\netcoreapp3.1\Bitmap\";
            double drawingFactor = 1;
            int ellipseStraal = 20;
            using (Bitmap bm = new Bitmap(Convert.ToInt32(bos.Xmaximum * drawingFactor), Convert.ToInt32(Math.Abs(bos.Yminimum) * drawingFactor)))
            using (Graphics g = Graphics.FromImage(bm))
            using (Pen p = new Pen(Color.Green, 1))
            {
                Console.WriteLine($"write bitmap routes wood: {bos.Id} -start");
                Array colorsArray = Enum.GetValues(typeof(KnownColor));
                KnownColor[] allColors = new KnownColor[colorsArray.Length];
                Array.Copy(colorsArray, allColors, colorsArray.Length);
                //bovenstaande om een nieuwe kleur bij elke aap in te stellen;
                //BOS MET BOMEN:
                foreach (Boom b in bos.Bomen.Values)
                {
                    g.DrawEllipse(p, b.X - 10, Math.Abs(b.Y) - 10, ellipseStraal, ellipseStraal);
                }
                //Bos met bomen aangemaakt.
                //STARTPOSITIE AAP & ROUTE AAP:
                int aapCount = 35;
                foreach (Aap aap in bos.Apen.Values)
                {
                    Brush brush = new SolidBrush(Color.FromName(allColors[aapCount].ToString()));
                    Pen penPerAap = new Pen(Color.FromName(allColors[aapCount].ToString()));
                    aapCount++;
                    g.FillEllipse(brush, (aap.StartBoom.X - 10), Math.Abs(aap.StartBoom.Y) - 10, ellipseStraal, ellipseStraal);
                    for (int i = 0; i < aap.BezochteBomen.Count - 1; i++)
                    {
                        g.DrawLine(penPerAap, aap.BezochteBomen[i].X, Math.Abs(aap.BezochteBomen[i].Y), aap.BezochteBomen[i + 1].X, Math.Abs(aap.BezochteBomen[i + 1].Y));
                    }
                    brush.Dispose();
                }
                bm.Save(Path.Combine(pth, $@"{bos.Id.ToString()}_escapeRoutes.jpg"), ImageFormat.Jpeg);
                Console.WriteLine($"write bitmap routes wood: {bos.Id} -end");
                await GenerateLogs(bos);
            }
        }
        public async Task GenerateLogs(Bos bos)
        {
            Console.WriteLine($"wood: {bos.Id} writes log -start");
            int langsteLogStringList = 0;
            string fileName = $@"{bos.Id}_log.txt";
            List<string> logsPerBos = new List<string>();
            foreach (Aap aap in bos.Apen.Values)
            {
                if (aap.LogsPerAap.Count > langsteLogStringList)
                {
                    langsteLogStringList = aap.LogsPerAap.Count;
                }
            }
            //Nu hebben we hebben we het meeste aantal sprongen voor een aap uit het bos.
            for (int i = 0; i < langsteLogStringList; i++)
            {
                foreach (Aap aap in bos.Apen.Values)
                {
                    if (i < aap.LogsPerAap.Count)
                    {
                        logsPerBos.Add(aap.LogsPerAap[i]);
                    }
                }
            }
            //Nu hebben we de List met strings in juiste volgorde om de log file mee te maken.
            System.IO.File.WriteAllLines(fileName, logsPerBos);
            logsPerBos.Clear();
            Console.WriteLine($"wood: {bos.Id} writes log -end");
        }
    }
}
