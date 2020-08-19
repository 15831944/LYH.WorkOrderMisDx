using System;
using System.Data;
using System.Windows.Forms;
using System.Collections.Generic;
using LYH.Framework.BaseUI;
using LYH.Framework.Commons;
using LYH.Framework.ControlUtil;

using WorkOrder.ProcCard.BLL;
using WorkOrder.ProcCard.Entity;

namespace WorkOrder.ProcCard.UI
{
	/// <summary>
	/// Ա������
	/// </summary>	
	public partial class FrmProcCardEmployee : BaseDock
	{
		public FrmProcCardEmployee()
		{
			InitializeComponent();

			InitDictItem();

			winGridViewPager1.OnPageChanged += winGridViewPager1_OnPageChanged;
			winGridViewPager1.OnStartExport += winGridViewPager1_OnStartExport;
			winGridViewPager1.OnEditSelected += winGridViewPager1_OnEditSelected;
			winGridViewPager1.OnAddNew += winGridViewPager1_OnAddNew;
			winGridViewPager1.OnDeleteSelected += winGridViewPager1_OnDeleteSelected;
			winGridViewPager1.OnRefresh += winGridViewPager1_OnRefresh;
			winGridViewPager1.AppendedMenu = contextMenuStrip1;
			winGridViewPager1.ShowLineNumber = true;
			winGridViewPager1.BestFitColumnWith = false;//�Ƿ�����Ϊ�Զ�������ȣ�falseΪ������
			winGridViewPager1.gridView1.DataSourceChanged +=gridView1_DataSourceChanged;
			winGridViewPager1.gridView1.CustomColumnDisplayText += gridView1_CustomColumnDisplayText;
			winGridViewPager1.gridView1.RowCellStyle += gridView1_RowCellStyle;

			//�����س������в�ѯ
			foreach (Control control in layoutControl1.Controls)
			{
				control.KeyUp += SearchControl_KeyUp;
			}
		}
		void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
		{
			//if (e.Column.FieldName == "OrderStatus")
			//{
			//    string status = this.winGridViewPager1.gridView1.GetRowCellValue(e.RowHandle, "OrderStatus").ToString();
			//    Color color = Color.White;
			//    if (status == "�����")
			//    {
			//        e.Appearance.BackColor = Color.Red;
			//        e.Appearance.BackColor2 = Color.LightCyan;
			//    }
			//}
		}
		void gridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
		{
			string columnName = e.Column.FieldName;
			if (e.Column.ColumnType == typeof(DateTime))
			{   
				if (e.Value != null)
				{
					if (e.Value == DBNull.Value || Convert.ToDateTime(e.Value) <= Convert.ToDateTime("1900-1-1"))
					{
						e.DisplayText = "";
					}
					else
					{
						e.DisplayText = Convert.ToDateTime(e.Value).ToString("yyyy-MM-dd HH:mm");//yyyy-MM-dd
					}
				}
			}
			//else if (columnName == "Age")
			//{
			//    e.DisplayText = string.Format("{0}��", e.Value);
			//}
			//else if (columnName == "ReceivedMoney")
			//{
			//    if (e.Value != null)
			//    {
			//        e.DisplayText = e.Value.ToString().ToDecimal().ToString("C");
			//    }
			//}
		}
		
		/// <summary>
		/// �����ݺ󣬷�����еĿ��
		/// </summary>
		private void gridView1_DataSourceChanged(object sender, EventArgs e)
		{
			if (winGridViewPager1.gridView1.Columns.Count > 0 && winGridViewPager1.gridView1.RowCount > 0)
			{
				//ͳһ����100���
				foreach (DevExpress.XtraGrid.Columns.GridColumn column in winGridViewPager1.gridView1.Columns)
				{
					column.Width = 100;
				}

				//�����������ر�Ŀ��
				//SetGridColumWidth("Note", 200);
			}
		}

		private void SetGridColumWidth(string columnName, int width)
		{
			DevExpress.XtraGrid.Columns.GridColumn column = winGridViewPager1.gridView1.Columns.ColumnByFieldName(columnName);
			if (column != null)
			{
				column.Width = width;
			}
		}

		/// <summary>
		/// ��д��ʼ�������ʵ�֣���������ˢ��
		/// </summary>
		public override void  FormOnLoad()
		{   
			BindData();
		}
		
		/// <summary>
		/// ��ʼ���ֵ��б�����
		/// </summary>
		private void InitDictItem()
		{
			//��ʼ������
		}
		
		/// <summary>
		/// ��ҳ�ؼ�ˢ�²���
		/// </summary>
		private void winGridViewPager1_OnRefresh(object sender, EventArgs e)
		{
			BindData();
		}
		
		/// <summary>
		/// ��ҳ�ؼ�ɾ������
		/// </summary>
		private void winGridViewPager1_OnDeleteSelected(object sender, EventArgs e)
		{
			if (MessageDxUtil.ShowYesNoAndTips("��ȷ��ɾ��ѡ���ļ�¼ô��") == DialogResult.No)
			{
				return;
			}

			int[] rowSelected = winGridViewPager1.GridView1.GetSelectedRows();
			foreach (int iRow in rowSelected)
			{
				string ID = winGridViewPager1.GridView1.GetRowCellDisplayText(iRow, "ID");
				BLLFactory<ProcCardEmployee>.Instance.Delete(ID);
			}
			 
			BindData();
		}
		
		/// <summary>
		/// ��ҳ�ؼ��༭�����
		/// </summary>
		private void winGridViewPager1_OnEditSelected(object sender, EventArgs e)
		{
			string ID = winGridViewPager1.gridView1.GetFocusedRowCellDisplayText("ID");
			List<string> IDList = new List<string>();
			for (int i = 0; i < winGridViewPager1.gridView1.RowCount; i++)
			{
				string strTemp = winGridViewPager1.GridView1.GetRowCellDisplayText(i, "ID");
				IDList.Add(strTemp);
			}

			if (!string.IsNullOrEmpty(ID))
			{
				FrmEditProcCardEmployee dlg = new FrmEditProcCardEmployee();
				dlg.ID = ID;
				dlg.IDList = IDList;
				dlg.OnDataSaved += dlg_OnDataSaved;
				dlg.InitFunction(LoginUserInfo, FunctionDict);//���Ӵ��帳ֵ�û�Ȩ����Ϣ
				
				if (DialogResult.OK == dlg.ShowDialog())
				{
					BindData();
				}
			}
		}        
		
		void dlg_OnDataSaved(object sender, EventArgs e)
		{
			BindData();
		}
		
		/// <summary>
		/// ��ҳ�ؼ���������
		/// </summary>        
		private void winGridViewPager1_OnAddNew(object sender, EventArgs e)
		{
			btnAddNew_Click(null, null);
		}
		
		/// <summary>
		/// ��ҳ�ؼ�ȫ����������ǰ�Ĳ���
		/// </summary> 
		private void winGridViewPager1_OnStartExport(object sender, EventArgs e)
		{
			string where = GetConditionSql();
			winGridViewPager1.AllToExport = BLLFactory<ProcCardEmployee>.Instance.FindToDataTable(where);
		 }

		/// <summary>
		/// ��ҳ�ؼ���ҳ�Ĳ���
		/// </summary> 
		private void winGridViewPager1_OnPageChanged(object sender, EventArgs e)
		{
			BindData();
		}
		
		/// <summary>
		/// �߼���ѯ����������
		/// </summary>
		private SearchCondition advanceCondition;
		
		/// <summary>
		/// ���ݲ�ѯ���������ѯ���
		/// </summary> 
		private string GetConditionSql()
		{
			//������ڸ߼���ѯ������Ϣ����ʹ�ø߼���ѯ����������ʹ������������ѯ
			SearchCondition condition = advanceCondition;
			if (condition == null)
			{
				condition = new SearchCondition();
				condition.AddCondition("Name", txtName.Text.Trim(), SqlOperator.Like);
			}
			string where = condition.BuildConditionSql().Replace("Where", "");
			return where;
		}
		
		/// <summary>
		/// ���б�����
		/// </summary>
		private void BindData()
		{
			//entity
			winGridViewPager1.DisplayColumns = "ID,TeamId,Name";
			winGridViewPager1.ColumnNameAlias = BLLFactory<ProcCardEmployee>.Instance.GetColumnNameAlias();//�ֶ�����ʾ����ת��

			#region ��ӱ�������

			//this.winGridViewPager1.AddColumnAlias("ID", "ID");
			//this.winGridViewPager1.AddColumnAlias("TeamId", "����");
			//this.winGridViewPager1.AddColumnAlias("Name", "����");

			#endregion

			string where = GetConditionSql();
				List<ProcCardEmployeeInfo> list = BLLFactory<ProcCardEmployee>.Instance.FindWithPager(where, winGridViewPager1.PagerInfo);
			winGridViewPager1.DataSource = list;//new WHC.Pager.WinControl.SortableBindingList<ProcCard_EmployeeInfo>(list);
				winGridViewPager1.PrintTitle = "Ա��������";
		 }
		
		/// <summary>
		/// ��ѯ���ݲ���
		/// </summary>
		private void btnSearch_Click(object sender, EventArgs e)
		{
			advanceCondition = null;//�������ò�ѯ������������ܻ�ʹ�ø߼���ѯ������
			BindData();
		}
		
		/// <summary>
		/// �������ݲ���
		/// </summary>
		private void btnAddNew_Click(object sender, EventArgs e)
		{
			FrmEditProcCardEmployee dlg = new FrmEditProcCardEmployee();
			dlg.OnDataSaved += dlg_OnDataSaved;
			dlg.InitFunction(LoginUserInfo, FunctionDict);//���Ӵ��帳ֵ�û�Ȩ����Ϣ
			
			if (DialogResult.OK == dlg.ShowDialog())
			{
				BindData();
			}
		}
		
		/// <summary>
		/// �ṩ���ؼ��س�ִ�в�ѯ�Ĳ���
		/// </summary>
		private void SearchControl_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				btnSearch_Click(null, null);
			}
		}        

						 
		private string moduleName = "Ա������";
		/// <summary>
		/// ����Excel�Ĳ���
		/// </summary>          
		private void btnImport_Click(object sender, EventArgs e)
		{
			string templateFile = string.Format("{0}-ģ��.xls", moduleName);
			FrmImportExcelData dlg = new FrmImportExcelData();
			dlg.SetTemplate(templateFile, System.IO.Path.Combine(Application.StartupPath, templateFile));
			dlg.OnDataSave += ExcelData_OnDataSave;
			dlg.OnRefreshData += ExcelData_OnRefreshData;
			dlg.ShowDialog();
		}

		void ExcelData_OnRefreshData(object sender, EventArgs e)
		{
			BindData();
		}

		/// <summary>
		/// ����ֶδ��ڣ����ȡ��Ӧ��ֵ�����򷵻�Ĭ�Ͽ�
		/// </summary>
		/// <param name="row">DataRow����</param>
		/// <param name="columnName">�ֶ�����</param>
		/// <returns></returns>
		private string GetRowData(DataRow row, string columnName)
		{
			string result = "";
			if (row.Table.Columns.Contains(columnName))
			{
				result = row[columnName].ToString();
			}
			return result;
		}
		
		bool ExcelData_OnDataSave(DataRow dr)
		{
			bool success = false;
			bool converted = false;
			DateTime dtDefault = Convert.ToDateTime("1900-01-01");
			DateTime dt;
			ProcCardEmployeeInfo info = new ProcCardEmployeeInfo();
			 info.TeamId = GetRowData(dr, "����").ToInt32();
			  info.Name = GetRowData(dr, "����");
			  info.Code = GetRowData(dr, "����");
  
			success = BLLFactory<ProcCardEmployee>.Instance.Insert(info);
			 return success;
		}

		/// <summary>
		/// ����Excel�Ĳ���
		/// </summary>
		private void btnExport_Click(object sender, EventArgs e)
		{
			string file = FileDialogHelper.SaveExcel(string.Format("{0}.xls", moduleName));
			if (!string.IsNullOrEmpty(file))
			{
				string where = GetConditionSql();
				List<ProcCardEmployeeInfo> list = BLLFactory<ProcCardEmployee>.Instance.Find(where);
				 DataTable dtNew = DataTableHelper.CreateTable("���|int,����,����,����");
				DataRow dr;
				int j = 1;
				for (int i = 0; i < list.Count; i++)
				{
					dr = dtNew.NewRow();
					dr["���"] = j++;
					 dr["����"] = list[i].TeamId;
					 dr["����"] = list[i].Name;
					 dr["����"] = list[i].Code;
					 dtNew.Rows.Add(dr);
				}

				try
				{
					string error = "";
					AsposeExcelTools.DataTableToExcel2(dtNew, file, out error);
					if (!string.IsNullOrEmpty(error))
					{
						MessageDxUtil.ShowError(string.Format("����Excel���ִ���{0}", error));
					}
					else
					{
						if (MessageDxUtil.ShowYesNoAndTips("�����ɹ����Ƿ���ļ���") == DialogResult.Yes)
						{
							System.Diagnostics.Process.Start(file);
						}
					}
				}
				catch (Exception ex)
				{
					LogTextHelper.Error(ex);
					MessageDxUtil.ShowError(ex.Message);
				}
			}
		 }
		 
		private FrmAdvanceSearch dlg;
		private void btnAdvanceSearch_Click(object sender, EventArgs e)
		{
			if (dlg == null)
			{
				dlg = new FrmAdvanceSearch();
				dlg.FieldTypeTable = BLLFactory<ProcCardEmployee>.Instance.GetFieldTypeList();
				dlg.ColumnNameAlias = BLLFactory<ProcCardEmployee>.Instance.GetColumnNameAlias();                
				 dlg.DisplayColumns = "TeamId,Name,Code";

				#region �����б�����

				//dlg.AddColumnListItem("UserType", Portal.gc.GetDictData("��Ա����"));//�ֵ��б�
				//dlg.AddColumnListItem("Sex", "��,Ů");//�̶��б�
				//dlg.AddColumnListItem("Credit", BLLFactory<ProcCard_Employee>.Instance.GetFieldList("Credit"));//��̬�б�

				#endregion

				dlg.ConditionChanged += dlg_ConditionChanged;
			}
			dlg.ShowDialog();
		}

		void dlg_ConditionChanged(SearchCondition condition)
		{
			advanceCondition = condition;
			BindData();
		}
	}
}
