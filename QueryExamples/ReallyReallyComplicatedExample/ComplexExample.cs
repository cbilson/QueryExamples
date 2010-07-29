using System;
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
