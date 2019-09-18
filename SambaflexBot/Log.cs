using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SambaflexBot
{
    public static class Log
    {
        public static string FileName { get { return "Log.txt"; } set { } }

        public static void addLog(string input, params string[] format)
        {
            addLog(string.Format(input, format), false);
        }

        public static void addLog(string input, bool isToSaveOnDisk = true)
        {
            var newList = new List<string>() { string.Format("[{0}] > {1}", DateTime.Now.ToString("dd'/'MM'/'yyyy - hh:mm:ss") ,input) };

            Save(newList, isToSaveOnDisk);
        }

        public static void addLog(Exception ex)
        {
            var newList = new List<string>() { string.Format("[{0}] > ERRO NA EXECUÇÃO:\n{1}" , DateTime.Now.ToString("dd'/'MM'/'yyyy - hh:mm:ss") , ex.Message) };
            var innerException = ex.InnerException;

            while (innerException != null)
            {
                newList.Add(string.Format("> {0}", innerException.Message));

                innerException = innerException.InnerException;
            }

            Save(newList, true);

        }

        private static void Save(List<string> newList, bool isToSaveOnDisk)
        {
            if (File.Exists(FileName))
            {
                var listFull = File.ReadAllLines(FileName).ToList();
                listFull.AddRange(newList);

                if (isToSaveOnDisk)
                    File.WriteAllLines(FileName, listFull);

            }
            else            
            if (isToSaveOnDisk)
                File.WriteAllLines(FileName, newList);
            

            newList.ForEach(l => Console.WriteLine(l));
        }
    }
}
