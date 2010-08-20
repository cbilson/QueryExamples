using System;
using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Criterion;

namespace QueryExamples.ReallyReallyComplicatedExample
{
    public class ComplexExample
    {
        private ISession session;

        public void Run()
        {
            Helpers.CreateDatabase(GetType().Namespace);

            session = Helpers.OpenSession(SetupMappings);

            //RecreateData();

            var account = GetAccount("AccountNumber7");

            var valuation = ValueAtPreviousClose(account);
        }

        private Account GetAccount(string accountnumber)
        {
            return session.CreateCriteria<Account>()
                .Add(Restrictions.Eq("AccountNumber", accountnumber))
                .UniqueResult<Account>();
        }

        private void RecreateData()
        {
            using (var transaction = session.BeginTransaction())
            {
                ClearOutDatabase();

                AddSomeTestData();

                transaction.Commit();
            }
        }

        /// <summary>
        /// Value the portfolio at previous day's close
        /// </summary>
        private PortfolioValuation ValueAtPreviousClose(Account account)
        {
            var reportEffectiveDate = DetachedCriteria.For<HoldingsReport>()
                .Add(Restrictions.Eq("Account", account))
                .SetProjection(Projections.Max("EffectiveDate"));

            var report = DetachedCriteria.For<HoldingsReport>()
                .Add(Subqueries.PropertyEq("EffectiveDate", reportEffectiveDate))
                .SetProjection(Projections.Id());

            var positions = session.CreateCriteria<Position>()
                .Add(Subqueries.PropertyEq("HoldingsReport", report))
                .List<Position>();

            var lastPrices = DetachedCriteria.For<PriceQuote>("lp")
                .Add(Restrictions.EqProperty("p.Security", "lp.Security"))
                .SetProjection(Projections.Max("EffectiveDate"));

            var prices = session.CreateCriteria<PriceQuote>("p")
                .Add(Subqueries.PropertyEq("p.EffectiveDate", lastPrices))
                .Add(Restrictions.In("Security", positions.Select(x => x.Security).ToArray()))
                .List<PriceQuote>();

            var value = positions.Select(x => new
                                      {
                                          Position = x,
                                          Price = prices.FirstOrDefault(p => p.Security == x.Security) as UnitPriceQuote
                                      })
                .Sum(x => x.Position.Quantity*x.Price.Price.Amount)
                .In("USD");

            return new PortfolioValuation {Account = account, MarketValue = value };
        }

        private void ClearOutDatabase()
        {
            session.CreateSQLQuery("delete from PriceQuote").ExecuteUpdate();
            session.CreateSQLQuery("delete from Position").ExecuteUpdate();
            session.CreateSQLQuery("delete from Security").ExecuteUpdate();
            session.CreateSQLQuery("delete from HoldingsReport").ExecuteUpdate();
            session.CreateSQLQuery("delete from Account").ExecuteUpdate();
        }

        private void AddSomeTestData()
        {
            var rnd = new Random();
            const int numberOfAccounts = 10;
            const int numberOfSecurities = 100;

            var accounts = CreateAccounts(numberOfAccounts);

            var securities = CreateSecurities(numberOfSecurities);

            CreatePrices();

            CreateHoldings(numberOfSecurities, accounts, rnd, securities);
        }

        private void CreateHoldings(int numberOfSecurities, IList<Account> accounts, Random rnd, IList<Security> securities)
        {
            Console.WriteLine("Creating 30 holdings reports for each account, with between 40 and 400 positions in each...");

            accounts.Each(acct => CreateAccount(acct, rnd, securities, numberOfSecurities));
        }

        private void CreateAccount(Account acct, Random rnd, IList<Security> securities, int numberOfSecurities)
        {
            var holdings = CreateHoldingsReports(acct, rnd, securities, numberOfSecurities);

            holdings.Each(report =>
                              {
                                  Console.WriteLine("Saving holdings for {0} on {1}...",
                                                    acct.AccountNumber,
                                                    report.EffectiveDate);
                                  session.Save(report);
                              });
        }

        private IList<HoldingsReport> CreateHoldingsReports(Account acct, Random rnd, IList<Security> securities, int numberOfSecurities)
        {
            return Builder<HoldingsReport>.CreateListOfSize(30)
                .WhereAll().Have(x => x.ID = 0)
                .HaveDoneToThem(x => x.Account = acct)
                .HaveDoneToThem(report =>
                                    {
                                        var numberOfPositions = rnd.Next(360) + 40;
                                        var positions =
                                            Builder<Position>.CreateListOfSize(numberOfPositions)
                                                .WhereAll().Have(x => x.ID = 0)
                                                .And(
                                                    x =>
                                                    x.Security =
                                                    securities[rnd.Next(numberOfSecurities)])
                                                .And(x => x.HoldingsReport = report)
                                                .And(x => report.Positions.Add(x))
                                                .Build();
                                    })
                .Build();
        }

        private IList<Security> CreateSecurities(int numberOfSecurities)
        {
            Console.WriteLine("Creating {0} securities...", numberOfSecurities);
            return Builder<Security>.CreateListOfSize(numberOfSecurities)
                .WhereAll().Have(x => x.ID = 0)
                .HaveDoneToThem(x => session.Save(x))
                .Build();
        }

        private IList<Account> CreateAccounts(int numberOfAccounts)
        {
            Console.WriteLine("Creating {0} accounts...", numberOfAccounts);
            return Builder<Account>.CreateListOfSize(numberOfAccounts)
                .WhereAll().Have(x => x.ID = 0)
                .HaveDoneToThem(x => session.Save(x))
                .Build();
        }

        private void CreatePrices()
        {
            var securities = session.CreateCriteria<Security>().List<Security>();
            var rnd = new Random();

            foreach (var security in securities)
            {
                foreach (var date in Dates.From(Clock.Today).To(1.MonthsFromToday()))
                    if (rnd.NextDouble() > 0.05)
                    {
                        session.Save(new UnitClosePrice()
                        {
                            EffectiveDate = date,
                            Security = security,
                            Price = rnd.NextDouble().In("USD")
                        });
                    }
            }
        }

        private static void SetupMappings(MappingConfiguration cfg)
        {
            cfg.FluentMappings.AddFromAssemblyOf<AccountMap>();
        }
    }

    public class PortfolioValuation
    {
        public Money MarketValue { get; set; }
        public Account Account { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}