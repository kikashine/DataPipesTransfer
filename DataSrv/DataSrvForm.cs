using System;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using StockToolKit.Common;

namespace StockToolKit.DataSrv
{
    public partial class DataSrvForm : Form
    {
        public DataSrvForm()
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 512;
            InitializeComponent();
        }
          
        private void btnDir_Click(object sender, EventArgs e)
        {
            Thread dhThread = new Thread(new ThreadStart(DataHost));
            dhThread.Start();
        }
       
        private void DataHost()
        {
            PipeServer pw = new PipeServer();
            pw.Start();
        }


    }

}
