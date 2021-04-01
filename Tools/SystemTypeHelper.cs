using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public static class SystemTypeHelper
    {
        /// <summary>
        /// 将对象转换成 字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Dictionary<string, string> Class4Dictionary<T>(T model)
        {
            Type t = model.GetType();
            PropertyInfo[] PropertyList = t.GetProperties();
            Dictionary<string, string> dir = new Dictionary<string, string>();
            foreach (PropertyInfo item in PropertyList)
            {
                string name = item.Name;
                object value = item.GetValue(model, null);
                dir.Add(name, value.ToString());
            }

            return dir;
        }

        /// <summary>
        /// 类名不同 属性相同 赋值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="W"></typeparam>
        /// <param name="modelT"></param>
        /// <param name="modelW"></param>
        public static void ClassT4ClassW<T, W>(T modelT, W modelW)
        {
            var t = modelT.GetType();
            var w = modelW.GetType();
            PropertyInfo[] Property_T = t.GetProperties();
            PropertyInfo[] Property_W = w.GetProperties();
            foreach (var item in Property_T)
            {
                var value = item.GetValue(modelT);
                foreach (var newItem in Property_W)
                {
                    if (item.Name == newItem.Name)
                    {
                        newItem.SetValue(modelW, value, null);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listLong"></param>
        /// <returns></returns>
        public static List<string> long4string(List<long> listLong)
        {
            List<string> listString = new List<string>();
            foreach (var item in listLong)
            {
                listString.Add(item.ToString());
            }

            return listString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listLong"></param>
        /// <returns></returns>
        public static List<long> string4long(List<string> listString)
        {
            List<long> listLong = new List<long>();
            foreach (var item in listString)
            {
                listLong.Add(long.Parse(item));
            }

            return listLong;
        }

    }
}
