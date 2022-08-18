using NZUCManagementSystemIA.Shared.Models;

namespace Domain.Interfaces
{
    public interface INZUCManagementRepository
    {
        Task<bool> DeleteAllAsync(int id);
        Task<IEnumerable<AuthorizerTable>> GetAllAuthorizers();
        Task<IEnumerable<EmployeeTable>> GetAllEmployees();
        Task<IEnumerable<ReviewTransactionTable>> GetAllTransactions();
        IEnumerable<IncomeExpenseBudgetType> GetIncomeExpenseBudgets(int BudgetType);
        IEnumerable<IncomeExpensesOperatingType> GetIncomeExpenseOperatingTypes(int BudgetType);
        Task<IEnumerable<OperatingIncomeExpense>> GetoperatingIncomeEX();
        Task<IEnumerable<ReviewersTable>> GetReviewers();
        Task<IEnumerable<YearlyBudgetTable>> GetYearlyBudgets();
    }
}