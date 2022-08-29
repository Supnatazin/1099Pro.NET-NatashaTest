<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ErrorPage.aspx.vb" Inherits="ErrorPage" %>

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
         if (window.top.opener != null && typeof (window.top.opener) != "undefined") 
         {
            if (window.top.opener.name != null && typeof (window.top.opener.name) != "undefined") 
            {
                if (window.top.opener.name != "MainWindow" && window.top.opener.name != "") 
                {                     
                    window.top.close();
                    return false;
                }                
            }                         
        }     
        window.location=url;           
        //self.history.back();            
        return false;
    }

    function goHome() {
        
        if (window.top.name != null && typeof (window.top.name) != "undefined") {
            
            if (window.top.name == "MainWindow") 
            {
              //part of the MainWindow
              window.top.location = '<%=homeURL%>';             
            }
            else 
            {
                //pop-up window
                if (window.top.opener != null && typeof (window.top.opener) != "undefined") {
                    window.top.opener.location = '<%=homeURL%>';
                }                
                window.top.close();  
            }
         }        

        return false;
    }

    function goBack() {
       
        var url = '<%=backURL%>';
        if (window.top.name != null && typeof (window.top.name) != "undefined") {

            if (window.top.name == "MainWindow") {
                //part of the MainWindow
                //window.top.location = '<%=backURL%>';
                window.top.history.back(); 
               
            }
           else {
               //pop-up window
               if (window.top.opener != null && typeof (window.top.opener) != "undefined") {
                   if (url.length > 0) {
                       window.top.location = '<%=backURL%>';
                   }
                   else {
                       self.close();
                   }                                       
                }
          }
        }
        return false;
    }
    
    function ShowError() {
          var pErrDet = document.getElementById("panelErrorDetails");
          var lErrDet = document.getElementById("lblErrorDetails");
          var ErrText = lErrDet.innerText;
          if (pErrDet != null && typeof (pErrDet) != "undefined" 
           && lErrDet != null && typeof (lErrDet) != "undefined") {             
              if (ErrText.length >0) {
                  pErrDet.style.display = "block";
              }
          }
         
         return false;
      }

      function HideError() {
          var errDet = document.getElementById("panelErrorDetails");
          if (errDet!=null && typeof(errDet)!="undefined") {
              errDet.style.display = "none";
          }
          return false;
      }
        
    </script>
    
</head>
<body onload="setTimeout('self.focus()',200);">
    <form id="form1" runat="server" >
    <div style="POSITION: relative; LEFT: 0px; TOP: 0px; width: 95%;">
    <asp:ImageButton ID="imgError" 
        runat="Server"          
        CssClass="ImageButton"   
        ImageURL="./Images/AppError.gif"
        OnClientClick="return ShowError();"
        style="  POSITION: absolute; LEFT: 3px; TOP: 3px;  border: 0px outset; width: 20px;height:20px; " 
        tabindex="3"      
        >
    </asp:ImageButton>
     <asp:Label ID="lblTitle" runat="Server"
        Text="An Error Has Occured" 
        Font-Names="Arial"
        ForeColor="darkblue" 
        Font-Bold="True"         
        Font-Size="11" 	              
        CssClass="Label"
        style="  POSITION: absolute;LEFT: 30px;TOP: 5px; Z-INDEX: 2; vertical-align:text-top; white-space:nowrap; " 
        tabindex="-1"
        >
    </asp:Label> 
     <asp:Label ID="lblErrorMsg" runat="Server"
          Text="1099Pro.Net application encountered an unexpected error while processing your request" 
          ForeColor="darkblue" 
          Font-Bold="False" 
          Font-Names="Arial"
          Font-Size="8"          	              
          CssClass="Label"
          style="  POSITION: relative;LEFT: 30px;TOP: 25px; vertical-align:text-top; line-height:15px; white-space:normal; Z-INDEX: 3;" 
          tabindex="-1"
          >
      </asp:Label>   
 
      <asp:Label ID="lblErrorDesc" runat="Server"
          Text="" 
          ForeColor="darkblue" 
          Font-Bold="False" 
          Font-Names="Arial"
          Font-Size="8"          	              
          CssClass="Label"
          Visible="False"
          style="  POSITION: relative;LEFT: 30px;TOP:25px;line-height:15px; vertical-align:text-top; white-space:normal; Z-INDEX: 4;" 
          tabindex="-1"
          >
      </asp:Label>   
      
      <asp:Panel runat="server"
         ID="panelErrorDetails"
         Style="position: absolute; left: 30px; top: 24px; width:90%;z-index: 99; display:none;"
        >
          <asp:Label ID="lblErrorDetails" runat="Server" 
            Text="" 
            BackColor="Gainsboro" 
            ForeColor="darkblue"
            Font-Bold="False" Font-Names="Arial" Font-Size="8" 
            CssClass="Label" 
            Style="position: absolute; left: 0px; top: 0px;  padding: 8px; border: solid 1px darkgray;z-index: 100;" TabIndex="-1">
          </asp:Label>
           <asp:Label ID="lblX" runat="Server"
            Text="x" 
            ForeColor="DarkGray"
            Font-Bold="True" Font-Names="Arial Black" Font-Size="12" 
            CssClass="Label" 
            onmouseover="this.style.cursor='pointer';this.style.color='DarkBlue';"
            onmouseout="this.style.cursor='auto';this.style.color='DarkGray';"
            onClick="return HideError();"
            Style="position: absolute; left: 97%; top: -4px; width:12px;height:12px; cursor:default; text-align:center;  z-index: 100;" TabIndex="-1">
          </asp:Label>         
        </asp:Panel>
      <p>
      <asp:Label ID="lblHints" runat="Server"
            Text="What you can do from here" 
            Font-Names="Arial"
            ForeColor="darkblue" 
            Font-Bold="True"          
            Font-Size="11" 	              
            CssClass="Label"
            style="  POSITION: relative; TOP: 20px; LEFT: 30px; Z-INDEX: 5; vertical-align:text-top; white-space:nowrap;  " 
            tabindex="-1"
            >
        </asp:Label>
         </p> 
        <p>
        <asp:Panel runat="server" 
          ID="panelHints"
          ForeColor="darkblue" 
          Font-Bold="False" 
          Font-Names="Arial"
          Font-Size="8" 
          CssClass="Label"
          style="  POSITION: relative;LEFT: 30px;TOP: 0px;   Z-INDEX: 6; vertical-align:text-top; height:auto;" 
          tabindex="-1">          
          <p>
          <span style="  POSITION: relative;LEFT: 0px;TOP: 10px;  vertical-align:text-top; white-space:normal; ">
          <font  face="Wingdings" size="1px">w</font>
          Validate your data: it should not contain dangerous data like html tags, java script, not-permitted characters
          </span> 
          </p> 
          <p>          
          <span style="  POSITION: relative;LEFT: 0px;TOP: 2px;  vertical-align:text-top; white-space:normal; ">
           <font  face="Wingdings" size="1px">w</font>
           Click <span onclick="return goBack();" onmouseover="this.style.cursor='pointer'; this.style.color='CornFlowerBlue';" 
            onmouseout="this.style.cursor='auto'; this.style.color='DarkBlue';"><font style=" text-decoration:underline;">Here</font></span> 
            to go back or close the window and try the operation again
          </span>
           </p> 
          <p>           
          <span style="  POSITION: relative;LEFT: 0px;TOP: -5px; vertical-align:text-top; white-space:normal; "
           onclick="return goHome();" onmouseover="this.style.cursor='pointer'; this.style.color='CornFlowerBlue';" 
           onmouseout="this.style.cursor='auto'; this.style.color='DarkBlue';">
           <font  face="Wingdings" size="1px">w</font>
           <font style=" text-decoration:underline;">Return to the Home page</font>
          </span> 
           </p>    
        </asp:Panel>
         </p> 
        <p>
         <asp:Label ID="lblSupport" runat="Server"
            Text="Support Information" 
            Font-Names="Arial"
            ForeColor="darkblue" 
            Font-Bold="True"            
            Font-Size="11" 	              
            CssClass="Label"
            style="  POSITION: relative;LEFT: 30px;TOP: -10px; Z-INDEX: 7; vertical-align:text-top; white-space:nowrap;  " 
            tabindex="-1"
            >
        </asp:Label>
        </p>
        <p>
         <asp:Label ID="lblSupportMsg" runat="Server"
            Text="If the error persists, please contact your System Administrator" 
            ForeColor="darkblue" 
            Font-Bold="False" 
            Font-Names="Arial"
            Font-Size="8"          	              
            CssClass="Label"
            style="  POSITION: relative;LEFT: 30px;TOP: -20px; width:320px; height:50px; Z-INDEX: 8; " 
            tabindex="-1"
             >
         </asp:Label> 
            
         <asp:Panel runat="server" 
          ID="panelSupport"
          ForeColor="darkblue" 
          Font-Bold="False" 
          Font-Names="Arial"
          Font-Size="8" 
          CssClass="Label"
          Visible="False"
          style="  POSITION: relative;LEFT: 30px;TOP: -20px;  Z-INDEX: 10; vertical-align:text-top; " 
          tabindex="-1">     
         
         <asp:Label ID="prompt_TSPhone" runat="Server"                
              Text="Phone:"  
              style="  POSITION: absolute;LEFT: 0px;TOP: 0px; width:50px; text-align:left;Z-INDEX: 11; " 
              tabindex="-1"
              >
         </asp:Label>
          <asp:Label ID="TSPhone" runat="Server"                           
              style="  POSITION: absolute;LEFT: 80px;TOP: 1px; width:700px; Z-INDEX: 12; " 
              tabindex="-1"
              >
         </asp:Label>
         <asp:Label ID="prompt_TSEmail" runat="Server"
              Text="E - mail:"    
              style="  POSITION: absolute;LEFT: 0px;TOP: 22px; width:50px;text-align:left; Z-INDEX: 13 " 
              tabindex="-1"
              >
         </asp:Label>  
         <asp:HyperLink ID="hlTSEmail" runat="Server"              
              Target="_blank"             
              style="  POSITION: absolute;LEFT:80px;TOP: 22px; width:700px; Z-INDEX: 14; " 
              tabindex="-1"
              >
         </asp:HyperLink>
         </asp:Panel>        
    </div>
    </form>
</body>
</html>
