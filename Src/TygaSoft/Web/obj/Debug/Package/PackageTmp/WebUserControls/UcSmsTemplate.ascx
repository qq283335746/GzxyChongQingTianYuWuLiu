<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UcSmsTemplate.ascx.cs" Inherits="TygaSoft.Web.WebUserControls.UcSmsTemplate" %>

<div id="toolbar" style="padding:5px;">
    
    标题： <input id="txtTitle" class="txt" />
    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="currFun.Search()">查 询</a>
</div>

<table id="dgUcSmsTemplate" class="easyui-datagrid" title="" data-options="singleSelect:true,rownumbers:true,pagination:true,fit:true,toolbar:'#toolbar'">
    <thead>
        <tr>
            <th data-options="field:'Id',checkbox:true"></th>
            <th data-options="field:'Title',width:700">标题</th>
            <th data-options="field:'TemplateType',width:80">模板类型</th>
            <th data-options="field:'IsDefault',width:80">是否默认</th>
        </tr>
    </thead>
</table>

<script type="text/javascript">
    $(function () {
        ucSmsTemplate.Grid(1, 10);
    })
    var ucSmsTemplate = {
        Grid: function (pageIndex, pageSize) {
            var dg = $('#dgUcSmsTemplate');
            $.ajax({
                url: "/ScriptServices/AdminService.asmx/GetJsonBySmgTemplate",
                type: "post",
                data: '{pageIndex:' + pageIndex + ',pageSize:' + pageSize + ',title:"' + $.trim($("#txtTitle").val()) + '"}',
                contentType: "application/json; charset=utf-8",
                beforeSend: function () {
                    $("#dlgWaiting").dialog('open');
                },
                complete: function () {
                    $("#dlgWaiting").dialog('close');
                },
                success: function (data) {
                    var json = data.d;
                    if (json.indexOf("异常") > -1) {
                        $.messager.alert('系統提示', msg, 'info');
                    }
                    else {
                        if (json == "") json = "[]";
                        json = eval("(" + json + ")");

                        dg.datagrid('loadData', json)
                    }
                }
            });
            var pager = dg.datagrid('getPager');
            $(pager).pagination({
                onSelectPage: function (pageNumber, pageSize) {
 
                    $.ajax({
                        url: "/ScriptServices/AdminService.asmx/GetJsonBySmgTemplate",
                        type: "post",
                        data: '{pageIndex:' + pageNumber + ',pageSize:' + pageSize + ',title:' + $.trim($("#txtTitle").val()) + '}',
                        contentType: "application/json; charset=utf-8",
                        beforeSend: function () {
                            $("#dlgWaiting").dialog('open');
                        },
                        complete: function () {
                            $("#dlgWaiting").dialog('close');
                        },
                        success: function (data) {
                            var json = data.d;
                            if (json.indexOf("异常") > -1) {
                                $.messager.alert('系統提示', msg, 'info');
                            }
                            else {
                                if (json == "") json = "[]";
                                json = eval("(" + json + ")");

                                dg.datagrid('loadData', json)
                            }
                        }
                    });
                }
            });
        }
    }
</script>

<asp:Literal runat="server" ID="ltrMyData"></asp:Literal>