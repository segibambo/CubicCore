using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cubic.Data.Utility
{
    //public static class SerializeDeserializeExtension
    //{
    //    public static string Serialize(this object o)
    //    {
    //        var sw = new StringWriter();
    //        var formatter = new LosFormatter();
    //        formatter.Serialize(sw, o);

    //        return sw.ToString();
    //    }

    //    public static object Deserialize(this string data)
    //    {
    //        if (String.IsNullOrEmpty(data))
    //            return null;

    //        var formatter = new LosFormatter();
    //        return formatter.Deserialize(data);
    //    }
    //}

    public static class  ExtensionUtility
    {

        public static string MapPath(string path)
        {
            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            return Path.Combine(path);
        }


        public static string[] ParseCommands(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new string[0];
            }

            var statements = new List<string>();
            using (var stream = File.OpenRead(filePath))
            using (var reader = new StreamReader(stream))
            {
                string statement;
                while ((statement = ReadNextStatementFromStream(reader)) != null)
                {
                    statements.Add(statement);
                }
            }

            return statements.ToArray();
        }


        /// <summary>
        /// Read the next statement from stream
        /// </summary>
        /// <param name="reader">Reader</param>
        /// <returns>String</returns>
        public static string ReadNextStatementFromStream(StreamReader reader)
        {
            var sb = new StringBuilder();
            while (true)
            {
                var lineOfText = reader.ReadLine();
                if (lineOfText == null)
                {
                    if (sb.Length > 0)
                        return sb.ToString();

                    return null;
                }

                if (lineOfText.TrimEnd().ToUpper() == "GO")
                    break;

                sb.Append(lineOfText + Environment.NewLine);
            }

            return sb.ToString();
        }
        

        public static string GetContollerName(this Controller controller)
        {
            return controller.GetContollerName();
           // return HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
        }
        public static string GetActionName(this Controller controller)
        {
            return controller.GetActionName();
        }
    }
}
