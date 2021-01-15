
using MemberShipWebsite.Areas.Admin.Models;
using MemberShipWebsite.Entities;
using MemberShipWebsite.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace MemberShipWebsite.Areas.Admin.Extensions
{
    public static class ConversionExtensions
    {
        #region Product
        public static async Task<IEnumerable<ProductModel>> Convert(
            this IEnumerable<Product> products, ApplicationDbContext db)
        {
            if (products.Count().Equals(0))
                return new List<ProductModel>();

            var texts = await db.ProductLinkTexts.ToListAsync();
            var types = await db.ProductTypes.ToListAsync();

            return from p in products
                   select new ProductModel
                   {
                       Id = p.Id,
                       Title = p.Title,
                       Description = p.Description,
                       ImageUrl = p.ImageUrl,
                       ProductLinkTextId = p.ProductLinkTextId,
                       ProductTypeId = p.ProductTypeId,
                       ProductLinkTexts = texts,
                       ProductTypes = types
                   };
        }

        public static async Task<ProductModel> Convert(
        this Product product, ApplicationDbContext db)
        {
            var text = await db.ProductLinkTexts.FirstOrDefaultAsync(
                p => p.Id.Equals(product.ProductLinkTextId));
            var type = await db.ProductTypes.FirstOrDefaultAsync(
                p => p.Id.Equals(product.ProductTypeId));

            var model = new ProductModel
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                ProductLinkTextId = product.ProductLinkTextId,
                ProductTypeId = product.ProductTypeId,
                ProductLinkTexts = new List<ProductLinkText>(),
                ProductTypes = new List<ProductType>()
            };

            model.ProductLinkTexts.Add(text);
            model.ProductTypes.Add(type);

            return model;
        }
        #endregion

        #region ProductItem

        public static async Task Change(
           this ProductItem productItem, ApplicationDbContext db)
        {
            var oldProductItem = await db.ProductItems.FirstOrDefaultAsync(
                    pi => pi.ProductId.Equals(productItem.OldProductId) &&
                    pi.ItemId.Equals(productItem.OldItemId));

            var newProductItem = await db.ProductItems.FirstOrDefaultAsync(
                pi => pi.ProductId.Equals(productItem.ProductId) &&
                pi.ItemId.Equals(productItem.ItemId));

            if (oldProductItem != null && newProductItem == null)
            {
                newProductItem = new ProductItem
                {
                    ItemId = productItem.ItemId,
                    ProductId = productItem.ProductId
                };

                using (var transaction = new TransactionScope(
                    TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        db.ProductItems.Remove(oldProductItem);
                        db.ProductItems.Add(newProductItem);

                        await db.SaveChangesAsync();
                        transaction.Complete();
                    }
                    catch { transaction.Dispose(); }
                }
            }
        }


        public static async Task<bool> CanChange(
            this ProductItem productItem, ApplicationDbContext db)
        {
            var oldPI = await db.ProductItems.CountAsync(pi =>
                pi.ProductId.Equals(productItem.OldProductId) &&
                pi.ItemId.Equals(productItem.OldItemId));

            var newPI = await db.ProductItems.CountAsync(pi =>
                pi.ProductId.Equals(productItem.ProductId) &&
                pi.ItemId.Equals(productItem.ItemId));

            return oldPI.Equals(1) && newPI.Equals(0);
        }


        public static async Task<IEnumerable<ProductItemModel>> Convert(
       this IQueryable<ProductItem> productItems, ApplicationDbContext db)
        {
            if(productItems.Count().Equals(0))
            {
                return new List<ProductItemModel>();
            }

            return await (from pi in productItems
                          select new ProductItemModel
                          {
                              ItemId = pi.ItemId,
                              ProductId = pi.ProductId,
                              ProductTitle = db.Products.FirstOrDefault(p => p.Id.Equals(pi.ProductId)).Title,
                              ItemTitle = db.Items.FirstOrDefault(p => p.Id.Equals(pi.ItemId)).Title
                          }).ToListAsync();
        }

        public static async Task<ProductItemModel> Convert(
        this ProductItem product, ApplicationDbContext db, bool addListData = true)
        {
            var model = new ProductItemModel
            {
                ProductId = product.ProductId,
                ItemId = product.ItemId,
                Items = addListData ? await db.Items.ToListAsync() : null,
                Products = addListData ? await db.Products.ToListAsync() : null,
                ItemTitle = (await db.Items.FirstOrDefaultAsync(pi => pi.Id.Equals(product.ItemId))).Title,
                ProductTitle = (await db.Items.FirstOrDefaultAsync(pi => pi.Id.Equals(product.ProductId))).Title
            };

            return  model;
        }


        #endregion


        #region subscriptionProduct
        public static async Task<IEnumerable<SubscriptionProductModel>> Convert(
      this IQueryable<SubscriptionProduct> subscriptionProducts, ApplicationDbContext db)
        {
            if (subscriptionProducts.Count().Equals(0))
            {
                return new List<SubscriptionProductModel>();
            }

            return await (from pi in subscriptionProducts
                          select new SubscriptionProductModel
                          {
                              SubscriptionId = pi.SubscriptionId,
                              ProductId = pi.ProductId,
                              ProductTitle = db.Products.FirstOrDefault(p => p.Id.Equals(pi.ProductId)).Title,
                              SubscriptionTitle = db.Items.FirstOrDefault(p => p.Id.Equals(pi.SubscriptionId)).Title
                          }).ToListAsync();
        }

        public static async Task<SubscriptionProductModel> Convert(
      this SubscriptionProduct subscriptionProduct, ApplicationDbContext db, bool addListData = true)
        {
           

            var Model =  new SubscriptionProductModel
                          {
                              SubscriptionId = subscriptionProduct.SubscriptionId,
                              ProductId = subscriptionProduct.ProductId,
                              ProductTitle = db.Products.FirstOrDefault(p => p.Id.Equals(subscriptionProduct.ProductId)).Title,
                              SubscriptionTitle = db.Items.FirstOrDefault(p => p.Id.Equals(subscriptionProduct.SubscriptionId)).Title,
                              Products =addListData? await db.Products.ToListAsync():null,
                              Subscriptions = addListData ? await db.Subscriptions.ToListAsync():null
                          };

            return Model;
        }

        public static async Task<bool> CanChange(
       this SubscriptionProduct subscriptionproduct, ApplicationDbContext db)
        {
            var oldPI = await db.SubscriptionProducts.CountAsync(pi =>
                pi.ProductId.Equals(subscriptionproduct.OldProductId) &&
                pi.SubscriptionId.Equals(subscriptionproduct.OldSubscriptionId));

            var newPI = await db.SubscriptionProducts.CountAsync(pi =>
                pi.ProductId.Equals(subscriptionproduct.ProductId) &&
                pi.SubscriptionId.Equals(subscriptionproduct.SubscriptionId));

            return oldPI.Equals(1) && newPI.Equals(0);
        }

        public static async Task Change(
          this SubscriptionProduct subscriptionProduct, ApplicationDbContext db)
        {
            var oldSubscriptionProduct = await db.SubscriptionProducts.FirstOrDefaultAsync(
                    pi => pi.ProductId.Equals(subscriptionProduct.OldProductId) &&
                    pi.SubscriptionId.Equals(subscriptionProduct.OldSubscriptionId));

            var newSubscriptionProduct = await db.SubscriptionProducts.FirstOrDefaultAsync(
                pi => pi.ProductId.Equals(subscriptionProduct.ProductId) &&
                pi.SubscriptionId.Equals(subscriptionProduct.SubscriptionId));

            if (oldSubscriptionProduct != null && newSubscriptionProduct == null)
            {
                newSubscriptionProduct = new SubscriptionProduct
                {
                    SubscriptionId = subscriptionProduct.SubscriptionId,
                    ProductId = subscriptionProduct.ProductId
                };

                using (var transaction = new TransactionScope(
                    TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        db.SubscriptionProducts.Remove(oldSubscriptionProduct);
                        db.SubscriptionProducts.Add(newSubscriptionProduct);

                        await db.SaveChangesAsync();
                        transaction.Complete();
                    }
                    catch { transaction.Dispose(); }
                }
            }
        }

        #endregion 



    }
}