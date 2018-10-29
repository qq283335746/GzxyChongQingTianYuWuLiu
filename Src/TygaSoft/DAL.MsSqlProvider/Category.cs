using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using TygaSoft.IDAL;
using TygaSoft.Model;
using TygaSoft.DBUtility;

namespace TygaSoft.DAL.MsSqlProvider
{
    public class Category : ICategory
    {
        #region ICategory Members

        public int Insert(CategoryInfo model)
        {
            if (model == null) return -1;

            if (IsExist(model.CategoryName, model.ParentId, Guid.Empty)) return 110;

            //定义查询命令
            string cmdText = "insert into Category (CategoryName,CategoryCode,ParentId,Sort,Remark,LastUpdatedDate) values (@CategoryName,@CategoryCode,@ParentId,@Sort,@Remark,@LastUpdatedDate)";
            //创建查询命令参数集
            SqlParameter[] parms = {
                                        new SqlParameter("@CategoryName",SqlDbType.NVarChar,256),
                                        new SqlParameter("@CategoryCode",SqlDbType.NVarChar,256),
                                        new SqlParameter("@ParentId",SqlDbType.UniqueIdentifier),
                                        new SqlParameter("@Sort",SqlDbType.Int),
                                        new SqlParameter("@Remark",SqlDbType.NVarChar,300),
                                        new SqlParameter("@LastUpdatedDate",SqlDbType.DateTime),
                                    };

            parms[0].Value = model.CategoryName;
            parms[1].Value = model.CategoryCode;
            parms[2].Value = Guid.Parse(model.ParentId.ToString());
            parms[3].Value = model.Sort;
            parms[4].Value = model.Remark;
            parms[5].Value = model.LastUpdatedDate;

            return SqlHelper.ExecuteNonQuery(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parms);
        }

        public int Update(CategoryInfo model)
        {
            if (model == null) return -1;

            if (IsExist(model.CategoryName, model.ParentId, model.Id)) return 110;

            string cmdText = "update [Category] set CategoryName = @CategoryName,CategoryCode = @CategoryCode,ParentId = @ParentId,Sort = @Sort,Remark = @Remark,LastUpdatedDate = @LastUpdatedDate where Id = @Id";
            SqlParameter[] parms = {
                                       new SqlParameter("@Id",SqlDbType.UniqueIdentifier),
                                       new SqlParameter("@CategoryName",SqlDbType.NVarChar,256),
                                       new SqlParameter("@CategoryCode",SqlDbType.NVarChar,256),
                                       new SqlParameter("@ParentId",SqlDbType.UniqueIdentifier),
                                       new SqlParameter("@Sort",SqlDbType.Int),
                                       new SqlParameter("@Remark",SqlDbType.NVarChar,300),
                                       new SqlParameter("@LastUpdatedDate",SqlDbType.DateTime)
                                   };

            parms[0].Value = Guid.Parse(model.Id.ToString());
            parms[1].Value = model.CategoryName;
            parms[2].Value = model.CategoryCode;
            parms[3].Value = Guid.Parse(model.ParentId.ToString());
            parms[4].Value = model.Sort;
            parms[5].Value = model.Remark;
            parms[6].Value = model.LastUpdatedDate;

            return SqlHelper.ExecuteNonQuery(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parms);
        }

        public int Delete(object Id)
        {
            Guid gId = Guid.Empty;
            Guid.TryParse(Id.ToString(), out gId);
            if (gId == Guid.Empty) return -1;

            string cmdText = "delete from Category where Id = @Id";
            SqlParameter parm = new SqlParameter("@Id", SqlDbType.UniqueIdentifier);
            parm.Value = gId;

            return SqlHelper.ExecuteNonQuery(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parm);
        }

        public CategoryInfo GetModel(object Id)
        {
            Guid gId = Guid.Empty;
            Guid.TryParse(Id.ToString(), out gId);
            if (gId.Equals(Guid.Empty)) return null;

            CategoryInfo model = null;
            string cmdText = "select top 1 * from Category where Id = @Id order by LastUpdatedDate desc";
            SqlParameter parm = new SqlParameter("@Id", SqlDbType.UniqueIdentifier);
            parm.Value = gId;
            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parm))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        model = new CategoryInfo();
                        model.Id = reader["Id"];
                        model.CategoryName = reader["CategoryName"].ToString();
                        model.CategoryCode = reader["CategoryCode"].ToString();
                        model.ParentId = reader["ParentId"];
                        model.Sort = int.Parse(reader["Sort"].ToString());
                        model.Remark = reader["Remark"].ToString();
                        model.LastUpdatedDate = DateTime.Parse(reader["LastUpdatedDate"].ToString());
                    }
                }
            }

            return model;
        }

        private bool IsExist(string categoryName, object parentId, object Id)
        {
            if (string.IsNullOrEmpty(categoryName)) return false;

            Guid gId = Guid.Empty;
            Guid.TryParse(Id.ToString(), out gId);

            string cmdText = @"select 1 from Category where lower(CategoryName) = @CategoryName and ParentId = @ParentId ";
            SqlParameter[] parms = {
                                       new SqlParameter("@CategoryName", SqlDbType.NVarChar,256),
                                       new SqlParameter("@ParentId", SqlDbType.UniqueIdentifier)
                                   };
            parms[0].Value = categoryName;
            parms[1].Value = Guid.Parse(parentId.ToString());

            if (gId != Guid.Empty)
            {
                cmdText = "select 1 from Category where lower(CategoryName) = @CategoryName and ParentId = @ParentId and Id <> @Id";
                Array.Resize(ref parms, parms.Length + 1);
                SqlParameter parm = new SqlParameter("@Id", SqlDbType.UniqueIdentifier);
                parm.Value = gId;
                parms[2] = parm;
            }

            object obj = SqlHelper.ExecuteScalar(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parms);
            if (obj != null) return true;

            return false;
        }

        public bool DeleteBatch(IList<object> list)
        {
            if (list == null || list.Count == 0) return false;
            bool result = false;
            StringBuilder sb = new StringBuilder();
            ParamsHelper parms = new ParamsHelper();
            int n = 0;
            foreach (string item in list)
            {
                n++;
                sb.Append(@"delete from [Category] where Id = @Id" + n + " ;");
                SqlParameter parm = new SqlParameter("@Id" + n + "", SqlDbType.UniqueIdentifier);
                parm.Value = Guid.Parse(item);
                parms.Add(parm);
            }
            using (SqlConnection conn = new SqlConnection(SqlHelper.SqlProviderConnString))
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        int effect = SqlHelper.ExecuteNonQuery(tran, CommandType.Text, sb.ToString(), parms != null ? parms.ToArray() : null);
                        tran.Commit();
                        if (effect > 0) result = true;
                    }
                    catch
                    {
                        tran.Rollback();
                    }
                }
            }
            return result;
        }

        public List<CategoryInfo> GetList(int pageIndex, int pageSize, out int totalCount, string sqlWhere, params SqlParameter[] cmdParms)
        {
            //获取数据集总数
            string cmdText = "select count(*) from [Category] t1 ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            totalCount = (int)SqlHelper.ExecuteScalar(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms);
            //返回分页数据
            int startIndex = (pageIndex - 1) * pageSize + 1;
            int endIndex = pageIndex * pageSize;
            cmdText = @"select * from(select row_number() over(order by Sort) as RowNumber,Id,CategoryName,CategoryCode,ParentId,Sort,Remark,LastUpdatedDate from [Category] ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            cmdText += ")as objTable where RowNumber between " + startIndex + " and " + endIndex + " ";

            List<CategoryInfo> list = new List<CategoryInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        CategoryInfo model = new CategoryInfo();
                        model.Id = reader["Id"];
                        model.CategoryName = reader["CategoryName"].ToString();
                        model.CategoryCode = reader["CategoryCode"].ToString();
                        model.ParentId = reader["ParentId"];
                        model.Sort = Int32.Parse(reader["Sort"].ToString());
                        model.Remark = reader["Remark"].ToString();
                        model.LastUpdatedDate = DateTime.Parse(reader["LastUpdatedDate"].ToString());

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        public List<CategoryInfo> GetList(int pageIndex, int pageSize, string sqlWhere, params SqlParameter[] cmdParms)
        {
            int startIndex = (pageIndex - 1) * pageSize + 1;
            int endIndex = pageIndex * pageSize;
            string cmdText = @"select * from(select row_number() over(order by Sort) as RowNumber,Id,CategoryName,CategoryCode,ParentId,Sort,Remark,LastUpdatedDate from [Category] ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            cmdText += ")as objTable where RowNumber between " + startIndex + " and " + endIndex + " ";

            List<CategoryInfo> list = new List<CategoryInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        CategoryInfo model = new CategoryInfo();
                        model.Id = reader["Id"];
                        model.CategoryName = reader["CategoryName"].ToString();
                        model.CategoryCode = reader["CategoryCode"].ToString();
                        model.ParentId = reader["ParentId"];
                        model.Sort = Int32.Parse(reader["Sort"].ToString());
                        model.Remark = reader["Remark"].ToString();
                        model.LastUpdatedDate = DateTime.Parse(reader["LastUpdatedDate"].ToString());

                        list.Add(model);

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        public List<CategoryInfo> GetList()
        {
            List<CategoryInfo> list = new List<CategoryInfo>();

            string cmdText = "select Id,CategoryName,CategoryCode,ParentId,Sort,Remark,LastUpdatedDate from [Category] ";

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        CategoryInfo model = new CategoryInfo();
                        model.Id = reader.GetGuid(0);
                        model.CategoryName = reader.GetString(1);
                        model.CategoryCode = reader.GetString(2);
                        model.ParentId = reader.GetGuid(3);
                        model.Sort = reader.GetInt32(4);
                        model.Remark = reader.GetString(5);
                        model.LastUpdatedDate = reader.GetDateTime(6);

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        public string GetTreeJson()
        {
            StringBuilder jsonAppend = new StringBuilder();
            List<CategoryInfo> list = GetList();
            if (list != null && list.Count > 0)
            {
                CreateTreeJson(list, Guid.Empty, ref jsonAppend);
            }
            else
            {
                jsonAppend.Append("[{\"id\":\"" + Guid.Empty + "\",\"text\":\"请选择\",\"state\":\"open\",\"attributes\":{\"parentId\":\"" + Guid.Empty + "\",\"parentName\":\"请选择\"}}]");
            }

            return jsonAppend.ToString();
        }

        /// <summary>
        /// 获取树json格式字符串
        /// </summary>
        /// <param name="list"></param>
        /// <param name="parentId"></param>
        /// <param name="jsonAppend"></param>
        private void CreateTreeJson(List<CategoryInfo> list, object parentId, ref StringBuilder jsonAppend)
        {
            jsonAppend.Append("[");
            var childList = list.FindAll(x => x.ParentId.Equals(parentId));
            if (childList.Count > 0)
            {
                int temp = 0;
                foreach (var model in childList)
                {
                    jsonAppend.Append("{\"id\":\"" + model.Id + "\",\"text\":\"" + model.CategoryName + "\",\"state\":\"open\",\"attributes\":{\"parentId\":\"" + model.ParentId + "\",\"categoryCode\":\"" + model.CategoryCode + "\",\"sort\":\"" + model.Sort + "\",\"remark\":\"" + model.Remark + "\"}");
                    if (list.Any(r => r.ParentId.Equals(model.Id)))
                    {
                        jsonAppend.Append(",\"children\":");
                        CreateTreeJson(list, model.Id, ref jsonAppend);
                    }
                    jsonAppend.Append("}");
                    if (temp < childList.Count - 1) jsonAppend.Append(",");
                    temp++;
                }
            }
            jsonAppend.Append("]");
        }  

        #endregion
    }
}
