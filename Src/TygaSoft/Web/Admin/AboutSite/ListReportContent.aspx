<%@ Page Title="用户上报内容" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="ListReportContent.aspx.cs" Inherits="TygaSoft.Web.Admin.AboutSite.ListReportContent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="server">

<div id="toolbar" style="padding:5px;">
用户名：<input type="text" runat="server" id="txtName" maxlength="256" class="txt" />
<a id="abtnSearch" href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-search'">查 询</a>
</div>

<table id="bindT" class="easyui-datagrid" title="上报内容列表" data-options="rownumbers:true,pagination:true,toolbar:'#toolbar'" style="height:auto;">
<thead>
    <tr>
        <th data-options="field:'f0',checkbox:true"></th>
        <th data-options="field:'f1',sortable:true">用户名</th>
        <th data-options="field:'f2',sortable:true">来源</th>
        <th data-options="field:'f3',sortable:true">来源</th>
        <th data-options="field:'f4',sortable:true">类型</th>
        <th data-options="field:'f5',sortable:true">提供名词</th>
        <th data-options="field:'f6',sortable:true">日期时间</th>
    </tr>
</thead>
<tbody>
<asp:Repeater ID="rpData" runat="server">
    <ItemTemplate>
        <tr>
            <td><%#Eval("Id")%></td>
            <td><%#Eval("UserName")%></td>
            <td><%#Eval("FromUrl")%></td>
            <td><%#Eval("FromType")%></td>
            <td><%#Eval("GiveName")%></td>
            <td><%#Eval("FromDate")%></td>
        </tr>
    </ItemTemplate>
</asp:Repeater>
</tbody>
</table>

<input type="hidden" id="hOp" runat="server" />
<input type="hidden" id="hV" runat="server" />
<input type="hidden" id="hQuery" runat="server" />
<input type="hidden" id="hPageIndex" runat="server" />
<input type="hidden" id="hPageSize" runat="server" />
<input type="hidden" id="hTotal" runat="server" />

<script type="text/javascript">
    $(function () {

        currFun.Init();

        $("#abtnDel").click(function () {
            var cbl = $('#bindT').datagrid("getSelections");
            if (!cbl || cbl.length == 0) {
                $.messager.alert('错误提醒', '请至少选择一行数据再进行操作', 'error');
                return false;
            }
            $.messager.confirm('温馨提醒', '确定要删除吗？', function (r) {
                if (r) {
                    var itemsAppend = "";
                    var cbLen = cbl.length;
                    for (var i = 0; i < cbLen; i++) {
                        itemsAppend += cbl[i].f0 + ",";
                    }

                    $("[id$=hOp]").val("OnDel");
                    $("[id$=hV]").val(itemsAppend);

                    $('#form1').submit();
                }
            });
        })
    })

    var currFun = {
        Init: function () {
            currFun.Grid($("[id$=hPageIndex]").val(), $("[id$=hPageSize]").val());
        },
        Total: function () {
            return parseInt($("[id$=hTotal]").val());
        },
        Grid: function (pageIndex, pageSize) {
            var pager = $('#bindT').datagrid('getPager');
            $(pager).pagination({
                total: currFun.Total(),
                pageNumber: $("[id$=hPageIndex]").val(),
                pageSize: pageSize,  //每页显示的记录条数，默认为10  
                onSelectPage: function (pageNumber, pageSize) {
                    var urlQuery = $("[id$=hQuery]").val();
                    if (urlQuery.length == 0) {
                        window.location = "?pageIndex=" + pageNumber + "&pageSize=" + pageSize + "";
                    }
                    else {
                        window.location = "?" + $("[id$=hQuery]").val() + "&pageIndex=" + pageNumber + "&pageSize=" + pageSize + "";
                    }
                }
            });
        },
        Search: function () {
            $('#form1').submit();
        }
    }  
</script>

</asp:Content>
