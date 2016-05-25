﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamGeneratorApp.Models;

namespace TeamGeneratorApp.DAL.Repositories
{
    public class CorrelationRepository : GenericRepository<Correlation>
    {
        public CorrelationRepository(TeamDb3Entities context) : base(context)
        {
        }


    }
}
