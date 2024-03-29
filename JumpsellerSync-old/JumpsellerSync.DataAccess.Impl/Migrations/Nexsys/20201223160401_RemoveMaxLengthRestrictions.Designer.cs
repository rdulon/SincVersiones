﻿// <auto-generated />
using JumpsellerSync.DataAccess.Impl.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace JumpsellerSync.DataAccess.Impl.Migrations.Nexsys
{
    [DbContext(typeof(NexsysNpgsqlDbContext))]
    [Migration("20201223160401_RemoveMaxLengthRestrictions")]
    partial class RemoveMaxLengthRestrictions
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("JumpsellerSync.Domain.Impl.Nexsys.NexsysBrand", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("NexsysId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("NexsysId")
                        .IsUnique();

                    b.ToTable("Brands");
                });

            modelBuilder.Entity("JumpsellerSync.Domain.Impl.Nexsys.NexsysCategory", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("SubCategoryId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("SubCategoryId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("JumpsellerSync.Domain.Impl.Nexsys.NexsysConfiguration", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Configuration");
                });

            modelBuilder.Entity("JumpsellerSync.Domain.Impl.Nexsys.NexsysProduct", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("BrandId")
                        .HasColumnType("text");

                    b.Property<string>("CategoryId")
                        .HasColumnType("text");

                    b.Property<string>("Currency")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("text");

                    b.Property<long>("Parent")
                        .HasColumnType("bigint");

                    b.Property<double>("Price")
                        .HasColumnType("double precision");

                    b.Property<string>("ProductCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RedcetusProductId")
                        .HasColumnType("text");

                    b.Property<double>("Stock")
                        .HasColumnType("double precision");

                    b.Property<string>("TaxExcluded")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("BrandId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ProductCode")
                        .IsUnique();

                    b.HasIndex("RedcetusProductId")
                        .IsUnique();

                    b.ToTable("Products");
                });

            modelBuilder.Entity("JumpsellerSync.Domain.Impl.Nexsys.NexsysCategory", b =>
                {
                    b.HasOne("JumpsellerSync.Domain.Impl.Nexsys.NexsysCategory", "SubCategory")
                        .WithMany()
                        .HasForeignKey("SubCategoryId");
                });

            modelBuilder.Entity("JumpsellerSync.Domain.Impl.Nexsys.NexsysProduct", b =>
                {
                    b.HasOne("JumpsellerSync.Domain.Impl.Nexsys.NexsysBrand", "Brand")
                        .WithMany()
                        .HasForeignKey("BrandId");

                    b.HasOne("JumpsellerSync.Domain.Impl.Nexsys.NexsysCategory", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");
                });
#pragma warning restore 612, 618
        }
    }
}
