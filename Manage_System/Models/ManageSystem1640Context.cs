using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Manage_System.models;

public partial class ManageSystem1640Context : DbContext
{
    public ManageSystem1640Context()
    {
    }

    public ManageSystem1640Context(DbContextOptions<ManageSystem1640Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Contribution> Contributions { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Faculty> Faculties { get; set; }

    public virtual DbSet<ImgFile> ImgFiles { get; set; }

    public virtual DbSet<Magazine> Magazines { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server = LAPTOP-N37QJK1L\\SQLEXPRESS;Database=Manage_System_1640;uid=sa;pwd=Admin@123;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comments__3213E83FD20A86B9");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CommentDate)
                .HasColumnType("datetime")
                .HasColumnName("commentDate");
            entity.Property(e => e.CommentText)
                .HasColumnType("text")
                .HasColumnName("commentText");
            entity.Property(e => e.ContributionId).HasColumnName("contributionId");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Contribution).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ContributionId)
                .HasConstraintName("FK__Comments__contri__5629CD9C");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Comments__userId__571DF1D5");
        });

        modelBuilder.Entity<Contribution>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Contribu__3213E83F4A1504BF");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content)
                .HasColumnType("text")
                .HasColumnName("content");
            entity.Property(e => e.LastModifiedDate)
                .HasColumnType("datetime")
                .HasColumnName("lastModifiedDate");
            entity.Property(e => e.MagazineId).HasColumnName("magazineId");
            entity.Property(e => e.Publics)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("publics");
            entity.Property(e => e.ShortDescription)
                .HasColumnType("text")
                .HasColumnName("shortDescription");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.SubmissionDate)
                .HasColumnType("datetime")
                .HasColumnName("submissionDate");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("title");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Magazine).WithMany(p => p.Contributions)
                .HasForeignKey(d => d.MagazineId)
                .HasConstraintName("FK__Contribut__magaz__5812160E");

            entity.HasOne(d => d.User).WithMany(p => p.Contributions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Contribut__userI__59063A47");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employee");

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Position)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Facultie__3213E83FA3774240");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<ImgFile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__imgFile__3213E83F67F825DA");

            entity.ToTable("imgFile");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ContributionId).HasColumnName("contributionId");
            entity.Property(e => e.Stype)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("stype");
            entity.Property(e => e.Url)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("url");

            entity.HasOne(d => d.Contribution).WithMany(p => p.ImgFiles)
                .HasForeignKey(d => d.ContributionId)
                .HasConstraintName("FK__imgFile__contrib__59FA5E80");
        });

        modelBuilder.Entity<Magazine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Magazine__3213E83FD5FB45EE");

            entity.ToTable("Magazine");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CloseYear)
                .HasColumnType("date")
                .HasColumnName("closeYear");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.StartYear)
                .HasColumnType("date")
                .HasColumnName("startYear");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3213E83F752E2C11");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3213E83F56046F46");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Avatar)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("avatar");
            entity.Property(e => e.CreateDay)
                .HasColumnType("datetime")
                .HasColumnName("createDay");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FacultyId).HasColumnName("facultyId");
            entity.Property(e => e.FullName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("fullName");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Faculty).WithMany(p => p.Users)
                .HasForeignKey(d => d.FacultyId)
                .HasConstraintName("FK__Users__facultyId__5AEE82B9");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Users__roleId__5BE2A6F2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
