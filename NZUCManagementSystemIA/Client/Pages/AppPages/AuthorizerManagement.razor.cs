using Microsoft.AspNetCore.Components;
using MudBlazor;
using NZUCManagementSystemIA.Client.Interfaces;
using NZUCManagementSystemIA.Client.Pages.AppPages.Dialogues;
using NZUCManagementSystemIA.Shared.Models;
using static System.Net.WebRequestMethods;

namespace NZUCManagementSystemIA.Client.Pages.AppPages
{
    public class AuthorizerManagementBase : ComponentBase
    {
        protected IList<AuthorizerTable> _authorizers = new List<AuthorizerTable>();
        protected IList<EmployeeTable> _employees = new List<EmployeeTable>();
        protected AuthorizerTable Authorizer = new();
        protected string searchString = "";
        protected int totalitems = 0;
        protected IEnumerable<AuthorizerTable> PagedData;
        protected MudTable<AuthorizerTable> table;
        protected EmployeeTable employee = new();
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Inject]IGenericRepositoryService _genericRepository { get; set; }
        [Inject]IDialogService DialogService { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] IDialogService Dialog { get; set; }
        [Inject] HttpClient Http { get; set; }
        NavigationManager Navigation;


        protected override async Task OnInitializedAsync()
        {
            await GetAuthorizers();
            await GetEmployees();
        }

        protected async Task GetAuthorizers()
        {
            var result = await _genericRepository.GetAllAsync<AuthorizerTable>("api/NZUCManagement/GetAuthorizers");
            _authorizers = result.ToList();
        }

          protected async Task OpenDialog(AuthorizerTable authorizer)
        {
            DialogOptions dialogOptions = new DialogOptions() { Position = DialogPosition.Center, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var parameters = new DialogParameters { ["authorizer"] = authorizer };

            var dialog = Dialog.Show<AddAuthorizer>("", parameters, dialogOptions);
            var result = await dialog.Result;

        }

        protected async void Submit()
        {
            
            await _genericRepository.SaveAllAsync("api/NZUCManagement/SaveAuthorizer",Authorizer);
            Snackbar.Add("Successfully Saved", Severity.Success);
            MudDialog.Close();
            Navigation.NavigateTo("/Pages/AppPages/AuthorizerManegement", forceLoad:true);
        }
        protected void Cancel()
        {
            MudDialog.Close();
            Snackbar.Add("Canceled", Severity.Error);
        }
        protected async Task GetEmployees()
        {
            var result = await _genericRepository.GetAllAsync<EmployeeTable>("api/NZUCManagement/GetEmployees");
            _employees = result.ToList();
        }
        #region Table Code
        protected async Task<TableData<AuthorizerTable>> ServerReload(TableState state)
        {
            IEnumerable<AuthorizerTable> data = await _genericRepository.GetAllAsync<AuthorizerTable>("api/NZUCManagement/GetAuthorizers");
            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.AuthorizerName.EmployeeName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.AuthorizerName.EmployeeID.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            }).ToArray();
            totalitems = data.Count();
            switch (state.SortLabel)
            {
                case "ID_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.Id);
                    break;
                case "AuthorizerName_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.AuthorizerName.EmployeeName);
                    break;
                case "AuthorizerEmployeeID_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.AuthorizerName.EmployeeID);
                    break;
                case "Department_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.AuthorizerName.Department.DepartmentName);
                    break;

            }

            PagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
            return new TableData<AuthorizerTable>() { TotalItems = totalitems, Items = PagedData };
        }

        protected void OnSearch(string text)
        {
            searchString = text;
            table.ReloadServerData();
        }

        protected async void Delete(int id)
        {
            await Http.DeleteAsync($"api/NZUCManagement/DeleteAuthorizer/{id}");
            Snackbar.Add("Successfully Deleted", Severity.Error);
            Navigation.NavigateTo("/Pages/AppPages/AuthorizerManagement", forceLoad: true);

        }
        #endregion

        protected Func<EmployeeTable, string> Employeeconverter = p => p?.EmployeeName;
    }
}
