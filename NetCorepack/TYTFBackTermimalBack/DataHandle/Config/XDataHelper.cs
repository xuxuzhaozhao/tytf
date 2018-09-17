using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace TYTFBackTermimalBack.DataHandle.Config
{
    public class XDataHelper
    {
        public static bool ExcuteNonQuery(string sql, object obj = null)
        {
            using (var con = new MySqlConnection(XConfig.MySqlConnectionString))
            {
                return con.Execute(sql, obj) > 0;
            }
        }

        public static T ExcuteScalar<T>(string sql, object obj = null)
        {
            using (var con = new MySqlConnection(XConfig.MySqlConnectionString))
            {
                return con.ExecuteScalar<T>(sql, obj);
            }
        }

        public static IEnumerable<T> ExcuteReader<T>(string sql, object obj = null)
        {
            using (var con = new MySqlConnection(XConfig.MySqlConnectionString))
            {
                return con.Query<T>(sql, obj);
            }
        }

        public static Tuple<IEnumerable<T1>, IEnumerable<T2>> ExcuteMultipleReader<T1,T2>(string sql, object obj = null)
        {
            IEnumerable<T1> result1 = null;
            IEnumerable<T2> result2 = null;
            using (var con = new MySqlConnection(XConfig.MySqlConnectionString))
            {
                using (var multi = con.QueryMultiple(sql, obj))
                {
                    if (!multi.IsConsumed)
                    {
                        result1 = multi.Read<T1>();
                        result2 = multi.Read<T2>();
                    }
                }
            }
            return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(result1, result2);
        }

        private void DapperTransaction(object param, params string[] query)
        {
            using (var con = new MySqlConnection(XConfig.MySqlConnectionString))
            {
                var tran = con.BeginTransaction();
                try
                {
                    foreach (var item in query)
                    {
                        con.Execute(item, param, tran);
                    }
                    tran.Commit();
                }
                catch(Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
        }
    }
}
