namespace sgProtoDiag
{
    partial class DialogDebugger
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Node1");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Node2");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Actors", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Node10");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Node11");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Items", new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode5});
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Node8");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Node9");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Locations", new System.Windows.Forms.TreeNode[] {
            treeNode7,
            treeNode8});
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Node6");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Node7");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Variables", new System.Windows.Forms.TreeNode[] {
            treeNode10,
            treeNode11});
            this.c_lvSpokenItems = new System.Windows.Forms.ListView();
            this.colStatement = new System.Windows.Forms.ColumnHeader();
            this.colActor = new System.Windows.Forms.ColumnHeader();
            this.colChoice = new System.Windows.Forms.ColumnHeader();
            this.colText = new System.Windows.Forms.ColumnHeader();
            this.c_rtbChatText = new System.Windows.Forms.RichTextBox();
            this.c_tvObjects = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.c_rtbSpeaker = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.c_isPlayer = new System.Windows.Forms.RadioButton();
            this.c_lbChatOptions = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // c_lvSpokenItems
            // 
            this.c_lvSpokenItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colStatement,
            this.colActor,
            this.colChoice,
            this.colText});
            this.c_lvSpokenItems.Location = new System.Drawing.Point(12, 278);
            this.c_lvSpokenItems.Name = "c_lvSpokenItems";
            this.c_lvSpokenItems.Size = new System.Drawing.Size(641, 262);
            this.c_lvSpokenItems.TabIndex = 0;
            this.c_lvSpokenItems.UseCompatibleStateImageBehavior = false;
            this.c_lvSpokenItems.View = System.Windows.Forms.View.Details;
            // 
            // colStatement
            // 
            this.colStatement.Text = "";
            this.colStatement.Width = 20;
            // 
            // colActor
            // 
            this.colActor.Text = "Actor";
            // 
            // colChoice
            // 
            this.colChoice.Text = "Choice";
            this.colChoice.Width = 50;
            // 
            // colText
            // 
            this.colText.Text = "Text";
            this.colText.Width = 501;
            // 
            // c_rtbChatText
            // 
            this.c_rtbChatText.Location = new System.Drawing.Point(12, 63);
            this.c_rtbChatText.Name = "c_rtbChatText";
            this.c_rtbChatText.Size = new System.Drawing.Size(331, 96);
            this.c_rtbChatText.TabIndex = 1;
            this.c_rtbChatText.Text = "";
            // 
            // c_tvObjects
            // 
            this.c_tvObjects.Location = new System.Drawing.Point(349, 33);
            this.c_tvObjects.Name = "c_tvObjects";
            treeNode1.Name = "Node1";
            treeNode1.Text = "Node1";
            treeNode2.Name = "Node2";
            treeNode2.Text = "Node2";
            treeNode3.Name = "Node0";
            treeNode3.Text = "Actors";
            treeNode4.Name = "Node10";
            treeNode4.Text = "Node10";
            treeNode5.Name = "Node11";
            treeNode5.Text = "Node11";
            treeNode6.Name = "Node3";
            treeNode6.Text = "Items";
            treeNode7.Name = "Node8";
            treeNode7.Text = "Node8";
            treeNode8.Name = "Node9";
            treeNode8.Text = "Node9";
            treeNode9.Name = "Node4";
            treeNode9.Text = "Locations";
            treeNode10.Name = "Node6";
            treeNode10.Text = "Node6";
            treeNode11.Name = "Node7";
            treeNode11.Text = "Node7";
            treeNode12.Name = "Node5";
            treeNode12.Text = "Variables";
            this.c_tvObjects.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode3,
            treeNode6,
            treeNode9,
            treeNode12});
            this.c_tvObjects.Size = new System.Drawing.Size(304, 239);
            this.c_tvObjects.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(346, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Game Objects";
            // 
            // c_rtbSpeaker
            // 
            this.c_rtbSpeaker.Location = new System.Drawing.Point(62, 30);
            this.c_rtbSpeaker.Name = "c_rtbSpeaker";
            this.c_rtbSpeaker.Size = new System.Drawing.Size(200, 27);
            this.c_rtbSpeaker.TabIndex = 4;
            this.c_rtbSpeaker.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Speaker:";
            // 
            // c_isPlayer
            // 
            this.c_isPlayer.AutoSize = true;
            this.c_isPlayer.Enabled = false;
            this.c_isPlayer.Location = new System.Drawing.Point(268, 33);
            this.c_isPlayer.Name = "c_isPlayer";
            this.c_isPlayer.Size = new System.Drawing.Size(65, 17);
            this.c_isPlayer.TabIndex = 6;
            this.c_isPlayer.TabStop = true;
            this.c_isPlayer.Text = "Is Player";
            this.c_isPlayer.UseVisualStyleBackColor = true;
            // 
            // c_lbChatOptions
            // 
            this.c_lbChatOptions.FormattingEnabled = true;
            this.c_lbChatOptions.Items.AddRange(new object[] {
            "one",
            "two",
            "three"});
            this.c_lbChatOptions.Location = new System.Drawing.Point(12, 177);
            this.c_lbChatOptions.Name = "c_lbChatOptions";
            this.c_lbChatOptions.Size = new System.Drawing.Size(331, 95);
            this.c_lbChatOptions.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 162);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Chat Options:";
            // 
            // DialogDebugger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(663, 549);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.c_lbChatOptions);
            this.Controls.Add(this.c_isPlayer);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.c_rtbSpeaker);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.c_tvObjects);
            this.Controls.Add(this.c_rtbChatText);
            this.Controls.Add(this.c_lvSpokenItems);
            this.Name = "DialogDebugger";
            this.Text = "Dialog Debugger";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView c_lvSpokenItems;
        private System.Windows.Forms.ColumnHeader colStatement;
        private System.Windows.Forms.ColumnHeader colActor;
        private System.Windows.Forms.ColumnHeader colText;
        private System.Windows.Forms.RichTextBox c_rtbChatText;
        private System.Windows.Forms.TreeView c_tvObjects;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ColumnHeader colChoice;
        private System.Windows.Forms.RichTextBox c_rtbSpeaker;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton c_isPlayer;
        private System.Windows.Forms.ListBox c_lbChatOptions;
        private System.Windows.Forms.Label label3;
    }
}