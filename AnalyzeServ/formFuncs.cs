using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StockToolKit.Analyze
{
    public partial class formFuncs : Form
    {
        //private formModelManage formmm;
        private DataKeeper _dkpr = new DataKeeper();
        public formFuncs()
        {
            InitializeComponent();
        }

        private void labelModelManage_Click(object sender, EventArgs e)
        {

            PipeClient _pclient = new PipeClient();
            _pclient.getDataUpdate();
        }

        private void labelAnalyze_Click(object sender, EventArgs e)
        {
            PipeClient _pclient= new PipeClient();
            _pclient.getStockList();
        }

        private void labelTradeManage_Click(object sender, EventArgs e)
        {
            _dkpr.getStockDataSet("600012");
        }
    }
}
