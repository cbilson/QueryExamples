using System;
using System.Collections.Generic;
using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Criterion;
using Ris.Common;

namespace QueryExamples.ForMatt {
    class Example1 {
        private ISession session;

        public void Run() {
            Helpers.CreateDatabase(this.GetType().Namespace);

            session = Helpers.OpenSession(SetupMappings);

            ClearOutDatabase();

            AddSomeTestData();

            var results = FindLatestPriceForEachSecurity_Criteria_Style();

            results.Each(x => Console.WriteLine("{0} @ {1:n2} on {2:D}",
                x.Sedol, x.NavPrice, x.ValuationDate));
        }

        /// <summary>
        /// when in doubt, the easiest thing to do, if you've already got a 
        /// SQL query that works, is to just use it
        /// </summary>
        private IList<UnitPrice> FindLatestPriceForEachSecurity_Raw_SQL_Style() {
            return session.CreateSQLQuery(
@"
SELECT A.PriceID, A.ValuationDate, A.Sedol, A.NavPrice
FROM UnitPrices A
INNER JOIN (
    SELECT Sedol, MAX(ValuationDate) AS ValuationDate
    FROM UnitPrices
GROUP BY Sedol) B
ON A.Sedol = B.Sedol AND A.ValuationDate = B.ValuationDate
")
                .AddEntity(typeof(UnitPrice))
                .List<UnitPrice>();
        }

        /// <summary>
        /// this is a criteria query that works like the above SQL
        /// </summary>
        private IList<UnitPrice> FindLatestPriceForEachSecurity_Criteria_Style() {

            // this looks a little different than the SQL - basically, we have
            // to move the sedol matching inside the sub-select, since we can
            // only project one property to use from a subquery
            var latestForSedol = DetachedCriteria.For<UnitPrice>("B")    // FROM UnitPrices B
                .SetProjection(Projections.Max("ValuationDate"))         
                .Add(Restrictions.EqProperty("B.Sedol", "A.Sedol"));

            session.CreateSQLQuery("drop table UnitPrices").ExecuteUpdate();


            return session.CreateCriteria<UnitPrice>("A")
                .Add(Subqueries.PropertyEq("ValuationDate", latestForSedol))
                .List<UnitPrice>();

            /*  The SQL looks like this:
             * 
             *  SELECT  
             *            this_.PriceID as PriceID0_0_
             *          , this_.ValuationDate as Valuatio2_0_0_
             *          , this_.Sedol as Sedol0_0_
             *          , this_.NavPrice as NavPrice0_0_ 
             *  FROM    
             *          UnitPrices this_ 
             *  WHERE 
             *          this_.ValuationDate = (
             *              SELECT max(this_0_.ValuationDate) as y0_ 
             *              FROM UnitPrices this_0_ 
             *              WHERE this_0_.Sedol = this_.Sedol)
             */

        }

        private void ClearOutDatabase() {
            session.CreateSQLQuery("delete from UnitPrices").ExecuteUpdate();
        }

        private void AddSomeTestData() {
            session.Save(new UnitPrice { Sedol = "11111", NavPrice = 20M, ValuationDate = new DateTime(2009, 10, 1) });
            session.Save(new UnitPrice { Sedol = "11111", NavPrice = 21M, ValuationDate = new DateTime(2009, 10, 4) });
            session.Save(new UnitPrice { Sedol = "11111", NavPrice = 19M, ValuationDate = new DateTime(2009, 10, 5) });

            session.Save(new UnitPrice { Sedol = "22222", NavPrice = 40M, ValuationDate = new DateTime(2009, 10, 1) });
            session.Save(new UnitPrice { Sedol = "22222", NavPrice = 39M, ValuationDate = new DateTime(2009, 10, 3) });

            session.Save(new UnitPrice { Sedol = "33333", NavPrice = 20M, ValuationDate = new DateTime(2009, 9, 30) });
            session.Save(new UnitPrice { Sedol = "33333", NavPrice = 21.5M, ValuationDate = new DateTime(2009, 10, 2) });
        }

        private static void SetupMappings(MappingConfiguration cfg) {
            cfg.FluentMappings.Add(typeof(UnitPriceMap));
        }
    }
}