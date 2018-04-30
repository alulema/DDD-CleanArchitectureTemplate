using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CleanDds.Domain.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CleanDds.Domain.Currencies
{
    [DataContract]
    public class Rate : IEntity
    {
        [DataMember(Name = "from")]
        [JsonConverter(typeof(StringEnumConverter))]
        public CurrencyType CurrencyFrom { get; set; }

        [DataMember(Name = "to")]
        [JsonConverter(typeof(StringEnumConverter))]
        public CurrencyType CurrencyTo { get; set; }

        [DataMember(Name = "rate")]
        [DataType(DataType.Currency)]
        public decimal CurrencyRate { get; set; }

        [Key]
        public Guid Id { get; set; }
    }
}
