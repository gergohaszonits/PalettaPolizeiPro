﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PalettaPolizeiPro.Database;

#nullable disable

namespace PalettaPolizeiPro.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20231127132915_PalettaPolizeiProDB")]
    partial class PalettaPolizeiProDB
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("OrderPaletta", b =>
                {
                    b.Property<long>("InScheduledId")
                        .HasColumnType("bigint");

                    b.Property<long>("ScheduledPalettasId")
                        .HasColumnType("bigint");

                    b.HasKey("InScheduledId", "ScheduledPalettasId");

                    b.HasIndex("ScheduledPalettasId");

                    b.ToTable("OrderPaletta");
                });

            modelBuilder.Entity("OrderPaletta1", b =>
                {
                    b.Property<long>("FinishedPalettasId")
                        .HasColumnType("bigint");

                    b.Property<long>("InFinishedId")
                        .HasColumnType("bigint");

                    b.HasKey("FinishedPalettasId", "InFinishedId");

                    b.HasIndex("InFinishedId");

                    b.ToTable("OrderPaletta1");
                });

            modelBuilder.Entity("PalettaPolizeiPro.Data.PalettaNotification", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Body")
                        .HasColumnType("text");

                    b.Property<int>("Level")
                        .HasColumnType("integer");

                    b.Property<long>("PalettaId")
                        .HasColumnType("bigint");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("PalettaId");

                    b.ToTable("PalettaNotifications");
                });

            modelBuilder.Entity("PalettaPolizeiPro.Data.Palettas.Order", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("EndSort")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("MaximumSort")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("Scheduled")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("StartSort")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<long?>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("PalettaPolizeiPro.Data.Palettas.Paletta", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Identifier")
                        .HasColumnType("text");

                    b.Property<int>("Loop")
                        .HasColumnType("integer");

                    b.Property<bool>("Marked")
                        .HasColumnType("boolean");

                    b.Property<bool>("PalettaError")
                        .HasColumnType("boolean");

                    b.Property<bool>("ServiceFlag")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Palettas");
                });

            modelBuilder.Entity("PalettaPolizeiPro.Data.Palettas.PalettaProperty", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("ActualCycle")
                        .HasColumnType("integer");

                    b.Property<string>("EngineNumber")
                        .HasColumnType("text");

                    b.Property<string>("Identifier")
                        .HasColumnType("text");

                    b.Property<long?>("PalettaId")
                        .HasColumnType("bigint");

                    b.Property<int>("PredefiniedCycle")
                        .HasColumnType("integer");

                    b.Property<DateTime>("ReadTime")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("PalettaId");

                    b.ToTable("PalettaProperties");
                });

            modelBuilder.Entity("PalettaPolizeiPro.Data.Palettas.QueryState", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<byte?>("ControlFlag")
                        .HasColumnType("smallint");

                    b.Property<byte?>("OperationStatus")
                        .HasColumnType("smallint");

                    b.Property<string>("PalettaName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("QueryState");
                });

            modelBuilder.Entity("PalettaPolizeiPro.Data.QueryNotification", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Body")
                        .HasColumnType("text");

                    b.Property<int>("Level")
                        .HasColumnType("integer");

                    b.Property<long>("QueryStateId")
                        .HasColumnType("bigint");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("QueryStateId");

                    b.ToTable("QueryNotifications");
                });

            modelBuilder.Entity("PalettaPolizeiPro.Data.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int[]>("Authorizations")
                        .HasColumnType("integer[]");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.Property<string>("WorkerID")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PalettaPolizeiPro.Data.UserNotification", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Body")
                        .HasColumnType("text");

                    b.Property<int>("Level")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserNotifications");
                });

            modelBuilder.Entity("OrderPaletta", b =>
                {
                    b.HasOne("PalettaPolizeiPro.Data.Palettas.Order", null)
                        .WithMany()
                        .HasForeignKey("InScheduledId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PalettaPolizeiPro.Data.Palettas.Paletta", null)
                        .WithMany()
                        .HasForeignKey("ScheduledPalettasId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OrderPaletta1", b =>
                {
                    b.HasOne("PalettaPolizeiPro.Data.Palettas.Paletta", null)
                        .WithMany()
                        .HasForeignKey("FinishedPalettasId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PalettaPolizeiPro.Data.Palettas.Order", null)
                        .WithMany()
                        .HasForeignKey("InFinishedId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PalettaPolizeiPro.Data.PalettaNotification", b =>
                {
                    b.HasOne("PalettaPolizeiPro.Data.Palettas.Paletta", "Paletta")
                        .WithMany()
                        .HasForeignKey("PalettaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Paletta");
                });

            modelBuilder.Entity("PalettaPolizeiPro.Data.Palettas.Order", b =>
                {
                    b.HasOne("PalettaPolizeiPro.Data.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PalettaPolizeiPro.Data.Palettas.PalettaProperty", b =>
                {
                    b.HasOne("PalettaPolizeiPro.Data.Palettas.Paletta", null)
                        .WithMany("Properties")
                        .HasForeignKey("PalettaId");
                });

            modelBuilder.Entity("PalettaPolizeiPro.Data.QueryNotification", b =>
                {
                    b.HasOne("PalettaPolizeiPro.Data.Palettas.QueryState", "QueryState")
                        .WithMany()
                        .HasForeignKey("QueryStateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("QueryState");
                });

            modelBuilder.Entity("PalettaPolizeiPro.Data.UserNotification", b =>
                {
                    b.HasOne("PalettaPolizeiPro.Data.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PalettaPolizeiPro.Data.Palettas.Paletta", b =>
                {
                    b.Navigation("Properties");
                });
#pragma warning restore 612, 618
        }
    }
}
