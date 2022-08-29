<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>
<%@ Register TagPrefix="Fenix" Namespace="Fenix.Web.Controls" Assembly="Fenix.Web.Controls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
   <link href="../Images/favicon.ico?" type="image/x-icon" rel="shortcut icon"/>
   <title><%=Microsoft.Security.Application.AntiXss.HtmlEncode(Session("GLO:AppTitle"))%></title>
   <link href="./Styles/Default.css" type="text/css" rel="stylesheet" />
   <script src="./Scripts/Security.js" type="text/javascript" ></script>  
   <script  type="text/javascript" language="javascript">
       function doOnLoad() {
           loadInParent();
           setWinStatus('<%=strTitle%>');           
           if ('<%=Page.IsPostBack%>' != 'True') 
               {
                DetectPopUp('');
            }

            //Turn off native IE10 caps-on warning to avoid double warning
            capsOff();
            
       }
    
      function loadInParent() {
   
       if ('<%=Page.IsPostBack%>' != 'True') {
           if (window.top != null && typeof (window.top) != "undefined") {           
                   if (window.top.name != window.name) {
                       window.top.location = window.location.href;
                   }               
             }
         }   //Page.IsPostBack       
       }

      
   </script>
</head>

 <body background="./Images/<%=Session("GLO:BgndImage")%>" onload="doOnLoad();" onresize="setWinSize('login');" >
  <form id="Default" runat="server" >
     <Fenix:FocusManager id="FocusMng" runat="server">                    
     </Fenix:FocusManager>     
       <asp:Panel ID="PanelStd" 
              BackColor="Transparent"
              runat="Server"             
              Font-Bold="True" 
              Font-Names="Arial"
              Font-Size="9"
              CssClass="Panel"
              style="  POSITION: absolute;LEFT: 0px;TOP: 0px; Z-INDEX:1;" 
              tabindex="1"               
              Height="100%" 
              Width="100%" 
              Visible="true"
              >         
       
          <asp:Panel ID="PanelMainShadow" 
             runat="Server"
             CssClass="shadowGray"
             BackColor="gainsboro"
             style="  POSITION: absolute;LEFT: 33%;TOP: 25%; Z-INDEX:1; " 
             tabindex="-1"
             Height="175px" 
             Width="422px" 
         >
         </asp:Panel> 
         <asp:Panel ID="PanelMain" 
              runat="Server"   
              Font-Bold="True" 
              Font-Names="Arial"
              Font-Size="9"
              CssClass="Panel"
              style="  POSITION: absolute;LEFT: 33%;TOP: 25%; border: outset 2px whitesmoke; Z-INDEX:1; " 
              tabindex="-1"               
              Height="175px" 
              Width="422px" 
              >         
         <asp:Image ID="AppImage" 
              ImageURL="./Images/1099App.gif"
              runat="Server"              
              CssClass="Image"
              style="  POSITION: absolute;LEFT: 16px;TOP: 10px;  Z-INDEX: 1 " 
              tabindex="7" BorderColor="White"
              >
         </asp:Image>
         
         <asp:Label ID="lblWelcome" 
              Text="Welcome to" runat="Server"
              ForeColor="#000080" 
              Font-Bold="True" 
              Font-Names="Arial"
              Font-Size="12" 
              CssClass="Label"
              style="  POSITION: absolute;LEFT: 132px;TOP: 10px;  Width:281px; Height:30px; text-align:center; Z-INDEX: 1 " 
              tabindex="-1"
              >
         </asp:Label>
        
         <asp:Label ID="AppName" 
              runat="Server"
              text="1099 Pro Enterprise/SQL"
              ForeColor="#000080" 
              Font-Bold="True" 
              Font-Names="Arial"
              Font-Size="14" 
              CssClass="Label"
              style="  POSITION: absolute;LEFT: 132px;TOP: 30px;  Width:281px; Height:30px; text-align:center; Z-INDEX: 1 " 
              tabindex="-1"
              >
         </asp:Label>
         
         <asp:Label ID="DisplayMsg" 
              runat="Server"
              text=""
              ForeColor="#000080" 
              Font-Bold="False" 
              Font-Names="Arial"
              Font-Size="10" 
              CssClass="Label"
              style="  POSITION: absolute;LEFT: 132px;TOP: 50px;  Width:281px; Height:30px; text-align:center; Z-INDEX: 1; " 
              tabindex="-1"
              >
         </asp:Label>
         
         <asp:Label ID="SecPrompt" 
              Text="Credential used to access the system:" runat="Server"
              Font-Bold="false"
              Font-Names="Arial"
              Font-Size="8" 
              CssClass="Label"
              style="  POSITION: absolute;LEFT: 115px;TOP: 110px;  Width:300px; Height:22px;  z-index: 1; " 
              tabindex="-1"
              >
         </asp:Label>
         
         <asp:Image ID="Image2" 
              ImageURL="./Images/Security.gif" runat="Server"              
              CssClass="Image"
              style="  POSITION: absolute;LEFT: 40px;TOP: 115px;  Z-INDEX: 1 " 
              tabindex="5"
              >             
         </asp:Image>
         
         <asp:Label ID="Loc_UserName_Prompt" 
              Text="" runat="Server"
              Font-Bold="True" 
              Font-Names="Arial"
              Font-Size="8pt" 
              CssClass="Prompt"
              style="  POSITION: absolute;LEFT: 130px;TOP: 130px;  Z-INDEX: 1 " 
              tabindex="-1"
              >
         </asp:Label>       
             
         <asp:Label ID="Loc_UserName" 
              runat="Server"
              Font-Bold="False" 
              Font-Names="Arial"
              Font-Size="8pt" 
              CssClass="TextBox"
              MaxLength="25"              
              style="  POSITION: absolute;LEFT: 180px;TOP:130px;  Width:177px;  Z-INDEX: 1; " 
              tabindex="2"
              >
         </asp:Label>            
        
         <asp:Panel ID="Panel1" 
              runat="Server"
              BackColor="Transparent" 
              Font-Bold="True" 
              Font-Names="Arial"
              Font-Size="9"
              CssClass="Panel"
              style="  POSITION: absolute;LEFT: 18px;TOP: 90px;  Width:388px; Height:62px; border: groove 1px whitesmoke;  Z-INDEX: 0;" 
              tabindex="9" 
              >
         </asp:Panel>    
             
         <Fenix:FenixValidationSummary ID="ValSummary" 
                runat="Server" 
                style="POSITION: absolute;LEFT: -25px;TOP: 190px; Z-INDEX: 1; "
                Font-Names="Arial"
                Font-Size="12px"              
                EnableClientScript="true" 
                ShowSummary="true" 
                ShowMessageBox="false"
                DisplayMode="BulletList"
                tabindex="-1" 
                Height="60px" 
                Width="480px">
          </Fenix:FenixValidationSummary>  
          
                  
          <asp:Label ID="AdminMsg" 
                Text="" runat="Server"              
                Font-Bold="False" 
                Font-Names="Arial"
                Font-Size="9" 
                CssClass="Label"
                style="  POSITION: absolute;LEFT: -50%;Width:200%;TOP: 320px;  border-top: dashed 1 gray; border-bottom: dashed 1 gray;text-align:left;" 
                tabindex="-1"
                >
         </asp:Label> 
      </asp:Panel>   
    </asp:Panel>              
  
 </form>
</body>
</html>
