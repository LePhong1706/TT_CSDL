using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_vuottai.Models;

[Table("LOAIHOCVIEN")]
public partial class LOAIHOCVIEN
{
    [Key]
    public int LoaiHVId { get; set; }

    [StringLength(100)]
    [Display(Name = "Tên loại học viên")] 
    public string? TenLoaiHV { get; set; }

    [Column(TypeName = "decimal(4, 2)")]
    [Display(Name = "Hệ số")] 
    public decimal HeSo { get; set; }

    [InverseProperty("LoaiHV")]
    public virtual ICollection<TKB_CHITIET> TKB_CHITIETs { get; set; } = new List<TKB_CHITIET>();
}
