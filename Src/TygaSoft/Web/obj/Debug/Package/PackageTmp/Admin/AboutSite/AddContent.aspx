<%@ Page Title="添加/修改论坛内容详情" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="AddContent.aspx.cs" Inherits="TygaSoft.Web.Admin.AddContent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

  <link href="/Scripts/plugins/kindeditor/themes/default/default.css" rel="stylesheet" type="text/css" />
  <link href="/Scripts/plugins/kindeditor/plugins/code/prettify.css" rel="stylesheet" type="text/css" /> 
  <script type="text/javascript" src="/Scripts/plugins/kindeditor/kindeditor.js"></script>
  <script type="text/javascript" src="/Scripts/plugins/kindeditor/lang/zh_CN.js"></script>
  <script type="text/javascript" src="/Scripts/plugins/kindeditor/plugins/code/prettify.js"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="server">

<div class="box">
<div class="h pd5">填写信息</div>
<div class="c pd10">
   <div class="row mt10">
        <span class="fl rl"><b class="cr">*</b>标题：</span>
        <div class="fl">
            <input type="text" runat="server" id="txtTitle" class="easyui-validatebox txtl" data-options="required:true,missingMessage:'必填项'" />
        </div>
        <div class="clr"></div>
    </div>
   <div class="row mt10">
        <span class="fl rl"><b class="cr">*</b>所属类型：</span>
        <div class="fl">
            <input id="txtParent" runat="server" class="easyui-combotree" style="width:200px;" />
        </div>
        <div class="clr"></div>
    </div>
   <div class="row mtb10">
        <span class="fl rl">描述：</span>
        <div class="fl">
            <textarea id="editor1" cols="100" rows="8" style="width:700px;height:600px;"></textarea>
        </div>
        <div class="clr"></div>
    </div>

    <div class="row mt10">
        <span class="fl rl">&nbsp;</span>
        <div class="fl">
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-save'" onclick="currFun.Save()">提交</a>
        </div>
        <span class="clr"></span>
    </div>
</div>
</div>

<input type="hidden" runat="server" id="hEditor1" value="" />
<script type="text/javascript">
    var editor1;
    KindEditor.ready(function (K) {
        editor1 = K.create('#editor1', {
            uploadJson: '/Handlers/HandlerKindeditor.ashx',
            fileManagerJson: '/Handlers/HandlerKindeditor.ashx',
            allowFileManager: true,
            afterCreate: function () {
                var self = this;
                K.ctrl(document, 13, function () {
                });
                K.ctrl(self.edit.doc, 13, function () {
                });
            }
        });
        prettyPrint();

    });

    $(function () {
        var hEditor1 = $("[id$=hEditor1]");
        if (hEditor1.val().length > 0) {
            $("#editor1").html(hEditor1.val());
        }

        //所属类型
        $.ajax({
            url: "/ScriptServices/AdminService.asmx/GetJsonForContentType",
            type: "post",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                var jsonData = (new Function("", "return " + data.d))();
                $('[id$=txtParent]').combotree('loadData', jsonData);
            }
        });
    })

    var currFun = {

        Save: function () {
            var isValid = $('#form1').form('validate');
            if (!isValid) return false;

            $("[id$=hEditor1]").val(encodeURIComponent(editor1.html()));

            $('#form1').submit();
        }
    }
</script>

</asp:Content>
