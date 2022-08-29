<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ErrorHelpPage.aspx.vb" Inherits="ErrorHelpPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <link href="../Images/favicon.ico?" type="image/x-icon" rel="shortcut icon"/>
    <title>Error Page</title>
    <script  type="text/javascript" language="javascript">
    

    function closeWin()
    {
    
    if (window.opener.name!=null && typeof(window.opener.name)!="undefined" && window.opener.name!="")
      {
      window.close();
      }
    else
      {
          window.history.back();
      
      }
    
    }
    
    function URLredirect(url)
      {
          //alert(url);
//          if (window.top.opener != null && typeof (window.top.opener) != "undefined") {
//              //alert(window.top.opener.name);
//              if (window.top.opener.name != "MainWindow" && window.top.opener.name !="") {
//                  window.top.close();
//                  return true;
//              }
//          }

          //window.location=url;      
          self.history.back();            
          return false;
      }
      
    
    </script>
    
</head>
<body onload="setTimeout('self.focus()',200);">
    <form id="form1" runat="server" >
    <div>
     <asp:Label ID="lblTitle" 
              Text="1099Pro.Net application encountered an error:" runat="Server"
              ForeColor="darkblue" 
              Font-Bold="True" 
              Font-Names="Arial"
              Font-Size="12" 	              
              CssClass="Label"
              style="  POSITION: absolute;LEFT: 10px;TOP: 8px; Z-INDEX: 2; vertical-align:text-top; white-space:nowrap; " 
              tabindex="-1"
              >
          </asp:Label> 
     <asp:Label ID="lblErrorTypePrompt" 
          Text="Error Type:" runat="Server"
          ForeColor="darkblue" 
          Font-Bold="True" 
          Font-Names="Arial"
          Font-Size="10" 	              
          CssClass="Label"
          style="  POSITION: absolute;LEFT: 10px;TOP: 40px; Z-INDEX: 2; vertical-align:text-top; white-space:nowrap; " 
          tabindex="-1"
          >
      </asp:Label>   
      <asp:Label ID="lblErrorType" 
          Text="" 
          runat="Server"
          ForeColor="darkblue" 
          Font-Bold="False" 
          Font-Names="Arial"
          Font-Size="8" 	              
          CssClass="Label"
          style="  POSITION: absolute;LEFT: 160px;TOP: 42px; Z-INDEX: 3; vertical-align:text-top; white-space:nowrap; " 
          tabindex="-1"
          >
        </asp:Label>
        <asp:Label ID="lblErrorTextPrompt" 
          Text="Error Description:" runat="Server"
          ForeColor="darkblue" 
          Font-Bold="True" 
          Font-Names="Arial"
          Font-Size="10" 	              
          CssClass="Label"
          style="  POSITION: absolute;LEFT: 10px;TOP: 70px; Z-INDEX: 2; vertical-align:text-top; white-space:nowrap; " 
          tabindex="-1"
          >
      </asp:Label>   
      <asp:Label ID="lblErrorText" 
          Text="" 
          runat="Server"
          ForeColor="darkblue" 
          Font-Bold="False" 
          Font-Names="Arial"
          Font-Size="8" 	              
          CssClass="Label"
          style="  POSITION: absolute;LEFT: 160px;TOP: 72px; Z-INDEX: 3; vertical-align:text-top; width" 
          tabindex="-1"
          >
        </asp:Label> 
        
         <asp:Button ID="btnClose" 
            Text="Close" runat="Server"
            ToolTip = "Close"
            ForeColor="Black" 
            Font-Bold="False" 
            Font-Names="Arial"
            Font-Size="8" 
            style=" POSITION: absolute;LEFT: 45%;TOP: 120px; Height:22px;width:70px; Z-INDEX: 57; white-space:normal; " 
            tabindex="20"
            >
         </asp:Button>
    </div>
    </form>
</body>
</html>
