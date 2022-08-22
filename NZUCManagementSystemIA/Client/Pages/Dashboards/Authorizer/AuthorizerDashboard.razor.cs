using Microsoft.AspNetCore.Components;
using MudBlazor;
using NZUCManagementSystemIA.Shared.Models;
using NZUCManagementSystemIA.Client.Interfaces;

namespace NZUCManagementSystemIA.Client.Pages.Dashboards.Authorizer
{
    public partial class AuthorizerDashboardBase : ComponentBase
    {
        #region Lists for Dialog
        protected IList<EmployeeTable> _employees = new List<EmployeeTable>();
        protected IList<Departments_Table> _departments_Tables = new List<Departments_Table>();
        protected IList<ReviewersTable> _reviewersTables = new List<ReviewersTable>();
        protected IList<PaymentMethodTable> _paymentMethods = new List<PaymentMethodTable>();
        protected IList<PaymentType> _paymentTypes = new List<PaymentType>();
        #endregion

        protected ReviewTransactionTable ReviewTransactionTable = new();
        protected IList<ReviewTransactionTable> _transactions = new List<ReviewTransactionTable>();
        protected IList<ReviewTransactionTable> _transactionsDisplay = new List<ReviewTransactionTable>();
        protected IEnumerable<ReviewTransactionTable> PagedData;
        [Inject] IGenericRepositoryService _genericRepositoryService { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] IDialogService Dialog { get; set; }
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        protected string searchString = "";
        protected string AuthorizerName = "Ngwira";
        protected int totalItems = 0;
        protected MudTable<ReviewTransactionTable> table;


        protected override async Task OnInitializedAsync()
        {
            await GetTransactions();

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

        //protected void OnRowClicked(TableRowClickEventArgs<ReviewTransactionTable> tableRow)
        //{
        //    yearlyBudget = tableRow.Item;
        //    OpenDialog(yearlyBudget).GetAwaiter();
        //}

        protected async Task OpenDialog(int id)
        {
            ReviewTransactionTable = _transactions.FirstOrDefault(x => x.id == id);
            DialogOptions dialogOptions = new DialogOptions() { Position = DialogPosition.Center, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var parameters = new DialogParameters { ["Transaction"] = ReviewTransactionTable };
            await GetPaymentTypesAsync();
            await GetEmployeesAsync();
            await GetPaymentMethodsAsync();
            await GetReviewersAsync();
            await GetDepartmentsAsync();

            var dialog = Dialog.Show<AuthorizerReviewTransactionDialog>("", parameters, dialogOptions);
            var result = await dialog.Result;

        }
        #region String Functions
        protected Func<PaymentType, string> PaymentTypeconverter = p => p?.PaymentType1;
        protected Func<EmployeeTable, string> Employeeconverter = p => p?.EmployeeName;
        protected Func<PaymentMethodTable, string> PaymentMethodconverter = p => p?.PaymentMethod;
        protected Func<Departments_Table, string> Departmentconverter = p => p?.DepartmentName;
        protected Func<ReviewersTable, string> Reviewerconverter = p => p?.Employee.EmployeeName;
        #endregion
        #endregion

        protected void Authorize()
        {
            if (ReviewTransactionTable.Employee != null)
            {
                ReviewTransactionTable.EmployeeId = ReviewTransactionTable.Employee.Id;
                ReviewTransactionTable.DepartmentId = ReviewTransactionTable.Employee.DepartmentId;
            }

            ReviewTransactionTable.PaymentMethodId = ReviewTransactionTable.PaymentMethod.Id;
            ReviewTransactionTable.PaymentTypeId = ReviewTransactionTable.PaymentType.Id;
            ReviewTransactionTable.ReviewerId = ReviewTransactionTable.Reviewer.Id;
            if (ReviewTransactionTable.Employee == null)
            {
                ReviewTransactionTable.DepartmentId = ReviewTransactionTable.Department.Id;
            }

            ReviewTransactionTable.Employee = null;
            ReviewTransactionTable.PaymentMethod = null;
            ReviewTransactionTable.Reviewer = null;
            ReviewTransactionTable.Department = null;
            ReviewTransactionTable.PaymentType = null;
            _genericRepositoryService.UpdateAsync<ReviewTransactionTable>("api/NZUCManagement/UpdateTransaction", ReviewTransactionTable);
            MudDialog.Close();
            Snackbar.Add("Approved!", Severity.Success);

        }
        protected void Reject()
        {
            if (ReviewTransactionTable.Employee != null)
            {
                ReviewTransactionTable.EmployeeId = ReviewTransactionTable.Employee.Id;
                ReviewTransactionTable.DepartmentId = ReviewTransactionTable.Employee.DepartmentId;
            }

            ReviewTransactionTable.PaymentMethodId = ReviewTransactionTable.PaymentMethod.Id;
            ReviewTransactionTable.PaymentTypeId = ReviewTransactionTable.PaymentType.Id;
            ReviewTransactionTable.ReviewerId = ReviewTransactionTable.Reviewer.Id;
            if (ReviewTransactionTable.Employee == null)
            {
                ReviewTransactionTable.DepartmentId = ReviewTransactionTable.Department.Id;
            }

            ReviewTransactionTable.Employee = null;
            ReviewTransactionTable.PaymentMethod = null;
            ReviewTransactionTable.Reviewer = null;
            ReviewTransactionTable.Department = null;
            ReviewTransactionTable.PaymentType = null;
            _genericRepositoryService.UpdateAsync<ReviewTransactionTable>("api/NZUCManagement/UpdateTransaction", ReviewTransactionTable);
            MudDialog.Close();
            Snackbar.Add("Rejected!", Severity.Error);

        }
    }
}