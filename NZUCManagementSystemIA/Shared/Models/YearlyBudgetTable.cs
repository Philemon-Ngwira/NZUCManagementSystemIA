﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NZUCManagementSystemIA.Shared.Models
{
    [Table("YearlyBudgetTable")]
    public partial class YearlyBudgetTable
    {
        [Key]
        public int Id { get; set; }
        public int? ActualBudget { get; set; }
        [Column(TypeName = "date")]
        public DateTime? YearStart { get; set; }
        [Column(TypeName = "date")]
        public DateTime? YearEnd { get; set; }
        public int? IncomeExpesenseId { get; set; }

        [ForeignKey("IncomeExpesenseId")]
        [InverseProperty("YearlyBudgetTables")]
        public virtual IncomeExpenseBudgetType IncomeExpesense { get; set; }
    }
}