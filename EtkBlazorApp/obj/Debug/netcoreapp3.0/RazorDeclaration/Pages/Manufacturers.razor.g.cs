#pragma checksum "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\Pages\Manufacturers.razor" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "769bff4699bece8efe441750a8c61690e8840fea"
// <auto-generated/>
#pragma warning disable 1591
#pragma warning disable 0414
#pragma warning disable 0649
#pragma warning disable 0169

namespace EtkBlazorApp.Pages
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
#nullable restore
#line 1 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\_Imports.razor"
using System.Net.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\_Imports.razor"
using Microsoft.AspNetCore.Authorization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\_Imports.razor"
using Microsoft.AspNetCore.Components.Authorization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\_Imports.razor"
using Microsoft.AspNetCore.Components.Forms;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\_Imports.razor"
using Microsoft.AspNetCore.Components.Routing;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\_Imports.razor"
using Microsoft.AspNetCore.Components.Web;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\_Imports.razor"
using Microsoft.JSInterop;

#line default
#line hidden
#nullable disable
#nullable restore
#line 8 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\_Imports.razor"
using EtkBlazorApp;

#line default
#line hidden
#nullable disable
#nullable restore
#line 9 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\_Imports.razor"
using EtkBlazorApp.Shared;

#line default
#line hidden
#nullable disable
#nullable restore
#line 10 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\_Imports.razor"
using BlazorInputFile;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\Pages\Manufacturers.razor"
using EtkBlazorApp.DataAccess;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\Pages\Manufacturers.razor"
using Microsoft.Extensions.Configuration;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\Pages\Manufacturers.razor"
using EtkBlazorApp.Components;

#line default
#line hidden
#nullable disable
    [Microsoft.AspNetCore.Components.RouteAttribute("/manufacturers")]
    public partial class Manufacturers : Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
        }
        #pragma warning restore 1998
#nullable restore
#line 67 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\Pages\Manufacturers.razor"
       

    DeleteConfirmDialog DeleteDialog;
    List<ManufacturerModel> manufacturers = null;
    ManufacturerModel editManufacturer = null;
    ManufacturerModel deleteManufacturer = null;

    private void EditManufacturer(ManufacturerModel manufacturer)
    {
        editManufacturer = manufacturer;
    }

    private void ShowManufacturerDeleteDialog(ManufacturerModel manufacturer)
    {
        deleteManufacturer = manufacturer;
        DeleteDialog.Show();
    }

    private async Task ConfirmManufacturerChanges(ManufacturerModel manufacturer)
    {
        string sql = "UPDATE oc_manufacturer SET shipment_period = @shipment_period WHERE manufacturer_id = @manufacturer_id";
        await _database.SaveData<ManufacturerModel>(sql, manufacturer, _config.GetConnectionString("openserver_etk_db"));
        editManufacturer = null;

    }

    protected async Task ConfirmClicked(bool deleteConfirmed)
    {
        if (deleteManufacturer != null && deleteConfirmed)
        {

            string sql = "DELETE FROM oc_manufacturer WHERE manufacturer_id = @manufacturer_id";
            await _database.SaveData<ManufacturerModel>(sql, deleteManufacturer, _config.GetConnectionString("openserver_etk_db"));

            manufacturers.Remove(deleteManufacturer);
            deleteManufacturer = null;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        string sql = "SELECT * FROM oc_manufacturer ORDER BY name";
        manufacturers = await Task.Run(() => _database.LoadData<ManufacturerModel, dynamic>(sql, new { }, _config.GetConnectionString("openserver_etk_db")));
    }


#line default
#line hidden
#nullable disable
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private IConfiguration _config { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private IDatabase _database { get; set; }
    }
}
#pragma warning restore 1591
