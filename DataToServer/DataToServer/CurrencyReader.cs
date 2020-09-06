using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace DataToServer
{
    partial class CurrencyReader : ServiceBase
    {
        public CurrencyReader()
        {
            InitializeComponent();
        }

        InnerOperation obj = new InnerOperation();

        protected override void OnStart(string[] args)
        {
            //System.Diagnostics.Debugger.Launch();
            obj.Start();
        }

        protected override void OnStop()
        {
            obj.Stop();
        }
    }
}
