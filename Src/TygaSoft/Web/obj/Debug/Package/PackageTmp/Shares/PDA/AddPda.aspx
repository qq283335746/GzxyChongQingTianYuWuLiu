<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddPda.aspx.cs" Inherits="TygaSoft.Web.Shares.PDA.AddPda" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>PDA</title>
    <link href="/Styles/Main.css" rel="stylesheet" type="text/css" />
    <link href="/Scripts/plugins/jeasyui/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="/Scripts/plugins/jeasyui/themes/icon.css" rel="stylesheet" type="text/css" />
    <link href="/Styles/Admin.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/plugins/jeasyui/jquery.min.js" type="text/javascript"></script>
    <script src="/Scripts/plugins/jeasyui/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="/Scripts/plugins/jeasyui/locale/easyui-lang-zh_CN.js" type="text/javascript"></script>
</head>
<body>
    
    <div style="margin:20px;">PDA界面表单</div>

    <form id="form1" runat="server">
    <div style="padding:10px;">
        操作类型: <asp:DropDownList runat="server" ID="ddlOpType"></asp:DropDownList> <br /><br />
        单号条码：<asp:TextBox runat="server" ID="tbBarCode"></asp:TextBox> <br /><br />
        扫描时间: <asp:TextBox runat="server" ID="tbScanTime" CssClass="easyui-datebox"></asp:TextBox> <br /><br />
        用户名：<asp:TextBox runat="server" ID="tbUserId"></asp:TextBox> <br /><br />

        <asp:Button runat="server" ID="btnCommit" Text="提 交" 
            onclick="btnCommit_Click" />
    </div>
    <div style="margin:10px;">
        <asp:Button runat="server" ID="btnCommitBatch" Text="批量提交" 
            onclick="btnCommitBatch_Click" />
    </div>
    </form>
</body>
</html>
