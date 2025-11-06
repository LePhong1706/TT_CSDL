using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_vuottai.Models;

[Table("PHANCONG")]
public partial class PHANCONG
{
    [Key]
    public int PhanCongId { get; set; }

    public int GiangVienId { get; set; }

    public int TkbCtId { get; set; }

    [StringLength(50)]
    public string? VaiTro { get; set; }

    [ForeignKey("GiangVienId")]
    [InverseProperty("PHANCONGs")]
    public virtual GIANGVIEN GiangVien { get; set; } = null!;

    [ForeignKey("TkbCtId")]
    [InverseProperty("PHANCONGs")]
    public virtual TKB_CHITIET TkbCt { get; set; } = null!;
}
