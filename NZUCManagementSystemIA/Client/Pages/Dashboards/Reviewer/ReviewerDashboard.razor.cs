using Microsoft.AspNetCore.Components;
using MudBlazor;
using NZUCManagementSystemIA.Client.Interfaces;
using NZUCManagementSystemIA.Shared.Models;

namespace NZUCManagementSystemIA.Client.Pages.Dashboards.Reviewer
{
    public class ReviewerDashboardBase :ComponentBase
    {
        #region Lists for Dialog
        protected IList<EmployeeTable> _employees = new List<EmployeeTable>();
        protected IList<Departments_Table> _departments_Tables = new List<Departments_Table>();
        protected IList<ReviewersTable> _reviewersTables = new List<ReviewersTable>();
        protected IList<PaymentMethodTable> _paymentMethods = new List<PaymentMethodTable>();
        protected IList<PaymentType> _paymentTypes = new List<PaymentType>();
        #endregion
        protected IList<OperatingIncomeExpense> _OperatingIncomeExpenses = new List<OperatingIncomeExpense>();
        protected IList<ReviewTransactionTable> _transactions = new List<ReviewTransactionTable>();
        protected string OperatingIncomeExpenseName = string.Empty;
        protected int? totalOperatingIncome;
        protected int? totalOperatingExpense;
        protected string operatingIncomeDisplay = string.Empty;
        protected string operatingExpenseDisplay = string.Empty;
        protected string searchString = "";
        protected int totalItems = 0;
        protected MudTable<ReviewTransactionTable> table;
        protected IEnumerable<ReviewTransactionTable> PagedData;
        [Inject] NavigationManager Navigation { get;set; }
        [Inject] IGenericRepositoryService _genericRepositoryService { get; set; }


        protected override async Task OnInitializedAsync()
        {
            await GetCurrentCash();
        }
        protected void NavigateToPaymentPage()
        {
            Navigation.NavigateTo("/Pages/AppPages/PaymentsForm");

        }
        protected void NavigateToOperatingIncome()
        {
            Navigation.NavigateTo("/Pages/AppPages/OperatingIncomeExpenseManagement");
        }

        protected async Task GetCurrentCash()
        {
            var result = await _genericRepositoryService.GetAllAsync<OperatingIncomeExpense>("api/NZUCManagement/GetOperatingIncomeExpense");
            _OperatingIncomeExpenses = result.ToList();
            OperatingIncomeExpenseName = "Income";
            totalOperatingIncome = _OperatingIncomeExpenses.Where(x => x.IncomeExpenseOperatingTypeNavigation.OperatingType1 == OperatingIncomeExpenseName).Sum(x => x.Amount);
            operatingIncomeDisplay = $"{totalOperatingIncome:n0}";
            OperatingIncomeExpenseName = "Expense";
            totalOperatingExpense = _OperatingIncomeExpenses.Where(x => x.IncomeExpenseOperatingTypeNavigation.OperatingType1 == OperatingIncomeExpenseName).Sum(x => x.Amount);
            operatingExpenseDisplay = $"{totalOperatingExpense:n0}";
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
        #region Table Code

        protected async Task GetTransactions()
        {
            var result = await _genericRepositoryService.GetAllAsync<ReviewTransactionTable>("api/NZUCManagement/GetReviewerTransactions");
            _transactions = result.ToList();
        }
        protected async Task<TableData<ReviewTransactionTable>> ServerReload(TableState state)
        {
            IEnumerable<ReviewTransactionTable> data = await _genericRepositoryService.GetAllAsync<ReviewTransactionTable>("api/NZUCManagement/GetReviewerTransactions");
            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.Department.DepartmentName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Employee.EmployeeName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            }).ToArray();
            totalItems = data.Count();
            switch (state.SortLabel)
            {
                case "ID_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.id);
                    break;
                case "Amount_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.Amount);
                    break;
                case "PaymentMethod_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.PaymentMethod.PaymentMethod);
                    break;
                case "PaymentType_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.PaymentType.PaymentType1);
                    break;

            }

            PagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
            return new TableData<ReviewTransactionTable>() { TotalItems = totalItems, Items = PagedData };
        }

        protected void OnSearch(string text)
        {
            searchString = text;
            table.ReloadServerData();
        }

       
       
        #region String Functions
        protected Func<PaymentType, string> PaymentTypeconverter = p => p?.PaymentType1;
        protected Func<EmployeeTable, string> Employeeconverter = p => p?.EmployeeName;
        protected Func<PaymentMethodTable, string> PaymentMethodconverter = p => p?.PaymentMethod;
        protected Func<Departments_Table, string> Departmentconverter = p => p?.DepartmentName;
        protected Func<ReviewersTable, string> Reviewerconverter = p => p?.Employee.EmployeeName;
        #endregion
        #endregion
    }
}
