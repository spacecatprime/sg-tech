using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace sgProtoDiag
{
    class DropTrays
    {
        public static bool InsertActiveTokenTray(FlowLayoutPanel aFlowPanel)
        {
            ActiveTokenTray activeTokenTray = new ActiveTokenTray();
            Util.CopyControl(activeTokenTray, aFlowPanel);
            activeTokenTray.FlowDirection = aFlowPanel.FlowDirection;
            activeTokenTray.BorderStyle = aFlowPanel.BorderStyle;
            DragDropManager.TheDragDropManager.ApplyDragTarget(activeTokenTray);
            aFlowPanel.Parent.Controls.Add(activeTokenTray);
            aFlowPanel.Hide();
            return true;
        }
    }

    //internal class TFlowTray<TButton> : FlowLayoutPanel, IDragDropTarget
    //{
    //    DragDropEffects IDragDropTarget.GetDragOverEffect(DragEventArgs e)
    //    {
    //        IDragDropObject obj = DragDropManager.TheDragDropManager.FindDragDropObject(e);
    //        if (null == obj)
    //        {
    //            return DragDropEffects.None;
    //        }
    //        return DragDropEffects.Copy;
    //    }
    //    void IDragDropTarget.HandleDroppedObject(DragEventArgs e)
    //    {
    //        IDragDropObject obj = DragDropManager.TheDragDropManager.FindDragDropObject(e);
    //        if (null != obj)
    //        {
    //            TButton actionButton = new TButton();
    //            IDragDropObject actionObj = actionButton as IDragDropObject;
    //            actionButton.SetGameObject(actionButton);
    //            Controls.Add(actionButton);
    //        }
    //    }
    //}

    internal class ActiveTokenTray : FlowLayoutPanel, IDragDropTarget
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
}
