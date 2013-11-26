using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace socketClient_win {

    class FileTranser {

        /**
         * 获得文件路径
         */
        public static String getFileName() {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = @"f:\";
            if (fileDialog.ShowDialog() == DialogResult.OK) {
                String fileName = fileDialog.FileName;
                return fileName;
            }
            return "";
        }

        /**
         * 获得文件夹路径
         */
        public static String getFolderPath() {
            Debug.WriteLine("11");
            FolderBrowserDialog folderD = new FolderBrowserDialog();
            folderD.Description = "选择路径";
            Debug.WriteLine("22");
            try {
                if (folderD.ShowDialog() == DialogResult.OK) {
                    return folderD.SelectedPath;
                }
            }
            catch (Exception ex) {
                Debug.WriteLine("show Dialog 异常"+ ex.Message);
            }
            return "";
        }

        /**
         * 获取文件的内容
         */
        public static String getFileContent(String fileName) {
            String content = "";
            StreamReader sr = null;
            using (sr = new StreamReader(fileName)) {
                content = sr.ReadToEnd();
            }

            return content;
        }

    }
}
