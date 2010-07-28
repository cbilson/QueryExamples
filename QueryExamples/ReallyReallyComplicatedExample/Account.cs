using FluentMigrator;
using FluentNHibernate.Mapping;

namespace QueryExamples.ReallyReallyComplicatedExample
{
    public class Account
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string AccountNumber { get; set; }
    }

    public class AccountMap : ClassMap<Account>
    {
        public AccountMap()
        {
            Id(x => x.ID).GeneratedBy.Identity();
            Map(x => x.Name);
            Map(x => x.AccountNumber);
        }
    }

    [Migration(2)]
    public class CreateAccountTable : Migration
    {
        public override void Up()
        {
            Create.Table("Account")
                .WithColumn("ID").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("Name").AsString(50).NotNullable()
                .WithColumn("AccountNumber").AsString(50).NotNullable();
        }

        public override void Down()
        {
            Delete.Table("Account");
        }
    }


}
