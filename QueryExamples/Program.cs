using System;

namespace QueryExamples {
    class Program {

        static void Main(string[] args) {
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
