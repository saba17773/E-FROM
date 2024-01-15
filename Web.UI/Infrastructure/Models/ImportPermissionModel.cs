using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.UI.Infrastructure.Models
{
    public class ImportPermissionModel
    {
        public static string VIEW_IMPORT { get => "View Import"; }
        public static string VIEW_IMPORT_TEMPLATE_SUPPLIER { get => "View Import Supplier"; }
        public static string VIEW_IMPORT_TEMPLATE_SAVING { get => "View Import Saving"; }
        public static string VIEW_IMPORT_KPI_RM_COST { get => "Import RM Cost"; }
        public static string VIEW_IMPORT_KPI_DELIVERY { get => "Import KPI Delivery"; }
        public static string VIEW_IMPORT_KPI_TURNOVER { get => "Import KPI Turn Over"; }
        public static string VIEW_IMPORT_KPI_SHORT_SUPPLY { get => "Import KPI Short Supply"; }
        public static string VIEW_IMPORT_KPI_AUDIT { get => "Import KPI Audit"; }
        public static string VIEW_IMPORT_KPI_STOCK_ACC { get => "Import KPI Stock Acc"; }
        public static string VIEW_IMPORT_MARKET_KPI { get => "Import Market KPI"; }
        public static string VIEW_IMPORT_MARKET_FORECAST { get => "Import Market Forecast"; }
        public static string VIEW_IMPORT_MARKET_SUBGROUP { get => "Import Market Subgroup"; }
        public static string VIEW_IMPORT_MARKET_FORECAST_ITEM { get => "Import Market Forecast Item"; }
        public static string VIEW_IMPORT_MARKET_FORECAST_SUBGROUP { get => "Import Market Forecast Subgroup"; }
        public static string VIEW_IMPORT_REFERENCE_SUBGROUP { get => "View Import Reference Subgroup"; }

        public static string GetPermissionText(string permissionKey)
        {
            foreach (var item in typeof(ImportPermissionModel).GetProperties())
            {
                if (item.Name == permissionKey)
                    return item.GetValue(typeof(ImportPermissionModel)).ToString();
            }

            return "";
        }
    }
}
