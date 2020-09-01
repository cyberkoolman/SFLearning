using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ECommerce.ProductCatalog.Model;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;

namespace ECommerce.ProductCatalog
{
    class ServiceFabricProductRepository : IProductRepository
    {
        private readonly IReliableStateManager _stateManger;

        public ServiceFabricProductRepository(IReliableStateManager stateManager)
        {
            _stateManger = stateManager;
        }

        public async Task AddProduct(Product product)
        {
            IReliableDictionary<Guid, Product> products = await _stateManger
                .GetOrAddAsync<IReliableDictionary<Guid, Product>>("products");

            using (ITransaction tx = _stateManger.CreateTransaction())
            {
                await products.GetOrAddAsync(tx, product.Id, product);
                await tx.CommitAsync();
            }
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            IReliableDictionary<Guid, Product> products = await _stateManger
                .GetOrAddAsync<IReliableDictionary<Guid, Product>>("products");
            var result = new List<Product>();

            using (ITransaction tx = _stateManger.CreateTransaction())
            {
                Microsoft.ServiceFabric.Data.IAsyncEnumerable<KeyValuePair<Guid, Product>> allProducts =
                    await products.CreateEnumerableAsync(tx, EnumerationMode.Unordered);

                using(Microsoft.ServiceFabric.Data.IAsyncEnumerator<KeyValuePair<Guid, Product>> enumerator = allProducts.GetAsyncEnumerator())
                {
                    while(await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        KeyValuePair<Guid, Product> current = enumerator.Current;
                        result.Add(current.Value);
                    }
                }
            }
            return result;
        }
    }
}
