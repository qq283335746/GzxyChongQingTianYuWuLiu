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
    public class TyUser
    {
        private static readonly ITyUser dal = DataAccess.CreateTyUser();

        #region 成员方法

        /// <summary>
        /// 添加数据到数据库
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(TyUserInfo model)
        {
            return dal.Insert(model);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(TyUserInfo model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除对应数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public int Delete(object Id)
        {
            return dal.Delete(Id);
        }

        /// <summary>
        /// 批量删除数据（启用事务
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool DeleteBatch(IList<object> list)
        {
            return dal.DeleteBatch(list);
        }

        /// <summary>
        /// 获取对应的数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public TyUserInfo GetModel(object Id)
        {
            return dal.GetModel(Id);
        }

        /// <summary>
        /// 获取当前用户相关信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public TyUserInfo GetModelByUser(string userName)
        {
            return dal.GetModelByUser(userName);
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
        public List<TyUserInfo> GetList(int pageIndex, int pageSize, out int totalCount, string sqlWhere, params SqlParameter[] cmdParms)
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
        public List<TyUserInfo> GetList(int pageIndex, int pageSize, string sqlWhere, params SqlParameter[] cmdParms)
        {
            return dal.GetList(pageIndex,pageSize,sqlWhere,cmdParms);
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <param name="sqlWhere"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public List<TyUserInfo> GetList(string sqlWhere, params SqlParameter[] cmdParms)
        {
            return dal.GetList(sqlWhere, cmdParms);
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        public List<TyUserInfo> GetList()
        {
            return dal.GetList();
        }

        /// <summary>
        /// 获取所有来自久用户表中的用户
        /// </summary>
        /// <returns></returns>
        public string[] GetTyUsers()
        {
            return dal.GetTyUsers();
        }

        /// <summary>
        /// 获取所有有效的用户信息
        /// </summary>
        /// <returns></returns>
        public DataSet GetAllUsers()
        {
            return dal.GetAllUsers();
        }

        /// <summary>
        /// 验证提供的用户名和密码是有效的。
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string ValidateUser(string username, string password)
        {
            return dal.ValidateUser(username, password);
        }

        /// <summary>
        /// 更新同步用户数据有效状态
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="isEnable"></param>
        /// <returns></returns>
        public int UpdateEnable(string userName, bool isEnable)
        {
            return dal.UpdateEnable(userName, isEnable);
        }

        #endregion
    }
}
