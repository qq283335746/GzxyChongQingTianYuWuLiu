<%@ Page Title="模板预览" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="ShowTemplate.aspx.cs" Inherits="TygaSoft.Web.Admin.Sms.ShowTemplate" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="server">

<div class="easyui-layout" data-options="fit:true">
    <div data-options="region:'west',title:'',split:true" style="width:210px;padding:10px 5px;">
        <div class="row">
            <span class="rl" style="width:60px;">订单号：</span>
            <div class="fl">
                <input type="text" id="txtOrderCode" class="txtm" />
            </div>
        </div>

        <div class="row mt10">
            <span class="rl" style="width:70px;">&nbsp;</span>
            <div class="fl">
                <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="currFun.Preview()">预 览</a>
            </div>
        </div>
    </div>
    <div data-options="region:'center',title:'短信内容预览'" style="padding:5px;">
        <table width="100%">
            <tr>
                <td style="border:1px solid #BBBBBB; border-right:none;">
                    <textarea id="txtaContent" name="txtaContent" runat="server" clientidmode="Static" rows="4" cols="80" style="width:98%; height:350px; border:0; line-height:20px;"></textarea>
                </td>
                <td valign="top" style="border:1px solid #BBBBBB; width:300px; padding:10px;">
                    <div>已选参数：</div>
                    <div id="divParam" runat="server" clientidmode="Static"></div>
                </td>
            </tr>
        </table>
    </div>
</div>

<asp:Literal runat="server" ID="ltrMyData"></asp:Literal>

<input type="hidden" id="hTemplateId" runat="server" clientidmode="Static" value="" />

<script type="text/javascript">
    $(function () {
        currFun.Init();
    })
    var currFun = {
        Init: function () {
            $("#cbbTranNode").combobox('loadData', currFun.GetMyData("myDataForTranNode"));
        },
        GetMyData: function (clientId) {
            var myData = $("#" + clientId + "").html();
            return eval("(" + myData + ")");
        },
        Preview: function () {
            var orderCode = $.trim($("#txtOrderCode").val());
            var templateId = $.trim($("#hTemplateId").val());
            $.ajax({
                url: "/ScriptServices/AdminService.asmx/PreviewTemplate",
                type: "post",
                data: '{orderCode:"' + orderCode + '",templateId:"' + templateId + '"}',
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
                        $("#txtaContent").val(content);
                    }
                }
            });
        }
    }
</script>

</asp:Content>
