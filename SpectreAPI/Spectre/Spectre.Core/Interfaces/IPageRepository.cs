﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Core.Models;

namespace Spectre.Core.Interfaces
{
    public interface IPageRepository
    {
        Task<Tuple<bool, List<Page>>> GetAll();
    }
}