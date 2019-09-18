using SambaflexBot.CustomErrors;
using SambaflexBot.Model;
using System;
using System.Globalization;
using Telegram.Bot.Types;

namespace SambaflexBot.API
{
    public class CalendarAPI : IAnswear
    {
        public AnswearResult GetMessage(Message message, CultureInfo culture = null)
        {
            var gCalendar = new GoogleCalendar();
            if (gCalendar.WasWellConnected)
            {
                try
                {
                    return new AnswearResult()
                    {
                        Message = gCalendar.PrintEvents(message.Text),
                        Result = Enums.AnswearResultEnum.SendMessage
                    };
                }
                catch (Exception ex)
                {
                    Log.addLog(ex);
                    throw new SambaflexException("Parece que as datas estão estranhas... Desculpe, mas não entendi.");                    
                }
            }
            else
                throw new SambaflexException("Erro na conexão com o calendário.");
        }
    }
}