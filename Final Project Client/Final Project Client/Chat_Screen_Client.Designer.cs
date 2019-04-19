namespace Final_Proj_Network_Client
{
    partial class Chat_Screen_Client
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Chat_Screen_Client));
            this.Chat_SelectionPan = new System.Windows.Forms.Panel();
            this.Chat_SelectionBtn = new System.Windows.Forms.Button();
            this.Chat_SelectionCkLsBx = new System.Windows.Forms.CheckedListBox();
            this.ConvoTxtBx = new System.Windows.Forms.TextBox();
            this.ConvoBtn = new System.Windows.Forms.Button();
            this.ConvoPan = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.ConvoLiBx = new System.Windows.Forms.ListBox();
            this.ConvoAttach = new System.Windows.Forms.Button();
            this.Chat_SelectionPan.SuspendLayout();
            this.ConvoPan.SuspendLayout();
            this.SuspendLayout();
            // 
            // Chat_SelectionPan
            // 
            this.Chat_SelectionPan.Controls.Add(this.Chat_SelectionBtn);
            this.Chat_SelectionPan.Controls.Add(this.Chat_SelectionCkLsBx);
            this.Chat_SelectionPan.Location = new System.Drawing.Point(596, 12);
            this.Chat_SelectionPan.Name = "Chat_SelectionPan";
            this.Chat_SelectionPan.Size = new System.Drawing.Size(180, 446);
            this.Chat_SelectionPan.TabIndex = 0;
            // 
            // Chat_SelectionBtn
            // 
            this.Chat_SelectionBtn.AutoSize = true;
            this.Chat_SelectionBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Chat_SelectionBtn.Location = new System.Drawing.Point(3, 414);
            this.Chat_SelectionBtn.Name = "Chat_SelectionBtn";
            this.Chat_SelectionBtn.Size = new System.Drawing.Size(174, 30);
            this.Chat_SelectionBtn.TabIndex = 5;
            this.Chat_SelectionBtn.Text = "Select";
            this.Chat_SelectionBtn.UseVisualStyleBackColor = true;
            // 
            // Chat_SelectionCkLsBx
            // 
            this.Chat_SelectionCkLsBx.FormattingEnabled = true;
            this.Chat_SelectionCkLsBx.Location = new System.Drawing.Point(3, 3);
            this.Chat_SelectionCkLsBx.Name = "Chat_SelectionCkLsBx";
            this.Chat_SelectionCkLsBx.Size = new System.Drawing.Size(174, 409);
            this.Chat_SelectionCkLsBx.TabIndex = 1;
            // 
            // ConvoTxtBx
            // 
            this.ConvoTxtBx.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConvoTxtBx.Location = new System.Drawing.Point(3, 412);
            this.ConvoTxtBx.Name = "ConvoTxtBx";
            this.ConvoTxtBx.Size = new System.Drawing.Size(407, 29);
            this.ConvoTxtBx.TabIndex = 1;
            // 
            // ConvoBtn
            // 
            this.ConvoBtn.AutoSize = true;
            this.ConvoBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConvoBtn.Location = new System.Drawing.Point(449, 412);
            this.ConvoBtn.Name = "ConvoBtn";
            this.ConvoBtn.Size = new System.Drawing.Size(75, 30);
            this.ConvoBtn.TabIndex = 2;
            this.ConvoBtn.Text = "Enter";
            this.ConvoBtn.UseVisualStyleBackColor = true;
            // 
            // ConvoPan
            // 
            this.ConvoPan.Controls.Add(this.ConvoAttach);
            this.ConvoPan.Controls.Add(this.button1);
            this.ConvoPan.Controls.Add(this.ConvoLiBx);
            this.ConvoPan.Controls.Add(this.ConvoBtn);
            this.ConvoPan.Controls.Add(this.ConvoTxtBx);
            this.ConvoPan.Location = new System.Drawing.Point(12, 12);
            this.ConvoPan.Name = "ConvoPan";
            this.ConvoPan.Size = new System.Drawing.Size(578, 446);
            this.ConvoPan.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.AutoSize = true;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(525, 412);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(50, 30);
            this.button1.TabIndex = 5;
            this.button1.Text = "Exit";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // ConvoLiBx
            // 
            this.ConvoLiBx.FormattingEnabled = true;
            this.ConvoLiBx.Location = new System.Drawing.Point(3, 3);
            this.ConvoLiBx.Name = "ConvoLiBx";
            this.ConvoLiBx.Size = new System.Drawing.Size(572, 407);
            this.ConvoLiBx.TabIndex = 4;
            // 
            // ConvoAttach
            // 
            this.ConvoAttach.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ConvoAttach.BackgroundImage")));
            this.ConvoAttach.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ConvoAttach.Location = new System.Drawing.Point(416, 412);
            this.ConvoAttach.Name = "ConvoAttach";
            this.ConvoAttach.Size = new System.Drawing.Size(38, 30);
            this.ConvoAttach.TabIndex = 6;
            this.ConvoAttach.UseVisualStyleBackColor = true;
            // 
            // Chat_Screen_Client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 466);
            this.Controls.Add(this.ConvoPan);
            this.Controls.Add(this.Chat_SelectionPan);
            this.Name = "Chat_Screen_Client";
            this.Text = "Chat_Screen_Client";
            this.Load += new System.EventHandler(this.Chat_Screen_Client_Load);
            this.Chat_SelectionPan.ResumeLayout(false);
            this.Chat_SelectionPan.PerformLayout();
            this.ConvoPan.ResumeLayout(false);
            this.ConvoPan.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Chat_SelectionPan;
        private System.Windows.Forms.Button Chat_SelectionBtn;
        private System.Windows.Forms.CheckedListBox Chat_SelectionCkLsBx;
        private System.Windows.Forms.TextBox ConvoTxtBx;
        private System.Windows.Forms.Button ConvoBtn;
        private System.Windows.Forms.Panel ConvoPan;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox ConvoLiBx;
        private System.Windows.Forms.Button ConvoAttach;
    }
}