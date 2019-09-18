using static SambaflexBot.Model.Enums;

namespace SambaflexBot.Model
{
    public class AnswearResult
    {
        public AnswearResultEnum Result { get; set; }
        public string Message { get; set; }
        public bool IsToAdmin { get; set; }

        /// <summary>
        /// Create a answear result structure.
        /// </summary>
        /// <param name="isToAdmin">Send message only for system admins</param>
        public AnswearResult(bool isToAdmin = false)
        {
            IsToAdmin = isToAdmin;
            Result = AnswearResultEnum.SendMessage;
        }

        /// <summary>
        /// Create a answear result structure.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="isToAdmin">Send message only for system admins</param>
        public AnswearResult(string message, bool isToAdmin = false)
        {
            Message = message;
            IsToAdmin = isToAdmin;
            Result = AnswearResultEnum.SendMessage;
        }

        /// <summary>
        /// Create a answear result structure.
        /// </summary>
        /// <param name="answearResult"></param>
        /// <param name="isToAdmin">Send message only for system admins</param>
        public AnswearResult(AnswearResultEnum answearResult, bool isToAdmin = false)
        {
            Result = answearResult;
            IsToAdmin = isToAdmin;
            Result = AnswearResultEnum.SendMessage;
        }


        /// <summary>
        /// Create a answear result structure.
        /// </summary>
        /// <param name="answearResult"></param>
        /// <param name="message"></param>
        /// <param name="isToAdmin">Send message only for system admins</param>
        public AnswearResult(AnswearResultEnum answearResult, string message, bool isToAdmin = false)
        {
            Result = answearResult;
            Message = message;
            IsToAdmin = isToAdmin;
            Result = AnswearResultEnum.SendMessage;
        }

        
    }
}
