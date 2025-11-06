using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_vuottai.Models;

[Table("CHUCDANH_GIOCHUAN")]
[Index("ChucDanhId", "NamHoc", "HocKy", Name = "UQ_CD_GC", IsUnique = true)]
public partial class CHUCDANH_GIOCHUAN
{
    [Key]
    public int Id { get; set; }

    public int ChucDanhId { get; set; }

    [StringLength(10)]
    [Display(Name = "Năm Học")]
    public string? NamHoc { get; set; }
[Display(Name = "Học Kỳ")]
    public int? HocKy { get; set; }
[Display(Name = "Giờ Chuẩn")]
    public int GioChuan { get; set; }

    [ForeignKey("ChucDanhId")]
    [InverseProperty("CHUCDANH_GIOCHUANs")]
    public virtual CHUCDANH ChucDanh { get; set; } = null!;
}
