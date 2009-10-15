using System;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Processors;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace QueryExamples {
    public static class Helpers {

        public static void CreateDatabase(string migrationsNamespace) {

            Console.WriteLine("Updating database...");

            var processorFactory = ProcessorFactory.GetFactory("Sqlite");
            var processor = processorFactory.Create("Data Source=Data.db");

            var conventions = new MigrationConventions();
            var runner = new MigrationVersionRunner(conventions, processor,
                new MigrationLoader(conventions), typeof(Helpers).Assembly,
                migrationsNamespace);

            runner.MigrateUp();
        }

        public static ISession OpenSession(Action<MappingConfiguration> mappings) {

            Console.WriteLine("Opening session");

            return Fluently.Configure()
                .Database(SQLiteConfiguration.Standard.UsingFile("Data.db"))
                .Mappings(mappings)
                .BuildSessionFactory()
                .OpenSession();
        }
    }
}
