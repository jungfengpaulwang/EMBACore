using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using FISCA.DSAUtil;
using K12.Data;
using K12.Data.Utility;
using System.Text;
using FISCA.LogAgent;

namespace EMBA.Photo
{
    public class Transfer
    {
        /// <summary>
        /// 匯入照片
        /// </summary>
        public static void ImportPhotos(List<StudPhotoEntity> StudPhotoEntityList)
        {
            if (StudPhotoEntityList.Count > 0)
            {
                BackgroundWorker bgImportWorker = new BackgroundWorker();
                bgImportWorker.DoWork += new DoWorkEventHandler(bgImportWorker_DoWork);
                bgImportWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgImportWorker_RunWorkerCompleted);
                bgImportWorker.RunWorkerAsync(StudPhotoEntityList);
            }
            //EditStudent.UpdatePhoto();
        }



        static void bgImportWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //MessageBox.Show("匯入完成");
        }

        static void Log(StudPhotoEntity Student)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("學生「" + Student.StudentName + "」，學號「" + Student.StudentNumber + "」");
            sb.AppendLine("被匯入「" + Student._PhotoKind.ToString() + "」照片。");

            ApplicationLog.Log("照片.匯入", "匯入", "student", Student.StudentID, sb.ToString());
        }

        static void bgImportWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //K12.Data.Photo.UpdateFreshmanPhoto(
            List<StudPhotoEntity> StudPhotoEntityList = (List<StudPhotoEntity>)e.Argument;
            DSXmlHelper xmlHelper = new DSXmlHelper("Request");
            foreach (StudPhotoEntity spe in StudPhotoEntityList)
            {
                string b64 = string.Empty;
                Bitmap pic = Photo.Resize(spe.PhotoFileInfo);
                if (spe._PhotoKind == StudPhotoEntity.PhotoKind.入學)                
                    K12.Data.Photo.UpdateFreshmanPhoto(pic, spe.StudentID);
                    //xmlHelper.AddElement("Student", "FreshmanPhoto", b64);
                if (spe._PhotoKind == StudPhotoEntity.PhotoKind.畢業)
                    K12.Data.Photo.UpdateGraduatePhoto(pic, spe.StudentID);

                Log(spe);
            }
                //if (spe._PhotoNameRule == StudPhotoEntity.PhotoNameRule.身分證號)
                //    xmlHelper.AddElement("Student", "IDNumber", spe.GetPhotoName());

                //if (spe._PhotoNameRule == StudPhotoEntity.PhotoNameRule.學號)
                //    xmlHelper.AddElement("Student", "StudentNumber", spe.GetPhotoName());

                // 班級姓名轉成身分證方
                //if (spe._PhotoNameRule == StudPhotoEntity.PhotoNameRule.班級姓名)
                //{
                //    // 先用身分證
                //    if (!string.IsNullOrEmpty(spe.StudentIDNumber))
                //        xmlHelper.AddElement("Student", "IDNumber", spe.StudentIDNumber);
                //    else
                //    {
                //        // 用學號
                //        if (!string.IsNullOrEmpty(spe.StudentNumber))
                //            xmlHelper.AddElement("Student", "StudentNumber", spe.StudentNumber);

                //    }
                //}
            //}

            //try
            //{
            //    DSAServices.CallService("SmartSchool.Student.UpdatePhoto", new DSRequest(xmlHelper.BaseElement));
                
            //    //EditStudent.UpdatePhoto(new DSRequest(xmlHelper.BaseElement));
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("上傳照片發生錯誤!");
            //    // 待寫
            //    return;
            //}
        }


        /// <summary>
        /// 匯出照片
        /// </summary>
        public static void ExportPhotos()
        {

        }



        /// <summary>
        /// 設定學生基本資訊
        /// </summary>
        /// <param name="StudPhtoEntityList"></param>
        /// <param name="_PhotoNameRule"></param>
        public static List<StudPhotoEntity> SetStudBaseInfo(StudPhotoEntity.PhotoNameRule PhotoNameRule,List<StudPhotoEntity> StudPhtoEntityList)
        {
            //Student.Instance.SyncAllBackground();
            Dictionary<string, StudentRecord> StudIdx = new Dictionary<string, StudentRecord>();

            List<StudentRecord> Students = Student.SelectAll();

            //學號
            if (PhotoNameRule == StudPhotoEntity.PhotoNameRule.學號)
            {
                foreach (StudentRecord studRec in Students)
                    if (!StudIdx.ContainsKey(studRec.StudentNumber.Trim().ToUpper()))
                        StudIdx.Add(studRec.StudentNumber.Trim().ToUpper(), studRec);
            }

            //// 身分證
            //if (PhotoNameRule == StudPhotoEntity.PhotoNameRule.身分證號)
            //{
            //    foreach (StudentRecord studRec in Students)
            //        if (!StudIdx.ContainsKey(studRec.IDNumber.Trim().ToUpper()))
            //            StudIdx.Add(studRec.IDNumber.Trim().ToUpper(), studRec);
            //}

            //// 班座
            //if (PhotoNameRule == StudPhotoEntity.PhotoNameRule.班級姓名)
            //{
            //    foreach (StudentRecord studRec in Students)
            //    {
            //        if (studRec.Class != null)
            //        {
            //            string key = studRec.Class.Name.Trim() + K12.Data.Int.GetString(studRec.SeatNo);
            //            if (!StudIdx.ContainsKey(key))
            //                StudIdx.Add(key, studRec);
            //        }
            //    }
            //}

            // 有符合以上3類填入值
            foreach (StudPhotoEntity spe in StudPhtoEntityList)
            {
                string PhotoName = spe.GetPhotoName();
                //Dictionary<string, StudentRecord> StudIdx_Upper = new Dictionary<string, StudentRecord>();
                //if (StudIdx.Count > 0)
                //{
                //    foreach (string key in StudIdx.Keys)
                //        StudIdx_Upper.Add(key.ToUpper(), StudIdx[key]);
                //}
                if (StudIdx.ContainsKey(PhotoName))
                {
                    if (StudIdx[PhotoName].Class != null)
                        spe.ClassName = StudIdx[PhotoName].Class.Name;

                    //spe.SeatNo = K12.Data.Int.GetString(StudIdx[PhotoName].SeatNo);
                    spe.StudentID = StudIdx[PhotoName].ID;
                    spe.StudentIDNumber = StudIdx[PhotoName].IDNumber;
                    spe.StudentName = StudIdx[PhotoName].Name;
                    spe.StudentNumber = StudIdx[PhotoName].StudentNumber;
                }
            }
            return StudPhtoEntityList;
        }

        /// <summary>
        /// 取得詳細資料列表
        /// </summary>
        /// <param name="id">學生編號</param>
        /// <returns></returns>
        [FISCA.Authentication.AutoRetryOnWebException()]
        private static DSResponse GetDetailList(IEnumerable<string> fields, params string[] list)
        {
            DSRequest dsreq = new DSRequest();
            DSXmlHelper helper = new DSXmlHelper("GetStudentListRequest");
            helper.AddElement("Field");
            bool hasfield = false;
            foreach (string field in fields)
            {
                helper.AddElement("Field", field);
                hasfield = true;
            }
            if (!hasfield)
                throw new Exception("必須傳入Field");
            helper.AddElement("Condition");
            foreach (string id in list)
            {
                helper.AddElement("Condition", "ID", id);
            }
            dsreq.SetContent(helper);
            return K12.Data.Utility.DSAServices.CallService("SmartSchool.Student.GetDetailList", dsreq);
        }

        public static List<StudPhotoEntity> GetStudentPhotoBitmap(List<StudPhotoEntity> StudPhotoEntityList)
        {
            List<string> StudentIDList = new List<string>();
            foreach (StudPhotoEntity spe in StudPhotoEntityList)
                if (!string.IsNullOrEmpty(spe.StudentID))
                    StudentIDList.Add(spe.StudentID);

            DSXmlHelper xmlHelper = new DSXmlHelper("Request");

            DSResponse DSRsp = GetDetailList(new string[] { "ID", "FreshmanPhoto", "GraduatePhoto" }, StudentIDList.ToArray());

            Dictionary<string, string> FreshmanPhotoStr = new Dictionary<string, string>();
            Dictionary<string, string> GraduatePhotoStr = new Dictionary<string, string>();

            if (DSRsp != null)
                foreach (XmlElement elm in DSRsp.GetContent().BaseElement.SelectNodes("Student"))
                {
                    if (!FreshmanPhotoStr.ContainsKey(elm.GetAttribute("ID")))
                    {
                        if (!string.IsNullOrEmpty(elm.SelectSingleNode("FreshmanPhoto").InnerText))
                            FreshmanPhotoStr.Add(elm.GetAttribute("ID"), elm.SelectSingleNode("FreshmanPhoto").InnerText);
                    }
                    if (!GraduatePhotoStr.ContainsKey(elm.GetAttribute("ID")))
                    {
                        if (!string.IsNullOrEmpty(elm.SelectSingleNode("GraduatePhoto").InnerText))
                            GraduatePhotoStr.Add(elm.GetAttribute("ID"), elm.SelectSingleNode("GraduatePhoto").InnerText);
                    }

                }

            foreach (StudPhotoEntity spe in StudPhotoEntityList)
            {
                if (spe._PhotoKind == StudPhotoEntity.PhotoKind.入學)
                {
                    if (FreshmanPhotoStr.ContainsKey(spe.StudentID))
                    {
                        spe.FreshmanPhotoBitmap = Photo.ConvertFromBase64Encoding(FreshmanPhotoStr[spe.StudentID], true);
                    }
                }

                if (spe._PhotoKind == StudPhotoEntity.PhotoKind.畢業)
                {
                    if (GraduatePhotoStr.ContainsKey(spe.StudentID))
                    {
                        spe.GraduatePhotoBitmap = Photo.ConvertFromBase64Encoding(GraduatePhotoStr[spe.StudentID], true);
                    }

                }
            }

            return StudPhotoEntityList;
        }
    }
}
