<%@ Page Title="在途查询" Language="C#" MasterPageFile="~/Shares/Shares.Master" AutoEventWireup="true" CodeBehind="SearchOrder.aspx.cs" Inherits="TygaSoft.Web.Shares.Orders.SearchOrder" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="server">

<div class="bg_cw">
<div id="toolbar" style="padding:5px;">
    订单号： <input type="text" id="txtOrderCode" runat="server" clientidmode="Static" class="txt" />
    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="currFun.Search()">查 询</a>
</div>

<table id="bindT" class="easyui-datagrid" title="" data-options="singleSelect:true,rownumbers:true,pagination:false,fitColumns:true,toolbar:'#toolbar',height:$(document).height()*0.8">
    <thead>
        <tr>
            <th data-options="field:'f1',width:300">在途</th>
            <th data-options="field:'f2',width:120">时间</th>
        </tr>
    </thead>
    <tbody>
        <asp:Repeater ID="rpData" runat="server">
            <ItemTemplate>
                <tr>
                    <td><%#Eval("Remark")%></td>
                    <td><%# DateTime.Parse(Eval("ScanTime").ToString()).ToString("yyyy-MM-dd HH:mm")%></td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </tbody>
</table>

</div>

<asp:Literal runat="server" ID="ltrMyData"></asp:Literal>

<script type="text/javascript">
//    var sPageIndex = 0;
//    var sPageSize = 0;
//    var sTotalRecord = 0;
//    var sQueryStr = "";

    $(function () {
//        var myData = currFun.GetMyData();
//        $.map(myData, function (item) {
//            sPageIndex = parseInt(item.PageIndex);
//            sPageSize = parseInt(item.PageSize);
//            sTotalRecord = parseInt(item.TotalRecord);
//            sQueryStr = item.QueryStr.replace(/&amp;/g, '&');
//        })

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
            var sOrder = $.trim($("#txtOrderCode").val());
            if (sOrder == "") {
                $.messager.alert('错误提示', '请输入订单号', 'error');
                return false;
            }

            window.location = "?order=" + sOrder + "";
        }
    } 
</script>

</asp:Content>
