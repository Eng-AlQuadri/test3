using API.Extensions;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers;

public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        if (!resultContext.HttpContext.User.Identity!.IsAuthenticated) return;

        var userId = resultContext.HttpContext.User.GetUserId();

        var userManager = resultContext.HttpContext.RequestServices.GetService<UserManager<AppUser>>();

        if (userManager is null) return;

        var user = await userManager.Users.SingleOrDefaultAsync(x => x.Id == userId);

        if (user is null) return;

        user.LastActive = DateTime.UtcNow;

        await userManager.UpdateAsync(user);
    }
}
