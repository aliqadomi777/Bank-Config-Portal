using System.Collections.Generic;

namespace WebPortal.Domain.Interfaces
{
    public interface IFetchableRepository<T> where T : class
    {
        T GetById(int id);
    }

    public interface IListableRepository<T> where T : class
    {
        IEnumerable<T> GetAll(int id);
    }

    public interface IAddableRepository<T> where T : class
    {
        int Add(T entity);
    }

    public interface IUpdateableRepository<T> where T : class
    {
        bool Update(T entity);
    }

    public interface IDeleteableRepository<T> where T : class
    {
        bool Delete(int id);
    }

    public interface IGetAllRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
    }
}
