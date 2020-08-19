using System;
using LYH.Framework.BaseUI;
using LYH.Framework.Commons;
using LYH.Framework.ControlUtil;
using WorkOrder.ProcCard.BLL;
using WorkOrder.ProcCard.Entity;

namespace WorkOrder.ProcCard.UI
{
	public partial class FrmEditProcCardEmployee : BaseEditForm
	{
		/// <summary>
		/// ����һ����ʱ���󣬷����ڸ��������л�ȡ���ڵ�GUID
		/// </summary>
		private ProcCardEmployeeInfo tempInfo = new ProcCardEmployeeInfo();
		
		public FrmEditProcCardEmployee()
		{
			InitializeComponent();
		}
				
		/// <summary>
		/// ʵ�ֿؼ�������ĺ���
		/// </summary>
		/// <returns></returns>
		public override bool CheckInput()
		{
			bool result = true;//Ĭ���ǿ���ͨ��

			#region MyRegion
			if (this.txtTeamId.Text.Trim().Length == 0)
			{
				MessageDxUtil.ShowTips("���������");
				this.txtTeamId.Focus();
				result = false;
			}
			 else if (this.txtName.Text.Trim().Length == 0)
			{
				MessageDxUtil.ShowTips("����������");
				this.txtName.Focus();
				result = false;
			}
			#endregion

			return result;
		}

		/// <summary>
		/// ��ʼ�������ֵ�
		/// </summary>
		private void InitDictItem()
		{
			//��ʼ������
		}                        

		/// <summary>
		/// ������ʾ�ĺ���
		/// </summary>
		public override void DisplayData()
		{
			InitDictItem();//�����ֵ���أ����ã�

			if (!string.IsNullOrEmpty(ID))
			{
				#region ��ʾ��Ϣ
				ProcCardEmployeeInfo info = BLLFactory<ProcCardEmployee>.Instance.FindById(ID);
				if (info != null)
				{
					tempInfo = info;//���¸���ʱ����ֵ��ʹָ֮����ڵļ�¼����
					
						txtTeamId.EditValue= info.TeamId;
							txtName.Text = info.Name;
							 } 
				#endregion
				//this.btnOK.Enabled = HasFunction("ProcCard_Employee/Edit");             
			}
			else
			{
  
				//this.btnOK.Enabled = Portal.gc.HasFunction("ProcCard_Employee/Add");  
			}
			
			//tempInfo�ڶ��������Ϊָ�������½�����ȫ�µĶ��󣬵���һЩ��ʼ����GUID���ڸ����ϴ�
			//SetAttachInfo(tempInfo);
		}

		//private void SetAttachInfo(ProcCard_EmployeeInfo info)
		//{
		//    this.attachmentGUID.AttachmentGUID = info.AttachGUID;
		//    this.attachmentGUID.userId = LoginUserInfo.Name;

		//    string name = txtName.Text;
		//    if (!string.IsNullOrEmpty(name))
		//    {
		//        string dir = string.Format("{0}", name);
		//        this.attachmentGUID.Init(dir, tempInfo.ID, LoginUserInfo.Name);
		//    }
		//}

		public override void ClearScreen()
		{
			this.tempInfo = new ProcCardEmployeeInfo();
			base.ClearScreen();
		}

		/// <summary>
		/// �༭���߱���״̬��ȡֵ����
		/// </summary>
		/// <param name="info"></param>
		private void SetInfo(ProcCardEmployeeInfo info)
		{
				info.TeamId = Convert.ToInt32(txtTeamId.EditValue);
					info.Name = txtName.Text;
			   }
		 
		/// <summary>
		/// ����״̬�µ����ݱ���
		/// </summary>
		/// <returns></returns>
		public override bool SaveAddNew()
		{
			ProcCardEmployeeInfo info = tempInfo;//����ʹ�ô��ڵľֲ���������Ϊ������Ϣ���ܱ�����ʹ��
			SetInfo(info);

			try
			{
				#region ��������

				bool succeed = BLLFactory<ProcCardEmployee>.Instance.Insert(info);
				if (succeed)
				{
					//�����������������

					return true;
				}
				#endregion
			}
			catch (Exception ex)
			{
				LogTextHelper.Error(ex);
				MessageDxUtil.ShowError(ex.Message);
			}
			return false;
		}                 

		/// <summary>
		/// �༭״̬�µ����ݱ���
		/// </summary>
		/// <returns></returns>
		public override bool SaveUpdated()
		{

			ProcCardEmployeeInfo info = BLLFactory<ProcCardEmployee>.Instance.FindById(ID);
			if (info != null)
			{
				SetInfo(info);

				try
				{
					#region ��������
					bool succeed = BLLFactory<ProcCardEmployee>.Instance.Update(info, info.ID);
					if (succeed)
					{
						//�����������������
					   
						return true;
					}
					#endregion
				}
				catch (Exception ex)
				{
					LogTextHelper.Error(ex);
					MessageDxUtil.ShowError(ex.Message);
				}
			}
		   return false;
		}
	}
}
