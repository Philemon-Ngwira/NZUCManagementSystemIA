using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MudBlazor;
using NZUCManagementSystemIA.Client.Interfaces;
using NZUCManagementSystemIA.Client.Pages.AppPages.Dialogues;
using NZUCManagementSystemIA.Shared.Models;
using static System.Net.WebRequestMethods;

namespace NZUCManagementSystemIA.Client.Pages.AppPages
{
    public class ReviewersManagementBase : ComponentBase
    {
        protected IList<ReviewersTable> _reviewers { get; set; } = new List<ReviewersTable>();
        protected IList<EmployeeTable> _employees { get; set; } = new List<EmployeeTable>();
        protected ReviewersTable Reviewer  = new();
        protected EmployeeTable Employee = new EmployeeTable();
        protected GenderTable Gender = new GenderTable();
        protected Departments_Table Department = new Departments_Table();
        
        [Inject] IGenericRepositoryService _genericRepositoryService { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] IDialogService Dialog { get; set; }
        [Inject] HttpClient Http { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }
        protected MudTable<ReviewersTable> table;
        protected string searchString;
        protected int totalitems;
        
        protected IEnumerable<ReviewersTable> PagedData;

        protected override async Task OnInitializedAsync()
        {
            await GetReviewers();
            await GetEmployees();
        }

        protected async Task GetReviewers()
        {
            var result = await _genericRepositoryService.GetAllAsync<ReviewersTable>("api/NZUCManagement/GetReviewers");
            _reviewers = result.ToList();
        }
        protected async Task OpenDialog(ReviewersTable reviewers)
        {
            DialogOptions dialogOptions = new DialogOptions() { Position = DialogPosition.Center, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var parameters = new DialogParameters { ["reviewers"] = reviewers };

            var dialog = Dialog.Show<AddReviewerDialog>("", parameters, dialogOptions);
            var result = await dialog.Result;

        }
        protected async Task GetEmployees()
        {
            var result = await _genericRepositoryService.GetAllAsync<EmployeeTable>("api/NZUCManagement/GetEmployees");
            _employees = result.ToList();
            
        }
        protected void OnRowClicked(TableRowClickEventArgs<ReviewersTable> tableRow)
        {
            Reviewer = tableRow.Item;
        }
        #region Table Code
        protected async Task<TableData<ReviewersTable>> ServerReload(TableState state)
        {
            IEnumerable<ReviewersTable> data = await _genericRepositoryService.GetAllAsync<ReviewersTable>("api/NZUCManagement/GetReviewers");
            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.Employee.EmployeeName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Employee.EmployeeID.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            }).ToArray();
            totalitems = data.Count();
            switch (state.SortLabel)
            {
                case "ID_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.Id);
                    break;
                case "ReviewerName_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.Employee.EmployeeName);
                    break;
                case "ReviewerEmployeeID_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.Employee.EmployeeID);
                    break;
                case "Department_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.Employee.Department.DepartmentName);
                    break;

            }

            PagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
            return new TableData<ReviewersTable>() { TotalItems = totalitems, Items = PagedData };
        }

        protected void OnSearch(string text)
        {
            searchString = text;
            table.ReloadServerData();
        }
        #endregion
        #region String Functions
        protected Func<EmployeeTable, string> Employeeconverter = p => p?.EmployeeName;
        protected Func<Departments_Table, string> Departmentconverter = p => p?.DepartmentName;
        protected Func<GenderTable, string> Genderconverter = p => p?.Gender;
        #endregion
        #region Crud Operations
        protected async void SaveReviewer()
        {
            Reviewer.Employee = Employee;
            Reviewer.EmployeeId = Reviewer.Employee.Id;
            Reviewer.Employee = null;
            await _genericRepositoryService.SaveAllAsync("api/NZUCManagement/SaveReviewer", Reviewer);
            Snackbar.Add("Successfully Saved", Severity.Success);
            NavigationManager.NavigateTo("Pages/AppPages/ReviewersManagement", forceLoad: true);

        }
        protected async void DeleteReviewer(int id)
        {
            await Http.DeleteAsync($"api/NZUCManagement/DeleteOperatingIncomeExp/{id}");
            Snackbar.Add("Successfully Deleted", Severity.Error);
            NavigationManager.NavigateTo("Pages/AppPages/ReviewersManagement", forceLoad: true);
        }
        #endregion

    }
}
