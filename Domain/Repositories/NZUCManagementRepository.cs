using Domain.Data;
using NZUCManagementSystemIA.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Repositories;
using Domain.Interfaces;

namespace NZUC_Domain.Repositories
{
    public class NZUCManagementRepository : GenericRepository, INZUCManagementRepository
    {
        private readonly NZUCMANAGEMENTSYSTEMContext _context;
        public NZUCManagementRepository(NZUCMANAGEMENTSYSTEMContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AuthorizerTable>> GetAllAuthorizers()
        {
            var result = await _context.AuthorizerTables
                .Include(x => x.AuthorizerName)
                .ToListAsync();
            return result;
        }
        public async Task<IEnumerable<ReviewTransactionTable>> GetAllTransactions()
        {
            var result = await _context.ReviewTransactionTables
                .Include(x => x.Department)
                .Include(x => x.Employee)
                .Include(x => x.PaymentMethod)
                .Include(x => x.PaymentType)
                .Include(x => x.Reviewer)
                .ThenInclude(x => x.Employee)
                .ToListAsync();
            return result;
        }
        public async Task<IEnumerable<EmployeeTable>> GetAllEmployees()
        {
            var result = await _context.EmployeeTables
                .Include(x => x.Gender)
                .Include(x => x.Department)
                .Include(x => x.SalaryNavigation)

                .ToListAsync();
            return result;
        }

        public async Task<IEnumerable<OperatingIncomeExpense>> GetoperatingIncomeEX()
        {
            var result = await _context.OperatingIncomeExpenses
                .Include(x => x.Reviewer)
                .ThenInclude(x => x.Employee)
                .Include(x => x.IncomeExpenseOperatingTypeNavigation)
                .Include(x => x.Conference)
                .Include(x => x.Department)


                .ToListAsync();
            return result;
        }
        public async Task<IEnumerable<YearlyBudgetTable>> GetYearlyBudgets()
        {
            var result = await _context.YearlyBudgetTables
                .Include(x => x.IncomeExpesense)
                .ThenInclude(x => x.TypeofBudget)

                .ToListAsync();

            return result;
        }
        public async Task<IEnumerable<ReviewersTable>> GetReviewers()
        {
            var result = await _context.ReviewersTables
                .Include(x => x.Employee)
                .ThenInclude(x => x.SalaryNavigation)
                .Include(x => x.Employee)
                .ThenInclude(x => x.Department)


                .ToListAsync();

            return result;
        }

        public async Task<bool> DeleteAllAsync(int id)
        {
            var siteToDelete = await _context.SalaryTables.FindAsync(id);
            if (siteToDelete == null)
            {
                return false;
            }
            _context.SalaryTables.Remove(siteToDelete);
            await _context.SaveChangesAsync();


            return true;
        }
        public async Task<bool> DeleteReviewTransactionAsync(int id)
        {
            try
            {
                var siteToDelete = await _context.ReviewTransactionTables.FindAsync(id);
                if (siteToDelete == null)
                {
                    return false;
                }
                _context.ReviewTransactionTables.Remove(siteToDelete);
                await _context.SaveChangesAsync();


                return true;
            }
            catch (Exception ex)
            {
                var _ = ex.Message;
                throw;
            }

        }

        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            try
            {
                var siteToDelete = await _context.Departments_Tables.FindAsync(id);
                if (siteToDelete == null)
                {
                    return false;
                }
                _context.Departments_Tables.Remove(siteToDelete);
                await _context.SaveChangesAsync();


                return true;
            }
            catch (Exception ex)
            {
                var _ = ex.Message;
                throw;
            }

        }
        public async Task<bool> DeleteConferenceAsync(int id)
        {
            try
            {
                var siteToDelete = await _context.ConferencesAndFields.FindAsync(id);
                if (siteToDelete == null)
                {
                    return false;
                }
                _context.ConferencesAndFields.Remove(siteToDelete);
                await _context.SaveChangesAsync();


                return true;
            }
            catch (Exception ex)
            {
                var _ = ex.Message;
                throw;
            }

        }

        public async Task<bool> DeleteAuthorizerAsync(int id)
        {
            try
            {
                var siteToDelete = await _context.AuthorizerTables.FindAsync(id);
                if (siteToDelete == null)
                {
                    return false;
                }
                _context.AuthorizerTables.Remove(siteToDelete);
                await _context.SaveChangesAsync();


                return true;
            }
            catch (Exception ex)
            {
                var _ = ex.Message;
                throw;
            }

        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            try
            {
                var siteToDelete = await _context.EmployeeTables.FindAsync(id);
                if (siteToDelete == null)
                {
                    return false;
                }
                _context.EmployeeTables.Remove(siteToDelete);
                await _context.SaveChangesAsync();


                return true;
            }
            catch (Exception ex)
            {
                var _ = ex.Message;
                throw;
            }

        }
      
        public async Task<bool> DeleteOperatingIncomeExpAsync(int id)
        {
            try
            {
                var siteToDelete = await _context.OperatingIncomeExpenses.FindAsync(id);
                if (siteToDelete == null)
                {
                    return false;
                }
                _context.OperatingIncomeExpenses.Remove(siteToDelete);
                await _context.SaveChangesAsync();


                return true;
            }
            catch (Exception ex)
            {
                var _ = ex.Message;
                throw;
            }

        }

        public async Task<bool> DeleteBudgetAsync(int id)
        {
            try
            {
                var siteToDelete = await _context.YearlyBudgetTables.FindAsync(id);
                if (siteToDelete == null)
                {
                    return false;
                }
                _context.YearlyBudgetTables.Remove(siteToDelete);
                await _context.SaveChangesAsync();


                return true;
            }
            catch (Exception ex)
            {
                var _ = ex.Message;
                throw;
            }

        }
        #region GetByForeignId
        public IEnumerable<IncomeExpenseBudgetType> GetIncomeExpenseBudgets(int BudgetType)
        {
            try
            {
                List<IncomeExpenseBudgetType> incomeExpenses = new List<IncomeExpenseBudgetType>();
                incomeExpenses = (from IncomExpenseName in _context.IncomeExpenseBudgetTypes where IncomExpenseName.TypeofBudgetId == BudgetType select IncomExpenseName).ToList();
                return incomeExpenses;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public IEnumerable<IncomeExpensesOperatingType> GetIncomeExpenseOperatingTypes(int BudgetType)
        {
            try
            {
                List<IncomeExpensesOperatingType> incomeExpenses = new List<IncomeExpensesOperatingType>();
                incomeExpenses = (from IncomExpenseName in _context.IncomeExpensesOperatingTypes where IncomExpenseName.OperatingType == BudgetType select IncomExpenseName).ToList();
                return incomeExpenses;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
    }
}
