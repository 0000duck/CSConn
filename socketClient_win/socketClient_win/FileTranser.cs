using System;
using System.Collections.Generic;
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
            fileDialog.InitialDirectory = @"d:\";
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
            FolderBrowserDialog folderD = new FolderBrowserDialog();
            folderD.Description = "选择路径";
            if (folderD.ShowDialog() == DialogResult.OK) {
                return folderD.SelectedPath;
            }
            return "";
        }

    }
}
