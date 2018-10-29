using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using TygaSoft.ThreadProcessor;

namespace TygaSoft.WS_Processor
{
    public partial class TygaService : ServiceBase
    {
        public TygaService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            TyUserProcessor.Processor();

            SmsProcessor.Processor();
        }

        protected override void OnStop()
        {
        }
    }
}
