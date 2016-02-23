using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// http://stackoverflow.com/questions/3231945/inherited-weakreference-throwing-reflectiontypeloadexception-in-silverlight

namespace sgProtoDiag.util
{
    public class WeakRef<T> where T : class
    {
        private WeakReference inner;

        public WeakRef(T target)
            : this(target, false)
        { 
        }

        public WeakRef(T target, bool trackResurrection)
        {
            if (target == null) 
                throw new ArgumentNullException("target");

            this.inner = new WeakReference((object)target, trackResurrection);
        }

        public T Target
        {
            get
            {
                return (T)this.inner.Target;
            }
            set
            {
                this.inner.Target = value;
            }
        }

        public bool IsAlive
        {
            get
            {
                return this.inner.IsAlive;
            }
        }
    }
}
