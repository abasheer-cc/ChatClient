//Name: Amshar Basheer
//Project Name: chatClient
//File Name: chatClientForm.Designer.cs
//Date: 11/3/2014
//Description: Form designer code

namespace chatClient
{
    partial class chatClientForm
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
            this.recText = new System.Windows.Forms.TextBox();
            this.sendText = new System.Windows.Forms.TextBox();
            this.sendTextLabel = new System.Windows.Forms.Label();
            this.sendBtn = new System.Windows.Forms.Button();
            this.connectBtn = new System.Windows.Forms.Button();
            this.nameBox = new System.Windows.Forms.TextBox();
            this.serverIPBox = new System.Windows.Forms.TextBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.serverIPLabel = new System.Windows.Forms.Label();
            this.connectLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // recText
            // 
            this.recText.Location = new System.Drawing.Point(54, 112);
            this.recText.Multiline = true;
            this.recText.Name = "recText";
            this.recText.ReadOnly = true;
            this.recText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.recText.Size = new System.Drawing.Size(446, 300);
            this.recText.TabIndex = 0;
            // 
            // sendText
            // 
            this.sendText.Location = new System.Drawing.Point(54, 462);
            this.sendText.Name = "sendText";
            this.sendText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.sendText.Size = new System.Drawing.Size(446, 20);
            this.sendText.TabIndex = 1;
            this.sendText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.sendText_KeyDown);
            // 
            // sendTextLabel
            // 
            this.sendTextLabel.AutoSize = true;
            this.sendTextLabel.Location = new System.Drawing.Point(22, 431);
            this.sendTextLabel.Name = "sendTextLabel";
            this.sendTextLabel.Size = new System.Drawing.Size(495, 13);
            this.sendTextLabel.TabIndex = 2;
            this.sendTextLabel.Text = "Type your message in the box below (type \'quit\' to exit) then press the Enter key" +
    " or click the send button";
            // 
            // sendBtn
            // 
            this.sendBtn.Location = new System.Drawing.Point(425, 497);
            this.sendBtn.Name = "sendBtn";
            this.sendBtn.Size = new System.Drawing.Size(75, 23);
            this.sendBtn.TabIndex = 3;
            this.sendBtn.Text = "send";
            this.sendBtn.UseVisualStyleBackColor = true;
            this.sendBtn.Click += new System.EventHandler(this.sendBtn_Click);
            // 
            // connectBtn
            // 
            this.connectBtn.Location = new System.Drawing.Point(319, 54);
            this.connectBtn.Name = "connectBtn";
            this.connectBtn.Size = new System.Drawing.Size(75, 23);
            this.connectBtn.TabIndex = 4;
            this.connectBtn.Text = "connect";
            this.connectBtn.UseVisualStyleBackColor = true;
            this.connectBtn.Click += new System.EventHandler(this.connectBtn_Click);
            // 
            // nameBox
            // 
            this.nameBox.Location = new System.Drawing.Point(133, 34);
            this.nameBox.Name = "nameBox";
            this.nameBox.Size = new System.Drawing.Size(169, 20);
            this.nameBox.TabIndex = 5;
            // 
            // serverIPBox
            // 
            this.serverIPBox.Location = new System.Drawing.Point(133, 73);
            this.serverIPBox.Name = "serverIPBox";
            this.serverIPBox.Size = new System.Drawing.Size(169, 20);
            this.serverIPBox.TabIndex = 6;
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(73, 41);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(38, 13);
            this.nameLabel.TabIndex = 7;
            this.nameLabel.Text = "Name:";
            // 
            // serverIPLabel
            // 
            this.serverIPLabel.AutoSize = true;
            this.serverIPLabel.Location = new System.Drawing.Point(73, 80);
            this.serverIPLabel.Name = "serverIPLabel";
            this.serverIPLabel.Size = new System.Drawing.Size(54, 13);
            this.serverIPLabel.TabIndex = 8;
            this.serverIPLabel.Text = "Server IP:";
            // 
            // connectLabel
            // 
            this.connectLabel.AutoSize = true;
            this.connectLabel.Location = new System.Drawing.Point(114, 9);
            this.connectLabel.Name = "connectLabel";
            this.connectLabel.Size = new System.Drawing.Size(259, 13);
            this.connectLabel.TabIndex = 9;
            this.connectLabel.Text = "Enter your name and the server IP then click connect";
            // 
            // chatClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(539, 535);
            this.Controls.Add(this.connectLabel);
            this.Controls.Add(this.serverIPLabel);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.serverIPBox);
            this.Controls.Add(this.nameBox);
            this.Controls.Add(this.connectBtn);
            this.Controls.Add(this.sendBtn);
            this.Controls.Add(this.sendTextLabel);
            this.Controls.Add(this.sendText);
            this.Controls.Add(this.recText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "chatClientForm";
            this.Text = "Chat Client";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox recText;
        private System.Windows.Forms.TextBox sendText;
        private System.Windows.Forms.Label sendTextLabel;
        private System.Windows.Forms.Button sendBtn;
        private System.Windows.Forms.Button connectBtn;
        private System.Windows.Forms.TextBox nameBox;
        private System.Windows.Forms.TextBox serverIPBox;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label serverIPLabel;
        private System.Windows.Forms.Label connectLabel;
    }
}

