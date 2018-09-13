using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TYTFBackTermimalBack.DataHandle.Config
{
    public class XPagination
    {
        [JsonProperty(PropertyName = "page")]
        public int Page { get; set; } = 1;

        [JsonProperty(PropertyName = "rowsPerPage")]
        public int RowsPerPage { get; set; } = 15;

        [JsonProperty(PropertyName = "descending")]
        public bool Descending { get; set; } = true;

        [JsonProperty(PropertyName = "sortBy")]
        public string SortBy { get; set; } = "Id";

        [JsonIgnore]
        public string Sort
        {
            get
            {
                string sort = SortBy;
                sort += Descending ? " DESC" : "";
                return sort;
            }
        }


    }

    public class QueryOrderPagination : XPagination
    {
        public int? PositionId { get; set; }
        public bool? IsBuyed { get; set; }
        public DateTime? date { get; set; }
        public DateTime? date1 { get; set; }

        public string GetWhereSql()
        {
            string sql = "WHERE 1=1 AND";
            if (PositionId != null && PositionId.Value > 0)
            {
                sql += $" PositionId = {PositionId} AND";
            }
            if (IsBuyed != null)
            {
                sql += $" IsBuyed = {(IsBuyed.Value ? 1 : 0)} AND";
            }
            if (date != null)
            {
                sql += $" CreateTime >= '{date.Value.ToString("yyyy-MM-dd")}' AND";
            }
            if (date1 != null)
            {
                sql += $" CreateTime < '{date1.Value.ToString("yyyy-MM-dd")}' AND";
            }
            sql = sql.Trim("AND");

            return sql;
        }
    }

    public class QueryMenuPagination: XPagination
    {
        public int? MenuTypeId { get; set; } 
        public string MenuName { get; set; }

        public string GetWhereSql()
        {
            string sql = "WHERE 1=1 AND";
            if (MenuTypeId != null && MenuTypeId.Value > 0)
            {
                sql += $" MenuTypeId = {MenuTypeId} AND";
            }
            if (!string.IsNullOrEmpty(MenuName))
            {
                sql += $" Name LIKE '%{MenuName}%' AND";
            }

            sql = sql.Trim("AND");
            return sql;

        }
    }
}
