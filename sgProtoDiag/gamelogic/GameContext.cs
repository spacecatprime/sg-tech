using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sgProtoDiag.gamelogic
{
    /// <summary>
    /// a game state change, interaction, or event that might affect the state of other GameObjects
    /// </summary>
    public class GameContext
    {
        public object m_source = null;
        public object m_target = null;
        public object m_payload = null;

        public GameContext()
        {
        }

        public GameContext(object aSrc, object aTarget, object aPayload)
        {
            m_source = aSrc;
            m_target = aTarget;
            m_payload = aPayload;
        }
    }

}
