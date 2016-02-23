using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

// http://msdn.microsoft.com/en-us/library/system.windows.forms.control.dodragdrop.aspx
// http://www.codeproject.com/Articles/16905/Drag-and-Drop-Image-in-C-NET
// http://stackoverflow.com/questions/3240603/c-sharp-drag-and-drop-show-the-dragged-item-while-dragging

namespace sgProtoDiag
{
    interface IDragDropObject
    {
        bool CanDrag();
        void ApplyCustomCursor(GiveFeedbackEventArgs e);
        Image GetImage();
        object GetGameObject();
        void SetGameObject(object aGameObj);
    }

    interface IDragDropTarget
    {
        DragDropEffects GetDragOverEffect(DragEventArgs e);
        void HandleDroppedObject(DragEventArgs e);
    }

    class DragDropManager
    {
        private Rectangle m_dragBoxFromMouseDown;

        static public DragDropManager TheDragDropManager;
        static DragDropManager()
        {
            TheDragDropManager = new DragDropManager();
        }

#region Source Draggable Control
        public bool ApplyDraggable(Control aControl)
        {
            aControl.MouseDown += new System.Windows.Forms.MouseEventHandler(Source_MouseDown);
//            aControl.QueryContinueDrag += new System.Windows.Forms.QueryContinueDragEventHandler(Source_QueryContinueDrag);
            aControl.MouseMove += new System.Windows.Forms.MouseEventHandler(Source_MouseMove);
//            aControl.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(Source_GiveFeedback);
            return true;
        }

        public bool RevokeDraggable(Control aControl)
        {
            aControl.MouseDown -= new System.Windows.Forms.MouseEventHandler(Source_MouseDown);
//            aControl.QueryContinueDrag -= new System.Windows.Forms.QueryContinueDragEventHandler(Source_QueryContinueDrag);
            aControl.MouseMove -= new System.Windows.Forms.MouseEventHandler(Source_MouseMove);
//            aControl.GiveFeedback -= new System.Windows.Forms.GiveFeedbackEventHandler(Source_GiveFeedback);
            return true;
        }

        private void Source_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Get the index of the item the mouse is below.
            IDragDropObject ddo = sender as IDragDropObject;
            if (ddo != null && ddo.CanDrag())
            {
                // Remember the point where the mouse down occurred. The DragSize indicates
                // the size that the mouse can move before a drag event should be started.                
                Size dragSize = SystemInformation.DragSize;

                // Create a rectangle using the DragSize, with the mouse position being
                // at the center of the rectangle.
                m_dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2),
                                                                 e.Y - (dragSize.Height / 2)), dragSize);
            }
            else
            {
                // Reset the rectangle if the mouse is not over an item in the ListBox.
                m_dragBoxFromMouseDown = Rectangle.Empty;
            }
        }

        private void Source_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            IDragDropObject ddo = sender as IDragDropObject;
            if (ddo == null)
            {
                return;
            }

            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                IDragDropObject srcControl = sender as IDragDropObject;
                if (srcControl == null)
                {
                    return;
                }

                // If the mouse moves outside the rectangle, start the drag.
                if (m_dragBoxFromMouseDown != Rectangle.Empty && !m_dragBoxFromMouseDown.Contains(e.X, e.Y))
                {
                    // Proceed with the drag-and-drop
                    Bitmap bmp = new Bitmap(ddo.GetImage());
                    bmp.Tag = sender;
                    Control ctrl = sender as Control;
                    DragDropEffects dde = ctrl.DoDragDrop(bmp, DragDropEffects.All);
                }
            }
        }

        private void Source_GiveFeedback(object sender, System.Windows.Forms.GiveFeedbackEventArgs e)
        {
            //IDragDropObject ddo = sender as IDragDropObject;
            //if (ddo != null)
            //{
            //    if (m_bDragging)
            //    {
            //        ddo.ApplyCustomCursor(e);
            //    }
            //    else
            //    {
            //        e.UseDefaultCursors = true;
            //        Cursor.Current = Cursors.Default;
            //    }
            //}
        }

        private void Source_QueryContinueDrag(object sender, System.Windows.Forms.QueryContinueDragEventArgs e)
        {
            //// Cancel the drag if the mouse moves off the form.
            //IDragDropObject srcObj = sender as IDragDropObject;
            //if (srcObj != null)
            //{
            //    if ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left)
            //    {
            //        e.Action = DragAction.Continue;
            //    }
            //    else
            //    {
            //        e.Action = DragAction.Cancel;
            //        m_bDragging = false;
            //    }
            //}
        }
#endregion

#region Target Draggable Control
        public bool ApplyDragTarget(Control aControl)
        {
            aControl.AllowDrop = true;
            //aControl.DragOver += new System.Windows.Forms.DragEventHandler(Target_DragOver);
            aControl.DragDrop += new System.Windows.Forms.DragEventHandler(Target_DragDrop);
            aControl.DragEnter += new System.Windows.Forms.DragEventHandler(Target_DragEnter);
            //aControl.DragLeave += new System.EventHandler(Target_DragLeave);
            return true;
        }

        public bool RevokeDragTarget(Control aControl)
        {
            aControl.AllowDrop = false;
            //aControl.DragOver -= new System.Windows.Forms.DragEventHandler(Target_DragOver);
            aControl.DragDrop -= new System.Windows.Forms.DragEventHandler(Target_DragDrop);
            aControl.DragEnter -= new System.Windows.Forms.DragEventHandler(Target_DragEnter);
            //aControl.DragLeave -= new System.EventHandler(Target_DragLeave);
            return true;
        }

        private void Target_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {
            //IDragDropTarget target = sender as IDragDropTarget;
            //if (target != null)
            //{
            //    // let the target determine if the effect can happen
            //    e.Effect = target.GetDragOverEffect(e);
            //}
            //else
            //{
            //    e.Effect = DragDropEffects.Copy;
            //}
        }

        private void Target_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            IDragDropTarget target = sender as IDragDropTarget;
            if (target != null)
            {
                target.HandleDroppedObject(e);
            }
        }

        private void Target_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            IDragDropTarget target = sender as IDragDropTarget;
            if (target != null)
            {
                e.Effect = target.GetDragOverEffect(e);
            }
            else
            {
              e.Effect = DragDropEffects.Copy;
            }
        }

        private void Target_DragLeave(object sender, System.EventArgs e)
        {
            //IDragDropTarget target = sender as IDragDropTarget;
            //if (target != null)
            //{
            //    target.HandleDragLeave(e);
            //}
        }

        public IDragDropObject FindDragDropObject(System.Windows.Forms.DragEventArgs e)
        {
            Bitmap bmp = (Bitmap)e.Data.GetData(typeof(Bitmap));
            if (bmp != null)
            {
                return bmp.Tag as IDragDropObject;
            }

            foreach (string type in e.Data.GetFormats())
            {
                object obj = e.Data.GetData(type);
                Type t = obj.GetType().GetInterface("IDragDropObject");
                if (t != null)
                {
                    return (IDragDropObject)obj;
                }
            }
            return null;
        }
#endregion
    }
}
