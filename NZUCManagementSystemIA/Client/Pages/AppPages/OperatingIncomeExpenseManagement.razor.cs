using Microsoft.AspNetCore.Components;
using MudBlazor;
using NZUCManagementSystemIA.Client.Interfaces;
using NZUCManagementSystemIA.Client.Pages.AppPages.Dialogues;
using NZUCManagementSystemIA.Shared.Models;

namespace NZUCManagementSystemIA.Client.Pages.AppPages
{
    public class OperatingIncomeExpenseManagementBase : ComponentBase
    {
        protected OperatingIncomeExpense? operatingIncomeExpense = new OperatingIncomeExpense();
        protected IncomeExpensesOperatingType incomeExpensesOperatingType = new();
        protected ReviewersTable reviewer = new();
        protected OperatingType operatingType = new();
        protected IList<OperatingIncomeExpense> operatingIncomeExpenses = new List<OperatingIncomeExpense>();
        protected IList<OperatingType> _operatingTypes = new List<OperatingType>();
        protected IList<IncomeExpensesOperatingType> incomeExpensesOperatingTypes = new List<IncomeExpensesOperatingType>();
        protected IList<ReviewersTable> reviewers = new List<ReviewersTable>();
        protected IList<ConferencesAndField> conferencesAndFields = new List<ConferencesAndField>();
        protected IEnumerable<OperatingIncomeExpense> PagedData;
        protected MudTable<OperatingIncomeExpense> table;
        protected int totalitems;
        protected int OperatingTypeId;
        protected string? incomeExpensesOperatingTypeName;
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Inject] IGenericRepositoryService _genericRepository { get; set; }
        [Inject] IDialogService? Dialog { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }

        protected string? searchString;
        protected bool? _hasConference;

        protected override async Task OnInitializedAsync()
        {
            await GetOperatingTypes();
            await GetReviewers();
            await GetOperatingIncomeExpense();
           
        }

        protected void OnIncomeOperatingTypeClicked()
        {
            conferencesAndFields.Clear();
           if(incomeExpensesOperatingType.Id == 2 || incomeExpensesOperatingType.Id ==  3)
            {
                _hasConference = true ;
                GetConferences().GetAwaiter();
            }
           else
            {
                _hasConference = false;
            }
        }
        protected async Task GetOperatingTypes()
        {
            var result = await _genericRepository.GetAllAsync<OperatingType>("api/NZUCManagement/GetOperatingTypes");
            _operatingTypes = result.ToList();
        }
        protected async Task GetConferences()
        {
            var result = await _genericRepository.GetAllAsync<ConferencesAndField>("api/NZUCManagement/GetConferences");
            conferencesAndFields = result.ToList();
        }
        protected async Task GetOperatingIncomeExpense()
        {
            var result = await _genericRepository.GetAllAsync<OperatingIncomeExpense>("api/NZUCManagement/GetOperatingIncomeExpense");
            operatingIncomeExpenses = result.ToList();
        }

        protected async Task GetReviewers()
        {
            var result = await _genericRepository.GetAllAsync<ReviewersTable>("api/NZUCManagement/GetReviewers");
            reviewers = result.ToList();
        }
        #region Table Code
        protected async Task<TableData<OperatingIncomeExpense>> ServerReload(TableState state)
        {
            IEnumerable<OperatingIncomeExpense> data = await _genericRepository.GetAllAsync<OperatingIncomeExpense>("api/NZUCManagement/GetOperatingIncomeExpense");
            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.DateMonth.Value.Date.Month.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            }).ToArray();
            totalitems = data.Count();
            switch (state.SortLabel)
            {
                case "ID_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.Id);
                    break;
                case "Amount_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.Amount);
                    break;
                case "TypeOfIncomeOrExpense_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.IncomeExpenseOperatingTypeNavigation.OperatingType1);
                    break;
                case "Reviewer_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.Reviewer.Employee.EmployeeName);
                    break;

            }


            PagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
            return new TableData<OperatingIncomeExpense>() { TotalItems = totalitems, Items = PagedData };
        }
        protected async Task OpenDialog(OperatingIncomeExpense operatingIncomeExpense)
        {
            DialogOptions dialogOptions = new DialogOptions() { Position = DialogPosition.Center, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var parameters = new DialogParameters { ["IncomeExpense"] = operatingIncomeExpense };

            var dialog = Dialog.Show<AddOperatingAmounts>("", parameters, dialogOptions);
            var result = await dialog.Result;

        }

        protected void OnSearch(string text)
        {
            searchString = text;
            table.ReloadServerData();
        }
        #endregion

        protected async Task OnOperatingTypeClicked()
        {
            incomeExpensesOperatingTypes.Clear();
            incomeExpensesOperatingTypeName = string.Empty;
            OperatingTypeId = operatingType.Id;

            incomeExpensesOperatingTypeName = _operatingTypes.FirstOrDefault(x => x.Id == OperatingTypeId).OperatingType1;

            var result = await _genericRepository.GetByIdAsync<IncomeExpensesOperatingType>($"api/NZUCManagement/GetIncomeExpenseByOperatingTypeId/{OperatingTypeId}");
            incomeExpensesOperatingTypes = result.ToList();

        }

        protected async void Submit()
        {

            operatingIncomeExpense.ReviewerId = operatingIncomeExpense.Reviewer.Id;
            operatingIncomeExpense.IncomeExpenseOperatingType = operatingType.Id;
            operatingIncomeExpense.ConferenceId = operatingIncomeExpense.Conference.Id;
            operatingIncomeExpense.Conference = null;
            operatingIncomeExpense.Reviewer = null;
            operatingIncomeExpense.IncomeExpenseOperatingTypeNavigation = null;
            try
            {
                await _genericRepository.SaveAllAsync("api/NZUCManagement/SaveOperatingIncomeExpense", operatingIncomeExpense);
                Snackbar.Add("Recorded Added!", Severity.Success);
                MudDialog.Close();
            }
            catch (Exception ex)
            {
                var _ = ex.Message;
                Snackbar.Add("Error Adding Record", Severity.Error);
                throw;
            }
           


        }

        protected void Cancel()
        {
            MudDialog.Close();
        }
        #region String Functions
#pragma warning disable CS8603 // Possible null reference return.
        protected Func<IncomeExpensesOperatingType, string> converter = p => p?.IncomeExpenseType;
#pragma warning restore CS8603 // Possible null reference return.
        protected Func<ReviewersTable, string> Reviewerconverter = p => p?.Employee.EmployeeName; 
        protected Func<OperatingType, string> Typeconverter = p => p?.OperatingType1;
        protected Func<ConferencesAndField, string> Conferenceconverter = p => p?.ConferenceName;
        #endregion
    }
}
