Imports Fenix
Imports Fenix.Web.Controls
Imports Fenix.FormManager
Imports Fenix.AppLogin
Imports Fenix.AppMode
Imports ComponentPro
Imports ComponentPro.Saml
Imports ComponentPro.Saml.Binding
Imports ComponentPro.Saml2
Imports ComponentPro.Saml2.Binding
Imports ComponentPro.Saml.Diagnostics
Imports ComponentPro.Saml2.Metadata

'Lee W. 7/21/2022/ Added Error logging functionality into the Catch statements to log to the database – pro1099.WB_ErrorLog table - since trace is disabled for security reasons.

'' -----------------------------------------------------
''                        Page Class 
'' -----------------------------------------------------
Partial Public Class Exit_Page : Inherits System.Web.UI.Page

    '' -------- Fenix Controls --------------------
    Public ValSummary As FenixValidationSummary
    Public LoginURL As String = ""

    '----------Query string Parameters--------------------------

    Public Action As String = ""                    '= Request.QueryString.Get("Action")
    Public ReturnURL As String = ""                 '= Request.QueryString.Get("ReturnURL")
    
    '----------Private Fields----------------------
    Private DBLogged As Byte = 0

    '' -----------------------------------------------------
    '' Page Events
    '' -----------------------------------------------------

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim QSPValid As Boolean = True
        Dim exc As Exception = HttpContext.Current.Server.GetLastError()   'Context.Error

        '--------Get QueryString Variables------------------------------------
        Action = Request.QueryString.Get("Action")
        If IsNothing(Action) Then Action = ""
        Action = Request.QueryString.Get("Action")
        If IsNothing(Action) Then Action = ""
        ReturnURL = Request.QueryString.Get("ReturnURL")
        If IsNothing(ReturnURL) Then ReturnURL = ""


        'Response.Write("Exit.Error:" & IsNothing(exc) & "/" & Session.Contents.Count & "/" & Fenix.SecManager.ValidateUser.ToString & "/" & Utils.GetStr(HttpContext.Current.Session("GLO:UserID")))
        'Response.End()

        '---test
        'Action = "ExpSession"

        If Utils.GetByte(Application("GLO:AppMode")) = eAppModeMultiDB Then
            '---MultiDB mode
            '---First do Cleaning after possible previous session
            '--- Remove Orphan records/ Log off user/ Clear Session Vars and Cookies
            Fenix.ConfigManager.DoCleanup()

            Session("GLO:SDLoggedIn") = False
            LoginURL = "./Security/LoginMDB.aspx"
        Else
            '---Standard single DB mode
            If Utils.GetByte(Application("GLO:AppLogin")) = eAppLoginSSO Then

                '---SSO login through Identity provider: has to go through the default.aspx to properly build request
                'LoginURL = "./SSO/LoginSSO.aspx"
                LoginURL = "./Default.aspx"

                '---Clear SAML cache
                UtilSSO.ClearSAMLCache()

                '--- Remove Orphan records/ Log off user/ Clear Session Vars and Cookies
                'Fenix.ConfigManager.DoLogOff()
                Fenix.ConfigManager.DoCleanup()

                ''---test
                'Dim DBLogged As Byte = DBManager.SaveErrorToDB_CY(0, "Exit.Load", "Action:" & Action _
                '                        & ";ReturnURL:" & ReturnURL & ";FilePath:" & Request.FilePath.ToString _
                '                        & "; LoginType=" & Utils.GetByte(Application("GLO:AppLogin")) & _
                '                        ";InitType:" & Utils.GetStr(Session("InitType")), "", "Exit.aspx", "OnLoad")

            Else
                '---Standard login user User Name and Password
                LoginURL = "./Security/Login.aspx"
            End If
        End If

        '--------Validate QueryString Variables---
        ValidateQSP()

        If QSPValid = False Then
            Action = "Quit"
        End If
      
        '-----end Validate QueryString Variables ---

        Try
            If exc IsNot Nothing Then
                DBLogged = DBManager.SaveErrorToDB_CY(1, exc.GetType.ToString, exc.Message, exc.StackTrace, exc.Source, "Exit.OnLoad")
            End If
            
            If InStr(Action, "ExpSession") > 0 Then
                doExpSession()
            ElseIf InStr(Action, "Quit") > 0 Then
                doQuitApp()
            ElseIf InStr(Action, "Login") > 0 Then
                doLogin()
            End If

        Catch ex As System.Data.SqlClient.SqlException
            Trace.Warn("Exit.Page_Load.SQL Error:" & ex.Message)
            DBLogged = DBManager.SaveErrorToDB_CY(1, "SQL Exception", ex.Message, ex.StackTrace, ex.Source, "Exit.OnLoad")
        Catch ex As System.Exception
            Trace.Warn("Exit.Page_Load.System Error:" & ex.Message)
            DBLogged = DBManager.SaveErrorToDB_CY(1, "System Exception", ex.Message, ex.StackTrace, ex.Source, "Exit.OnLoad")
        End Try

    End Sub

    Private Function ValidateQSP() As Boolean
        Dim bResult As Boolean = True
        Dim strPattern As String = ""

        '---Action
        strPattern = "^[a-zA-Z_]{1,20}$"
        bResult = Fenix.FormManager.ValidateQSP(Action, "StringRegExp", 20, strPattern)

        Return bResult

    End Function


    Sub doLogin()
        Dim RecsAffected As Byte = 0
        Dim JS As String = ""

        '---Check and delete Orphan records/TransLog
        RecsAffected = DBManager.CheckOrphanRecords()

        '--- Log off last user, if any            
        '--- Clear Session Vars and Cookies
        Fenix.ConfigManager.DoLogOff()

        ''---Abandon Session
        'Session.Abandon()

        '---Sign out
        FormsAuthentication.SignOut()

        '---display login screen
        JS = "gotoLogin('" & LoginURL & "');"
        Utils.RegisterJScript(Me, JS, "doLogin")

        ''--- Alternative way to Redirect to Login page
        'FormsAuthentication.LoginUrl = loginURL
        'FormsAuthentication.RedirectToLoginPage()

    End Sub

    Sub doExpSession()
        Dim strMsg As String = ""
        Dim RecsAffected As Byte = 0
        Dim JS As String = ""

        On Error Resume Next

        strMsg = "Exit: Your 1099 Pro Session has expired! You have to log in again."

        '---Session var Session("GLO:ExpSessionPrompt") is created 
        '---to avoid double prompt about expired session
        If Utils.GetByte(Session("GLO:ExpSessionPrompt")) < 1 Then
            Utils.CreateMessageAlert(Me, strMsg, "PromptExpSession")
            Session("GLO:ExpSessionPrompt") = Utils.GetByte(Session("GLO:ExpSessionPrompt"))
            Session("GLO:ExpSessionPrompt") += 1
        End If

        '---Check and delete Orphan records/TransLog
        RecsAffected = DBManager.CheckOrphanRecords()

        '--- Log off last user, if any            
        '--- Clear Session Vars and Cookies
        Fenix.ConfigManager.DoLogOff()

        ''---Abandon Session
        'Session.Abandon()

        '---Sign out
        FormsAuthentication.SignOut()

        '---display login screen
        JS = "gotoLogin('" & LoginURL & "');"
        Utils.RegisterJScript(Me, JS, "doExpSession")

        ''--- Alternative way to Redirect to Login page
        'FormsAuthentication.LoginUrl = loginURL
        'FormsAuthentication.RedirectToLoginPage()



    End Sub


    Sub doQuitApp()
        Dim JS As String = ""
        Dim RecsAffected As Byte = 0

        Try

            '---Check and delete Orphan records/TransLog
            RecsAffected = DBManager.CheckOrphanRecords()

            '--- Log off user             
            '--- Clear Session Vars and Cookies
            Fenix.ConfigManager.DoLogOff()

        Catch ex As Exception
            Trace.Warn(ex.ToString)
            DBLogged = DBManager.SaveErrorToDB_CY(1, "System Exception", ex.Message, ex.StackTrace, "Exit.aspx.vb", "doQuitApp")

        Finally
            '--Close Connections
            If Not Session("GLO:ActiveConnection") Is Nothing Then
                Session("GLO:ActiveConnection").Close()
                Session("GLO:ActiveConnection").Dispose()
                Session("GLO:ActiveConnection") = Nothing
            End If
        End Try

        '---Sign out
        FormsAuthentication.SignOut()

        '---Abandon Session: disable, as it causes Session to expire
        '---In case of SSO kills all Session vars
        'Session.Abandon()

        If Action = "Quit" Then
            JS = "setTimeout('CloseWin()',100);"
            Utils.RegisterJScript(Me, JS, "EndApp")
        End If
    End Sub

End Class

