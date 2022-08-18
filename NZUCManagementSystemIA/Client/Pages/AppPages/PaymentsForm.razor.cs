using Microsoft.AspNetCore.Components;
using MudBlazor;
using NZUCManagementSystemIA.Shared.Models;
using NZUCManagementSystemIA.Client.Interfaces;

namespace NZUCManagementSystemIA.Client.Pages.AppPages
{
    public partial class PaymentsFormBase : ComponentBase 
    {
        #region Variables
        #region ModelInstances
        protected ReviewTransactionTable ReviewTransactionTable = new ReviewTransactionTable();
        protected EmployeeTable employee = new();
        protected Departments_Table departments = new();
        protected ReviewersTable reviewers = new();
        protected PaymentMethodTable paymentMethod = new PaymentMethodTable();
        protected PaymentType PaymentType = new PaymentType();
        #endregion
        #region Lists
        protected IList<EmployeeTable> _employees = new List<EmployeeTable>();
        protected IList<Departments_Table> _departments_Tables = new List<Departments_Table>();
        protected IList<ReviewersTable> _reviewersTables = new List<ReviewersTable>();
        protected IList<PaymentMethodTable> _paymentMethods = new List<PaymentMethodTable>();
        protected IList<PaymentType> _paymentTypes = new List<PaymentType>();
        #endregion
        #region Injections
        [Inject] IGenericRepositoryService _genericRepositoryService { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }
        #endregion
        protected bool IsSalary = false;
        #endregion



        protected override async Task OnInitializedAsync()
        {
            await GetPaymentTypesAsync();
            await GetEmployeesAsync();
            await GetPaymentMethodsAsync();
            await GetDepartmentsAsync();
            await GetReviewersAsync();

        }

        #region GetFunctions
        protected async Task GetEmployeesAsync()
        {
            var result = await _genericRepositoryService.GetAllAsync<EmployeeTable>("api/NZUCManagement/GetEmployees");
            _employees = result.ToList();
        }
        protected async Task GetDepartmentsAsync()
        {
            var result = await _genericRepositoryService.GetAllAsync<Departments_Table>("api/NZUCManagement/GetDepartments");
            _departments_Tables = result.ToList();
        }
        protected async Task GetReviewersAsync()
        {
            var result = await _genericRepositoryService.GetAllAsync<ReviewersTable>("api/NZUCManagement/GetReviewers");
            _reviewersTables = result.ToList();
        }
        protected async Task GetPaymentMethodsAsync()
        {
            var result = await _genericRepositoryService.GetAllAsync<PaymentMethodTable>("api/NZUCManagement/GetPaymentMethods");
            _paymentMethods = result.ToList();
        }
        protected async Task GetPaymentTypesAsync()
        {
            var result = await _genericRepositoryService.GetAllAsync<PaymentType>("api/NZUCManagement/GetPaymentTypes");
            _paymentTypes = result.ToList();
        }
        #endregion

        #region DropDownFunctionality
        protected void OnPaymentTypeSelected()
        {
            if (ReviewTransactionTable.PaymentType.Id == 1)
            {
                IsSalary = true;
            }
            else
            {
                IsSalary = false;
            }
        }
        #endregion
        #region Save
        protected async void Save()
        {
            if (IsSalary)
            {
                ReviewTransactionTable.EmployeeId = ReviewTransactionTable.Employee.Id;
                ReviewTransactionTable.PaymentMethodId = ReviewTransactionTable.PaymentMethod.Id;
                ReviewTransactionTable.PaymentTypeId = ReviewTransactionTable.PaymentType.Id;
                ReviewTransactionTable.ReviewerId = ReviewTransactionTable.Reviewer.Id;
                ReviewTransactionTable.DepartmentId = ReviewTransactionTable.Employee.DepartmentId;
                ReviewTransactionTable.Employee = null;
                ReviewTransactionTable.PaymentMethod = null;
                ReviewTransactionTable.Reviewer = null;
                ReviewTransactionTable.Department = null;
                ReviewTransactionTable.PaymentType = null;
                
                await _genericRepositoryService.SaveAllAsync("api/NZUCManagement/SaveReviewerTransaction", ReviewTransactionTable);
                Snackbar.Add("Successfully Submitted!",Severity.Success);
                NavigationManager.NavigateTo("/Pages/Dashboards/Reviewer/ReviewerDashboard");
            }
            else if (!IsSalary)
            {
                ReviewTransactionTable.DepartmentId = ReviewTransactionTable.Department.Id;
                ReviewTransactionTable.PaymentMethodId = ReviewTransactionTable.PaymentMethod.Id;
                ReviewTransactionTable.PaymentTypeId = ReviewTransactionTable.PaymentType.Id;
                ReviewTransactionTable.ReviewerId = ReviewTransactionTable.Reviewer.Id;
                ReviewTransactionTable.Employee = null;
                ReviewTransactionTable.PaymentMethod = null;
                ReviewTransactionTable.Reviewer = null;
                ReviewTransactionTable.Department = null;
                ReviewTransactionTable.PaymentType = null;

                
                await _genericRepositoryService.SaveAllAsync("api/NZUCManagement/SaveReviewerTransaction", ReviewTransactionTable);
                Snackbar.Add("Successfully Submitted!", Severity.Success);
                NavigationManager.NavigateTo("/Pages/Dashboards/Reviewer/ReviewerDashboard");

            }
        }
        #endregion

    }
}