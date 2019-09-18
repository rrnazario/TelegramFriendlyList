using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SambaflexBot
{
    public class GoogleSf
    {
        protected UserCredential Credential { get; set; }
        protected string CurrentUser { get; set; }

        public GoogleSf()
        {

        }

        public bool WasWellConnected
        {
            get
            {
                return Credential != null;
            }
        }

        public string ApplicationName
        {
            get
            {
                return "Sambaflex Bot para lista amiga 0.1";
            }
            private set { }
        }

        public UserCredential Connect(string[] scope, string path)
        {
            try
            {
                using (var stream =
                new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    var credPath = string.Concat(path, "SambaflexCredenciais.json");

                    return GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        scope,
                        "rrnazario@gmail.com",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }
            }
            catch (Exception ex)
            {
                Log.addLog(ex);

                return null;
            }
        }

        public virtual BaseClientService GetService()
        {
            throw new NotImplementedException();
        }        
    }
}
