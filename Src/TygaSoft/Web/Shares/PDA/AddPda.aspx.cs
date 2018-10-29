using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using TygaSoft.Model;
using TygaSoft.WebHelper;

namespace TygaSoft.Web.Shares.PDA
{
    public partial class AddPda : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindOpType();
            }
        }

        private void BindOpType()
        {
            List<string> list = new List<string>();
            list.Add("取货");
            list.Add("干线发运");
            list.Add("干线到达");
            list.Add("送货发运");
            list.Add("客户接收");

            ddlOpType.DataSource = list;
            ddlOpType.DataBind();
        }

        protected void btnCommit_Click(object sender, EventArgs e)
        {
            string opType = ddlOpType.SelectedItem.Text;
            string sBarCode = tbBarCode.Text.Trim();
            string sScanTime = tbScanTime.Text.Trim();
            string sUserId = tbUserId.Text.Trim();

            DateTime scanTime = DateTime.MinValue;
            if (!DateTime.TryParse(sScanTime, out scanTime))
            {
                MessageBox.Show(Page,btnCommit, "请输入正确日期格式的扫描时间");
                return;
            }

            scanTime = DateTime.Now;

            //long userId = 0;
            //if (!long.TryParse(sUserId, out userId))
            //{
            //    MessageBox.Show(Page, btnCommit, "用户ID不合法，请检查");
            //    return;
            //}

            string error = "";
            try
            {

                PDAOrderClient client = new PDAOrderClient();
                string str = client.Insert(opType, sBarCode, scanTime, sUserId);

                MessageBox.Show(Page, btnCommit, str);
                return;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(Page, btnCommit, error);
                return;
            }

            MessageBox.Show(Page, btnCommit, "操作成功");
        }

        protected void btnCommitBatch_Click(object sender, EventArgs e)
        {
            string error = "";

            //必须进行命名DataTable
            DataTable dt = new DataTable("dtTest");
            dt.Columns.Add("opType", typeof(string));
            dt.Columns.Add("barCode", typeof(string));
            dt.Columns.Add("scanTime", typeof(DateTime));
            dt.Columns.Add("userName", typeof(string));

            #region 添加数据行一

            DataRow dr = dt.NewRow();
            dr["opType"] = "取货";
            dr["barCode"] = "CP140906009";
            dr["scanTime"] = DateTime.Now;
            dr["userName"] = "迈兴";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["opType"] = "干线发运";
            dr["barCode"] = "CG140906001";
            dr["scanTime"] = DateTime.Now;
            dr["userName"] = "迈兴";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["opType"] = "干线到达";
            dr["barCode"] = "CG140906001";
            dr["scanTime"] = DateTime.Now;
            dr["userName"] = "迈兴";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["opType"] = "送货发运";
            dr["barCode"] = "CP140906009";
            dr["scanTime"] = DateTime.Now;
            dr["userName"] = "迈兴";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["opType"] = "客户接收";
            dr["barCode"] = "CP140906009";
            dr["scanTime"] = DateTime.Now;
            dr["userName"] = "迈兴";
            dt.Rows.Add(dr);

            #endregion

            #region 添加数据行二

            dr = dt.NewRow();
            dr["opType"] = "取货";
            dr["barCode"] = "CP140905061";
            dr["scanTime"] = DateTime.Now;
            dr["userName"] = "迈兴";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["opType"] = "干线发运";
            dr["barCode"] = "CG140906004";
            dr["scanTime"] = DateTime.Now;
            dr["userName"] = "迈兴";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["opType"] = "干线到达";
            dr["barCode"] = "CG140906004";
            dr["scanTime"] = DateTime.Now;
            dr["userName"] = "迈兴";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["opType"] = "送货发运";
            dr["barCode"] = "CP140905061";
            dr["scanTime"] = DateTime.Now;
            dr["userName"] = "迈兴";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["opType"] = "客户接收";
            dr["barCode"] = "CP140905061";
            dr["scanTime"] = DateTime.Now;
            dr["userName"] = "迈兴";
            dt.Rows.Add(dr);

            #endregion

            try
            {
                PDAOrderClient client = new PDAOrderClient();
                string str = client.InsertByBatch(dt);

                MessageBox.Show(Page, btnCommit, str);
                return;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(Page, btnCommit, error);
                return;
            }

            MessageBox.Show(Page, btnCommit, "操作成功");
        }
    }
}