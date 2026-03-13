using Library.Domain;
using Library.MVC.Areas.Admin.Controllers;
using Library.MVC.Controllers;
using Library.MVC.Data;
using Library.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class UnitTest5
{
    //This test doesn't check the authorization process itself, but rather that the controller
    //is protected by the [Authorize(Roles = "Admin")] attribute. In ASP.NET Core, this
    //attribute determines that only users with the Admin role are allowed to access the page,
    //while everyone else will automatically receive a 403 Forbidden response.
    [Fact]
    public void RolesController_HasAdminAuthorizeAttribute()
    {
        var type = typeof(RolesController);

        var attribute = type.GetCustomAttributes(typeof(AuthorizeAttribute), true)
                            .Cast<AuthorizeAttribute>()
                            .FirstOrDefault();

        Assert.NotNull(attribute);
        Assert.Equal("Admin", attribute.Roles);
    }
}
