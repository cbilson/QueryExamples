using System;

namespace QueryExamples {
    class Program {

        static void Main(string[] args) {

            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();

            try {
                new ForMatt.Example1().Run();
            } catch (Exception ex) {
                Console.Error.WriteLine(ex.ToString());
            }

            Console.WriteLine("Hit any key to exit");
            Console.ReadKey();
        }
    }

}
