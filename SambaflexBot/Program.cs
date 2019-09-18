using SambaflexBot.API;
using SambaflexBot.CustomErrors;
using SambaflexBot.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SambaflexBot
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Log.addLog("Getting configurations...", false);
                    Sambaflex.RefreshConfigs();
                    Log.addLog("Config obtained!", false);

                    AsyncMonitor().Wait();
                }
                catch (Exception ex)
                {
                    Log.addLog(ex);
                    Thread.Sleep(10000);
                }
            }            
        }

        private async static Task AsyncMonitor()
        {
            var ptBrCulture = new CultureInfo("pt-br");
            Thread.CurrentThread.CurrentCulture = ptBrCulture;
            Thread.CurrentThread.CurrentUICulture = ptBrCulture;

            var bot = new TelegramBotClient(Sambaflex.getTelegramKey());
            Log.addLog("Connected.", false);

            var me = await bot.GetMeAsync();

            var offset = 0;

            while (true)
            {
                var updates = await bot.GetUpdatesAsync(offset);

                foreach (var update in updates)
                {
                    if (update.Message != null && !string.IsNullOrEmpty(update.Message.Text) && update.Message.Chat.Type == ChatType.Private)
                    {
                        Log.addLog(string.Format("Mensagem de {0}:\n{1}", update.Message.Chat.FirstName, update.Message.Text), false);
                        Message message;

                        try
                        {
                            var answearAPI = AnswearsFactory.Create(update.Message.Text);
                            var answear = answearAPI?.GetMessage(update.Message);

                            switch (answear.Result)
                            {
                                case Model.Enums.AnswearResultEnum.SendMessage:
                                    if (!string.IsNullOrEmpty(answear.Message))
                                    {
                                        if ((answear.IsToAdmin && Sambaflex.GetMainAdminName() == update.Message.Chat.FirstName.ToLowerInvariant())
                                           || (!answear.IsToAdmin))
                                            message = await bot.SendTextMessageAsync(update.Message.Chat.Id, answear.Message);
                                    }
                                        
                                    break;
                                case Model.Enums.AnswearResultEnum.BlockSend:
                                default:
                                    break;
                            }
                            
                        }
                        catch (SambaflexException ex)
                        {
                            message = await bot.SendTextMessageAsync(update.Message.Chat.Id, string.Format("Alguma coisa saiu errado!\n{0}", ex.Message));
                        }
                        catch (Exception ex)
                        {
                            if (!(ex is SambaflexException))
                                Log.addLog(ex);

                            continue;
                        }
                    }

                    offset = update.Id + 1;

                    await Task.Delay(1000);
                }                    
            }
        }            
    }
}
