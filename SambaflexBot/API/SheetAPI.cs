using SambaflexBot.Model;
using System;
using System.Globalization;
using Telegram.Bot.Types;
using static SambaflexBot.Model.Enums;
using System.Linq;
using SambaflexBot.CustomErrors;

namespace SambaflexBot.API
{
    public class SheetAPI : IAnswear
    {
        public AnswearResult GetMessage(Message message, CultureInfo culture = null)
        {
            var sheet = new GoogleSheet();            

            if (message.Text.ToLowerInvariant().Equals("total"))
            {
                return GetTotal(sheet);
            }
            else
            if (message.Text.ToLowerInvariant().Equals("encerrar"))
            {               
                return EndList(sheet, message);
            }
            else
            if (message.Text.ToLowerInvariant().Equals("imprimir") || message.Text.ToLowerInvariant().Equals("listar"))
            {                
                return PrintList(sheet);
            }
            else
            if (message.Text.ToLowerInvariant().Contains("sheet") && message.Text.Any(char.IsDigit)) //Changing current sheet Id
            {
                return ChangeSheet(sheet, message);
            }
            else //Threat as a name to be sent to a list;
            {
                return InsertNames(sheet, message);
            }
        }

        private AnswearResult InsertNames(GoogleSheet sheet, Message message)
        {
            var answearResult = new AnswearResult(AnswearResultEnum.SendMessage);

            if (sheet.ListHasEnded(Sambaflex.GetCurrentSheetId()))
            {
                answearResult.Message = string.Format("Lista encerrada! Favor enviar os nomes no privado do Rogim. :)");
                return answearResult;
            }
            else
            {
                var result = sheet.Save(message.Text);
                var sufix = (result == 1) 
                    ? string.Format("1 nome adicionado com sucesso, lista '{0}'.", Sambaflex.GetCurrentSheetName()) 
                    : string.Format("{0} nomes adicionados com sucesso, lista '{1}'.", result, Sambaflex.GetCurrentSheetName());

                var sentMessageDate = new DateTime(message.Date.Year, message.Date.Month, message.Date.Day, message.Date.Hour, message.Date.Minute, message.Date.Second);
                sentMessageDate = sentMessageDate.AddHours(DateTimeOffset.Now.Offset.Hours);

                if ((DateTime.Now - sentMessageDate).TotalMinutes >= 5)
                    answearResult.Message = string.Format("Desculpe a demora, estava \"dormindo\"! rsrsrs\n\n{0}", sufix);
                else
                    answearResult.Message = string.Format("{0} Pode mandar mais quando quiser.", sufix);

                return answearResult;
            }
        }

        private AnswearResult ChangeSheet(GoogleSheet sheet, Message message)
        {
            var split = message.Text.Split(' ');
            CurrentSheetEnum currentSheet;

            if (split[0].ToLowerInvariant().Equals("sheet"))
            {
                try
                {
                    var enumItems = Enum.GetValues(typeof(CurrentSheetEnum)).Cast<int>().Select(x => x).ToList();

                    currentSheet = (CurrentSheetEnum)int.Parse(split[1]);
                    if (!enumItems.Contains((int)currentSheet))
                        currentSheet = CurrentSheetEnum.Tardezinha;

                    if (sheet.SetSpecificLine(1, message.Chat.FirstName, ((int)currentSheet).ToString(), Sambaflex.GetConfigSheet()))
                    {
                        //Update informations for the current app running.
                        Sambaflex.RefreshConfigs();
                        return new AnswearResult(string.Format("Planilha alterada. Utilizando agora '{0}'.", currentSheet.ToString()), true);
                    }

                }
                catch
                {
                    throw new SambaflexException("O id da planilha não corresponde a um válido.");
                }
            }

            return new AnswearResult();
        }

        private AnswearResult PrintList(GoogleSheet sheet)
        {
            //This command is only for admins from system.
            return new AnswearResult(AnswearResultEnum.SendMessage, sheet.PrintNames(Sambaflex.GetCurrentSheetId()), true);
        }

        private AnswearResult EndList(GoogleSheet sheet, Message message)
        {
            var answearResult = new AnswearResult(AnswearResultEnum.SendMessage);

            if (sheet.EndList(message.Chat.FirstName))
                answearResult.Message = string.Format("Lista encerrada com sucesso!");
            else
                answearResult.Message = string.Empty;

            return answearResult;
        }

        private AnswearResult GetTotal(GoogleSheet sheet)
        {
            var total = sheet.GetTotal(Sambaflex.GetCurrentSheetId());

            return new AnswearResult()
            {
                Message = string.Format("Total de nomes na lista '{0}' : {1}.", Sambaflex.GetCurrentSheetName(), total.ToString()),
                Result = AnswearResultEnum.SendMessage
            };
        }
    }
}