<%@ OutputCache Duration="43200" VaryByParam="None" Shared="true" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserLeft.ascx.cs" Inherits="TygaSoft.Web.WebUserControls.UserLeft" %>

<div class="side-wrap">

    <div class="side-menu">
        <ul>
            <li id="customer" class=""><a class="order" href="/s/t">在途查询</a></li>
            <li id="order" class=""><a class="customer" href="/u/t">订单列表</a></li>
            <li id="message" class=""><a class="message" href="/s/y">公告</a></li>
            <li><a class="customer" href="/u/psw">修改密码</a></li>
        </ul>
    </div>
</div>