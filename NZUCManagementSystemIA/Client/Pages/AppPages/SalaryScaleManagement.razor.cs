using Microsoft.AspNetCore.Components;
using MudBlazor;
using NZUCManagementSystemIA.Client.Interfaces;
using NZUCManagementSystemIA.Client.Pages.AppPages.Dialogues;
using NZUCManagementSystemIA.Shared.Models;

namespace NZUCManagementSystemIA.Client.Pages.AppPages
{
    public class SalaryScaleManagementBase : ComponentBase
    {
        protected IList<SalaryTable> _salaries = new List<SalaryTable>();
        protected SalaryTable salaryTable = new();
        protected int totalitems;
        protected string searchString = "";
        protected IEnumerable<SalaryTable> PagedData;
        protected MudTable<SalaryTable> table;
        [Inject] IGenericRepositoryService _genericRepositoryService { get; set; }
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Inject] IDialogService Dialog { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        protected int ItemDeleted;
        protected override async Task OnInitializedAsync()
        {
            await GetSalaries();
        }

        protected async Task GetSalaries()
        {
            var result = await _genericRepositoryService.GetAllAsync<SalaryTable>("api/NZUCManagement/GetSalaries");
            _salaries = result.ToList();
        }

        protected void OnRowClicked(TableRowClickEventArgs<SalaryTable> args)
        {
            salaryTable = args.Item;
            OpenEditDialog(salaryTable).GetAwaiter();
        }

        protected async Task OpenDialog(SalaryTable salary)
        {
            DialogOptions dialogOptions = new DialogOptions() { Position = DialogPosition.Center, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var parameters = new DialogParameters { ["salary"] = salary };

            var dialog = Dialog.Show<AddSalaryScaleDialog>("", parameters, dialogOptions);
            var result = await dialog.Result;

        }
        protected async Task OpenEditDialog(SalaryTable salary)
        {
            DialogOptions dialogOptions = new DialogOptions() { Position = DialogPosition.Center, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var parameters = new DialogParameters { ["salary"] = salary };

            var dialog = Dialog.Show<EditSalaryScaleDialog>("", parameters, dialogOptions);
            var result = await dialog.Result;

        }

        protected void DeleteUser(int id)
        {
            
            var parameters = new DialogParameters();
            parameters.Add("ContentText", "Do you really want to delete these records? This process cannot be undone.");
            parameters.Add("ButtonText", "Delete");
            parameters.Add("Color", Color.Error);

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

            Dialog.Show<DeleteSalaryScaleDialog>("Delete", parameters, options);
            
        }
        protected async void DeleteData(int id)
        {
            salaryTable = _salaries.FirstOrDefault(x => x.Id == id);
            ItemDeleted = id;
            await _genericRepositoryService.DeleteAsync($"api/NZUCManagement/DeleteSalaryScale/{id}");
        }
        protected void Cancel()
        {
            MudDialog.Close();
            Snackbar.Add("Canceled!", Severity.Info);
        }

        #region Table Code
        protected async Task<TableData<SalaryTable>> ServerReload(TableState state)
        {
            IEnumerable<SalaryTable> data = await _genericRepositoryService.GetAllAsync<SalaryTable>("api/NZUCManagement/GetSalaries");
            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.SalaryTier.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            }).ToArray();
            totalitems = data.Count();
            switch (state.SortLabel)
            {
                case "ID_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.Id);
                    break;
                case "Scale_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.SalaryTier);
                    break;

            }

            PagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
            return new TableData<SalaryTable>() { TotalItems = totalitems, Items = PagedData };
        }

        protected void OnSearch(string text)
        {
            searchString = text;
            table.ReloadServerData();
        }
        #endregion
    }

}
