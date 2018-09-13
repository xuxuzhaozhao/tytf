using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TYTFBackTermimalBack.DataHandle.Config
{
    public static class XCommons
    {
        /// <summary>
        /// 删除空格
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static string DelSpace(this string sql)
        {
            while (sql?.IndexOf(" ") > -1)
            {
                sql = sql?.Replace(" ", "");
            }
            return sql;
        }

        /// <summary>
        /// 去掉字符串S首尾特定字符串R
        /// </summary>
        /// <param name="s"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static string Trim(this string s, string r)
        {
            if (string.IsNullOrEmpty(s)) return s;
            while (s.StartsWith(r))
            {
                s = s.Substring(r.Length);
            }
            while (s.EndsWith(r))
            {
                s = s.Substring(0, s.Length - r.Length);
            }
            return s;
        }

        ///<summary>
        /// 获取中国国家授时中心网络服务器时间发布的当前时间
        ///</summary>
        ///<returns></returns>
        public static DateTime GetChineseDateTime()
        {
            DateTime dt = DateTime.Now;
            dt = dt.AddHours(12);
            return dt;
        }

        ///<summary>
        /// 从指定的字符串中获取整数
        ///</summary>
        ///<param name="origin">原始的字符串</param>
        ///<param name="fullMatch">是否完全匹配，若为false，则返回字符串中的第一个整数数字</param>
        ///<returns>整数数字</returns>
        private static int GetInt(string origin, bool fullMatch)
        {
            if (string.IsNullOrEmpty(origin))
            {
                return 0;
            }
            origin = origin.Trim();
            if (!fullMatch)
            {
                string pat = @"-?\d+";
                Regex reg = new Regex(pat);
                origin = reg.Match(origin.Trim()).Value;
            }
            int res = 0;
            int.TryParse(origin, out res);
            return res;
        }
    }
}
