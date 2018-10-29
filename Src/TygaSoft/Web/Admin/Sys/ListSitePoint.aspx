<%@ Page Title="积分设置" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="ListSitePoint.aspx.cs" Inherits="TygaSoft.Web.Admin.Sys.ListSitePoint" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="server">

<div id="toolbar">
积分名称：<input type="text" runat="server" id="txtName" maxlength="256" class="txt" />
<a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="currFun.Search()">查 询</a>
</div>

<table id="bindT" class="easyui-datagrid" title="积分设置列表" data-options="rownumbers:true,pagination:true,toolbar:'#toolbar'" style="height:auto;">
    <thead>
        <tr>
            <th data-options="field:'f0',checkbox:true"></th>
            <th data-options="field:'f1',sortable:true">积分名称</th>
            <th data-options="field:'f2',sortable:true">积分数</th>
            <th data-options="field:'f3',sortable:true">备注</th>
        </tr>
    </thead>
    <tbody>
<asp:Repeater ID="rpData" runat="server">
    <ItemTemplate>
        <tr>
            <td><%#Eval("Id")%></td>
            <td><a href='AddSitePoint.aspx?nId=<%#Eval("Id")%>'><%#Eval("PointName")%></a> </td>
                <td><%#Eval("PointNum")%></td>
            <td><%#Eval("Remark")%></td>
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

        $("#btnNew").click(function () {
            window.location = "AddSitePoint.aspx";
        })
        $("#abtnNew").click(function () {
            window.location = "AddSitePoint.aspx";
        })

        $("#abtnEdit").click(function () {
            var cbl = $('#bindT').datagrid("getSelections");
            if (cbl && cbl.length == 1) {
                window.location = "AddSitePoint.aspx?nId=" + cbl[0].f0 + "";
            }
            else {
                $.messager.alert('错误提醒', '请选择一行且仅一行进行编辑', 'error');
            }
        })

        $("#abtnDel").click(function () {
            var cbl = $('#bindT').datagrid("getSelections");
            if (!cbl || cbl.length == 0) {
                $.messager.alert('错误提醒', '请至少选择一行数据再进行操作', 'error');
                return false;
            }
            $.messager.confirm('温馨提醒', '确定要删除吗？', function (r) {
                if (r) {
                    var itemsAppend = "";
                    for (var i = 0; i < cbl.length; i++) {
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
                pageNumber: pageIndex,
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
            $("[id$=hOp]").val("OnSearch");
            $('#form1').submit();
        }
    } 
</script>

</asp:Content>
