using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SambaflexBot.Model
{
    public class Enums
    {
        public enum ValidateItemResultEnum
        {
            Success = 1,
            SuccessConcated = 2,
            Failure = 3
        }

        public enum AnswearResultEnum
        {
            SendMessage = 1,
            BlockSend = 2
        }

        public enum CurrentSheetEnum
        {
            Tardezinha = 1,
            Lounge = 2,
            Pubs = 3
        }
    }
}
