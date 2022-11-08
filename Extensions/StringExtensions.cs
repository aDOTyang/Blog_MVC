using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Blog_MVC.Extensions
{
    public static class StringExtensions
    {
        public static string Slugify(this string phrase)
        {
            // remove all accents and make string lowercase
            string output = phrase.RemoveAccents().ToLower();

            // remove special characters 
            output = Regex.Replace(output, @"[^A-Za-z0-9\s]", "");

            // remove all additional spaces in favour of just one
            output = Regex.Replace(output, @"\s+", " ").Trim();

            // replace all spaces with hyphen
            output = Regex.Replace(output, @"\s", "-");

            //return slug
            return output;
        }
        private static string RemoveAccents(this string phrase)
        {
            if(string.IsNullOrWhiteSpace(phrase)) 
            { 
                return phrase;
            }
            // convert for unicode
            phrase = phrase.Normalize(System.Text.NormalizationForm.FormD);

            // format unicode/ascii
            char[] chars = phrase.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();

            // convert and return new phrase
            return new string(chars).Normalize(System.Text.NormalizationForm.FormC);
        }
    }
}
