

using Microsoft.AspNetCore.Mvc;

namespace DataShareCore.Common.Helper;

public class JwtHeper
{
    public static int GetUserId(ControllerBase ctl)
    {
        var claims = ctl.HttpContext.User.Claims;
        string userId = "0";
        foreach (var claim in claims)
        {
            if (claim.Type == "Id")
            {
                userId = claim.Value; break;
            }
        }

        return Int32.Parse(userId);
    }
}