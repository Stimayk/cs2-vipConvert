using CounterStrikeSharp.API.Modules.Entities;
namespace Iks_ASConvert;

public class Admin
{
    public SteamID Steamid;
    public string VipFlags;
    public int Sid;

    public Admin(string account_id, string group, int sid)
    {
        Steamid = new SteamID(ulong.Parse(account_id));
        VipFlags = group;
        Sid = sid;
    }
}
