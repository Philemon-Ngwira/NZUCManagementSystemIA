using Microsoft.AspNetCore.Components;
using MudBlazor;
using NZUCManagementSystemIA.Client.Interfaces;
using NZUCManagementSystemIA.Shared.Models;

namespace NZUCManagementSystemIA.Client.Pages.AppPages
{
    public class AddNewBudgetBase : ComponentBase
    {
        protected YearlyBudgetTable yearlyBudget = new();
        protected IList<BudgetTypeTable> _budgettypes = new List<BudgetTypeTable>();
        protected IList<IncomeExpenseBudgetType> _incomeExpenses = new List<IncomeExpenseBudgetType>();
        protected IncomeExpenseBudgetType _incomeExpensesType = new();
        [Inject] IGenericRepositoryService _genericRepositoryService { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        string IncomeExpenseName;
        int budgetTypeId;
        string BudgetTypeName;


        protected override async Task OnInitializedAsync()
        {
            await GetBudgetTypes();
        }

        private async Task GetBudgetTypes()
        {
            var result = await _genericRepositoryService.GetAllAsync<BudgetTypeTable>("api/NZUCManagement/GetBudgetTypes");
            _budgettypes = result.ToList();
        }

        protected async Task OnBudgetTypeClicked()
        {
            _incomeExpenses.Clear();
            IncomeExpenseName = string.Empty;
            budgetTypeId = _incomeExpensesType.TypeofBudget.Id;
            BudgetTypeName = _budgettypes.FirstOrDefault(x => x.Id == budgetTypeId).BudgetType;
            var result = await _genericRepositoryService.GetByIdAsync<IncomeExpenseBudgetType>($"api/NZUCManagement/GetIncomeExpenseByBudgetTypeId/{budgetTypeId}");
            _incomeExpenses = result.ToList();

        }
        protected async void OnSubmit()
        {
            yearlyBudget.IncomeExpesenseId = _incomeExpensesType.Id;
            await _genericRepositoryService.SaveAllAsync("api/NZUCManagement/SaveYearlyBudget", yearlyBudget);
            Snackbar.Add("Budget Saved" ,Severity.Success);
        }
        protected void Cancel()
        {
            yearlyBudget = null;
        }
        #region stringFunc
        protected Func<IncomeExpenseBudgetType, string> IncomeExpesnseconverter = p => p?.IncomeBudgetType;
        #endregion

    }
}
