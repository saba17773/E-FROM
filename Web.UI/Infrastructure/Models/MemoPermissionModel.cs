using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public class MemoPermissionModel
    {
        public static string VIEW_MEMO { get => "View Transactions"; }
        public static string ADD_MEMO { get => "Add Memo"; }
        public static string GetPermissionText(string permissionKey)
        {
            foreach (var item in typeof(MemoPermissionModel).GetProperties())
            {
                if (item.Name == permissionKey)
                    return item.GetValue(typeof(MemoPermissionModel)).ToString();
            }

            return "";
        }
    }
}
