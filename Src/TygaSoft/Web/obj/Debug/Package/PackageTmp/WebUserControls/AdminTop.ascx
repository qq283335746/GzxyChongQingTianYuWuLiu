<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminTop.ascx.cs" Inherits="TygaSoft.Web.WebUserControls.AdminTop" %>
<div class="vt">
    <div class="w">
        <ul class="fl lh">
            <li>
                后台管理系统-<a href="/">首 页</a>
            </li>
        </ul>
        <ul class="fr lh">
            <li> 
            <asp:LoginView ID="lvUser" runat="server">
                <AnonymousTemplate> 
                    <a href="/y.html" class="alkc">[注册]</a>
                    <a href="/t.html" class="alkc">[登录]</a>
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
<div class="w">
  <div id="logoBox"> 
      <span> 后台管理系统</span>
  </div>
</div>
<div style="display:none;"><asp:SiteMapPath ID="SitePaths" runat="server" ClientIDMode="Static" /></div>
<div id="nav" class="nav mb10">
  <div class="w">
    <a href="/" class="curr">首 页</a>
  </div>
</div>