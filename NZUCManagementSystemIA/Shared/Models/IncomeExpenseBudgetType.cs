﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NZUCManagementSystemIA.Shared.Models
{
    public partial class IncomeExpenseBudgetType
    {
        public IncomeExpenseBudgetType()
        {
            YearlyBudgetTables = new HashSet<YearlyBudgetTable>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string IncomeBudgetType { get; set; }
        public int? TypeofBudgetId { get; set; }

        [ForeignKey("TypeofBudgetId")]
        [InverseProperty("IncomeExpenseBudgetTypes")]
        public virtual BudgetTypeTable TypeofBudget { get; set; }
        [InverseProperty("IncomeExpesense")]
        public virtual ICollection<YearlyBudgetTable> YearlyBudgetTables { get; set; }
    }
}