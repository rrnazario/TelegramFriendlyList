using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource.AppendRequest;
using StringUtils;
using static SambaflexBot.Model.Enums;
using SambaflexBot.CustomErrors;
using System.Text;
using System.Configuration;

namespace SambaflexBot
{
    public class GoogleSheet : GoogleSf
    {
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };

        public GoogleSheet() : base()
        {
            Credential = Connect();
        }        

        public UserCredential Connect()
        {
            return Connect(Scopes, "GoogleSheet.json");
        }        

        /// <summary>
        /// Obtain Google Sheet API Servie.
        /// </summary>
        /// <returns></returns>
        public override BaseClientService GetService()
        {
            return new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = Credential,
                ApplicationName = ApplicationName,
            });
        }

        /// <summary>
        /// Get how many items has in the current sheet.
        /// </summary>
        /// <returns></returns>
        public int GetTotal(string currentSheetId)
        {
            return GetItems(currentSheetId).Count;
        }

        public string PrintNames(string currentSheetId)
        {
            var values = GetItems(currentSheetId).OrderBy(s => s).ToList();
            var result = new StringBuilder();

            if (values != null && values.Count > 0)
            {
                //Console.WriteLine("Nomes:");
                foreach (var row in values)                
                    result.AppendLine(row);                
            }
            else            
                Console.WriteLine("Sem dados na planilha.");

            return result.ToString();
        }

        /// <summary>
        /// Save names into the current sheet. It guarantees that unrecognized names are not inserted.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public int Save(string message)
        {
            //Remove invalid chars
            var lista = ThreatListNames(message);

            try
            {
                return InsertNames(lista);
            }
            catch (Exception ex)
            {
                Log.addLog(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Check if the current list has ended.
        /// </summary>
        /// <returns></returns>
        public bool ListHasEnded(string currentSheetId)
        {
            var items = GetItems(currentSheetId);

            return (items.Count > 0 && items[items.Count - 1].ToLowerInvariant() == "fim");
        }

        private List<string> ThreatListNames(string message)
        {
            var arquivo = new List<string>();

            Func<List<string>, string, int> addOnList = (vet, item) =>
            {
                if (message != vet[0])
                    for (int i = 0; i <= vet.Count - 1; i++)
                    {
                        var validateResult = IsValidItem(vet[i], arquivo, i != vet.Count - 1 ? vet[i + 1] : null);

                        if (validateResult == ValidateItemResultEnum.Success || validateResult == ValidateItemResultEnum.SuccessConcated)
                        {                            
                            if (validateResult == ValidateItemResultEnum.SuccessConcated)
                            {
                                arquivo.Add(string.Concat(vet[i].WellFormat(), " ", vet[i + 1].WellFormat()));
                                i++; //jump one line, because on last step we used two of it.
                            }
                            else
                                arquivo.Add(vet[i].WellFormat());

                        }
                    }
                return 0;
            };

            //threats '/n' and ',' chars
            if (message.Contains("\n") || message.Contains(","))
            {
                var vet = message.Split('\n').Where(i => !string.IsNullOrEmpty(i)).ToList();
                addOnList(vet, message);
                

                vet = message.Split(',').Where(i => !string.IsNullOrEmpty(i)).ToList();
                addOnList(vet, message);                
            }
            else
            {
                var result = IsValidItem(message, arquivo);

                if (result == ValidateItemResultEnum.Success || result == ValidateItemResultEnum.SuccessConcated)                
                    arquivo.Add(message.WellFormat());                                                    
            }

            //Check if items just sent were already added on sheet.
            var cloneArquivo = new List<string>();
            arquivo.ForEach(a => cloneArquivo.Add(a));

            var items = GetItems(Sambaflex.GetCurrentSheetId());

            cloneArquivo.ForEach(a =>
            {
                if (items.Contains(a))
                    arquivo.Remove(a);
            });

            return arquivo;
        }
        private void addItem(List<string> list, string item)
        {
            list.Add(item.RemoveMultipleSpaces().CapitalizeEachWord());
        }

        private ValidateItemResultEnum IsValidItem(string item, List<string> list, string nextItem = null)
        {            
            var concated = false;

            //Tardezinha's list now needs of RG together of names.
            var isToValidateRGOnTardezinhaList = bool.Parse(ConfigurationManager.AppSettings["IsToValidateRGOnTardezinhaList"]);

            if (!item.Any(char.IsDigit) && Sambaflex.GetCurrentSheet() == CurrentSheetEnum.Tardezinha)
            {
                //if (string.IsNullOrEmpty(nextItem) || !nextItem.Any(char.IsDigit))
                //    throw new SambaflexException(string.Format("O nome '{0}' não foi passado com um RG.", item));
                //else
                    concated = (!string.IsNullOrEmpty(nextItem) && nextItem.Any(char.IsDigit));                    
            }                

            //if (item.ToLowerInvariant().Contains(" vip"))
            var isValid = !list.Contains(item) && !item.ToLowerInvariant().Equals("fim") && !item.Equals("/start")
                && ((isToValidateRGOnTardezinhaList && item.Any(char.IsDigit) && Sambaflex.GetCurrentSheet() == CurrentSheetEnum.Tardezinha && !string.IsNullOrEmpty(nextItem) && nextItem.Any(char.IsDigit)) || !Sambaflex.GetCurrentSheetName().ToLowerInvariant().Equals("tardezinha") || !isToValidateRGOnTardezinhaList);

            if (isValid)
                if (concated)
                    return ValidateItemResultEnum.SuccessConcated;
                else
                    return ValidateItemResultEnum.Success;
            else
                return ValidateItemResultEnum.Failure;
        }

        /// <summary>
        /// Check whether RG was passed on each name line.
        /// </summary>
        /// <param name="list"></param>
        private void CheckRGinNames(List<string> list)
        {

        }

        /// <summary>
        /// Insert names into Google Sheet.
        /// </summary>
        /// <param name="names"></param>
        private int InsertNames(List<string> names)
        {
            try
            {
                if (names.Count == 0) return 0;

                var service = (SheetsService) GetService();

                var rangeSet = "Sheet1!A:A";
                var valueRange = new ValueRange();

                var listaSet = new List<IList<object>>();

                names.ForEach(name => listaSet.Add(new List<object>() { name }));

                valueRange.Values = listaSet;

                var requestSet = service.Spreadsheets.Values.Append(valueRange, Sambaflex.GetCurrentSheetId(), rangeSet);
                requestSet.ValueInputOption = ValueInputOptionEnum.RAW;
                requestSet.InsertDataOption = InsertDataOptionEnum.OVERWRITE;

                var resp = requestSet.Execute();

                Console.WriteLine("Linhas inseridas: {0}", resp.Updates.UpdatedCells);

                return resp.Updates.UpdatedCells.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get well-formatted Items for the current sheet.
        public List<string> GetItems(string currentSheetId)
        {
            var rawItems = GetRawItems(currentSheetId);

            try
            {
                var returnList = new List<string>();
                if (rawItems != null)
                    foreach (var item in rawItems)
                        if (item.Count > 0)
                            returnList.Add((string)item[0]);

                return returnList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Obtain all items in the current sheet.
        /// </summary>
        /// <returns></returns>
        private IList<IList<object>> GetRawItems(string currentSheetId)
        {
            var service = (SheetsService)GetService();

            var range = "Sheet1!A:A";
            var request = service.Spreadsheets.Values.Get(currentSheetId, range);

            var response = request.Execute();

            return response.Values;
        }

        public bool EndList(string user)
        {
            if (user.ToLowerInvariant().Equals(Sambaflex.GetMainAdminName()))
            {
                InsertNames(new List<string>() { "fim" });
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Set an especific line from a given sheet.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="user"></param>
        /// <param name="content"></param>
        /// <param name="sheetId"></param>
        public bool SetSpecificLine(int line, string user, string content, string sheetId)
        {
            if (user.ToLowerInvariant().Equals(Sambaflex.GetMainAdminName()))
            {
                var service = (SheetsService)GetService();


                var rangeSet = string.Format("Sheet1!A{0}:A{0}", line);
                var valueRange = new ValueRange();

                var listaSet = new List<IList<object>>();

                listaSet.Add(new List<object>() { content });

                valueRange.Values = listaSet;
                var requestSet = service.Spreadsheets.Values.Update(valueRange, sheetId, rangeSet);
                requestSet.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

                var answear = requestSet.Execute();

                Console.WriteLine("Linha atualizada: A{0}", line);

                return (answear.UpdatedCells.Value > 0);                
            }

            return false;
        }
    }
}
