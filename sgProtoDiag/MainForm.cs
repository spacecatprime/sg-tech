using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using sgProtoDiag.gamelogic;
using System.IO;

namespace sgProtoDiag
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Program.TheTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
        }

        private void splitContainer2_Panel2_Paint(object sender, PaintEventArgs e)
        {
        }

        private void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            eventsys.GameTime gt = Program.s_MM.TheGameTime;
            c_toolStripStatusLabel.Text = gt.ToString();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadDebug();
        }

        private void LoadDebug()
        {
            string[] tokenImages = 
            { 
                @"C:\Users\allen\Projects\spygame\proto\sgProtoDiag\sgProtoDiag\art\png\32\wallet.png", 
                @"C:\Users\allen\Projects\spygame\proto\sgProtoDiag\sgProtoDiag\art\png\32\client.png", 
                @"C:\Users\allen\Projects\spygame\proto\sgProtoDiag\sgProtoDiag\art\png\32\search.png", 
            };

            TabPage page = this.spyTokenStorage.TabPages[0];
            FlowLayoutPanel panel = page.Controls[0] as FlowLayoutPanel;
            for (int idx = 0; idx < 25; ++idx)
            {
                GameToken token = new GameToken();
                if (idx < tokenImages.Length)
                {
                    SpyToken spyToken = new SpyToken(new GameObject());
                    spyToken.Name = Path.GetFileNameWithoutExtension(tokenImages[idx]);
                    spyToken.TheMediaProxy.Visual = tokenImages[idx];
                    token.AssignSpyToken(spyToken);
                }
                panel.Controls.Add(token);
            }

            DropTrays.InsertActiveTokenTray(c_activeTokens);

            sgProtoDiag.ConversationDlg convDlg = new sgProtoDiag.ConversationDlg();
            convDlg.Dock = DockStyle.Fill;
            tabControlDialogs.TabPages[2].Controls.Add(convDlg);
        }

        internal class BitmapTray : FlowLayoutPanel, IDragDropTarget
        {
            #region IDragDropTarget Members

            DragDropEffects IDragDropTarget.GetDragOverEffect(DragEventArgs e)
            {
                IDragDropObject obj = DragDropManager.TheDragDropManager.FindDragDropObject(e);
                if (null == obj)
                {
                    return DragDropEffects.None;
                }
                return DragDropEffects.Copy;
            }

            void IDragDropTarget.HandleDroppedObject(DragEventArgs e)
            {
                IDragDropObject obj = DragDropManager.TheDragDropManager.FindDragDropObject(e);
                if (null != obj)
                {
                    ActionButton actionButton = new ActionButton();
                    actionButton.SetImage(obj.GetImage());
                    Controls.Add(actionButton);
                }
            }

            #endregion
        }

        private void OnPortraitDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else if (e.Data.GetData(typeof(Bitmap)) != null)
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void OnPortraitDragDrop(object sender, DragEventArgs e)
        {
            util.Monitoring.Logging.Debug(e.ToString());
        }
    }
}
