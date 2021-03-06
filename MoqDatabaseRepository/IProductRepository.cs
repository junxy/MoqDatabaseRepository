﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoqDatabaseRepository
{
    public interface IProductRepository
    {
        IList<Product> FindAll();

        Product FindByName(string productName);

        Product FindById(int productId);

        bool Save(Product target);

    }
}
