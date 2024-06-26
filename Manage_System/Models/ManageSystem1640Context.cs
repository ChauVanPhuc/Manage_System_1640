﻿using System;
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

    public virtual DbSet<Faculty> Faculties { get; set; }

    public virtual DbSet<ImgFile> ImgFiles { get; set; }

    public virtual DbSet<LastLogin> LastLogins { get; set; }

    public virtual DbSet<Magazine> Magazines { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Rule> Rules { get; set; }

    public virtual DbSet<Security> Securities { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comments__3213E83FD20A86B9");

            entity.HasIndex(e => e.ContributionId, "IX_Comments_contributionId");

            entity.HasIndex(e => e.UserId, "IX_Comments_userId");

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
                .HasConstraintName("FK_Comments_Contributions");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Comments__userId__571DF1D5");
        });

        modelBuilder.Entity<Contribution>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LastModifiedDate)
                .HasColumnType("datetime")
                .HasColumnName("lastModifiedDate");
            entity.Property(e => e.MagazineId).HasColumnName("magazineId");
            entity.Property(e => e.Publics).HasColumnName("publics");
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
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("title");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Magazine).WithMany(p => p.Contributions)
                .HasForeignKey(d => d.MagazineId)
                .HasConstraintName("FK_Contributions_Magazine");

            entity.HasOne(d => d.User).WithMany(p => p.Contributions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Contributions_Users");
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

            entity.HasIndex(e => e.ContributionId, "IX_imgFile_contributionId");

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
                .HasConstraintName("FK_imgFile_Contributions");
        });

        modelBuilder.Entity<LastLogin>(entity =>
        {
            entity.ToTable("LastLogin");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.History)
                .HasColumnType("datetime")
                .HasColumnName("history");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.User).WithMany(p => p.LastLogins)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_LastLogin_Users");
        });

        modelBuilder.Entity<Magazine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Magazine__3213E83FD5FB45EE");

            entity.ToTable("Magazine");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClosureDay)
                .HasColumnType("date")
                .HasColumnName("closureDay");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.FinalClosureDay)
                .HasColumnType("date")
                .HasColumnName("finalClosureDay");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content)
                .HasColumnType("text")
                .HasColumnName("content");
            entity.Property(e => e.Receiver).HasColumnName("receiver");
            entity.Property(e => e.Sender).HasColumnName("sender");
            entity.Property(e => e.SentAt)
                .HasColumnType("datetime")
                .HasColumnName("sentAt");

            entity.HasOne(d => d.ReceiverNavigation).WithMany(p => p.MessageReceiverNavigations)
                .HasForeignKey(d => d.Receiver)
                .HasConstraintName("FK_Messages_Users");

            entity.HasOne(d => d.SenderNavigation).WithMany(p => p.MessageSenderNavigations)
                .HasForeignKey(d => d.Sender)
                .HasConstraintName("FK_Messages_Users1");
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

        modelBuilder.Entity<Rule>(entity =>
        {
            entity.ToTable("Rule");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Rules)
                .IsUnicode(false)
                .HasColumnName("rules");
        });

        modelBuilder.Entity<Security>(entity =>
        {
            entity.ToTable("Security");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Security1)
                .IsUnicode(false)
                .HasColumnName("security");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3213E83F56046F46");

            entity.HasIndex(e => e.FacultyId, "IX_Users_facultyId");

            entity.HasIndex(e => e.RoleId, "IX_Users_roleId");

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
            entity.Property(e => e.PasswordResetToken)
                .IsUnicode(false)
                .HasColumnName("passwordResetToken");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TokenExpiration)
                .HasColumnType("datetime")
                .HasColumnName("tokenExpiration");

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
