<%@ Page Title="订单列表" Language="C#" MasterPageFile="~/Users/Users.Master" AutoEventWireup="true" CodeBehind="ListOrder.aspx.cs" Inherits="TygaSoft.Web.Users.Orders.ListOrder" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="server">

<div class="easyui-layout" style="height:530px; background:#FFF;">
    <div data-options="region:'center',title:'',border:false" style=" padding-right:10px;">
        <div id="toolbar" style="padding:5px;">
            订单号： <input id="txtOrderCode" runat="server" clientidmode="Static" class="txt" style="width:100px;" /> &nbsp;&nbsp;
            开始日期： <input id="txtStartDate" runat="server" clientidmode="Static" class="easyui-datebox" style="width:100px;" /> &nbsp;&nbsp;
            截止日期： <input id="txtEndDate"  runat="server" clientidmode="Static" class="easyui-datebox" style="width:100px;" />
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="currFun.Search()">查 询</a>
        </div>

        <table id="bindT" class="easyui-datagrid" title="" data-options="singleSelect:true,rownumbers:true,pagination:true,fitColumns:true,toolbar:'#toolbar',onClickRow:currFun.OnClickRow,height:$(document).height()*0.6">
            <thead>
                <tr>
                    <th data-options="field:'f0',hidden:true"></th>
                    <th data-options="field:'f1',width:80">日期</th>
                    <th data-options="field:'f2',width:100">订单号</th>
                    <th data-options="field:'f3',width:60">箱数</th>
                    <th data-options="field:'f4',width:200">发货单位</th>
                    <th data-options="field:'f5',width:200">收货单位</th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater ID="rpData" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td><%#Eval("OrderCode")%></td>
                            <td><%#DateTime.Parse(Eval("BusinessDate").ToString()).ToString("yyyy-MM-dd")%></td>
                            <td><%#Eval("OrderCode")%></td>
                            <td><%#Eval("TotalPackageCount")%></td>
                            <td><%#Eval("SenderName")%></td>
                            <td><%#Eval("ReceiverName")%></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </div>
    <div data-options="region:'east',title:'',border:false" style="width:430px;">
    
        <div id="childToolbar" style=" padding:10px;">
            <div style="text-align:center;margin:0 0 20px 0;">在途查询</div>
            订单号：<span id="lbCurrOrderCode"></span>
        </div>
        <table id="childT" class="easyui-datagrid" data-options="singleSelect:true,rownumbers:true,pagination:false,fitColumns:true,toolbar:'#childToolbar',height:$(document).height()*0.6">
            <thead>
                <tr>
                    <th data-options="field:'Remark',width:270">在途</th>
                    <th data-options="field:'ScanTime',width:120">时间</th>
                </tr>
            </thead>
        
        </table>
    
    </div>
</div>

<asp:Literal runat="server" ID="ltrMyData"></asp:Literal>

<script type="text/javascript">
    var sPageIndex = 0;
    var sPageSize = 0;
    var sTotalRecord = 0;
    var sQueryStr = "";

    $(function () {
        var myData = currFun.GetMyData("myDataForPage");
        $.map(myData, function (item) {
            sPageIndex = parseInt(item.PageIndex);
            sPageSize = parseInt(item.PageSize);
            sTotalRecord = parseInt(item.TotalRecord);
            sQueryStr = item.QueryStr.replace(/&amp;/g, '&');
        })

        currFun.Init();
    })

    var currFun = {
        Init: function () {
            currFun.Grid(sPageIndex, sPageSize);
        },
        GetMyData: function (clientId) {
            var myData = $("#" + clientId + "").html();
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
            var startDate = $.trim($("#txtStartDate").datebox('getValue'));
            var endDate = $.trim($("#txtEndDate").datebox('getValue'));
            var orderCode = $.trim($("#txtOrderCode").val());
            window.location = "?order=" + orderCode + "&startDate=" + startDate + "&endDate=" + endDate + "";
        },
        OnClickRow: function (rowIndex, rowData) {
            var orderCode = rowData.f0;
            $("#lbCurrOrderCode").text(orderCode);
            var myDataFor = $("#myDataFor" + orderCode + "");
            if (myDataFor != undefined && myDataFor.html() != undefined) {
                var jsonData = eval("(" + myDataFor.html() + ")");
                $("#childT").datagrid('loadData', jsonData);
            }
            else {
                $("#childT").datagrid('loadData', { "total": 0, "rows": [] });
            }
        }
    } 
</script>

</asp:Content>
