using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sgProtoDiag.util
{
    /// <summary>
    /// used for any storing flags stored in an 'enum'
    /// </summary>
    public class FlagSet<TypeFlags>
    {
        private UInt32 m_value = 0;

        public UInt32 Flags
        {
            get { return m_value; }
        }
        public void SetFlag(TypeFlags aFlag)
        {
            m_value |= System.Convert.ToUInt32(aFlag);
        }
        public void ClearFlag(TypeFlags aFlag)
        {
            m_value &= ~(System.Convert.ToUInt32(aFlag));
        }
    }
}
