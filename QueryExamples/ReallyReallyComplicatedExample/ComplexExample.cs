using System;
using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Criterion;
using FizzWare.NBuilder;

namespace QueryExamples.ReallyReallyComplicatedExample
{
    public class ComplexExample
    {
        private ISession session;

        public void Run()
        {
            Helpers.CreateDatabase(this.GetType().Namespace);

            session = Helpers.OpenSession(SetupMappings);

            ClearOutDatabase();

            AddSomeTestData();

        }

        /// <summary>
        /// Value the portfolio at previous day's close
        /// </summary>
        private PortfolioValuation ValueAtPreviousClose(Account account)
        {
            throw new NotImplementedException();
        }

        private void ClearOutDatabase()
        {
            session.CreateSQLQuery("delete from Price").ExecuteUpdate();
            session.CreateSQLQuery("delete from Position").ExecuteUpdate();
            session.CreateSQLQuery("delete from Security").ExecuteUpdate();
            session.CreateSQLQuery("delete from HoldingsReport").ExecuteUpdate();
            session.CreateSQLQuery("delete from Account").ExecuteUpdate();
        }

        private void AddSomeTestData()
        {
          var rnd = new Random();

          Console.WriteLine("Creating 100 accounts...");

          var accounts = Builder<Account>.CreateListOfSize(100)
            .WhereAll().Have(x => x.ID = 0)
            .HaveDoneToThem(x => session.Save(x))
            .Build();

          Console.WriteLine("Creating 1000 securities...");

          var securities = Builder<Security>.CreateListOfSize(1000)
            .WhereAll().Have(x => x.ID = 0)
            .HaveDoneToThem(x => session.Save(x))
            .Build();

          Console.WriteLine("Creating 30 holdings reports for each account, with between 40 and 400 positions in each...");

          accounts.Each(acct => {
              var holdings = Builder<HoldingsReport>.CreateListOfSize(30)
              .WhereAll().Have(x => x.ID = 0)
              .HaveDoneToThem(x => x.Account = acct)
              .HaveDoneToThem(report => {
                  var numberOfPositions = rnd.Next(360) + 40;
                  var positions = Builder<Position>.CreateListOfSize(numberOfPositions)
                  .WhereAll().Have(x => x.ID = 0)
                  .And(x => x.Security = securities[rnd.Next(1000)])
                  .And(x => x.HoldingsReport = report)
                  .And(x => report.Positions.Add(x))
                  .Build();
                })
              .HaveDoneToThem(x => Console.WriteLine("Saving holdings for {0} on {1}...", acct.AccountNumber, x.EffectiveDate))
              .HaveDoneToThem(x => session.Save(x))
              .Build();
            });

          accounts.Each(x => Console.WriteLine("{0}, {1}: {2}",
                                               x.ID,
                                               x.AccountNumber,
                                               x.Name));
        }

        private static void SetupMappings(MappingConfiguration cfg)
        {
            cfg.FluentMappings.AddFromAssemblyOf<AccountMap>();
        }
    }

    internal class PortfolioValuation
    {
    }
}
