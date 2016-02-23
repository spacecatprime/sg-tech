using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sgProtoDiag.ui
{
    /// <summary>
    /// contains the descriptors for bitmaps, sounds, dialog trees,
    /// </summary>
    public class MediaProxy
    {
        public string Visual { get; set; }
        public string Audio { get; set; }

        public virtual System.Drawing.Image GetImage()
        {
            if (string.IsNullOrEmpty(Visual))
            {
                return null;
            }
            try
            {
                return System.Drawing.Image.FromFile(Visual);
            }
            catch (System.Exception ex)
            {
                util.Monitoring.Logging.Exception(ex);
            }
            return null;
        }
    }
}
