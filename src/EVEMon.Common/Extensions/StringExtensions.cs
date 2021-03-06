﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using EVEMon.Common.Constants;

namespace EVEMon.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Removes the project local path from the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string RemoveProjectLocalPath(this string text)
            => Regex.Replace(text, @"[a-zA-Z]+:\\.*\\(?=EVEMon)",
                String.Empty, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        ///Converts a string that has been HTML-encoded for HTTP transmission into a decoded string.        
        /// </summary>
        /// <param name="text">The string to decode.</param>
        /// <returns></returns>
        public static string HtmlDecode(this string text)
        {
            while (text != HttpUtility.HtmlDecode(text))
            {
                text = HttpUtility.HtmlDecode(text);
            }
            return text;
        }

        /// <summary>
        /// Determines whether the string is of a valid email format.
        /// </summary>
        /// <param name="strIn">The string.</param>
        /// <returns>
        /// 	<c>true</c> if the string is of a valid email format; otherwise, <c>false</c>.
        /// </returns>
        // Return true if strIn is in valid e-mail format
        public static bool IsValidEmail(this string strIn) => Regex.IsMatch(strIn,
            @"^(?("")(""[^""]+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");

        /// <summary>
        /// Converts new lines to break lines.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">text</exception>
        public static string NewLinesToBreakLines(this string text)
        {
            text.ThrowIfNull(nameof(text));

            if (String.IsNullOrWhiteSpace(text))
                return text;

            text = text.Replace(@"\r\n", Environment.NewLine)
                .Replace(@"\r", Environment.NewLine)
                .Replace(@"\n", Environment.NewLine);

            using (StringReader sr = new StringReader(text))
            using (StringWriter sw = new StringWriter(CultureConstants.InvariantCulture))
            {
                //Loop while next character exists
                while (sr.Peek() > -1)
                {
                    // Read a line from the string and writes it to an internal StringBuilder created automatically
                    sw.Write(sr.ReadLine());

                    // Adds an HTML break line as long as it's not the last line
                    if (sr.Peek() > -1)
                        sw.Write("<br>");
                }

                return sw.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// Decodes the unicode characters.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">text</exception>
        public static string DecodeUnicodeCharacters(this string text)
        {
            text.ThrowIfNull(nameof(text));

            return Regex.Replace(text, @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m => ((char)int.Parse(m.Groups["Value"].Value,
                    NumberStyles.HexNumber,
                    CultureConstants.InvariantCulture)).ToString(
                        CultureConstants.DefaultCulture));
        }

        /// <summary>
        /// Converts the upper to lower camel case.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">text</exception>
        public static string ConvertUpperToLowerCamelCase(this string text)
        {
            text.ThrowIfNull(nameof(text));

            return String.Concat(text.Substring(0, 1).ToLowerInvariant(), text.Substring(1, text.Length - 1));
        }

        /// <summary>
        /// Converts an upper camel case text to a sentence string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string ConvertUpperCamelCaseToString(this string text)
            => Regex.Replace(text.Trim(), "\\B([A-Z])", " $1", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// Determines whether the source contains the specified text.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="text">The text.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <returns></returns>
        public static bool Contains(this string source, string text, bool ignoreCase = false)
        {
            if (!ignoreCase)
                return source.Contains(text);

            return source.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Converts the specified string to titlecase.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">text</exception>
        public static string ToTitleCase(this string text)
        {
            text.ThrowIfNull(nameof(text));

            string[] words = text.Split(" ".ToCharArray());
            StringBuilder sb = new StringBuilder();

            foreach (string word in words)
            {
                if (String.IsNullOrEmpty(word))
                {
                    sb.Append(" ");
                    continue;
                }

                sb.Append(String.Concat(word.Substring(0, 1).ToUpperInvariant(),
                    word.Substring(1, word.Length - 1).ToLowerInvariant()));
                if (word != words.Last())
                    sb.Append(" ");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Convert an Int32 number to string with the specified decimals.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="decimals">The decimals.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public static string ToNumericString(this int number, int decimals, CultureInfo culture = null)
            => ToNumericString(Convert.ToInt64(number), decimals, culture);

        /// <summary>
        /// Convert an Single number to string with the specified decimals.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="decimals">The decimals.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public static string ToNumericString(this float number, int decimals, CultureInfo culture = null)
            => ToNumericString(Convert.ToDouble(number), decimals, culture);

        /// <summary>
        /// Convert an Decimal number to string with the specified decimals.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="decimals">The decimals.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public static string ToNumericString(this decimal number, int decimals, CultureInfo culture = null)
            => ToNumericString(Convert.ToDouble(number), decimals, culture);

        /// <summary>
        /// Convert an Int64 number to string with the specified decimals.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="decimals">The decimals.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public static string ToNumericString(this long number, int decimals, CultureInfo culture = null)
        {
            string decimalsString = String.Format(culture ?? CultureConstants.DefaultCulture, "N{0}", decimals);
            return number.ToString(decimalsString, culture ?? CultureConstants.DefaultCulture);
        }

        /// <summary>
        /// Convert an Double number to string with the specified decimals.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="decimals">The decimals.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public static string ToNumericString(this double number, int decimals, CultureInfo culture = null)
        {
            string decimalsString = String.Format(culture ?? CultureConstants.DefaultCulture, "N{0}", decimals);
            return number.ToString(decimalsString, culture ?? CultureConstants.DefaultCulture);
        }

        /// <summary>
        /// Remove line feeds and some other characters to format the string.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxLength"></param>
        /// <param name="removeNewLine"> </param>
        /// <returns></returns>
        public static string WordWrap(this string text, int maxLength, bool removeNewLine = true)
        {
            if (String.IsNullOrWhiteSpace(text))
                return String.Empty;

            text = removeNewLine
                ? text.Replace(Environment.NewLine, " ")
                : text.Replace(Environment.NewLine, " " + Environment.NewLine + " ");

            text = text.Replace(".", ". ");
            text = text.Replace(">", "> ");
            text = text.Replace("\t", " ");
            text = text.Replace(",", ", ");
            text = text.Replace(";", "; ");

            string[] words = text.Split(' ');
            List<string> lines = new List<string>();
            int currentLineLength = 0;
            string currentLine = String.Empty;
            bool inTag = false;

            foreach (string currentWord in words.Where(currentWord => currentWord.Length > 0))
            {
                if (currentWord.Substring(0, 1) == "<")
                    inTag = true;

                if (inTag)
                {
                    //Handle filenames inside html tags
                    if (currentLine.EndsWith(".", StringComparison.CurrentCulture))
                        currentLine += currentWord;
                    else
                        currentLine += currentWord + " ";

                    if (currentWord.IndexOf(">", StringComparison.CurrentCulture) > -1)
                        inTag = false;
                }
                else
                {
                    if (currentWord != Environment.NewLine && currentLine != Environment.NewLine &&
                        currentLineLength + currentWord.Length + 1 < maxLength)
                    {
                        currentLine += currentWord + " ";
                        currentLineLength += currentWord.Length + 1;
                    }
                    else
                    {
                        lines.Add(currentLine.Trim());
                        currentLine = currentWord + " ";
                        currentLineLength = currentWord.Length;
                    }
                }
            }

            if (currentLine.Length != 0)
                lines.Add(currentLine.Trim());

            string[] textLinesStr = new string[lines.Count];
            lines.CopyTo(textLinesStr, 0);

            return textLinesStr.Aggregate(String.Empty,
                (current, line) => $"{current}{line}{Environment.NewLine}");
        }
    }
}
