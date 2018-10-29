<%@ Page Title="公告列表" Language="C#" MasterPageFile="~/Shares/Shares.Master" AutoEventWireup="true" CodeBehind="ListContent.aspx.cs" Inherits="TygaSoft.Web.Shares.AboutSite.ListContent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="server">

<div class="bg_cw">

<div id="companyInfoBox">
    <asp:Literal runat="server" ID="ltrCompanyInfo"></asp:Literal>
</div>

<table id="bindT" class="easyui-datagrid" title="公告" data-options="rownumbers:false,pagination:true,fitColumns:true,singleSelect:true,height:$(document).height()*0.8">
<thead>
    <tr>
        <th data-options="field:'f0',hidden:true"></th>
        <th data-options="field:'f1',width:400">标题</th>
        <th data-options="field:'f2',width:100">日期</th>
    </tr>
</thead>
<tbody>
<asp:Repeater ID="rpData" runat="server">
    <ItemTemplate>
        <tr>
            <td><%#Eval("Id")%></td>
            <td><a href='/s/g?nId=<%#Eval("Id")%>'><%#Eval("Title")%></a> </td>
            <td><%# DateTime.Parse(Eval("LastUpdatedDate").ToString()).ToString("yyyy-MM-dd")%></td>
        </tr>
    </ItemTemplate>
</asp:Repeater>
</tbody>
</table>

</div>

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
            sQueryStr = item.QueryStr;
        })

        //currFun.Init();
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

            $("#form1").submit();
        }
    } 
</script>

</asp:Content>
