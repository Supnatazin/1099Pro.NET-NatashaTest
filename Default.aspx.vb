Imports ComponentPro.Saml
Imports Fenix
Imports Fenix.Utils
Imports Fenix.SecFlags
Imports System.Data
Imports Fenix.TableManagers
Imports Fenix.SecUserAction
Imports System.Data.SqlClient
Imports Fenix.ProgramType
Imports Fenix.SecLogin
Imports Fenix.AppLogin
Imports Fenix.AppMode

'Lee W. 8/4/2022/ Added Error logging functionality into the Catch statements to log to the database – pro1099.WB_ErrorLog table - since trace is disabled for security reasons.

'' -----------------------------------------------------
''                        Page Class 
'' -----------------------------------------------------
Partial Public Class _Default
    Inherits System.Web.UI.Page

    '' -----------------------------------------------------
    '' Page Events
    '' -----------------------------------------------------
    Dim relayState As String = ""                       '= Request.QueryString.Get("relayState") 
    Dim UserName As String = ""
    Dim aSecPref As Fenix.TableManagers.SecPrefs
    Dim GloAppLogin As Byte = 0
    Dim GloAppMode As Byte = 0
    Public strTitle As String = ""
    Dim ErrorMsg As String = ""

    Dim DBLogged As Byte = 0

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try

            ''---test
            ' Dim SessionIdNumber As String = Session.SessionID
            ' DBLogged = DBManager.SaveErrorToDB_CY(0, "Default.Page_Load", _
            '"ProgID=" & Utils.GetByte(Session("GLO:ProgID")) & ";relayState=" & relayState & _
            '";InitType=" & Utils.GetStr(Session("InitType")) & ";UserName=" & Utils.GetStr(Session("userName")) & _
            '";SessionIdNumber=" & SessionIdNumber, _
            '"", "Default.aspx", "Page_Load")

            '---For MultiDB configuration (AppMode=1) exit this page and go to Login page (LoginMDB.aspx)
            GloAppMode = Fenix.Utils.GetByte(Application("GLO:AppMode"))
            GloAppLogin = Fenix.Utils.GetByte(Application("GLO:AppLogin"))

            If GloAppMode = eAppModeMultiDB Then
                '--- Redirect to Login page
                FormsAuthentication.RedirectToLoginPage()   '"./Security/LoginMDB.aspx"
            End If

            '***************************************************************************
            '--- Initiate the System: set connections, set defaults, ect.
            '***************************************************************************  
            ErrorMsg = InitSystemSettings()

            ''--- Display any error message resulting from a failed system initializing
            If ErrorMsg.Trim.Length > 0 Then
                ValSummary.AddErrorMessage(ErrorMsg)
                Exit Sub
            End If

            '---Assign display settings from WB_Config
            If GloAppLogin = eAppLoginSSO Then
                SetGUI()
            End If

            Page_Refresh(Not Page.IsPostBack)

        Catch ex As System.Data.SqlClient.SqlException
            DBLogged = DBManager.SaveErrorToDB_CY(1, "Default.SqlException", ex.Message, _
                                Left(ex.StackTrace, 749), "Default.aspx", "Load")

        Catch ex As System.Exception
            DBLogged = DBManager.SaveErrorToDB_CY(1, "Default.SystemException", ex.Message, _
                                Left(ex.StackTrace, 749), "Default.aspx", "Load")
        End Try

    End Sub

    '' ------------------------------------------------------------------
    '' ----------- Refresh Controls (and Synchronize Components) --------
    '' ------------------------------------------------------------------
    Private Sub Page_Refresh(ByVal ForceRefresh As Boolean)

        Dim SAMLrelayState As String = ""
        Dim strMsg As String = ""
        Dim ReturnURL As String = ""
        Dim AssertionURL As String = ""
        Dim InitType As String = ""
        Dim DBLogged As Byte = 0

        On Error Resume Next

        If GloAppLogin = eAppLoginSSO Then
            '---UserName from Assertion page
            UserName = Utils.GetStr(Session("userName"))
            InitType = Utils.GetStr(Request.QueryString.Get("InitType"))  'Utils.GetStr(Session("InitType"))
            relayState = Utils.GetStr(Request.QueryString.Get("relayState"))
            ReturnURL = Utils.GetStr(Request.QueryString.Get("ReturnURL"))
            SAMLrelayState = Utils.GetStr(Session("relayState"))

            ' '---Test
            ' Dim RequestURL As String = Fenix.Utils.GetStr(Request.FilePath.ToString)

            ' DBLogged = DBManager.SaveErrorToDB_CY(0, "Default.Page_Refresh", _
            '"ProgID=" & Utils.GetByte(Session("GLO:ProgID")) & ";relayState=" & relayState & "=Sessionrelay:" & Utils.GetStr(Session("relayState")) & _
            '";InitType=" & InitType & "=Session:" & Utils.GetStr(Session("InitType")) & ";UserName=" & UserName & _
            '";ReturnURL:" & ReturnURL, _
            '"", "Default.aspx", "Page_Refresh")

            '====================================================
            '--- Returned from Assertion page
            '====================================================
            Select Case InitType
                Case "IdP"
                    '---IdP-initiated login

                    If UserName.Length > 0 Then
                        '---Do standard 1099Pro user validation
                        '---Get system settings, security profile, set default, etc.
                        strMsg = Open1099ProApp(UserName)
                    Else
                        strMsg = "IdP-initiated login: No User in cache. Please, contact Technical Support for instructions."
                        DBLogged = DBManager.SaveErrorToDB_CY(0, "Default.Page_Refresh", strMsg, "", "Default.aspx", "Page_Refresh")
                    End If

                    If strMsg.Trim.Length > 0 Then
                        ValSummary.AddErrorMessage(strMsg)
                    End If

                    Exit Sub

                Case Else    '"SP" or fresh login to default.aspx
                    '--- User was already loged in
                    '--- Continue validation if relayState not empty

                    If Not (IsNothing(relayState) Or Fenix.Utils.GetStr(relayState) = "") _
                        And Not (IsNothing(SAMLrelayState) Or Fenix.Utils.GetStr(SAMLrelayState) = "") Then


                        If relayState = SAMLrelayState Then

                            If UserName.Length > 0 Then
                                '---Do standard 1099Pro user validation
                                '---Get system settings, security profile, set default, etc.
                                strMsg = Open1099ProApp(UserName)
                            Else
                                strMsg = "SP-initiated login: No User in cache. Please, contact Technical Support for instructions."
                                DBLogged = DBManager.SaveErrorToDB_CY(0, "Default.Page_Refresh", strMsg, "", "Default.aspx", "Page_Refresh")
                            End If

                            If strMsg.Trim.Length > 0 Then
                                ValSummary.AddErrorMessage(strMsg)
                            End If

                            Exit Sub
                        End If
                    Else
                        
                        ''================================================
                        ''---Fresh login, redirect to Login page
                        ''================================================
                        ''--- Log off user/ Clear Session vars, SAML cache
                        'Fenix.ConfigManager.DoLogOff()

                        '---This redirect causing thread abort error, 
                        '---so we'll use FormsAuthentication.RedirectToLoginPage() below

                        ''--- Redirect to Login page                        
                        'Dim LoginURL As String = ""
                        'LoginURL = "./SSO/LoginSSO.aspx?ReturnURL=../default.aspx"
                        'Response.Redirect(LoginURL)
                        'Exit Sub
                    End If
            End Select
        End If

        ''================================================
        ''---Fresh login, redirect to Login page
        ''================================================

        '--- Log off user/ Clear Session Vars and Cookies
        Fenix.ConfigManager.DoLogOff()

        '--- Redirect to Login page
        '---Add Query string DefaultURL for case when initial link to app didn't include ../default.aspx
        '---In this case returnUrl in SSO/LoginSSO.aspx will be empty and won't  AuthenticateUser, as it's supposed to
        FormsAuthentication.RedirectToLoginPage("DefaultURL=../default.aspx")

    End Sub

    Function Open1099ProApp(ByVal pLogin As String) As String
        Dim strResult As String = ""
        Dim strClientIP As String = ""
        Dim strDomain As String = ""
        Dim strLogin As String = ""
        Dim bSecLoginType As Byte = 0
        Dim bValidateDomain As Boolean = False
        Dim IDFormat As Byte = 0
        Dim objDataSet As New DataSet

        ''---Assign display settings from WB_Config
        SetGUI()

        '***************************************************************************
        '---Find login type from SecPrefs table: standard or active directory
        '***************************************************************************
        aSecPref = New Fenix.TableManagers.SecPrefs

        '---Right now we take into consideration LoginType preference from SecPrefs
        '---Fetch the record for .NET version to determine Login Type
        '--- old: aSecPref.Fetch(Application("GLO:CSProgID"))    '4 - CSProgID

        '---Old
        'aSecPref.Fetch(Application("GLO:ProgID"))                '---14 - eASPStandard
        'bSecLoginType = Utils.GetByte(aSecPref.LoginType)

        '---New SecPrefs
        If Not IsNothing(aSecPref) Then

            objDataSet = aSecPref.GetSecPrefs(" RecID=" & Utils.GetByte(Application("GLO:ProgID")).ToString)
            If IsNothing(objDataSet) Then
                objDataSet.Dispose()
                objDataSet = Nothing

                Trace.Warn("Default.Open1099ProApp: SecPrefs - no Dataset!")
                strResult = "Sorry, your system is not properly configured (no Security Preferences dataset found)!" _
                    & Chr(10) & Chr(13) & " Please, contact your System Administrator!"
                Return strResult
            End If

            If Not objDataSet.Tables.Count > 0 Then
                objDataSet.Dispose()
                objDataSet = Nothing

                Trace.Warn("Default.Open1099ProApp: SecPrefs - no table!")
                strResult = "Sorry, your system is not properly configured (no Security Preferences table found)!" _
                    & Chr(10) & Chr(13) & " Please, contact your System Administrator!"
                Return strResult
            End If

            If (objDataSet.Tables(0).Rows.Count > 0) Then
                bSecLoginType = Utils.GetByte(objDataSet.Tables(0).Rows(0)("SPrefs_LoginType"))
                objDataSet.Dispose()
                objDataSet = Nothing
            Else
                objDataSet.Dispose()
                objDataSet = Nothing
                Trace.Warn("Default.Open1099ProApp: SecPrefs - no records!")
                strResult = "Sorry, your system is not properly configured (no Security Preferences records found)!" _
                    & Chr(10) & Chr(13) & " Please, contact your System Administrator!"
                Return strResult
            End If

            '---Check for CS SecPrefs (RecID=4)
            objDataSet = aSecPref.GetSecPrefs(" RecID=" & Utils.GetByte(Application("GLO:CSProgID")).ToString)
            If IsNothing(objDataSet) Then
                objDataSet.Dispose()
                objDataSet = Nothing

                Trace.Warn("Login_Page.InitSystemSettings: CS SecPrefs - no Dataset!")
                strResult = "Sorry, your system is not properly configured (no CS Security Preferences dataset found)!" _
                    & Chr(10) & Chr(13) & " Please, contact your System Administrator!"
                Return strResult
            End If

            If Not objDataSet.Tables.Count > 0 Then
                objDataSet.Dispose()
                objDataSet = Nothing

                Trace.Warn("Login_Page.InitSystemSettings: CS SecPrefs - no table!")
                strResult = "Sorry, your system is not properly configured (no CS Security Preferences table found)!" _
                    & Chr(10) & Chr(13) & " Please, contact your System Administrator!"
                Return strResult
            End If

        Else
            strResult = "Problem with database configuration: no Security Preferences found. Please, contact your System Administrator."
            Return strResult
        End If
        '---End of new SecPrefs

        If GloAppLogin = eAppLoginSSO Then
            '---SSO type of Login with IdP
            '---Force LoginType to SSO=3 in case if it's not set in SecPrefs.LoginType
            bSecLoginType = eSecLoginSSO
            IDFormat = Utils.GetByte(Application("SSO:NameIDFormat"))


                Select Case IDFormat
                    Case 0
                        '---Only UserName was passed, no domain. Ex: Voya.
                        strLogin = pLogin
                        strDomain = ""
                        bValidateDomain = False

                    Case Else
                        '---For future use: other types, using domain.
                        strLogin = pLogin
                        strDomain = ""
                        bValidateDomain = True
                End Select

            '---Clear SAML cache
            UtilSSO.ClearSAMLCache()


        Else
            '---Not-SSO Login: AD (Active Directory) or regular UserID/Password

            If bSecLoginType = eSecLoginActiveDir Then
                '--- AD (active Directory login: SecPrefs=2) 
                bValidateDomain = True

                '---Get domain \username           
                If InStr(pLogin, "\") > 0 Then
                    strLogin = Trim(pLogin.Split("\")(1))
                    strDomain = Trim(pLogin.Split("\")(0))
                    'ElseIf InStr(pLogin, "@") > 0 Then
                    '---Possibly for future use
                    '    strLogin = Trim(pLogin.Split("@")(0))
                    '    strDomain = Trim(pLogin.Split("@")(1))
                Else
                    strLogin = pLogin
                    strDomain = ""
                End If

                '---Get IP adress ---
                strClientIP = Context.Request.ServerVariables("REMOTE_ADDR")
                Session("GLO:MachineID") = strClientIP
            Else
                '---Regular Login/Password (SecPrefs.LoginType=0)
                strLogin = pLogin
                strDomain = ""
                bValidateDomain = False

                '---Reset bSecLoginType to standard UserID/Password in case if it was set incorrectly in SecPrefs
                '---Cannot be anything else in Not-SSO Login mode/ not-AD login
                bSecLoginType = eSecLoginDefault
            End If
        End If

        '***************************************************************************
        '---Validate User: if user exists, if has access to app, etc.
        '***************************************************************************
        strResult = SecManager.ValidateUserPro(strLogin, "", strDomain, bValidateDomain, bSecLoginType)

        If strResult.Trim.Length > 0 Then
            '---Some problem occured: 
            '--- Log off user/ Clear Session Vars and Cookies
            Fenix.ConfigManager.DoLogOff()

            Return strResult
        End If

        '***************************************************************************
        '---User Authenticated: start the system, redirect to Frame.aspx
        '---Session variables (Session("GLO:UserID"), etc.) are set at this point
        '***************************************************************************
        strResult = RedirectToApp(pLogin)

        Return strResult

    End Function


    Function InitSystemSettings() As String
        Dim strResult As String = ""
        Dim ProgID As Byte = eASPStandard

        '---Encrypt web.config for standard (not MDB) app
        ConfigManager.EncryptConnStr("DataProtectionConfigurationProvider")
        'ConfigManager.DecryptConnStr()

        '=====================================================================
        '---Set initial Appliation variables
        '=====================================================================
        Fenix.ConfigManager.SetApplicationDefaults()

        '=====================================================================
        '---Set Session defaults
        '=====================================================================
        ConfigManager.SetSessionDefaults()

      
        '--- Get DB Connection Info from DB
        '--- Because this code ony for SSO login, it only could be standard single DB mode, not  multi-DB
        strResult = DBManager.GetConnectInfoSDB()

        If strResult.Trim.Length > 0 Then
            Return strResult
        End If

        '--- Set Session Variables from System Tables (SystemID/ WB_Config)
        ProgID = Utils.GetByte(Session("GLO:ProgID"))
        strResult = ConfigManager.GetSystemSettings(ProgID)


        '---Check if DB version and application versions match
        strResult = ConfigManager.CheckVersionMatch()

        If strResult.Trim.Length > 0 Then
            Return strResult
        End If

        Return strResult

    End Function

    Sub SetGUI()

        '---Set Page Title:
        strTitle = Utils.GetStr(Session("GLO:AppName")) & " for " & Utils.GetInt(Session("GLO:AppYear")) & " Login"


        '---Welcome Icon:
        If Not (Session("GLO:WelcomeMsg") Is Nothing Or Session("GLO:WelcomeMsg") = "") Then
            lblWelcome.Text = Session("GLO:WelcomeMsg")
        Else
            lblWelcome.Text = ""
        End If

        '---App Icon     
        If Not (Session("GLO:AppImage") Is Nothing Or Session("GLO:AppImage") = "") Then
            AppImage.ImageUrl = "./Images/" & Session("GLO:AppImage")
        Else
            AppImage.ImageUrl = "./Images/1099App.gif"
        End If

        '---App Name
        If Not Utils.GetStr(Session("GLO:AppName")) = "" _
         And Not Utils.GetStr(Session("GLO:AppEdition")) = "" Then
            AppName.Text = Utils.GetStr(Session("GLO:AppName")) & " " & Utils.GetStr(Session("GLO:AppEdition"))
        Else
            AppName.Text = "1099 Pro.NET"
        End If

        '---Display Message
        If Not Utils.GetStr(Session("GLO:DisplayMsg")) = "" Then
            DisplayMsg.Text = Utils.GetStr(Session("GLO:DisplayMsg"))
        Else
            DisplayMsg.Text = ""
        End If

        '---Sec Prompt       
        'If Not Utils.GetStr(Session("GLO:SecPrompt")) = "" Then
        '    SecPrompt.Text = Utils.GetStr(Session("GLO:SecPrompt"))
        'Else
        SecPrompt.Text = "Credential used to access the system:"
        'End If

        '---Sec Icon - Image2
        If Not Utils.GetStr(Session("GLO:SecImage")) = "" Then
            Image2.ImageUrl = "./Images/" & Utils.GetStr(Session("GLO:SecImage"))
        Else
            Image2.ImageUrl = "./Images/Security.gif"
        End If

        '---Loc_UserName_Prompt
        If Not Utils.GetStr(Session("GLO:LoginPrompt")) = "" Then
            Loc_UserName_Prompt.Text = Utils.GetStr(Session("GLO:LoginPrompt"))
        Else
            Loc_UserName_Prompt.Text = "User ID"
        End If

        Loc_UserName.Text = UserName

        '---Admin Message
        If Not (Session("GLO:AdminMsg") Is Nothing Or Session("GLO:AdminMsg") = "") Then
            AdminMsg.Text = Utils.GetStr(Session("GLO:AdminMsg"))
        Else
            AdminMsg.Visible = False
            AdminMsg.Text = ""
        End If


    End Sub

    Function RedirectToApp(ByVal pLogin As String) As String
        Dim strResult As String = ""
        Dim NT_Name As String = ""
        Dim RetUrl As String = ""
        Dim RetUrl1 As String = ""
        Dim JS As String = ""
        Dim strAdminMsgIDs As String = ""
        Dim AdminMsgID As String = ""
        Dim EmptyFilterDigest As String = ""

        '=====================================================================
        '---Get System Preferences 
        '=====================================================================
        strResult = ConfigManager.GetSystemPreferences()

        If strResult.Trim.Length > 0 Then
            '--- Some problem occured:
            ''--- Log off user/ Clear Session Vars and Cookies
            'Fenix.ConfigManager.DoLogOff()
            Return strResult
        End If
        '=====================================================================
        '---Check if system is locked - stop if it is
        '=====================================================================
        If Utils.GetByte(HttpContext.Current.Session("GLO:Preferences.SystemLock")) = 1 Then
            '---System is locked
            If Utils.GetStr(HttpContext.Current.Session("GLO:Preferences.SystemLockMsg")).Length > 0 Then
                strResult = HttpContext.Current.Session("GLO:Preferences.SystemLockMsg")
            Else
                strResult = "System is currently unavailable. Please, try again later."
            End If

            '--- Some problem occured:
            ''--- Log off user/ Clear Session Vars and Cookies
            'Fenix.ConfigManager.DoLogOff()

            Return strResult
        End If

        '=====================================================================
        '---Set Session vars and cookies
        '=====================================================================
        '--- Get last saved Default Values from WB_User table---
        Session("GLO:UserID") = ConfigManager.GetUserSettings()
        Session("GLO:IsLoggedIn." & Utils.GetStr(Session("GLO:UserID"))) = True

        '--- Create Cookie in case if session variables corrupted ---
        Utils.SetCookie("GLO:UserID", Utils.GetStr(Session("GLO:UserID")))
        Utils.SetCookie("GLO:ProgID", Utils.GetStr(Session("GLO:ProgID")))

        '===================================================================================
        '--- Set Authentication cookie depending on type of connection: HTTP or HTTPS
        '===================================================================================
        'FormsAuthentication.SetAuthCookie(Utils.GetStr(Session("GLO:UserName")), False, _
        '         FormsAuthentication.FormsCookiePath)

        If HttpContext.Current.Request.IsSecureConnection = True Or Fenix.Utils.GetByte(HttpContext.Current.Application("GLO:UseSSL")) = 1 Then
            '---Works for HTTPS sites
            FormsAuthentication.SetAuthCookie(Utils.GetStr(Session("GLO:UserName")), False, _
                                FormsAuthentication.FormsCookiePath + "; SameSite=None; Secure")
        Else

            '---Works for HTTP sites
            FormsAuthentication.SetAuthCookie(Utils.GetStr(Session("GLO:UserName")), False, _
                             FormsAuthentication.FormsCookiePath + "; SameSite=Lax ")
        End If

        '=====================================================================
        '---Check and delete abanoned records and related ---
        '=====================================================================
        DBManager.DoOrphanMaintenance()

        '=====================================================================
        '---Admin Messages: Check if there are Admin Messages to display
        '=====================================================================
        '---Check if there are any not viewed AdminMsg 
        strAdminMsgIDs = Fenix.AdminMsg.CheckAdminMsg()

        If Len(strAdminMsgIDs) > 0 Then
            EmptyFilterDigest = SecManager.GetDigest("Filter=")

            '---Display only one message at a time 
            '---Next (if any) will appear after first window is closed with OK
            If InStr(strAdminMsgIDs, ",") > 0 Then
                AdminMsgID = Trim(Left(strAdminMsgIDs, InStr(strAdminMsgIDs, ",") - 1))
                strAdminMsgIDs = Trim(Mid(strAdminMsgIDs, InStr(strAdminMsgIDs, ",") + 1))
            Else
                AdminMsgID = Trim(strAdminMsgIDs)
                strAdminMsgIDs = ""
            End If
        End If

        '========================================================
        '--- Redirect to main app ---
        '========================================================
        '---Redirect to main program window 
        RetUrl = "./Frame/Frame.aspx?"

        '---Display Admin Messages, if any
        If Len(AdminMsgID) > 0 Then
            JS = "OpenAdminMsg('" & AdminMsgID & "','" & EmptyFilterDigest & "');"
        End If

        ''---Open link in  a custom  app-style window
        JS += "openMainWin('" & RetUrl & "'); "
        Utils.RegisterJScript(Me, JS, "NavigateToMainWindow")

        Return strResult

    End Function

    Sub Submit(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            'Page.Validate()

        Catch ex As System.Data.SqlClient.SqlException
            Trace.Warn("Default.Submit.SQL Error:" & ex.Message)

            DBLogged = DBManager.SaveErrorToDB_CY(1, "SQL Exception", ex.Message, ex.StackTrace, "Default.aspx.vb", "Submit")

        Catch ex As System.Exception
            Trace.Warn("Default.Submit.System Error:" & ex.Message)

            DBLogged = DBManager.SaveErrorToDB_CY(1, "System Exception", ex.Message, ex.StackTrace, "Default.aspx.vb", "Submit")
        End Try

    End Sub

End Class
