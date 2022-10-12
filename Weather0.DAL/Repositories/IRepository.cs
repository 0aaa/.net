using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather0.DAL.Repositories
{
    interface IRepository<type>
    {
        IEnumerable<type> GetAll();
        type Get(int id);
        void AddUpdate(type entity);
        void Delete(type entity);
        void Save();
    }
}
