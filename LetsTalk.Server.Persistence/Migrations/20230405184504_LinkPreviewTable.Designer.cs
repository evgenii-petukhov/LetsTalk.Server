﻿// <auto-generated />
using System;
using LetsTalk.Server.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LetsTalk.Server.Persistence.Migrations
{
    [DbContext(typeof(LetsTalkDbContext))]
    [Migration("20230405184504_LinkPreviewTable")]
    partial class LinkPreviewTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("LetsTalk.Server.Domain.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("AccountTypeId")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .HasColumnType("longtext");

                    b.Property<string>("ExternalId")
                        .HasColumnType("longtext");

                    b.Property<string>("FirstName")
                        .HasColumnType("longtext");

                    b.Property<string>("LastName")
                        .HasColumnType("longtext");

                    b.Property<string>("PhotoUrl")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("AccountTypeId");

                    b.ToTable("accounts");
                });

            modelBuilder.Entity("LetsTalk.Server.Domain.AccountType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("accounttypes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Facebook"
                        },
                        new
                        {
                            Id = 2,
                            Name = "VK"
                        });
                });

            modelBuilder.Entity("LetsTalk.Server.Domain.LinkPreview", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .HasColumnType("longtext");

                    b.Property<string>("Url")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("LinkPreview");
                });

            modelBuilder.Entity("LetsTalk.Server.Domain.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<long?>("DateCreatedUnix")
                        .HasColumnType("bigint");

                    b.Property<long?>("DateReadUnix")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsRead")
                        .HasColumnType("tinyint(1)");

                    b.Property<int?>("LinkPreviewId")
                        .HasColumnType("int");

                    b.Property<int>("RecipientId")
                        .HasColumnType("int");

                    b.Property<int>("SenderId")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .HasColumnType("longtext");

                    b.Property<string>("TextHtml")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("LinkPreviewId");

                    b.HasIndex("RecipientId");

                    b.HasIndex("SenderId");

                    b.ToTable("messages");
                });

            modelBuilder.Entity("LetsTalk.Server.Domain.Account", b =>
                {
                    b.HasOne("LetsTalk.Server.Domain.AccountType", "AccountType")
                        .WithMany()
                        .HasForeignKey("AccountTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AccountType");
                });

            modelBuilder.Entity("LetsTalk.Server.Domain.Message", b =>
                {
                    b.HasOne("LetsTalk.Server.Domain.LinkPreview", "LinkPreview")
                        .WithMany()
                        .HasForeignKey("LinkPreviewId");

                    b.HasOne("LetsTalk.Server.Domain.Account", "Recipient")
                        .WithMany()
                        .HasForeignKey("RecipientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LetsTalk.Server.Domain.Account", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LinkPreview");

                    b.Navigation("Recipient");

                    b.Navigation("Sender");
                });
#pragma warning restore 612, 618
        }
    }
}