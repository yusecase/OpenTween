namespace OpenTween.SocialProtocol.Misskey
{
    partial class MisskeySetupDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MisskeySetupDialog));
            this.bindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.groupBoxInputServerName = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxServerHostname = new System.Windows.Forms.TextBox();
            this.buttonGetAuthorizeUri = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.menuItemCopyLink = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuLinkLabel = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.linkLabelAuthorize = new System.Windows.Forms.LinkLabel();
            this.groupBoxAuthorize = new System.Windows.Forms.GroupBox();
            this.buttonGetAccessToken = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).BeginInit();
            this.groupBoxInputServerName.SuspendLayout();
            this.contextMenuLinkLabel.SuspendLayout();
            this.groupBoxAuthorize.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxInputServerName
            // 
            resources.ApplyResources(this.groupBoxInputServerName, "groupBoxInputServerName");
            this.groupBoxInputServerName.Controls.Add(this.label1);
            this.groupBoxInputServerName.Controls.Add(this.label2);
            this.groupBoxInputServerName.Controls.Add(this.textBoxServerHostname);
            this.groupBoxInputServerName.Controls.Add(this.buttonGetAuthorizeUri);
            this.groupBoxInputServerName.Name = "groupBoxInputServerName";
            this.groupBoxInputServerName.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // textBoxServerHostname
            // 
            resources.ApplyResources(this.textBoxServerHostname, "textBoxServerHostname");
            this.textBoxServerHostname.Name = "textBoxServerHostname";
            // 
            // buttonGetAuthorizeUri
            // 
            resources.ApplyResources(this.buttonGetAuthorizeUri, "buttonGetAuthorizeUri");
            this.buttonGetAuthorizeUri.Name = "buttonGetAuthorizeUri";
            this.buttonGetAuthorizeUri.UseVisualStyleBackColor = true;
            this.buttonGetAuthorizeUri.Click += new System.EventHandler(this.ButtonGetAuthorizeUri_Click);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // menuItemCopyLink
            // 
            this.menuItemCopyLink.Name = "menuItemCopyLink";
            resources.ApplyResources(this.menuItemCopyLink, "menuItemCopyLink");
            this.menuItemCopyLink.Click += new System.EventHandler(this.MenuItemCopyLink_Click);
            // 
            // contextMenuLinkLabel
            // 
            this.contextMenuLinkLabel.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemCopyLink});
            this.contextMenuLinkLabel.Name = "contextMenuLinkLabel";
            resources.ApplyResources(this.contextMenuLinkLabel, "contextMenuLinkLabel");
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // linkLabelAuthorize
            // 
            resources.ApplyResources(this.linkLabelAuthorize, "linkLabelAuthorize");
            this.linkLabelAuthorize.AutoEllipsis = true;
            this.linkLabelAuthorize.ContextMenuStrip = this.contextMenuLinkLabel;
            this.linkLabelAuthorize.Name = "linkLabelAuthorize";
            this.linkLabelAuthorize.TabStop = true;
            this.linkLabelAuthorize.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelAuthorize_LinkClicked);
            // 
            // groupBoxAuthorize
            // 
            resources.ApplyResources(this.groupBoxAuthorize, "groupBoxAuthorize");
            this.groupBoxAuthorize.Controls.Add(this.label3);
            this.groupBoxAuthorize.Controls.Add(this.linkLabelAuthorize);
            this.groupBoxAuthorize.Controls.Add(this.label4);
            this.groupBoxAuthorize.Controls.Add(this.buttonGetAccessToken);
            this.groupBoxAuthorize.Name = "groupBoxAuthorize";
            this.groupBoxAuthorize.TabStop = false;
            // 
            // buttonGetAccessToken
            // 
            resources.ApplyResources(this.buttonGetAccessToken, "buttonGetAccessToken");
            this.buttonGetAccessToken.Name = "buttonGetAccessToken";
            this.buttonGetAccessToken.UseVisualStyleBackColor = true;
            this.buttonGetAccessToken.Click += new System.EventHandler(this.ButtonGetAccessToken_Click);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // MisskeySetupDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.groupBoxInputServerName);
            this.Controls.Add(this.groupBoxAuthorize);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MisskeySetupDialog";
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).EndInit();
            this.groupBoxInputServerName.ResumeLayout(false);
            this.groupBoxInputServerName.PerformLayout();
            this.contextMenuLinkLabel.ResumeLayout(false);
            this.groupBoxAuthorize.ResumeLayout(false);
            this.groupBoxAuthorize.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.BindingSource bindingSource;
        private System.Windows.Forms.GroupBox groupBoxInputServerName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxServerHostname;
        private System.Windows.Forms.Button buttonGetAuthorizeUri;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStripMenuItem menuItemCopyLink;
        private System.Windows.Forms.ContextMenuStrip contextMenuLinkLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel linkLabelAuthorize;
        private System.Windows.Forms.GroupBox groupBoxAuthorize;
        private System.Windows.Forms.Button buttonGetAccessToken;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label2;
    }
}