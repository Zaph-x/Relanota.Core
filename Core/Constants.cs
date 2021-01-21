using System;
using System.Collections.Generic;
using System.Text;
using Core.StateHandler;

namespace Core
{
    public class Constants
    {
        public static string DATABASE_PATH { get; set; }
        public static string DATABASE_NAME { get; set; }
        public static string APP_NAME { get {
#if DEBUG
                return $"Relanota : {AppState.Current}";
#elif !DEBUG
                return $"Relanota";
#endif
            } }
    }
}
