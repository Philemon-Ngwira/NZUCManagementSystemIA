using Microsoft.AspNetCore.Components;
using MudBlazor;
using NZUCManagementSystemIA.Client.Interfaces;
using NZUCManagementSystemIA.Client.Pages.AppPages.Dialogues;
using NZUCManagementSystemIA.Shared.Models;

namespace NZUCManagementSystemIA.Client.Pages.AppPages
{
    public class DepartmentManagementBase : ComponentBase
    {
        protected IList<Departments_Table> _departments = new List<Departments_Table>();
        protected Departments_Table departments = new();
        [Inject] IGenericRepositoryService _repositoryService { get; set; }
        [Inject] IDialogService Dialog { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        [CascadingParameter] MudDialogInstance DialogInstance { get; set; }
        [Inject] HttpClient Http { get; set; }
        [Inject] NavigationManager Navigation { get; set; }

        protected string? searchString;
        protected int totalItems;
        protected IEnumerable<Departments_Table>? PagedData;
        protected MudTable<Departments_Table>? table;

        protected override async Task OnInitializedAsync()
        {
            await GetDepartment();
        }

        private async Task GetDepartment()
        {
            var result = await _repositoryService.GetAllAsync<Departments_Table>("api/NZUCManagement/GetDepartments");
            _departments = result.ToList();
        }


        #region Table Code
        protected async Task<TableData<Departments_Table>> ServerReload(TableState state)
        {
            IEnumerable<Departments_Table> data = await _repositoryService.GetAllAsync<Departments_Table>("api/NZUCManagement/GetDepartments");
            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.DepartmentName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.DepartmentCode.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            }).ToArray();
            totalItems = data.Count();
            switch (state.SortLabel)
            {
                case "ID_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.Id);
                    break;
                case "DepartmentName_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.DepartmentName);
                    break;
                case "DepartmentCode_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.DepartmentCode);
                    break;

            }

            PagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
            return new TableData<Departments_Table>() { TotalItems = totalItems, Items = PagedData };
        }

        protected void OnSearch(string text)
        {
            searchString = text;
            table.ReloadServerData();
        }

        protected async Task OpenDialog(Departments_Table department)
        {
            DialogOptions dialogOptions = new DialogOptions() { Position = DialogPosition.Center, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var parameters = new DialogParameters { ["department"] = department };

            var dialog = Dialog.Show<AddDepartmentDialog>("", parameters, dialogOptions);
            var result = await dialog.Result;

        }
        protected void onRowClicked(TableRowClickEventArgs<Departments_Table> eventArgs)
        {
            departments = eventArgs.Item;
            OpenEditDialog(departments).GetAwaiter();
        }
        protected async Task OpenEditDialog(Departments_Table departments)
        {
            DialogOptions dialogOptions = new DialogOptions() { Position = DialogPosition.Center, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var parameters = new DialogParameters { ["department"] = departments };

            var dialog = Dialog.Show<EditDepartmentDialog>("", parameters, dialogOptions);
            var result = await dialog.Result;

        }

        protected async void DeleteItem(int id)
        {
            await Http.DeleteAsync($"api/NZUCManagement/DeleteDepartment/{id}");
            Snackbar.Add("Successfully Deleted", Severity.Error);
            Navigation.NavigateTo("Pages/AppPages/DepartmentManagement", forceLoad: true);
        }
        #endregion
    }
}
