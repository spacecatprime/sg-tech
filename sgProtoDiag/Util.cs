using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace sgProtoDiag
{
    class Util
    {
        public static bool CopyControl(System.Windows.Forms.Control aDst, System.Windows.Forms.Control aSrc)
        {
            aDst.Margin = aSrc.Margin;
            aDst.Location = aSrc.Location;
            aDst.Size = aSrc.Size;
            aDst.TabStop = aSrc.TabStop;
            aDst.TabIndex = aSrc.TabIndex;
            aDst.Padding = aSrc.Padding;
            aDst.Enabled = aSrc.Enabled;
            aDst.Dock = aSrc.Dock;
            aDst.BackgroundImageLayout = aSrc.BackgroundImageLayout;
            aDst.BackgroundImage = aSrc.BackgroundImage;
            aDst.BackColor = aSrc.BackColor;
            return true;
        }

        public static bool SafeDictionaryAdd<TKey, TVal>(Dictionary<TKey, TVal> aDictionary, TKey aKey, TVal aVal)
        {
            if (aDictionary.ContainsKey(aKey))
            {
                aDictionary[aKey] = aVal;
                return false;
            }
            aDictionary.Add(aKey, aVal);
            return true;
        }
    }

    namespace WindowsApplication
    {
        static class Program
        {
            [DllImport("kernel32.dll",
                EntryPoint = "GetStdHandle",
                SetLastError = true,
                CharSet = CharSet.Auto,
                CallingConvention = CallingConvention.StdCall)]
            private static extern IntPtr GetStdHandle(int nStdHandle);

            [DllImport("kernel32.dll",
                EntryPoint = "AllocConsole",
                SetLastError = true,
                CharSet = CharSet.Auto,
                CallingConvention = CallingConvention.StdCall)]
            private static extern int AllocConsole();

            private const int STD_OUTPUT_HANDLE = -11;
            private const int MY_CODE_PAGE = 437;

            public static void StartConsole()
            {
                AllocConsole();
                IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
                SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
                FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
                Encoding encoding = System.Text.Encoding.GetEncoding(MY_CODE_PAGE);
                StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
                standardOutput.AutoFlush = true;
                Console.SetOut(standardOutput);
            }
        }
    }
}
