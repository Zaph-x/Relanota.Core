using System;
namespace Core.TextFormatters
{
    public class MarkdownFormatter : Formatter
    {
        public static MarkdownFormatter Get => Instance ?? new MarkdownFormatter();
        private static MarkdownFormatter Instance { get; set; }

        private MarkdownFormatter()
        {
            Instance = this;
        }

        private bool IsStartInsideSelf(string text, int start, int end, Formatting type)
        {
            if (start > end)
            {
                throw new IndexOutOfRangeException("Start was greater than end.");
            }
            switch (type)
            {
                case Formatting.Bold:
                    if (start - 2 < 0)
                    {

                    }
                    else if (start - 1 < 0)
                    {

                    }
                    else if (text.Substring(start,(start + 2)) == "**")
                    {

                    }
                    break;
            }
            return false;
        }

        private bool IsEndInsideSelf(string text, int end, Formatting type)
        {
            switch (type)
            {
                case Formatting.Bold:
                    if (end + 2 > text.Length)
                    {
                        return text.Substring((end - 2),end) == "**";
                    }
                    else if (end + 1 > text.Length)
                    {
                        return text.Substring((end - 2),end) == "**" ||
                                text.Substring((end - 1),(end+1)) == "**" ||
                                text.Substring(end,end+2) == "**";
                    }
                    else if (text.Substring(end,end+2) == "**")
                    {

                    }
                    break;
            }
            return false;
        }


        public override void Format(ref string text, int start, int end, Formatting type)
        {
            if (start > end)
            {
                throw new IndexOutOfRangeException("Start was greater than end.");
            }
            switch (type)
            {
                case Formatting.Bold:
                    text = text.Insert(start, "**");
                    text = text.Insert(end + 2, "**");
                    break;
                case Formatting.Italics:
                    text = text.Insert(start, "*");
                    text = text.Insert(end + 1, "*");
                    break;
                case Formatting.Strikethrough:
                    text = text.Insert(start, "~~");
                    text = text.Insert(end + 2, "~~");
                    break;

                default:
                    throw new NotImplementedException($"The formatting {nameof(type)} is not implemeted in this formatter.");
            }
        }

    }
}