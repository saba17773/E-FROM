using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Infrastructure.Entities;
using Web.UI.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.IO;
using System.Text;

namespace Web.UI.Pages
{
    public class IndexModel : PageModel
    {

        //public List<SelectListItem> DropDown { get; set; }
        public string SessionInfo_Name { get; private set; }
        public IActionResult OnGet()
        {
            //List<SelectListItem> demo = new List<SelectListItem>(); 
            
            //demo.Add(new SelectListItem { Value = "1", Text = "Wattana" });
            //demo.Add(new SelectListItem { Value = "2", Text = "Worawut" });
            
            // HttpContext.Session.SetString("keyname", "1234");
            // var sessionData = HttpContext.Session.GetString("keyname");
            // var sessionOld = HttpContext.Session.GetString("user_name");
            // Console.WriteLine(sessionData);
            return Page();
        }
    }
}
