namespace Core.TextFormatters
{
    public abstract class Formatter
    {
        public string BoldSymbol {get;set;}
        public string ItalicsSymbol {get;set;}
        public string StrikethroughSymbol {get;set;}
        public string TitleSymbol {get;set;}
        private Formatter Instance {get;set;}
        public abstract void Format(ref string text, int start, int end, Formatting type);
    }
}