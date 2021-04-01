using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BaseBLL<T> where T : class, new()
    {
        BaseDAL<T> dal = new BaseDAL<T>();
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual T Add(T model)
        {
            return dal.Add(model);
        }
        /// <summary>
        /// 批量导入数据
        /// </summary>
        /// <param name="entityList">数据列表</param>
        /// <returns>是否成功</returns>
        public virtual bool AddTran(List<T> entityList)
        {
            return dal.AddTran(entityList);
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual bool Del(Expression<Func<T, bool>> deleWhere)
        {
            return dal.DeleteByConditon(deleWhere);
        }
        /// <summary>
        /// 查找单个
        /// </summary>
        /// <param name="whereLambda">查询条件</param>
        /// <returns></returns>
        public virtual T GetSingle(Expression<Func<T, bool>> whereLambda)
        {
            return dal.GetSingle(whereLambda);

        }
        /// <summary>
        /// 查找单个
        /// </summary>
        /// <param name="whereLambda">查询条件</param>
        /// <returns></returns>
        public virtual IQueryable<T> GetList(Expression<Func<T, bool>> whereLambda)
        {

            return dal.GetList(whereLambda);

        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual bool Del(T model)
        {
            return dal.Delete(model);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="proName"></param>
        /// <returns></returns>
        public virtual bool Modify(T model, params string[] proName)
        {
            return dal.UpdatePartial(model);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isPort">true:部分更新,主键必须赋值,负责更新失败</param>
        /// <returns></returns>
        public virtual int Modify(Expression<Func<T, bool>> whereLambda, T model, bool isAll = true)
        {
            return dal.UpdatePartial(whereLambda, model, isAll);

        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="isPort">true:部分更新</param>
        /// <returns></returns>
        public virtual bool Modify(T model, bool isPort = true)
        {
            if (isPort)
                return dal.UpdatePartial(model);
            else
                return dal.Update(model);
        }
        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> GetAll()
        {
            return dal.GetAll();
        }
        /// <summary>
        /// 通过主键获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetById(string id)
        {
            return dal.GetById(id);
        }
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="orderLambda"></param>
        /// <param name="isDesc"></param>
        /// <param name="whereLambda"></param>
        /// <param name="pagesize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public virtual IQueryable<T> GetPageList<TKey>(Expression<Func<T, TKey>> orderLambda, bool isDesc, Expression<Func<T, bool>> whereLambda, int pagesize, int pageIndex, out int count)
        {
            return dal.GetListPaged(pageIndex, pagesize, whereLambda, orderLambda, isDesc, out count);
        }
        public IQueryable<T> GetPageList(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, out int totalcount, params OrderModelField[] orderModelFiled)
        {
            return dal.GetListPaged(pageIndex, pageSize, whereLambda, out totalcount, orderModelFiled);
        }
        public IQueryable<T> GetPageList(int pageIndex, int pageSize, out int totalcount)
        {
            return dal.GetListPaged(pageIndex, pageSize, out totalcount);
        }
        /// <summary>
        /// 获取分页集合(无条件多字段排序)
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> GetPageList(int pageIndex, int pageSize, out int totalcount, params OrderModelField[] orderByExpression)
        {
            return dal.GetListPaged(pageIndex, pageSize, out totalcount, orderByExpression);
        }
    }
}
