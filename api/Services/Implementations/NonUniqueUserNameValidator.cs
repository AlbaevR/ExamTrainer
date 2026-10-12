using api.Models;
using Microsoft.AspNetCore.Identity;

namespace api.Services.Implementations;

// Usernames are display names only (login is by email, which stays unique),
// so several users may share the same username.
public class NonUniqueUserNameValidator : UserValidator<AppUser>
{
    public NonUniqueUserNameValidator(IdentityErrorDescriber? errors = null) : base(errors)
    {
    }

    public override async Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
    {
        var result = await base.ValidateAsync(manager, user);
        if (result.Succeeded)
            return result;

        var errors = result.Errors
            .Where(e => e.Code != nameof(IdentityErrorDescriber.DuplicateUserName))
            .ToArray();

        return errors.Length == 0 ? IdentityResult.Success : IdentityResult.Failed(errors);
    }
}
