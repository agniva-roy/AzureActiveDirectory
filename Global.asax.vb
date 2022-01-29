Imports System.Web.Optimization

Public Class Global_asax
    Inherits HttpApplication

    Sub Application_Start(sender As Object, e As EventArgs)
        ' Fires when the application is started
    End Sub

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires at the beginning of each request
        If Not Context.Request.IsSecureConnection Then
            Response.Redirect(Context.Request.Url.ToString().Replace("http:", "https:"))
        End If
    End Sub
End Class