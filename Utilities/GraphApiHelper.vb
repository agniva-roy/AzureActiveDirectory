
Imports System.Linq
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Security.Claims
Imports Microsoft.Graph
Imports Microsoft.Identity.Client

Namespace Utilities
    Module GraphApiHelper
        Private Function GetGraphApiAccessToken(ByVal scopes As IEnumerable(Of String)) As String
            Dim authority As String = ConfigurationManager.AppSettings("Authority")
            Dim clientId = ConfigurationManager.AppSettings("ClientId")
            Dim clientSecret = ConfigurationManager.AppSettings("ClientSecret")
            Dim confidentialClientApp As IConfidentialClientApplication = ConfidentialClientApplicationBuilder.Create(clientId).WithClientSecret(clientSecret).WithAuthority(New Uri(authority)).Build()
            Dim userContext = ClaimsPrincipal.Current.Identities.First().BootstrapContext.ToString()
            Dim authenticationResult = confidentialClientApp.AcquireTokenOnBehalfOf(scopes, New UserAssertion(userContext)).ExecuteAsync().Result
            Dim userAccessToken = authenticationResult.AccessToken
            Return userAccessToken
        End Function

        Function GetUserProfile() As User
            Dim graphApiAccessToken = GetGraphApiAccessToken(New List(Of String) From {
                "user.read"
            })
            Dim httpClient = New HttpClient()
            httpClient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", graphApiAccessToken)
            Dim result = httpClient.GetAsync("https://graph.microsoft.com/v1.0/me?$select=employeeId").Result

            If result.IsSuccessStatusCode Then
                Dim content = result.Content.ReadAsStringAsync().Result
                Dim user = System.Text.Json.JsonSerializer.Deserialize(Of User)(content)
                Return user
            End If

            Return Nothing
        End Function
    End Module
End Namespace





