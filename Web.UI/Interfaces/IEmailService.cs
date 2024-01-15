using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.Infrastructure.Models;

namespace Web.UI.Interfaces
{
    public interface IEmailService
    {
        ResponseModel SendEmail(string subject, string htmlBody, List<string> mailTo, List<string> mailCc, string sender = "", string replyTo = "", List<string> files = null);
    }
}
