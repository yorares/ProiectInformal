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
    //ExtractLicense  - eliminates eveything from the standard JSON output of the OCR Api that does not look like/belong in a Romanian license plate.
    //Note on efficiency and the 6 different strings created below. Use of StringBuilder was not possible due to the fact that it does not recognise regular expressions
        
    public static List<string> ExtractLicense(string input)
        {
            string bbox = @"[0-9]{2,4},[0-9]{2,4},[0-9]{2,4},[0-9]{2,4}";

            //removes the "header" - everything in the JSON before the first batch of read image text which is preceded by "text": "
            int first = input.IndexOf("text\""); 
            input = input.Substring(first);

            //removes the boundingbox coordinates
            input = Regex.Replace(input, bbox, "");

            //removes everything thatis not a uppercase letter or number and also the word "boundingBox"
            input = Regex.Replace(input, @"[\u003A-\u0040]", "");
            input = Regex.Replace(input, @"\bboundingBox\b", "");
            input = Regex.Replace(input, @"[^\u0030-\u005A]", "");

            //removes whitespaces
            input = Regex.Replace(input, @" ","");
            //of what remains, if it matches the license plate pattern it gets added to a list of strings and returned by the method
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

