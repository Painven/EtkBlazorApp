﻿using EtkBlazorApp.BL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EtkBlazorApp.BL.CronTask
{
    public abstract class CronTaskByPriceListBase : CronTaskBase
    {
        private readonly Type templateType;

        public CronTaskByPriceListBase(Type templateType, CronTaskService service, CronTaskPrefix prefix) : base(service, prefix)
        {
            this.templateType = templateType;
        }

        protected override async Task Run()
        {
            var templateGuid = PriceListManager.GetPriceListGuidByType(templateType);
            var templateInfo = await service.templates.GetPriceListTemplateById(templateGuid);

            var loader = service.remoteTemplateLoaderFactory.GetMethod(templateInfo.remote_uri, templateInfo.remote_uri_method, templateGuid);
            using (var ms = new MemoryStream(await loader.GetBytes()))
            {
                var lines = await service.priceListManager.ReadTemplateLines(templateType, ms);
                await service.updateManager.UpdatePriceAndStock(lines, clearStockBeforeUpdate: true);
            }
        }
    }
}
