using System.Text.Json.Serialization;

namespace ShitheadCardsApi.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum StatusEnum
    {
        SETUP,
        PLAYING,
        OUT
    }
}
