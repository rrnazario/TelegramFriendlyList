using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StringUtils
{
    public static class StringExtensions
    {
        /// <summary>
        /// Put the first letter of a word in UpperCase.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string Capitalize(this string word)
        {
            return (new CultureInfo("pt-br")).TextInfo.ToTitleCase(word);
        }

        /// <summary>
        /// Capitalizes every word that contains in a given sentence.
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public static string CapitalizeEachWord(this string sentence)
        {
            var result = string.Empty;

            sentence.ToLowerInvariant().Split(' ').ToList().ForEach(aWord =>
            {
                result += string.IsNullOrEmpty(result) ? aWord.Capitalize() : string.Concat(" ", aWord.Capitalize());
            });
                

            return result;
        }

        public static string RemoveInvalidChars(this string input)
        {
            if (input.Contains(":"))
            {
                var split = input.Split(':');
                return split[split.Count() - 1];
            }
            else
                return input;
        }

        public static string WellFormat(this string input)
        {
            return input.RemoveMultipleSpaces().RemoveInvalidChars().CapitalizeEachWord();
        }

        /// <summary>
        /// Add a string in many list at once.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="lists"></param>
        public static void AddInMultipleLists(this string line, params List<string>[] lists)
        {
            foreach (var list in lists)
                list.Add(line);
        }

        /// <summary>
        /// Remove multiples spaces that occours on a given string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveMultipleSpaces(this string input)
        {
            var regex = new Regex("[ ]{2,}", RegexOptions.None);
            input = input.TrimEnd().TrimStart().Trim();
            input = regex.Replace(input, " ");

            return input;
        }
    }
}
