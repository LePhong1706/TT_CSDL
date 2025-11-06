using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Web_vuottai.Controllers;
using Web_vuottai.Models;

namespace Web_vuottai.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CAKIP> CAKIPs { get; set; }

    public virtual DbSet<CHUCDANH> CHUCDANHs { get; set; }

    public virtual DbSet<CHUCDANH_GIOCHUAN> CHUCDANH_GIOCHUANs { get; set; }

    public virtual DbSet<CHUCVU> CHUCVUs { get; set; }

    public virtual DbSet<DONGIAGIO> DONGIAGIOs { get; set; }

    public virtual DbSet<DONVI> DONVIs { get; set; }

    public virtual DbSet<GIANGVIEN> GIANGVIENs { get; set; }

    public virtual DbSet<GV_CHUCDANH> GV_CHUCDANHs { get; set; }

    public virtual DbSet<LOAIHOCVIEN> LOAIHOCVIENs { get; set; }

    public virtual DbSet<LOP> LOPs { get; set; }

    public virtual DbSet<MONHOC> MONHOCs { get; set; }

    public virtual DbSet<NGONNGU> NGONNGUs { get; set; }

    public virtual DbSet<PHANCONG> PHANCONGs { get; set; }

    public virtual DbSet<THOIKHOABIEU> THOIKHOABIEUs { get; set; }

    public virtual DbSet<TKB_CHITIET> TKB_CHITIETs { get; set; }

    public virtual DbSet<VUOTTAI_SUM> VUOTTAI_SUMs { get; set; }

    public virtual DbSet<v_PhanCong_ChiTiet> v_PhanCong_ChiTiets { get; set; }

    public virtual DbSet<v_VuotTai_TongQuan> v_VuotTai_TongQuans { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CAKIP>(entity =>
        {
            entity.HasKey(e => e.CaKipId).HasName("PK__CAKIP__3D0DFDCA09980122");

            entity.Property(e => e.HeSo).HasDefaultValue(1.0m);
        });

        modelBuilder.Entity<CHUCDANH>(entity =>
        {
            entity.HasKey(e => e.ChucDanhId).HasName("PK__CHUCDANH__76C81AED3842562D");
        });

        modelBuilder.Entity<CHUCDANH_GIOCHUAN>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CHUCDANH__3214EC07A6454E5F");

            entity.HasOne(d => d.ChucDanh).WithMany(p => p.CHUCDANH_GIOCHUANs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CHUCDANH___ChucD__6383C8BA");
        });

        modelBuilder.Entity<CHUCVU>(entity =>
        {
            entity.HasKey(e => e.ChucVuId).HasName("PK__CHUCVU__3457B641E2A3BE40");

            entity.Property(e => e.TyLeDinhMuc).HasDefaultValue(100);
        });

        modelBuilder.Entity<DONGIAGIO>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DONGIAGI__3214EC07EDE73830");

            entity.HasOne(d => d.ChucDanh).WithMany(p => p.DONGIAGIOs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DONGIAGIO__ChucD__6754599E");
        });

        modelBuilder.Entity<DONVI>(entity =>
        {
            entity.HasKey(e => e.DonViId).HasName("PK__DONVI__1CB88517472AC132");
        });

        modelBuilder.Entity<GIANGVIEN>(entity =>
        {
            entity.HasKey(e => e.GiangVienId).HasName("PK__GIANGVIE__626127E2D0E43D35");

            entity.HasOne(d => d.ChucVu).WithMany(p => p.GIANGVIENs).HasConstraintName("FK__GIANGVIEN__ChucV__5AEE82B9");

            entity.HasOne(d => d.DonVi).WithMany(p => p.GIANGVIENs).HasConstraintName("FK__GIANGVIEN__DonVi__59FA5E80");
        });

        modelBuilder.Entity<GV_CHUCDANH>(entity =>
        {
            entity.HasKey(e => e.GvChucDanhId).HasName("PK__GV_CHUCD__706F772C9DB09B78");

            entity.HasOne(d => d.ChucDanh).WithMany(p => p.GV_CHUCDANHs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GV_CHUCDA__ChucD__5FB337D6");

            entity.HasOne(d => d.GiangVien).WithMany(p => p.GV_CHUCDANHs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GV_CHUCDA__Giang__5EBF139D");
        });

        modelBuilder.Entity<LOAIHOCVIEN>(entity =>
        {
            entity.HasKey(e => e.LoaiHVId).HasName("PK__LOAIHOCV__B8463743240D083B");

            entity.Property(e => e.HeSo).HasDefaultValue(1.0m);
        });

        modelBuilder.Entity<LOP>(entity =>
        {
            entity.HasKey(e => e.LopId).HasName("PK__LOP__40585D2B6DB4843D");
        });

        modelBuilder.Entity<MONHOC>(entity =>
        {
            entity.HasKey(e => e.MonHocId).HasName("PK__MONHOC__32C3DE7D650EE738");
        });

        modelBuilder.Entity<NGONNGU>(entity =>
        {
            entity.HasKey(e => e.NgonNguId).HasName("PK__NGONNGU__BB618896580CE462");

            entity.Property(e => e.HeSo).HasDefaultValue(1.0m);
        });

        modelBuilder.Entity<PHANCONG>(entity =>
        {
            entity.HasKey(e => e.PhanCongId).HasName("PK__PHANCONG__7EF840BDAEE6D882");

            entity.ToTable("PHANCONG", tb => tb.HasTrigger("trg_PhanCong_AIU_Recalc"));

            entity.HasOne(d => d.GiangVien).WithMany(p => p.PHANCONGs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PHANCONG__GiangV__74AE54BC");

            entity.HasOne(d => d.TkbCt).WithMany(p => p.PHANCONGs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PHANCONG__TkbCtI__75A278F5");
        });

        modelBuilder.Entity<THOIKHOABIEU>(entity =>
        {
            entity.HasKey(e => e.TkbId).HasName("PK__THOIKHOA__F5B795064F6FDBE4");
        });

        modelBuilder.Entity<TKB_CHITIET>(entity =>
        {
            entity.HasKey(e => e.TkbCtId).HasName("PK__TKB_CHIT__E06AEF6D7FB8B773");

            entity.ToTable("TKB_CHITIET", tb => tb.HasTrigger("trg_TkbCt_AIU_Recalc"));

            entity.HasOne(d => d.CaKip).WithMany(p => p.TKB_CHITIETs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TKB_CHITI__CaKip__70DDC3D8");

            entity.HasOne(d => d.LoaiHV).WithMany(p => p.TKB_CHITIETs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TKB_CHITI__LoaiH__6EF57B66");

            entity.HasOne(d => d.Lop).WithMany(p => p.TKB_CHITIETs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TKB_CHITI__LopId__6D0D32F4");

            entity.HasOne(d => d.MonHoc).WithMany(p => p.TKB_CHITIETs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TKB_CHITI__MonHo__6E01572D");

            entity.HasOne(d => d.NgonN).WithMany(p => p.TKB_CHITIETs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TKB_CHITI__NgonN__6FE99F9F");

            entity.HasOne(d => d.Tkb).WithMany(p => p.TKB_CHITIETs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TKB_CHITI__TkbId__6C190EBB");
        });

        modelBuilder.Entity<VUOTTAI_SUM>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VUOTTAI___3214EC0774105AB9");

            entity.Property(e => e.LastCalcAt).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.GiangVien).WithMany(p => p.VUOTTAI_SUMs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__VUOTTAI_S__Giang__797309D9");
        });

        modelBuilder.Entity<v_PhanCong_ChiTiet>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("v_PhanCong_ChiTiet", "dbo");
        });

        modelBuilder.Entity<v_VuotTai_TongQuan>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("v_VuotTai_TongQuan");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
