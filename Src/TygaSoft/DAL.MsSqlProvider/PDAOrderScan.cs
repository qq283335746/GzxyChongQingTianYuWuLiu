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
    public class PDAOrderScan : IPDAOrderScan
    {
        #region 成员方法

        /// <summary>
        /// 添加数据到数据库
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(PDAOrderScanInfo model)
        {
            if (model == null) return -1;

            string cmdText = "insert into [PDA_OrderScan] (OrderCode,NextNodeId,CurrNodeId,IsFinish,LastUpdatedDate) values (@OrderCode,@NextNodeId,@CurrNodeId,@IsFinish,@LastUpdatedDate)";

            SqlParameter[] parms = {
                                       new SqlParameter("@OrderCode",SqlDbType.VarChar,36), 
                                       new SqlParameter("@NextNodeId",SqlDbType.UniqueIdentifier),
                                       new SqlParameter("@CurrNodeId",SqlDbType.UniqueIdentifier), 
                                       new SqlParameter("@IsFinish",SqlDbType.Bit), 
                                       new SqlParameter("@LastUpdatedDate",SqlDbType.DateTime)
                                   };
            parms[0].Value = model.OrderCode;
            parms[1].Value = Guid.Parse(model.NextNodeId.ToString());
            parms[2].Value = Guid.Parse(model.CurrNodeId.ToString());
            parms[3].Value = model.IsFinish;
            parms[4].Value = model.LastUpdatedDate;

            //执行数据库操作
            return SqlHelper.ExecuteNonQuery(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parms);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(PDAOrderScanInfo model)
        {
            if (model == null) return -1;

            //定义查询命令
            string cmdText = @"update [PDA_OrderScan] set NextNodeId = @NextNodeId,CurrNodeId = @CurrNodeId,IsFinish = @IsFinish,LastUpdatedDate = LastUpdatedDate
                            where OrderCode = @OrderCode";

            SqlParameter[] parms = {
                                       new SqlParameter("@OrderCode",SqlDbType.VarChar,36), 
                                       new SqlParameter("@NextNodeId",SqlDbType.UniqueIdentifier),
                                       new SqlParameter("@CurrNodeId",SqlDbType.UniqueIdentifier), 
                                       new SqlParameter("@IsFinish",SqlDbType.Bit), 
                                       new SqlParameter("@LastUpdatedDate",SqlDbType.DateTime)
                                   };

            parms[0].Value = model.OrderCode;
            parms[1].Value = Guid.Parse(model.NextNodeId.ToString());
            parms[2].Value = Guid.Parse(model.CurrNodeId.ToString());
            parms[3].Value = model.IsFinish;
            parms[4].Value = model.LastUpdatedDate;

            return SqlHelper.ExecuteNonQuery(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parms);
        }

        /// <summary>
        /// 获取当前订单号对应的数据
        /// </summary>
        /// <param name="orderCode"></param>
        /// <returns></returns>
        public PDAOrderScanInfo GetModel(string orderCode)
        {
            PDAOrderScanInfo model = null;

            string cmdText = @"select top 1 OrderCode,NextNodeId,CurrNodeId,IsFinish,LastUpdatedDate 
                            from [PDA_OrderScan] where OrderCode = @OrderCode order by LastUpdatedDate desc ";
            SqlParameter parm = new SqlParameter("@OrderCode", SqlDbType.VarChar,36);
            parm.Value = orderCode;

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parm))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        model = new PDAOrderScanInfo();
                        model.OrderCode = reader.GetString(0).Trim();
                        model.NextNodeId = reader[1];
                        model.CurrNodeId = reader[2];
                        model.IsFinish = reader.GetBoolean(3);
                        model.LastUpdatedDate = reader.GetDateTime(4);
                    }
                }
            }

            return model;
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
        public List<PDAOrderScanInfo> GetList(int pageIndex, int pageSize, out int totalCount, string sqlWhere, params SqlParameter[] cmdParms)
        {
            //获取数据集总数
            string cmdText = "select count(*) from [PDA_OrderScan] ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            totalCount = (int)SqlHelper.ExecuteScalar(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms);
            //返回分页数据
            int startIndex = (pageIndex - 1) * pageSize + 1;
            int endIndex = pageIndex * pageSize;
            cmdText = @"select * from(select row_number() over(order by LastUpdatedDate desc) as RowNumber,OrderCode,NextNodeId,CurrNodeId,IsFinish,LastUpdatedDate
                      from [PDA_OrderScan] ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            cmdText += ")as objTable where RowNumber between " + startIndex + " and " + endIndex + " ";

            List<PDAOrderScanInfo> list = new List<PDAOrderScanInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        PDAOrderScanInfo model = new PDAOrderScanInfo();
                        model.OrderCode = reader.GetString(1).Trim();
                        model.NextNodeId = reader[2];
                        model.CurrNodeId = reader[3];
                        model.IsFinish = reader.GetBoolean(4);
                        model.LastUpdatedDate = reader.GetDateTime(5);

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
        public List<PDAOrderScanInfo> GetList(int pageIndex, int pageSize, string sqlWhere, params SqlParameter[] cmdParms)
        {
            int startIndex = (pageIndex - 1) * pageSize + 1;
            int endIndex = pageIndex * pageSize;
            string cmdText = @"select * from(select row_number() over(order by LastUpdatedDate desc) as RowNumber,OrderCode,NextNodeId,CurrNodeId,IsFinish,LastUpdatedDate
                             from [PDA_OrderScan] ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            cmdText += ")as objTable where RowNumber between " + startIndex + " and " + endIndex + " ";

            List<PDAOrderScanInfo> list = new List<PDAOrderScanInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        PDAOrderScanInfo model = new PDAOrderScanInfo();
                        model.OrderCode = reader.GetString(1).Trim();
                        model.NextNodeId = reader[2];
                        model.CurrNodeId = reader[3];
                        model.IsFinish = reader.GetBoolean(4);
                        model.LastUpdatedDate = reader.GetDateTime(5);

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        #endregion
    }
}
