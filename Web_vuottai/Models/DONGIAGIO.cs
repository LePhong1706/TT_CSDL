using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_vuottai.Models;

[Table("DONGIAGIO")]
[Index("ChucDanhId", "NamHoc", "HocKy", Name = "UQ_DGG", IsUnique = true)]
public partial class DONGIAGIO
{
    [Key]
    public int Id { get; set; }

    public int ChucDanhId { get; set; }

    [StringLength(10)]
    public string? NamHoc { get; set; }

    public int? HocKy { get; set; }

    [Column("DonGiaGio", TypeName = "decimal(18, 2)")]
    public decimal DonGiaGio1 { get; set; }

    [ForeignKey("ChucDanhId")]
    [InverseProperty("DONGIAGIOs")]
    public virtual CHUCDANH ChucDanh { get; set; } = null!;
}
