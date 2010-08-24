using System;
using System.Collections.Generic;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Processors;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace QueryExamples {
    public static class Helpers {

      public static void Each<T>(this IEnumerable<T> xs, Action<T> f) {
        foreach (var x in xs)
          f(x);
      }

        public static void CreateDatabase(string migrationsNamespace) {

            Console.WriteLine("Updating database...");

            var runner = GetRunner(migrationsNamespace);

            runner.MigrateUp();
        }

        private static MigrationVersionRunner GetRunner(string migrationsNamespace)
        {
            var conventions = new MigrationConventions();

            var announcer = new BaseAnnouncer(Console.WriteLine);

            var processor = GetProcessor(announcer);

            return new MigrationVersionRunner(conventions, processor,
                                              new MigrationLoader(conventions), 
                                              typeof(Helpers).Assembly,
                                              migrationsNamespace, announcer, "");
        }

        private static IMigrationProcessor GetProcessor(BaseAnnouncer announcer)
        {
            var processorFactory = ProcessorFactory.GetFactory("Sqlite");

            var options = new ProcessorOptions { };
            return processorFactory.Create("Data Source=Data.db", announcer, options);
        }

        public static ISession OpenSession(Action<MappingConfiguration> mappings) {

            Console.WriteLine("Opening session");

            return Fluently.Configure()
                .Database(SQLiteConfiguration.Standard.UsingFile("Data.db").AdoNetBatchSize(500))
                .Mappings(mappings)
                .BuildSessionFactory()
                .OpenSession();
        }
    }
}
