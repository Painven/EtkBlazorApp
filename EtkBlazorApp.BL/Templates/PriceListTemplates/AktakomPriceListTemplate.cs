﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EtkBlazorApp.BL.Templates
{
    [PriceListTemplateGuid("D31FFE97-53BC-41DC-9D54-43FABBC51BCD")]
    public class AktakomPriceListTemplate : CsvPriceListTemplateBase
    {
        public AktakomPriceListTemplate(string fileName) : base(fileName)
        {

        }

        public override async Task<List<PriceLine>> ReadPriceLines(CancellationToken? token = null)
        {
            var list = new List<PriceLine>();

            var lines = await ReadCsvLines('\t', Encoding.GetEncoding("windows-1251"));

            foreach (var row in lines)
            {
                string name = row[1].Trim();
                string sku = row[2].Trim();
                string quantityString = row[20].Trim();
                string priceInRUR = row[21].Trim();

                decimal? parsedPrice = null;
                if (decimal.TryParse(priceInRUR.Trim(), out var price))
                {
                    parsedPrice = Math.Floor(price);
                }

                int? parsedQuantity = null;
                if (int.TryParse(quantityString, out var quantity))
                {
                    parsedQuantity = quantity;
                }

                if (parsedPrice.HasValue || parsedQuantity.HasValue)
                {
                    var priceLine = new PriceLine(this)
                    {
                        Name = name,
                        Currency = CurrencyType.RUB,
                        Stock = StockName.Aktakom,
                        Model = sku,
                        Sku = sku,
                        Price = parsedPrice,
                        Quantity = parsedQuantity,
                        Manufacturer = "Aktakom"
                    };
                    list.Add(priceLine);
                }
            }

            return list;
        }
    }
}
