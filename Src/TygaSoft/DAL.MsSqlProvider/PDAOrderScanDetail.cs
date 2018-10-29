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
    public class PDAOrderScanDetail : IPDAOrderScanDetail
    {
        #region 成员方法

        /// <summary>
        /// 添加数据到数据库
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(PDAOrderScanDetailInfo model)
        {
            if (model == null) return -1;

            string cmdText = "insert into [PDA_OrderScanDetail] (OrderCode,CurrNodeId,ScanTime,Remark,UserId,LastUpdatedDate) values (@OrderCode,@CurrNodeId,@ScanTime,@Remark,@UserId,@LastUpdatedDate)";

            SqlParameter[] parms = {
                                       new SqlParameter("@OrderCode",SqlDbType.VarChar,36), 
                                       new SqlParameter("@CurrNodeId",SqlDbType.UniqueIdentifier), 
                                       new SqlParameter("@ScanTime",SqlDbType.DateTime),
                                       new SqlParameter("@Remark",SqlDbType.NVarChar,300), 
                                       new SqlParameter("@UserId",SqlDbType.UniqueIdentifier),
                                       new SqlParameter("@LastUpdatedDate",SqlDbType.DateTime)
                                   };
            parms[0].Value = model.OrderCode;
            parms[1].Value = Guid.Parse(model.CurrNodeId.ToString());
            parms[2].Value = model.ScanTime;
            parms[3].Value = model.Remark;
            parms[4].Value = Guid.Parse(model.UserId.ToString());
            parms[5].Value = model.LastUpdatedDate;

            //执行数据库操作
            return SqlHelper.ExecuteNonQuery(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parms);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(PDAOrderScanDetailInfo model)
        {
            if (model == null) return -1;

            //定义查询命令
            string cmdText = @"update [PDA_OrderScanDetail] set OrderCode = @OrderCode, CurrNodeId = @CurrNodeId,ScanTime = @ScanTime,Remark = @Remark,UserId = @UserId,LastUpdatedDate = @LastUpdatedDate
                             where Id = @Id ";

            SqlParameter[] parms = {
                                       new SqlParameter("@Id",SqlDbType.UniqueIdentifier), 
                                       new SqlParameter("@OrderCode",SqlDbType.VarChar,36), 
                                       new SqlParameter("@CurrNodeId",SqlDbType.UniqueIdentifier), 
                                       new SqlParameter("@ScanTime",SqlDbType.DateTime), 
                                       new SqlParameter("@Remark",SqlDbType.NVarChar,300), 
                                       new SqlParameter("@UserId",SqlDbType.UniqueIdentifier),
                                       new SqlParameter("@LastUpdatedDate",SqlDbType.DateTime)
                                   };
            parms[0].Value = model.Id;
            parms[1].Value = model.OrderCode;
            parms[2].Value = Guid.Parse(model.CurrNodeId.ToString());
            parms[3].Value = model.ScanTime;
            parms[4].Value = model.Remark;
            parms[5].Value = Guid.Parse(model.UserId.ToString());
            parms[6].Value = model.LastUpdatedDate;

            return SqlHelper.ExecuteNonQuery(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parms);
        }

        /// <summary>
        /// 获取当前订单号对应的数据
        /// </summary>
        /// <param name="orderCode"></param>
        /// <returns></returns>
        public PDAOrderScanDetailInfo GetModel(object Id)
        {
            PDAOrderScanDetailInfo model = null;

            string cmdText = @"select top 1 Id,OrderCode,CurrNodeId,ScanTime,Remark,UserId,LastUpdatedDate 
                             from PDA_OrderScanDetail where Id = @Id order by LastUpdatedDate desc ";
            SqlParameter parm = new SqlParameter("@Id", SqlDbType.UniqueIdentifier);
            parm.Value = Guid.Parse(Id.ToString());

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parm))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        model = new PDAOrderScanDetailInfo();
                        model.Id = reader[0];
                        model.OrderCode = reader.GetString(1).Trim();
                        model.CurrNodeId = reader[2];
                        model.ScanTime = reader.GetDateTime(3);
                        model.Remark = reader.GetString(4);
                        model.UserId = reader[5];
                        model.LastUpdatedDate = reader.GetDateTime(6);
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
        public List<PDAOrderScanDetailInfo> GetList(int pageIndex, int pageSize, out int totalRecords, string sqlWhere, params SqlParameter[] cmdParms)
        {
            //获取数据集总数
            string cmdText = "select count(*) from [PDA_OrderScanDetail] ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            totalRecords = (int)SqlHelper.ExecuteScalar(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms);
            //返回分页数据
            int startIndex = (pageIndex - 1) * pageSize + 1;
            int endIndex = pageIndex * pageSize;
            cmdText = @"select * from(select row_number() over(order by LastUpdatedDate desc) as RowNumber,osd.Id,osd.OrderCode,
                      osd.CurrNodeId,osd.ScanTime,osd.Remark,osd.UserId,osd.LastUpdatedDate,se.EnumValue,se.Remark EnumRemark,se.Sort 
                      from [PDA_OrderScanDetail] osd left join Sys_Enum se on se.Id = osd.CurrNodeId ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            cmdText += ")as objTable where RowNumber between " + startIndex + " and " + endIndex + " ";
            cmdText += "order by Sort desc ";

            List<PDAOrderScanDetailInfo> list = new List<PDAOrderScanDetailInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        PDAOrderScanDetailInfo model = new PDAOrderScanDetailInfo();
                        model.Id = reader[1];
                        model.OrderCode = reader.GetString(2).Trim();
                        model.CurrNodeId = reader[3];
                        model.ScanTime = reader.GetDateTime(4);
                        model.Remark = reader.GetString(5);
                        model.UserId = reader[6];
                        model.LastUpdatedDate = reader.GetDateTime(7);
                        model.SysEnumValue = reader.GetString(8).Trim();
                        model.SysEnumRemark = reader.GetString(9).Trim();

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
        public List<PDAOrderScanDetailInfo> GetList(int pageIndex, int pageSize, string sqlWhere, params SqlParameter[] cmdParms)
        {
            int startIndex = (pageIndex - 1) * pageSize + 1;
            int endIndex = pageIndex * pageSize;
            string cmdText = @"select * from(select row_number() over(order by osd.LastUpdatedDate desc) as RowNumber,
                             osd.Id,OrderCode,osd.CurrNodeId,osd.ScanTime,osd.Remark,osd.UserId,osd.LastUpdatedDate,se.EnumValue SysEnumValue
                             from [PDA_OrderScanDetail] osd join Sys_Enum se on se.Id = osd.CurrNodeId ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            cmdText += ")as objTable where RowNumber between " + startIndex + " and " + endIndex + " ";

            List<PDAOrderScanDetailInfo> list = new List<PDAOrderScanDetailInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        PDAOrderScanDetailInfo model = new PDAOrderScanDetailInfo();
                        model.Id = reader[1];
                        model.OrderCode = reader.GetString(2).Trim();
                        model.CurrNodeId = reader[3];
                        model.ScanTime = reader.GetDateTime(4);
                        model.Remark = reader.GetString(5);
                        model.UserId = reader[6];
                        model.LastUpdatedDate = reader.GetDateTime(7);

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
        public List<PDAOrderScanDetailInfo> GetList(string sqlWhere, params SqlParameter[] cmdParms)
        {
            string cmdText = @"select osd.Id,osd.OrderCode,osd.CurrNodeId,osd.ScanTime,osd.Remark,osd.UserId,osd.LastUpdatedDate,se.EnumValue,se.Remark EnumRemark
                             from [PDA_OrderScanDetail] osd left join Sys_Enum se on se.Id = osd.CurrNodeId ";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            cmdText += " order by se.Sort desc ";

            List<PDAOrderScanDetailInfo> list = new List<PDAOrderScanDetailInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        PDAOrderScanDetailInfo model = new PDAOrderScanDetailInfo();
                        model.Id = reader[0];
                        model.OrderCode = reader.GetString(1).Trim();
                        model.CurrNodeId = reader[2];
                        model.ScanTime = reader.GetDateTime(3);
                        model.Remark = reader.GetString(4);
                        model.UserId = reader[5];
                        model.LastUpdatedDate = reader.GetDateTime(6);
                        model.SysEnumValue = reader.GetString(7);
                        model.SysEnumRemark = reader.GetString(8);

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 获取当前订单的所有发收货扫描明细数据列表
        /// </summary>
        /// <param name="orderCode"></param>
        /// <returns></returns>
        public List<PDAOrderScanDetailInfo> GetList(string orderCode)
        {
            string cmdText = @"select Id,OrderCode,CurrNodeId,ScanTime,Remark,UserId,LastUpdatedDate
                             from [PDA_OrderScanDetail] where OrderCode = @OrderCode ";

            SqlParameter parm = new SqlParameter("@OrderCode", SqlDbType.VarChar, 36);
            parm.Value = orderCode;

            List<PDAOrderScanDetailInfo> list = new List<PDAOrderScanDetailInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.SqlProviderConnString, CommandType.Text, cmdText, parm))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        PDAOrderScanDetailInfo model = new PDAOrderScanDetailInfo();
                        model.Id = reader[0];
                        model.OrderCode = reader.GetString(1).Trim();
                        model.CurrNodeId = reader[2];
                        model.ScanTime = reader.GetDateTime(3);
                        model.Remark = reader.GetString(4);
                        model.UserId = reader[5];
                        model.LastUpdatedDate = reader.GetDateTime(6);

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        #endregion
    }
}
