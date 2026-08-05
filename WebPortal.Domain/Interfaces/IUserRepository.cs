using System;
using System.Collections.Generic;
namespace WebPortal.Domain.Interfaces
{
    public interface IFetchableRepository<T> where T : class
    {
        T GetByName(string bankName, string userName);
    }
}
