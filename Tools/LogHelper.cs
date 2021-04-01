using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Tools
{
    public class FileOperate
    {

        #region 写文件
        /****************************************
         * 函数名称：WriteFile
         * 功能说明：当文件不存时，则创建文件，并追加文件
         * 参    数：Path:文件路径,Strings:文本内容
         * 调用示列：
         *           string Path = Server.MapPath("Default2.aspx");       
         *           string Strings = "这是我写的内容啊";
         *           DotNet.Utilities.FileOperate.WriteFile(Path,Strings);
        *****************************************/
        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="Path">文件路径</param>
        /// <param name="Strings">文件内容</param>
        public static void WriteFile(string Path, string Strings)
        {
            try
            {
                FolderCreate(Path.Substring(0, Path.LastIndexOf('\\')));
                if (!System.IO.File.Exists(Path))
                {
                    System.IO.FileStream f = System.IO.File.Create(Path);
                    f.Close();
                    f.Dispose();
                }
                System.IO.StreamWriter f2 = new System.IO.StreamWriter(Path, true, System.Text.Encoding.UTF8);
                f2.WriteLine(Strings);
                f2.Close();
                f2.Dispose();
            }
            catch (Exception ex)
            {

                throw;
            }



        }
        #endregion

        #region 在当前目录下创建目录
        /****************************************
         * 函数名称：FolderCreate
         * 功能说明：在当前目录下创建目录
         * 参    数：OrignFolder:当前目录,NewFloder:新目录
         * 调用示列：
         *           string OrignFolder = Server.MapPath("test/");    
         *           string NewFloder = "new";
         *           DotNet.Utilities.FileOperate.FolderCreate(OrignFolder, NewFloder); 
        *****************************************/
        /// <summary>
        /// 在当前目录下创建目录
        /// </summary>
        /// <param name="OrignFolder">当前目录</param>
        /// <param name="NewFloder">新目录</param>
        public static void FolderCreate(string OrignFolder, string NewFloder)
        {
            Directory.SetCurrentDirectory(OrignFolder);
            Directory.CreateDirectory(NewFloder);
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="Path"></param>
        public static void FolderCreate(string Path)
        {
            // 判断目标目录是否存在如果不存在则新建之
            if (!Directory.Exists(Path))
                Directory.CreateDirectory(Path);
        }

        #endregion
    }

    public static class LogHelper
    {
        /// <summary>
        /// 日志记录表,会以天为单位来创建
        /// </summary>
        /// <param name="strMain">主内容:记录主要的操作位置</param>
        /// <param name="strNote">内容信息:记录要跟踪的数据以文本类型输出</param>
        /// <param name="bIsBug">是否启用调试状态.1:写入日志,0:不写入日志</param>
        public static void LogWriteBug(string strMain, string strNote, string bIsBug)
        {
            try
            {
                if (bIsBug == "1")
                {
                    string strFilePath = HttpContext.Current.Server.MapPath(string.Format(@"\fileServer\Logs\{0}\{1}\log", DateTime.Now.Year, DateTime.Now.Month) + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
                    FileOperate.WriteFile(strFilePath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + strMain + strNote);
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
