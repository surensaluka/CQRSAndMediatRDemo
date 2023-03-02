using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Reflection.PortableExecutable;
using System.Security.Claims;

namespace CQRSAndMediatRDemo.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class WeatherForecastController : ControllerBase
	{
		private static readonly string[] Summaries = new[]
		{
		"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
	};

		private readonly ILogger<WeatherForecastController> _logger;

		public WeatherForecastController(ILogger<WeatherForecastController> logger)
		{
			_logger = logger;
		}

		//https://devblogs.microsoft.com/dotnet/announcing-net-5-0/#expanding-system-directoryservices-protocols-to-linux-and-macos
		//https://www.c-sharpcorner.com/article/how-to-get-domain-users-search-users-and-user-from-active-directory-using-net/
#pragma warning disable CA1416 // Validate platform compatibility
		[HttpGet(Name = "GetWeatherForecast")]
		public IEnumerable<WeatherForecast> Get()
		{
			List<KeyValuePair<string, string>> headers = null;

			foreach (KeyValuePair<string, string> header in headers)
			{

			}

			#region MyRegion - PrincipalContext Works
			PrincipalContext context = new PrincipalContext(ContextType.Domain);

			UserPrincipal principal = new UserPrincipal(context);
			principal.UserPrincipalName = "*@*";
			principal.Enabled = true;
			principal.SamAccountName = "*gi*";

			PrincipalSearcher searcher = new PrincipalSearcher(principal);

			var users = searcher.FindAll()
				.AsQueryable()
				.Cast<UserPrincipal>()
				.Where(x => x.DistinguishedName.Contains("OU=Finance"))
				.OrderBy(x => x.Surname)
				.ToList();

			var ww = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, "surenm");

			if (context.ValidateCredentials("surenm", "Password99("))
			{
				const string LDAP_PATH1 = "LDAP://BOARDPAC/OU=Users,DC=boardpac,DC=local";

				using (var de = new System.DirectoryServices.DirectoryEntry(LDAP_PATH1))
				using (var ds = new DirectorySearcher(de))
				{
					ds.SearchRoot = de;
					// Other logic to verify user has correct permissions

					// User authenticated and authorized
					var identities = new List<ClaimsIdentity> { new ClaimsIdentity("custom auth type") };
					var ticket = new AuthenticationTicket(new ClaimsPrincipal(identities), Options.DefaultName);
				}
			}

			#endregion

			#region MyRegion DirectoryEntry
			//DirectoryEntry de = new DirectoryEntry();
			//de.Path = "LDAP://BOARDPAC/DC=boardpac,DC=local";
			//de.AuthenticationType = AuthenticationTypes.None;

			//DirectorySearcher deSearch = new DirectorySearcher();
			//string userName = "surenm";

			//deSearch.SearchRoot = de;
			//deSearch.Filter = "(uid=" + userName + ")";

			//SearchResult result = deSearch.FindOne();
			//var www = deSearch.FindAll();
			#endregion

			#region MyRegion
			const string LDAP_PATH = "LDAP://BOARDPAC/OU=Users,DC=boardpac,DC=local";
			const string LDAP_DOMAIN = "boardpac"; //@boardpac.co

			using (var context1 = new PrincipalContext(ContextType.Domain, LDAP_DOMAIN))
			{
				if (context1.ValidateCredentials("surenm", "Password99("))
				{
					using (var de = new System.DirectoryServices.DirectoryEntry(LDAP_PATH))
					using (var ds = new DirectorySearcher(de))
					{
						// other logic to verify user has correct permissions

						// User authenticated and authorized
						var identities = new List<ClaimsIdentity> { new ClaimsIdentity("custom auth type") };
						var ticket = new AuthenticationTicket(new ClaimsPrincipal(identities), Options.DefaultName);
					}
				}
			}
			#endregion

			return Enumerable.Range(1, 5).Select(index => new WeatherForecast
			{
				Date = DateTime.Now.AddDays(index),
				TemperatureC = Random.Shared.Next(-20, 55),
				Summary = Summaries[Random.Shared.Next(Summaries.Length)]
			})
			.ToArray();
		}
	}
}