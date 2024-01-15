using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Interfaces
{
    public interface IHelperService
    {
        object GetPropertyValue(object src, string propName);
        string Base64Encode(string plainText);
        string Base64Decode(string base64EncodedData);
        string Slugify(string text);
        string RemoveAccent(string text);
    }
}
