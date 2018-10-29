using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using TygaSoft.IDAL;
using TygaSoft.Model;
using TygaSoft.DALFactory;

namespace TygaSoft.BLL
{
    public class Order
    {
        private static readonly IOrder dal = DataAccess.CreateOrder();

        #region 成员方法

        /// <summary>
        /// 获取数据分页列表，并返回所有记录数
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <param name="sqlWhere"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public List<OrderInfo> GetList(int pageIndex, int pageSize, out int totalCount, string sqlWhere, params SqlParameter[] cmdParms)
        {
            return dal.GetList(pageIndex, pageSize, out totalCount, sqlWhere, cmdParms);
        }

        /// <summary>
        /// 获取当前派车单号包含的所有订单号
        /// </summary>
        /// <param name="carCode"></param>
        /// <returns></returns>
        public string[] GetOrderByCarcode(string carCode)
        {
            return dal.GetOrderByCarcode(carCode);
        }

        #endregion
    }
}
