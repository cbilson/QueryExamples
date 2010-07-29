using System;
using FluentMigrator;
using FluentNHibernate.Mapping;

namespace QueryExamples.ReallyReallyComplicatedExample
{
    public class Security
    {
        public virtual int ID { get; set; }
        public virtual string Ticker { get; set; }
        public virtual string ExchangeCode { get; set; }
        public virtual string Cusip { get; set; }
        public virtual string Sedol { get; set; }
        public virtual string Isin { get; set; }
        public virtual string SecurityTypeCode { get; set; }
        public virtual string GicsIndustry { get; set; }
        public virtual string GicsSector { get; set; }
        public virtual string NraTaxCountry { get; set; }
        public virtual string ExposureCurrency { get; set; }
        public virtual DateTime ActiveStartDate { get; set; }
        public virtual DateTime? ActiveEndDate { get; set; }
    }

    public class SecurityMap : ClassMap<Security>
    {
        public SecurityMap()
        {
            Id(x => x.ID).GeneratedBy.Identity();
            Map(x => x.Ticker);
            Map(x => x.ExchangeCode);
            Map(x => x.Cusip);
            Map(x => x.Sedol);
            Map(x => x.Isin);
            Map(x => x.SecurityTypeCode);
            Map(x => x.GicsIndustry);
            Map(x => x.GicsSector);
            Map(x => x.NraTaxCountry);
            Map(x => x.ExposureCurrency);
            Map(x => x.ActiveStartDate);
            Map(x => x.ActiveEndDate);
        }
    }

    [Migration(4)]
    public class create_security_table : Migration
    {
        public override void Up()
        {
            Create.Table("Security")
                .WithColumn("ID").AsInt32().PrimaryKey().Identity()
                .WithColumn("Ticker").AsString(15)
                .WithColumn("ExchangeCode").AsString(5)
                .WithColumn("Cusip").AsString(9)
                .WithColumn("Sedol").AsString(7)
                .WithColumn("Isin").AsString(12)
                .WithColumn("SecurityTypeCode").AsString(2)
                .WithColumn("GicsIndustry").AsInt32()
                .WithColumn("GicsSector").AsInt32()
                .WithColumn("NraTaxCountry").AsString(2)
                .WithColumn("ExposureCurrency").AsString(3)
                .WithColumn("ActiveStartDate").AsDate()
                .WithColumn("ActiveEndDate").AsDate().Nullable();
        }

        public override void Down()
        {
            Delete.Table("Security");
        }
    }
}