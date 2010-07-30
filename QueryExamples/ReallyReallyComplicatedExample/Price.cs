using System;
using FluentMigrator;
using FluentNHibernate.Mapping;

namespace QueryExamples.ReallyReallyComplicatedExample
{
    public abstract class PriceQuote
    {
        public virtual int ID { get; set; }
        public virtual Security Security { get; set; }
        public virtual DateTime EffectiveDate { get; set; }
    }

    public class PriceQuoteMap : ClassMap<PriceQuote>
    {
        public PriceQuoteMap()
        {
            Id(x => x.ID).GeneratedBy.Identity();
            References(x => x.Security);
            Map(x => x.EffectiveDate);

            DiscriminateSubClassesOnColumn("Type");
        }
    }

    public abstract class UnitPriceQuote : PriceQuote
    {
        public virtual Money Price { get; set; }
    }

    public class UnitPriceQuoteMap : SubclassMap<UnitPriceQuote>
    {
        public UnitPriceQuoteMap()
        {
            Component(x => x.Price, m =>
            {
                m.Map(money => money.Amount);
                m.Map(money => money.CurrencyCode);
            });
        }
    }


    public abstract class DiscountPriceQuote : PriceQuote
    {
        public virtual decimal Discount { get; set; }
    }

    public class DiscountPriceQuoteMap : SubclassMap<DiscountPriceQuote>
    {
        public DiscountPriceQuoteMap()
        {
            Map(x => x.Discount);
        }
    }

    public class UnitClosePrice : UnitPriceQuote
    {}

    public class UnitClosePriceMap : SubclassMap<UnitClosePrice> { }

    public class UnitOpenPrice : UnitPriceQuote
    { }

    public class UnitOpenPriceMap : SubclassMap<UnitOpenPrice> { }

    public class UnitSpotPrice : UnitPriceQuote
    {
        public virtual TimeSpan Time { get; set; }
    }

    public class UnitSpotPriceMap : SubclassMap<UnitSpotPrice> { }

    public class DiscountClosePrice : DiscountPriceQuote
    { }

    public class DiscountClosePriceMap : SubclassMap<DiscountClosePrice> { }

    public class DiscountOpenPrice : DiscountPriceQuote
    { }

    public class DiscountOpenPriceMap : SubclassMap<DiscountOpenPrice> { }

    public class DiscountSpotPrice : DiscountPriceQuote
    {
        public virtual TimeSpan Time { get; set; }
    }

    public class DiscountSpotPriceMap : SubclassMap<DiscountSpotPrice>
    {
        public DiscountSpotPriceMap()
        {
            Map(x => x.Time);
        }
    }

    [Migration(6)]
    public class create_price_table : Migration
    {
        public override void Up()
        {
            Create.Table("Price")
                .WithColumn("ID").AsInt32().PrimaryKey().Identity()
                .WithColumn("Security_id").AsInt32().NotNullable()
                .WithColumn("EffectiveDate").AsDate().NotNullable()
                .WithColumn("Type").AsString(50).NotNullable()
                .WithColumn("Price_Amount").AsDecimal(15, 2).Nullable()
                .WithColumn("Price_CurrencyCode").AsString(3)
                .WithColumn("Discount").AsDecimal(15, 12).Nullable()
                .WithColumn("Time").AsTime().Nullable();

            Create.ForeignKey("FK_Price_Security")
                .FromTable("Price").ForeignColumn("Security_id")
                .ToTable("Security").PrimaryColumn("ID");
        }

        public override void Down()
        {
            Delete.ForeignKey("FK_Price_Security");

            Delete.Table("Price");
        }
    }
}
