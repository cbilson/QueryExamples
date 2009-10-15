using FluentMigrator;

namespace QueryExamples.ForMatt
{
    [Migration(1)]
    public class create_unit_prices_table : Migration {

        public override void Up()
        {
            Create.Table("UnitPrices")
                .WithColumn("PriceID").AsInt32().PrimaryKey().Identity()
                .WithColumn("ValuationDate").AsDateTime()
                .WithColumn("Sedol").AsString()
                .WithColumn("NavPrice").AsDecimal();
        }

        public override void Down()
        {
            Delete.Table("UnitPrices");
        }
    }
}