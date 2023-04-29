using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG_2023
{
    public static class ConsoleExtensions
    {
        const int STD_INPUT_HANDLE = -10;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CancelIoEx(IntPtr handle, IntPtr lpOverlapped);


        public static void CancelInput()
        {
            var handle = GetStdHandle(STD_INPUT_HANDLE);
            CancelIoEx(handle, IntPtr.Zero);
        }
        public static void CancelInput(int ms)
        {
            Task.Delay(ms).ContinueWith(_ =>
            {
                var handle = GetStdHandle(STD_INPUT_HANDLE);
                CancelIoEx(handle, IntPtr.Zero);
            });
        }

    }
}
