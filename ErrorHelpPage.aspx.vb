
Partial Class ErrorHelpPage
    Inherits System.Web.UI.Page


    Dim ErrDesc As String = ""
    Dim ErrType As String = ""

    Dim ErrMsg As String = ""
    Dim ErrDetails As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim URL As String = ""
        '---new
        Dim ex As Exception = HttpContext.Current.Server.GetLastError()   'Context.Error
        Dim loc As String = ""
       
        If ex IsNot Nothing Then

            If ex.GetType Is GetType(HttpRequestValidationException) Then
                ErrType = ex.GetType.ToString
                ErrDesc = "Your input contains some dangerous data like html tags, java script, not-permitted characters. "
                ErrDesc += " Please correct your input and try again!"

            Else
                If TypeOf ex Is HttpUnhandledException AndAlso ex.InnerException IsNot Nothing Then
                    ex = ex.InnerException
                    ErrType = "An unhandled exception"
                Else
                    ErrType = ex.GetType.ToString
                End If

                ErrDesc = "Message:" & ex.Message & ". StackTrace:" & ex.StackTrace & ". Location:" & ex.TargetSite.Name

            End If

            lblErrorType.Text = ErrType
            lblErrorText.Text = ErrDesc

            If IsNothing(Request.UrlReferrer) Then
                btnClose.Text = "Close"
                btnClose.Attributes.Add("onclick", "window.close();")
            Else
                btnClose.Text = "Back"
                URL = Request.UrlReferrer.ToString

                If InStr(URL, "Action=Add") > 0 Then
                    URL = Replace(URL, "Action=Add", "Action=ChangeNew")
                End If
                btnClose.Attributes.Add("onclick", "return URLredirect('" & URL & "');")
            End If

            Server.ClearError()
        End If



    End Sub

End Class
