using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Filters
{
    public class AdminAuthorizationFilter : IAsyncActionFilter
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AdminAuthorizationFilter(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Check if user is signed in
            if (!_signInManager.IsSignedIn(context.HttpContext.User))
            {
                // Redirect to login page
                context.Result = new RedirectToActionResult("SimpleLogin", "Student", null);
                return;
            }

            // Get current user
            var currentUser = await _userManager.GetUserAsync(context.HttpContext.User);
            
            // Check if user exists and has admin role
            if (currentUser == null || currentUser.Role != "Admin")
            {
                // Redirect to unauthorized page or login
                context.Result = new RedirectToActionResult("SimpleLogin", "Student", null);
                return;
            }

            // User is authorized, continue with action
            await next();
        }
    }

    // Attribute version for easier use
    public class RequireAdminAttribute : TypeFilterAttribute
    {
        public RequireAdminAttribute() : base(typeof(AdminAuthorizationFilter))
        {
        }
    }
}
