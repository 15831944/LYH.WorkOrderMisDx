﻿using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using LYH.Framework.Commons;
using LYH.WorkOrder.Properties;
using LYH.WorkOrder.share;

namespace LYH.WorkOrder
{
    public partial class FrmProcCardBom : Form
    {
        public FrmProcCardBom()
        {
            //KeyDown += FrmWin_KeyDown;
            InitializeComponent();
        }

        private void FrmWin_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Close();
                    DialogResult = DialogResult.Cancel;
                    break;
            }
        }

        private void Form14_Load(object sender, EventArgs e)
        {
            var sql = $"SELECT Craft FROM PD_ProcCard_Craft WHERE DeptId='{SqlHelper.DeptId}' ORDER BY ID";
            var ds = SqlHelper.ExecuteDataset(SqlHelper.GetConnectionString("dzdj"), CommandType.Text, sql);
            cbxCraft.SpellSearchSource =
                (from DataRow dr in ds.Tables[0].Rows where !Convert.IsDBNull(dr[0]) select dr[0].ToString().Trim())
                    .ToArray();
            var acsc = new AutoCompleteStringCollection();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                acsc.Add(dr[0].ToString());
            }
            cbxCraft.AutoCompleteCustomSource = acsc;

            groupBox3.Visible = false;
            panel1.Visible = false;
            btnCancel.Enabled = false;
            Dgv2BindData();
            txtWONo.Focus();
        }

        /// <summary>
        ///     绑定数据到dataGridView1
        /// </summary>
        private void BindData()
        {
            var sql = "SELECT IDL,xuhaoone '工序号',xuhaoname '工序名称',xuhaotwo '加工工序'," +
                      "zuone '调机时间',zutwo '单件时间',xuj1 '单价',xuj2 '公式',xuj3 '补助',beizu1 '备注'," +
                      "cjri '创建日期',cje '创建人',qgri '修改日期',qge '修改人' FROM udtwo " +
                      $"WHERE tuhao='{txtDwgNo.Text.Trim()}' AND yema='{txtPageNo.Text.Trim()}' AND DeptId='{SqlHelper.DeptId}'" +
                      " ORDER BY xuhaoone,xuhaotwo";
            var ds = SqlHelper.ExecuteDataset(SqlHelper.GetConnection(), CommandType.Text, sql);
            dataGridView1.DataSource = ds.Tables[0];
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Sort(dataGridView1.Columns[1], ListSortDirection.Ascending);
        }

        /// <summary>
        ///     绑定数据到dataGridView2
        /// </summary>
        private void Dgv2BindData()
        {
            var sql = $"SELECT distinct tuhao '图号',yema '页码' FROM udtwo WHERE DeptId='{SqlHelper.DeptId}'";
            var ds = SqlHelper.ExecuteDataset(SqlHelper.GetConnection(), CommandType.Text, sql);
            dataGridView2.DataSource = ds.Tables[0];
            dataGridView2.Columns[0].Width = 200;
            dataGridView2.Columns[1].Width = 143;
            dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Ascending);
        }

        private void txtSn_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSn.Text) && !ValidateUtil.IsNumber(txtSn.Text))
            {
                MessageBox.Show(Resources.IsNotNumber, Resources.T提示);
                txtSn.Text = "";
            }
        }

        private void txtProcessSn_TextChanged(object sender, EventArgs e)
        {
            for (var i = 0; i < txtProcessSn.Text.Length; i++)
            {
                if (txtProcessSn.Text[i] >= '0' && txtProcessSn.Text[i] <= '9')
                {
                }
                else
                {
                    MessageBox.Show(Resources.IsNotNumber, Resources.T提示);
                    txtProcessSn.Text = "";
                }
            }
        }

        private void txtWONo_TextChanged(object sender, EventArgs e)
        {
            if (txtWONo.Text.Length != 7) return;
            var sql =
                $"SELECT TOP 1 * FROM udone WHERE sgdhao='{txtWONo.Text.Trim()}' AND DeptId='{SqlHelper.DeptId}'";
            var dr = SqlHelper.ExecuteReader(SqlHelper.GetConnection(), CommandType.Text, sql);
            if (dr.HasRows) return;
            MessageBox.Show($"此工单号{txtWONo.Text.Trim()}不存在，请重新输入!!", Resources.T提示);
            dr.Close();
            txtWONo.Text = "";
            txtWONo.Focus();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (btnAdd.Text == Resources.A新增)
            {
                btnAdd.Text = Resources.A保存;
                EnableBtn(false);
                btnAdd.Enabled = true;
                txtSn.Focus();
            }
            else
            {
                if (string.IsNullOrEmpty(txtDwgNo.Text))
                {
                    MessageBox.Show("图号不能为空!");
                    return;
                }

                if (string.IsNullOrEmpty(SqlHelper.DeptId))
                {
                    MessageBox.Show("未查到此账号所在部门!");
                }
                var sql =
                    $"SELECT TOP 1 * FROM udtwo WHERE tuhao='{txtDwgNo.Text.Trim()}' AND yema='{txtPageNo.Text.Trim()}' AND xuhaoone='{txtSn.Text.Trim()}' " +
                    $"AND xuhaotwo='{txtProcessSn.Text.Trim()}' AND DeptId='{SqlHelper.DeptId}'";
                var dr = SqlHelper.ExecuteReader(SqlHelper.GetConnection(), CommandType.Text, sql);
                if (dr.HasRows)
                {
                    MessageBox.Show($"加工工序-- {txtProcessSn.Text.Trim()} --已存在，请重新输入!!", Resources.T提示);
                    dr.Close();
                    txtProcessSn.Focus();
                }
                else
                {
                    sql =
                        "insert into udtwo(DeptId,tuhao,yema,xuhaoone,xuhaotwo,xuhaoname,zuone,zutwo,xuj1,xuj2,xuj3," +
                        $"beizu1,cje) values('{SqlHelper.DeptId}','{txtDwgNo.Text.Trim()}','{txtPageNo.Text.Trim()}',{txtSn.Text.Trim()}," +
                        $"{txtProcessSn.Text.Trim()},'{cbxCraft.Text.Trim()}','{txtAdjustingTime.Text.Trim()}'," +
                        $"'{txtProcessTime.Text.Trim()}','{txtUprice.Text.Trim()}','{txtFormula.Text.Trim()}'," +
                        $"'{txtSubsidy.Text.Trim()}','{txtRemark.Text.Trim()}','{SqlHelper.UserName}')";
                    SqlHelper.ExecuteNonQuery(SqlHelper.GetConnection(), CommandType.Text, sql);
                    btnAdd.Text = Resources.A新增;
                    var seq = int.Parse(txtSn.Text);
                    EnableBtn(true);
                    ClearTxtCtrl();
                    BindData();
                    Dgv2BindData();
                    txtSn.Text = (seq + 1).ToString();
                    cbxCraft.Focus();
                }
            }
        }

        private void EnableBtn(bool b)
        {
            btnAdd.Enabled = b;
            btnUpd.Enabled = b;
            btnDel.Enabled = b;
            btnCancel.Enabled = !b;
            btnFinish.Enabled = b;
        }

        private void btnUpd_Click(object sender, EventArgs e)
        {
            if (btnUpd.Text == Resources.X修改 || txtSn.Text == "")
            {
                btnUpd.Text = Resources.X保存;
                EnableBtn(false);
                btnUpd.Enabled = true;
                panel1.Visible = true;
                txtSn.ReadOnly = true;
                txtProcessSn.ReadOnly = true;
                txtSn.Focus();

                var mMiw = dataGridView1.SelectedCells[0].Value.ToString().Trim();
                var sql = $"SELECT * FROM udtwo WHERE IDL='{mMiw}'";
                var dr = SqlHelper.ExecuteReader(SqlHelper.GetConnection(), CommandType.Text, sql);
                dr.Read();
                txtSn.Text = dr["xuhaoone"].ToString().Trim();
                cbxCraft.Text = dr["xuhaoname"].ToString().Trim();
                txtProcessSn.Text = dr["xuhaotwo"].ToString().Trim();
                txtAdjustingTime.Text = dr["zuone"].ToString().Trim();
                txtProcessTime.Text = dr["zutwo"].ToString().Trim();
                txtUprice.Text = dr["xuj1"].ToString().Trim();
                txtFormula.Text = dr["xuj2"].ToString().Trim();
                txtSubsidy.Text = dr["xuj3"].ToString().Trim();
                txtRemark.Text = dr["beizu1"].ToString().Trim();
                dr.Close();
            }
            else if (btnUpd.Text == Resources.X保存)
            {
                var sql =
                    $"UPDATE udtwo set xuhaoname='{cbxCraft.Text.Trim()}',zuone='{txtAdjustingTime.Text.Trim()}',zutwo='{txtProcessTime.Text.Trim()}',qge='{SqlHelper.UserName}',xuj1='{txtUprice.Text.Trim()}',qgri='{DateTime.Now.ToString("yyyy-MM-dd HH:mm")}'," +
                    $"xuj2='{txtFormula.Text.Trim()}',xuj3='{txtSubsidy.Text.Trim()}',beizu1='{txtRemark.Text.Trim()}' WHERE tuhao ='{txtDwgNo.Text.Trim()}'AND yema='{txtPageNo.Text.Trim()}' AND xuhaoone='{txtSn.Text.Trim()}'AND xuhaotwo='{txtProcessSn.Text.Trim()}'";
                SqlHelper.ExecuteNonQuery(SqlHelper.GetConnection(), CommandType.Text, sql);
                btnUpd.Text = Resources.X修改;
                EnableBtn(true);
                panel1.Visible = false;
                txtSn.ReadOnly = false;
                txtProcessSn.ReadOnly = false;
                txtSn.Focus();
                ClearTxtCtrl();
                BindData();
            }
            else
            {
                MessageBox.Show("请先选择行后在进行修改!!", Resources.T提示);
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            var mMi = dataGridView1.SelectedCells[0].Value.ToString().Trim();
            if (MessageBox.Show("是否要删除所选择的行", Resources.J警告, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) ==
                DialogResult.OK)
            {
                var sql = $"DELETE FROM udtwo WHERE IDL='{mMi}';" +
                          $"UPDATE udtwo SET xuhaoone=xuhaoone-1 WHERE xuhaoone>{dataGridView1.SelectedCells[1].Value.ToString().Trim()};";
                SqlHelper.ExecuteNonQuery(SqlHelper.GetConnection(), CommandType.Text, sql);
                BindData();
            }
            else
            {
                BindData();
            }
        }

        private void txtUprice_TextChanged(object sender, EventArgs e)
        {
            if (!StringHelper.IsNumeric(txtUprice.Text) && !string.IsNullOrEmpty(txtUprice.Text))
            {
                MessageBox.Show(Resources.IsNotNumber, Resources.T提示);
                txtUprice.Text = "";
            }
        }

        private void txtAdjustingTime_TextChanged(object sender, EventArgs e)
        {
            if (!StringHelper.IsNumeric(txtAdjustingTime.Text) && !string.IsNullOrEmpty(txtAdjustingTime.Text))
            {
                MessageBox.Show(Resources.IsNotNumber, Resources.T提示);
                txtAdjustingTime.Text = "";
            }
        }

        private void txtProcessTime_TextChanged(object sender, EventArgs e)
        {
            if (!StringHelper.IsNumeric(txtProcessTime.Text) && !string.IsNullOrEmpty(txtProcessTime.Text))
            {
                MessageBox.Show(Resources.IsNotNumber, Resources.T提示);
                txtProcessTime.Text = "";
            }
        }

        private void txtSubsidy_TextChanged(object sender, EventArgs e)
        {
            if (!StringHelper.IsNumeric(txtSubsidy.Text) && !string.IsNullOrEmpty(txtSubsidy.Text))
            {
                MessageBox.Show(Resources.IsNotNumber, Resources.T提示);
                txtSubsidy.Text = "";
            }
        }

        private void txtFormula_TextChanged(object sender, EventArgs e)
        {
            if (!StringHelper.IsNumeric(txtFormula.Text) && !string.IsNullOrEmpty(txtFormula.Text))
            {
                MessageBox.Show("只允许输入正整数数字，请重新输入！", Resources.T提示);
                txtFormula.Text = "";
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (txtDwgNo.Text == "" || txtPageNo.Text == "")
            {
                MessageBox.Show(@"图号,页码均不能为空，请重新输入！", Resources.T提示);
            }
            else
            {
                var sql =
                    $"SELECT TOP 1 * FROM udone WHERE tuhao='{txtDwgNo.Text.Trim()}' AND kehu='{txtCust.Text.Trim()}' AND yema='{txtPageNo.Text.Trim()}'";
                var dr = SqlHelper.ExecuteReader(SqlHelper.GetConnection(), CommandType.Text, sql);
                if (dr.HasRows)
                {
                    dr.Read();
                    txtPartName.Text = dr["name"].ToString().Trim();
                    txtMaterial.Text = dr["cailiao"].ToString().Trim();
                    dr.Close();
                    VisibeCtrl();
                    txtSn.Focus();
                    BindData();
                }
                else
                {
                    dr.Close();
                }
                sql = $"SELECT TOP 1 * FROM udtwo WHERE tuhao='{txtDwgNo.Text.Trim()}'";
                dr.Close();
                dr = SqlHelper.ExecuteReader(SqlHelper.GetConnection(), CommandType.Text, sql);
                if (dr.HasRows)
                {
                    dr.Close();
                    VisibeCtrl();
                    txtSn.Focus();
                    BindData();
                }
                else
                {
                    dr.Close();
                    if (
                        MessageBox.Show("此图号未收过单，是否创建？", Resources.J警告, MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Warning) == DialogResult.OK)
                    {
                        VisibeCtrl();
                        txtSn.Focus();
                        BindData();
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            btnUpd.Text = Resources.X修改;
            btnAdd.Text = Resources.A新增;
            BindData();
            EnableBtn(true);
            ClearTxtCtrl();
            txtSn.Focus();
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            groupBox3.Visible = false;
            txtWONo.ReadOnly = false;
            txtDwgNo.ReadOnly = false;
            txtPageNo.ReadOnly = false;
            txtWONo.Text = "";
            txtCraftSeq.Text = "";
            txtDwgNo.Text = "";
            txtPageNo.Text = "";
            ClearTxtCtrl();
            BindData();
            Dgv2BindData();
            txtWONo.Focus();
        }

        private void ClearTxtCtrl()
        {
            txtSn.Text = "";
            txtProcessSn.Text = "";
            txtAdjustingTime.Text = "";
            txtProcessTime.Text = "";
            txtUprice.Text = "";
            txtFormula.Text = "";
            txtSubsidy.Text = "";
            txtCust.Text = "";
            txtPartName.Text = "";
            txtMaterial.Text = "";
            txtRemark.Text = "";
            cbxCraft.Text = "";
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (groupBox3.Visible == false)
            {
                var mMi = dataGridView2.SelectedCells[0].Value.ToString().Trim();
                var zMi = dataGridView2.SelectedCells[1].Value.ToString().Trim();
                txtDwgNo.Text = mMi;
                txtPageNo.Text = zMi;
            }
            btnSearch_Click(sender, e);
        }

        private void btnUprice_Click(object sender, EventArgs e)
        {
            var frmUprice = new FrmProcCardUprice();
            frmUprice.ShowDialog();
        }

        private void txtProcessTime_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (cbxCraft.Text != "")
                {
                    if (txtProcessTime.Text == "")
                    {
                        MessageBox.Show("单件加工时间不能为空", Resources.T提示);
                        txtProcessTime.Focus();
                    }
                    else
                    {
                        var sql =
                            string.Format(
                                "SELECT TOP 1 * FROM udodt WHERE iname='{0}' AND qtime<='{1}'AND ztime>='{1}'",
                                cbxCraft.Text.Trim(), txtProcessTime.Text.Trim());
                        var dr = SqlHelper.ExecuteReader(SqlHelper.GetConnection(), CommandType.Text, sql);
                        if (dr.HasRows)
                        {
                            dr.Read();
                            var hyj = dr["jilu"].ToString().Trim();
                            var ayj = dr["jisu"].ToString().Trim();
                            dr.Close();
                            var huj = decimal.Parse(txtProcessTime.Text.Trim());
                            var kuj = huj*decimal.Parse(hyj)*decimal.Parse(ayj);
                            txtUprice.Text = kuj.ToString(CultureInfo.InvariantCulture);
                            txtSubsidy.Focus();
                        }
                        else
                        {
                            MessageBox.Show("此加工时间范围有误，请核对", Resources.T提示);
                            txtProcessTime.Text = "";
                            txtProcessTime.Focus();
                            dr.Close();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("工序名称不能为空", Resources.T提示);
                    cbxCraft.Focus();
                }
            }
        }

        /// <summary>
        ///     插入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnIns_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtWONo.Text))
            {
                MessageBox.Show(@"工单号不能为空！", Resources.T提示);
                txtWONo.Focus();
            }
            else if (string.IsNullOrEmpty(txtSn.Text))
            {
                MessageBox.Show(@"序号不能为空！", Resources.T提示);
                txtSn.Focus();
            }
            else
            {
                var sql =
                    $"UPDATE udtwo SET xuhaoone=xuhaoone+1 WHERE xuhaoone>='{txtSn.Text.Trim()}';" +
                    "INSERT INTO udtwo(DeptId,tuhao,yema,xuhaoone,xuhaotwo,xuhaoname,zuone,zutwo,xuj1,xuj2,xuj3," +
                    $"beizu1,cje) values('{SqlHelper.DeptId}','{txtDwgNo.Text.Trim()}','{txtPageNo.Text.Trim()}',{txtSn.Text.Trim()}," +
                    $"{txtProcessSn.Text.Trim()},'{cbxCraft.Text.Trim()}','{txtAdjustingTime.Text.Trim()}'," +
                    $"'{txtProcessTime.Text.Trim()}','{txtUprice.Text.Trim()}','{txtFormula.Text.Trim()}'," +
                    $"'{txtSubsidy.Text.Trim()}','{txtRemark.Text.Trim()}','{SqlHelper.UserName}')";
                SqlHelper.ExecuteNonQuery(SqlHelper.GetConnection(), CommandType.Text, sql);
                btnAdd.Text = Resources.A新增;
                var seq = int.Parse(txtSn.Text);
                EnableBtn(true);
                ClearTxtCtrl();
                BindData();
                Dgv2BindData();
                txtSn.Text = (seq + 1).ToString();
                cbxCraft.Focus();
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Debug.Assert(dataGridView1.CurrentRow != null, "dataGridView1.CurrentRow != null");
            txtSn.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            //var vKeyEventArgs = new KeyEventArgs(Keys.Enter);
            //txtSn_KeyDown(sender, vKeyEventArgs);
        }

        private void txtCraftSeq_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCraftSeq.Text.Trim())) return;
            if (!ValidateUtil.IsNumber(txtCraftSeq.Text))
            {
                MessageBox.Show(Resources.IsNotNumber);
                return;
            }
            if (txtWONo.Text.Length != 7) return;
            var sql =
                $"SELECT TOP 1 * FROM udone WHERE sgdhao='{txtWONo.Text.Trim()}' AND xuhao='{int.Parse(txtCraftSeq.Text)}' AND DeptId='{SqlHelper.DeptId}'";
            var dr = SqlHelper.ExecuteReader(SqlHelper.GetConnection(), CommandType.Text, sql);
            if (dr.HasRows)
            {
                dr.Read();
                txtCust.Text = dr["kehu"].ToString().Trim();
                txtMaterial.Text = dr["cailiao"].ToString().Trim();
                txtDwgNo.Text = dr["tuhao"].ToString().Trim();
                txtPartName.Text = dr["name"].ToString().Trim();
                txtPageNo.Text = dr["yema"].ToString().Trim();
                dr.Close();
                VisibeCtrl();
                txtSn.Text = @"1";
                txtSn.Focus();
                BindData();
            }
            else
            {
                MessageBox.Show($"此工序号{txtCraftSeq.Text.Trim()}不存在，请重新输入!!", Resources.T提示);
                dr.Close();
                txtCraftSeq.Text = "";
                txtCraftSeq.Focus();
            }
        }

        private void VisibeCtrl()
        {
            groupBox3.Visible = true;
            panel1.Visible = true;
            txtWONo.ReadOnly = true;
            txtDwgNo.ReadOnly = true;
            txtPageNo.ReadOnly = true;
        }

        private void cbxCraft_Leave(object sender, EventArgs e)
        {
            txtProcessSn.Text = @"1";
        }
    }
}