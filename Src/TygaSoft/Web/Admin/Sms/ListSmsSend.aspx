<%@ Page Title="短信发送列表" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="ListSmsSend.aspx.cs" Inherits="TygaSoft.Web.Admin.Sms.ListSmsSend" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="server">

<div id="toolbar" style="padding:5px;">
    <a href="AddSmsSend.aspx" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true">新建短信</a>
    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-remove',plain:true" onclick="currFun.Del()">删除</a>
    <a href="ImportSmsSend.aspx" class="easyui-linkbutton" data-options="iconCls:'icon-redo',plain:true">导入发送</a>
    开始日期： <input id="txtStartDate" runat="server" clientidmode="Static" class="easyui-datebox" style="width:100px;" />
    截止日期： <input id="txtEndDate"  runat="server" clientidmode="Static" class="easyui-datebox" style="width:100px;" />
    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="currFun.Search()">查 询</a>
</div>

<table id="bindT" class="easyui-datagrid" title="" data-options="singleSelect:true,rownumbers:true,pagination:true,fitColumns:true,toolbar:'#toolbar'">
    <thead>
        <tr>
            <th data-options="field:'f0',checkbox:true"></th>
            <th data-options="field:'f1',width:80">手机号码</th>
            <th data-options="field:'f2',width:120">收信人</th>
            <th data-options="field:'f3',width:210">短信内容</th>
            <th data-options="field:'f4',width:110">发送时间</th>
            <th data-options="field:'f5',width:60">短信状态</th>
            <th data-options="field:'f6',width:80">订单号</th>
            <th data-options="field:'f7',width:80">派车单号</th>
            <th data-options="field:'f8',width:80">运输环节</th>
            <th data-options="field:'f9',width:80">委托客户</th>
        </tr>
    </thead>
    <tbody>
        <asp:Repeater ID="rpData" runat="server">
            <ItemTemplate>
                <tr>
                    <td><%#Eval("Id")%></td>
                    <td><%#Eval("MobilePhone")%></td>
                    <td><%#Eval("Receiver")%></td>
                    <td>
                        <%#Eval("SmsContent")%>
                    </td>
                    <td><%#DateTime.Parse(Eval("SendDate").ToString()).ToString("yyyy-MM-dd HH:mm")%></td>
                    <td><%#Eval("SendStatusText")%></td>
                    <td><%#Eval("OrderCode")%></td>
                    <td><%#Eval("CarScanCode")%></td>
                    <td><%#Eval("TranNode")%></td>
                    <td><%#Eval("Customer")%></td>

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
            var myData = $("#"+clientId+"").html();
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
            window.location = "?startDate=" + startDate + "&endDate=" + endDate + "";
        },
        Del: function () {
            var cbl = $('#bindT').datagrid("getSelections");
            if (!cbl || cbl.length == 0) {
                $.messager.alert('错误提示', '请至少选择一行数据再进行操作', 'error');
                return false;
            }
            $.messager.confirm('温馨提醒', '确定要删除吗？', function (r) {
                if (r) {
                    var itemsAppend = "";
                    for (var i = 0; i < cbl.length; i++) {
                        if (i > 0) itemsAppend += ",";
                        itemsAppend += cbl[i].f0;
                    }

                    $.ajax({
                        url: "/ScriptServices/AdminService.asmx/DelSmsSend",
                        type: "post",
                        data: '{itemAppend:"' + itemsAppend + '"}',
                        contentType: "application/json; charset=utf-8",
                        beforeSend: function () {
                            $("#dlgWaiting").dialog('open');
                        },
                        complete: function () {
                            $("#dlgWaiting").dialog('close');
                        },
                        success: function (data) {
                            var msg = data.d;
                            if (msg == "1") {
                                window.location.reload();
                            }
                            else {
                                $.messager.alert('系統提示', msg, 'info');
                            }
                        }
                    });
                }
            });
        }
    } 
</script>

</asp:Content>
