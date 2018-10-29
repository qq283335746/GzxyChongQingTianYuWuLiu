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
    public class PDAOrderScanDetail
    {
        private static readonly IPDAOrderScanDetail dal = DataAccess.CreatePDAOrderScanDetail();

        #region 成员方法

        /// <summary>
        /// 添加数据到数据库
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(PDAOrderScanDetailInfo model)
        {
            return dal.Insert(model);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(PDAOrderScanDetailInfo model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 获取当前订单号对应的数据
        /// </summary>
        /// <param name="orderCode"></param>
        /// <returns></returns>
        public PDAOrderScanDetailInfo GetModel(string orderCode)
        {
            return dal.GetModel(orderCode);
        }

        /// <summary>
        /// 获取数据分页列表，并返回所有记录数
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <param name="sqlWhere"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public List<PDAOrderScanDetailInfo> GetList(int pageIndex, int pageSize, out int totalCount, string sqlWhere, params SqlParameter[] cmdParms)
        {
            return dal.GetList(pageIndex, pageSize, out totalCount, sqlWhere, cmdParms);
        }

        /// <summary>
        /// 获取满足当前条件的数据列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sqlWhere"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public List<PDAOrderScanDetailInfo> GetList(int pageIndex, int pageSize, string sqlWhere, params SqlParameter[] cmdParms)
        {
            return dal.GetList(pageIndex, pageSize, sqlWhere, cmdParms);
        }

        /// <summary>
        /// 获取满足当前条件的数据列表
        /// </summary>
        /// <param name="sqlWhere"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public List<PDAOrderScanDetailInfo> GetList(string sqlWhere, params SqlParameter[] cmdParms)
        {
            return dal.GetList(sqlWhere, cmdParms);
        }

        /// <summary>
        /// 获取当前订单的所有发收货扫描明细数据列表
        /// </summary>
        /// <param name="orderCode"></param>
        /// <returns></returns>
        public List<PDAOrderScanDetailInfo> GetList(string orderCode)
        {
            return dal.GetList(orderCode);
        }

        #endregion
    }
}
