<%@ Page Title="同步用户登录信息" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="ListTyUser.aspx.cs" Inherits="TygaSoft.Web.Admin.Sys.ListTyUser" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="server">

<div id="toolbar" style="padding:5px;">
    用户名： <input type="text" id="txtUserName" runat="server" clientidmode="Static" class="txt" />
    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="currFun.Search()">查 询</a>
</div>

<table id="bindT" class="easyui-datagrid" title="同步用户列表" data-options="rownumbers:true,pagination:true,singleSelect:true,height:$(document).height()*0.68">
<thead>
    <tr>
        <th data-options="field:'f0',hidden:true"></th>
        <th data-options="field:'f1'">用户</th>
        <th data-options="field:'f2'">密码</th>
        <th data-options="field:'f3'">密码状态</th>
    </tr>
</thead>
<tbody>
<asp:Repeater ID="rpData" runat="server">
    <ItemTemplate>
        <tr>
            <td><%#Eval("UserName")%></td>
            <td><%#Eval("UserName")%> </td>
            <td><%#Eval("Password")%> </td>
            <td><%#bool.Parse(Eval("IsEnable").ToString()) == true ? "有效，未修改" : "无效，已修改"%> </td>
            <td><%# DateTime.Parse(Eval("LastUpdatedDate").ToString()).ToString("yyyy-MM-dd")%></td>
        </tr>
    </ItemTemplate>
</asp:Repeater>
</tbody>
</table>

<asp:Literal runat="server" ID="ltrMyData"></asp:Literal>

<script type="text/javascript">
    var sPageIndex = 0;
    var sPageSize = 0;
    var sTotalRecord = 0;
    var sQueryStr = "";

    $(function () {
        var myData = currFun.GetMyData();
        $.map(myData, function (item) {
            sPageIndex = parseInt(item.PageIndex);
            sPageSize = parseInt(item.PageSize);
            sTotalRecord = parseInt(item.TotalRecord);
            sQueryStr = item.QueryStr.replace(/&amp;/g, '&'); ;
        })

        currFun.Init();
    })

    var currFun = {
        Init: function () {
            currFun.Grid(sPageIndex, sPageSize);
        },
        GetMyData: function () {
            var myData = $("#myDataForPage").html();
            return eval("(" + myData + ")");
        },
        Grid: function (pageIndex, pageSize) {
            var pager = $('#bindT').datagrid('getPager');
            $(pager).pagination({
                total: sTotalRecord,
                pageNumber: sPageIndex,
                pageSize: sPageSize,
                onSelectPage: function (pageNumber, pageSize) {
                    if (sQueryStr.length == 0) {
                        window.location = "?pageIndex=" + pageNumber + "&pageSize=" + pageSize + "";
                    }
                    else {
                        window.location = "?" + sQueryStr + "&pageIndex=" + pageNumber + "&pageSize=" + pageSize + "";
                    }
                }
            });
        },
        Search: function () {

            window.location = "?userName=" + $.trim($("#txtUserName").val()) + "";
        }
    } 
</script>

</asp:Content>
