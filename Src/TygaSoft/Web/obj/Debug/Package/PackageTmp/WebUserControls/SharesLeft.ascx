
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SharesLeft.ascx.cs" Inherits="TygaSoft.Web.WebUserControls.SharesLeft" %>

<div class="side-wrap">

    <div class="side-menu">
        <ul>
            <li id="customer" class=""><a class="order" href="/s/t">在途查询</a></li>
            <li id="order" class=""><a class="customer" href="javascript:void(0)" onclick="jsHelper.IsLogin('/u/t')">订单列表</a></li>
            <li id="message" class=""><a class="message" href="/s/y">公告</a></li>
            <li id="psw"><a class="customer" href="javascript:void(0)" onclick="jsHelper.IsLogin('/u/psw')">修改密码</a></li>
        </ul>
    </div>
</div>

<asp:Literal runat="server" ID="ltrUserInfo"></asp:Literal>