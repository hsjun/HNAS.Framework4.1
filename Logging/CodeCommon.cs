using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace HNAS.Framework4.Logging
{
    /// <summary>
    /// CodeCommon
    /// </summary>
    /// Copyright (c) 2012 海南海航航空信息系统有限公司
    /// 创 建 人：HNAS .Net Framework 4.0 项目组
    /// 创建日期：2012-2-1
    /// 修 改 人：
    /// 修改日期：
    /// 版 本：1.0

    class CodeCommon
    {
        public static readonly string WebUIPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\WebUI\\";//webui相关文件所在文件夹
        public static readonly string WebUIMaserPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\WebUI\\Master\\";//模板文件所在文件夹



        /// <summary>
        /// 数据表字段的命名规则
        /// </summary>
        public static string[] bMProNameRule = new string[] { "cnbi", "cnsi", "cnti", "cnde", "cnsm", "cnui", "cnvb", "cnvc", "cnim", "cnsd", "cnm", "cnc", "cnt", "cnb", "cndt", "cnd", "cnf", "cnr", "cnn", "cni", };
        private static string[] tableNameRule = new string[] { "tb" };

        #region 返回属性名
        /// <summary>
        /// 返回属性名
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        public static string GetPorName(string colName, string rule)
        {
            string porName = colName;
            if (rule == "cols")
            {
                for (int i = 0; i < bMProNameRule.Length; i++)
                {
                    if (porName.StartsWith(bMProNameRule[i]))
                    {
                        porName = colName.Remove(0, bMProNameRule[i].Length);
                        break;
                    }
                }
            }
            else if (rule == "table")
            {
                for (int i = 0; i < tableNameRule.Length; i++)
                {
                    if (porName.StartsWith(tableNameRule[i]))
                    {
                        porName = colName.Remove(0, tableNameRule[i].Length);
                        break;
                    }
                }
            }
            return porName;

        }
        #endregion

        #region 获取code格式

        /// <summary>
        /// 获取code格式
        /// </summary>
        /// <returns></returns>
        public static string GetFileMaster(string path, string fileName)
        {
            //取得文件内容 
            string filePath = path + fileName;
            string line = "";
            StreamReader sr = new StreamReader(filePath);
            if (sr != null)
            {
                line = sr.ReadToEnd();
            }
            sr.Close();
            return line;
        }

        #endregion

        #region 返回首字母小写

        /// <summary>
        /// 返回首字母小写
        /// </summary>
        /// <returns></returns>
        public static string ToFirstLowerCase(string str)
        {
            if (str.Length > 1)
            {
                str = str.Substring(0, 1).ToLower() + str.Substring(1, str.Length - 1);
            }
            else if (str.Length == 1)
            {
                str = str.ToLower();
            }
            else
            {
                return "";
            }
            return str;
        }

        #endregion

        #region 获取AD帐号
        /// <summary>
        /// 获取AD帐号
        /// </summary>
        /// <returns>获取AD帐号</returns>
        /// 创 建 人：杨栋(dong_yang1)
        /// 创建日期：2011-12-30
        public string GetLoginName()
        {
            string loginName = "";
            if (System.Security.Principal.WindowsIdentity.GetCurrent().IsAuthenticated)
            {
                loginName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                if (!string.IsNullOrEmpty(loginName) && loginName.Contains("HNANET"))
                {
                    loginName = loginName.Split('\\')[1];
                }
            }
#if DEBUG
          loginName = "wang_yu5";
#endif
            return loginName;
        }
        #endregion


        #region 复制文件夹
        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcdir">源文件夹</param>
        /// <param name="desdir">目的地文件夹</param>
        public static void CopyDirectory(string srcdir, string desdir)
        {
            string folderName = srcdir.Substring(srcdir.LastIndexOf("\\") + 1);

            string desfolderdir = desdir + "\\" + folderName;

            if (desdir.LastIndexOf("\\") == (desdir.Length - 1))
            {
                desfolderdir = desdir + folderName;
            }
            string[] filenames = Directory.GetFileSystemEntries(srcdir);

            foreach (string file in filenames)// 遍历所有的文件和目录
            {
                if (Directory.Exists(file))// 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                {

                    string currentdir = desfolderdir + "\\" + file.Substring(file.LastIndexOf("\\") + 1);
                    if (!Directory.Exists(currentdir))
                    {
                        Directory.CreateDirectory(currentdir);
                    }

                    CopyDirectory(file, desfolderdir);
                }

                else // 否则直接copy文件
                {
                    string srcfileName = file.Substring(file.LastIndexOf("\\") + 1);

                    srcfileName = desfolderdir + "\\" + srcfileName;


                    if (!Directory.Exists(desfolderdir))
                    {
                        Directory.CreateDirectory(desfolderdir);
                    }
                    if (File.Exists(srcfileName))
                    {
                        File.Delete(srcfileName);
                    }
                    File.Copy(file, srcfileName);

                }
            }
        }

        public static void CopyFile(string srcfile, string desfile)
        {
            string FileName = srcfile.Substring(srcfile.LastIndexOf("\\") + 1);

            string desfolderfile = desfile + "\\" + FileName;

            if (desfile.LastIndexOf("\\") == (desfile.Length - 1))
            {
                desfolderfile = desfile + FileName;
            }
            if (File.Exists(desfolderfile))
            {
                File.Delete(desfolderfile);
            }
            File.Copy(srcfile, desfolderfile);
        }
        #endregion

        /// <summary>
        /// 获得数据库说明中的展示部分
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetComment(string str)
        {
            if (str.Contains("$"))
            {
                str = GetBetweenString(str, "$", "$", 1);
            }
            str = InputText(str, 6);
            str = str.Replace("'", "");
            str = str.Replace("\"", "");
            str = str.Replace("=", "");
            str = str.Replace("+", "");
            str = str.Replace(";", "");
            str = str.Replace("<", "");
            str = str.Replace(">", "");
            str = str.Replace("{", "");
            str = str.Replace("}", "");
            str = str.Replace("\n", "").Replace("\r", "");
            str = Regex.Replace(str, @"[^\w]", "");

            return str;
        }

        /// <summary>
        /// 获取2个字符串之间的字符串
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="beginTag">开始字符串</param>
        /// <param name="endTag">截止字符串</param>
        /// <param name="beginOffset">开始字符串偏移量</param>
        /// <returns></returns>
        /// 创 建 人：HNAS .Net Framework 4.0 项目组
        /// 创建日期：2011-12-5
        /// 修 改 人：
        /// 修改日期：
        public static string GetBetweenString(string str, string beginTag, string endTag, int beginOffset)
        {
            string str2 = "";


            int startIndex = str.IndexOf(beginTag);
            int endIndex = str.LastIndexOf(endTag);
            int length = endIndex - (startIndex);
            if (length <= -1)
                return "";
            else
                return str2 = str.Substring(startIndex + beginOffset, length - beginOffset);


        }

        /// <summary>
        /// Method to make sure that user's inputs are not malicious
        /// </summary>
        /// <param name="text">User's Input</param>
        /// <param name="maxLength">Maximum length of input</param>
        /// <returns>The cleaned up version of the input</returns>
        public static string InputText(string text, int maxLength)
        {
            text = text.Trim();
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            if (text.Length > maxLength)
                text = text.Substring(0, maxLength);
            text = Regex.Replace(text, "[\\s]{2,}", " ");	//two or more spaces
            text = Regex.Replace(text, "(<[b|B][r|R]/*>)+|(<[p|P](.|\\n)*?>)", "\n");	//<br>
            text = Regex.Replace(text, "(\\s*&[n|N][b|B][s|S][p|P];\\s*)+", " ");	//&nbsp;
            text = Regex.Replace(text, "<(.|\\n)*?>", string.Empty);	//any other tags
            text = text.Replace("'", "''");
            return text;
        }

        /// <summary>
        /// WriteCodeToFile
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="pathAndName"></param>
        /// <param name="csFileName"></param>
        /// <param name="fileType"></param>
        public static void WriteCodeToFile(string inputs, string pathAndName, string csFileName, string fileType = ".cs")
        {
            FileInfo fi = new FileInfo(pathAndName + "\\" + csFileName + fileType);
            // FileIOPermission fileIOPermission = new FileIOPermission(FileIOPermissionAccess.Write | FileIOPermissionAccess.Read, pathAndName);
            if (fi.Exists)
            {
                fi.Delete();
            }
            using (FileStream fs = fi.OpenWrite())
            {
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                sw.BaseStream.Seek(0, SeekOrigin.Begin);
                sw.Write(inputs);
                sw.Flush();
                sw.Close();
            }
        }

        /// <summary>
        /// CreateFile
        /// </summary>
        /// <param name="pathAndName"></param>
        public static void CreateFile(string pathAndName)
        {
            if (!Directory.Exists(pathAndName))
            {
                Directory.CreateDirectory(pathAndName);
            }
        }

        /// <summary>
        /// DbTypeToCS
        /// </summary>
        /// <param name="dbtype"></param>
        /// <returns></returns>
        public static string DbTypeToCS(string dbtype)
        {
            switch (dbtype.ToLower().Trim())
            {
                case "varchar":
                case "varchar2":
                case "nvarchar":
                case "nvarchar2":
                case "char":
                case "nchar":
                case "text":
                case "ntext":
                case "string":
                case "xml":
                    return "string";
                case "date":
                case "datetime":
                case "smalldatetime":
                case "timestamp":
                case "DateTime":
                    return "DateTime?";
                case "smallint":
                case "int":
                case "number":
                    return "int?";
                case "tinyint":
                    return "byte?";
                case "bigint":
                    return "long?";
                case "numeric":
                case "decimal":
                case "money":
                case "smallmoney":
                    return "decimal?";
                case "float":
                    return "double?";
                case "real":
                    return "Single?";
                case "bit":
                case "bool":
                    return "bool";
                case "binary":
                case "varbinary":
                case "image":
                case "raw":
                case "long":
                case "long raw":
                case "blob":
                case "bfile":
                case "byte[]":
                    return "byte[]";
                case "uniqueidentifier":
                case "Guid":
                    return "Guid?";
                case "sql_variant":
                    return "object";
            }
            return "string";
        }
    }
}
