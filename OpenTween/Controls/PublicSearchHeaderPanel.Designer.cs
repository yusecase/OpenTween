namespace OpenTween.Controls
{
    partial class PublicSearchHeaderPanel
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PublicSearchHeaderPanel));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxQuery = new System.Windows.Forms.ComboBox();
            this.comboBoxLang = new System.Windows.Forms.ComboBox();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxQuery, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxLang, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonSearch, 3, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Enter += new System.EventHandler(this.TableLayoutPanel1_Enter);
            this.tableLayoutPanel1.Leave += new System.EventHandler(this.TableLayoutPanel1_Leave);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // comboBoxQuery
            // 
            resources.ApplyResources(this.comboBoxQuery, "comboBoxQuery");
            this.comboBoxQuery.FormattingEnabled = true;
            this.comboBoxQuery.Name = "comboBoxQuery";
            this.comboBoxQuery.TabStop = false;
            this.comboBoxQuery.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ComboBoxQuery_KeyDown);
            // 
            // comboBoxLang
            // 
            resources.ApplyResources(this.comboBoxLang, "comboBoxLang");
            this.comboBoxLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLang.FormattingEnabled = true;
            this.comboBoxLang.Items.AddRange(new object[] {
            resources.GetString("comboBoxLang.Items"),
            resources.GetString("comboBoxLang.Items1"),
            resources.GetString("comboBoxLang.Items2"),
            resources.GetString("comboBoxLang.Items3"),
            resources.GetString("comboBoxLang.Items4"),
            resources.GetString("comboBoxLang.Items5"),
            resources.GetString("comboBoxLang.Items6"),
            resources.GetString("comboBoxLang.Items7"),
            resources.GetString("comboBoxLang.Items8"),
            resources.GetString("comboBoxLang.Items9"),
            resources.GetString("comboBoxLang.Items10"),
            resources.GetString("comboBoxLang.Items11"),
            resources.GetString("comboBoxLang.Items12"),
            resources.GetString("comboBoxLang.Items13"),
            resources.GetString("comboBoxLang.Items14"),
            resources.GetString("comboBoxLang.Items15"),
            resources.GetString("comboBoxLang.Items16"),
            resources.GetString("comboBoxLang.Items17"),
            resources.GetString("comboBoxLang.Items18"),
            resources.GetString("comboBoxLang.Items19")});
            this.comboBoxLang.Name = "comboBoxLang";
            this.comboBoxLang.TabStop = false;
            this.comboBoxLang.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ComboBoxLang_KeyDown);
            // 
            // buttonSearch
            // 
            resources.ApplyResources(this.buttonSearch, "buttonSearch");
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.TabStop = false;
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.ButtonSearch_Click);
            // 
            // PublicSearchHeaderPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PublicSearchHeaderPanel";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxQuery;
        private System.Windows.Forms.ComboBox comboBoxLang;
        private System.Windows.Forms.Button buttonSearch;
    }
}
