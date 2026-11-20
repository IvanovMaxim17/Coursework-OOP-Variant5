using System;
using System.Collections.Generic;
using HotelManagement.Core.Models;

namespace HotelManagement.Core.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        void Add(T item);
        bool Remove(Guid id);
        T? GetById(Guid id);
        List<T> GetAll();
        List<T> Find(Predicate<T> match);
        void ReplaceAll(List<T> items);
    }
}