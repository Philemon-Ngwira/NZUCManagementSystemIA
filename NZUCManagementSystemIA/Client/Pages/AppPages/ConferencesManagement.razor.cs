using Microsoft.AspNetCore.Components;
using MudBlazor;
using NZUCManagementSystemIA.Client.Interfaces;
using NZUCManagementSystemIA.Client.Pages.AppPages.Dialogues;
using NZUCManagementSystemIA.Shared.Models;

namespace NZUCManagementSystemIA.Client.Pages.AppPages
{
    public class ConferencesManagementBase : ComponentBase
    {
        protected IList<ConferencesAndField> conferences = new List<ConferencesAndField>();
        protected ConferencesAndField conferenceAndField = new();
        protected IEnumerable<ConferencesAndField>? PagedData;
        [Inject] IGenericRepositoryService _genericRepositoryService { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Inject] IDialogService Dialog { get;set; }
        [Inject] NavigationManager NavigationManager { get; set; }
        protected string searchString = "";
        protected int totalItems = 0;
        protected MudTable<ConferencesAndField> table;
        protected override async Task OnInitializedAsync()
        {
            await GetConferences();
        }

        protected async Task GetConferences()
        {
            var res = await _genericRepositoryService.GetAllAsync<ConferencesAndField>("api/NZUCManagement/GetConferences");
            conferences = res.ToList();
        }


        #region Table Code
        protected async Task<TableData<ConferencesAndField>> ServerReload(TableState state)
        {
            IEnumerable<ConferencesAndField> data = await _genericRepositoryService.GetAllAsync<ConferencesAndField>("api/NZUCManagement/GetConferences");
            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.ConferenceName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.ConferenceCode.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            }).ToArray();
            totalItems = data.Count();
            switch (state.SortLabel)
            {
                case "ID_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.Id);
                    break;
                case "ConferenceName_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.ConferenceName);
                    break;
                case "ConferenceCode_field":
                    data = data.OrderByDirection(state.SortDirection, o => o.ConferenceCode);
                    break;

            }

            PagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
            return new TableData<ConferencesAndField>() { TotalItems = totalItems, Items = PagedData };
        }

        protected void OnSearch(string text)
        {
            searchString = text;
            table.ReloadServerData();
        }

        protected async Task OpenDialog(ConferencesAndField conferences)
        {
            DialogOptions dialogOptions = new DialogOptions() { Position = DialogPosition.Center, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var parameters = new DialogParameters { ["conferences"] = conferences };

            var dialog = Dialog.Show<AddConferenceOrField>("", parameters, dialogOptions);
            var result = await dialog.Result;

        }
        protected void onRowClicked(TableRowClickEventArgs<ConferencesAndField> eventArgs)
        {
            conferenceAndField = eventArgs.Item;
            OpenEditDialog(conferenceAndField).GetAwaiter();
        }
        protected async Task OpenEditDialog(ConferencesAndField conference)
        {
            DialogOptions dialogOptions = new DialogOptions() { Position = DialogPosition.Center, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var parameters = new DialogParameters { ["conference"] = conference };

            var dialog = Dialog.Show<EditConference>("", parameters, dialogOptions);
            var result = await dialog.Result;
            
        }
        protected async void CommitEdit()
        {
            await _genericRepositoryService.UpdateAsync("api/NZUCManagement/UpdateConference", conferenceAndField);
            Snackbar.Add("Successfully Updated!", Severity.Success);
            MudDialog.Close();
            NavigationManager.NavigateTo("/Pages/AppPages/ConferencesManagement", forceLoad: true);
        }

        protected async void saveConference()
        {
            await _genericRepositoryService.SaveAllAsync("api/NZUCManagement/SaveConference", conferenceAndField);
            Snackbar.Add("Successfully Added!", Severity.Success);
            MudDialog.Close();
            NavigationManager.NavigateTo("/Pages/AppPages/ConferencesManagement", forceLoad: true);
        }
        protected void Delete()
        {

        }
        protected void Cancel()
        {
            MudDialog.Close();
        }
        #endregion
    }
}
