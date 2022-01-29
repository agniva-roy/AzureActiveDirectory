Imports System.Threading.Tasks
Imports Microsoft.IdentityModel.Protocols.OpenIdConnect
Imports Microsoft.IdentityModel.Tokens
Imports Microsoft.Owin
Imports Microsoft.Owin.Security
Imports Microsoft.Owin.Security.Cookies
Imports Microsoft.Owin.Security.Notifications
Imports Microsoft.Owin.Security.OpenIdConnect
Imports Owin


<Assembly: OwinStartupAttribute("TestAD.Startup", GetType(TestAD.Startup))>
Namespace TestAD

    Public Class Startup
        Public Sub Configuration(ByVal app As IAppBuilder)
            Dim redirectUri As String = ConfigurationManager.AppSettings("RedirectUri")
            Dim clientId = ConfigurationManager.AppSettings("ClientId")
            Dim tenantId = ConfigurationManager.AppSettings("TenantId")
            Dim allowedIssuer As String = ""
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType)
            app.UseCookieAuthentication(New CookieAuthenticationOptions())
            app.UseOpenIdConnectAuthentication(New OpenIdConnectAuthenticationOptions With {
                    .ClientId = clientId,
                    .ClientSecret = ConfigurationManager.AppSettings("ClientSecret"),
                    .Authority = ConfigurationManager.AppSettings("Authority"),
                    .RedirectUri = redirectUri,
                    .UsePkce = True,
                    .PostLogoutRedirectUri = ConfigurationManager.AppSettings("PostLogoutRedirectUri"),
                    .Scope = $"{OpenIdConnectScope.OpenIdProfile} user.read",
                    .ResponseType = OpenIdConnectResponseType.Code,
                    .SaveTokens = True,
                    .Notifications = New OpenIdConnectAuthenticationNotifications With {
                        .AuthenticationFailed = AddressOf OnAuthenticationFailed,
                        .RedirectToIdentityProvider = AddressOf OnRedirectToIdentityProvider
                    },
                    .RedeemCode = True,
                    .TokenValidationParameters = New TokenValidationParameters() With {
                        .SaveSigninToken = True,
                        .ValidateIssuer = True,
                        .ValidateAudience = True,
                        .ValidateLifetime = True,
                        .RequireExpirationTime = True,
                        .RequireSignedTokens = True,
                        .ValidateIssuerSigningKey = True,
                        .ValidateTokenReplay = True,
                        .ValidAudience = clientId,
                        .ValidIssuers = New List(Of String) From {
                            $"https://sts.windows.net/{tenantId}/",
                            $"https://login.microsoftonline.com/{tenantId}/v2.0"
                        },
                        .IssuerValidator = Function(issuer, securityToken, tokenValidationParameters)

                                               For Each allowedIssuer In tokenValidationParameters.ValidIssuers

                                                   If issuer.Equals(allowedIssuer) Then
                                                       Return issuer
                                                   End If
                                               Next

                                               Throw New SecurityTokenInvalidIssuerException($"Invalid issuer. Issuer '{issuer}' does not match any of the allowed issuers.")
                                           End Function
                    }
                })
        End Sub

        '<summary>
        'Handle failed authentication requests by redirecting the user To the home page With an Error In the query String
        '</summary>
        '<param name = "context" ></param>
        '<returns></returns>
        Private Function OnAuthenticationFailed(ByVal context As AuthenticationFailedNotification(Of OpenIdConnectMessage, OpenIdConnectAuthenticationOptions)) As Task
            context.HandleResponse()
            context.Response.Redirect("/?errormessage=" & context.Exception.Message)
            Return Task.FromResult(0)
        End Function

        '<summary>
        'Adds a domain hint To the protocol redirect URL To the identity provider, so that the initial Microsoft login page Is bypassed.
        '</summary>
        '<param name = "context" > The OpenID Connect authentication context.</param>
        '<returns> A <see cref="Task"/>.</returns>
        Private Function OnRedirectToIdentityProvider(ByVal context As RedirectToIdentityProviderNotification(Of OpenIdConnectMessage, OpenIdConnectAuthenticationOptions)) As Task
            context.ProtocolMessage.DomainHint = ConfigurationManager.AppSettings("DomainHint")
            Return Task.FromResult(0)
        End Function
    End Class
End Namespace

