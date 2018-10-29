<%@ Page Title="短信模板列表" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="ListSmsTemplate.aspx.cs" Inherits="TygaSoft.Web.Admin.Sms.ListSmsTemplate" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="server">

<div id="toolbar" style="padding:5px;">
    <a href="AddSmsTemplate.aspx" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true" onclick="currFun.Add()">新建</a>
    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-edit',plain:true" onclick="currFun.Edit()">编辑</a>
    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-remove',plain:true" onclick="currFun.Del()">删除</a>
    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-save',plain:true" onclick="currFun.SetDefault()">设为默认</a>
    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-ok',plain:true" onclick="currFun.Preview()">预览</a>
    标题： <input id="txtTitle" runat="server" clientidmode="Static" class="txt" /> &nbsp;&nbsp;
    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="currFun.Search()">查 询</a>
</div>

<table id="bindT" class="easyui-datagrid" title="" data-options="singleSelect:false,rownumbers:true,pagination:true,fitColumns:true,toolbar:'#toolbar'">
    <thead>
        <tr>
            <th data-options="field:'f0',checkbox:true"></th>
            <th data-options="field:'f1',width:150">标题</th>
            <th data-options="field:'f2',width:200">内容</th>
            <th data-options="field:'f3',width:200">参数集</th>
            <th data-options="field:'f4',width:60">模板类型</th>
            <th data-options="field:'f5',width:80">是否系统默认</th>
            <th data-options="field:'f6',width:100">最后更新时间</th>
        </tr>
    </thead>
    <tbody>
        <asp:Repeater ID="rpData" runat="server">
            <ItemTemplate>
                <tr>
                    <td><%#Eval("Id")%></td>
                    <td>
                        <%#Eval("Title").ToString().Length > 15 ? Eval("Title").ToString().Substring(0, 15) + "..." : Eval("Title")%>
                    </td>
                    <td>
                        <%#Eval("SmsContent")%>
                    </td>
                    <td>
                        <%#Eval("ParamsName")%>
                    </td>
                    <td><%#Eval("TemplateType").ToString() == "auto" ? "自动" : "自定义"%></td>
                    <td><%#Eval("IsDefault").ToString() == "True" ? "是" : "否"%></td>
                    <td><%#DateTime.Parse(Eval("LastUpdatedDate").ToString()).ToString("yyyy-MM-dd HH:mm")%></td>
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

            var title = $.trim($("#txtTitle").val());
            window.location = "?title=" + title + "";
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
        },
        SetDefault: function () {
            var dg = $('#bindT');
            var rows = dg.datagrid("getSelections");
            if (!rows || rows.length != 1) {
                $.messager.alert('错误提醒', '请选择一行且仅一行数据进行操作', 'error');
                return false;
            }
            var row = rows[0];
            var rowIndex = dg.datagrid('getRowIndex', row);
            $.ajax({
                url: "/ScriptServices/AdminService.asmx/SetDefaultSmgTemplate",
                type: "post",
                data: '{Id:"' + row.f0 + '"}',
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
                        jeasyuiFun.show("温馨提示", "保存成功！");
                        window.location.reload();
                    }
                    else {
                        $.messager.alert('系統提示', msg, 'info');
                    }
                }
            });
        },
        Del: function () {
            var cbl = $('#bindT').datagrid("getSelections");
            if (!cbl || cbl.length == 0) {
                $.messager.alert('错误提示', '请至少选择一行数据再进行操作', 'error');
                return false;
            }
            $.messager.confirm('温馨提醒', '确定要删除吗？', function (r) {
                if (r) {
                    var isError = false;
                    var itemsAppend = "";
                    for (var i = 0; i < cbl.length; i++) {
                        if (i > 0) itemsAppend += ",";
                        itemsAppend += cbl[i].f0;
                        if (cbl[i].f5 == "是") {
                            isError = true;
                        }
                    }

                    if (isError) {
                        var myDataForUserInfo = currFun.GetMyData("myDataForUserInfo");
                        var isSysDataAdmin = false;
                        $.map(myDataForUserInfo, function (item) {
                            if (item.SysDataAdmin == "1") isSysDataAdmin = true;
                        })
                        if (!isSysDataAdmin) {
                            $.messager.alert('错误提示', '选择的数据行中包含系统默认行，不能删除', 'error');
                            return false;
                        }
                    }

                    $.ajax({
                        url: "/ScriptServices/AdminService.asmx/DelSmgTemplate",
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
        },
        Edit: function () {
            var cbl = $('#bindT').datagrid("getSelections");
            if (cbl && cbl.length == 1) {
                window.location = "AddSmsTemplate.aspx?nId=" + cbl[0].f0 + "";
            }
            else {
                $.messager.alert('错误提醒', '请选择一行且仅一行进行编辑', 'error');
            }
        },
        Preview: function () {
            var rows = $('#bindT').datagrid("getSelections");
            if (rows && rows.length == 1) {
                window.location = "ShowTemplate.aspx?nId=" + rows[0].f0 + "";
            }
            else {
                $.messager.alert('错误提醒', '请选择一行且仅一行进行预览', 'error');
            }
        }
    } 
</script>

</asp:Content>
