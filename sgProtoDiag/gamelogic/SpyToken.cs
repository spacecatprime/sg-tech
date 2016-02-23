using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sgProtoDiag.gamelogic
{
    /// <summary>
    /// represents any kind of object that can be examined and or used either an interaction
    /// </summary>
    public class SpyToken
    {
        /// <summary>
        /// the type of storage the token can be stored as flags
        /// </summary>
        public enum StorageFlags { Inventory = 1, Journal = 2, Memory = 4 };
        public util.FlagSet<StorageFlags> TheStorage { get; set; }
        public String Name { get; set; }
        public ui.MediaProxy TheMediaProxy { get; set; }

        private util.WeakRef<GameObject> m_ObjectRef;
        public GameObject ObjectRef
        {
            get { return m_ObjectRef.Target; }
        }

        public SpyToken(GameObject aGameObj)
        {
            TheStorage = new util.FlagSet<StorageFlags>();
            TheMediaProxy = new ui.MediaProxy();
            m_ObjectRef = new util.WeakRef<GameObject>(aGameObj);
        }
    }
}
