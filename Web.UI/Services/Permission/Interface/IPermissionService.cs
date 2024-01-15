using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Services.Permission.Interface
{
    public interface IPermissionService
    {
        string GetPermissionText(Type permissionClass, string permissionKey);
    }
}
