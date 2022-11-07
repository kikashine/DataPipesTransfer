using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using StockToolKit.Common;

namespace StockToolKit.Analyze
{
    /// <summary>
    /// 选择、设置需要分析的模型
    /// </summary>
    public class AnalyzeModelSelection
    {

        //选择的分析模型的汉字名称
        public List<string> strSelectedModels;


        public AnalyzeModelSelection()
        {
            strSelectedModels = new List<string>();
        }
        /// <summary>
        /// 生成分析选择项部分
        /// </summary>
        /// <param name="theForm"></param>
        public void genAnalyzeSelection(Control.ControlCollection Controls)
        {
            System.Windows.Forms.Panel panel1 = new System.Windows.Forms.Panel();
            System.Windows.Forms.Button btnremove1 = new System.Windows.Forms.Button();
            System.Windows.Forms.Button btnselect1 = new System.Windows.Forms.Button();
            System.Windows.Forms.Label label6 = new System.Windows.Forms.Label();
            System.Windows.Forms.ListBox lbAnalyzeModelsActive = new System.Windows.Forms.ListBox();
            System.Windows.Forms.ListBox lbAnalyzeModels = new System.Windows.Forms.ListBox();
            panel1.SuspendLayout();
            // 
            // panel1
            // 

            //panel1.Controls.Add(btncfg1);
            panel1.Controls.Add(btnremove1);
            panel1.Controls.Add(btnselect1);
            panel1.Controls.Add(label6);
            panel1.Controls.Add(lbAnalyzeModelsActive);
            panel1.Controls.Add(lbAnalyzeModels);
            panel1.Location = new System.Drawing.Point(2, 60);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(625, 183);
            panel1.TabIndex = 0;
            // 
            // btnremove1
            // 
            btnremove1.Location = new System.Drawing.Point(295, 55);
            btnremove1.Name = "btnremove1";
            btnremove1.Size = new System.Drawing.Size(35, 20);
            btnremove1.TabIndex = 51;
            btnremove1.Text = "<-";
            btnremove1.UseVisualStyleBackColor = true;
            btnremove1.Click += new System.EventHandler(this.btnremove1_Click);
            // 
            // btnselect1
            // 
            btnselect1.Location = new System.Drawing.Point(295, 29);
            btnselect1.Name = "btnselect1";
            btnselect1.Size = new System.Drawing.Size(35, 20);
            btnselect1.TabIndex = 50;
            btnselect1.Text = "->";
            btnselect1.UseVisualStyleBackColor = true;
            btnselect1.Click += new System.EventHandler(this.btnselect1_Click);
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(350, 14);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(41, 12);
            label6.TabIndex = 49;
            label6.Text = "已选择";
            // 
            // lbAnalyzeModelsActive
            // 
            lbAnalyzeModelsActive.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lbAnalyzeModelsActive.FormattingEnabled = true;
            lbAnalyzeModelsActive.ItemHeight = 12;
            lbAnalyzeModelsActive.Location = new System.Drawing.Point(352, 29);
            lbAnalyzeModelsActive.Name = "lbAnalyzeModelsActive";
            lbAnalyzeModelsActive.Size = new System.Drawing.Size(262, 146);
            lbAnalyzeModelsActive.TabIndex = 48;
            lbAnalyzeModelsActive.DoubleClick += new EventHandler(lbAnalyzeModelsActive_DoubleClick);
            lbAnalyzeModelsActive.MouseDown += new MouseEventHandler(lbAnalyzeModelsActive_MouseDown);
            // 
            // lbAnalyzeModels
            // 
            lbAnalyzeModels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lbAnalyzeModels.FormattingEnabled = true;
            lbAnalyzeModels.ItemHeight = 12;
            //添加所有可用选项
            lbAnalyzeModels.Items.AddRange(AnalyzeModelFunc.getAllModelChs());
            lbAnalyzeModels.Location = new System.Drawing.Point(10, 29);
            lbAnalyzeModels.Name = "lbAnalyzeModels";
            lbAnalyzeModels.Size = new System.Drawing.Size(262, 146);
            lbAnalyzeModels.TabIndex = 47;
            lbAnalyzeModels.DoubleClick += new EventHandler(lbAnalyzeModels_DoubleClick);
            lbAnalyzeModels.MouseDown += new MouseEventHandler(lbAnalyzeModels_MouseDown);

            Controls.Add(panel1);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
        }

        void lbAnalyzeModels_MouseDown(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.ListBox lbAnalyzeModels = (System.Windows.Forms.ListBox)sender;
            System.Windows.Forms.ListBox lbAnalyzeModelsActive = (System.Windows.Forms.ListBox)lbAnalyzeModels.Parent.Controls["lbAnalyzeModelsActive"];
            //没有选择项目
            if (lbAnalyzeModels.SelectedItems.Count == 0)
            {
                return;
            }
            //非双击
            if(e.Clicks<=1)
            {
                return;
            }
            int index = lbAnalyzeModels.IndexFromPoint(e.X, e.Y);
            //双击项目索引和选择项目索引不一致
            if (index != lbAnalyzeModels.SelectedIndex)
            {
                return;
            }

            //双击项目已经存在于已选项目listbox中
            foreach (object ob in lbAnalyzeModelsActive.Items)
            {
                if (ob.Equals(lbAnalyzeModels.SelectedItem))
                {
                    return;
                }
            }

            lbAnalyzeModelsActive.Items.Add(lbAnalyzeModels.SelectedItem);


            this.strSelectedModels.Clear();
            foreach (string n in lbAnalyzeModelsActive.Items)
            {
                this.strSelectedModels.Add(n);
            }
            //throw new NotImplementedException();
        }

        void lbAnalyzeModelsActive_MouseDown(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.ListBox lbAnalyzeModelsActive = (System.Windows.Forms.ListBox)sender;
            System.Windows.Forms.ListBox lbAnalyzeModels = (System.Windows.Forms.ListBox)lbAnalyzeModelsActive.Parent.Controls["lbAnalyzeModels"];
            //没有选择项目
            if (lbAnalyzeModelsActive.SelectedItems.Count == 0)
            {
                return;
            }
            //非双击
            if (e.Clicks <= 1)
            {
                return;
            }
            int index = lbAnalyzeModelsActive.IndexFromPoint(e.X, e.Y);
            //双击项目索引和选择项目索引不一致
            if (index != lbAnalyzeModelsActive.SelectedIndex)
            {
                return;
            }

            lbAnalyzeModelsActive.Items.Remove(lbAnalyzeModelsActive.SelectedItem);
            this.strSelectedModels.Clear();
            foreach (string n in lbAnalyzeModelsActive.Items)
            {
                this.strSelectedModels.Add(n);
            }
            //throw new NotImplementedException();
        }

        void lbAnalyzeModels_DoubleClick(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        void lbAnalyzeModelsActive_DoubleClick(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        ///添加要分析的项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnselect1_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Button btn = (System.Windows.Forms.Button)sender;
            System.Windows.Forms.ListBox lbAnalyzeModelsActive = (System.Windows.Forms.ListBox)btn.Parent.Controls["lbAnalyzeModelsActive"];
            System.Windows.Forms.ListBox lbAnalyzeModels = (System.Windows.Forms.ListBox)btn.Parent.Controls["lbAnalyzeModels"];

            if (lbAnalyzeModels.SelectedItem == null)
            {
                return;
            }
            foreach (object ob in lbAnalyzeModelsActive.Items)
            {
                if (ob.Equals(lbAnalyzeModels.SelectedItem))
                {
                    return;
                }
            }
            lbAnalyzeModelsActive.Items.Add(lbAnalyzeModels.SelectedItem);


            this.strSelectedModels.Clear();
            foreach (string n in lbAnalyzeModelsActive.Items)
            {
                this.strSelectedModels.Add(n);
            }
        }

        /// <summary>
        /// 从待分析的项目列表中去除选中项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnremove1_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Button btn = (System.Windows.Forms.Button)sender;
            System.Windows.Forms.ListBox lbAnalyzeModelsActive = (System.Windows.Forms.ListBox)btn.Parent.Controls["lbAnalyzeModelsActive"];
            System.Windows.Forms.ListBox lbAnalyzeModels = (System.Windows.Forms.ListBox)btn.Parent.Controls["lbAnalyzeModels"];

            if (lbAnalyzeModelsActive.SelectedItem == null)
            {
                return;
            }

            //int index = 0;
            //if (lbAnalyzeModelsActive.SelectedIndex >= 0)
            //{
            //    index = lbAnalyzeModelsActive.SelectedIndex;
            //}
            lbAnalyzeModelsActive.Items.Remove(lbAnalyzeModelsActive.SelectedItem);
            //if (lbAnalyzeModelsActive.Items.Count > 0)
            //{
            //    this.lbStatisticsActive.SetSelected(index,true);
            //}
            this.strSelectedModels.Clear();
            foreach (string n in lbAnalyzeModelsActive.Items)
            {
                this.strSelectedModels.Add(n);
            }
        }

        /// <summary>
        /// 根据选项得到参数
        /// </summary>
        /// <param name="JType"></param>
        /// <param name="tstart"></param>
        /// <param name="tend"></param>
        /// <param name="SameStockInOneLine"></param>
        /// <returns></returns>
        public List<AnalyzeParameters> getParameters(JobType JType, DateTime tstart, DateTime tend)
        {
            //准备参数
            //toAnalyze = new ArrayList();
            //ArrayList toAnalyzeThisTime = new ArrayList();
            List<AnalyzeParameters>  parameters = new List<AnalyzeParameters>();
            //List<AnalyzeParameters> paramatersThisTime = new List<AnalyzeParameters>();

            string DateStart = Convert2DBDate.toDBDate(tstart);
            string DateEnd = Convert2DBDate.toDBDate(tend);
            foreach (string n in this.strSelectedModels)
            {
                switch (AnalyzeModelFunc.Chs2Model(n))
                {
                    case AnalyzeModel.MATrendRise:
                        MATrendRiseParameters param1 = new MATrendRiseParameters();
                        param1.DateStart = DateStart;
                        param1.DateEnd = DateEnd;
                        param1.JType = JType;
                        param1.Mode = AnalyzeModel.MATrendRise;
                        param1.SameStockInOneLine = false;
                        parameters.Add(param1);
                        break;
                    case AnalyzeModel.WLTrendRise:
                        WLTrendRiseParameters param2 = new WLTrendRiseParameters();
                        param2.DateStart = DateStart;
                        param2.DateEnd = DateEnd;
                        param2.JType = JType;
                        param2.Mode = AnalyzeModel.WLTrendRise;
                        param2.SameStockInOneLine = false;
                        parameters.Add(param2);
                        break;
                    case AnalyzeModel.Test:
                        TestParameters param3 = new TestParameters();
                        param3.DateStart = DateStart;
                        param3.DateEnd = DateEnd;
                        param3.JType = JType;
                        param3.Mode = AnalyzeModel.Test;
                        param3.SameStockInOneLine = false;
                        parameters.Add(param3);
                        break;
                    case AnalyzeModel.ShowKLine:
                        ShowKLineParameters param4 = new ShowKLineParameters();
                        param4.DateStart = DateStart;
                        param4.DateEnd = DateEnd;
                        param4.JType = JType;
                        param4.Mode = AnalyzeModel.ShowKLine;
                        param4.SameStockInOneLine = false;
                        parameters.Add(param4);
                        break;
                    default:
                        break;
                }
            }
            return parameters;
        }

     }
}
