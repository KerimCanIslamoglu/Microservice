using FreeCourse.IdentityServer.Models;
using IdentityModel;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace FreeCourse.IdentityServer.Services
{
    public class IdentityResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityResourceOwnerPasswordValidator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = await _userManager.FindByEmailAsync(context.UserName);

            if (user == null)
            {
                var errors = new Dictionary<string, object>();
                errors.Add("errors",new List<string> {"Email or Password is incorrect." });
                context.Result.CustomResponse = errors;

                return;
            }
            var password = await _userManager.CheckPasswordAsync(user, context.Password);

            if (password == false)
            {
                var errors = new Dictionary<string, object>();
                errors.Add("errors", new List<string> { "Email or Password is incorrect." });
                context.Result.CustomResponse = errors;

                return;
            }

            context.Result = new GrantValidationResult(user.Id.ToString(),OidcConstants.AuthenticationMethods.Password);
        }
    }
}
