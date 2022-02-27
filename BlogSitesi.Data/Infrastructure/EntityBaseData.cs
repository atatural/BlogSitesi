using BlogSitesi.Data.Infrastructor.Entities;
using BlogSitesi.Model.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace BlogSitesi.Data.Infrastructor
{
    public class EntityBaseData<T> : IData<T> where T : ModelBase
    {
        protected readonly DbContext _context;
        public EntityBaseData(DbContext context)
        {
            this._context = context;
        }

        public void DetachAllEntities()
        {
            var entries = _context.ChangeTracker.Entries()
                .Where(e => e.State != EntityState.Detached)
                .ToList();
            foreach(var entry in entries)
            {
                if(entry.Entity !=null)
                {
                    entry.State = EntityState.Detached;
                }
            }
        }

        protected virtual void BeforeUpdate() { }
        protected virtual void AfterUpdate() { }
        protected virtual void BeforeInsert() { }
        protected virtual void AfterInsert() { }
        protected virtual void BeforeDelete() { }
        protected virtual void AfterDelete() { }


        public DataResult Delete(T t)
        {
            return DeleteByKey(t.Id);
        }

        public DataResult DeleteByKey(int id)
        {
            try
            {
                T aModel = _context.Set<T>().Where(x => x.Id == id).FirstOrDefault();
                if(aModel == null)
                    return new DataResult(false, "Silinecek kayıt bulunamadı.");
                BeforeDelete();
                _context.Set<T>().Remove(aModel);
                AfterDelete();
                _context.SaveChanges();

                return new DataResult(true, "");
            }
            catch (Exception exc)
            {
                return new DataResult(false, exc.Message + exc.InnerException == null ? "" : "(" + exc.InnerException + ")");
            }
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate, bool asNoTracking = false)
        {
            try
            {
                var query = _context.Set<T>().AsQueryable();

                if (asNoTracking)
                    query = query.AsNoTracking();

                return query.Where(predicate).Take(1).FirstOrDefault();
            }
            catch (Exception exc)
            {
                return null;
            }
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate, string orderBy = "Id", bool isDesc = false, bool asNoTracking = false)
        {
            try
            {
                var query = _context.Set<T>().AsQueryable();

                if (asNoTracking)
                    query = query.AsNoTracking();

                query = isDesc
                    ? query.OrderByDescending(orderBy)
                    : query.OrderBy(orderBy);

                return query.Where(predicate).FirstOrDefault();
            }
            catch (Exception exc)
            {
                return null;
            }
        }

        public List<T> GetAll()
        {
            try
            {
                return _context.Set<T>().ToList();
            }
            catch (Exception exc)
            {

                return new List<T>();
            }
        }

        public List<T> GetAll(string orderBy, bool isDesc = false)
        {
            return isDesc
                ? _context.Set<T>().OrderByDescending(orderBy).ToList()
                : _context.Set<T>().OrderBy(orderBy).ToList();
        }

        public List<T> GetBy(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return _context.Set<T>()
                    .Where(predicate)
                    .ToList();
            }
            catch (Exception exc)
            {
                return new List<T>();
            }
        }

        public List<T> GetBy(Expression<Func<T, bool>> predicate, string orderBy, bool isDesc = false)
        {
            try
            {
                return isDesc
                    ? _context.Set<T>().Where(predicate).OrderByDescending(orderBy).ToList()
                    : _context.Set<T>().Where(predicate).OrderBy(orderBy).ToList();
            }
            catch (Exception exc)
            {
                return new List<T>();            
            }
        }
        public T GetByKey(int id)
        {
            try
            {
                T aModel = _context.Set<T>().Where(x => x.Id == id).FirstOrDefault();
                return aModel;
            }
            catch (Exception exc)
            {
                return null;
            }
        }

        public int GetCount()
        {
            return _context.Set<T>().Select(x => x.Id).Count();
        }

        public int GetCount(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>()
                .Where(predicate)
                .Count();
        }

        public List<T> GetByPage(int pageNumber, int pageCount, string orderBy = "Id", bool isDesc = false)
        {
            try
            {
                return isDesc
                    ? _context.Set<T>().OrderByDescending(orderBy).Skip((pageNumber - 1)*pageCount).Take(pageCount).ToList()
                    : _context.Set<T>().OrderBy(orderBy).Skip((pageNumber - 1) * pageCount).Take(pageCount).ToList();
            }
            catch (Exception exc)
            {
                return new List<T>();
            }
        }

        public List<T> GetByPage(Expression<Func<T, bool>> predicate, int pageNumber, int pageCount, string orderBy = "Id", bool isDesc = false)
        {
            try
            {
                return isDesc
                    ? _context.Set<T>().OrderByDescending(orderBy).Where(predicate).Skip((pageNumber - 1) * pageCount).Take(pageCount).ToList()
                    : _context.Set<T>().OrderBy(orderBy).Where(predicate).Skip((pageNumber - 1) * pageCount).Take(pageCount).ToList();
            }
            catch (Exception exc)
            {
                return new List<T>();
            }
        }

        public DataResult Insert(T t)
        {
            try
            {
                BeforeInsert();
                _context.Set<T>().Add(t);
                AfterInsert();
                _context.SaveChanges();
                
                return new DataResult(true, "");
            }
            catch (Exception exc)
            {
                return new DataResult(false, exc.Message + exc.InnerException == null ? " " : "(" + exc.InnerException + ")");
            }
        }

        public DataResult InsertBulk(List<T> ts, bool validateAndIgnoreBefore = false)
        {
            if (ts.Count <= 0)
                return new DataResult(true, "");
            try
            {
                foreach (var item in ts)
                {
                    if (validateAndIgnoreBefore && typeof(IValidateObject).IsAssignableFrom(item.GetType()))
                    {
                        var results = new List<ValidationResult>();
                        bool isValid = Validator.TryValidateObject(item, new ValidationContext(item, null, null), results, true);

                        if(!isValid)
                        {
                            var resultFirst = results[0];
                            continue;
                        }
                    }
                    _context.Set<T>().Add(item);
                }
                _context.SaveChanges();
                return new DataResult(true, "");
            }
            catch (Exception exc)
            {
                return new DataResult(false, exc.Message); //31:50
            }
        }

        public DataResult Update(T t)
        {
            try
            {
                int updateId = t.Id;

                T aModel = _context.Set<T>().Where(x => x.Id == updateId).FirstOrDefault();

                if (aModel == null)
                    return new DataResult(false, "Güncelleme yapılacak kayıt bulunamadı.");
                
                BeforeUpdate();
                foreach (var propertyInfo in typeof(T).GetProperties())
                {
                    var hasIgnore = Attribute.IsDefined(propertyInfo, typeof(IgnoredAttribute));
                    if (hasIgnore)
                        continue;

                    propertyInfo.SetValue(aModel, propertyInfo.GetValue(t, null), null);
                }
                AfterUpdate();
                _context.SaveChanges();
                return new DataResult(true, "");
            }
            catch (Exception exc)
            {
                return new DataResult(false, exc.Message + exc.InnerException == null ? " " : "(" + exc.InnerException + ")");
            }
        }

        public List<T> GetPage(int pageNumber, int pageCount, string orderBy = "Id", bool isDesc = false)
        {
            throw new NotImplementedException();
        }

        public List<T> GetPage(Expression<Func<T, bool>> predicate, int pageNumber, int pageCount, string orderBy = "Id", bool isDesc = false)
        {
            throw new NotImplementedException();
        }
    }
}
