namespace FlappyBird
{
    partial class ScoreBoardForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabControl = new TabControl();
            bottomPanel = new Panel();
            btnClear = new Button();
            btnClose = new Button();
            bottomPanel.SuspendLayout();
            SuspendLayout();
            //
            // tabControl
            //
            tabControl.Dock = DockStyle.Fill;
            tabControl.Location = new Point(0, 0);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(434, 321);
            tabControl.TabIndex = 0;
            //
            // bottomPanel
            //
            bottomPanel.Controls.Add(btnClear);
            bottomPanel.Controls.Add(btnClose);
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Location = new Point(0, 321);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Padding = new Padding(8);
            bottomPanel.Size = new Size(434, 46);
            bottomPanel.TabIndex = 1;
            //
            // btnClear
            //
            btnClear.Dock = DockStyle.Left;
            btnClear.Location = new Point(8, 8);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(130, 30);
            btnClear.TabIndex = 0;
            btnClear.Text = "Tabloyu &Temizle";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += BtnClear_Click;
            //
            // btnClose
            //
            btnClose.DialogResult = DialogResult.OK;
            btnClose.Dock = DockStyle.Right;
            btnClose.Location = new Point(316, 8);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(110, 30);
            btnClose.TabIndex = 1;
            btnClose.Text = "&Kapat";
            btnClose.UseVisualStyleBackColor = true;
            //
            // ScoreBoardForm
            //
            AcceptButton = btnClose;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnClose;
            ClientSize = new Size(434, 367);
            Controls.Add(tabControl);
            Controls.Add(bottomPanel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ScoreBoardForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Skor Tablosu - İlk 10";
            bottomPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl;
        private Panel bottomPanel;
        private Button btnClear;
        private Button btnClose;
    }
}
