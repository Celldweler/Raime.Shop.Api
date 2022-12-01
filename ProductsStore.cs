using Shop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raime.Shop.Api
{
    public class ProductsStore
    {
        private List<Product> _products = new List<Product>();
        private const int _count = 3; 
        //public ProductsStore()
        //{
        //    _products = new List<Product>
        //    {
        //        new Product
        //        {
        //            ProductId = 1,
        //            Name = "test",
        //            Description="test",
        //            Price = 45,
        //        },
        //        new Product
        //        {
        //            ProductId = 2,
        //            Name = "test 222",
        //            Description="test 222",
        //            Price = 22,
        //        },
        //        new Product
        //        {
        //            ProductId = 3,
        //            Name = "test333",
        //            Description="test333 33",
        //            Price = 333,
        //        },
        //    };
        //}

        public IEnumerable<Product> GetProducts => _products;

        public Product GetProductById(int id) => _products.First(p => p.ProductId == id);

        public void UpdateProduct(Product editedProduct)
        {
            var product = _products.First(p => p.ProductId == editedProduct.ProductId);

            product.Name = editedProduct.Name;
            product.Price = editedProduct.Price;
            product.Description = editedProduct.Description;
        }

        public void RemoveProduct(int productId)
        {
            var product = _products.First(p => p.ProductId == productId);
            
            _products.Remove(product);
        }

        public Product AddNewProduct(Product product)
        {
            //product.ProductId = _products.Count() + 1;
            
            _products.Add(product);

            return product;
        }
    }
}
