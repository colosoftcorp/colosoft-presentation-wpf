using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Windows.Documents;

namespace Colosoft.Presentation.Converters
{
    [ValueConversion(typeof(string), typeof(IEnumerable<Inline>))]
    public class FormattedTextConverter : ValueConverter
    {
        private enum InlineType
        {
            Run,
            LineBreak,
            Hyperlink,
            Bold,
            Italic,
            Underline,
        }

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string source = (string)value;

            if (string.IsNullOrEmpty(source))
            {
                return Binding.DoNothing;
            }

            List<Inline> inlines = new List<Inline>();

            char current;
            char? next;

            string[] lines = source.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                StringBuilder sb = new StringBuilder();
                Span parentSpan = new Span();

                for (int i = 0; i < line.Length; ++i)
                {
                    current = line[i];
                    next = (i + 1 < line.Length) ? line[i + 1] : (char?)null;

                    if (current == '[' && next != '[')
                    {
                        string text = sb.ToString();
                        sb = new StringBuilder();

                        i += (next == '/') ? 2 : 1;
                        current = line[i];

                        while (i < line.Length && current != ']')
                        {
                            sb.Append(current);

                            ++i;
                            if (i < line.Length)
                            {
                                current = line[i];
                            }
                        }

                        if (text.Length > 0)
                        {
                            parentSpan.Inlines.Add(text);
                        }

                        if (next == '/' && parentSpan.Parent != null)
                        {
                            parentSpan = (Span)parentSpan.Parent;
                        }
                        else
                        {
                            string[] tag = sb.ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (tag.Length > 0)
                            {
                                InlineType inlineType = this.GetInlineType(tag[0].TrimEnd('/'));
                                if (inlineType == InlineType.LineBreak)
                                {
                                    parentSpan.Inlines.Add(new LineBreak());
                                }
                                else if (inlineType != InlineType.Run)
                                {
                                    string tagParam = (tag.Length > 1) ? tag[1] : null;

                                    Span newParentSpan = this.CreateSpan(inlineType, tagParam);
                                    parentSpan.Inlines.Add(newParentSpan);
                                    parentSpan = newParentSpan;
                                }
                            }
                        }

                        sb = new StringBuilder();
                    }
                    else
                    {
                        if (current == '[' && next == '[')
                        {
                            ++i;
                        }

                        sb.Append(current);
                    }
                }

                if (sb.Length > 0)
                {
                    parentSpan.Inlines.Add(sb.ToString());
                }

                inlines.Add(parentSpan);
                inlines.Add(new LineBreak());
            }

            return inlines.ToArray();
        }

        private InlineType GetInlineType(string type)
        {
            switch (type)
            {
                case "b":
                    return InlineType.Bold;
                case "i":
                    return InlineType.Italic;
                case "u":
                    return InlineType.Underline;
                case "h":
                    return InlineType.Hyperlink;
                case "nl":
                    return InlineType.LineBreak;
                default:
                    return InlineType.Run;
            }
        }

        private Span CreateSpan(InlineType inlineType, string param)
        {
            Span span = null;

            switch (inlineType)
            {
                case InlineType.Hyperlink:
                    Hyperlink link = new Hyperlink();

                    Uri uri;
                    if (Uri.TryCreate(param, UriKind.Absolute, out uri))
                    {
                        link.NavigateUri = uri;
                    }

                    span = link;
                    break;
                case InlineType.Bold:
                    span = new Bold();
                    break;
                case InlineType.Italic:
                    span = new Italic();
                    break;
                case InlineType.Underline:
                    span = new Underline();
                    break;
                default:
                    span = new Span();
                    break;
            }

            return span;
        }
    }
}
