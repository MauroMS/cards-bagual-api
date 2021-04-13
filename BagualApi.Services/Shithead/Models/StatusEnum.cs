using System.Text.Json.Serialization;

namespace Bagual.Services.Shithead.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum StatusEnum
    {
        SETUP,
        PLAYING,
        OUT
    }
}
