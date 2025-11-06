using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_vuottai.Models;

[Table("CHUCVU")]
public partial class CHUCVU
{
    [Key]

    public int ChucVuId { get; set; }

    [StringLength(100)]
    [Required(ErrorMessage = "Tên chức vụ là bắt buộc")]
    [Display(Name = "Tên Chức vụ")]
    public string? TenChucVu { get; set; }
    [Display(Name = "Tỷ lệ định mức")]
    public int TyLeDinhMuc { get; set; }

    [InverseProperty("ChucVu")]
    public virtual ICollection<GIANGVIEN> GIANGVIENs { get; set; } = new List<GIANGVIEN>();
}
