using System;
using System.Collections.Generic;
using FluentMigrator;
using FluentNHibernate.Mapping;

namespace QueryExamples.ReallyReallyComplicatedExample
{
    public class HoldingsReport
    {
        public virtual int ID { get; set; }
        public virtual Account Account { get; set; }
        public virtual DateTime EffectiveDate { get; set; }
        public virtual ICollection<Position> Positions { get; set; }
    }

    public class HoldingsReportMap : ClassMap<HoldingsReport>
    {
        public HoldingsReportMap()
        {
            Id(x => x.ID).GeneratedBy.Identity();
            References(x => x.Account);
            HasMany(x => x.Positions).Cascade.AllDeleteOrphan();
            Map(x => x.EffectiveDate);
        }
    }

    [Migration(3)]
    public class create_holdings_report_table : Migration
    {
        public override void Up()
        {
            Create.Table("HoldingsReport")
                .WithColumn("ID").AsInt32().PrimaryKey().Identity()
                .WithColumn("AccountID").AsInt32().NotNullable()
                .WithColumn("EffectiveDate").AsDate().NotNullable();

            Create.ForeignKey("FK_HoldingsReport_Account")
                .FromTable("HoldingsReport").ForeignColumn("AccountID")
                .ToTable("Account").PrimaryColumn("ID");
        }

        public override void Down()
        {
            Delete.ForeignKey("FK_HoldingsReport_Account");

            Delete.Table("HoldingsReport");
        }
    }
}