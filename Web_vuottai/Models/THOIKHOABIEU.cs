using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_vuottai.Models;

[Table("THOIKHOABIEU")]
public partial class THOIKHOABIEU
{
    [Key]
    public int TkbId { get; set; }

    [StringLength(10)]
    public string? NamHoc { get; set; }

    public int? HocKy { get; set; }

    [InverseProperty("Tkb")]
    public virtual ICollection<TKB_CHITIET> TKB_CHITIETs { get; set; } = new List<TKB_CHITIET>();
}
