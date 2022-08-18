using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using NZUCManagementSystemIA.Shared.Models;
using NZUCManagementSystemIA.Client.Interfaces;

namespace NZUCManagementSystemIA.Client.Shared
{
    public partial class NavMenu
    {

        protected IList<ReviewTransactionTable> _transactions = new List<ReviewTransactionTable>();
        [Inject] IGenericRepositoryService _genericRepositoryService { get; set; }
        [Inject] AuthenticationStateProvider AuthenticationState { get; set; }
        protected int TotalTransactions = 0;

        protected override async Task OnInitializedAsync()
        {
                await GetTransactions();
        }
        protected async Task GetTransactions()
        {
            var result = await _genericRepositoryService.GetAllAsync<ReviewTransactionTable>("api/NZUCManagement/GetReviewerTransactions");
            _transactions = result.ToList();
            TotalTransactions = _transactions.Count();
        }
    }
}