namespace TwitchBotWindowsApp
{
    partial class TwitchBot
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
            this.chatBox = new System.Windows.Forms.TextBox();
            this.chatLabel = new System.Windows.Forms.Label();
            this.timer_clear = new System.Windows.Forms.Timer(this.components);
            this.updateButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.viewersBox = new System.Windows.Forms.ListBox();
            this.timer_addpoints = new System.Windows.Forms.Timer(this.components);
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.CommandSpamTimer = new System.Windows.Forms.Timer(this.components);
            this.CommandSpamTimer_gift = new System.Windows.Forms.Timer(this.components);
            this.backgroundWorker3 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker_irc_response = new System.ComponentModel.BackgroundWorker();
            this.timer_PingResponse = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // chatBox
            // 
            this.chatBox.Location = new System.Drawing.Point(18, 65);
            this.chatBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chatBox.Multiline = true;
            this.chatBox.Name = "chatBox";
            this.chatBox.Size = new System.Drawing.Size(702, 612);
            this.chatBox.TabIndex = 0;
            // 
            // chatLabel
            // 
            this.chatLabel.AutoSize = true;
            this.chatLabel.Font = new System.Drawing.Font("Trebuchet MS", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chatLabel.Location = new System.Drawing.Point(292, 14);
            this.chatLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.chatLabel.Name = "chatLabel";
            this.chatLabel.Size = new System.Drawing.Size(95, 44);
            this.chatLabel.TabIndex = 1;
            this.chatLabel.Text = "Chat";
            // 
            // timer_clear
            // 
            this.timer_clear.Enabled = true;
            this.timer_clear.Interval = 60000;
            this.timer_clear.Tick += new System.EventHandler(this.timer_clear_Tick);
            // 
            // updateButton
            // 
            this.updateButton.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.updateButton.Location = new System.Drawing.Point(816, 124);
            this.updateButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(226, 97);
            this.updateButton.TabIndex = 2;
            this.updateButton.Text = "Clear and ViewersUPD";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Palatino Linotype", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(775, 14);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(320, 24);
            this.label1.TabIndex = 3;
            this.label1.Text = "The bot chat is cleared every 60 seconds";
            // 
            // viewersBox
            // 
            this.viewersBox.FormattingEnabled = true;
            this.viewersBox.ItemHeight = 20;
            this.viewersBox.Location = new System.Drawing.Point(816, 322);
            this.viewersBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.viewersBox.Name = "viewersBox";
            this.viewersBox.Size = new System.Drawing.Size(224, 324);
            this.viewersBox.TabIndex = 5;
            // 
            // timer_addpoints
            // 
            this.timer_addpoints.Enabled = true;
            this.timer_addpoints.Interval = 480000;
            this.timer_addpoints.Tick += new System.EventHandler(this.timer_addpoints_Tick);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // CommandSpamTimer
            // 
            this.CommandSpamTimer.Enabled = true;
            this.CommandSpamTimer.Interval = 5000;
            this.CommandSpamTimer.Tick += new System.EventHandler(this.CommandSpamTimer_Tick);
            // 
            // CommandSpamTimer_gift
            // 
            this.CommandSpamTimer_gift.Enabled = true;
            this.CommandSpamTimer_gift.Interval = 6000;
            this.CommandSpamTimer_gift.Tick += new System.EventHandler(this.CommandSpamTimer_gift_Tick);
            // 
            // backgroundWorker3
            // 
            this.backgroundWorker3.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker3_DoWork);
            // 
            // backgroundWorker_irc_response
            // 
            this.backgroundWorker_irc_response.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_irc_response_DoWork);
            // 
            // timer_PingResponse
            // 
            this.timer_PingResponse.Enabled = true;
            this.timer_PingResponse.Interval = 60000;
            this.timer_PingResponse.Tick += new System.EventHandler(this.timer_PingResponse_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1108, 697);
            this.Controls.Add(this.viewersBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.updateButton);
            this.Controls.Add(this.chatLabel);
            this.Controls.Add(this.chatBox);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.Text = "Twitch Bot";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox chatBox;
        private System.Windows.Forms.Label chatLabel;
        private System.Windows.Forms.Timer timer_clear;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox viewersBox;
        private System.Windows.Forms.Timer timer_addpoints;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private System.Windows.Forms.Timer CommandSpamTimer;
        private System.Windows.Forms.Timer CommandSpamTimer_gift;
        private System.ComponentModel.BackgroundWorker backgroundWorker3;
        private System.ComponentModel.BackgroundWorker backgroundWorker_irc_response;
        private System.Windows.Forms.Timer timer_PingResponse;
    }
}

