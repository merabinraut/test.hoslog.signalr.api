using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace hoslog.signalr.api.Filters;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string AuthorizationHeader = "Authorization";
    private const string BasicScheme = "Basic";

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(AuthorizationHeader, out var authHeader))
        {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return AuthenticateResult.Fail("Missing authorization header");
        }


        var authHeaderValue = authHeader.ToString();
        if (!authHeaderValue.StartsWith(BasicScheme, StringComparison.OrdinalIgnoreCase))
        {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return AuthenticateResult.Fail("Invalid authorization scheme");
        }

        var base64Credentials = authHeaderValue.Substring(BasicScheme.Length).Trim();
        string[] credentials;
        try
        {
            var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(base64Credentials));
            credentials = decodedCredentials.Split(':', 2);
        }
        catch
        {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return AuthenticateResult.Fail("Invalid Base64 encoding");
        }

        if (credentials.Length != 2)
        {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return AuthenticateResult.Fail("Invalid credential format");
        }

        var username = credentials[0];
        var password = credentials[1];

        if (username == "rabin" && password == "raut")
        {
            var claims = new[] { new Claim(ClaimTypes.Name, username) };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }

        Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        return AuthenticateResult.Fail("Invalid credentials");
    }
}