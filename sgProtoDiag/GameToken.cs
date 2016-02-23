using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using sgProtoDiag.gamelogic;

namespace sgProtoDiag
{
    /// <summary>
    /// 
    /// </summary>
    public partial class GameToken : UserControl, IDragDropObject, IDisposable
    {
        private WeakReference m_targetObject = null;

        public GameToken()
        {
            InitializeComponent();
            this.pictureBox1.Enabled = false;
        }

        internal bool AssignSpyToken(SpyToken spyToken)
        {
            if (spyToken != null)
            {
                m_targetObject = new WeakReference(spyToken);
                pictureBox1.BackgroundImage = spyToken.TheMediaProxy.GetImage();
                DragDropManager.TheDragDropManager.ApplyDraggable(this);
                return true;
            }
            return false;
        }

#region IDragDropObject Members

        bool IDragDropObject.CanDrag()
        {
            return m_targetObject.IsAlive;
        }

        void IDragDropObject.ApplyCustomCursor(GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;
            Cursor.Current = Cursors.Help;
        }

        Image IDragDropObject.GetImage()
        {
            return pictureBox1.BackgroundImage;
        }

        object IDragDropObject.GetGameObject()
        {
            return m_targetObject.Target;
        }

        void IDragDropObject.SetGameObject(object aGameObj)
        {
            if (aGameObj is SpyToken)
            { 
                SpyToken spyTok = aGameObj as SpyToken;
                pictureBox1.BackgroundImage = spyTok.TheMediaProxy.GetImage();
            }
        }

#endregion

#region IDisposable Members
        void IDisposable.Dispose()
        {
            DragDropManager.TheDragDropManager.RevokeDraggable(this);
        }
#endregion

    }
}
