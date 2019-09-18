using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using SambaflexBot.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace SambaflexBot
{
    public class GoogleCalendar : GoogleSf
    {
        static string[] Scopes = { CalendarService.Scope.CalendarReadonly };        

        public GoogleCalendar() : base()
        {
            Credential = Connect();
        }

        public UserCredential Connect() 
        {
            return Connect(Scopes, "GoogleCalendar.json");
        }

        /// <summary>
        /// Obtain Google Calendar API Servie.
        /// </summary>
        /// <returns></returns>
        public override BaseClientService GetService()
        {
            return new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = Credential,                
                ApplicationName = ApplicationName
            });
        }

        public string PrintEvents(string message)
        {
            var strArray = message.ToLowerInvariant().Split(' ');
            var arrayCount = strArray.Count();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-br");
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            switch (arrayCount)
            {
                case 1:
                    {
                        return PrintEvents();
                    }
                case 2:
                    {
                        if (strArray[1].IsValidMonthName())
                        {
                            //Get the current month date range.
                            return PrintEvents(new DateTime(DateTime.Now.Year, DateUtils.MonthStringToInt(strArray[1]), 1), new DateTime(DateTime.Now.Year, DateUtils.MonthStringToInt(strArray[1]), DateTime.DaysInMonth(DateTime.Now.Year, DateUtils.MonthStringToInt(strArray[1]))));
                        }
                        else
                            //try to use with only one date.
                            return PrintEvents(DateTime.Parse(strArray[1]), DateTime.Parse(strArray[1] + " 11:59:59 PM")); //Dates could not be valids
                    }
                case 3:
                    {
                        return PrintEvents(DateTime.Parse(strArray[1]), DateTime.Parse(strArray[2])); //Dates could not be valids
                    }
                default:
                    return null;
            }
        }
        
        /// <summary>
        /// Get 
        /// </summary>
        /// <returns></returns>
        private string PrintEvents()
        {            
            return PrintEvents(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(10));            
        }

        private string PrintEvents(DateTime begin, DateTime end)
        {
            try
            {
                var service = (CalendarService)GetService();

                // Define parameters of request.
                EventsResource.ListRequest request = service.Events.List(Sambaflex.getSambaflexCalendarId()); //Sambaflex calendar
                request.TimeMin = begin;
                request.TimeMax = end;
                request.ShowDeleted = false;
                request.SingleEvents = true;
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

                var events = request.Execute();
                var returnStr = new StringBuilder();

                if (events.Items != null && events.Items.Count > 0)
                {
                    foreach (var eventItem in events.Items)
                    {
                        var when = eventItem.Start.DateTime != null ? eventItem.Start.DateTime.Value : DateTime.Parse(eventItem.Start.Date);

                        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-br");

                        returnStr.AppendLine(string.Format("{0} ({1} - {2})", eventItem.Summary, when.ToString("dd'/'MM'/'yyyy"), StrDayOfWeek(when)));
                    }
                }
                else
                    returnStr = null;

                return returnStr.ToString();
            }
            catch (Exception ex)
            {
                Log.addLog(ex);

                return null;
            }
        }

        public void ShowCalendarIds()
        {
            //Get sambaflex credentials.
            try
            {
                var service = (CalendarService)GetService();

                var calendars = service.CalendarList.List();
                var calendarlist = calendars.Execute();
                foreach (var item in calendarlist.Items)
                {
                    Console.WriteLine("{0} - {1}", item.Summary, item.Id);
                }
            }
            catch (Exception ex)
            {
                Log.addLog(ex);
            }
        }

        #region private methods
        private string StrDayOfWeek(DateTime date)
        {
            return StrDayOfWeek(date.ToString("dd'/'MM'/'yyyy"));
        }
        private string StrDayOfWeek(string date)
        {
            DateTime parsedDate;
            if (DateTime.TryParse(date, out parsedDate))
            {
                switch (parsedDate.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        return "Segunda-feira";
                    case DayOfWeek.Tuesday:
                        return "Terça-feira";
                    case DayOfWeek.Wednesday:
                        return "Quarta-feira";
                    case DayOfWeek.Friday:
                        return "Sexta-feira";
                    case DayOfWeek.Saturday:
                        return "Sábado";
                    case DayOfWeek.Sunday:
                        return "Domingo";
                    default:
                        return "";
                }
            }
            else
                return string.Empty;
        }
        #endregion
    }
}
