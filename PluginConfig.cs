
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;
namespace Iks_VIPConvert;

public class PluginConfig : BasePluginConfig
{
    [JsonPropertyName("host")] public string Host { get; set; } = "host";
    [JsonPropertyName("database")] public string Database { get; set; } = "database";
    [JsonPropertyName("user")] public string User { get; set; } = "user";
    [JsonPropertyName("pass")] public string Pass { get; set; } = "pass";
    [JsonPropertyName("port")] public string Port { get; set; } = "3306";
    [JsonPropertyName("sid")] public int Sid { get; set; } = 2;

    [JsonPropertyName("ConvertVips")]
    public Dictionary<string, List<string>>  ConvertVips { get; set; } = new Dictionary<string, List<string>>()
    {
        {"VipGod", new List<string>() {
            "@css/vipgod"
        }},
        {"VipLite", new List<string>() {
            "@css/viplite"
        }},
        {"VipPremium", new List<string>() {
            "@css/vipprem"
        }}
    };
}
