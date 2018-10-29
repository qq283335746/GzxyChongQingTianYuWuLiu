<%@ Page Title="找回密码 天涯孤岸 国内个人网站首选" Language="C#" MasterPageFile="~/Shares/Shares.Master" AutoEventWireup="true" CodeBehind="PswReset.aspx.cs" Inherits="TygaSoft.Web.Shares.PswReset" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="server">

<div class="box">
    <div class="h pd5">找回密码</div>
    <div class="c pd10">

        <div class="row mt10">
            <span class="fl rl">电子邮箱：</span>
            <div class="fl">
                <input id="txtEmail" runat="server" tabindex="1" maxlength="50" class="easyui-validatebox txt" data-options="required:true,validType:'email'" />
            </div>
            <span class="clr"></span>
        </div>
        <div class="row mt10">
            <span class="fl rl">验证码：</span>
            <div class="fl">
                <input type="text" id="txtVc" runat="server" tabindex="2" style="width:50px;" class="easyui-validatebox txt" data-options="required:true,validType:'length[4,4]'" />
                <img border="0" id="imgCode" src="/Handlers/ValidateCode.ashx?vcType=pswReset" alt="看不清，单击换一张" onclick="this.src='/Handlers/ValidateCode.ashx?vcType=pswReset&abc='+Math.random()" style="vertical-align:middle;" />
            </div>
            <span class="clr"></span>
        </div>

        <div class="row mtb10">
            <span class="fl rl">&nbsp</span>
            <div class="fl">
                <a href="javascript:void(0)" class="easyui-linkbutton" onclick="onLogin()">提交</a>
            </div>
            <span class="clr"></span>
        </div>
    </div>
</div>

</asp:Content>
