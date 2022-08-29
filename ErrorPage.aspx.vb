Imports Fenix
Imports Fenix.ProgramType
Imports System.Data.SqlClient
Imports System.Data

Partial Class ErrorPage
    Inherits System.Web.UI.Page

    Dim ErrDesc As String = ""
    Dim ErrType As String = ""
    Dim ErrTypeFriendly As String = ""
    Dim ErrMsg As String = ""
    Dim ErrStack As String = ""
    Public ErrDetails As String = ""
    Public backURL As String = ""
    Public homeURL As String = "../Frame/Frame.aspx?Reset=False"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim ex As Exception = HttpContext.Current.Server.GetLastError()   'Context.Error
        Dim ErrorUrl As String = Utils.GetStr(Request.Params("aspxerrorpath")).Trim
        Dim DBLogged As Byte = 0

        If ex IsNot Nothing Then

            If ex.GetType Is GetType(HttpRequestValidationException) Then
                ErrType = ex.GetType.ToString
                ErrTypeFriendly = "HTTP Request Validation Exception"

                ErrDesc = "Request Validation has detected a potentially dangerous client input value "
                ErrDesc += "(like html tags, java script, not-permitted characters). "
                ErrDesc += "This value may indicate an attempt to compromise the security of your application, "
                ErrDesc += "so processing of the request has been aborted."

                ErrDetails = Trim(" Message:" & ex.Message & ".")
            Else
                If TypeOf ex Is HttpUnhandledException AndAlso ex.InnerException IsNot Nothing Then
                    ex = ex.InnerException
                End If

                ErrType = ex.GetType.ToString

                If ex.GetType Is GetType(SystemException) Then
                    ErrTypeFriendly = "System Exception"
                ElseIf ex.GetType Is GetType(FormatException) Or ex.GetType Is GetType(System.InvalidCastException) Then
                    ErrTypeFriendly = "Invalid Format Exception"
                ElseIf ex.GetType Is GetType(System.Data.SqlClient.SqlException) Then
                    ErrTypeFriendly = "SQL Exception"
                ElseIf ex.GetType Is GetType(OutOfMemoryException) Then
                    ErrTypeFriendly = "OutOfMemory Exception"
                ElseIf ex.GetType Is GetType(HttpUnhandledException) Then
                    ErrTypeFriendly = "Unhandled Exception"
                Else
                    ErrTypeFriendly = ErrType
                End If

                ErrMsg = ex.Message
                ErrStack = ex.StackTrace
                ErrDetails = ErrType & " has occured " & Now.ToString & _
                    ". Message:" & ErrMsg & " StackTrace:" & ErrStack & ". "
                ErrDetails = ErrDetails.Trim

            End If

            If ErrTypeFriendly.Trim.Length > 0 Then
                lblErrorMsg.Text = "1099Pro.Net application encountered an error while processing your request: " _
                               & Microsoft.Security.Application.AntiXss.HtmlEncode(ErrTypeFriendly.Trim) & "."
            End If
            If ErrDesc.Trim.Length > 0 Then
                lblErrorDesc.Text = Microsoft.Security.Application.AntiXss.HtmlEncode(ErrDesc)
                lblErrorDesc.Visible = True
            Else
                lblErrorDesc.Visible = False
            End If

            lblErrorDetails.Text = Microsoft.Security.Application.AntiXss.HtmlEncode(ErrDetails)

            '---GetData From SystemID table for Support info:
            GetSupportInfo()

            If IsNothing(Request.UrlReferrer) Then
                backURL = ""
            Else
                backURL = Request.UrlReferrer.ToString

                If backURL.Trim.Length > 0 Then
                    If InStr(backURL, "Action=Add") > 0 Then
                        backURL = Replace(backURL, "Action=Add", "Action=ChangeNew")
                    End If
                End If
            End If

            '---Log Error:
            LogError()

            '---Save Error To DB
            DBLogged = DBManager.SaveErrorToDB(1, ErrType, ErrMsg, ErrStack, ErrorUrl, "")

            '---Last Step
            'Server.ClearError()
            Context.ClearError()
        End If

    End Sub

    Sub LogError()
        '---Log Error:
        Trace.Write("ErrorPage:", ErrDetails)
   
    End Sub


    Sub GetSupportInfo()
        Dim aSystemID As Fenix.TableManagers.SystemID
        aSystemID = New Fenix.TableManagers.SystemID
        If Not (Application("GLO:ProgID") Is Nothing Or Utils.GetStr(Application("GLO:ProgID")) = "") Then
            aSystemID.Fetch(Application("GLO:ProgID"))
        Else
            aSystemID.Fetch(e1099Pro)
        End If

        '---Name
        If aSystemID.SupportName.Trim.Length > 0 Then
            lblSupportMsg.Text = "If the error persists, please contact " _
            & Microsoft.Security.Application.AntiXss.HtmlEncode(aSystemID.SupportName)
        End If

        '---Phone
        TSPhone.Text = Microsoft.Security.Application.AntiXss.HtmlEncode(aSystemID.SupportPhone)
        If TSPhone.Text = "" Then
            Me.prompt_TSPhone.Visible = False
        End If

        '---E-mail
        hlTSEmail.Text = Microsoft.Security.Application.AntiXss.HtmlEncode(aSystemID.SupportEmail)
        If hlTSEmail.Text = "" Then
            Me.prompt_TSEmail.Visible = False
        Else
            hlTSEmail.NavigateUrl = "mailto:" & aSystemID.SupportEmail
        End If

        If TSPhone.Text.Length > 0 Or hlTSEmail.Text.Length > 0 Then
            panelSupport.Visible = True
        Else
            panelSupport.Visible = False
        End If

    End Sub

End Class
