using CleanDds.Domain.Common;
using CleanDds.Domain.Currencies;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace CleanDds.Testing
{
    public class SerializationTests
    {
        [Fact]
        public void RateSerialization_ShouldConvertRateObjectToJsonCorrectly()
        {
            // Arrange
            var entityTest = new Rate
            {
                CurrencyFrom = CurrencyType.AUD,
                CurrencyTo = CurrencyType.CAD,
                CurrencyRate = 0.88m
            };

            // Act
            var converted = JsonConvert.SerializeObject(entityTest);

            // Assert
            converted.Should().Be(@"{""from"":""AUD"",""to"":""CAD"",""rate"":0.88}", 
                because: "the serialized object should match the expected JSON string");
        }
        
        [Fact]
        public void TransactionSerialization_ShouldConvertTransactionObjectToJsonCorrectly()
        {
            // Arrange
            var entityTest = new Transaction
            {
                Amount = 10.23m,
                Currency = CurrencyType.CAD,
                Sku = "T2006"
            };

            // Act
            var converted = JsonConvert.SerializeObject(entityTest);

            // Assert
            converted.Should().Be(@"{""sku"":""T2006"",""amount"":10.23,""currency"":""CAD""}", 
                because: "the serialized object should match the expected JSON string");
        }
    }
}