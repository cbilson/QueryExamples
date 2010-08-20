namespace QueryExamples
{
    public class Money
    {
        public virtual decimal Amount { get; set; }
        public virtual string CurrencyCode { get; set; }
    }

    public static class MoneyHelpers
    {
        public static Money In(this int amount, string currencyCode)
        {
            return new Money {Amount = amount, CurrencyCode = currencyCode};
        }

        public static Money In(this decimal amount, string currencyCode)
        {
            return new Money { Amount = amount, CurrencyCode = currencyCode };
        }

        public static Money In(this double amount, string currencyCode)
        {
            return new Money { Amount = (decimal) amount, CurrencyCode = currencyCode };
        }


    }
}