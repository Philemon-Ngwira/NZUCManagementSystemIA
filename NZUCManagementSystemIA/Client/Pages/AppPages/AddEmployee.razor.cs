using Microsoft.AspNetCore.Components;
using MudBlazor;
using NZUCManagementSystemIA.Client.Interfaces;
using NZUCManagementSystemIA.Shared.Models;

namespace NZUCManagementSystemIA.Client.Pages.AppPages
{
    public class AddEmployeeBase : ComponentBase
    {
        #region Varaiables
        protected EmployeeTable employee = new();
        protected IList<EmployeeTable> _employeeslist = new List<EmployeeTable>();
        protected IList<GenderTable> _genderlist = new List<GenderTable>();
        protected IList<Departments_Table> _departmentslist = new List<Departments_Table>();
        protected IList<SalaryTable> _salaryScalelist = new List<SalaryTable>();
        [Inject] IGenericRepositoryService? _genericRepository { get; set; }
        [Inject] ISnackbar? Snackbar { get; set; }
        protected bool Success = false;
        #endregion

        #region CodeLogic
        protected override async Task OnInitializedAsync()
        {
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
        #endregion

        protected async void OnValidSubmit()
        {
            employee.Salary = employee.SalaryNavigation.Id;
            employee.DepartmentId = employee.Department.Id;
            employee.GenderId = employee.Gender.Id;
            employee.Gender = null;
            employee.Department = null;
            employee.SalaryNavigation = null;
            Success = !Success;
            await _genericRepository.SaveAllAsync("api/NZUCManagement/SaveEmployee", employee);
            Snackbar.Add("Submitted", Severity.Success);
            employee = new();
        }

        

        #endregion

    }

}
