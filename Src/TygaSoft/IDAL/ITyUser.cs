using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using TygaSoft.Model;

namespace TygaSoft.IDAL
{
    public interface ITyUser
    {
        #region 成员方法

        /// <summary>
        /// 添加数据到数据库
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        int Insert(TyUserInfo model);

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        int Update(TyUserInfo model);

        /// <summary>
        /// 删除对应数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        int Delete(object Id);

        /// <summary>
        /// 批量删除数据（启用事务
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        bool DeleteBatch(IList<object> list);

        /// <summary>
        /// 获取对应的数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        TyUserInfo GetModel(object Id);

        /// <summary>
        /// 获取当前用户相关信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        TyUserInfo GetModelByUser(string userName);

        /// <summary>
        /// 获取数据分页列表，并返回所有记录数
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <param name="sqlWhere"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        List<TyUserInfo> GetList(int pageIndex, int pageSize, out int totalCount, string sqlWhere, params SqlParameter[] cmdParms);

        /// <summary>
        /// 获取满足当前条件的数据列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sqlWhere"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        List<TyUserInfo> GetList(int pageIndex, int pageSize, string sqlWhere, params SqlParameter[] cmdParms);

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <param name="sqlWhere"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        List<TyUserInfo> GetList(string sqlWhere, params SqlParameter[] cmdParms);

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        List<TyUserInfo> GetList();

        /// <summary>
        /// 获取所有来自久用户表中的用户
        /// </summary>
        /// <returns></returns>
        string[] GetTyUsers();

        /// <summary>
        /// 获取所有有效的用户信息
        /// </summary>
        /// <returns></returns>
        DataSet GetAllUsers();

        /// <summary>
        /// 验证提供的用户名和密码是有效的。
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        string ValidateUser(string username, string password);

        /// <summary>
        /// 更新同步用户数据有效状态
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="isEnable"></param>
        /// <returns></returns>
        int UpdateEnable(string userName, bool isEnable);

        #endregion
    }
}
