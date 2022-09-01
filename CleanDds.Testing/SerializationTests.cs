using CleanDds.Domain.Common;
using CleanDds.Domain.Currencies;
using Newtonsoft.Json;
using Xunit;

namespace CleanDds.Testing;

public class SerializationTests
{
    [Fact]
    public void TestRateToJson()
    {
        var entityTest = new Rate
        {
            CurrencyFrom = CurrencyType.AUD,
            CurrencyTo = CurrencyType.CAD,
            CurrencyRate = 0.88m
        };

        var converted = JsonConvert.SerializeObject(entityTest);
        Assert.Equal(@"{""from"":""AUD"",""to"":""CAD"",""rate"":0.88}", converted);
    }
    
    [Fact]
    public void TestTransactionToJson()
    {
        var entityTest = new Transaction
        {
            Amount = 10.23m,
            Currency = CurrencyType.CAD,
            Sku = "T2006"
        };

        var converted = JsonConvert.SerializeObject(entityTest);
        Assert.Equal(@"{""sku"":""T2006"",""amount"":10.23,""currency"":""CAD""}", converted);
    }
}