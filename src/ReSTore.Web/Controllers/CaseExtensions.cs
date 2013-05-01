using System;
using System.Text;

namespace ReSTore.Web.Controllers
{
    public enum Case
    {
        PascalCase,
        CamelCase
    }

    public static class CaseExtensions
    {
        /// <summary>
        /// Converts the phrase to specified convention.
        /// </summary>
        /// <param name="phrase"></param>
        /// <param name="cases">The cases.</param>
        /// <returns>string</returns>
        public static string ToCase(this string phrase, Case cases)
        {
            string[] splittedPhrase = phrase.Split(' ', '-', '.');
            var sb = new StringBuilder();

            if (cases == Case.CamelCase)
            {
                
                sb.Append(char.ToLower(splittedPhrase[0][0]));
                sb.Append(splittedPhrase[0].Substring(1));
                splittedPhrase[0] = string.Empty;
            }
            else if (cases == Case.PascalCase)
                sb = new StringBuilder();

            foreach (var s in splittedPhrase)
            {
                char[] splittedPhraseChars = s.ToCharArray();
                if (splittedPhraseChars.Length > 0)
                {
                    splittedPhraseChars[0] = ((new String(splittedPhraseChars[0], 1)).ToUpper().ToCharArray())[0];
                }
                sb.Append(new String(splittedPhraseChars));
            }
            return sb.ToString();
        }

    }
}