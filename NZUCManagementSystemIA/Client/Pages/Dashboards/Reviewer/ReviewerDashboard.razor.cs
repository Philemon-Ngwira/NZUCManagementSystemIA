using Microsoft.AspNetCore.Components;
using MudBlazor;
using NZUCManagementSystemIA.Client.Interfaces;
using NZUCManagementSystemIA.Shared.MailingSystem;
using NZUCManagementSystemIA.Shared.Models;

namespace NZUCManagementSystemIA.Client.Pages.Dashboards.Reviewer
{
    public class ReviewerDashboardBase : ComponentBase
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
        protected IList<TransactionStatus> _transactionStatuses = new List<TransactionStatus>();
        protected ReviewTransactionTable ReviewTransactionTable = new();
        protected TransactionTable TransactionTable = new();
        protected OperatingIncomeExpense OperatingIncomeExpense = new OperatingIncomeExpense();
        protected EmailDto email = new();
        protected TransactionStatus status = new();
        protected string CFOMail = string.Empty;
        protected string OperatingIncomeExpenseName = string.Empty;
        protected int? totalOperatingIncome;
        protected int? totalOperatingExpense;
        protected string operatingIncomeDisplay = string.Empty;
        protected string operatingExpenseDisplay = string.Empty;
        protected string searchString = "";
        protected int totalItems = 0;
        protected MudTable<ReviewTransactionTable> table;
        protected IEnumerable<ReviewTransactionTable> PagedData;
        [Inject] NavigationManager Navigation { get; set; }
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Inject] IGenericRepositoryService _genericRepositoryService { get; set; }
        [Inject] IDialogService Dialog { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] IMailingServiceClient MailingServiceClient { get; set; }
        [Inject] HttpClient Http { get; set; }

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

        protected async Task OpenEditDialog(ReviewTransactionTable Transaction)
        {
            DialogOptions dialogOptions = new DialogOptions() { Position = DialogPosition.Center, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var parameters = new DialogParameters { ["Transaction"] = Transaction };
            await GetDepartmentsAsync();
            await GetEmployeesAsync();
            await GetPaymentMethodsAsync();
            await GetPaymentTypesAsync();
            await GetReviewersAsync();

            var dialog = Dialog.Show<ReviewerReviewTransactionPage>("", parameters, dialogOptions);
            var result = await dialog.Result;

        }
        protected void OnRowClicked(TableRowClickEventArgs<ReviewTransactionTable> tableRowClickEventArgs)
        {
            ReviewTransactionTable = tableRowClickEventArgs.Item;
            OpenEditDialog(ReviewTransactionTable).GetAwaiter();
        }

        protected async void Submit()
        {
            //Assigning Values
            if (ReviewTransactionTable.Employee != null)
            {
                ReviewTransactionTable.EmployeeId = ReviewTransactionTable.Employee.Id;
                ReviewTransactionTable.DepartmentId = ReviewTransactionTable.Employee.DepartmentId;
                TransactionTable.EmployeeId = ReviewTransactionTable.EmployeeId;

            }
            ReviewTransactionTable.PaymentMethodId = ReviewTransactionTable.PaymentMethod.Id;
            ReviewTransactionTable.PaymentTypeId = ReviewTransactionTable.PaymentType.Id;
            ReviewTransactionTable.ReviewerId = ReviewTransactionTable.Reviewer.Id;
            ReviewTransactionTable.Status = ReviewTransactionTable.StatusNavigation.ID;
            if (ReviewTransactionTable.Employee == null)
            {
                ReviewTransactionTable.DepartmentId = ReviewTransactionTable.Department.Id;
            }
            //Email
            if (ReviewTransactionTable.Employee != null)
            {
                var body = "Transaction Made by:" + ReviewTransactionTable.Reviewer.Employee.EmployeeName + "," + "Payment Method:" + ReviewTransactionTable.PaymentMethod.PaymentMethod + ","
                          + "Amount : " + "ZMW" + ReviewTransactionTable.Amount + "," + "Payed To:" + ReviewTransactionTable.Employee.EmployeeName
                           + "," + "Reason For Payemnt:" + ReviewTransactionTable.Reason + "," + "Date Paid:" + ReviewTransactionTable.DateIssued;
                email.EmailBody = body.ToString();
            }
            else if (ReviewTransactionTable.Employee == null)
            {
                var body = "Transaction Made by:" + ReviewTransactionTable.Reviewer.Employee.EmployeeName + "," + "Payment Method:" + ReviewTransactionTable.PaymentMethod.PaymentMethod + ","
                          + "Amount : " + "ZMW" + ReviewTransactionTable.Amount + "," + "Payed To:" + ReviewTransactionTable.Department.DepartmentName
                           + "," + "Reason For Payemnt:" + ReviewTransactionTable.Reason + "," + "Date Paid:" + ReviewTransactionTable.DateIssued; ;
                email.EmailBody = body.ToString();
            }

            CFOMail = "ngwiram@nzu.adventist.org";
            email.EmailSubject = ReviewTransactionTable.PaymentType.PaymentType1;
            email.To = CFOMail;

            //Nulling Virtual Objects
            ReviewTransactionTable.Employee = null;
            ReviewTransactionTable.PaymentMethod = null;
            ReviewTransactionTable.Reviewer = null;
            ReviewTransactionTable.Department = null;
            ReviewTransactionTable.StatusNavigation = null;
            ReviewTransactionTable.PaymentType = null;
            var deleteId = ReviewTransactionTable.id;
            await Http.DeleteAsync($"api/NZUCManagement/DeleteReviewTransaction/{deleteId}");
            MailingServiceClient.SendMailAsync("api/Mailing/SendEmail", email);
            SavePermanentTransaction();
            Snackbar.Add("Transaction Closed !", Severity.Success);
            MudDialog.Close();
            Navigation.NavigateTo("/Pages/Dashboards/Reviewer/ReviewerDashboard");

        }
        protected async void Canceled()
        {
            //check if Authorized != null
            if (ReviewTransactionTable.Authorized != null)
            {
                if (ReviewTransactionTable.Employee != null)
                {
                    ReviewTransactionTable.EmployeeId = ReviewTransactionTable.Employee.Id;
                    ReviewTransactionTable.DepartmentId = ReviewTransactionTable.Employee.DepartmentId;
                    TransactionTable.EmployeeId = ReviewTransactionTable.EmployeeId;

                }
                ReviewTransactionTable.PaymentMethodId = ReviewTransactionTable.PaymentMethod.Id;
                ReviewTransactionTable.PaymentTypeId = ReviewTransactionTable.PaymentType.Id;
                ReviewTransactionTable.ReviewerId = ReviewTransactionTable.Reviewer.Id;
                ReviewTransactionTable.Status = ReviewTransactionTable.StatusNavigation.ID;
                if (ReviewTransactionTable.Employee == null)
                {
                    ReviewTransactionTable.DepartmentId = ReviewTransactionTable.Department.Id;
                }
                ReviewTransactionTable.Employee = null;
                ReviewTransactionTable.PaymentMethod = null;
                ReviewTransactionTable.Reviewer = null;
                ReviewTransactionTable.Department = null;
                ReviewTransactionTable.StatusNavigation = null;
                ReviewTransactionTable.PaymentType = null;
                var deleteId = ReviewTransactionTable.id;
                await Http.DeleteAsync($"api/NZUCManagement/DeleteReviewTransaction/{deleteId}");
                Snackbar.Add("Transaction Closed !", Severity.Success);
                MudDialog.Close();
            }
            else
            {
                MudDialog.Close();
                Snackbar.Add("Canceled !", Severity.Error);
            }
            //if Authorized != null
        }
        protected async void SavePermanentTransaction()
        {
            TransactionTable.Amount = ReviewTransactionTable.Amount;
            TransactionTable.ReviewerId = ReviewTransactionTable.ReviewerId;
            TransactionTable.DepartmentID = ReviewTransactionTable.DepartmentId;
            TransactionTable.PaymentMethodId = ReviewTransactionTable.PaymentMethodId;
            TransactionTable.Status = ReviewTransactionTable.Status;
            TransactionTable.DateIssued = ReviewTransactionTable.DateIssued;
            TransactionTable.Reason = ReviewTransactionTable.Reason;
            TransactionTable.AuthorizationState = ReviewTransactionTable.Authorized;
            OperatingIncomeExpense.Amount = TransactionTable.Amount;
            OperatingIncomeExpense.ReviewerId = TransactionTable.ReviewerId;
            OperatingIncomeExpense.IncomeExpenseOperatingType = 2;
            OperatingIncomeExpense.DateMonth = ReviewTransactionTable.DateIssued;
            OperatingIncomeExpense.DepartmentId = ReviewTransactionTable.DepartmentId;
            await _genericRepositoryService.SaveAllAsync("api/NZUCManagement/SavePermanentTransaction", TransactionTable);
            await _genericRepositoryService.SaveAllAsync("api/NZUCManagement/SaveOperatingIncomeExpense", OperatingIncomeExpense);
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
