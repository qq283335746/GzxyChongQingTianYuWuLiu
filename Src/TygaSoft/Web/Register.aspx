<%@ Page Title="注册会员" Language="C#" MasterPageFile="~/Shares/Shares.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="TygaSoft.Web.Register" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<script src="Scripts/JeasyuiExtend.js" type="text/javascript"></script>
<script src="Scripts/JeasyuiHelper.js" type="text/javascript"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="server">

<div id="register">
    <div class="box">
        <div class="h pd5">注册会员</div>
        <div class="c">
            <div class="fl pd10" style="border-right:1px solid #E9E9E9; width:360px;">

                <div class="row mt10">
                    <span class="fl rl"><b class="cr">*</b> 用户名：</span>
                    <div class="fl">
                        <input type="text" id="txtUserName" name="txtUserName" maxlength="50" tabindex="1" class="easyui-validatebox txt" data-options="required:true" />
                    </div>
                    <span class="clr"></span>
                </div>
                <div class="row mt10">
                    <span class="fl rl"><b class="cr">*</b>设置密码：</span>
                    <div class="fl"><input type="password" id="txtPsw" name="txtPsw" maxlength="50" tabindex="2" class="easyui-validatebox  txt" data-options="required:true,validType:'psw'" /></div>
                    <span class="clr"></span>
                </div>
                <div class="row mt10">
                    <span class="fl rl"><b class="cr">*</b>确认密码：</span>
                    <div class="fl"><input type="password" id="txtCfmPsw" maxlength="50" name="txtCfmPsw" tabindex="3" class="easyui-validatebox  txt" data-options="required:true" validType="cfmPsw['#txtPsw']" /></div>
                    <span class="clr"></span>
                </div>
                <div class="row mt10">
                    <span class="fl rl"><b class="cr">*</b>电子邮箱：</span>
                    <div class="fl">
                        <input type="text" id="txtEmail" name="txtEmail" maxlength="50" tabindex="4" class="easyui-validatebox txt" data-options="required:true,validType:'email'" />
                    </div>
                    <span class="clr"></span>
                </div>
                <div class="row mt10">
                    <span class="fl rl"><b class="cr">*</b>验证码：</span>
                    <div class="fl">
                        <input type="text" id="txtVc" name="txtVc" maxlength="4" tabindex="5" style="width:50px;" class="easyui-validatebox txt" data-options="required:true,validType:'length[4,4]'" />
                        <img border="0" id="imgCode" src="Handlers/ValidateCode.ashx?vcType=register" alt="看不清，单击换一张" onclick="this.src='Handlers/ValidateCode.ashx?vcType=register&abc='+Math.random()" style="vertical-align:middle;padding-right:5px;" />
                    </div>
                    <span class="clr"></span>
                </div>
                <div class="row mtb10">
                    <span class="fl rl">&nbsp</span>
                    <div class="fl"><a href="javascript:void(0)" class="easyui-linkbutton" onclick="onRegister()">提交</a></div>
                    <span class="clr"></span>
                </div>
            </div>
            <div class="fl pd10">
                <ul id="step">
                    <li class="aa">
                        <a id="currStep" class="icon_a">1</a>
                        <p> 填写注册会员信息：
                            QQ号码：请正确输入您的QQ号码；密码：登录本网站的密码，可任意设置；
                            请记住您的密码。
                        </p>
                    </li>
                    <li> <a class="icon_a">2</a>
                        <p>登录邮箱，查看邮件，并按照提示操作</p>
                    </li>
                    <li class="end"> <a class="icon_a">3</a>
                        <p>完成注册</p>
                    </li>
                </ul>
                
            </div>
            <span class="clr"></span>
        </div>
    </div>
</div>

<script type="text/javascript">
$(function () {

    $(document).bind("keydown", function (e) {
        var key = e.which;
        if (key == 13) {
            onRegister();
        }
    })

    //输入用户名鼠标离开事件
    $("#txtUserName").blur(function () {
            
        $.ajax({
            url: "/ScriptServices/SharesService.asmx/CheckUserName",
            type: "post",
            data: "{userName:'" + $("#txtUserName").val() + "'}",
            contentType: "application/json; charset=utf-8",
            success: function (msg) {
                msg = msg.d;
                if (msg == "1") {
                    $("#txtUserName").val("");
                    $.messager.alert('系统提示', "已存在相同用户，请换一个再重试", 'error');
                }
            }
        });
    })
})

function onRegister() {

    var isValid = $('#form1').form('validate');
    if (!isValid) return false;

    $.ajax({
        url: "/ScriptServices/SharesService.asmx/Register",
        type: "post",
        data: "{userName:'" + $("#txtUserName").val() + "',psw:'" + $("#txtPsw").val() + "',email:'" + $("#txtEmail").val() + "',sVc:'" + $("#txtVc").val() + "'}",
        contentType: "application/json; charset=utf-8",
        success: function (msg) {
            msg = msg.d;
            if (msg.indexOf("成功") > -1) {
                var currStep = $("#currStep");
                currStep.text("ok");
                currStep.parent().addClass("curr");
                jeasyuiFun.show("温馨提示", "注册成功，邮件已发送到您的邮箱，请尽快打开邮件查看并完成注册");
            }
            else {
                $.messager.alert('系统提示', msg, 'info');
            }
        }
    });

    return false;
}
</script>
</asp:Content>
