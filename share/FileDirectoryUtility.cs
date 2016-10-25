using System.IO;
using System.Xml;

namespace LYH.WorkOrder.share
{
    /// <summary>
    /// FileDirectoryUtility ��,�����������쳣����
    /// </summary>
    public class FileDirectoryUtility
    {
        /// <summary>
        /// ·���ָ��
        /// </summary>
        private const string PathSplitChar = "\\";

        /// <summary>
        /// ���캯��
        /// </summary>
        private FileDirectoryUtility()
        {
        }

        /// <summary>
        /// ����ָ��Ŀ¼�������ļ�,��������Ŀ¼����Ŀ¼�е��ļ�
        /// </summary>
        /// <param name="sourceDir">ԭʼĿ¼</param>
        /// <param name="targetDir">Ŀ��Ŀ¼</param>
        /// <param name="overWrite">���Ϊtrue,��ʾ����ͬ���ļ�,���򲻸���</param>
        public static void CopyFiles(string sourceDir, string targetDir, bool overWrite)
        {
            CopyFiles(sourceDir, targetDir, overWrite, false);
        }

        /// <summary>
        /// ����ָ��Ŀ¼�������ļ�
        /// </summary>
        /// <param name="sourceDir">ԭʼĿ¼</param>
        /// <param name="targetDir">Ŀ��Ŀ¼</param>
        /// <param name="overWrite">���Ϊtrue,����ͬ���ļ�,���򲻸���</param>
        /// <param name="copySubDir">���Ϊtrue,����Ŀ¼,���򲻰���</param>
        public static void CopyFiles(string sourceDir, string targetDir, bool overWrite, bool copySubDir)
        {
            //���Ƶ�ǰĿ¼�ļ�
            foreach (var sourceFileName in Directory.GetFiles(sourceDir))
            {
                var targetFileName = Path.Combine(targetDir, sourceFileName.Substring(sourceFileName.LastIndexOf(PathSplitChar) + 1));

                if (File.Exists(targetFileName))
                {
                    if (overWrite == true)
                    {
                        File.SetAttributes(targetFileName, FileAttributes.Normal);
                        File.Copy(sourceFileName, targetFileName, overWrite);
                    }
                }
                else
                {
                    File.Copy(sourceFileName, targetFileName, overWrite);
                }
            }
            //������Ŀ¼
            if (copySubDir)
            {
                foreach (var sourceSubDir in Directory.GetDirectories(sourceDir))
                {
                    var targetSubDir = Path.Combine(targetDir, sourceSubDir.Substring(sourceSubDir.LastIndexOf(PathSplitChar) + 1));
                    if (!Directory.Exists(targetSubDir))
                        Directory.CreateDirectory(targetSubDir);
                    CopyFiles(sourceSubDir, targetSubDir, overWrite, true);
                }
            }
        }

        /// <summary>
        /// ����ָ��Ŀ¼�������ļ�,��������Ŀ¼
        /// </summary>
        /// <param name="sourceDir">ԭʼĿ¼</param>
        /// <param name="targetDir">Ŀ��Ŀ¼</param>
        /// <param name="overWrite">���Ϊtrue,����ͬ���ļ�,���򲻸���</param>
        public static void MoveFiles(string sourceDir, string targetDir, bool overWrite)
        {
            MoveFiles(sourceDir, targetDir, overWrite, false);
        }

        /// <summary>
        /// ����ָ��Ŀ¼�������ļ�
        /// </summary>
        /// <param name="sourceDir">ԭʼĿ¼</param>
        /// <param name="targetDir">Ŀ��Ŀ¼</param>
        /// <param name="overWrite">���Ϊtrue,����ͬ���ļ�,���򲻸���</param>
        /// <param name="moveSubDir">���Ϊtrue,����Ŀ¼,���򲻰���</param>
        public static void MoveFiles(string sourceDir, string targetDir, bool overWrite, bool moveSubDir)
        {
            //�ƶ���ǰĿ¼�ļ�
            foreach (var sourceFileName in Directory.GetFiles(sourceDir))
            {
                var targetFileName = Path.Combine(targetDir, sourceFileName.Substring(sourceFileName.LastIndexOf(PathSplitChar) + 1));
                if (File.Exists(targetFileName))
                {
                    if (overWrite == true)
                    {
                        File.SetAttributes(targetFileName, FileAttributes.Normal);
                        File.Delete(targetFileName);
                        File.Move(sourceFileName, targetFileName);
                    }
                }
                else
                {
                    File.Move(sourceFileName, targetFileName);
                }
            }
            if (moveSubDir)
            {
                foreach (var sourceSubDir in Directory.GetDirectories(sourceDir))
                {
                    var targetSubDir = Path.Combine(targetDir, sourceSubDir.Substring(sourceSubDir.LastIndexOf(PathSplitChar) + 1));
                    if (!Directory.Exists(targetSubDir))
                        Directory.CreateDirectory(targetSubDir);
                    MoveFiles(sourceSubDir, targetSubDir, overWrite, true);
                    Directory.Delete(sourceSubDir);
                }
            }
        }

        /// <summary>
        /// ɾ��ָ��Ŀ¼�������ļ�����������Ŀ¼
        /// </summary>
        /// <param name="targetDir">����Ŀ¼</param>
        public static void DeleteFiles(string targetDir)
        {
            DeleteFiles(targetDir, false);
        }

        /// <summary>
        /// ɾ��ָ��Ŀ¼�������ļ�����Ŀ¼
        /// </summary>
        /// <param name="targetDir">����Ŀ¼</param>
        /// <param name="delSubDir">���Ϊtrue,��������Ŀ¼�Ĳ���</param>
        public static void DeleteFiles(string targetDir, bool delSubDir)
        {
            foreach (var fileName in Directory.GetFiles(targetDir))
            {
                File.SetAttributes(fileName, FileAttributes.Normal);
                File.Delete(fileName);
            }
            if (delSubDir)
            {
                var dir = new DirectoryInfo(targetDir);
                foreach (var subDi in dir.GetDirectories())
                {
                    DeleteFiles(subDi.FullName, true);
                    subDi.Delete();
                }
            }
        }

        /// <summary>
        /// ����ָ��Ŀ¼
        /// </summary>
        /// <param name="targetDir"></param>
        public static void CreateDirectory(string targetDir)
        {
            var dir = new DirectoryInfo(targetDir);
            if (!dir.Exists)
                dir.Create();
        }

        /// <summary>
        /// ������Ŀ¼
        /// </summary>
        /// <param name="targetDir">Ŀ¼·��</param>
        /// <param name="subDirName">��Ŀ¼����</param>
        public static void CreateDirectory(string parentDir, string subDirName)
        {
            CreateDirectory(parentDir + PathSplitChar + subDirName);
        }

        /// <summary>
        /// ɾ��ָ��Ŀ¼
        /// </summary>
        /// <param name="targetDir">Ŀ¼·��</param>
        public static void DeleteDirectory(string targetDir)
        {
            var dirInfo = new DirectoryInfo(targetDir);
            if (dirInfo.Exists)
            {
                DeleteFiles(targetDir, true);
                dirInfo.Delete(true);
            }
        }

        /// <summary>
        /// ɾ��ָ��Ŀ¼��������Ŀ¼,�������Ե�ǰĿ¼�ļ���ɾ��
        /// </summary>
        /// <param name="targetDir">Ŀ¼·��</param>
        public static void DeleteSubDirectory(string targetDir)
        {
            foreach (var subDir in Directory.GetDirectories(targetDir))
            {
                DeleteDirectory(subDir);
            }
        }

        /// <summary>
        /// ��ָ��Ŀ¼�µ���Ŀ¼���ļ�����xml�ĵ�
        /// </summary>
        /// <param name="targetDir">��Ŀ¼</param>
        /// <returns>����XmlDocument����</returns>
        public static XmlDocument CreateXml(string targetDir)
        {
            var myDocument = new XmlDocument();
            var declaration = myDocument.CreateXmlDeclaration("1.0", "utf-8", null);
            myDocument.AppendChild(declaration);
            var rootElement = myDocument.CreateElement(targetDir.Substring(targetDir.LastIndexOf(PathSplitChar) + 1));
            myDocument.AppendChild(rootElement);
            foreach (var fileName in Directory.GetFiles(targetDir))
            {
                var childElement = myDocument.CreateElement("File");
                childElement.InnerText = fileName.Substring(fileName.LastIndexOf(PathSplitChar) + 1);
                rootElement.AppendChild(childElement);
            }
            foreach (var directory in Directory.GetDirectories(targetDir))
            {
                var childElement = myDocument.CreateElement("Directory");
                childElement.SetAttribute("Name", directory.Substring(directory.LastIndexOf(PathSplitChar) + 1));
                rootElement.AppendChild(childElement);
                CreateBranch(directory, childElement, myDocument);
            }
            return myDocument;
        }

        /// <summary>
        /// ����Xml��֧
        /// </summary>
        /// <param name="targetDir">��Ŀ¼</param>
        /// <param name="xmlNode">��Ŀ¼XmlDocument</param>
        /// <param name="myDocument">XmlDocument����</param>
        private static void CreateBranch(string targetDir, XmlElement xmlNode, XmlDocument myDocument)
        {
            foreach (var fileName in Directory.GetFiles(targetDir))
            {
                var childElement = myDocument.CreateElement("File");
                childElement.InnerText = fileName.Substring(fileName.LastIndexOf(PathSplitChar) + 1);
                xmlNode.AppendChild(childElement);
            }
            foreach (var directory in Directory.GetDirectories(targetDir))
            {
                var childElement = myDocument.CreateElement("Directory");
                childElement.SetAttribute("Name", directory.Substring(directory.LastIndexOf(PathSplitChar) + 1));
                xmlNode.AppendChild(childElement);
                CreateBranch(directory, childElement, myDocument);
            }
        }
    }
}