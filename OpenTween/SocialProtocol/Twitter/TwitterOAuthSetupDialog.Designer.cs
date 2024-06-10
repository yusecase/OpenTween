namespace OpenTween.SocialProtocol.Twitter
{
    partial class TwitterOAuthSetupDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TwitterOAuthSetupDialog));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxInputConsumerKey = new System.Windows.Forms.GroupBox();
            this.checkBoxUseCustomConsumerKey = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxCustomConsumerKey = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxCustomConsumerSecret = new System.Windows.Forms.TextBox();
            this.buttonGetAuthorizeUri = new System.Windows.Forms.Button();
            this.groupBoxInputPinCode = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.linkLabelAuthorize = new System.Windows.Forms.LinkLabel();
            this.contextMenuLinkLabel = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemCopyLink = new System.Windows.Forms.ToolStripMenuItem();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxPinCode = new System.Windows.Forms.TextBox();
            this.buttonGetAccessToken = new System.Windows.Forms.Button();
            this.bindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.groupBoxInputConsumerKey.SuspendLayout();
            this.groupBoxInputPinCode.SuspendLayout();
            this.contextMenuLinkLabel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // groupBoxInputConsumerKey
            // 
            resources.ApplyResources(this.groupBoxInputConsumerKey, "groupBoxInputConsumerKey");
            this.groupBoxInputConsumerKey.Controls.Add(this.checkBoxUseCustomConsumerKey);
            this.groupBoxInputConsumerKey.Controls.Add(this.label1);
            this.groupBoxInputConsumerKey.Controls.Add(this.textBoxCustomConsumerKey);
            this.groupBoxInputConsumerKey.Controls.Add(this.label2);
            this.groupBoxInputConsumerKey.Controls.Add(this.textBoxCustomConsumerSecret);
            this.groupBoxInputConsumerKey.Controls.Add(this.buttonGetAuthorizeUri);
            this.groupBoxInputConsumerKey.Name = "groupBoxInputConsumerKey";
            this.groupBoxInputConsumerKey.TabStop = false;
            // 
            // checkBoxUseCustomConsumerKey
            // 
            resources.ApplyResources(this.checkBoxUseCustomConsumerKey, "checkBoxUseCustomConsumerKey");
            this.checkBoxUseCustomConsumerKey.Name = "checkBoxUseCustomConsumerKey";
            this.checkBoxUseCustomConsumerKey.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // textBoxCustomConsumerKey
            // 
            resources.ApplyResources(this.textBoxCustomConsumerKey, "textBoxCustomConsumerKey");
            this.textBoxCustomConsumerKey.Name = "textBoxCustomConsumerKey";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // textBoxCustomConsumerSecret
            // 
            resources.ApplyResources(this.textBoxCustomConsumerSecret, "textBoxCustomConsumerSecret");
            this.textBoxCustomConsumerSecret.Name = "textBoxCustomConsumerSecret";
            // 
            // buttonGetAuthorizeUri
            // 
            resources.ApplyResources(this.buttonGetAuthorizeUri, "buttonGetAuthorizeUri");
            this.buttonGetAuthorizeUri.Name = "buttonGetAuthorizeUri";
            this.buttonGetAuthorizeUri.UseVisualStyleBackColor = true;
            this.buttonGetAuthorizeUri.Click += new System.EventHandler(this.ButtonGetAuthorizeUri_Click);
            // 
            // groupBoxInputPinCode
            // 
            resources.ApplyResources(this.groupBoxInputPinCode, "groupBoxInputPinCode");
            this.groupBoxInputPinCode.Controls.Add(this.label3);
            this.groupBoxInputPinCode.Controls.Add(this.linkLabelAuthorize);
            this.groupBoxInputPinCode.Controls.Add(this.label4);
            this.groupBoxInputPinCode.Controls.Add(this.textBoxPinCode);
            this.groupBoxInputPinCode.Controls.Add(this.buttonGetAccessToken);
            this.groupBoxInputPinCode.Name = "groupBoxInputPinCode";
            this.groupBoxInputPinCode.TabStop = false;
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
            // contextMenuLinkLabel
            // 
            this.contextMenuLinkLabel.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemCopyLink});
            this.contextMenuLinkLabel.Name = "contextMenuLinkLabel";
            resources.ApplyResources(this.contextMenuLinkLabel, "contextMenuLinkLabel");
            // 
            // menuItemCopyLink
            // 
            this.menuItemCopyLink.Name = "menuItemCopyLink";
            resources.ApplyResources(this.menuItemCopyLink, "menuItemCopyLink");
            this.menuItemCopyLink.Click += new System.EventHandler(this.MenuItemCopyLink_Click);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // textBoxPinCode
            // 
            resources.ApplyResources(this.textBoxPinCode, "textBoxPinCode");
            this.textBoxPinCode.Name = "textBoxPinCode";
            // 
            // buttonGetAccessToken
            // 
            resources.ApplyResources(this.buttonGetAccessToken, "buttonGetAccessToken");
            this.buttonGetAccessToken.Name = "buttonGetAccessToken";
            this.buttonGetAccessToken.UseVisualStyleBackColor = true;
            this.buttonGetAccessToken.Click += new System.EventHandler(this.ButtonGetAccessToken_Click);
            // 
            // TwitterOAuthSetupDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.groupBoxInputConsumerKey);
            this.Controls.Add(this.groupBoxInputPinCode);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "TwitterOAuthSetupDialog";
            this.groupBoxInputConsumerKey.ResumeLayout(false);
            this.groupBoxInputConsumerKey.PerformLayout();
            this.groupBoxInputPinCode.ResumeLayout(false);
            this.groupBoxInputPinCode.PerformLayout();
            this.contextMenuLinkLabel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBoxInputConsumerKey;
        private System.Windows.Forms.CheckBox checkBoxUseCustomConsumerKey;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxCustomConsumerKey;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxCustomConsumerSecret;
        private System.Windows.Forms.Button buttonGetAuthorizeUri;
        private System.Windows.Forms.GroupBox groupBoxInputPinCode;
        private System.Windows.Forms.LinkLabel linkLabelAuthorize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxPinCode;
        private System.Windows.Forms.Button buttonGetAccessToken;
        private System.Windows.Forms.ContextMenuStrip contextMenuLinkLabel;
        private System.Windows.Forms.ToolStripMenuItem menuItemCopyLink;
        private System.Windows.Forms.BindingSource bindingSource;
    }
}