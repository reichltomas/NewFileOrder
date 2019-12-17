using System;
using System.Collections.Generic;
using System.Text;

namespace NewFileOrder.Models
{
    class Manager
    {
        protected MyDbContext _db;
        public Manager(MyDbContext dbContext)
        {
            _db = dbContext;
        }
    }
}
