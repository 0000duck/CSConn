﻿namespace socketClient_win {
    partial class Form1 {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            this.tb_server_id = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_conn = new System.Windows.Forms.Button();
            this.tb_history = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_send = new System.Windows.Forms.Button();
            this.tb_msg = new System.Windows.Forms.RichTextBox();
            this.tb_port = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tb_server_id
            // 
            this.tb_server_id.Location = new System.Drawing.Point(63, 23);
            this.tb_server_id.Name = "tb_server_id";
            this.tb_server_id.ReadOnly = true;
            this.tb_server_id.Size = new System.Drawing.Size(154, 21);
            this.tb_server_id.TabIndex = 0;
            this.tb_server_id.Text = "127.0.0.1";
            this.tb_server_id.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "服务器地址";
            // 
            // btn_conn
            // 
            this.btn_conn.Location = new System.Drawing.Point(310, 17);
            this.btn_conn.Name = "btn_conn";
            this.btn_conn.Size = new System.Drawing.Size(64, 30);
            this.btn_conn.TabIndex = 2;
            this.btn_conn.Text = "连接";
            this.btn_conn.UseVisualStyleBackColor = true;
            this.btn_conn.Click += new System.EventHandler(this.btn_conn_Click);
            // 
            // tb_history
            // 
            this.tb_history.Location = new System.Drawing.Point(21, 80);
            this.tb_history.Name = "tb_history";
            this.tb_history.Size = new System.Drawing.Size(355, 202);
            this.tb_history.TabIndex = 3;
            this.tb_history.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "历史记录";
            // 
            // btn_send
            // 
            this.btn_send.Location = new System.Drawing.Point(300, 288);
            this.btn_send.Name = "btn_send";
            this.btn_send.Size = new System.Drawing.Size(75, 23);
            this.btn_send.TabIndex = 5;
            this.btn_send.Text = "发送";
            this.btn_send.UseVisualStyleBackColor = true;
            this.btn_send.Click += new System.EventHandler(this.btn_send_Click);
            // 
            // tb_msg
            // 
            this.tb_msg.Location = new System.Drawing.Point(21, 318);
            this.tb_msg.Name = "tb_msg";
            this.tb_msg.Size = new System.Drawing.Size(353, 73);
            this.tb_msg.TabIndex = 6;
            this.tb_msg.Text = "";
            // 
            // tb_port
            // 
            this.tb_port.Location = new System.Drawing.Point(239, 23);
            this.tb_port.Name = "tb_port";
            this.tb_port.ReadOnly = true;
            this.tb_port.Size = new System.Drawing.Size(61, 21);
            this.tb_port.TabIndex = 7;
            this.tb_port.Text = "8889";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(220, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "：";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(580, 402);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tb_port);
            this.Controls.Add(this.tb_msg);
            this.Controls.Add(this.btn_send);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tb_history);
            this.Controls.Add(this.btn_conn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tb_server_id);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_server_id;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_conn;
        private System.Windows.Forms.RichTextBox tb_history;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_send;
        private System.Windows.Forms.RichTextBox tb_msg;
        private System.Windows.Forms.TextBox tb_port;
        private System.Windows.Forms.Label label3;
    }
}
