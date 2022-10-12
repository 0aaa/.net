using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather0.DAL.Context;

namespace Weather0.DAL.Repositories
{
    public class GenericRepository<type> : IRepository<type> where type : class
    {
        private DbContext Context { get; set; }
        private DbSet<type> Set { get; set; }
        public GenericRepository()
        {
            Context = new WeatherContext();
            Set = Context.Set<type>();
        }
        public void AddUpdate(type entity) => Set.AddOrUpdate(entity);

        public void Delete(type entity) => Set.Remove(entity);

        public type Get(int id) => Set.Find(id);

        public IEnumerable<type> GetAll() => Set.ToList();

        public void Save() => Context.SaveChanges();
    }
}
