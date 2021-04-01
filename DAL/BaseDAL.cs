using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tools;
using System.Data.Entity;

namespace DAL
{
    public struct OrderModelField
    {
        /// <summary>
        /// 排序字段的名称
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// 排序方式是否倒序
        /// </summary>
        public bool IsDESC { get; set; }
    }
    public class PageParam { public int PageSize { get; set; } public int PageIndex { get; set; } }


    public class BaseDAL<T> where T : class, new()
    {

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>返回受影响行数</returns>
        public virtual T Add(T entity)
        {
            using (Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities())
            {
                db.Entry<T>(entity).State = EntityState.Added;
                db.SaveChanges();
                return entity;
            }
        }
        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="entityList">实体</param>
        /// <returns>是否成功</returns>
        public virtual bool AddTran(List<T> entityList)
        {
            using (Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities())
            {
                foreach (var entity in entityList)
                {
                    db.Set<T>().Attach(entity);
                    db.Entry<T>(entity).State = EntityState.Added;
                }
                return db.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// 全部修改
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>返回受影响行数</returns>
        public bool Update(T entity)
        {
            using (Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities())
            {
                db.Set<T>().Attach(entity);
                db.Entry<T>(entity).State = EntityState.Modified;
                return db.SaveChanges() > 0;
            }
        }
        /// <summary>
        /// 全部修改
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>返回受影响行数</returns>
        public virtual bool Update(List<T> entityList)
        {
            using (Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities())
            {
                foreach (var entity in entityList)
                {
                    db.Set<T>().Attach(entity);
                    db.Entry<T>(entity).State = EntityState.Modified;
                }
                return db.SaveChanges() > 0;
            }
        }
        /// <summary>
        /// 局部修改
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>返回受影响行数</returns>
        public virtual bool UpdatePartial(T entity)
        {
            using (Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities())
            {
                entity.SetNullPropertys<T>();
                db.Set<T>().Attach(entity);
                db.SetModifiedPropertys(entity);

                return db.SaveChanges() > 0;
            }
        }
        /// <summary>
        /// 部分更新
        /// </summary>
        /// <param name="whereLambda">表达式</param>
        /// <param name="entity">更新实体</param>
        /// <param name="isAll">是否全部更新</param>
        /// <returns></returns>
        public virtual int UpdatePartial(Expression<Func<T, bool>> whereLambda, T entity, bool isAll = true)
        {
            using (Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities())
            {
                var entityslist = db.Set<T>().Where(whereLambda);
                if (!isAll)
                {
                    entityslist = entityslist.Take(1);
                }
                List<T> entitys = entityslist.ToList();
                int number = entitys.Count;
                foreach (var item in entitys)
                {
                    Type itemtype = entity.GetType();
                    System.Reflection.PropertyInfo[] itemps = itemtype.GetProperties().ToArray();

                    Type type = entity.GetType();
                    System.Reflection.PropertyInfo[] ps = type.GetProperties().Where(m => m.GetValue(entity, null) != null).ToArray();
                    foreach (var pr in ps)
                    {
                        if (pr.PropertyType.Name != "ICollection`1" && !pr.PropertyType.FullName.Contains("Model."))
                        {
                            var psitem = itemtype.GetProperties().Where(k => k.Name == pr.Name).FirstOrDefault();
                            psitem.SetValue(item, pr.GetValue(entity, null));
                        }
                    }
                }
                db.SaveChanges();
                return number;
            }
        }
        /// <summary>
        /// 局部修改
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>返回受影响行数</returns>
        public virtual bool UpdatePartial(List<T> entityList)
        {
            using (Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities())
            {
                foreach (var entity in entityList)
                {
                    db.Set<T>().Attach(entity);
                    db.Entry<T>(entity).State = EntityState.Modified;
                    db.SetModifiedPropertys(entity);
                }
                return db.SaveChanges() > 0;
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>返回受影响行数</returns>
        public virtual bool Delete(T entity)
        {
            using (Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities())
            {
                db.Set<T>().Attach(entity);
                db.Entry<T>(entity).State = EntityState.Deleted;
                return db.SaveChanges() > 0;
            }
        }
        /// <summary>
        /// 根据条件删除
        /// </summary>
        /// <param name="deleWhere">删除条件</param>
        /// <returns>返回受影响行数</returns>
        public virtual bool DeleteByConditon(Expression<Func<T, bool>> deleWhere)
        {
            using (Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities())
            {
                List<T> entitys = db.Set<T>().Where(deleWhere).ToList();
                if (entitys.Count > 0)
                {
                    entitys.ForEach(m => db.Entry<T>(m).State = EntityState.Deleted);
                    return db.SaveChanges() > 0;
                }
                else
                {
                    return true;
                }


            }
        }
        /// <summary>
        /// 查找单个
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public T GetById(string id)
        {

            Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities();
            return db.Set<T>().Find(id);

        }
        /// <summary>
        /// 查找单个
        /// </summary>
        /// <param name="whereLambda">查询条件</param>
        /// <returns></returns>
        public T GetSingle(Expression<Func<T, bool>> whereLambda)
        {

            Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities();
            return db.Set<T>().FirstOrDefault(whereLambda);

        }

        /// <summary>
        /// 获取所有实体集合
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> GetAll()
        {
            Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities();
            return db.Set<T>();
        }

        /// <summary>
        /// 获取所有实体集合(多个排序)
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> GetAll(params OrderModelField[] orderByExpression)
        {

            Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities();
            return CommonSort(db.Set<T>(), orderByExpression);

        }
        /// <summary>
        /// 单个排序通用方法
        /// </summary>
        /// <typeparam name="Tkey">排序字段</typeparam>
        /// <param name="data">要排序的数据</param>
        /// <param name="orderLambda">排序条件</param>
        /// <param name="isDesc">是否倒序</param>
        /// <returns>排序后的集合</returns>
        public IQueryable<T> CommonSort<Tkey>(IQueryable<T> data, Expression<Func<T, Tkey>> orderLambda, bool isDesc)
        {
            if (isDesc)
            {
                return data.OrderByDescending(orderLambda);
            }
            else
            {
                return data.OrderBy(orderLambda);
            }
        }
        /// <summary>
        /// 多个排序通用方法
        /// </summary>
        /// <typeparam name="Tkey">排序字段</typeparam>
        /// <param name="data">要排序的数据</param>
        /// <param name="orderLambdaAndIsDesc">字典集合(排序条件,是否倒序)</param>
        /// <returns>排序后的集合</returns>
        public IQueryable<T> CommonSort(IQueryable<T> data, params OrderModelField[] orderByExpression)
        {
            //创建表达式变量参数
            var parameter = Expression.Parameter(typeof(T), "o");

            if (orderByExpression != null && orderByExpression.Length > 0)
            {
                for (int i = 0; i < orderByExpression.Length; i++)
                {
                    //根据属性名获取属性
                    var property = typeof(T).GetProperty(orderByExpression[i].PropertyName);
                    //创建一个访问属性的表达式
                    var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                    var orderByExp = Expression.Lambda(propertyAccess, parameter);

                    string OrderName = "";
                    if (i > 0)
                    {
                        OrderName = orderByExpression[i].IsDESC ? "ThenByDescending" : "ThenBy";
                    }
                    else
                        OrderName = orderByExpression[i].IsDESC ? "OrderByDescending" : "OrderBy";

                    MethodCallExpression resultExp = Expression.Call(typeof(Queryable), OrderName, new Type[] { typeof(T), property.PropertyType },
                        data.Expression, Expression.Quote(orderByExp));

                    data = data.Provider.CreateQuery<T>(resultExp);
                }
            }
            return data;
        }
        /// <summary>
        /// 根据条件查询实体集合
        /// </summary>
        /// <param name="whereLambda">查询条件 lambel表达式</param>
        /// <returns></returns>
        public IQueryable<T> GetList(Expression<Func<T, bool>> whereLambda)
        {
            Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities();

            return db.Set<T>().Where(whereLambda);

        }
        /// <summary>
        /// 根据条件查询实体集合(单个字段排序)
        /// </summary>
        /// <param name="whereLambda">查询条件 lambel表达式</param>
        /// <returns></returns>
        public IQueryable<T> GetList<Tkey>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, Tkey>> orderLambda, bool isDesc)
        {

            Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities();
            return CommonSort(db.Set<T>().Where(whereLambda), orderLambda, isDesc);

        }
        /// <summary>
        /// 根据条件查询实体集合(多个字段排序)
        /// </summary>
        /// <param name="whereLambda">查询条件 lambel表达式</param>
        /// <returns></returns>
        public IQueryable<T> GetList(Expression<Func<T, bool>> whereLambda, params OrderModelField[] orderByExpression)
        {

            Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities();
            return CommonSort(db.Set<T>().Where(whereLambda), orderByExpression);

        }
        /// <summary>
        /// 获取分页集合(无条件无排序)
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> GetListPaged<Tkey>(int pageIndex, int pageSize, out int totalcount)
        {
            Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities();
            totalcount = db.Set<T>().Count();//获取总数
                                             //需要增加AsExpandable(),否则查询的是所有数据到内存，然后再排序  AsExpandable是linqkit.dll中的方法
            return db.Set<T>().Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }
        /// <summary>
        /// 获取分页集合(无条件单个排序)
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> GetListPaged<Tkey>(int pageIndex, int pageSize, Expression<Func<T, Tkey>> orderLambda, bool isDesc, out int totalcount)
        {

            Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities();
            totalcount = db.Set<T>().Count();//获取总数
                                             //需要增加AsExpandable(),否则查询的是所有数据到内存，然后再排序  AsExpandable是linqkit.dll中的方法
            return CommonSort(db.Set<T>(), orderLambda, isDesc).Skip((pageIndex - 1) * pageSize).Take(pageSize);

        }
        /// <summary>
        /// 获取分页集合(无条件多字段排序)
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> GetListPaged(int pageIndex, int pageSize, out int totalcount, params OrderModelField[] orderByExpression)
        {

            Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities();
            totalcount = db.Set<T>().Count();//获取总数
                                             //需要增加AsExpandable(),否则查询的是所有数据到内存，然后再排序  AsExpandable是linqkit.dll中的方法
            return CommonSort(db.Set<T>(), orderByExpression).Skip((pageIndex - 1) * pageSize).Take(pageSize);

        }
        /// <summary>
        /// 获取分页集合(有条件无排序)
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> GetListPaged<Tkey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, out int totalcount)
        {

            Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities();
            totalcount = db.Set<T>().Where(whereLambda).Count();//获取总数
                                                                //需要增加AsExpandable(),否则查询的是所有数据到内存，然后再排序  AsExpandable是linqkit.dll中的方法
            return db.Set<T>().Where(whereLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize);

        }
        /// <summary>
        /// 获取分页集合(有条件单个排序)
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> GetListPaged<Tkey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, Expression<Func<T, Tkey>> orderLambda, bool isDesc, out int totalcount)
        {

            Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities();
            totalcount = db.Set<T>().Where(whereLambda).Count();//获取总数
                                                                //需要增加AsExpandable(),否则查询的是所有数据到内存，然后再排序  AsExpandable是linqkit.dll中的方法
            return CommonSort(db.Set<T>().Where(whereLambda), orderLambda, isDesc).Skip((pageIndex - 1) * pageSize).Take(pageSize);

        }
        /// <summary>
        /// 获取分页集合(有条件多字段排序)
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> GetListPaged(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, out int totalcount, params OrderModelField[] orderModelFiled)
        {

            Employee_Welfare_DBEntities db = new Employee_Welfare_DBEntities();
            totalcount = db.Set<T>().Where(whereLambda).Count();//获取总数
                                                                //需要增加AsExpandable(),否则查询的是所有数据到内存，然后再排序  AsExpandable是linqkit.dll中的方法
            return CommonSort(db.Set<T>().Where(whereLambda), orderModelFiled).Skip((pageIndex - 1) * pageSize).Take(pageSize);

        }
        /// <summary>
        /// 增加rownum列
        /// </summary>
        /// <param name="lsTemp"></param>
        /// <returns></returns>
        public List<T> AddRowNum(List<T> lsTemp)
        {
            if (typeof(T).GetProperty("RowNum") != null)
            {
                int i = 0;
                foreach (var item in lsTemp)
                {
                    typeof(T).GetProperty("RowNum").SetValue(item, i, null);
                    i++;
                }
            }
            return lsTemp;
        }
    }

}
