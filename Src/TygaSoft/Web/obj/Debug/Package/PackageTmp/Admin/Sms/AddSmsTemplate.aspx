<%@ Page Title="新建/编辑短信模板" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="AddSmsTemplate.aspx.cs" Inherits="TygaSoft.Web.Admin.Sms.AddSmsTemplate" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="server">

<div class="easyui-panel" data-options="fit:true" style="padding:10px;">
    <div class="row">
        <span class="rl"><b class="cr">*</b>标题：</span>
        <div class="fl">
            <input type="text" id="txtTitle" runat="server" clientidmode="Static" class="txtl" style="width:700px;" />
        </div>
    </div>
    <div class="row mt10">
        <span class="rl">模板类型：</span>
        <div class="fl">
            <select id="cbbTemplateType" runat="server" clientidmode="Static" class="easyui-combobox" data-options="onSelect:currFun.CbbTemplateTypeSelect" style="width:200px;">
                <option>请选择</option>
                <option value="auto">自动</option>
                <option value="custom">自定义</option>
            </select>
        </div>
        <div class="fl" style="line-height:28px; margin-left:20px;">
            <input type="checkbox" id="cbIsDefault" runat="server" clientidmode="Static" value="0" />
            <label for="cbIsDefault">系统默认</label>
        </div>
    </div>
    <div class="row mt10">
        <span class="rl"><b class="cr">*</b>短信内容：</span>
        <div class="fl">
            <table width="100%">
                <tr>
                    <td style="border:1px solid #BBBBBB; border-right:none;">
                        <textarea id="txtaContent" runat="server" clientidmode="Static" rows="4" cols="80" style="width:300px; height:350px; border:0; line-height:20px;"></textarea>
                    </td>
                    <td valign="top" style="border:1px solid #BBBBBB; width:300px; padding:10px;">
                        <div>已选参数：</div>
                        <div id="divParam" runat="server" clientidmode="Static"></div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="row mt10">
        <span class="rl">&nbsp;</span>
        <div class="fl">
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="currFun.Save()">保 存</a>
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="currFun.Preview()">短信内容预览</a>
        </div>
    </div>
</div>

<input type="hidden" id="hId" runat="server" clientidmode="Static" value="" />
<asp:Literal runat="server" ID="ltrMyData"></asp:Literal>

<input type="hidden" id="hParamsCode" runat="server" clientidmode="Static" value="" />
<input type="hidden" id="hParamsName" runat="server" clientidmode="Static" value="" />
<input type="hidden" id="hParamsValue" runat="server" clientidmode="Static" value="" />

<div id="dlgTemplate" class="easyui-dialog" title="短信模板参数" data-options="closed:true,modal:true,
buttons: [{
	    text:'确定',iconCls:'icon-ok',
	    handler:function(){
		    currFun.DlgTemplateSave();
	    }
    },{
	    text:'取消',iconCls:'icon-cancel',
	    handler:function(){
		    $('#dlgTemplate').dialog('close');
	    }
    }]" style="width:780px;height:500px;padding:10px">

    <table id="dgTemplate" class="easyui-datagrid" title=""
            data-options="singleSelect: true, onClickCell: onClickCell" style="height:auto">
        <thead>
            <tr>
                <th data-options="field:'EnumCode',hidden:true"></th>
                <th data-options="field:'EnumValue',width:250">参数</th>
                <th data-options="field:'ParamsValue',width:250,editor:'text'">参数值</th>
                <th data-options="field:'status',width:60,align:'center',editor:{type:'checkbox',options:{on:'是',off:'否'}}">是否启用</th>
            </tr>
        </thead>
    </table>
 
    <script type="text/javascript">

        $.extend($.fn.datagrid.methods, {
            editCell: function (jq, param) {
                return jq.each(function () {
                    var opts = $(this).datagrid('options');
                    var fields = $(this).datagrid('getColumnFields', true).concat($(this).datagrid('getColumnFields'));
                    for (var i = 0; i < fields.length; i++) {
                        var col = $(this).datagrid('getColumnOption', fields[i]);
                        col.editor1 = col.editor;
                        if (fields[i] != param.field) {
                            col.editor = null;
                        }
                    }
                    $(this).datagrid('beginEdit', param.index);
                    for (var i = 0; i < fields.length; i++) {
                        var col = $(this).datagrid('getColumnOption', fields[i]);
                        col.editor = col.editor1;
                    }
                });
            }
        });

        var editIndex = undefined;
        function endEditing() {
            if (editIndex == undefined) { return true }
            if ($('#dgTemplate').datagrid('validateRow', editIndex)) {
                $('#dgTemplate').datagrid('endEdit', editIndex);
                editIndex = undefined;
                return true;
            } else {
                return false;
            }
        }
        function onClickCell(index, field) {
            if (endEditing()) {
                $('#dgTemplate').datagrid('selectRow', index)
                        .datagrid('editCell', { index: index, field: field });
                editIndex = index;
            }
        }

    </script>
    
</div>

<div id="dlgPreviewStepOne" class="easyui-dialog" title="自动模板类型需输入订单号" data-options="closed:true,modal:true,
buttons: [{
	    text:'确定',iconCls:'icon-ok',
	    handler:function(){
		    currFun.DlgPreviewStepOneSave();
	    }
    },{
	    text:'取消',iconCls:'icon-cancel',
	    handler:function(){
		    $('#dlgPreviewStepOne').dialog('close');
	    }
    }]" style="width:270px;height:130px;padding:10px">

    订单号：<input type="text" id="dlgOrderCode" value="" />

</div>

<div id="dlgPreview" style="padding:5px;"></div>

<script type="text/javascript">
    $(function () {
        currFun.Init();
    })
    var currFun = {
        Init: function () {
            var dgEditTempData = currFun.GetMyData("myDataForSmsParam");
            var templateType = $.trim($("#cbbTemplateType").combobox('getValue'));
            var paramsCode = $.trim($("#hParamsCode").val());
            var paramsName = $.trim($("#hParamsName").val());
            var paramsValue = $.trim($("#hParamsValue").val());
            if (templateType == "auto") {
                if (paramsCode != "") {
                    $.map(dgEditTempData, function (item) {
                        if (paramsCode.indexOf(item.EnumCode) > -1) {
                            item.status = "是";
                        }
                    })
                }
            }
            else if (templateType == "custom") {
                if (paramsValue != "") {
                    var paramsCodeArr = paramsCode.split(",");
                    var paramsValueArr = paramsValue.split(",");
                    $.map(dgEditTempData, function (item) {
                        for (var i = 0; i < paramsCodeArr.length; i++) {
                            if (paramsCodeArr[i] == item.EnumCode) {
                                item.ParamsValue = paramsValueArr[i];
                            }
                        }
                    })
                }
            }
            $("#dgTemplate").datagrid('loadData', dgEditTempData);
        },
        GetMyData: function (clientId) {
            var myData = $("#" + clientId + "").html();
            return eval("(" + myData + ")");
        },
        CbbTemplateTypeSelect: function (record) {
            var selectValue = $(this).combobox('getValue');

            switch (selectValue) {
                case "auto":
                    $("#dgTemplate").datagrid('hideColumn', 'ParamsValue');
                    $("#dgTemplate").datagrid('showColumn', 'status');
                    $("#dlgTemplate").dialog('open');
                    break;
                case "custom":
                    $("#dgTemplate").datagrid('hideColumn', 'status');
                    $("#dgTemplate").datagrid('showColumn', 'ParamsValue');
                    $("#dlgTemplate").dialog('open');
                    break;
                default:
                    $("#txtaContent").val("");
                    $("#divParam").html("");
                    break;
            }
        },
        DlgTemplateSave: function () {
            $("#dgTemplate").datagrid('acceptChanges');
            var templateType = $("#cbbTemplateType").combobox('getValue');
            var rows = $("#dgTemplate").datagrid('getRows');
            if (rows && rows.length > 0) {
                var paramsCode = "";
                var paramsName = "";
                var paramsValue = "";
                var sFormatAppend = "";
                var paramsAppend = "";
                var rowsLen = rows.length;
                var n = -1;
                for (var i = 0; i < rowsLen; i++) {
                    if (templateType == "auto") {
                        if (rows[i].status == "是") {
                            n++;
                            if (n > 0) {
                                sFormatAppend += "，";
                                paramsAppend += "，";
                                paramsCode += ",";
                                paramsName += ",";
                                paramsValue += ",";
                            }
                            sFormatAppend += "{" + n + "}";
                            paramsAppend += "{" + n + "}：" + rows[i].EnumValue;
                            paramsCode += rows[i].EnumCode;
                            paramsName += rows[i].EnumValue;
                        }
                    }
                    else if (templateType == "custom") {
                        var currParamsValue = $.trim(rows[i].ParamsValue);
                        if (currParamsValue != "") {
                            n++;
                            if (n > 0) {
                                sFormatAppend += "，";
                                paramsAppend += "，";
                                paramsCode += ",";
                                paramsName += ",";
                                paramsValue += ",";
                            }
                            sFormatAppend += "{" + n + "}";
                            paramsAppend += "{" + n + "}" + "：" + currParamsValue + "(" + rows[i].EnumValue + ")";
                            paramsCode += rows[i].EnumCode;
                            paramsName += rows[i].EnumValue;
                            paramsValue += currParamsValue;
                        }
                    }
                }

                $("#divParam").text(paramsAppend);
                $("#txtaContent").val(sFormatAppend);
                $("#hParamsCode").val(paramsCode);
                $("#hParamsName").val(paramsName);
                $("#hParamsValue").val(paramsValue);
            }

            $("#dlgTemplate").dialog('close');
        },
        Save: function () {
            var Id = $.trim($("#hId").val());
            var title = $.trim($("#txtTitle").val());
            if (title == "") {
                $.messager.alert('错误提示', '有“*”标识的为必填项，请检查！', 'error');
                return false;
            }
            var content = $.trim($("#txtaContent").val());
            if (content == "") {
                $.messager.alert('错误提示', '有“*”标识的为必填项，请检查！', 'error');
                return false;
            }
            var templateType = $("#cbbTemplateType").combobox('getValue');
            var isDefault = $("#cbIsDefault").is(":checked");
            var paramsCode = $.trim($("#hParamsCode").val());
            var paramsName = $.trim($("#hParamsName").val());
            var paramsValue = $.trim($("#hParamsValue").val());
            $.ajax({
                url: "/ScriptServices/AdminService.asmx/SaveSmgTemplate",
                type: "post",
                data: '{model:{Id:"' + Id + '",Title:"' + title + '",SmsContent:"' + content + '",TemplateType:"' + templateType + '",IsDefault:"' + isDefault + '",ParamsCode:"' + paramsCode + '",ParamsName:"' + paramsName + '",ParamsValue:"' + paramsValue + '"}}',
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
                        window.location = "/Admin/Sms/ListSmsTemplate.aspx";
                    }
                    else {
                        $.messager.alert('系統提示', msg, 'info');
                    }
                }
            });
        },
        Preview: function () {
            var templateType = $("#cbbTemplateType").combobox('getValue');
            var orderCode = "";
            var content = $.trim($("#txtaContent").val());
            if (content == "") {
                $.messager.alert('错误提示', '短信内容为空字符串，无法预览！', 'error');
                return false;
            }
            var templateType = $("#cbbTemplateType").combobox('getValue');
            var paramsCode = $.trim($("#hParamsCode").val());
            var paramsValue = $.trim($("#hParamsValue").val());
            if (templateType == "") {
                $('#dlgPreview').dialog({
                    title: '短信内容预览',
                    width: 500,
                    height: 400,
                    closed: false,
                    cache: false,
                    content: content,
                    modal: true
                });
            }
            else if (templateType == "auto") {
                $("#dlgPreviewStepOne").dialog('open');
            }
            else {
                
                $.ajax({
                    url: "/ScriptServices/AdminService.asmx/PreviewTemplate",
                    type: "post",
                    data: '{model:{OrderCode:"' + orderCode + '",SmsContent:"' + content + '",TemplateType:"' + templateType + '",ParamsCode:"' + paramsCode + '",ParamsValue:"' + paramsValue + '"}}',
                    contentType: "application/json; charset=utf-8",
                    beforeSend: function () {
                        $("#dlgWaiting").dialog('open');
                    },
                    complete: function () {
                        $("#dlgWaiting").dialog('close');
                    },
                    success: function (data) {
                        var content = data.d;
                        if (content.indexOf("异常") > -1) {
                            $.messager.alert('系統提示', content, 'info');
                        }
                        else {
                            $('#dlgPreview').dialog({
                                title: '短信内容预览',
                                width: 500,
                                height: 400,
                                closed: false,
                                cache: false,
                                content: content,
                                modal: true
                            });
                        }
                    }
                });
            }
        },
        DlgPreviewStepOneSave: function () {
            var orderCode = $.trim($('#dlgOrderCode').val());
            if (orderCode == "") {
                $.messager.alert('错误提示', "请输入订单号", 'error');
                return false;
            }
            var content = $.trim($("#txtaContent").val());
            if (content == "") {
                $.messager.alert('错误提示', '短信内容为空字符串，无法预览！', 'error');
                return false;
            }
            var templateType = $("#cbbTemplateType").combobox('getValue');
            var paramsCode = $.trim($("#hParamsCode").val());
            var paramsValue = $.trim($("#hParamsValue").val());

            $.ajax({
                url: "/ScriptServices/AdminService.asmx/PreviewTemplate",
                type: "post",
                data: '{model:{OrderCode:"' + orderCode + '",SmsContent:"' + content + '",TemplateType:"' + templateType + '",ParamsCode:"' + paramsCode + '",ParamsValue:"' + paramsValue + '"}}',
                contentType: "application/json; charset=utf-8",
                beforeSend: function () {
                    $("#dlgWaiting").dialog('open');
                },
                complete: function () {
                    $("#dlgWaiting").dialog('close');
                },
                success: function (data) {
                    var content = data.d;
                    if (content.indexOf("异常") > -1) {
                        $.messager.alert('系統提示', content, 'info');
                    }
                    else {
                        $('#dlgPreview').dialog({
                            title: '短信内容预览',
                            width: 500,
                            height: 400,
                            closed: false,
                            cache: false,
                            content: content,
                            modal: true
                        });
                        $("#dlgPreviewStepOne").dialog('close');
                    }
                }
            });
        }
    }
</script>

</asp:Content>
