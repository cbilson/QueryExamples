using System;
using QueryExamples.ReallyReallyComplicatedExample;

namespace QueryExamples {
    class Program {

        static void Main(string[] args) {

            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();

            try {
          //      new ForMatt.Example1().Run();
                new ComplexExample().Run();
            } catch (Exception ex) {
                Console.Error.WriteLine(ex.ToString());
            }

            Console.WriteLine("Hit any key to exit");
            Console.ReadKey();
        }
    }

}
