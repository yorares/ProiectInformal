using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stark.Models;

namespace Stark.Models
{
    public static class Support
    {
        public static List<string> ExtractLicense(string input)
        {
            string bbox = @"[0-9]{2,4},[0-9]{2,4},[0-9]{2,4},[0-9]{2,4}";
            int first = input.IndexOf("text\"");
            input = input.Substring(first);
            input = Regex.Replace(input, bbox, "");
            input = Regex.Replace(input, @"[\u003A-\u0040]", "");
            input = Regex.Replace(input, @"\bboundingBox\b", "");
            input = Regex.Replace(input, @"[^\u0030-\u005A]", "");
            input = Regex.Replace(input, @" ", string.Empty);
            input.Trim();
            string pattern = @"[A-Z]{1,2}[0-9]{2,3}[A-Z]{3}";
            RegexOptions options = RegexOptions.Multiline;
            List<string> myMatchedStrings = new List<string>();
            foreach (Match m in Regex.Matches(input, pattern, options))
            {
                myMatchedStrings.Add(m.Value);
            }
            return myMatchedStrings.Distinct().ToList();
        }
    }
}
