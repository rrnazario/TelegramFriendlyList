using System;
using System.Configuration;
using System.Linq;
using static SambaflexBot.Model.Enums;

namespace SambaflexBot
{
    public static class Sambaflex
    {
        private static CurrentSheetEnum currentSheet { get; set; }

        /// <summary>
        /// Obtain current configurations on google sheet configuration
        /// </summary>
        public static void RefreshConfigs()
        {
            var configItems = (new GoogleSheet()).GetItems(GetConfigSheet());

            if (configItems.Count > 0)
            {
                //First item: current sheet
                currentSheet = (CurrentSheetEnum)int.Parse(configItems[0]);
                var enumItems = Enum.GetValues(typeof(CurrentSheetEnum)).Cast<int>().Select(x => x).ToList();

                if (!enumItems.Contains((int)currentSheet))
                    currentSheet = CurrentSheetEnum.Tardezinha;
            }
            else
            {
                currentSheet = CurrentSheetEnum.Tardezinha;
            }
        }

        public static string getTelegramKey()
        {
            return ConfigurationManager.AppSettings["TelegramKey"]; 
        }

        /// <summary>
        /// Obtain the current sheet based on "CurrentSheet" AppSetting param.
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentSheetId()
        {
            switch (currentSheet)
            {
                case CurrentSheetEnum.Tardezinha:
                    return ConfigurationManager.AppSettings["TardezinhaSheetID"];
                case CurrentSheetEnum.Lounge:
                case CurrentSheetEnum.Pubs:
                    return ConfigurationManager.AppSettings["LoungeSheetID"];
                default:
                    return null;
            }   
        }

        /// <summary>
        /// Returns config sheet Id.
        /// </summary>
        /// <returns></returns>
        public static string GetConfigSheet()
        {
            return ConfigurationManager.AppSettings["ConfigSheetId"];
        }

        /// <summary>
        /// Obtain current sheet name.
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentSheetName()
        {
            switch (currentSheet)
            {
                case CurrentSheetEnum.Tardezinha:
                    return "Tardezinha";
                case CurrentSheetEnum.Lounge:
                    return "Lounge";
                case CurrentSheetEnum.Pubs:
                    return "Pubs";
                default:
                    return null;
            }                
        }

        /// <summary>
        /// Get current sheet id enum.
        /// </summary>
        /// <returns></returns>
        public static CurrentSheetEnum GetCurrentSheet()
        {
            return currentSheet;
        }

        /// <summary>
        /// Obtain default google calendar Id.
        /// </summary>
        /// <returns></returns>
        public static string getSambaflexCalendarId()
        {
            return ConfigurationManager.AppSettings["SambaflexCalendarId"];
        }

        /// <summary>
        /// Get Rogim Name (completely changed).
        /// </summary>
        /// <returns></returns>
        public static string GetMainAdminName()
        {
            return ConfigurationManager.AppSettings["AdminName"];
        }
    }
}
