<%@ Page Title="新建/编辑积分设置" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="AddSitePoint.aspx.cs" Inherits="TygaSoft.Web.Admin.Sys.AddSitePoint" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="server">

<div class="box">
  <div class="h pd5">填写信息</div>
  <div class="c pd10">
      <div class="row">
          <span class="fl rl">名称：</span>
          <div class="fl ml10">
              <input type="text" id="txtName" runat="server" maxlength="50" tabindex="1" class="easyui-validatebox txt" data-options="required:true" />
              <span class="cr">*</span>
          </div>
      </div>
      <div class="row mt10">
          <span class="fl rl">积分数：</span>
          <div class="fl ml10">
              <input type="text" id="txtPointNum" runat="server" maxlength="50" tabindex="2" class="easyui-validatebox txt" data-options="required:true,validType:'number'" />
              <span class="cr">*</span>
          </div>
      </div>
      <div class="row mt10">
          <span class="fl rl">备注：</span>
          <div class="fl ml10">
              <textarea id="txtRemark" runat="server" rows="5" cols="80" class="easyui-validatebox txtarea" data-options="validType:'length[0,300]'" tabindex="3"></textarea>
          </div>
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

<script type="text/javascript">
    var currFun = {

        Save: function () {
            var isValid = $('#form1').form('validate');
            if (!isValid) return false;

            $('#form1').submit();
        }
    }
</script>

</asp:Content>
