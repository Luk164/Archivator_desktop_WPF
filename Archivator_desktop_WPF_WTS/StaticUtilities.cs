using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Archivator_desktop_WPF_WTS
{
    public static class StaticUtilities
    {
        public const double MAX_FILE_SIZE = 2.5e+7;

        public const string DEFAULT_CONNECTION_STRING =
            @"Server=(localdb)\\mssqllocaldb;Database=Archivator_autoMigTest;Trusted_Connection=True;";

        public const string CONN_STRING_KEY = "DbConnString";

        public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> items)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (items == null) throw new ArgumentNullException(nameof(items));

            if (list is List<T> asList)
            {
                asList.AddRange(items);
            }
            else
            {
                foreach (var item in items)
                {
                    list.Add(item);
                }
            }
        }

        //public static string GetConnectionString()
        //{
        //    return DEFAULT_CONNECTION_STRING;

        //    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
        //    builder.ConnectionString =
        //        "Data Source=(LocalDB)\\MSSQLLocalDB;Database=Archivator_LocalTest;AttachDbFilename=D:\\User_files\\Projekty\\C#\\Archivator_desktop_WPF_WTS\\Archivator_desktop_WPF_WTS\\LocalDb.mdf;Integrated Security=True;Trusted_Connection=True";
        //    builder.AttachDBFilename = Directory.GetCurrentDirectory() + "\\LocalDb.mdf";
            
        //    return builder.ConnectionString;
        //}

        public static bool TestConnection(this DbContext context)
        {
            DbConnection conn = context.Database.GetDbConnection();

            try
            {
                conn.Open();   // Check the database connection

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
