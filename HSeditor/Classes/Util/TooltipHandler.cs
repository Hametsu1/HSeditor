using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace HSeditor.Classes.Util
{
    public static class TooltipHandler
    {
        static List<TextBlock> tooltips = new List<TextBlock>();
        public static void SetToolTip(TextBlock tb)
        {
            if (tooltips.Contains(tb)) return; else tooltips.Add(tb);
            string[] temp = tb.Text.Split();
            string[]? colors = tb.Tag == null ? null : tb.Tag.ToString().Split();
            tb.Text = "";
            int counter = 0;
            List<string> words = new List<string>();

            for (int i = 0; i < temp.Length; i++)
            {
                string end = i == temp.Length - 1 ? "" : " ";

                // not highlighted
                if (!temp[i].Contains("*") && words.Count == 0)
                {
                    tb.Inlines.Add(new Run(temp[i] + end));
                    continue;
                }

                // Single highlighted word
                if (temp[i][0] == '*' && (temp[i][temp[i].Length - 1] == '*' || temp[i][temp[i].Length - 2] == '*'))
                {
                    tb.Inlines.Add(new Run(temp[i].Replace("*", string.Empty)[temp[i].Replace("*", string.Empty).Length - 1] == '.' ? temp[i].Replace("*", string.Empty).Remove(temp[i].Replace("*", string.Empty).Length - 1) + end : temp[i].Replace("*", string.Empty) + end)
                    {
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colors == null ? "#ad9247" : counter < colors.Length ? colors[counter] : colors[0]))
                    });
                    counter++;
                    continue;
                }


                // multiple highlighted words seperated by space
                words.Add(temp[i]);
                if ((temp[i][temp[i].Length - 1] == '*' || (i < temp[i].Length && (temp[i][temp[i].Length - 2] == '*')) && temp[i][temp[i].Length - 2] == '*'))
                {
                    string s = "";
                    words.ForEach(o => { s += o + " "; });
                    s = s.Remove(s.Length - 1);
                    s = s.Replace("*", string.Empty);

                    tb.Inlines.Add(new Run(s + end)
                    {
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colors == null ? "#ad9247" : counter < colors.Length ? colors[counter] : colors[0]))
                    });
                    counter++;
                    words = new List<string>();
                }
            }

        }
        public static void SetHyperlink(TextBlock tb, string content)
        {
            string[] temp = content.Split();
            tb.Text = "";
            for (int i = 0; i < temp.Length; i++)
            {
                string s = temp[i];
                string end = i == temp.Length - 1 ? "" : " ";
                Uri uriResult;
                bool result = Uri.TryCreate(s, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                if (!result)
                    tb.Inlines.Add(new Run(s + end));
                else
                {
                    Run run = new Run(s + end);
                    run.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3784db"));
                    run.Cursor = Cursors.Hand;
                    run.MouseEnter += delegate { /*run.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#61a5f2"));*/ run.TextDecorations = TextDecorations.Underline; };
                    run.MouseLeave += delegate { /*run.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3784db"));*/ run.TextDecorations = null; };
                    run.MouseDown += delegate { Process.Start(new ProcessStartInfo(uriResult.AbsoluteUri) { UseShellExecute = true }); };
                    tb.Inlines.Add(run);
                }
            }
        }
    }
}
