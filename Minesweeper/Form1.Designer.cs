namespace Minesweeper
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.gameFieldPictureBox = new System.Windows.Forms.PictureBox();
            this.restartButton = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.elapsedTimeLabel = new System.Windows.Forms.Label();
            this.bombCountLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gameFieldPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // gameFieldPictureBox
            // 
            this.gameFieldPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gameFieldPictureBox.Location = new System.Drawing.Point(4, 32);
            this.gameFieldPictureBox.Name = "gameFieldPictureBox";
            this.gameFieldPictureBox.Size = new System.Drawing.Size(231, 236);
            this.gameFieldPictureBox.TabIndex = 0;
            this.gameFieldPictureBox.TabStop = false;
            this.gameFieldPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.gameFieldPictureBox_Paint);
            this.gameFieldPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gameFieldPictureBox_MouseDown);
            this.gameFieldPictureBox.MouseLeave += new System.EventHandler(this.gameFieldPictureBox_MouseLeave);
            this.gameFieldPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gameFieldPictureBox_MouseMove);
            this.gameFieldPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gameFieldPictureBox_MouseUp);
            // 
            // restartButton
            // 
            this.restartButton.ForeColor = System.Drawing.Color.DarkRed;
            this.restartButton.Location = new System.Drawing.Point(4, 3);
            this.restartButton.Name = "restartButton";
            this.restartButton.Size = new System.Drawing.Size(75, 23);
            this.restartButton.TabIndex = 1;
            this.restartButton.Text = "Restart";
            this.restartButton.UseVisualStyleBackColor = true;
            this.restartButton.Click += new System.EventHandler(this.RestartButton_Click);
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // elapsedTimeLabel
            // 
            this.elapsedTimeLabel.AutoSize = true;
            this.elapsedTimeLabel.Location = new System.Drawing.Point(96, 8);
            this.elapsedTimeLabel.Name = "elapsedTimeLabel";
            this.elapsedTimeLabel.Size = new System.Drawing.Size(35, 13);
            this.elapsedTimeLabel.TabIndex = 2;
            this.elapsedTimeLabel.Text = "label1";
            // 
            // bombCountLabel
            // 
            this.bombCountLabel.AutoSize = true;
            this.bombCountLabel.Location = new System.Drawing.Point(163, 8);
            this.bombCountLabel.Name = "bombCountLabel";
            this.bombCountLabel.Size = new System.Drawing.Size(35, 13);
            this.bombCountLabel.TabIndex = 3;
            this.bombCountLabel.Text = "label2";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(236, 274);
            this.Controls.Add(this.bombCountLabel);
            this.Controls.Add(this.elapsedTimeLabel);
            this.Controls.Add(this.restartButton);
            this.Controls.Add(this.gameFieldPictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "MinesWeeper";
            ((System.ComponentModel.ISupportInitialize)(this.gameFieldPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox gameFieldPictureBox;
        private System.Windows.Forms.Button restartButton;
        private System.Windows.Forms.Label elapsedTimeLabel;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label bombCountLabel;
    }
}

