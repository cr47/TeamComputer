﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodProject.Models;

namespace FoodProject.Abstract
{
    public interface IRecipeRepository
    {
        IEnumerable<Recipe> Recipes { get; }
        Recipe GetRecipeID(int recipeID);
        void Add(Recipe r);

        void Save();
    }
}
