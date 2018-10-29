<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Top.ascx.cs" Inherits="TygaSoft.Web.WebUserControls.Top" %>
<div id="vt">
    <div class="w">
        <ul class="fl lh">
            <li class="fore1"><a href="/">首 页</a></li>
        </ul>
        <ul class="fr lh">
            <li id="loginbar" class="fore1 ld"> 
            <asp:LoginView ID="lvUser" runat="server">
                <AnonymousTemplate> 
                  <a href="/y.html" class="alkc">[注册]</a>
                  <a href="/t" class="alkc">[登录]</a>
                </AnonymousTemplate>
                <LoggedInTemplate>
                <asp:LoginName ID="lnUser" runat="server" FormatString="您好，{0}" />
                <asp:LoginStatus ID="lsUser" runat="server" LogoutText="[退出]" CssClass="alkc ml10" />
                </LoggedInTemplate>
            </asp:LoginView> 
             </li>
        </ul>
        <span class="clr"></span>
    </div>
</div>
