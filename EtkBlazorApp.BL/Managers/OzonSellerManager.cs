﻿using EtkBlazorApp.DataAccess;
using EtkBlazorApp.DataAccess.Entity;
using EtkBlazorApp.Integration.Ozon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtkBlazorApp.BL
{
    public class OzonSellerManager
    {
        public const int OZON_MINIMUM_PRICE = 80;

        private readonly IProductStorage productsStorage;
        private readonly ISettingStorage settings;

        private Dictionary<string, decimal> manufacturerDiscounts;
        private List<OzonProductModel> offers;
        private List<ProductEntity> etkProducts;      
        private Dictionary<OzonProductModel, ProductEntity> correlationData;
        private OzonSellerApiClient api;

        public OzonSellerManager(ISettingStorage settings, IProductStorage productsStorage)
        {
            this.productsStorage = productsStorage;
            this.settings = settings;
        }

        public async Task Update()
        {
            offers = null;
            manufacturerDiscounts = null;
            etkProducts = null;
            correlationData = null;
            //await InitializeData();
            //await UpdateStock();
            //await UpdatePrice();

            await settings.SetValue("ozon_seller_last_update", DateTime.Now.ToString());
        }

        private async Task UpdateStock()
        {            
            try
            {
                Dictionary<OzonProductModel, int> offerToQuantity = correlationData.ToDictionary(cd => cd.Key, cd => cd.Value.quantity);
                await api.UpdateQuantity(offerToQuantity);
            }
            catch
            {
                throw;
            }
        }

        private async Task UpdatePrice()
        {
            try
            {
                var offerToPrice = new Dictionary<OzonProductModel, decimal>();
                foreach (var g in correlationData.GroupBy(p => p.Value.manufacturer))
                {
                    var manufacturerDiscount = manufacturerDiscounts[g.Key];
                    foreach (var item in g)
                    {
                        var priceWithDiscount = Math.Floor(item.Value.price * (1 + (manufacturerDiscount / 100m)) / 10m) * 10;
                        offerToPrice[item.Key] = Math.Max(OZON_MINIMUM_PRICE, priceWithDiscount);
                    }
                }

                await api.UpdatePrice(offerToPrice);
            }
            catch
            {
                throw;
            }
        }

        private async Task InitializeData()
        {
            string client_id = await settings.GetValue("ozon_seller_client_id");
            string api_key = await settings.GetValue("ozon_seller_api_key");
            api = new OzonSellerApiClient(client_id, api_key);

            //if (offers == null)
            //{
            //    offers = await api.GetAllProducts();
            //}

            //if (manufacturerDiscounts == null)
            //{
            //    string rawArray = await settings.GetValue("ozon_seller_manufacturer_discounts");
            //    manufacturerDiscounts = rawArray.Split(";").Select(chunk => chunk.Split("=")).ToDictionary(x => x[0], x => decimal.Parse(x[1]));
            //}

            //if (etkProducts == null)
            //{
            //    etkProducts = await dbManager.ReadProducts(etkConnection, false, manufacturerDiscounts.Select(d => d.Key.manufacturer_id));
            //}

            //if (correlationData == null)
            //{
            //    if (offers != null && etkProducts != null && (offers.Any() && etkProducts.Any()))
            //    {
            //        correlationData = await Task.Run(() => correlator.GetCorrelation(offers, etkProducts));
            //    }
            //}
        }       
    }
}
