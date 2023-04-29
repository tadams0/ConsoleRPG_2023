using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace CustomConsoleColors
{
    /// <summary>
    /// Defines a class that can change the color scheme of the <see cref="Console"/>.
    /// </summary>
    public static class ConsoleColorHandler
    {
        [StructLayout(LayoutKind.Sequential)]
        struct CONSOLE_SCREEN_BUFFER_INFO_EX
        {
            public uint cbSize;
            public COORD dwSize;
            public COORD dwCursorPosition;
            public ushort wAttributes;
            public SMALL_RECT srWindow;
            public COORD dwMaximumWindowSize;

            public ushort wPopupAttributes;
            public bool bFullscreenSupported;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public COLORREF[] ColorTable;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct COORD
        {
            public short X;
            public short Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct SMALL_RECT
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct COLORREF
        {
            public uint ColorDWORD;

            public COLORREF(int r, int g, int b)
                : this(Color.FromArgb(r, g, b)) { }
            public COLORREF(Color color)
            {
                ColorDWORD = (uint)color.R
                             + (((uint)color.G) << 8)
                             + (((uint)color.B) << 16);
            }

            public Color GetColor()
            {
                return Color.FromArgb((int)(0x000000FFU & ColorDWORD),
                                      (int)(0x0000FF00U & ColorDWORD) >> 8,
                                      (int)(0x00FF0000U & ColorDWORD) >> 16);
            }

            public void SetColor(Color color)
            {
                ColorDWORD = (uint)color.R
                             + (((uint)color.G) << 8)
                             + (((uint)color.B) << 16);
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleMode(SafeFileHandle hConsoleHandle, int mode);
        
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleMode(SafeFileHandle handle, out int mode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern SafeFileHandle GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleScreenBufferInfoEx(SafeFileHandle hConsoleOutput,
                                                                      ref CONSOLE_SCREEN_BUFFER_INFO_EX ConsoleScreenBufferInfo);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleScreenBufferInfoEx(SafeFileHandle ConsoleOutput,
                                                                      ref CONSOLE_SCREEN_BUFFER_INFO_EX ConsoleScreenBufferInfoEx);

        private static SafeFileHandle handle;

        /// <summary>
        /// Gets the console buffer info.
        /// </summary>
        private static CONSOLE_SCREEN_BUFFER_INFO_EX GetConsoleBufferInfo()
        {
            if (handle == null)
            {
                handle = GetStdHandle(-11);
            }

            if (handle.IsInvalid)
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            CONSOLE_SCREEN_BUFFER_INFO_EX info = new CONSOLE_SCREEN_BUFFER_INFO_EX();

            info.cbSize = (uint)Marshal.SizeOf(info);
            if (!GetConsoleScreenBufferInfoEx(handle, ref info))
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());// <- this gets thrown
                // System.ArgumentException: 'Value does not fall within the expected range.'
            }

            return info;
        }

        public static void SetConsoleTo256ColorMode()
        {
            if (handle == null)
            {
                handle = GetStdHandle(-11);
            }

            int mode;
            GetConsoleMode(handle, out mode);
            SetConsoleMode(handle, mode | 0x4);
        }

        /// <summary>
        /// Replaces the given <see cref="ConsoleColor"/> with the new given color.
        /// </summary>
        /// <param name="colorToReplace"></param>
        /// <param name="newColor"></param>
        public static void ReplaceColor(ConsoleColor colorToReplace, Color newColor)
        {
            if (handle == null)
            {
                handle = GetStdHandle(-11);
            }

            CONSOLE_SCREEN_BUFFER_INFO_EX info = GetConsoleBufferInfo();

            info.ColorTable[(int)colorToReplace] = new COLORREF(newColor);

            SetInfo(handle, ref info);
            //handle.Dispose();
        }

        private static void SetInfo(SafeFileHandle handle, ref CONSOLE_SCREEN_BUFFER_INFO_EX info)
        {
            //There's a bug where the get/set detects the lower right bounds of the window inclusively while the Get call does it exclusively. 
            info.srWindow.Right += 1;
            info.srWindow.Bottom += 1;

            SetConsoleScreenBufferInfoEx(handle, ref info);
        }

        /// <summary>
        /// Replaces all colors within the console with the new given color scheme.
        /// </summary>
        /// <param name="newScheme"></param>
        public static void ReplaceColorScheme(ConsoleColorScheme newScheme)
        {
            if (handle == null)
            {
                handle = GetStdHandle(-11);
            }

            CONSOLE_SCREEN_BUFFER_INFO_EX info = GetConsoleBufferInfo();

            Color[] newColors = newScheme.Colors;
            for (int i = 0; i < newColors.Length; i++)
            {
                info.ColorTable[i] = new COLORREF(newColors[i]);
            }

            SetInfo(handle, ref info);

            //handle.Dispose();
        }

        /// <summary>
        /// Gets the current active color scheme in the console based on the kernel32 return.
        /// </summary>
        /// <returns>Returns a newly generated <see cref="ConsoleColorScheme"/> instance based off what the console is actively using.</returns>
        public static ConsoleColorScheme GetActiveColorScheme()
        {
            if (handle == null)
            {
                handle = GetStdHandle(-11);
            }

            CONSOLE_SCREEN_BUFFER_INFO_EX info = GetConsoleBufferInfo();
            Color[] existingColors = new Color[16];
            for (int i = 0; i < existingColors.Length; i++)
            {
                existingColors[i] = info.ColorTable[i].GetColor();
            }
            
            //handle.Dispose();

            return new ConsoleColorScheme(existingColors);
        }

        /// <summary>
        /// Method used to debug the current list of colors.
        /// </summary>
        public static void WriteAllDefaultColors()
        {
            foreach (var val in Enum.GetValues(typeof(ConsoleColor)))
            {
                ConsoleColor color = (ConsoleColor)val;
                Console.ForegroundColor = color;
                if (color == ConsoleColor.Black)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                Console.WriteLine(val.ToString());
            }
        }

        public static void WriteAllDrawingColors()
        {
            foreach (KeyValuePair<string, Color> color in GetAllColorsDictionary())
            {
                string colorSequence = VirtualConsoleSequenceBuilder.GetColorForegroundSequence(color.Value);
                Console.Write(colorSequence + "|" + color.Key + "|");
            }
        }

        private static Dictionary<string, Color> GetAllColorsDictionary()
        {
            Dictionary<string, Color> colors = new Dictionary<string, Color>();

            PropertyInfo[] properties = typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public);

            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType == typeof(Color))
                {
                    string name = property.Name;
                    Color color = (Color)property.GetValue(null);
                    colors.Add(name, color);
                }
            }

            return colors;
        }

        /// <summary>
        /// Gets all System.Drawing colors.
        /// </summary>
        /// <returns></returns>
        public static Color[] GetAllColors()
        {
            PropertyInfo[] colorProperties = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static);

            var colors = new List<Color>();
            foreach (PropertyInfo property in colorProperties)
            {
                if (property.PropertyType == typeof(Color))
                {
                    colors.Add((Color)property.GetValue(null, null));
                }
            }

            return colors.ToArray();
        }

        /// <summary>
        /// Method used to debug the active list of colors.
        /// </summary>
        public static void WriteAllActiveColors()
        {
            if (handle == null)
            {
                handle = GetStdHandle(-11);
            }

            CONSOLE_SCREEN_BUFFER_INFO_EX info = GetConsoleBufferInfo();

            for (int i = 0; i < info.ColorTable.Length; i++)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = (ConsoleColor)i;
                if (Console.ForegroundColor == Console.BackgroundColor)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                }

                Console.WriteLine(info.ColorTable[i].GetColor().ToString());

            }
        }

    }

}