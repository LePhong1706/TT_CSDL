using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_vuottai.Models;

[Table("LOP")]
[Index("MaLop", Name = "UQ__LOP__3B98D272408E744E", IsUnique = true)]
public partial class LOP
{
    [Key]
    public int LopId { get; set; }

    [StringLength(50)]
    [Display(Name = "Mã Lớp")]
    public string? MaLop { get; set; }

    [StringLength(200)]
    [Display(Name = "Tên Lớp")]
    public string? TenLop { get; set; }
    [Display(Name = "Quân số")]
    public int QuanSo { get; set; }

    [InverseProperty("Lop")]
    public virtual ICollection<TKB_CHITIET> TKB_CHITIETs { get; set; } = new List<TKB_CHITIET>();
}
