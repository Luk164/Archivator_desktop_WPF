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

        public const string FILE_FILTER_STRING =
            "All files (*.*)|*.*|" +
            "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|" +
            "Text Files(*.txt)|*.txt|" +
            "Office documents(*.docx;*.pptx,*.xlsx)|*.docx;*.pptx,*.xlsx|" +
            "Office 2013 documents(*.doc;*.ppt,*.xls)|*.doc;*.ppt,*.xls|" +
            "PDF Documents(*.pdf)|*.pdf";

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
