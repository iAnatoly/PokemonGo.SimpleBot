using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.SimpleBot.Utils
{
    class ErrorStats
    {
        private static readonly Dictionary<DateTime, Type> ErrorLog = new Dictionary<DateTime, Type>();
        public static void RegisterException(Exception ex)
        {
            if (ex == null) return;
            
            ErrorLog[DateTime.Now] = ex.GetType();
        }
        public static int GetNumberOfRecentErrors(Exception ex, int timeSpanMinutes=3)
        {
            var dateThreshold = DateTime.Now.AddMinutes(-timeSpanMinutes);
            var targetType = ex.GetType();
            int number = ErrorLog
                .Where(k => k.Key > dateThreshold && k.Value == targetType)
                .Count();
            return number;
        }
        public static void Resest()
        {
            ErrorLog.Clear();
        }
    }
}
