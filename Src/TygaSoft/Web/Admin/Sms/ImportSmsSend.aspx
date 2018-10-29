<%@ Page Title="导入发送短信" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="ImportSmsSend.aspx.cs" Inherits="TygaSoft.Web.Admin.Sms.ImportSmsSend" %>
<%@ Register src="../../WebUserControls/UcSmsTemplate.ascx" tagname="UcSmsTemplate" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<style type="text/css">
.row .rl{ width:75px;}
</style>
<link href="../../Scripts/plugins/uploadify/css/uploadify.css" rel="stylesheet" type="text/css" />
<script src="../../Scripts/plugins/uploadify/scripts/jquery.uploadify.min.js" type="text/javascript"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="server">

<div class="easyui-layout" data-options="fit:true" style="height:500px;">
    <div data-options="region:'east',title:'手机号码',collapsible:false" style="width:400px; padding:5px;">
        <textarea id="txtaMobile" rows="20" cols="50" style="line-height:20px; width:385px; height:458px;"></textarea>
    </div>
    <div data-options="region:'center',title:''" style="padding:5px; border-right:none;">
        <div class="row">
            <span class="rl">&nbsp;</span>
            <div class="fl">
                <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-redo',plain:true" onclick="currFun.DlgUpload()">导入手机号码</a>
            </div>
        </div>
        <div class="row mt10">
            <span class="rl">模板：</span>
            <div class="fl">
                <select id="cbbOpType" name="cbbOpType" class="easyui-combobox" data-options="onSelect:currFun.CbbOpTypeSelect" style="width:200px;">
                    <option value="all">请选择</option>
                    <option value="custom">自定义</option>
                </select>
            </div>
            <div class="fl ml10">
                <input type="checkbox" id="cbIsUseTemplate" onclick="currFun.CbUcTemplate(this)" />
                <label for="cbIsUseTemplate">已有模板中选择</label>
                <input type="hidden" id="hTemplateId" value="" />
            </div>
        </div>
        <div class="row mt10">
            <span class="rl"><b class="cr">*</b>短信内容：</span>
            <div class="fl">
                <table width="100%">
                    <tr>
                        <td style="border:1px solid #BBBBBB; border-right:none;">
                            <textarea id="txtaContent" name="txtaContent" rows="4" cols="80" style="width:300px; height:350px; border:0; line-height:20px;"></textarea>
                        </td>
                        <td valign="top" style="border:1px solid #BBBBBB; width:300px; padding:10px;">
                            <div>已选参数：</div>
                            <div id="divParam"></div>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div class="row mt10">
            <span class="rl">&nbsp;</span>
            <div class="fl">
                <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="currFun.Send()">发 送</a>
            </div>
        </div>
    </div>
</div>

<asp:Literal runat="server" ID="ltrMyData"></asp:Literal>

<input type="hidden" id="hParamsCode" value="" />
<input type="hidden" id="hParamsName" value="" />
<input type="hidden" id="hParamsValue" value="" />

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
            data-options="singleSelect: true, onClickCell: onClickCell,onLoadSuccess:dgLoadSuccess" style="height:auto">
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

        function dgLoadSuccess(data) {

            var tempType = $.trim($("#cbbOpType").combobox('getValue'));
            var paramsCode = $.trim($("#hParamsCode").val());
            var paramsValue = $.trim($("#hParamsValue").val());

            if (tempType != "") {
                if (tempType == "auto") {
                    if (paramsName != "") {
                        $.map(data, function (item) {
                            if (paramsCode.indexOf(item.EnumCode) > -1) {
                                item.status = "是";
                            }
                        })
                    }
                }
                else if (tempType == "custom") {
                    if (paramsValue != "") {
                        var paramCodeArr = paramsCode.split(",");
                        var paramValueArr = paramsValue.split(",");
                        var index = -1;
                        $.map(data, function (item) {
                            for (var i = 0; i < paramCodeArr.length; i++) {
                                if (paramCodeArr[i] == item.EnumCode) {
                                    item.ParamsValue = paramValueArr[i];
                                }
                            }
                        })
                    }
                }
            }
        }

    </script>
    
</div>

<div id="dlgUcTemplate" class="easyui-dialog" title="短信模板列表" data-options="closed:true,modal:true,
buttons: [{
	    text:'确定',iconCls:'icon-ok',
	    handler:function(){
		    currFun.DlgUcTemplateSave();
	    }
    },{
	    text:'取消',iconCls:'icon-cancel',
	    handler:function(){
		    $('#dlgUcTemplate').dialog('close');
            currFun.OnDlgUcTemplateClose();
	    }
    }]" style="width:960px;height:500px;padding:10px">

<uc1:UcSmsTemplate ID="UcSmsTemplate1" runat="server" />

</div>

<div id="dlgUpload" class="easyui-dialog" title="导入" data-options="closed:true,modal:true,
buttons: [{
	text:'确定',iconCls:'icon-ok',
	handler:function(){
		currFun.GetUploadData();
	}
},{
	text:'取消',iconCls:'icon-cancel',
	handler:function(){
		$('#dlgUpload').dialog('close');
	}
}]" style="width:960px;height:550px;padding:5px;">

    <div  class="easyui-layout" data-options="fit:true">
        <div data-options="region:'west',title:''" style="width:230px;padding:10px; border-right:none;">
            <div class="row" style="border-bottom:1px dotted #ECF3FF; padding-bottom:10px;">
                <span class="fl">下载模板：</span>
                <div class="fl">
                    <a href="../../FilesRoot/DownloadTemplate/短信手机号码导入模板.xlsx">短信手机号码导入模板</a>
                </div>
            </div>
            <input type="file" id="file_upload" name="file_upload" />
            <a href="javascript:$('#file_upload').uploadify('upload', '*');" class="easyui-linkbutton">上传</a>
        </div>
        <div data-options="region:'center',title:'已导入手机号码'" style="padding:5px;">
            <textarea id="txtaImportData" rows="10" cols="80" style=" height:425px; width:690px; line-height:20px;"></textarea>
        </div>
    </div>
 
</div>

<script type="text/javascript" src="../../Scripts/UploadifyFun.js"></script>

<script type="text/javascript">
    $(function () {
        //上传
        uploadifyFun.SmsPhoneUpload("file_upload");

        currFun.Init();
    })
    var currFun = {
        Init: function () {
            var dgEditTempData = currFun.GetMyData("myDataForSmsParam");
            $("#dgTemplate").datagrid('loadData', dgEditTempData);
        },
        GetMyData: function (clientId) {
            var myData = $("#" + clientId + "").html();
            return eval("(" + myData + ")");
        },
        CbbOpTypeSelect: function (record) {
            var selectValue = $(this).combobox('getValue');

            switch (selectValue) {
                case "auto":
                    $("#cbIsUseTemplate").removeAttr("checked");
                    $("#dgTemplate").datagrid('hideColumn', 'ParamsValue');
                    $("#dgTemplate").datagrid('showColumn', 'status');
                    $("#txtaContent").removeAttr('readonly');
                    $("#dlgTemplate").dialog('open');
                    break;
                case "custom":
                    $("#cbIsUseTemplate").removeAttr("checked");
                    $("#dgTemplate").datagrid('hideColumn', 'status');
                    $("#dgTemplate").datagrid('showColumn', 'ParamsValue');
                    $("#txtaContent").removeAttr('readonly');
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
            var templateType = $("#cbbOpType").combobox('getValue');
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
                                sFormatAppend += ",";
                                paramsAppend += ",";
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
                                sFormatAppend += ",";
                                paramsAppend += ",";
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
        OnDlgUcTemplateClose: function () {
            if ($("#cbIsUseTemplate").is(":checked")) {
                $("#cbIsUseTemplate").removeAttr("checked");
            }
        },
        DlgUcTemplateSave: function () {
            var dg = $("#dgUcSmsTemplate");
            var row = dg.datagrid('getSelected');
            if (!row) {
                $("#cbIsUseTemplate").removeAttr("checked");
                return false;
            }
            $("#txtaContent").val("已选择模板：" + row.Title);
            $("#hTemplateId").val(row.Id);
            $("#divParam").html("");
            $("#cbbOpType").combobox('setValue', "all");
            $('#dlgUcTemplate').dialog('close');
        },
        CbUcTemplate: function (h) {
            if ($(h).is(":checked")) {
                $("#txtaContent").attr("readonly", "readonly");
                $('#dlgUcTemplate').dialog('open');
            }
            else {
                $("#hTemplateId").val("");
                $("#txtaContent").val("");
                $("#txtaContent").removeAttr('readonly');
            }
        },
        CbbNumTypeSelect: function () {
            var selectValue = $(this).combobox('getValue');
            if (selectValue == "OrderCode") {
                $(this).parent().next().next().show();
            }
            else {
                $(this).parent().next().next().hide();
            }
        },
        Send: function () {
            var mobilePhone = $.trim($("#txtaMobile").val());            //手机号码
            var content = $.trim($("#txtaContent").val());               //内容
            var paramsCode = $.trim($("#hParamsCode").val());
            var paramsName = $.trim($("#hParamsName").val());
            var paramsValue = $.trim($("#hParamsValue").val());
            var templateId = $.trim($("#hTemplateId").val());

            $.ajax({
                url: "/ScriptServices/AdminService.asmx/SmsSendByImport",
                type: "post",
                data: '{model:{MobilePhone:"' + mobilePhone + '",SmsContent:"' + content + '",ParamsCode:"' + paramsCode + '",ParamsName:"' + paramsName + '",ParamsValue:"' + paramsValue + '",SmsTemplateId:"' + templateId + '"}}',
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
                        jeasyuiFun.show("温馨提示", "短信准备就绪，注意查收！");

                    }
                    else {
                        $.messager.alert('系統提示', msg, 'info');
                    }
                }
            });
        },
        DlgUpload: function () {
            $("#dlgUpload").dialog("open");
        },
        GetUploadData: function () {
            $("#txtaMobile").text($.trim($("#txtaImportData").text()));
            $('#dlgUpload').dialog('close');
        }
    }
</script>

</asp:Content>
