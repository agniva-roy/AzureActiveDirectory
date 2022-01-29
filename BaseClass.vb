Imports System.Threading
Imports Microsoft.Owin.Security
Imports Microsoft.Owin.Security.OpenIdConnect
Imports TestAD.Utilities

Public Class BaseClass
    Inherits System.Web.UI.Page

    Private m_functionUrl As String
    Public Property FunctionUrl() As String
        Get
            Return m_functionUrl
        End Get
        Set(ByVal Value As String)
            m_functionUrl = Value
        End Set
    End Property

    Public Shadows ReadOnly Property UserID() As String
        Get
            Dim value As String = String.Empty
            Dim _identity As System.Security.Claims.ClaimsPrincipal = TryCast(Thread.CurrentPrincipal, System.Security.Claims.ClaimsPrincipal)
            If _identity.Identity.IsAuthenticated Then
                value = GetUserProfile().EmployeeId
            End If
            Return value
        End Get
    End Property

    Protected Sub checkUserPrevilige()
        Try
            If Not IsPostBack Then
                Dim ServiceURL As String = ""
                If Not Request.IsAuthenticated Then
                    HttpContext.Current.GetOwinContext().Authentication.Challenge(New AuthenticationProperties With {
.RedirectUri = "/"
}, OpenIdConnectAuthenticationDefaults.AuthenticationType)
                    Exit Sub
                End If
                Dim user_id As String = Me.UserID
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12
                If user_id.Length > 0 Then
                    Response.Redirect("~/About.aspx")
                End If
            End If
        Catch ex As Exception
            Server.Transfer("GenericErrorPage.aspx", True)
        End Try
    End Sub
End Class
