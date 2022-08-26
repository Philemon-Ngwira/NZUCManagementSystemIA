using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MudBlazor;
using NZUCManagementSystemIA.Client.Interfaces;
using NZUCManagementSystemIA.Client.Pages.AppPages.Dialogues;
using NZUCManagementSystemIA.Shared.Models;
using static System.Net.WebRequestMethods;

namespace NZUCManagementSystemIA.Client.Pages.AppPages
{
    public class EmployeeManagementBase : ComponentBase
    {
        protected IList<EmployeeTable> _employeeslist = new List<EmployeeTable>();
        protected IList<GenderTable> _genderlist = new List<GenderTable>();
        protected IList<Departments_Table> _departmentslist = new List<Departments_Table>();
        protected IList<SalaryTable> _salaryScalelist = new List<SalaryTable>();

        protected EmployeeTable employee = new();
        protected EmployeeTable dialogemployee = new();
        [Inject] IGenericRepositoryService _genericRepository { get; set; }
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Inject] IDialogService Dialog { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] HttpClient Http { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        protected string searchString = null;
        protected int totalemployees;
        protected MudTable<EmployeeTable> table;
        protected IEnumerable<EmployeeTable> pagedData;


        protected override async Task OnInitializedAsync()
        {
            await GetEmployees();
            await GetGenders();
            await GetDepartments();
            await GetSalaries(); 
        }

        #region GetFunctions
        protected async Task GetGenders()
        {
            var result = await _genericRepository.GetAllAsync<GenderTable>("api/NZUCManagement/GetGenders");
            _genderlist = result.ToList();
        }

        protected async Task GetDepartments()
        {
            var result = await _genericRepository.GetAllAsync<Departments_Table>("api/NZUCManagement/GetDepartments");
            _departmentslist = result.ToList();
        }
        protected async Task GetSalaries()
        {
            var result = await _genericRepository.GetAllAsync<SalaryTable>("api/NZUCManagement/GetSalaries");
            _salaryScalelist = result.ToList();
        }
        protected async Task GetEmployees()
        {
            var result = await _genericRepository.GetAllAsync<EmployeeTable>("api/NZUCManagement/GetEmployees");
            _employeeslist = result.ToList();
        }
        #endregion

        #region Other Functionality
        protected void OnTableRowClicked(TableRowClickEventArgs<EmployeeTable> tableRow)
        {
            employee = tableRow.Item;
            OpenStoreOperatorDialog(employee).GetAwaiter();
        }
        protected void SubmitEdit()
        {
            MudDialog.Close(DialogResult.Ok(employee));
            save();
        }
        private async void save()
        {
            try
            {
                employee.DepartmentId = employee.Department.Id;
                employee.GenderId = employee.Gender.Id;
                employee.Salary = employee.SalaryNavigation.Id;

                employee.SalaryNavigation = null;
                employee.Gender = null;
                employee.Department = null;

                await _genericRepository.UpdateAsync("api/NZUCManagement/UpdateEmployee", employee);
                Snackbar.Add("Succefully Updated", Severity.Success);
                Navigation.NavigateTo("Pages/AppPages/EmployeeManagement", forceLoad: true);

            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
        }

        protected void Cancel()
        {
            MudDialog.Close();
            Snackbar.Add("Canceled!", Severity.Info);
        }


        async Task OpenStoreOperatorDialog(EmployeeTable employee)
        {
            DialogOptions dialogOptions = new DialogOptions() { Position = DialogPosition.Center, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var parameters = new DialogParameters { ["employee"] = employee };

            var dialog = Dialog.Show<EditEmployeeDialog>("", parameters, dialogOptions);
            var result = await dialog.Result;

        }
        #endregion

        #region Table Code
        protected async Task<TableData<EmployeeTable>> ServerReload(TableState state)
        {
            IEnumerable<EmployeeTable> data = await _genericRepository.GetAllAsync<EmployeeTable>("api/NZUCManagement/GetEmployees");
            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.EmployeeName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.EmployeeID.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if ($"{element.Department}".Contains(searchString))
                    return true;
                return false;
            }).ToArray();
            totalemployees = data.Count();
            switch (state.SortLabel)
            {
                case "ID_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.Id);
                    break;
                case "CompanyID_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.EmployeeID);
                    break;
                case "Name_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.EmployeeName);
                    break;
                case "Department_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.Department.DepartmentName);
                    break;
                case "Gender_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.Gender.Gender);
                    break;
                case "Salary_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.SalaryNavigation.SalaryTier);
                    break;
            }

            pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
            return new TableData<EmployeeTable>() { TotalItems = totalemployees, Items = pagedData };
        }

        protected void OnSearch(string text)
        {
            searchString = text;
            table.ReloadServerData();
        }
        #endregion

        #region StringFunctions
        protected Func<GenderTable, string> Genderconverter = p => p?.Gender;
        protected Func<Departments_Table, string> Departmentconverter = p => p?.DepartmentName;
        protected Func<SalaryTable, string> Salaryconverter = p => p?.SalaryTier;
        #endregion

        protected async void DeleteItem(int id)
        {
            await Http.DeleteAsync($"api/NZUCManagement/DeleteEmployee/{id}");
            Snackbar.Add("Successfully Deleted", Severity.Error);
            Navigation.NavigateTo("Pages/AppPages/EmployeeManagement", forceLoad: true);
        }
    }
}
