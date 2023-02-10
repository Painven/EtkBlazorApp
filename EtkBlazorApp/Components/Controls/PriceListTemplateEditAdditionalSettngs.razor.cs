using Blazored.Toast.Services;
using EtkBlazorApp.BL;
using EtkBlazorApp.DataAccess;
using EtkBlazorApp.DataAccess.Entity;
using EtkBlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtkBlazorApp.Components.Controls;

public partial class PriceListTemplateEditAdditionalSettngs
{
    [Parameter] public PriceListTemplateItemViewModel sourceTemplate { get; set; }
    [Inject] public IPriceListTemplateStorage templateStorage { get; set; }
    [Inject] public UserLogger logger { get; set; }
    [Inject] public IToastService toasts { get; set; }

    SkipManufacturerListType newSkipManufacturerListType = SkipManufacturerListType.black_list;
    ManufacturerEntity newSkipManufacturerItem;
    ManufacturerEntity newManufacturerMapRecordItem;
    ManufacturerEntity newDiscountMapRecordItem;
    string newManufacturerMapRecordWord;
    string newQuantityMapRecordWord;
    decimal newDiscountMapValue;
    int newQuantityMapRecordValue;

    bool addNewManufacturerMapButtonDisabled
    {
        get
        {
            return string.IsNullOrWhiteSpace(newManufacturerMapRecordWord) ||
                newManufacturerMapRecordItem == null || newManufacturerMapRecordItem.name.Equals(newManufacturerMapRecordWord, StringComparison.OrdinalIgnoreCase);
        }
    }
    bool skipManufacturerAddNewRecordButtonDisabled
    {
        get
        {
            return newSkipManufacturerItem == null ||
                sourceTemplate.ManufacturerSkipList.Any(i => i.manufacturer_id == newSkipManufacturerItem.manufacturer_id || i.ListType != newSkipManufacturerListType);
        }
    }

    private async Task AddDiscountMapRecord()
    {
        await templateStorage.AddDiscountMapRecord(sourceTemplate.Guid, newDiscountMapRecordItem.manufacturer_id, newDiscountMapValue);

        sourceTemplate.ManufacturerDiscountMap.Add(new ManufacturerDiscountItemViewModel()
        {
            manufacturer_id = newDiscountMapRecordItem.manufacturer_id,
            manufacturer_name = newDiscountMapRecordItem.name,
            discount = newDiscountMapValue
        });
        StateHasChanged();

        toasts.ShowSuccess("���������");
        await logger.Write(LogEntryGroupName.TemplateUpdate, "���������", $"������ � ������ � �����-�����'{newDiscountMapRecordItem.name}' --> '{newDiscountMapValue}' ��� ������� {sourceTemplate.Title}");
    }

    private async Task RemoveDiscountMapRecord(ManufacturerDiscountItemViewModel data)
    {
        await templateStorage.RemoveDiscountMapRecord(sourceTemplate.Guid, data.manufacturer_id);
        sourceTemplate.ManufacturerDiscountMap.Remove(data);
        StateHasChanged();

        toasts.ShowSuccess("���������");
        await logger.Write(LogEntryGroupName.TemplateUpdate, "������", $"������� ��� '{data.manufacturer_name}' �� ������� {sourceTemplate.Title}");
    }

    private async Task AddManufacturerMapRecord()
    {
        await templateStorage.AddManufacturerMapRecord(sourceTemplate.Guid, newManufacturerMapRecordWord, newManufacturerMapRecordItem.manufacturer_id);

        sourceTemplate.ManufacturerNameMap[newManufacturerMapRecordWord] = newManufacturerMapRecordItem.name;
        StateHasChanged();

        toasts.ShowSuccess("���������");
        await logger.Write(LogEntryGroupName.TemplateUpdate, "���������", $"�������������� �������� ������ '{newManufacturerMapRecordWord}' --> '{newManufacturerMapRecordItem.name}' ��� ������� {sourceTemplate.Title}");
    }

    private async Task RemoveManufacturerMapRecord(string word)
    {
        await templateStorage.RemoveManufacturerMapRecord(sourceTemplate.Guid, word);
        sourceTemplate.ManufacturerNameMap.Remove(word);
        StateHasChanged();

        toasts.ShowSuccess("���������");
        await logger.Write(LogEntryGroupName.TemplateUpdate, "������", $"�������������� �������� ������ '{word}' �� ������� {sourceTemplate.Title}");
    }

    private async Task AddNewQuantityMapRecord()
    {
        await templateStorage.AddQuantityMapRecord(sourceTemplate.Guid, newQuantityMapRecordWord, newQuantityMapRecordValue);
        sourceTemplate.QuantityMap[newQuantityMapRecordWord] = newQuantityMapRecordValue;
        StateHasChanged();

        toasts.ShowSuccess("���������");
        await logger.Write(LogEntryGroupName.TemplateUpdate, "���������", $"�������������� �������� '{newQuantityMapRecordWord}' --> '{newQuantityMapRecordValue}' ��� ������� {sourceTemplate.Title}");
    }

    private async Task RemoveQuantityMapRecord(string word)
    {
        await templateStorage.RemoveQuantityMapRecord(sourceTemplate.Guid, word);
        sourceTemplate.QuantityMap.Remove(word);
        StateHasChanged();

        toasts.ShowSuccess("���������");
        await logger.Write(LogEntryGroupName.TemplateUpdate, "������", $"�������������� �������� '{word}' �� ������� {sourceTemplate.Title}");
    }

    private async Task AddSkipManufacturerRecord()
    {
        await templateStorage.AddSkipManufacturerRecord(sourceTemplate.Guid, newSkipManufacturerItem.manufacturer_id, newSkipManufacturerListType.ToString());
        var skipItem = new ManufacturerSkipItemViewModel()
        {
            manufacturer_id = newSkipManufacturerItem.manufacturer_id,
            Name = newSkipManufacturerItem.name,
            ListType = newSkipManufacturerListType
        };
        sourceTemplate.ManufacturerSkipList.Add(skipItem);
        StateHasChanged();

        toasts.ShowSuccess("���������");
        await logger.Write(LogEntryGroupName.TemplateUpdate, "���������", $"���������� ������ '{newSkipManufacturerItem.name}' ({skipItem.ListTypeDescription}) ��� ������� {sourceTemplate.Title}");

    }

    private async Task RemoveSkipManufacturerRecord(ManufacturerSkipItemViewModel skipInfo)
    {
        await templateStorage.RemoveSkipManufacturerRecord(sourceTemplate.Guid, skipInfo.manufacturer_id, skipInfo.ListType.ToString());
        sourceTemplate.ManufacturerSkipList.Remove(skipInfo);
        StateHasChanged();

        toasts.ShowSuccess("���������");
        await logger.Write(LogEntryGroupName.TemplateUpdate, "������", $"���������� ������ '{skipInfo.Name}' ({skipInfo.ListTypeDescription}) �� ������� {sourceTemplate.Title}");
    }

}
