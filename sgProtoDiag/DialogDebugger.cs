using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace sgProtoDiag
{
    public partial class DialogDebugger : Form
    {
        private gamelogic.GameConversation m_GameConv;

        public DialogDebugger(gamelogic.GameConversation aGameConversation)
        {
            InitializeComponent();
            m_GameConv = aGameConversation;

            c_lbChatOptions.DoubleClick += new EventHandler(OnOption_DoubleClick);

            FillOutOptions();
        }

        void OnOption_DoubleClick(object sender, EventArgs e)
        {
            if (c_lbChatOptions.SelectedItem == null)
            {
                return;
            }
            List<gamedata.ConversationOption> options = m_GameConv.GetAvaiableOptionsForActive();
            string selectedItem = c_lbChatOptions.SelectedItem.ToString();
            if (options.Count == 1)
            {
                m_GameConv.Advance(options[0]);
            }
            else if(!string.IsNullOrEmpty(selectedItem))
            {
                gamedata.ConversationOption o = options.Find(delegate(gamedata.ConversationOption opt)
                {
                    string[] menuText = selectedItem.Split('~');
                    return opt.MenuText == menuText[0];
                });
                if (null != o)
                {
                    m_GameConv.Advance(o);
                }
            }
            FillOutOptions();
        }

        private void FillOutOptions()
        {
            this.Text = string.Format("Debugger: {0} of {1}", m_GameConv.ID, m_GameConv.ActiveDialog.TheDialogEntry.Title);

            c_isPlayer.Checked = true;
            c_rtbSpeaker.Text = m_GameConv.ActiveDialog.TheDialogEntry.Title;

            if (string.IsNullOrEmpty(m_GameConv.ActiveDialog.TheDialogEntry.DialogueText))
            {
                c_rtbChatText.Text = "(none)";
            }
            else
            {
                c_rtbChatText.Text = m_GameConv.ActiveDialog.TheDialogEntry.DialogueText;
            }

            c_lbChatOptions.Items.Clear();
            List<gamedata.ConversationOption> options = m_GameConv.GetAvaiableOptionsForActive();
            if (options.Count == 0)
            {
                c_lbChatOptions.Items.Add("ERROR");
            }
            else if (options.Count == 1)
            {
                c_lbChatOptions.Items.Add(options[0].MenuText == null ? "Advance" : options[0].MenuText);
            }
            else
            {
                foreach (var o in options)
                {
                    if (string.IsNullOrEmpty(o.MenuText))
                    {
                        c_lbChatOptions.Items.Add("empty:" + o.DestinationDialogID.ToString());
                    }
                    else
                    {

                        c_lbChatOptions.Items.Add(string.Format("{0}~{1}~{2}", o.MenuText, o.IsPassThrough, o.IsConnector));
                        //c_lbChatOptions.Items.Add(o.MenuText);
                    }
                }
            }
        }
    }
}
