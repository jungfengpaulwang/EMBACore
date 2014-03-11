using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Presentation.Controls;
using FISCA.Presentation;
using FISCA.Permission;
using System.Windows.Forms;
using FISCA.Data;
using K12.Data;
using System.Data;
using K12.Presentation;
using EMBACore.DetailItems;

namespace EMBACore.Initialization
{
    class ClassInit
    {
        public static void Init()
        {
            #region 資料項目

            /*  註冊權限  */
            Catalog detail = RoleAclSource.Instance["班級"]["資料項目"];

            detail.Add(new DetailItemFeature(typeof(Class_Brief)));

            if (UserAcl.Current[typeof(Class_Brief)].Viewable)
                NLDPanels.Class.AddDetailBulider<Class_Brief>();
            #endregion

            #region RibbonBar 班級/編輯

            //新增班級
            MotherForm.RibbonBarItems["班級", "編輯"]["新增"].Image = Properties.Resources.btnAddClass;
            MotherForm.RibbonBarItems["班級", "編輯"]["新增"].Size = RibbonBarButton.MenuButtonSize.Large;

            Catalog button_add = RoleAclSource.Instance["班級"]["功能按鈕"];
            button_add.Add(new RibbonFeature("Class_Button_AddClass", "新增班級"));
            MotherForm.RibbonBarItems["班級", "編輯"]["新增"].Enable = UserAcl.Current["Class_Button_AddClass"].Executable;

            MotherForm.RibbonBarItems["班級", "編輯"]["新增"].Click += delegate
            {
                (new Class_Add()).ShowDialog();
            };
            //刪除班級
            MotherForm.RibbonBarItems["班級", "編輯"]["刪除"].Image = Properties.Resources.btnDeleteClass;
            MotherForm.RibbonBarItems["班級", "編輯"]["刪除"].Size = RibbonBarButton.MenuButtonSize.Large;

            Catalog button_delete = RoleAclSource.Instance["班級"]["功能按鈕"];
            button_delete.Add(new RibbonFeature("Class_Button_DeleteClass", "刪除班級"));
            MotherForm.RibbonBarItems["班級", "編輯"]["刪除"].Enable = UserAcl.Current["Class_Button_DeleteClass"].Executable;

            MotherForm.RibbonBarItems["班級", "編輯"]["刪除"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Class.SelectedSource.Count == 1)
                {
                    K12.Data.ClassRecord deleteRec = K12.Data.Class.SelectByID(K12.Presentation.NLDPanels.Class.SelectedSource[0]);
                    // 當有學生
                    string msg = string.Empty;
                    if (deleteRec.Students.Count > 0)
                        msg = string.Format("確定要刪除「{0}」？班上" + deleteRec.Students.Count + "位學生將移到未分年級未分班級.", deleteRec.Name);
                    else
                        msg = string.Format("確定要刪除「{0}」？", deleteRec.Name);

                    if (FISCA.Presentation.Controls.MsgBox.Show(msg, "刪除班級", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        K12.Data.Class.Delete(deleteRec);

                        //PermRecLogProcess prlp = new PermRecLogProcess();
                        //prlp.SaveLog("學籍.班級", "刪除班級", "刪除班級資料，班級名稱：" + classRec.Name);
                        //Class.Instance.SyncDataBackground(record.ID);
                    }
                    else
                        return;
                }
                else
                    MsgBox.Show("僅能單筆刪除！");
            };
            #endregion
        }
    }
}
