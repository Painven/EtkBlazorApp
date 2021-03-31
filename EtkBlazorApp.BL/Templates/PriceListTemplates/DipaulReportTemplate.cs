﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EtkBlazorApp.BL.Templates.PriceListTemplates
{
    [PriceListTemplateDescription("C53B8C85-3115-421F-A579-0B5BFFF6EF48")]
    public class DipaulPriceListTemplate : ExcelPriceListTemplateBase
    {
        static readonly Dictionary<string, string> ValidManufacturersMap = new Dictionary<string, string>()
        {
            ["Hakko"] = "Hakko",
            ["Keysight"] = "Keysight",
            ["ITECH ВЭД"] = "ITECH"
        };

        public DipaulPriceListTemplate(string fileName) : base(fileName) { }

        protected override List<PriceLine> ReadDataFromExcel()
        {
            var list = new List<PriceLine>();
            var tab = Excel.Workbook.Worksheets[0];

            for (int row = 1; row < tab.Dimension.Rows; row++)
            {
                string manufacturer = tab.Cells[row, 2].ToString();

                if (!ValidManufacturersMap.ContainsKey(manufacturer))
                {
                    continue;
                }
                else
                {
                    manufacturer = ValidManufacturersMap[manufacturer];
                }

                string skuNumber = tab.Cells[row, 0].ToString();
                string productName = tab.Cells[row, 1].ToString();
                string quantityString = tab.Cells[row, 3].ToString();
                string priceString = tab.Cells[row, 4].ToString();
                string model = Regex.Match(productName, "^(.*?), ").Groups[1].Value;
                string currencyTypeString = tab.Cells[row, 5]?.ToString()?.Replace("руб.", "RUB");

                CurrencyType? priceCurreny = null;
                if (!string.IsNullOrEmpty(currencyTypeString))
                {
                    priceCurreny = Enum.Parse<CurrencyType>(currencyTypeString);
                }

                decimal? parsedPrice = null;
                if (priceCurreny.HasValue && decimal.TryParse(priceString, out var price))
                {
                    parsedPrice = price;
                }

                int? parsedQuantity = null;
                if (int.TryParse(quantityString, out var quantity))
                {
                    parsedQuantity = Math.Max(quantity, 0);
                }

                if (parsedPrice.HasValue || parsedQuantity.HasValue)
                {
                    var priceLine = new PriceLine(this)
                    {
                        Name = productName,
                        Manufacturer = manufacturer,
                        Model = model,
                        Sku = skuNumber,
                        Price = parsedPrice,
                        Currency = priceCurreny.Value,
                        Quantity = parsedQuantity
                    };
                    list.Add(priceLine);
                }
            }

            return list;
        }
    }

    [PriceListTemplateDescription("5CFDD5BD-816C-44DC-8AF3-9418F4052BF2")]
    public class HakkoPriceListTemplate : ExcelPriceListTemplateBase
    {
        public HakkoPriceListTemplate(string fileName) : base(fileName) { }

        protected override List<PriceLine> ReadDataFromExcel()
        {
            var list = new List<PriceLine>();
            var tab = Excel.Workbook.Worksheets[0];

            for (int row = 1; row < tab.Dimension.Rows; row++)
            {
                string skuNumber = tab.Cells[row, 0].ToString();
                string name = tab.Cells[row, 1].ToString();
                string priceString = tab.Cells[row, 2].ToString();

                if (decimal.TryParse(priceString, out var price))
                {
                    var priceLine = new PriceLine(this)
                    {
                        Name = name,
                        Currency = CurrencyType.RUB,
                        Manufacturer = "Hakko",
                        Sku = skuNumber,
                        Price = price
                    };
                    list.Add(priceLine);
                }
            }

            return list;
        }
        
    }
}
