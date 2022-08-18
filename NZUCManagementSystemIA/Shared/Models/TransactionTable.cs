﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NZUCManagementSystemIA.Shared.Models
{
    [Table("TransactionTable")]
    public partial class TransactionTable
    {
        [Key]
        public int Id { get; set; }
        public int? Department { get; set; }
        public int? Amount { get; set; }
        public int? ReviewerId { get; set; }
        public int? AuthorizationTableId { get; set; }
        public int? AuthorizerId { get; set; }

        [ForeignKey("AuthorizerId")]
        [InverseProperty("TransactionTables")]
        public virtual AuthorizerTable Authorizer { get; set; }
    }
}