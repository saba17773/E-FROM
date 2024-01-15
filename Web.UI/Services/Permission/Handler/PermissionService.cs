using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.Services.Permission.Interface;

namespace Web.UI.Services.Permission.Handler
{
    public class PermissionService : IPermissionService
    {
        public string GetPermissionText(Type permissionClass, string permissionKey)
        {
            foreach (var item in permissionClass.GetProperties())
            {
                if (item.Name == permissionKey)
                {
                    return item.GetValue(permissionClass).ToString();
                }
            }

            return "";
        }
    }
}
