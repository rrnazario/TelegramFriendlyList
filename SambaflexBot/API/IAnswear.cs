using SambaflexBot.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace SambaflexBot.API
{
    public interface IAnswear
    {
        AnswearResult GetMessage(Message message, CultureInfo culture = null);
    }
}
