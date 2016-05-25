﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamGeneratorApp.Models;

namespace TeamGeneratorApp.DAL.Repositories
{
    public class PoolRepository : GenericRepository<Pool>
    {
        public PoolRepository(TeamDb3Entities context) : base(context)
        {
        }


    }
}