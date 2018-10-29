using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using TygaSoft.Model;

namespace TygaSoft.IDAL
{
    public interface IOrder
    {
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
        List<OrderInfo> GetList(int pageIndex, int pageSize, out int totalCount, string sqlWhere, params SqlParameter[] cmdParms);

        /// <summary>
        /// 获取当前派车单号包含的所有订单号
        /// </summary>
        /// <param name="carCode"></param>
        /// <returns></returns>
        string[] GetOrderByCarcode(string carCode);

        #endregion
    }
}
