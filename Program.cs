using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EscapeFromTheWoods
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Bos bos1 = new Bos(1000, -1000);
            bos1.CreateBos(50, 5);
            Bos bos2 = new Bos(500, -500);
            bos2.CreateBos(30, 3);
            Bos bos3 = new Bos(1000, -1000);
            bos3.CreateBos(40, 4);
            DatabankTools databankTools = new DatabankTools();
            databankTools.ClearDatabase();
            //ASYNC TESTCODE:
            Taken taken = new Taken();
            List<Task> tasks = new List<Task>();
            tasks.Add(Task.Run(() => taken.AllAsyncStuff(bos1)));
            tasks.Add(Task.Run(() => taken.AllAsyncStuff(bos2)));
            //Task.WaitAll(tasks[1]); 
            //Kan gebruikt worden stel dat we het anders async willen laten werken.
            tasks.Add(Task.Run(() => taken.AllAsyncStuff(bos3)));
            Task.WaitAll(tasks.ToArray());
            stopwatch.Stop();
            Console.WriteLine();
            Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
            Console.WriteLine("______________________________________________");
            Console.ReadKey();
        }
    }
}
