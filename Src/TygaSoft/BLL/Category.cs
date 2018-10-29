using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using TygaSoft.IDAL;
using TygaSoft.Model;
using TygaSoft.DALFactory;

namespace TygaSoft.BLL
{
    public class Category
    {
        private static readonly ICategory dal = DataAccess.CreateCategory();

        #region 成员方法

        /// <summary>
        /// 添加数据到数据库
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(CategoryInfo model)
        {
            return dal.Insert(model);
        }

        /// <summary>
        /// 更改数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(CategoryInfo model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public int Delete(object Id)
        {
            return dal.Delete(Id);
        }

        /// <summary>
        /// 获取对应的数据
        /// </summary>
        /// <param name="numberId"></param>
        /// <returns></returns>
        public CategoryInfo GetModel(object Id)
        {
            return dal.GetModel(Id);
        }

        /// <summary>
        /// 批量删除数据（启用事务）
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool DeleteBatch(IList<object> list)
        {
            return dal.DeleteBatch(list);
        }

        /// <summary>
        /// 获取所有分类数据集
        /// </summary>
        /// <returns></returns>
        public List<CategoryInfo> GetList()
        {
            return dal.GetList();
        }

        /// <summary>
        /// 获取树json格式字符串
        /// </summary>
        /// <returns></returns>
        public string GetTreeJson()
        {
            return dal.GetTreeJson();
        }

        #endregion
    }
}
