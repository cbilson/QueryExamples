using System;
using FluentMigrator;
using FluentNHibernate.Mapping;

namespace QueryExamples.ReallyReallyComplicatedExample
{
    public class Position
    {
        public virtual int ID { get; set; }
        public virtual HoldingsReport HoldingsReport { get; set; }
        public virtual Security Security { get; set; }
        public virtual decimal Quantity { get; set; }
    }

    public class PositionMap : ClassMap<Position>
    {
        public PositionMap()
        {
            Id(x => x.ID).GeneratedBy.Identity();
            References(x => x.HoldingsReport);
            References(x => x.Security);
            Map(x => x.Quantity);
        }
    }

    [Migration(5)]
    public class create_position_table : Migration
    {
        public override void Up()
        {
            Create.Table("Position")
                .WithColumn("ID").AsInt32().PrimaryKey().Identity()
                .WithColumn("HoldingsReport_id").AsInt32().NotNullable()
                .WithColumn("Security_id").AsInt32().NotNullable()
                .WithColumn("Quantity").AsDecimal().NotNullable();

            Create.ForeignKey("FK_Position_HoldingsReport")
                .FromTable("Position").ForeignColumn("HoldingsReport_id")
                .ToTable("HoldingsReport").PrimaryColumn("ID");

            Create.ForeignKey("FK_Position_Security")
                .FromTable("Position").ForeignColumn("Security_id")
                .ToTable("Security").PrimaryColumn("ID");

        }

        public override void Down()
        {
            Delete.ForeignKey("FK_Position_HoldingsReport");

            Delete.ForeignKey("FK_Position_Security");

            Delete.Table("Position");
        }
    }
}
