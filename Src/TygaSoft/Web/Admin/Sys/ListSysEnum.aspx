<%@ Page Title="系统枚举项管理" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="ListSysEnum.aspx.cs" Inherits="TygaSoft.Web.Admin.Sys.ListSysEnum" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="server">

<div class="easyui-panel" title="系统枚举项" data-options="fit:true">
    <div class="mtb5">
       <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true" onclick="currFun.Add()">新建</a>
       <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-edit',plain:true" onclick="currFun.Edit()">编辑</a>
       <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-remove',plain:true" onclick="currFun.Del()">删除</a>
    </div>
   <ul id="treeCt" class="easyui-tree"></ul>
   <div id="mmTree" class="easyui-menu" style="width:120px;">
       <div onclick="currFun.Add()" data-options="iconCls:'icon-add'">添加</div>
       <div onclick="currFun.Edit()" data-options="iconCls:'icon-edit'">编辑</div>
       <div onclick="currFun.Del()" data-options="iconCls:'icon-remove'">删除</div>
   </div> 
</div>

<div id="dlg" class="easyui-dialog" title="新建/编辑系统枚举项" data-options="iconCls:'icon-save',closed:true,modal:true,
href:'/Templates/Admin/AddSysEnum.htm',buttons: [{
	    text:'确定',iconCls:'icon-ok',
	    handler:function(){
		    currFun.Save();
	    }
    },{
	    text:'取消',iconCls:'icon-cancel',
	    handler:function(){
		    $('#dlg').dialog('close');
	    }
    }]" style="width:485px;height:390px;padding:10px">

    
</div>

<script type="text/javascript">
    $(function () {
        $("#dlg").dialog('refresh', '/Templates/Admin/AddSysEnum.htm');
        currFun.Init();
    })

    var currFun = {
        Url: "",
        Init: function () {
            currFun.Load();
        },
        Load: function () {
            var t = $("#treeCt");

            $.ajax({
                url: "/ScriptServices/AdminService.asmx/GetJsonForSysEnum",
                type: "post",
                data: "{}",
                contentType: "application/json; charset=utf-8",
                success: function (json) {
                    var jsonData = (new Function("", "return " + json.d))();
                    t.tree({
                        data: jsonData,
                        animate: true,
                        onContextMenu: function (e, node) {
                            e.preventDefault();
                            $(this).tree('select', node.target);
                            $('#mmTree').menu('show', {
                                left: e.pageX,
                                top: e.pageY
                            });
                        }
                    })
                    currFun.OnCurrExpand(t);
                }
            });
        },
        Add: function () {
            currFun.Url = "/ScriptServices/AdminService.asmx/SaveSysEnum";
            var t = $("#treeCt");
            var node = t.tree('getSelected');
            if (!node) {
                $.messager.alert('错误提示', '请选中一个节点再进行操作', 'error');
                return false;
            }

            $("#dlg").dialog('open');
            dlgFun.Add(node);
        },
        Edit: function () {
            currFun.Url = "/ScriptServices/AdminService.asmx/SaveSysEnum";
            var t = $("#treeCt");
            var node = t.tree('getSelected');
            if (!node) {
                $.messager.alert('错误提示', '请选中一个节点再进行操作', 'error');
                return false;
            }
            $("#dlg").dialog('open');
            dlgFun.Edit(node, t);
        },
        Del: function () {
            var t = $("#treeCt");
            var node = t.tree('getSelected');

            if (!node) {
                $.messager.alert('错误提示', "请选中一个节点再进行操作", 'error');
                return false;
            }

            var childNodes = t.tree('getChildren', node.target);
            if (childNodes && childNodes.length > 0) {
                $.messager.alert('错误提示', "请先删除子节点再删除此节点", 'error');
                return false;
            }

            if (node) {
                $.messager.confirm('温馨提醒', '确定要删除吗?', function (r) {
                    if (r) {
                        var parentNode = t.tree('getParent', node.target);
                        if (parentNode) {
                            $("#hCurrExpandNode").val(parentNode.id);
                        }
                        $.ajax({
                            type: "POST",
                            url: "/ScriptServices/AdminService.asmx/DelSysEnum",
                            contentType: "application/json; charset=utf-8",
                            data: "{id:'" + node.id + "'}",
                            success: function (data) {
                                var msg = data.d;
                                if (msg == "1") {
                                    jeasyuiFun.show("温馨提醒", "保存成功！");
                                    currFun.Load();
                                    $('#dlg').dialog('close');
                                }
                                else {
                                    $.messager.alert('系统提示', msg, 'info');
                                }
                            }
                        })
                    }
                });
            }
        },
        Save: function () {
            var isValid = $('#dlgFm').form('validate');
            if (!isValid) return false;

            var sSort = $.trim($("#txtSort").val());
            if (sSort.length == 0) sSort = 0;

            $.ajax({
                url: currFun.Url,
                type: "post",
                data: '{sysEnumModel:{Id:"' + $("#hId").val() + '",EnumName:"' + $("#txtName").val() + '",EnumCode:"' + $("#txtCode").val() + '",EnumValue:"' + $("#txtValue").val() + '",ParentId:"' + $("#hParentId").val() + '",Sort:' + sSort + ',Remark:"' + $("#txtRemark").val() + '"}}',
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    var msg = data.d;
                    if (msg == "1") {
                        jeasyuiFun.show("温馨提示", "保存成功！");
                        currFun.Load();
                        $('#dlg').dialog('close');
                    }
                    else {
                        $.messager.alert('系统提示', msg, 'info');
                    }
                }
            });
        },
        OnCurrExpand: function (t) {
            var root = t.tree('getRoot');
            if (root) {
                var childNodes = t.tree('getChildren', root.target);
                if (childNodes && childNodes != undefined && (childNodes.length > 0)) {
                    var cnLen = childNodes.length;
                    for (var i = 0; i < cnLen; i++) {
                        t.tree('collapse', childNodes[i].target);
                    }
                }
            }
            var currNode = t.tree('find', $("#hCurrExpandNode").val());
            if (currNode) {
                currFun.OnExpand(t, currNode);
            }
        },
        OnExpand: function (t, node) {
            if (node) {
                t.tree('expand', node.target);
                var pNode = t.tree('getParent', node.target);
                if (pNode) {
                    currFun.OnExpand(t, pNode);
                }
            }
        }
    }
</script>

</asp:Content>
