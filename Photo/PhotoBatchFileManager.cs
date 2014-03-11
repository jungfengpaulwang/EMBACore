using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace EMBA.Photo
{
    class PhotoBatchFileManager
    {
        private string _FilePath;
        private List<FileInfo> _Files;
        private Dictionary<DirectoryInfo, List<FileInfo>> _DefaultFolderAndFilesInfo;
        private Dictionary<string, List<string>> _DefaultFolederAndFilesName;
        private Dictionary<string, List<int>> _DefaultFolederAndFilesNameForClassNameSeatNo;

        /// <summary>
        /// 取得檔案路徑
        /// </summary>
        /// <returns></returns>
        public string GetFilePath()
        {
            return _FilePath.Trim().ToUpper();
        }

        /// <summary>
        /// 設定檔案路徑
        /// </summary>
        /// <param name="pathName"></param>
        public void SetFilePath(string pathName)
        {
            _FilePath = pathName.Trim().ToUpper();
        }

        public PhotoBatchFileManager()
        {
            _Files = new List<FileInfo>();
            _DefaultFolderAndFilesInfo = new Dictionary<DirectoryInfo, List<FileInfo>>();
            _DefaultFolederAndFilesName = new Dictionary<string, List<string>>();
            _DefaultFolederAndFilesNameForClassNameSeatNo = new Dictionary<string, List<int>>();
        }

        public void SaveFiles(Dictionary<string, Bitmap> SaveFileDic)
        {
            foreach (KeyValuePair<string, Bitmap> file in SaveFileDic)
            {
                file.Value.Save(file.Key, System.Drawing.Imaging.ImageFormat.Jpeg);
            }

        }

        /// <summary>
        /// 取得目前資料夾內的完整資料夾與檔案
        /// </summary>
        /// <returns></returns>
        public Dictionary<DirectoryInfo, List<FileInfo>> GetCurrentFullFoldersAndFilesInfo()
        {
            return _DefaultFolderAndFilesInfo;
        }

        /// <summary>
        /// 設定目前資料夾內的完整資料夾與檔案
        /// </summary>
        /// <param name="FolderPath"></param>
        public bool SetCurrentFullFoldersAndFilesInfo(string FolderPath)
        {
            bool checkSetPass = false;
            _DefaultFolderAndFilesInfo.Clear();
            _DefaultFolederAndFilesName.Clear();
            _DefaultFolederAndFilesNameForClassNameSeatNo.Clear();

            if (checkHasFolder(FolderPath))
            {

                DirectoryInfo dirInfo = new DirectoryInfo(FolderPath);
                DirectoryInfo[] dicInfoArray = dirInfo.GetDirectories();
                foreach (DirectoryInfo di in dicInfoArray)
                {
                    List<int> IntFileNameList = new List<int>();
                    List<string> filesName = new List<string>();
                    List<FileInfo> FilesInfo = new List<FileInfo>();
                    foreach (FileInfo fi in di.GetFiles())
                    {
                        int num = 0;
                        filesName.Add(fi.Name.Trim().ToUpper());
                        FilesInfo.Add(fi);
                        int.TryParse(fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length), out num);
                        IntFileNameList.Add(num);
                    }
                    _DefaultFolderAndFilesInfo.Add(di, FilesInfo);
                    _DefaultFolederAndFilesName.Add(di.Name, filesName);
                    _DefaultFolederAndFilesNameForClassNameSeatNo.Add(di.Name, IntFileNameList);
                }
                checkSetPass = true;
            }
            return checkSetPass;
        }

        /// <summary>
        /// 檢查資料夾名稱與檔名是否存在目前目錄內
        /// </summary>
        /// <param name="FolderName"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public bool CheckFolderAndFileInCurrent(string FolderName, string FileName)
        {
            bool returnValue = false, check1 = false, check2 = false;

            if (!string.IsNullOrEmpty(FolderName))
                if (_DefaultFolederAndFilesName.ContainsKey(FolderName))
                {
                    check1 = true;
                    if (!string.IsNullOrEmpty(FileName))
                        if (_DefaultFolederAndFilesName[FolderName].Contains(FileName.Trim().ToUpper()))
                        {
                            check2 = true;
                        }
                }
            if (check1 == true && check2 == true)
                returnValue = true;

            return returnValue;
        }

        /// <summary>
        /// 檢查資料夾名稱與檔名是否存在目前目錄內(班級座號檢查用)
        /// </summary>
        /// <param name="FolderName"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public bool CheckFolderAndFileInCurrentForClassNameSeatNo(string ClassName, string SeatNo)
        {
            bool returnValue = false, check1 = false, check2 = false;
            if (!string.IsNullOrEmpty(ClassName))
                if (_DefaultFolederAndFilesNameForClassNameSeatNo.ContainsKey(ClassName.Trim().ToUpper()))
                {
                    check1 = true;
                    int IntSeatNo = 0;
                    int.TryParse(SeatNo, out IntSeatNo);
                    if (!string.IsNullOrEmpty(SeatNo))
                        if (_DefaultFolederAndFilesNameForClassNameSeatNo[ClassName.Trim().ToUpper()].Contains(IntSeatNo))
                        {
                            check2 = true;
                        }
                }
            if (check1 == true && check2 == true)
                returnValue = true;

            return returnValue;
        }


        /// <summary>
        /// 取得檔案資訊
        /// </summary>
        /// <returns></returns>
        public List<FileInfo> GetFiles()
        {
            _Files.Clear();

            if (string.IsNullOrEmpty(_FilePath.Trim()))
            {
                MessageBox.Show("請選擇存放照片的資料夾。");
                return null;
            }

            DirectoryInfo dirInfo;
            try
            {
                dirInfo = new DirectoryInfo(_FilePath);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }

            if (!dirInfo.Exists)
            {
                MessageBox.Show("資料夾不存在", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            FileInfo[] file1 = dirInfo.GetFiles("*.jpg", SearchOption.AllDirectories);
            FileInfo[] file2 = dirInfo.GetFiles("*.jpeg", SearchOption.AllDirectories);

            _Files.AddRange(file1);
            _Files.AddRange(file2);

            return _Files;
        }

        /// <summary>
        /// 檢查目錄是否存在
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public bool checkHasFolder(string folderPath)
        {
            bool check = true;

            DirectoryInfo dirInfo;
            try
            {
                dirInfo = new DirectoryInfo(folderPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

            if (!dirInfo.Exists)
                check = false;
            return check;
        }

        /// <summary>
        /// 建立目錄,目錄已存在不建立
        /// </summary>
        /// <param name="FolderPath"></param>
        public void CreateFolder(string FolderPath)
        {
            // 當目錄不存在
            if (checkHasFolder(FolderPath) == false)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(FolderPath);
                dirInfo.Create();
            }
        }

        /// <summary>
        /// 透過 Dialog 取得所選的檔案路徑
        /// </summary>
        /// <returns></returns>
        public string GetFilefoldrBrowserDialog()
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            if (folderBrowser.ShowDialog() != DialogResult.OK)
                _FilePath = string.Empty;
            else
                _FilePath = folderBrowser.SelectedPath;
            return _FilePath;
        }

        /// <summary>
        /// 取得檔案資訊
        /// </summary>
        /// <returns></returns>
        public List<FileInfo> GetFileInfo()
        {
            List<FileInfo> FileInfos = new List<FileInfo>();
            if (_Files.Count > 0)
                FileInfos = _Files;
            else
                FileInfos = GetFiles();

            return FileInfos;
        }

        /// <summary>
        /// 檢查是否有同名
        /// </summary>
        /// <returns></returns>
        public bool checkHasSameName()
        {
            bool check = false;
            List<string> fileName1 = new List<string>();
            foreach (FileInfo fi in GetFileInfo())
            {
                fileName1.Add(fi.Name);
            }


            string fName = "";
            fileName1.Sort();
            foreach (string str in fileName1)
            {
                if (str == fName)
                {
                    check = true;
                    break;
                }
                fName = str;
            }
            return check;
        }
    }
}
