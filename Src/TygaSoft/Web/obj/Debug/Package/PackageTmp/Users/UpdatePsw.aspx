<%@ Page Title="修改密码" Language="C#" MasterPageFile="~/Users/Users.Master" AutoEventWireup="true" CodeBehind="UpdatePsw.aspx.cs" Inherits="TygaSoft.Web.Users.UpdatePsw" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="server">

<div class="bg_cw">

<div class="box">
  <div class="h pd5">修改密码</div>
  <div class="c pd10">
      <div class="row mt10">
        <span class="rl"><b class="cr">*</b>当前密码：</span>
        <div class="fl">
            <input type="password" runat="server" id="txtOldPsw" maxlength="50" class="easyui-validatebox txt" data-options="required:true,validType:'psw'" />
        </div>
        <span class="clr"></span>
      </div>
      <div class="row mt10">
        <span class="rl"><b class="cr">*</b>新密码：</span>
        <div class="fl">
            <input type="password" runat="server" id="txtNewPsw" clientidmode="Static" maxlength="50" class="easyui-validatebox txt" data-options="required:true,validType:'psw'" />
        </div>
        <span class="clr"></span>
      </div>
      <div class="row mt10">
        <span class="rl"><b class="cr">*</b>确认密码：</span>
        <div class="fl">
            <input type="password" runat="server" id="txtCfmPsw" maxlength="50" class="easyui-validatebox txt" data-options="required:true" validType="cfmPsw['#txtNewPsw']" />
        </div>
        <span class="clr"></span>
      </div>

      <div class="row mtb10">
        <span class="fl rl">&nbsp</span>
        <div class="fl"><a href="javascript:void(0)" class="easyui-linkbutton" onclick="currFun.Save()">提 交</a></div>
        <span class="clr"></span>
      </div>
  </div>
</div>

</div>

<script type="text/javascript">

    $(function () {
        $(document).bind("keydown", function (e) {
            var key = e.which;
            if (key == 13) {
                currFun.Save();
            }
        })
    })

    var currFun = {

        Save: function () {
            var isValid = $('#form1').form('validate');
            if (!isValid) return false;

            $('#form1').submit();
        }
    }
</script>

</asp:Content>
