using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using Iks_ASConvert;
using MySqlConnector;

namespace Iks_VIPConvert;

[EventName("round_start")]
public class VipConvert : BasePlugin, IPluginConfig<PluginConfig>
{
    public override string ModuleName => "vipConvert";
    public override string ModuleVersion => "1.0.2";
    public override string ModuleAuthor => "iks x nipos x E!N";

    private string _dbConnectionString = "";

    public PluginConfig Config { get; set; } = new();

    public void OnConfigParsed(PluginConfig config)
    {
        _dbConnectionString = $"Server={config.Host};Database={config.Database};port={config.Port};User Id={config.User};password={config.Pass}";
        Config = config;

        RegisterEventHandler<EventRoundStart>(EventRoundStart);
        _ = SetFlagsToVipsAsync();
    }

    private HookResult EventRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        _ = SetFlagsToVipsAsync();
        return HookResult.Continue;
    }

    private async Task SetFlagsToVipsAsync()
    {
        List<Admin> admins = new();

        string sql = $@"SELECT * FROM vip_users WHERE (expires > {DateTimeOffset.UtcNow.ToUnixTimeSeconds()} OR expires = 0) AND sid = {Config.Sid}";
        try
        {
            using var connection = new MySqlConnection(_dbConnectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                admins.Add(new Admin(reader.GetInt32("account_id").ToString(), reader.GetString("group"), reader.GetInt32("sid")));
            }
        }
        catch (MySqlException ex)
        {
            Console.WriteLine($"[vipConvert] Db error: {ex}");
            return;
        }

        Server.NextFrame(() => SetVipFlags(admins));
    }

    private void SetVipFlags(List<Admin> admins)
    {
        foreach (var admin in admins)
        {
            var adminData = AdminManager.GetPlayerAdminData(admin.Steamid);
            if (adminData != null)
            {
                var existingFlags = adminData.Flags;
                var flagsToRemove = existingFlags.SelectMany(kvp => kvp.Value)
                                                 .Where(IsPluginFlag)
                                                 .ToList();

                foreach (var flag in flagsToRemove)
                {
                    AdminManager.RemovePlayerPermissions(admin.Steamid, flag);
                }
            }

            if (admin.Sid != Config.Sid) continue;

            foreach (var vipFlags in Config.ConvertVips)
            {
                if (admin.VipFlags.Contains(vipFlags.Key))
                {
                    AdminManager.AddPlayerPermissions(admin.Steamid, vipFlags.Value.ToArray());
                }
            }
        }
    }

    private bool IsPluginFlag(string flag)
    {
        return Config.ConvertVips.Values.Any(vipFlags => vipFlags.Contains(flag));
    }

    [CommandHelper(whoCanExecute: CommandUsage.SERVER_ONLY)]
    [ConsoleCommand("css_vip_convert")]
    public void OnConvertCommand(CCSPlayerController? controller, CommandInfo info)
    {
        _ = SetFlagsToVipsAsync();
    }
}
