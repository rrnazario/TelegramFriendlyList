using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SambaflexBot.Extensions
{
    public static class StringExtensions
    {
        public static bool IsSaudacao(this string input)
        {
            var cultureInfo = new CultureInfo("pt-br");

            //To check if bot is alive.
            return input.ToLower(cultureInfo).Equals("olá") ||
                input.ToLower(cultureInfo).Equals("ola") ||
                input.ToLower(cultureInfo).Equals("oi") || 
                input.ToLower(cultureInfo).Contains("boa tarde") || 
                input.ToLower(cultureInfo).Contains("boa noite") || 
                input.ToLower(cultureInfo).Contains("bom dia");
        }

        public static bool IsValidMonthName(this string input)
        {
            return input.ToLowerInvariant().Equals("janeiro") ||
                   input.ToLowerInvariant().Equals("fevereiro") ||
                   input.ToLowerInvariant().Equals("março") || input.ToLowerInvariant().Equals("marco") ||
                   input.ToLowerInvariant().Equals("abril") ||
                   input.ToLowerInvariant().Equals("maio") ||
                   input.ToLowerInvariant().Equals("junho") ||
                   input.ToLowerInvariant().Equals("julho") ||
                   input.ToLowerInvariant().Equals("agosto") ||
                   input.ToLowerInvariant().Equals("setembro") ||
                   input.ToLowerInvariant().Equals("outubro") ||
                   input.ToLowerInvariant().Equals("novembro") ||
                   input.ToLowerInvariant().Equals("dezembro");            
        }
    }
}
