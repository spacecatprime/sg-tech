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
    public partial class ActionButton : UserControl
    {
        public ActionButton()
        {
            InitializeComponent();
        }

        public void SetImage(Image aImage)
        {
            this.button1.Image = aImage;
        }
    }

}
