using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SambaflexBot.Extensions
{
    public static class DateUtils
    {
        public static int MonthStringToInt(string month)
        {
            var strMonths = new List<string>() { "janeiro", "fevereiro", "março", "abril", "maio", "junho", "julho", "agosto", "setembro", "outubro", "novembro", "dezembro" };

            return strMonths.IndexOf(month) + 1;
        }
    }
}
