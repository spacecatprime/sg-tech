using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sgProtoDiag.util
{
    public class PropertyLabel : IComparable
    {
        private string m_label;

        public static PropertyLabel INVALID = new PropertyLabel(".");

        public string Label
        {
            get { return m_label; }
        }

        public PropertyLabel(string aLabel)
        {
            if (string.IsNullOrEmpty(aLabel))
            {
                throw new Exception("Invalid label!");
            }
            else if (aLabel.Contains(' '))
            {
                string[] parts = aLabel.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                m_label = string.Join(".", parts);
                m_label = m_label.ToLower();
            }
            else
            {
                m_label = aLabel.ToLower();
            }
        }

        public bool DoesMatch(string aSzLabel)
        {
            try
            {
                return DoesMatch(new PropertyLabel(aSzLabel));
            }
            catch (System.Exception ex)
            {
                util.Monitoring.Logging.Exception(ex);
                return false;
            }
        }

        public bool DoesMatch(PropertyLabel aLabel)
        {
            if (aLabel.m_label == m_label)
            {
                return true;
            }
            string[] src = aLabel.m_label.Split('.');
            string[] dst = m_label.Split('.');

            // to specific?
            if (src.Length > dst.Length)
            {
                return false;
            }

            int x = 0;
            do
            {
                if (src[x] != "*" && dst[x] != "*")
                {
                    if (src[x] != dst[x])
                    {
                        return false;
                    }
                }
                ++x;
            }
            while( x < src.Length && x < dst.Length);

            // true if both same length 
            return src[src.Length - 1] == "*";
        }

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// a generic container of properties and values where the keys are stored as dot separated strings
    /// </summary>
    public class PropertyBag
    {
        public delegate double HandleValueList(string aLabel, List<Double> aList);

        // map properties pivoted by labels [TODO: come up with adjustment filters?]
        private Dictionary<string, List<double>> mPropertyMap = new Dictionary<string, List<double>>(); 

        /// <summary>
        /// assigns a value to a PropertyLabel
        /// </summary>
        /// <param name="aLabel">a dot separated label such as "mood.greed.npc" </param>
        /// <param name="aValue">a double value to be assigned with the label</param>
        /// <returns>current number of values listed with this label</returns>
        public int Add(PropertyLabel aLabel, double aValue)
        {
            if (mPropertyMap.ContainsKey(aLabel.Label))
            {
                mPropertyMap[aLabel.Label].Add(aValue);
            }
            else
            {
                List<double> dlist = new List<double>();
                dlist.Add(aValue);
                mPropertyMap.Add(aLabel.Label, dlist);
                return 1;
            }
            return mPropertyMap[aLabel.Label].Count;
        }

        /// <summary>
        /// for a certain label, this reduces all of the values down to one value
        /// </summary>
        /// <param name="aLabel"></param>
        /// <returns></returns>
        public double Collapse(PropertyLabel aLabel)
        {
            if (mPropertyMap.ContainsKey(aLabel.Label) == false)
            {
                return 0.0;
            }
            double d = 0.0;
            List<double> vlist = mPropertyMap[aLabel.Label];
            foreach (double v in vlist)
            {
                d += v;
            }
            
            // collapse all values down to one
            vlist.Clear();
            vlist.Add(d);
            mPropertyMap[aLabel.Label] = vlist;

            return d;
        }

        /// <summary>
        /// given a label this sums up all the values for that label
        /// given a value handler, specialized logic can determine a value
        /// </summary>
        /// <param name="aLabel"></param>
        /// <param name="aDelagate"></param>
        /// <returns></returns>
        public double ComputeForLabel(PropertyLabel aLabel, HandleValueList aDelagate)
        { 
            Dictionary<string, List<double>>.Enumerator it = 
                mPropertyMap.GetEnumerator();

            double d = 0.0;

            while (it.MoveNext())
            {
                if (aLabel.DoesMatch(it.Current.Key))
                {
                    if (aDelagate == null)
                    {
                        d = it.Current.Value.Sum();
                    }
                    else
                    {
                        d = aDelagate(it.Current.Key, it.Current.Value);
                    }
                }
            }

            return d;
        }

        internal bool Set(string aLabel, ulong aValue)
        {
            try
            {
                PropertyLabel label = new PropertyLabel(aLabel);
                double val = (double)aValue;

                if (mPropertyMap.ContainsKey(label.Label))
                {
                    mPropertyMap[label.Label].Clear();
                    mPropertyMap[label.Label].Add(val);
                }
                else
                {
                    List<double> dlist = new List<double>();
                    dlist.Add(val);
                    mPropertyMap.Add(label.Label, dlist);
                }
                return true;
            }
            catch (System.Exception ex)
            {
                util.Monitoring.Logging.Exception(ex);
                return false;
            }
        }
    }
}
