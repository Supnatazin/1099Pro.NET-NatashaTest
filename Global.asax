<%@ Application Language="VB" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Diagnostics" %>
<%@ Import Namespace="System.Globalization"%>
<%@ Import Namespace="System.Security" %>
<%@ Import Namespace="System.Net.Security" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="System.Security.Cryptography.X509Certificates" %>
<%@ Import Namespace="Fenix.Utils"%>
<%@ Import Namespace="Fenix.SecManager"%>
<%@ Import Namespace="Fenix.FormManager"%>
<%@ Import Namespace="Fenix.ConfigManager"%>
<%@ Import Namespace="ComponentPro" %>
<%@ Import Namespace="ComponentPro.Saml"%>
<%@ Import Namespace="ComponentPro.Saml.Diagnostics" %>
<%@ Import Namespace="ComponentPro.Saml2" %>
<%@ Import Namespace="ComponentPro.Saml2.Metadata" %>
<%@ Import Namespace="ComponentPro.TrialLicenseKey" %>


<script runat="server" >    
    
    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        '--- Code that runs on application startup    
             
        '--- Current Version of the program
        Application("GLO:ProgramVersion") = "2021.17.0901"
       
        '---Set initial Application/Session variables
        Fenix.ConfigManager.SetApplicationDefaults()
     
    End Sub
   
    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ''--- Fires at the beginning of each request    
        
        '====================================================================================================
        ''---UseSSL is coming from WB_Config table - retrieved on Login/LoginSSO page (ConfigManager.GetSystemSettings)   
                
        If (Request.IsSecureConnection = False _
            And HttpContext.Current.Request.IsLocal = False _
            And Fenix.Utils.GetStr(HttpContext.Current.Application("GLO:UseSSL")) = "1") Then
            Dim redir As String = "https://" & Request.ServerVariables("HTTP_HOST") & Request.RawUrl
            Response.Redirect(redir)
        End If
        '====================================================================================================
        Application("GLO:Host") = Request.ServerVariables("HTTP_HOST")
        
    End Sub
    
    Protected Sub Application_PreSendRequestHeaders(ByVal sender As Object, ByVal e As EventArgs)
        
        ''---Disable to prebent information disclosure via HTTP headers
        ''---Need to remove the remote web server disclosure of information via HTTP headers
        HttpContext.Current.Response.Headers.Remove("Server")
        
        
        ''--- Disable VersionHeader to prevent ASP.NET Version Disclosure
        ''----Also, set in web.config  <httpRuntime enableVersionHeader = "false" ...
        HttpContext.Current.Response.Headers.Remove("X-AspNet-Version")
        
        ''-- Remove X-Powered-By to prevent information disclosure
        ''---This doesn't work here, so have to use web.config <remove name="X-Powered-By" /> 
        'HttpContext.Current.Response.Headers.Remove("X-Powered-By")

        
        ''---SAMEORIGIN will allow pages to be displayed in a frame on the same origin as the page itself 
        ''---Same as web.config  <add name="X-Frame-Options" value="SAMEORIGIN"/>
        HttpContext.Current.Response.AddHeader("X-Frame-Options", "SAMEORIGIN")
        
        ''--- Add X‐XSS‐Protection header to help prevent XSS arttacks -->
        ''---Same as web.config  <add name="X-XSS-Protection" value="1; mode=block"/>
        HttpContext.Current.Response.AddHeader("X-XSS-Protection", "1; mode=block")
        
        ' ''--- Prevents the browser from MIME-sniffing a response away from the declared content-type.
        ' ''---This will minimize a risk of a Cross-Site Scripting (XSS) attack-->
        ' ''---Same as web.config   <add name="X-Content-Type-Options" value="nosniff" />
        HttpContext.Current.Response.AddHeader("X-Content-Type-Options", "nosniff")
        
        ''--- This is default setting for Referrer-Policy. 
        ''--- The origin, path, and querystring of the URL are sent as a referrer 
        ''--- when the protocol security level stays the same (HTTP→HTTP, HTTPS→HTTPS) or improves (HTTP→HTTPS), 
        ''---but(isn) 't sent to less secure destinations (HTTPS→HTTP). -->
        ''---Same as web.config<add name="Referrer-Policy" value="no-referrer-when-downgrade" />
        '' HttpContext.Current.Response.Headers.Add("Referrer-Policy", "no-referrer-when-downgrade")
        HttpContext.Current.Response.AddHeader("Referrer-Policy", "no-referrer-when-downgrade")
        
        '---Set default Content-Type
        '---Do NOT set default type as it messes up pdf files
        'HttpContext.Current.Response.ContentType = "text/html"
        
        
        
    End Sub
    
    Sub Application_EndRequest(ByVal sender As Object, ByVal e As EventArgs)
        '---This is set in web.config and Default.aspx/ Login.aspx
        Dim authCookie As String = FormsAuthentication.FormsCookieName
        Dim cookieNames() As String = Request.Cookies.AllKeys
        Dim sCookie As String = ""
        
        For Each sCookie In cookieNames
            If (sCookie.Equals(authCookie) Or authCookie.ToUpper = "ASP.NET_SESSIONID") Then
                Dim HttpCookie As HttpCookie = Response.Cookies(sCookie)
                If Not IsNothing(HttpCookie) Then
                    If HttpContext.Current.Request.IsSecureConnection = True Or Fenix.Utils.GetByte(HttpContext.Current.Application("GLO:UseSSL")) = 1 Then
                        HttpCookie.Path = "/; SameSite=None; Secure"
                        HttpCookie.Secure = True
                    Else
                        HttpCookie.Path = "/; SameSite=Lax"
                    End If
                    HttpCookie.HttpOnly = True
                End If
            End If
        Next
    End Sub
    
    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when an unhandled error occurs
        ' All details about the error will be handed in ErrorPage.aspx
        Dim url As String = ""
        Dim ex As Exception = HttpContext.Current.Server.GetLastError()   'Context.Error
       
        If ex IsNot Nothing Then
            url = "~/ErrorPage.aspx"
            Server.Transfer(url)
        End If
        
    End Sub
    
   
    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        '--- Code that runs on application shutdown  
        '--- Remove Orphan records/ Log off user/ Clear Session Vars and Cookies
        Fenix.ConfigManager.DoCleanup()
        
    End Sub
        
    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
    
     
        '============================================================
        '---Redirect to Exit page if Session has expired
        '============================================================
        Dim url As String = ""
        Dim ReturnURL As String = ""
        Dim DefaultURL As String = ""
        Dim Action As String = ""
        Dim LoginType As Byte = 0
        Dim AssertionURL As String = ""
        Dim RequestURL As String = ""
        Try
            If Session.IsNewSession Then
                '---Clear old session data
                Session.Clear()
        
               
                '---Set new Session defaults 
                Fenix.ConfigManager.SetSessionDefaults()
                
                If Not IsNothing(Request.Headers("Cookie")) Then
                    
                    If Request.Headers("Cookie").IndexOf("ASP.NET_SessionId") >= 0 Then
                        
                        If Request.IsSecureConnection Then
                            Response.Cookies("ASP.NET_SessionId").Secure = True
                        End If
                        
                        ReturnURL = Fenix.Utils.GetStr(Request.QueryString.Get("ReturnURL"))
                        Action = Fenix.Utils.GetStr(Request.QueryString.Get("Action"))
                        RequestURL = Fenix.Utils.GetStr(Request.FilePath.ToString)
                        
                        If InStr(UCase(ReturnURL), "/PUBLIC") > 0 Then
                            DefaultURL = "PUBLIC/DEFAULT.ASPX"
                        Else
                            DefaultURL = "DEFAULT.ASPX"
                        End If
                                       
                        '---Not using Application vars as it may not exist
                        If Not IsNothing(System.Configuration.ConfigurationManager.AppSettings("AppLogin")) Then
                            LoginType = Fenix.Utils.GetByte(System.Configuration.ConfigurationManager.AppSettings("AppLogin"))
                        Else
                            LoginType = Fenix.AppLogin.eAppLoginStd   '0
                        End If
                        
                        If Not IsNothing(System.Configuration.ConfigurationManager.AppSettings("sp-assertionUrl")) Then
                            AssertionURL = Fenix.Utils.GetStr(System.Configuration.ConfigurationManager.AppSettings("sp-assertionUrl"))
                        Else
                            AssertionURL = "SSO/AssertionService.aspx"   '0
                        End If
                                      
                        If InStr(UCase(ReturnURL), UCase(Application("FilePath:AppPath") & DefaultURL)) > 0 _
                         Or InStr(UCase(RequestURL), UCase(Application("FilePath:AppPath") & DefaultURL)) > 0 Then
                            '---Do nothing - just initial page, do not go to Exit  
                            
                        ElseIf (LoginType = Fenix.AppLogin.eAppLoginSSO And InStr(UCase(RequestURL), UCase(AssertionURL)) > 0) Then
                            '---Session expired after AssertionService, need to re-login using SP_initiated login
                            url = "~/default.aspx"
                            Response.Redirect(HttpUtility.UrlPathEncode(url))
                            Exit Sub
                        Else
                            
                            If Not Action = "Quit" Then
                                '---Expired session, go to exit  
                                If LoginType = Fenix.AppLogin.eAppLoginSSO _
                                    And InStr(UCase(RequestURL), UCase(AssertionURL)) > 0 Then
                                    Action = "Login"
                                Else
                                    Action = "ExpSession"
                                End If
                                
                                '---Clear SAML cache
                                Fenix.UtilSSO.ClearSAMLCache()
                                
                                'Else
                                '---Session expired when user clicked on Logout button, need to quit 
                            End If
                            
                            If InStr(UCase(ReturnURL), "/PUBLIC") > 0 Then
                                url = "../Public/Exit.aspx?Action=" & Action & "&ReturnURL=" & ReturnURL
                            Else
                                url = "./Exit.aspx?Action=" & Action & "&ReturnURL=" & ReturnURL
                            End If
                                                                                   
                            Response.Redirect(HttpUtility.UrlPathEncode(url))
                            Exit Sub
                            
                        End If
                    End If
                End If

            End If
            
             
        Catch ex As Exception
            Trace.Write("Global_asax.System Error:" & ex.Message)
        End Try
    End Sub
        
    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        
        ' Code that runs when a session ends. 
        ' Note: The Session_End event is raised only when the sessionstate mode
        ' is set to InProc in the Web.config file. If session mode is set to StateServer 
        ' or SQLServer, the event is not raised.
             
        '--- Remove Orphan records/ Log off user/ Clear Session Vars and Cookies
        Fenix.ConfigManager.DoCleanup()
        
    End Sub
       
</script>