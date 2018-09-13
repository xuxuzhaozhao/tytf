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

        public static Tuple<IEnumerable<Vm_MenuClass>, IEnumerable<MenuTypeClass>> ExcuteMultipleReader(string sql, object obj = null)
        {
            IEnumerable<Vm_MenuClass> menuList = null;
            IEnumerable<MenuTypeClass> typeList = null;
            using (var con = new MySqlConnection(XConfig.MySqlConnectionString))
            {
                using (var multi = con.QueryMultiple(sql, obj))
                {
                    if (!multi.IsConsumed)
                    {
                        menuList = multi.Read<Vm_MenuClass>();
                        typeList = multi.Read<MenuTypeClass>();
                    }
                }
            }
            return new Tuple<IEnumerable<Vm_MenuClass>, IEnumerable<MenuTypeClass>>(menuList, typeList);
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
    public class Vm_MenuClass
    {
        public int Id { get; set; }
        public string MenuTypeName { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string RootUrl { get; set; }
        public string ICon { get; set; }
    }
    public class MenuTypeClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Sort { get; set; }
    }
}
