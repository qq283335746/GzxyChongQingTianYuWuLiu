<%@ Page Title="成员管理" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="ListUsers.aspx.cs" Inherits="TygaSoft.Web.Admin.Members.ListUsers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="server">

<div class="easyui-layout" data-options="border:false" style="height:500px;">
    <div data-options="region:'east',title:'',border:false" data-options="border:false" style="width:230px; padding-left:10px;">
        <div id="toolbarRole" style="padding:5px;">
            将“<span id="lbUserName"></span>”添加到角色:
        </div>
        <table id="dgRole" class="easyui-datagrid" title="角色" data-options="rownumbers:true,singleSelect:true,toolbar:'#toolbarRole',onLoadSuccess:currFun.OnRoleLoadSuccess">
            <thead>
            <tr>
                <th data-options="hidden:true,field:'IsInRole'"></th>
                <th data-options="field:'RoleName',formatter:currFun.RoleFormatter"></th>
            </tr>
        </thead>
        </table>
    </div>
    <div data-options="region:'center',title:'',border:false">
        <div id="toolbar" style="padding:5px;">
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true" onclick="currFun.Add()">新建</a>
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-remove',plain:true" onclick="currFun.Del()">删除</a>
            用户名：<input type="text" runat="server" id="txtUserName" maxlength="50" class="txt" />
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="currFun.Search()">查 询</a>
        </div>

        <table id="bindT" class="easyui-datagrid" title="用户列表" data-options="rownumbers:true,pagination:true,singleSelect:true,toolbar:'#toolbar'">
        <thead>
            <tr>
            <th data-options="field:'f0',hidden:true"></th>
            <th data-options="field:'f1'">用户名</th>
            <th data-options="field:'f2',hidden:true">电子邮箱</th>
            <th data-options="field:'f3'">创建日期</th>
            <th data-options="field:'f4'">最后一次登录时间</th>
            <th data-options="field:'f5'">是否启用</th>
            <th data-options="field:'f6'">是否锁定</th>
            <th data-options="field:'f7'">角色</th>
            </tr>
        </thead>
        <tbody>
        <asp:Repeater ID="rpData" runat="server">
            <ItemTemplate>
                <tr>
                    <td><%#Eval("UserName")%></td>
                    <td><%#Eval("UserName")%></td><td><%#Eval("Email")%></td><td><%#Eval("CreationDate")%></td>
                    <td><%#Eval("LastLoginDate")%></td>
                    <td><a href="javascript:void(0)" onclick="currFun.OnIsApproved(this)" code='<%#Eval("UserName") %>'><%#Eval("IsApproved").ToString() == "False" ? "禁用" : "启用"%></a></td>
                    <td><a href="javascript:void(0)" onclick="currFun.OnIsLockedOut(this)" code='<%#Eval("UserName") %>'><%#Eval("IsLockedOut").ToString() == "1" ? "已锁定":"正常" %></a></td>
                    <td><a href="javascript:void(0)" onclick="currFun.EditRole('<%#Eval("UserName") %>')">编辑角色</a></td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
        </tbody>
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
        var myData = currFun.GetMyData($("#myDataForPage"));
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
        GetMyData: function (obj) {
            var myData = obj.html();
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
            window.location = "?name=" + $("[id$=txtUserName]").val() + "";
        },
        OnIsLockedOut: function (h) {
            try {
                var currObj = $(h);
                if (currObj.text() == "正常") {
                    return false;
                }
                $.messager.confirm('温馨提醒', '确定要执行此操作吗？', function (r) {
                    if (r) {
                        $.ajax({
                            url: "/ScriptServices/AdminService.asmx/SaveIsLockedOut",
                            type: "post",
                            data: "{userName:'" + currObj.attr("code") + "'}",
                            contentType: "application/json; charset=utf-8",
                            beforeSend: function () {
                                $("#dlgWaiting").dialog('open');
                            },
                            complete: function () {
                                $("#dlgWaiting").dialog('close');
                            },
                            success: function (msg) {
                                msg = msg.d;
                                if (msg == "1") {
                                    currObj.text("已锁定");
                                    jeasyuiFun.show("温馨提醒", "操作成功");
                                }
                                else if (msg == "0") {
                                    currObj.text("正常");
                                    jeasyuiFun.show("温馨提醒", "操作成功");
                                }
                                else {
                                    $.messager.alert('错误提醒', msg, 'error');
                                }
                            }
                        });
                    }
                })
            }
            catch (e) {
                $.messager.alert('错误提醒', e.name + ": " + e.message, 'error');
            }
        },
        OnIsApproved: function (h) {
            try {
                var currObj = $(h);
                $.messager.confirm('温馨提醒', '确定要执行此操作吗？', function (r) {
                    if (r) {
                        $.ajax({
                            url: "/ScriptServices/AdminService.asmx/SaveIsApproved",
                            type: "post",
                            data: "{userName:'" + currObj.attr("code") + "'}",
                            contentType: "application/json; charset=utf-8",
                            beforeSend: function () {
                                $("#dlgWaiting").dialog('open');
                            },
                            complete: function () {
                                $("#dlgWaiting").dialog('close');
                            },
                            success: function (msg) {
                                msg = msg.d;
                                if (msg == "1") {
                                    currObj.text("启用");
                                    jeasyuiFun.show("温馨提醒", "操作成功");
                                }
                                else if (msg == "0") {
                                    currObj.text("禁用");
                                    jeasyuiFun.show("温馨提醒", "操作成功");
                                }
                                else {
                                    $.messager.alert('错误提醒', msg, 'error');
                                }
                            }
                        });
                    }
                })
            }
            catch (e) {
                $.messager.alert('错误提醒', e.name + ": " + e.message, 'error');
            }
        },
        Add: function () {
            window.location = "AddUser.aspx";
        },
        Edit: function () {

        },
        Del: function () {
            try {
                var cbl = $('#bindT').datagrid("getSelections");
                if (!cbl || cbl.length != 1) {
                    $.messager.alert('错误提醒', '请选择一行且仅一行数据进行操作', 'error');
                    return false;
                }
                $.messager.confirm('温馨提醒', '确定要删除吗？', function (r) {
                    if (r) {
                        var userName = cbl[0].f0;
                        $.ajax({
                            url: "/ScriptServices/AdminService.asmx/DelUser",
                            type: "post",
                            contentType: "application/json; charset=utf-8",
                            data: '{userName:"' + userName + '"}',
                            beforeSend: function () {
                                $("#dlgWaiting").dialog('open');
                            },
                            complete: function () {
                                $("#dlgWaiting").dialog('close');
                            },
                            success: function (data) {
                                var msg = data.d;
                                if (msg != "1") {
                                    $.messager.alert('系统提示', msg, 'info');
                                    return false;
                                }
                                window.location.reload();
                            }
                        });
                    }
                });
            }
            catch (e) {
                $.messager.alert('错误提醒', e.name + ": " + e.message, 'error');
            }
        },
        Save: function () {

        },
        SaveRole: function () {

        },
        EditRole: function (userName) {
            $("#lbUserName").text(userName);
            currFun.BindRole();
        },
        BindRole: function () {
            $("#dgRole").datagrid('loadData', currFun.GetMyData($("#myDataForRole")));
        },
        RoleFormatter: function (value, row, index) {
            var isInRole = row.IsInRole == "True";
            if (isInRole) {
                return "<input type=\"checkbox\" checked=\"checked\" value=\"" + value + "\" onclick=\"currFun.CbIsInRole(this)\" />" + value;
            }
            return "<input type=\"checkbox\" value=\"" + value + "\" onclick=\"currFun.CbIsInRole(this)\" />" + value;
        },
        CbIsInRole: function (h) {
            try {
                var $_obj = $(h);
                var userName = $.trim($("#lbUserName").text());
                var roleName = $_obj.val();
                var isInRole = $_obj.is(":checked");

                $.ajax({
                    url: "/ScriptServices/AdminService.asmx/SaveUserInRole",
                    type: "post",
                    contentType: "application/json; charset=utf-8",
                    data: '{userName:"' + userName + '",roleName:"' + roleName + '",isInRole:"' + isInRole + '"}',
                    contentType: "application/json; charset=utf-8",
                    beforeSend: function () {
                        $("#dlgWaiting").dialog('open');
                    },
                    complete: function () {
                        $("#dlgWaiting").dialog('close');
                    },
                    success: function (data) {
                        var msg = data.d;
                        if (msg != "1") {
                            $.messager.alert('系统提示', msg, 'info');
                        }
                    }
                });
            }
            catch (e) {
                $.messager.alert('错误提醒', e.name + ": " + e.message, 'error');
            }
        },
        OnRoleLoadSuccess: function (data) {
            try {
                var userName = $("#lbUserName").text();
                var dg = $('#dgRole');
                var rows = dg.datagrid('getRows');
                if (rows && rows.length > 0) {
                    $.ajax({
                        url: "/ScriptServices/AdminService.asmx/GetUserInRole",
                        type: "post",
                        contentType: "application/json; charset=utf-8",
                        data: '{userName:"' + userName + '"}',
                        contentType: "application/json; charset=utf-8",
                        beforeSend: function () {
                            $("#dlgWaiting").dialog('open');
                        },
                        complete: function () {
                            $("#dlgWaiting").dialog('close');
                        },
                        success: function (data) {
                            var roles = data.d;
                            if (roles.indexOf("异常") > -1) {
                                $.messager.alert('系统提示', msg, 'info');
                            }
                            if (roles.length > 0) {
                                for (var i = 0; i < rows.length; i++) {
                                    if (roles.indexOf(rows[i].RoleName) > -1) {
                                        dg.datagrid('updateRow', {
                                            index: i,
                                            row: {
                                                IsInRole: 'True'
                                            }
                                        });
                                    }
                                }
                            }
                        }
                    });
                }
            }
            catch (e) {
                $.messager.alert('错误提醒', e.name + ": " + e.message, 'error');
            }
        }
    } 
</script>

</asp:Content>
