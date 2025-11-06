using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_vuottai.Models;

[Table("TKB_CHITIET")]
public partial class TKB_CHITIET
{
    [Key]
    public int TkbCtId { get; set; }

    public int TkbId { get; set; }

    public DateOnly NgayHoc { get; set; }

    public int LopId { get; set; }

    public int MonHocId { get; set; }

    public int LoaiHVId { get; set; }

    public int NgonNguId { get; set; }

    public int CaKipId { get; set; }

    public int SoTiet { get; set; }

    [ForeignKey("CaKipId")]
    [InverseProperty("TKB_CHITIETs")]
    public virtual CAKIP CaKip { get; set; } = null!;

    [ForeignKey("LoaiHVId")]
    [InverseProperty("TKB_CHITIETs")]
    public virtual LOAIHOCVIEN LoaiHV { get; set; } = null!;

    [ForeignKey("LopId")]
    [InverseProperty("TKB_CHITIETs")]
    public virtual LOP Lop { get; set; } = null!;

    [ForeignKey("MonHocId")]
    [InverseProperty("TKB_CHITIETs")]
    public virtual MONHOC MonHoc { get; set; } = null!;

    [ForeignKey("NgonNguId")]
    [InverseProperty("TKB_CHITIETs")]
    public virtual NGONNGU NgonN { get; set; } = null!;

    [InverseProperty("TkbCt")]
    public virtual ICollection<PHANCONG> PHANCONGs { get; set; } = new List<PHANCONG>();

    [ForeignKey("TkbId")]
    [InverseProperty("TKB_CHITIETs")]
    public virtual THOIKHOABIEU Tkb { get; set; } = null!;
}
