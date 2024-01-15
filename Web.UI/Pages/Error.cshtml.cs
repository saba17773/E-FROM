using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace Web.UI.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorModel : PageModel
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet(int statusCode)
        {
            ErrorCode = statusCode;
            ErrorMessage = ReasonPhrases.GetReasonPhrase(statusCode);
        }
    }
}
