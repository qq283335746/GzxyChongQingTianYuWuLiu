using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TygaSoft.ThreadProcessor
{
    //考勤日报信息
    public struct DayStatInfo
    {
        public string userId;
        public string userName;
        public string statDate;
        public Double checkOn;
        public Double overTime;
        public Double publicTime;
        public Double njLeave;
        public Double hjLeave;
        public Double khjLeave;
        public Double sjLeave;
        public Double cjLeave;
        public Double cjWork;
        public Double lcjLeave;
        public Double lcjWork;
        public Double jyjLeave;
        public Double jyjWork;
        public Double shjLeave;
        public Double bjLeave;
        public Double bjWork;
        public Double late;
        public Double longLate;
        public Double early;
        public Double amNot;
        public Double pmNot;
        public string outgo;
        public Double onBusi;
        public string amAgree;
        public string pmAgree;
        public string ApproveStatus;
        public DateTime ApproveTime;
        public string ApproveLeaderID;
        public string InstanceNum;
        public DateTime WFTime;
    }		
}
