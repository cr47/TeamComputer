﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodProject.Models;

namespace FoodProject.Abstract
{
    public interface IFoodRepo
    {
        IEnumerable<Food> Foods { get; }
    }
}
