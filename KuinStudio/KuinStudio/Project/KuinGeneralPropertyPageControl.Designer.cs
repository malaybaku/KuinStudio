namespace Baku.KuinStudio.Project
{ 
    partial class KuinGeneralPropertyPageControl
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.radioButtonUseWebMode = new System.Windows.Forms.RadioButton();
            this.radioButtonUseCuiMode = new System.Windows.Forms.RadioButton();
            this.radioButtonUseWndMode = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxStartupFileName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxOutputFileName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxIconFileName = new System.Windows.Forms.TextBox();
            this.checkBoxUseReleaseBuild = new System.Windows.Forms.CheckBox();
            this.textBoxCustomSystemDirectory = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxWorkingDirectory = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 153F));
            this.tableLayoutPanel1.Controls.Add(this.radioButtonUseWebMode, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonUseCuiMode, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonUseWndMode, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.textBoxStartupFileName, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxOutputFileName, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.textBoxIconFileName, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxUseReleaseBuild, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.textBoxCustomSystemDirectory, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.textBoxWorkingDirectory, 1, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(10, 10);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(10);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(495, 193);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // radioButtonUseWebMode
            // 
            this.radioButtonUseWebMode.AutoSize = true;
            this.radioButtonUseWebMode.Enabled = false;
            this.radioButtonUseWebMode.Location = new System.Drawing.Point(345, 103);
            this.radioButtonUseWebMode.Name = "radioButtonUseWebMode";
            this.radioButtonUseWebMode.Size = new System.Drawing.Size(44, 16);
            this.radioButtonUseWebMode.TabIndex = 2;
            this.radioButtonUseWebMode.TabStop = true;
            this.radioButtonUseWebMode.Text = "Web";
            this.radioButtonUseWebMode.UseVisualStyleBackColor = true;
            this.radioButtonUseWebMode.CheckedChanged += new System.EventHandler(this.OnChanged);
            // 
            // radioButtonUseCuiMode
            // 
            this.radioButtonUseCuiMode.AutoSize = true;
            this.radioButtonUseCuiMode.Location = new System.Drawing.Point(231, 103);
            this.radioButtonUseCuiMode.Name = "radioButtonUseCuiMode";
            this.radioButtonUseCuiMode.Size = new System.Drawing.Size(40, 16);
            this.radioButtonUseCuiMode.TabIndex = 1;
            this.radioButtonUseCuiMode.TabStop = true;
            this.radioButtonUseCuiMode.Text = "Cui";
            this.radioButtonUseCuiMode.UseVisualStyleBackColor = true;
            this.radioButtonUseCuiMode.CheckedChanged += new System.EventHandler(this.OnChanged);
            // 
            // radioButtonUseWndMode
            // 
            this.radioButtonUseWndMode.AutoSize = true;
            this.radioButtonUseWndMode.Location = new System.Drawing.Point(117, 103);
            this.radioButtonUseWndMode.Name = "radioButtonUseWndMode";
            this.radioButtonUseWndMode.Size = new System.Drawing.Size(44, 16);
            this.radioButtonUseWndMode.TabIndex = 0;
            this.radioButtonUseWndMode.TabStop = true;
            this.radioButtonUseWndMode.Text = "Wnd";
            this.radioButtonUseWndMode.UseVisualStyleBackColor = true;
            this.radioButtonUseWndMode.CheckedChanged += new System.EventHandler(this.OnChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 105);
            this.label1.Margin = new System.Windows.Forms.Padding(5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "Application Type";
            // 
            // textBoxStartupFileName
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textBoxStartupFileName, 3);
            this.textBoxStartupFileName.Location = new System.Drawing.Point(117, 3);
            this.textBoxStartupFileName.Name = "textBoxStartupFileName";
            this.textBoxStartupFileName.Size = new System.Drawing.Size(375, 19);
            this.textBoxStartupFileName.TabIndex = 4;
            this.textBoxStartupFileName.TextChanged += new System.EventHandler(this.OnChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 5);
            this.label2.Margin = new System.Windows.Forms.Padding(5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "Startup File";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 30);
            this.label3.Margin = new System.Windows.Forms.Padding(5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "Output File (.exe)";
            // 
            // textBoxOutputFileName
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textBoxOutputFileName, 3);
            this.textBoxOutputFileName.Location = new System.Drawing.Point(117, 28);
            this.textBoxOutputFileName.Name = "textBoxOutputFileName";
            this.textBoxOutputFileName.Size = new System.Drawing.Size(375, 19);
            this.textBoxOutputFileName.TabIndex = 7;
            this.textBoxOutputFileName.TextChanged += new System.EventHandler(this.OnChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 55);
            this.label4.Margin = new System.Windows.Forms.Padding(5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "Icon File (.ico)";
            // 
            // textBoxIconFileName
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textBoxIconFileName, 3);
            this.textBoxIconFileName.Location = new System.Drawing.Point(117, 53);
            this.textBoxIconFileName.Name = "textBoxIconFileName";
            this.textBoxIconFileName.Size = new System.Drawing.Size(375, 19);
            this.textBoxIconFileName.TabIndex = 9;
            this.textBoxIconFileName.TextChanged += new System.EventHandler(this.OnChanged);
            // 
            // checkBoxUseReleaseBuild
            // 
            this.checkBoxUseReleaseBuild.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.checkBoxUseReleaseBuild, 2);
            this.checkBoxUseReleaseBuild.Enabled = false;
            this.checkBoxUseReleaseBuild.Location = new System.Drawing.Point(3, 125);
            this.checkBoxUseReleaseBuild.Name = "checkBoxUseReleaseBuild";
            this.checkBoxUseReleaseBuild.Size = new System.Drawing.Size(133, 16);
            this.checkBoxUseReleaseBuild.TabIndex = 10;
            this.checkBoxUseReleaseBuild.Text = "Enable Release Build";
            this.checkBoxUseReleaseBuild.UseVisualStyleBackColor = true;
            this.checkBoxUseReleaseBuild.Visible = false;
            this.checkBoxUseReleaseBuild.CheckedChanged += new System.EventHandler(this.OnChanged);
            // 
            // textBoxCustomSystemDirectory
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textBoxCustomSystemDirectory, 3);
            this.textBoxCustomSystemDirectory.Location = new System.Drawing.Point(117, 147);
            this.textBoxCustomSystemDirectory.Name = "textBoxCustomSystemDirectory";
            this.textBoxCustomSystemDirectory.Size = new System.Drawing.Size(375, 19);
            this.textBoxCustomSystemDirectory.TabIndex = 11;
            this.textBoxCustomSystemDirectory.TextChanged += new System.EventHandler(this.OnChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 149);
            this.label5.Margin = new System.Windows.Forms.Padding(5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(90, 24);
            this.label5.TabIndex = 12;
            this.label5.Text = "Custom System Directory";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 80);
            this.label6.Margin = new System.Windows.Forms.Padding(5);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 12);
            this.label6.TabIndex = 13;
            this.label6.Text = "WorkingDirectory";
            // 
            // textBoxWorkingDirectory
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textBoxWorkingDirectory, 3);
            this.textBoxWorkingDirectory.Location = new System.Drawing.Point(117, 78);
            this.textBoxWorkingDirectory.Name = "textBoxWorkingDirectory";
            this.textBoxWorkingDirectory.Size = new System.Drawing.Size(375, 19);
            this.textBoxWorkingDirectory.TabIndex = 14;
            this.textBoxWorkingDirectory.TextChanged += new System.EventHandler(this.OnChanged);
            // 
            // KuinGeneralPropertyPageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "KuinGeneralPropertyPageControl";
            this.Size = new System.Drawing.Size(519, 229);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.RadioButton radioButtonUseWndMode;
        private System.Windows.Forms.RadioButton radioButtonUseCuiMode;
        private System.Windows.Forms.RadioButton radioButtonUseWebMode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxStartupFileName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxOutputFileName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxIconFileName;
        private System.Windows.Forms.CheckBox checkBoxUseReleaseBuild;
        private System.Windows.Forms.TextBox textBoxCustomSystemDirectory;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxWorkingDirectory;
    }
}
