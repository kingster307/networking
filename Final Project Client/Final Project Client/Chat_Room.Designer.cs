namespace Final_Project_Client
{
    partial class Chat_Room
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Chat_Room));
            this.Chat_Box_UserinputTxt = new System.Windows.Forms.TextBox();
            this.Chat_Box_ExitBtn = new System.Windows.Forms.Button();
            this.Chat_Box_SendBtn = new System.Windows.Forms.Button();
            this.Chat_Box_MessagingP = new System.Windows.Forms.Panel();
            this.pbConvHeader = new System.Windows.Forms.PictureBox();
            this.lblConvHeader = new System.Windows.Forms.Label();
            this.Chat_Box_AttachBtn = new System.Windows.Forms.Button();
            this.Chat_Box_Tab_Lay_Pan = new System.Windows.Forms.TableLayoutPanel();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.pbUserIMG = new System.Windows.Forms.PictureBox();
            this.lblDisplayName = new System.Windows.Forms.Label();
            this.tlpClients = new System.Windows.Forms.TableLayoutPanel();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.Chat_Box_MessagingP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbConvHeader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbUserIMG)).BeginInit();
            this.SuspendLayout();
            // 
            // Chat_Box_UserinputTxt
            // 
            this.Chat_Box_UserinputTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Chat_Box_UserinputTxt.Location = new System.Drawing.Point(4, 775);
            this.Chat_Box_UserinputTxt.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Chat_Box_UserinputTxt.Name = "Chat_Box_UserinputTxt";
            this.Chat_Box_UserinputTxt.Size = new System.Drawing.Size(607, 30);
            this.Chat_Box_UserinputTxt.TabIndex = 1;
            this.Chat_Box_UserinputTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Chat_Box_UserinputTxt_KeyDown);
            // 
            // Chat_Box_ExitBtn
            // 
            this.Chat_Box_ExitBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Chat_Box_ExitBtn.Location = new System.Drawing.Point(760, 775);
            this.Chat_Box_ExitBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Chat_Box_ExitBtn.Name = "Chat_Box_ExitBtn";
            this.Chat_Box_ExitBtn.Size = new System.Drawing.Size(57, 32);
            this.Chat_Box_ExitBtn.TabIndex = 2;
            this.Chat_Box_ExitBtn.Text = "Exit";
            this.Chat_Box_ExitBtn.UseVisualStyleBackColor = true;
            this.Chat_Box_ExitBtn.Click += new System.EventHandler(this.Chat_Box_ExitBtn_Click);
            // 
            // Chat_Box_SendBtn
            // 
            this.Chat_Box_SendBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Chat_Box_SendBtn.Location = new System.Drawing.Point(668, 775);
            this.Chat_Box_SendBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Chat_Box_SendBtn.Name = "Chat_Box_SendBtn";
            this.Chat_Box_SendBtn.Size = new System.Drawing.Size(93, 32);
            this.Chat_Box_SendBtn.TabIndex = 4;
            this.Chat_Box_SendBtn.Text = "Send";
            this.Chat_Box_SendBtn.UseVisualStyleBackColor = true;
            this.Chat_Box_SendBtn.Click += new System.EventHandler(this.button2_Click);
            // 
            // Chat_Box_MessagingP
            // 
            this.Chat_Box_MessagingP.Controls.Add(this.pbConvHeader);
            this.Chat_Box_MessagingP.Controls.Add(this.lblConvHeader);
            this.Chat_Box_MessagingP.Controls.Add(this.Chat_Box_AttachBtn);
            this.Chat_Box_MessagingP.Controls.Add(this.Chat_Box_SendBtn);
            this.Chat_Box_MessagingP.Controls.Add(this.Chat_Box_Tab_Lay_Pan);
            this.Chat_Box_MessagingP.Controls.Add(this.Chat_Box_ExitBtn);
            this.Chat_Box_MessagingP.Controls.Add(this.Chat_Box_UserinputTxt);
            this.Chat_Box_MessagingP.Location = new System.Drawing.Point(16, 15);
            this.Chat_Box_MessagingP.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Chat_Box_MessagingP.Name = "Chat_Box_MessagingP";
            this.Chat_Box_MessagingP.Size = new System.Drawing.Size(821, 814);
            this.Chat_Box_MessagingP.TabIndex = 0;
            // 
            // pbConvHeader
            // 
            this.pbConvHeader.Location = new System.Drawing.Point(4, 4);
            this.pbConvHeader.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pbConvHeader.Name = "pbConvHeader";
            this.pbConvHeader.Size = new System.Drawing.Size(85, 79);
            this.pbConvHeader.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbConvHeader.TabIndex = 7;
            this.pbConvHeader.TabStop = false;
            // 
            // lblConvHeader
            // 
            this.lblConvHeader.AutoSize = true;
            this.lblConvHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConvHeader.Location = new System.Drawing.Point(100, 25);
            this.lblConvHeader.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblConvHeader.Name = "lblConvHeader";
            this.lblConvHeader.Size = new System.Drawing.Size(132, 46);
            this.lblConvHeader.TabIndex = 6;
            this.lblConvHeader.Text = "label2";
            // 
            // Chat_Box_AttachBtn
            // 
            this.Chat_Box_AttachBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Chat_Box_AttachBtn.BackgroundImage")));
            this.Chat_Box_AttachBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Chat_Box_AttachBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Chat_Box_AttachBtn.Location = new System.Drawing.Point(616, 775);
            this.Chat_Box_AttachBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Chat_Box_AttachBtn.Name = "Chat_Box_AttachBtn";
            this.Chat_Box_AttachBtn.Size = new System.Drawing.Size(53, 32);
            this.Chat_Box_AttachBtn.TabIndex = 5;
            this.Chat_Box_AttachBtn.UseVisualStyleBackColor = true;
            this.Chat_Box_AttachBtn.Click += new System.EventHandler(this.button3_Click);
            // 
            // Chat_Box_Tab_Lay_Pan
            // 
            this.Chat_Box_Tab_Lay_Pan.AutoScroll = true;
            this.Chat_Box_Tab_Lay_Pan.ColumnCount = 2;
            this.Chat_Box_Tab_Lay_Pan.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.Chat_Box_Tab_Lay_Pan.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.Chat_Box_Tab_Lay_Pan.Location = new System.Drawing.Point(7, 92);
            this.Chat_Box_Tab_Lay_Pan.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Chat_Box_Tab_Lay_Pan.Name = "Chat_Box_Tab_Lay_Pan";
            this.Chat_Box_Tab_Lay_Pan.RowCount = 2;
            this.Chat_Box_Tab_Lay_Pan.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.Chat_Box_Tab_Lay_Pan.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.Chat_Box_Tab_Lay_Pan.Size = new System.Drawing.Size(811, 676);
            this.Chat_Box_Tab_Lay_Pan.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(915, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label1.Size = new System.Drawing.Size(99, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Signed on as: ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // pbUserIMG
            // 
            this.pbUserIMG.Location = new System.Drawing.Point(1023, 11);
            this.pbUserIMG.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pbUserIMG.Name = "pbUserIMG";
            this.pbUserIMG.Size = new System.Drawing.Size(85, 79);
            this.pbUserIMG.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbUserIMG.TabIndex = 2;
            this.pbUserIMG.TabStop = false;
            // 
            // lblDisplayName
            // 
            this.lblDisplayName.AutoSize = true;
            this.lblDisplayName.BackColor = System.Drawing.Color.Transparent;
            this.lblDisplayName.Location = new System.Drawing.Point(968, 39);
            this.lblDisplayName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDisplayName.Name = "lblDisplayName";
            this.lblDisplayName.Size = new System.Drawing.Size(46, 17);
            this.lblDisplayName.TabIndex = 3;
            this.lblDisplayName.Text = "label2";
            // 
            // tlpClients
            // 
            this.tlpClients.AutoScroll = true;
            this.tlpClients.ColumnCount = 2;
            this.tlpClients.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.tlpClients.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpClients.Location = new System.Drawing.Point(841, 97);
            this.tlpClients.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tlpClients.Name = "tlpClients";
            this.tlpClients.RowCount = 2;
            this.tlpClients.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 79F));
            this.tlpClients.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpClients.Size = new System.Drawing.Size(361, 731);
            this.tlpClients.TabIndex = 5;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Chat_Room
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1219, 837);
            this.Controls.Add(this.tlpClients);
            this.Controls.Add(this.lblDisplayName);
            this.Controls.Add(this.pbUserIMG);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Chat_Box_MessagingP);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Chat_Room";
            this.Text = "Chat_Room";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Chat_Room_FormClosing);
            this.Load += new System.EventHandler(this.Chat_Room_Load);
            this.Chat_Box_MessagingP.ResumeLayout(false);
            this.Chat_Box_MessagingP.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbConvHeader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbUserIMG)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Chat_Box_UserinputTxt;
        private System.Windows.Forms.Button Chat_Box_ExitBtn;
        private System.Windows.Forms.Button Chat_Box_SendBtn;
        private System.Windows.Forms.Panel Chat_Box_MessagingP;
        private System.Windows.Forms.Button Chat_Box_AttachBtn;
        private System.Windows.Forms.TableLayoutPanel Chat_Box_Tab_Lay_Pan;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pbUserIMG;
        private System.Windows.Forms.Label lblDisplayName;
        private System.Windows.Forms.TableLayoutPanel tlpClients;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label lblConvHeader;
        private System.Windows.Forms.PictureBox pbConvHeader;
        private System.Windows.Forms.Timer timer1;
    }
}