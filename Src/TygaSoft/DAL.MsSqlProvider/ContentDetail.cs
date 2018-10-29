using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using TygaSoft.IDAL;
using TygaSoft.Model;
using TygaSoft.DBUtility;

namespace TygaSoft.DAL.MsSqlProvider
{
    public class ContentDetail : IContentDetail
    {
        /// <summary>
        /// 添加数据到数据库
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(ContentDetailInfo model)
        {
            if (model == null) return -1;

            //判断当前记录是否存在，如果存在则返回;
            if (IsExist(model.Title,model.ContentTypeId, "")) return 110;

            string cmdText = "insert into [Sys_ContentDetail] (ContentTypeId,Title,ContentText,Sort,LastUpdatedDate,UserId) values (@ContentTypeId,@Title,@ContentText,@Sort,@LastUpdatedDate,@UserId)";
            //创建查询命令参数集
            SqlParameter[] parms = {
                                     new SqlParameter("@ContentTypeId",SqlDbType.UniqueIdentifier), 
                                     new SqlParameter("@Title",SqlDbType.NVarChar,256), 
                                     new SqlParameter("@ContentText",SqlDbType.NText), 
                                     new SqlParameter("@Sort",SqlDbType.Int), 
                                     new SqlParameter("@LastUpdatedDate",SqlDbType.DateTime), 
                                     new SqlParameter("@UserId",SqlDbType.UniqueIdentifier)  
                                   };
            parms[0].Value = Guid.Parse(model.ContentTypeId.ToString());
            parms[1].Value = model.Title;
            parms[2].Value = model.ContentText;
            parms[3].Value = model.Sort;
            parms[4].Value = model.LastUpdatedDate;
            parms[5].Value = Guid.Parse(model.UserId.ToString());

            //执行数据库操作
            return SqlHelper.ExecuteNonQuery(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parms);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(ContentDetailInfo model)
        {
            if (IsExist(model.Title,model.ContentTypeId, model.Id)) return 110;

            //定义查询命令
            string cmdText = @"update [Sys_ContentDetail] set ContentTypeId = @ContentTypeId,Title = @Title,ContentText = @ContentText,Sort = @Sort,LastUpdatedDate = @LastUpdatedDate,UserId = @UserId where Id = @Id";

            //创建查询命令参数集
            SqlParameter[] parms = {
                                     new SqlParameter("@Id",SqlDbType.UniqueIdentifier),
                                     new SqlParameter("@ContentTypeId",SqlDbType.UniqueIdentifier), 
                                     new SqlParameter("@Title",SqlDbType.NVarChar,256), 
                                     new SqlParameter("@ContentText",SqlDbType.NText), 
                                     new SqlParameter("@Sort",SqlDbType.Int), 
                                     new SqlParameter("@LastUpdatedDate",SqlDbType.DateTime), 
                                     new SqlParameter("@UserId",SqlDbType.UniqueIdentifier)
                                   };
            parms[0].Value = Guid.Parse(model.Id.ToString());
            parms[1].Value = Guid.Parse(model.ContentTypeId.ToString());
            parms[2].Value = model.Title;
            parms[3].Value = model.ContentText;
            parms[4].Value = model.Sort;
            parms[5].Value = model.LastUpdatedDate;
            parms[6].Value = Guid.Parse(model.UserId.ToString());


            return SqlHelper.ExecuteNonQuery(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parms);
        }

        /// <summary>
        /// 删除对应数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public int Delete(object Id)
        {
            Guid gId = Guid.Empty;
            Guid.TryParse(Id.ToString(), out gId);
            if (gId == Guid.Empty) return -1;

            string cmdText = "delete from Sys_ContentDetail where Id = @Id";
            SqlParameter parm = new SqlParameter("@Id", SqlDbType.UniqueIdentifier);
            parm.Value = gId;

            return SqlHelper.ExecuteNonQuery(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parm);
        }

        /// <summary>
        /// 批量删除数据（启用事务）
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool DeleteBatch(IList<string> list)
        {
            if (list == null || list.Count == 0) return false;

            bool result = false;
            StringBuilder sb = new StringBuilder();
            ParamsHelper parms = new ParamsHelper();
            int n = 0;
            foreach (string item in list)
            {
                n++;
                sb.Append(@"delete from [Sys_ContentDetail] where Id = @Id" + n + " ;");
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

        /// <summary>
        /// 获取对应的数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ContentDetailInfo GetModel(object Id)
        {
            Guid gId = Guid.Empty;
            Guid.TryParse(Id.ToString(), out gId);
            if (gId == Guid.Empty) return null;

            ContentDetailInfo model = new ContentDetailInfo();

            string cmdText = @"select top 1 Id,ContentTypeId,Title,ContentText,Sort,LastUpdatedDate,UserId from [Sys_ContentDetail] where Id = @Id order by LastUpdatedDate desc ";
            SqlParameter parm = new SqlParameter("@Id", SqlDbType.UniqueIdentifier);
            parm.Value = gId;

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parm))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        model.Id = reader.GetGuid(0);
                        model.ContentTypeId = reader.GetGuid(1);
                        model.Title = reader.GetString(2);
                        model.ContentText = reader.GetString(3);
                        model.Sort = reader.GetInt32(4);
                        model.LastUpdatedDate = reader.GetDateTime(5);
                        model.UserId = reader.GetGuid(6);
                    }
                }
            }

            return model;
        }

        /// <summary>
        /// 是否存在对应数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        private bool IsExist(string name, object ContentTypeId, object Id)
        {
            Guid gId = Guid.Empty;
            Guid.TryParse(Id.ToString(), out gId);
            ParamsHelper parms = new ParamsHelper();

            string cmdText = "select 1 from [Sys_ContentDetail] where lower(Title) = @Title and ContentTypeId = @ContentTypeId ";
            if (gId != Guid.Empty)
            {
                cmdText = "select 1 from [Sys_ContentDetail] where lower(Title) = @Title and ContentTypeId = @ContentTypeId and Id <> @Id ";
                SqlParameter parm1 = new SqlParameter("@Id", SqlDbType.UniqueIdentifier);
                parm1.Value = gId;
                parms.Add(parm1);
            }
            SqlParameter parm = new SqlParameter("@Title", SqlDbType.NVarChar, 256);
            parm.Value = name;
            parms.Add(parm);

            parm = new SqlParameter("@ContentTypeId", SqlDbType.UniqueIdentifier);
            parm.Value = Guid.Parse(ContentTypeId.ToString());
            parms.Add(parm);

            object obj = SqlHelper.ExecuteScalar(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parms.ToArray());
            if (obj != null) return true;

            return false;
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
        public List<ContentDetailInfo> GetList(int pageIndex, int pageSize, out int totalCount, string sqlWhere, params SqlParameter[] cmdParms)
        {
            //获取数据集总数
            string cmdText = "select count(*) from [Sys_ContentDetail] cd left join Sys_ContentType ct on ct.Id = cd.ContentTypeId ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            totalCount = (int)SqlHelper.ExecuteScalar(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms);
            //返回分页数据
            int startIndex = (pageIndex - 1) * pageSize + 1;
            int endIndex = pageIndex * pageSize;
            cmdText = @"select * from(select row_number() over(order by cd.LastUpdatedDate desc) as RowNumber,cd.Id,cd.ContentTypeId,cd.Title,cd.ContentText,cd.Sort,cd.LastUpdatedDate,cd.UserId from [Sys_ContentDetail] cd
                      left join Sys_ContentType ct on ct.Id = cd.ContentTypeId ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            cmdText += ")as objTable where RowNumber between " + startIndex + " and " + endIndex + " ";

            List<ContentDetailInfo> list = new List<ContentDetailInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ContentDetailInfo model = new ContentDetailInfo();
                        model.Id = reader.GetGuid(1);
                        model.ContentTypeId = reader.GetGuid(2);
                        model.Title = reader.GetString(3);
                        model.ContentText = reader.GetString(4);
                        model.Sort = reader.GetInt32(5);
                        model.LastUpdatedDate = reader.GetDateTime(6);
                        model.UserId = reader.GetGuid(7);

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 获取满足当前条件的数据列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sqlWhere"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public List<ContentDetailInfo> GetList(int pageIndex, int pageSize, string sqlWhere, params SqlParameter[] cmdParms)
        {
            int startIndex = (pageIndex - 1) * pageSize + 1;
            int endIndex = pageIndex * pageSize;
            string cmdText = @"select * from(select row_number() over(order by cd.LastUpdatedDate desc) as RowNumber,cd.Id,cd.ContentTypeId,cd.Title,cd.ContentText,cd.Sort,cd.LastUpdatedDate,cd.UserId from [Sys_ContentDetail] cd 
                             left join Sys_ContentType ct on ct.Id = cd.ContentTypeId ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            cmdText += ")as objTable where RowNumber between " + startIndex + " and " + endIndex + " ";

            List<ContentDetailInfo> list = null;

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    list = new List<ContentDetailInfo>();

                    while (reader.Read())
                    {
                        ContentDetailInfo model = new ContentDetailInfo();
                        model.Id = reader.GetGuid(1);
                        model.ContentTypeId = reader.GetGuid(2);
                        model.Title = reader.GetString(3);
                        model.ContentText = reader.GetString(4);
                        model.Sort = reader.GetInt32(5);
                        model.LastUpdatedDate = reader.GetDateTime(6);
                        model.UserId = reader.GetGuid(7);

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 获取满足当前条件的数据列表
        /// </summary>
        /// <param name="sqlWhere"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public List<ContentDetailInfo> GetList(string sqlWhere, params SqlParameter[] cmdParms)
        {
            string cmdText = @"select cd.Id,cd.ContentTypeId,cd.Title,cd.ContentText,cd.Sort,cd.LastUpdatedDate,cd.UserId from [Sys_ContentDetail] cd 
                             left join Sys_ContentType ct on ct.Id = cd.ContentTypeId ";

            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            cmdText += " order by cd.LastUpdatedDate desc ";

            List<ContentDetailInfo> list = new List<ContentDetailInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    list = new List<ContentDetailInfo>();

                    while (reader.Read())
                    {
                        ContentDetailInfo model = new ContentDetailInfo();
                        model.Id = reader.GetGuid(0);
                        model.ContentTypeId = reader.GetGuid(1);
                        model.Title = reader.GetString(2);
                        model.ContentText = reader.GetString(3);
                        model.Sort = reader.GetInt32(4);
                        model.LastUpdatedDate = reader.GetDateTime(5);
                        model.UserId = reader.GetGuid(6);

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 获取当前内容类型的内容明细
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public Dictionary<object, string> GetKeyValueByType(string typeName)
        {
            Dictionary<object, string> dic = new Dictionary<object, string>();

            string cmdText = @"select cd.Id,cd.Title from Sys_ContentDetail cd join Sys_ContentType ct on ct.Id = cd.ContentTypeId 
                             where ct.TypeName = @TypeName";
            SqlParameter parm = new SqlParameter("@TypeName", SqlDbType.NVarChar, 256);
            parm.Value = typeName;

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parm))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        dic.Add(reader["Id"], reader.GetString(1));
                    }
                }
            }

            return dic;
        }

        /// <summary>
        /// 获取当前内容类型ID的子项
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public Dictionary<object, string> GetKeyValueByTypeId(object typeId)
        {
            Dictionary<object, string> dic = new Dictionary<object, string>();

            string cmdText = @"select cd.Id,cd.Title from Sys_ContentDetail cd where cd.ContentTypeId = @ContentTypeId";
            SqlParameter parm = new SqlParameter("@ContentTypeId", SqlDbType.UniqueIdentifier);
            parm.Value = Guid.Parse(typeId.ToString());

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parm))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        dic.Add(reader["Id"], reader.GetString(1));
                    }
                }
            }

            return dic;
        }
    }
}
