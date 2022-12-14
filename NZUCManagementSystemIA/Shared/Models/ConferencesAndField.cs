// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NZUCManagementSystemIA.Shared.Models
{
    [Table("ConferencesAndField")]
    public partial class ConferencesAndField
    {
        public ConferencesAndField()
        {
            OperatingIncomeExpenses = new HashSet<OperatingIncomeExpense>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string ConferenceName { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string ConferenceCode { get; set; }

        [InverseProperty("Conference")]
        public virtual ICollection<OperatingIncomeExpense> OperatingIncomeExpenses { get; set; }
    }
}