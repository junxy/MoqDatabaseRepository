using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;

namespace MoqDatabaseRepository
{
    public class Fack1
    {

        private readonly IProductRepository _mockProductsRespository;

        public Fack1()
        {
            IList<Product> products = new List<Product>
            {
                new Product
                {
                    ProductId = 1,
                    Name = "C# Unleashed",
                    Description = "Short description here",
                    Price = 49.99
                },
                new Product
                {
                    ProductId = 2,
                    Name = "ASP.Net Unleashed",
                    Description = "Short description here",
                    Price = 59.99
                },
                new Product
                {
                    ProductId = 3,
                    Name = "Silverlight Unleashed",
                    Description = "Short description here",
                    Price = 29.99
                }
            };

            var mockProductRepository = new Mock<IProductRepository>();

            mockProductRepository.Setup(mr => mr.FindAll()).Returns(products);

            mockProductRepository.Setup(mr => mr.FindById(It.IsAny<int>()))
                .Returns((int i) => products.SingleOrDefault(x => x.ProductId == i));

            mockProductRepository.Setup(mr => mr.FindByName(It.IsAny<string>()))
                .Returns((string s) => products.SingleOrDefault(x => x.Name == s));

            mockProductRepository.Setup(mr => mr.Save(It.IsAny<Product>()))
                .Returns((Product target) =>
                {
                    var now = DateTime.Now;

                    if (target.ProductId.Equals(default(int)))
                    {
                        target.DateCreated = now;
                        target.DateModified = now;
                        target.ProductId = products.Count() + 1;
                        products.Add(target);
                    }
                    else
                    {
                        var original = products.SingleOrDefault(q => q.ProductId == target.ProductId);

                        if (original == null)
                            return false;

                        original.Name = target.Name;
                        original.Price = target.Price;
                        original.Description = target.Description;
                        original.DateModified = now;
                    }
                    return true;
                });
            _mockProductsRespository = mockProductRepository.Object;
        }

        [Fact(DisplayName = "可以返回产品根据产品ID")]
        public void CanReturnProductById()
        {
            var testProduct = _mockProductsRespository.FindById(2);

            Assert.NotNull(testProduct);
            Assert.IsType<Product>(testProduct);
            Assert.Equal("ASP.Net Unleashed", testProduct.Name);

        }

        [Fact(DisplayName = "可以返回产品根据产品名称")]
        public void CanReturnProductByName()
        {
            var testProduct = _mockProductsRespository.FindByName("Silverlight Unleashed");

            Assert.NotNull(testProduct);
            Assert.IsType<Product>(testProduct);
            Assert.Equal(3, testProduct.ProductId);
        }

        [Fact(DisplayName = "可以返回所有产品")]
        public void CanReturnAllProducts()
        {
            var testProducts = _mockProductsRespository.FindAll();

            //Assert.NotNull(testProducts);
            Assert.NotEmpty(testProducts);
            Assert.Equal(3, testProducts.Count());

        }

        [Fact(DisplayName = "可以添加产品")]
        public void CanInsertProduct()
        {
            var newProduct = new Product
            {
                Name = "Pro C#",
                Description = "Short description here",
                Price = 39.99
            };

            var productContent = _mockProductsRespository.FindAll().Count();
            Assert.Equal(3, productContent);

            _mockProductsRespository.Save(newProduct);

            productContent = _mockProductsRespository.FindAll().Count();
            Assert.Equal(4, productContent);

            var testProduct = _mockProductsRespository.FindByName("Pro C#");
            Assert.NotNull(testProduct);
            Assert.IsType<Product>(testProduct);
            Assert.Equal(4, testProduct.ProductId);

        }

        [Fact(DisplayName = "可以更新产品")]
        public void CanUpdateProduct()
        {
            var testProduct = _mockProductsRespository.FindById(1);
            testProduct.Name = "C# 3.5 Unleashed";
            _mockProductsRespository.Save(testProduct);

            Assert.Equal("C# 3.5 Unleashed", _mockProductsRespository.FindById(1).Name);

        }

    }
}
