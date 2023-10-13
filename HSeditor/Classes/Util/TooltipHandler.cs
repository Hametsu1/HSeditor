using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace HSeditor.Classes.Util
{
    public static class TooltipHandler
    {
        private static readonly char highlightChar = '*';
        static List<TextBlock> tooltips = new List<TextBlock>();
        public static void SetToolTip(TextBlock tb)
        {
            if (tooltips.Contains(tb)) return; else tooltips.Add(tb);
            string[] temp = tb.Text.Split();
            tb.Text = "";
            List<string> seperatedWords = new List<string>();
            List<string> colors = tb.Tag == null ? new List<string> { "#ad9247" } : tb.Tag.ToString().Split().ToList();

            int counter = 0;
            foreach (string word in temp)
            {
                string end = counter == temp.Length - 1 ? "" : " ";
                if (word.StartsWith(highlightChar))
                {
                    // single word
                    if (word.EndsWith(highlightChar) || (word.Length >= 2 && word[word.Length - 2] == highlightChar))
                    {
                        string append = word.EndsWith(highlightChar) ? String.Empty : word[word.Length - 1].ToString();
                        string word2 = word.EndsWith(highlightChar) ? word : word.Remove(word.Length - 1);
                        tb.Inlines.Add(new Run(word2.Replace("*", string.Empty))
                        {
                            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colors.Count <= counter ? colors[colors.Count - 1] : colors[counter]))
                        });
                        tb.Inlines.Add(new Run(append + end));

                        counter++;
                        continue;
                    }

                    // multiple words start
                    seperatedWords.Add(word);
                    continue;
                }


                if (seperatedWords.Count > 0)
                {
                    // multiple words end
                    if (word.EndsWith(highlightChar) || (word.Length >= 2 && word[word.Length - 2] == highlightChar))
                    {
                        string append = word.EndsWith(highlightChar) ? String.Empty : word[word.Length - 1].ToString();
                        string words = "";
                        seperatedWords.ForEach(o => { words += o.Replace("*", String.Empty) + " "; });
                        seperatedWords.Clear();
                        string word2 = word.EndsWith(highlightChar) ? word : word.Remove(word.Length - 1);
                        tb.Inlines.Add(new Run(words + word2.Replace("*", string.Empty))
                        {
                            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colors.Count <= counter ? colors[colors.Count - 1] : colors[counter]))
                        });
                        tb.Inlines.Add(new Run(append + end));

                        counter++;
                        continue;
                    }
                    seperatedWords.Add(word);
                    continue;
                }

                tb.Inlines.Add(new Run(word + end));
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
                    tb.Inlines.Add(s == "" ? new LineBreak() : new Run(s + end));
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
