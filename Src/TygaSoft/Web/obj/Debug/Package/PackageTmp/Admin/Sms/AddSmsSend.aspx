<%@ Page Title="新建/编辑短信" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="AddSmsSend.aspx.cs" Inherits="TygaSoft.Web.Admin.Sms.AddSmsSend" %>
<%@ Register src="../../WebUserControls/UcSmsTemplate.ascx" tagname="UcSmsTemplate" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="server">

<div class="easyui-panel" data-options="fit:true" style="padding:10px;">
    <div class="row">
        <span class="rl"><b class="cr">*</b>号码类型：</span>
        <div class="fl">
            <select id="cbbNumType" name="cbbNumType" class="easyui-combobox" data-options="onSelect:currFun.CbbNumTypeSelect" style="width:200px;">
                <option value="MobilePhone">手机号码</option>
                <option value="OrderCode">订单号</option>
                <option value="CarScanCode">派车单号</option>
            </select>
        </div>
        <div class="fl ml10">
            <span class="rl" style="width:50px; line-height:20px;"><b class="cr">*</b>号码：</span>
            <div class="fl">
                <input type="text" id="txtNumCode" class="txt" style="height:12px; width:160px;" />
            </div>
        </div>
        <div class="fl" style="display:none;">
            <span class="rl">运输环节：</span>
            <select id="cbbTranNode" name="cbbTranNode" class="easyui-combobox" data-options="valueField:'id',textField:'text'" style="width:100px;"></select>
        </div>
    </div>
    <div class="row mt10">
        <span class="rl">模板类型：</span>
        <div class="fl">
            <select id="cbbTemplateType" name="cbbTemplateType" class="easyui-combobox" data-options="onSelect:currFun.CbbTemplateTypeSelect" style="width:200px;">
                <option value="all">请选择</option>
                <option value="auto">自动</option>
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
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="currFun.Preview()">预 览</a>
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

            var tempType = $.trim($("#cbbTemplateType").combobox('getValue'));
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

<div id="dlgPreview" style="width:500px;height:400px;padding:5px;"></div>

<script type="text/javascript">
    $(function () {
        currFun.Init();
    })
    var currFun = {
        Init: function () {
            var dgEditTempData = currFun.GetMyData("myDataForSmsParam");
            $("#dgTemplate").datagrid('loadData', dgEditTempData);

            $("#cbbTranNode").combobox('loadData', currFun.GetMyData("myDataForTranNode"));
        },
        GetMyData: function (clientId) {
            var myData = $("#" + clientId + "").html();
            return eval("(" + myData + ")");
        },
        CbbTemplateTypeSelect: function (record) {
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
            $("#cbbTemplateType").combobox('setValue', "all");
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
            var numberType = $("#cbbNumType").combobox('getValue');          //号码类型
            var numberCode = $.trim($("#txtNumCode").val());                 //号码类型对应的号码
            var tranNode = $("#cbbTranNode").combobox('getValue');           //运输环节值
            var tranNodeText = $("#cbbTranNode").combobox('getText');        //运输环节文本
            var content = $.trim($("#txtaContent").val());                   //内容
            var templateType = $("#cbbTemplateType").combobox('getValue');   //模板类型
            var paramsCode = $.trim($("#hParamsCode").val());
            var paramsName = $.trim($("#hParamsName").val());
            var paramsValue = $.trim($("#hParamsValue").val());
            var templateId = $.trim($("#hTemplateId").val());

            if (numberType == "MobilePhone") {
                if (templateType == "auto") {
                    $.messager.alert('错误提示', "号码类型为手机号时，模板类型不能是自动", 'error');
                    return false;
                }
            }

            if (content == "") {
                $.messager.alert('错误提示', "有“*”标识的为必填项，请检查！", 'error');
                return false;
            }

            $.ajax({
                url: "/ScriptServices/AdminService.asmx/SmsSend",
                type: "post",
                data: '{model:{NumberType:"' + numberType + '",NumberCode:"' + numberCode + '",TranNode:"' + tranNode + '",TranNodeText:"' + tranNodeText + '",SmsContent:"' + content + '",ParamsCode:"' + paramsCode + '",ParamsName:"' + paramsName + '",ParamsValue:"' + paramsValue + '",SmsTemplateId:"' + templateId + '",TemplateType:"' + templateType + '"}}',
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
        Preview: function () {
            var numberType = $("#cbbNumType").combobox('getValue');          //号码类型
            var numberCode = $.trim($("#txtNumCode").val());                 //号码类型对应的号码
            var tranNode = $("#cbbTranNode").combobox('getValue');           //运输环节值
            var tranNodeText = $("#cbbTranNode").combobox('getText');        //运输环节文本
            var content = $.trim($("#txtaContent").val());                   //内容
            var templateType = $("#cbbTemplateType").combobox('getValue');   //模板类型
            var paramsCode = $.trim($("#hParamsCode").val());
            var paramsName = $.trim($("#hParamsName").val());
            var paramsValue = $.trim($("#hParamsValue").val());
            var templateId = $.trim($("#hTemplateId").val());

            if (numberType == "MobilePhone") {
                if (templateType == "auto") {
                    $.messager.alert('错误提示', "号码类型为手机号时，模板类型不能是自动", 'error');
                    return false;
                }
            }
            if (content == "") {
                $.messager.alert('错误提示', "有“*”标识的为必填项，请检查！", 'error');
                return false;
            }

            $.ajax({
                url: "/ScriptServices/AdminService.asmx/PreviewSmsSend",
                type: "post",
                data: '{model:{NumberType:"' + numberType + '",NumberCode:"' + numberCode + '",TranNode:"' + tranNode + '",TranNodeText:"' + tranNodeText + '",SmsContent:"' + content + '",ParamsCode:"' + paramsCode + '",ParamsName:"' + paramsName + '",ParamsValue:"' + paramsValue + '",SmsTemplateId:"' + templateId + '",TemplateType:"' + templateType + '"}}',
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
                        $.messager.alert('系統提示', msg, 'info');
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
    }
</script>

</asp:Content>
