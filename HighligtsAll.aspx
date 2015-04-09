<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HighligtsAll.aspx.cs" Inherits="Raw_programs.WebForm1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .style1
        {
            text-decoration: none;
        }
        .style2
        {
            color: #FF0000;
            font-family: Calibri;
        }
    </style>
</head>
<body>
  
    <table>
    <tr>
    <td>
    <div>  
      <a href="output\<%=sv %>\MyHighlights.epub" class="style1">
     <div style="border-left : thin solid #00FF00; border-right: thin solid #00FF00; border-bottom: thin solid #00FF00; width: 488px; text-align: center; background-color: #99FF99; border-top:thick double #ff0000; height: 61px;">    <br/>  
         <span class="style2"><strong>Click here to download ePub file....</strong></span></div></a> 
      <br/>
      
    
     
    </div>
    </td>
    <td style=" text-align:center;"> If you liked this tool than please support it ,donation can help to pay for virtual private hosting which is $40/month <br/> (it's via Paypal)<br/><form action="https://www.paypal.com/cgi-bin/webscr" method="post">
<input type="hidden" name="cmd" value="_s-xclick">
<input type="hidden" name="encrypted" value="-----BEGIN PKCS7-----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-----END PKCS7-----
">
<input type="image" src="https://www.paypalobjects.com/en_AU/i/btn/btn_donateCC_LG.gif" border="0" name="submit" alt="PayPal — The safer, easier way to pay online.">
<img alt="" border="0" src="https://www.paypalobjects.com/en_AU/i/scr/pixel.gif" width="1" height="1">
</form>
</td>
    </tr>
    <tr><td colspan="2">  <br/>
        <asp:Label ID="Label1" runat="server"></asp:Label>        
        <asp:Label ID="Label2" runat="server" Text=""></asp:Label></td></tr>
    </table>
  
</body>
</html>
