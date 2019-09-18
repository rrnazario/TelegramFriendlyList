using SambaflexBot.Extensions;

namespace SambaflexBot.API
{
    public static class AnswearsFactory
    {
        public static IAnswear Create(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                if (message.ToLowerInvariant().Contains("agenda") || message.ToLowerInvariant().Contains("datas"))
                    return new CalendarAPI();
                if (message.IsSaudacao() || message.ToLowerInvariant().Contains("ajuda"))
                    return new ThreatmentsAPI();

                return new SheetAPI();
            }
            return null;
        }
    }
}
