using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace LibraryManagement_FrontEnd.Models
{
	public class AdditionalUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
	{
		public AdditionalUserClaimsPrincipalFactory(
			UserManager<ApplicationUser> userManager,
			IOptions<IdentityOptions> optionsAccessor)
			: base(userManager, optionsAccessor) { }
		protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
		{
			var identity = await base.GenerateClaimsAsync(user);
			identity.AddClaim(new Claim("Name", user.Name));
			identity.AddClaim(new Claim("IsAdmin", user.IsAdmin.ToString()));
			
			return identity;
		}
	}
}
