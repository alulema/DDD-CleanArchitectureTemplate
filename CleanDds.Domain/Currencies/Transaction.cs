using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CleanDds.Domain.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CleanDds.Domain.Currencies;

[DataContract]
public class Transaction : IEntity
{
    [DataMember(Name = "sku")]
    public string Sku { get; set; }

    [DataMember(Name = "amount")]
    public decimal Amount { get; set; }

    [DataMember(Name = "currency")]
    [JsonConverter(typeof(StringEnumConverter))]
    public CurrencyType Currency { get; set; }

    [Key]
    public Guid Id { get; set; }
}
