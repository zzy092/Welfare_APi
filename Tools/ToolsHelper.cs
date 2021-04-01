using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;

namespace Tools
{
    public static class ToolsHelper
    {
        #region 扩展类使实现部分更新
        /// <summary>
        /// 扩展类使实现部分更新
        /// </summary>
        /// <typeparam name="T">实体对象类型</typeparam>
        /// <param name="dbcontext">IObjectContextAdapter</param>
        /// <param name="t">实体对象</param>
        /// <returns>实体对象</returns>
        public static T SetModifiedPropertys<T>(this IObjectContextAdapter dbcontext, T t) where T : class
        {
            var stateEntry = ((IObjectContextAdapter)dbcontext).ObjectContext.
                   ObjectStateManager.GetObjectStateEntry(t);
            Type type = t.GetType();
            System.Reflection.PropertyInfo[] ps = type.GetProperties().Where(m => m.GetValue(t, null) != null).ToArray();
            foreach (var item in ps)
            {
                if (item.PropertyType.Name != "ICollection`1")
                {
                    stateEntry.SetModifiedProperty(item.Name);
                }
            }
            return t;
        }
        public static T SetNullPropertys<T>(this T t) where T : class
        {

            Type type = t.GetType();
            System.Reflection.PropertyInfo[] ps = type.GetProperties().Where(m => m.GetValue(t, null) != null).ToArray();
            foreach (var item in ps)
            {
                if (item.PropertyType.FullName.Contains("Model."))
                {
                    item.SetValue(t, null);
                }
            }
            return t;
        }
        public static void CopyObj<T, K>(this T cur, K t) where T : class
        {

            Type type = t.GetType();
            System.Reflection.PropertyInfo[] ps = type.GetProperties().Where(m => m.GetValue(t, null) != null).ToArray();
            Type newtype = cur.GetType();
            foreach (var item in ps)
            {
                if (!item.PropertyType.FullName.Contains("Model.") && item.PropertyType.Name != "ICollection`1")
                {
                    newtype.GetProperties().Where(m => m.Name == item.Name).FirstOrDefault().SetValue(cur, item.GetValue(t, null));
                }
            }
        }
        #endregion
    }
}
