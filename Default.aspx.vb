Imports Microsoft.Owin.Security
Imports Microsoft.Owin.Security.OpenIdConnect

Public Class _Default
    Inherits BaseClass

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        'SignIn()
        checkUserPrevilige()
    End Sub

    Protected Sub SignIn()
        If Not Request.IsAuthenticated Then
            HttpContext.Current.GetOwinContext().Authentication.Challenge(New AuthenticationProperties With {
.RedirectUri = "/"
}, OpenIdConnectAuthenticationDefaults.AuthenticationType)
            Exit Sub
        End If
    End Sub
End Class