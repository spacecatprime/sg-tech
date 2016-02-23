using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace sgProtoDiag
{
    public partial class ConversationDlg : UserControl
    {
        int m_count = 10;
        gamelogic.GameStory m_gameStory = null;

        public ConversationDlg()
        {
            InitializeComponent();
            tvTalker.ImageList = this.imageList1;
            tvTalker.Nodes.Clear();

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(@"C:\Users\allen\Projects\spygame\proto\sgProtoDiag\sgProtoDiag\docs\Example.xml");

            m_gameStory = new gamelogic.GameStory();
            m_gameStory.Load(doc);

            List<gamelogic.GameConversation> convList = m_gameStory.ConversationList;
            convList.ForEach(delegate(gamelogic.GameConversation conv)
            {
                TreeNode tnode = new TreeNode(conv.Conversation.Name);
                tnode.Tag = conv;
                FollowDialogTree(new gamedata.ConversationContext(conv.Conversation), 0, tnode);
                tvTalker.Nodes.Add(tnode);
            });
        }

        private gamelogic.GameConversation GetGameConv(TreeNode aKid)
        {
            if (null == aKid || aKid.Tag == null)
            {
                return null;
            }
            gamelogic.GameConversation gameConv = aKid.Tag as gamelogic.GameConversation;
            if (null == gameConv)
            {
                return GetGameConv(aKid.Parent);
            }
            return gameConv;
        }

        private gamedata.ConversationDialog UnpackConversationDialog(TreeNode aNode)
        {
            if (aNode.Tag is KeyValuePair<gamedata.ConversationContext, int>)
            {
                KeyValuePair<gamedata.ConversationContext, int> dialogInfo =
                    (KeyValuePair<gamedata.ConversationContext, int>)aNode.Tag;

                return dialogInfo.Key.TheConversation.GetDialogById(dialogInfo.Value);
            }
            return null;
        }

        private void AdvanceConversationNode(TreeNode treeNode)
        {
            foreach (TreeNode kidNode in treeNode.Nodes)
            {
                if (kidNode.Checked)
                {
                    gamedata.ConversationDialog srcDlg = UnpackConversationDialog(treeNode);
                    gamedata.ConversationDialog optDlg = UnpackConversationDialog(kidNode);
                    gamelogic.GameConversation gameConv = GetGameConv(kidNode);
                    if (gameConv.ApplyOptionForDialog(srcDlg, new gamedata.ConversationOption(optDlg)))
                    {
                        treeNode.ImageKey = "auction-hammer.png";
                    }
                    return;
                }
            }
        }

        private void FollowDialogTree(gamedata.ConversationContext aContext, int aDialogId, TreeNode aTreeNode)
        {
            gamedata.ConversationDialog dlg = aContext.TheConversation.GetDialogById(aDialogId);

            TreeNode tnode = new TreeNode(string.Format("[{0}] {1}", dlg.DialogEntryID, dlg.TheDialogEntry.Title));
            tnode.ImageKey = "light-bulb-off";
            aTreeNode.Nodes.Add(tnode);
            tnode.Tag = new KeyValuePair<gamedata.ConversationContext, int>(aContext, aDialogId);

            var optionList = dlg.GetOptions(aContext);
            foreach (gamedata.ConversationOption opt in optionList)
            {
                if (opt.DestinationDialogID > aDialogId)
                {
                    FollowDialogTree(aContext, opt.DestinationDialogID, tnode);
                }
            }
        }

        void menu_LoadConversation(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CheckedListBox.CheckedItemCollection items = checkedListBox1.CheckedItems;

            foreach (object box in checkedListBox1.CheckedItems)
            {
                switch (box.ToString())
                {
                    case "new node":
                    {
                        if (tvTalker.SelectedNode != null)
                        {
                            tvTalker.SelectedNode.Nodes.Add("node" + m_count.ToString());
                        }
                        break;
                    }
                    case "delete node":
                    {
                        if (tvTalker.SelectedNode != null)
                        {
                            tvTalker.SelectedNode.Remove();
                        }
                        break;
                    }
                    case "forecolor":
                    {
                        if (tvTalker.SelectedNode != null)
                        {
                            tvTalker.SelectedNode.ForeColor = RandomColor();
                        }
                        break;
                    }
                    case "backcolor":
                    {
                        if (tvTalker.SelectedNode != null)
                        {
                            tvTalker.SelectedNode.BackColor = RandomColor();
                        }
                        break;
                    }
                    case "picture":
                    {
                        if (tvTalker.SelectedNode != null)
                        {
                            tvTalker.SelectedNode.ImageIndex = m_count++ % tvTalker.ImageList.Images.Count;
                            tvTalker.SelectedNode.SelectedImageIndex = m_count++ % tvTalker.ImageList.Images.Count;
                        }
                        break;
                    }
                    case "start":
                    {
                        if (tvTalker.SelectedNode != null)
                        {
                            util.Monitoring.Logging.Debug("starting dlg at {0}", tvTalker.SelectedNode.Tag.ToString());
                            gamelogic.GameConversation gameConv = tvTalker.SelectedNode.Tag as gamelogic.GameConversation;
                            if (null != gameConv)
                            {
                                gameConv.SetActiveDialog(0);
                                DialogDebugger dd = new DialogDebugger(gameConv);
                                dd.Show(this);
                            }
                        }
                        break;
                    }
                    case "reset":
                    {
                        if (tvTalker.SelectedNode != null)
                        {
                            util.Monitoring.Logging.Debug("heard reset");
                        }
                        break;
                    }
                    default:
                    {
                        if (tvTalker.SelectedNode != null)
                        {
                            util.Monitoring.Logging.Debug("starting dlg at {0}", tvTalker.SelectedNode.Tag.ToString());
                            gamelogic.GameConversation gameConv = tvTalker.SelectedNode.Tag as gamelogic.GameConversation;
                            if (null != gameConv)
                            {
                                DialogDebugger dd = new DialogDebugger(gameConv);
                                dd.Show(this);
                            }
                        }
                        break;
                    }
                }
            }
        }

        private Color RandomColor()
        {
            Color[] colors = new Color[] 
            { 
                Color.Red, 
                Color.SaddleBrown, 
                Color.Blue, 
                Color.Green 
            };
            m_count++;
            return colors[m_count % colors.Length];
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void tvTalker_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeView tview = sender as TreeView;
                if (tview != null && tview.SelectedNode != null)
                {
                    AdvanceConversationNode(tview.SelectedNode);
                }
            }
        }
    }
}
