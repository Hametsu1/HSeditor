using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HSeditor.Classes.Util
{
    public static class Util
    {
        public static int FormatString(string row)
        {
            if (row == null) return 0;
            try
            {
                return Int32.Parse((row.Trim('"').Split('.'))[0]);
            }
            catch
            {
                return Int32.MaxValue;
            }
        }

        public static string Clean(this string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(ref Win32Point pt);

        public static Point GetCurrentCursorPosition(Visual relativeTo)
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return relativeTo.PointFromScreen(new Point(w32Mouse.X, w32Mouse.Y));
        }


        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }



        public static string GetFullMessage(this Exception ex)
        {
            return ex.InnerException == null
                 ? ex.Message + " --> " + new StackTrace(ex, true).GetFrame(0).GetFileLineNumber()
                 : ex.Message + " --> " + ex.InnerException.GetBaseException();
        }

        public static string GetEmbeddedResource(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"HSeditor.Resources.{filename}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static double GetScaleFactor(this Viewbox viewbox)
        {
            if (viewbox.Child == null ||
                (viewbox.Child is FrameworkElement) == false)
            {
                return double.NaN;
            }
            FrameworkElement child = viewbox.Child as FrameworkElement;
            return viewbox.ActualWidth / child.ActualWidth;
        }



        public static string MakeUpper(string str, int cut = 9)
        {
            if (str == null) return "";
            string tempstr = str.Remove(0, cut);
            string ability = "";
            for (int i = 0; i < tempstr.Length; i++)
            {
                if (Char.IsUpper(tempstr[i]))
                {
                    ability += " " + tempstr[i];
                    continue;
                }
                ability += i == 0 ? Char.ToUpper(tempstr[i]) : tempstr[i];
            }
            return ability;
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield return (T)Enumerable.Empty<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject ithChild = VisualTreeHelper.GetChild(depObj, i);
                if (ithChild == null) continue;
                if (ithChild is T t) yield return t;
                foreach (T childOfChild in FindVisualChildren<T>(ithChild)) yield return childOfChild;
            }
        }

        public static BitmapImage GetControlAsImage(UIElement element)
        {
            element.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
            element.Arrange(new Rect(new System.Windows.Point(), element.DesiredSize));

            RenderTargetBitmap rtb =
              new RenderTargetBitmap(
                (int)element.DesiredSize.Width,
                (int)element.DesiredSize.Height,
                96, 96, PixelFormats.Pbgra32);

            rtb.Render(element);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            using (var ms = new MemoryStream())
            {
                encoder.Save(ms);
                using (var bmp = new System.Drawing.Bitmap(ms))
                {
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    MemoryStream ms2 = new MemoryStream();
                    bmp.Save(ms2, ImageFormat.Bmp);
                    ms2.Seek(0, SeekOrigin.Begin);
                    bi.StreamSource = ms2;
                    bi.EndInit();
                    return bi;
                }
            }
        }

        public static SolidColorBrush ColorFromString(string s)
        {
            return new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(s));
        }



    }
}
