using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TygaSoft.Model;

namespace TygaSoft.IDAL
{
    public interface ICategory
    {
        #region 成员方法

        /// <summary>
        /// 添加数据到数据库
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        int Insert(CategoryInfo model);

        /// <summary>
        /// 更改数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        int Update(CategoryInfo model);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        int Delete(object Id);

        /// <summary>
        /// 获取对应的数据
        /// </summary>
        /// <param name="numberId"></param>
        /// <returns></returns>
        CategoryInfo GetModel(object Id);

        /// <summary>
        /// 批量删除数据（启用事务）
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        bool DeleteBatch(IList<object> list);

        /// <summary>
        /// 获取数据分页列表，并返回所有记录数
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <param name="sqlWhere"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        List<CategoryInfo> GetList(int pageIndex, int pageSize, out int totalCount, string sqlWhere, params SqlParameter[] cmdParms);

        /// <summary>
        /// 获取满足当前条件的数据列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sqlWhere"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        List<CategoryInfo> GetList(int pageIndex, int pageSize, string sqlWhere, params SqlParameter[] cmdParms);

        /// <summary>
        /// 获取所有分类数据集
        /// </summary>
        /// <returns></returns>
        List<CategoryInfo> GetList();

        /// <summary>
        /// 获取树json格式字符串
        /// </summary>
        /// <returns></returns>
        string GetTreeJson();

        #endregion
    }
}
