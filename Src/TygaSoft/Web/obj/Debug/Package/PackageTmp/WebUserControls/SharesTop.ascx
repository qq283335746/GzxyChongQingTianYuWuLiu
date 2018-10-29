<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SharesTop.ascx.cs" Inherits="TygaSoft.Web.WebUserControls.SharesTop" %>
<div class="vt">
    <div class="w">
        <ul class="fl lh">
            <li>
                <a href="/">首 页</a>
            </li>
        </ul>
        <ul class="fr lh">
            <li> 
            <asp:LoginView ID="lvUser" runat="server">
                <AnonymousTemplate> 
                    <a href="/y.html" class="alkc">[注册]</a>
                    <a href="/t" class="alkc">[登录]</a>
                </AnonymousTemplate>
                <LoggedInTemplate>
                  <a href="/u">
                    <asp:LoginName ID="lnUser" runat="server" FormatString="您好，{0}" />
                  </a>
                    <asp:LoginStatus ID="lsUser" runat="server" LogoutText="[退出]" CssClass="ml10" />
                </LoggedInTemplate>
            </asp:LoginView> 
             </li>
        </ul>
        <span class="clr"></span>
    </div>
</div>
<div class="w">
  <ul>
      <li class="fl pdtb10">
          <a href="/"><img src="/Images/tyga.jpg" alt="天涯孤岸" /></a>
      </li>
      <li class="fl" id="searchBox">
          <div class="aa">
              <div class="ab">
                  <input id="txtKeyword" class="txt" />
                  <input type="button" value="搜索" class="btn" />
              </div>
          </div>
      </li>
  </ul>
  <span class="clr"></span>
</div>

<div style="display:none;"><asp:SiteMapPath ID="SitePaths" runat="server" ClientIDMode="Static" /></div>
<div id="nav" class="nav mb10">
  <div class="w">
    <a href="/" class="curr">首 页</a>
  </div>
</div>

<script type="text/javascript">
    $(function () {
        shareNav.Init();
    })
    function qq(value, name) {
        alert(value + ":" + name)
    }
</script>