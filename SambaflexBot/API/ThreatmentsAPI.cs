using SambaflexBot.Extensions;
using SambaflexBot.Model;
using System;
using System.Globalization;
using System.Text;
using Telegram.Bot.Types;

namespace SambaflexBot.API
{
    public class ThreatmentsAPI : IAnswear
    {
        public AnswearResult GetMessage(Message message, CultureInfo culture = null)
        {
            if (message.Text.IsSaudacao())
            {
                return new AnswearResult()
                {
                    Message = string.Format("Olá {0}. Estou ligado!", message.Chat.FirstName),
                    Result = Enums.AnswearResultEnum.SendMessage
                };
            }            
            else
            if (message.Text.ToLowerInvariant().Equals("ajuda"))
            {
                var strReturn = new StringBuilder();
                strReturn.AppendFormat("Olá {0}.\n", message.Chat.FirstName);
                strReturn.AppendFormat("Eis aqui o que eu sei fazer:\n");
                strReturn.AppendFormat("DIGITE \"Oi\" (sem aspas) pra saber se estou vivo. \n");
                strReturn.AppendFormat("DIGITE \"Agenda\" (sem aspas)  pra saber da agenda do grupo. \n");
                strReturn.AppendFormat(">>> FORMAS: Agenda OU Agenda 18/02/2017 OU Agenda maio OU Agenda 18/02/2017 25/02/2017 \n");
                strReturn.AppendFormat("DIGITE \"Ajuda\" (sem aspas) pra ver essa mensagem. \n");
                strReturn.AppendFormat("Caso você queira enviar nomes pra lista '{0}', basta enviá-los separados por LINHA ou VÍRGULA.", Sambaflex.GetCurrentSheetName());

                return new AnswearResult()
                {
                    Message = strReturn.ToString(),
                    Result = Enums.AnswearResultEnum.SendMessage
                };
            }

            return null;
        }
    }
}