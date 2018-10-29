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
    public class Order : IOrder
    {
        /// <summary>
        /// 获取数据分页列表，并返回所有记录数
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <param name="sqlWhere"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public List<OrderInfo> GetList(int pageIndex, int pageSize, out int totalCount, string sqlWhere, params SqlParameter[] cmdParms)
        {
            string sqlText = @"select row_number() over(order by o.BusinessDate desc) as RowNumber,
                            o.Code OrderCode,o.BusinessDate,o.TotalPackageCount,rs.Name SenderName,rcv.Name ReceiverName
                            from LM_Order o 
                            join Base_ReceiverSender rs on rs.ID = o.SenderID and rs.IsUsed = 1 and rs.IsSender = 1 
                            join Base_ReceiverSender rcv on rcv.ID = o.ReceiverID and rcv.IsUsed = 1 and rcv.IsReceiver = 1 
                            join Base_Customer c on c.ID = o.CustomerID and c.IsUsed = 1
                            join SM_User u on u.TargetID = c.ID ";
            //获取数据集总数
            string cmdText = @"select count(*) from("+sqlText+"";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            cmdText += " group by o.Code,o.BusinessDate,o.TotalPackageCount,rs.Name,rcv.Name ";
            cmdText += " )as objTable ";
   
            totalCount = (int)SqlHelper.ExecuteScalar(SqlHelper.TySqlProviderConnString, CommandType.Text, cmdText, cmdParms);
            //返回分页数据
            int startIndex = (pageIndex - 1) * pageSize + 1;
            int endIndex = pageIndex * pageSize;
            cmdText = @"select * from("+sqlText+"";
            if (!string.IsNullOrEmpty(sqlWhere)) cmdText += "where 1=1 " + sqlWhere;
            cmdText += " group by o.Code,o.BusinessDate,o.TotalPackageCount,rs.Name,rcv.Name ";
            cmdText += ")as objTable where RowNumber between " + startIndex + " and " + endIndex + " ";

            List<OrderInfo> list = new List<OrderInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.TySqlProviderConnString, CommandType.Text, cmdText, cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        OrderInfo model = new OrderInfo();
                        model.OrderCode = reader.GetString(1).Trim();
                        model.BusinessDate = reader.GetDateTime(2);
                        model.TotalPackageCount = (float)Math.Round(reader.GetDouble(3),2);
                        model.SenderName = reader.GetString(4).Trim();
                        model.ReceiverName = reader.GetString(5).Trim();

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 获取当前派车单号包含的所有订单号
        /// </summary>
        /// <param name="carCode"></param>
        /// <returns></returns>
        public string[] GetOrderByCarcode(string carCode)
        {
            string itemAppend = "";

            string cmdText = @"select distinct o.Code OrderCode 
                            from TM_ADC adc
                            join TG_ADCOrderGoods adcog on adcog.ADCID = adc.ID
                            join TG_Task tgt on tgt.ID = adcog.TaskID
                            join LM_Order o on o.ID = tgt.OrderID
                            where adc.Code = @Carcode";

            SqlParameter parm = new SqlParameter("@Carcode", SqlDbType.NVarChar, 50);
            parm.Value = carCode;

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.TySqlProviderConnString, CommandType.Text, cmdText, parm))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        itemAppend += reader.GetString(0).Trim() + ",";
                    }
                }
            }

            itemAppend = itemAppend.Trim(',');

            return itemAppend.Split(new char[]{','},StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
