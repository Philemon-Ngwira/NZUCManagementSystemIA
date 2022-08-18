using Microsoft.AspNetCore.Components;
using MudBlazor;
using NZUCManagementSystemIA.Client.Interfaces;
using NZUCManagementSystemIA.Client.Pages.AppPages.Dialogues;
using NZUCManagementSystemIA.Shared.Models;

namespace NZUCManagementSystemIA.Client.Pages.AppPages
{
    public class ManageBudgetsBase : ComponentBase
    {
        protected IList<YearlyBudgetTable> yearlyBudgets =  new List<YearlyBudgetTable>();
        protected IEnumerable<YearlyBudgetTable> PagedData;
        protected YearlyBudgetTable yearlyBudget;
        protected IList<BudgetTypeTable> _budgettypes = new List<BudgetTypeTable>();
        protected IList<IncomeExpenseBudgetType> _incomeExpenses = new List<IncomeExpenseBudgetType>();
        protected IncomeExpenseBudgetType _incomeExpensesType = new();
        [Inject] IGenericRepositoryService _genericRepositoryService { get; set; }
        [Inject] IDialogService Dialog { get; set; }
        protected string searchString ="";
        protected int totalItems;
        protected MudTable<YearlyBudgetTable> table;
        string IncomeExpenseName;
        int budgetTypeId;
        string BudgetTypeName;
        protected override async Task OnInitializedAsync()
        {
            await GetBudgets();
            await GetBudgetTypes();
        }

        protected async Task GetBudgets()
        {
            var result = await _genericRepositoryService.GetAllAsync<YearlyBudgetTable>("api/NZUCManagement/GetBudgets");
            yearlyBudgets = result.ToList();
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

        #region Table Code
        protected async Task<TableData<YearlyBudgetTable>> ServerReload(TableState state)
        {
            IEnumerable<YearlyBudgetTable> data = await _genericRepositoryService.GetAllAsync<YearlyBudgetTable>("api/NZUCManagement/GetBudgets");
            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.IncomeExpesense.IncomeBudgetType.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.IncomeExpesense.TypeofBudget.BudgetType.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            }).ToArray();
            totalItems = data.Count();
            switch (state.SortLabel)
            {
                case "ID_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.Id);
                    break;
                case "ActualBudget_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.ActualBudget);
                    break;
                case "BudgetName_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.IncomeExpesense.IncomeBudgetType);
                    break;
                case "BudgetType_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.IncomeExpesense.TypeofBudget.BudgetType);
                    break;

            }

            PagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
            return new TableData<YearlyBudgetTable>() { TotalItems = totalItems, Items = PagedData };
        }

        protected void OnSearch(string text)
        {
            searchString = text;
            table.ReloadServerData();
        }

        protected void OnRowClicked(TableRowClickEventArgs<YearlyBudgetTable> tableRow)
        {
            yearlyBudget = tableRow.Item;
            OpenDialog(yearlyBudget).GetAwaiter();
        }
        protected async Task OpenDialog(YearlyBudgetTable Budget)
        {
            DialogOptions dialogOptions = new DialogOptions() { Position = DialogPosition.Center, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var parameters = new DialogParameters { ["Budget"] = Budget };

            var dialog = Dialog.Show<ManageBudgetDialog>("", parameters, dialogOptions);
            var result = await dialog.Result;

        }
        #endregion
        #region stringFunc
        protected Func<IncomeExpenseBudgetType, string> IncomeExpesnseconverter = p => p?.IncomeBudgetType;
        protected Func<BudgetTypeTable, string> BudgetTypeconverter = p => p?.BudgetType;
        #endregion
    }
}
