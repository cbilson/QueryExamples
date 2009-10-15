using System;
using FluentNHibernate.Mapping;

namespace QueryExamples.ForMatt {

    public class UnitPriceMap : ClassMap<UnitPrice> {
        public UnitPriceMap() {
            Table("UnitPrices");
            Id(x => x.PriceID).GeneratedBy.Identity();
            Map(x => x.ValuationDate);
            Map(x => x.Sedol);
            Map(x => x.NavPrice);
        }
    }

    public class UnitPrice {
        public virtual int PriceID { get; set; }
        public virtual DateTime ValuationDate { get; set; }
        public virtual string Sedol { get; set; }
        public virtual decimal NavPrice { get; set; }
    }
}
