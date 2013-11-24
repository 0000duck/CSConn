namespace socketServer_win {
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
            this.tb_history = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.tb_msg = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.tb_port = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checked_lb_client = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // tb_history
            // 
            this.tb_history.Location = new System.Drawing.Point(20, 58);
            this.tb_history.Name = "tb_history";
            this.tb_history.Size = new System.Drawing.Size(445, 246);
            this.tb_history.TabIndex = 0;
            this.tb_history.Text = "";
            this.tb_history.TextChanged += new System.EventHandler(this.tb_history_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "历史记录";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(399, 332);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(64, 26);
            this.button1.TabIndex = 3;
            this.button1.Text = "发送";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tb_msg
            // 
            this.tb_msg.Location = new System.Drawing.Point(20, 378);
            this.tb_msg.Name = "tb_msg";
            this.tb_msg.Size = new System.Drawing.Size(443, 89);
            this.tb_msg.TabIndex = 4;
            this.tb_msg.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(498, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "在线客户端列表";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(390, 22);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(72, 26);
            this.button2.TabIndex = 6;
            this.button2.Text = "启动服务";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tb_port
            // 
            this.tb_port.Location = new System.Drawing.Point(304, 22);
            this.tb_port.Name = "tb_port";
            this.tb_port.ReadOnly = true;
            this.tb_port.Size = new System.Drawing.Size(63, 21);
            this.tb_port.TabIndex = 7;
            this.tb_port.Text = "8889";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(234, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "监听端口";
            // 
            // checked_lb_client
            // 
            this.checked_lb_client.FormattingEnabled = true;
            this.checked_lb_client.Location = new System.Drawing.Point(500, 58);
            this.checked_lb_client.Name = "checked_lb_client";
            this.checked_lb_client.Size = new System.Drawing.Size(169, 308);
            this.checked_lb_client.TabIndex = 9;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(696, 489);
            this.Controls.Add(this.checked_lb_client);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tb_port);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tb_msg);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tb_history);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox tb_history;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RichTextBox tb_msg;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox tb_port;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckedListBox checked_lb_client;
    }
}

