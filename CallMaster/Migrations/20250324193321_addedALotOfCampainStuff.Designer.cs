﻿// <auto-generated />
using System;
using CallMaster.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CallMaster.Migrations
{
    [DbContext(typeof(CallMasterDbContext))]
    [Migration("20250324193321_addedALotOfCampainStuff")]
    partial class addedALotOfCampainStuff
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CallMaster.Models.CallMaster.Models.Campaign", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool?>("AutoDialEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("CompletionDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Configuration")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DaysToLookBack")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastGenerated")
                        .HasColumnType("datetime2");

                    b.Property<int?>("MaxAttemptsPerNumber")
                        .HasColumnType("int");

                    b.Property<int?>("MinCallsPerHour")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("PrioritizeOlderCallbacks")
                        .HasColumnType("bit");

                    b.Property<decimal?>("RequiredCallbackRate")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Campaigns");
                });

            modelBuilder.Entity("CallMaster.Models.CallMaster.Models.CampaignPhoneNumber", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CampaignId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("Outcome")
                        .HasColumnType("int");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CampaignId");

                    b.ToTable("CampaignPhoneNumbers");
                });

            modelBuilder.Entity("CallMaster.Models.LicenceKeys", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UsuableAcounts")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("LicenceKeys");
                });

            modelBuilder.Entity("CallMaster.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("LastLogin")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LicenceKeyId")
                        .HasColumnType("int");

                    b.Property<int?>("ManagerId")
                        .HasColumnType("int");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PersonalPhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("LicenceKeyId");

                    b.HasIndex("ManagerId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CallMaster.Models.VoIPLoginDetails", b =>
                {
                    b.Property<string>("InboundNumber")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Agent")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Extension")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Principal")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Received")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("InboundNumber");

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasFilter("[UserId] IS NOT NULL");

                    b.ToTable("VoIPLoginDetails");
                });

            modelBuilder.Entity("CampaignUser", b =>
                {
                    b.Property<int>("AssignedAgentsId")
                        .HasColumnType("int");

                    b.Property<int>("CampaignId")
                        .HasColumnType("int");

                    b.HasKey("AssignedAgentsId", "CampaignId");

                    b.HasIndex("CampaignId");

                    b.ToTable("CampaignAgents", (string)null);
                });

            modelBuilder.Entity("CallMaster.Models.CallMaster.Models.CampaignPhoneNumber", b =>
                {
                    b.HasOne("CallMaster.Models.CallMaster.Models.Campaign", "Campaign")
                        .WithMany("PhoneNumbers")
                        .HasForeignKey("CampaignId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Campaign");
                });

            modelBuilder.Entity("CallMaster.Models.User", b =>
                {
                    b.HasOne("CallMaster.Models.LicenceKeys", "LicenceKey")
                        .WithMany("Users")
                        .HasForeignKey("LicenceKeyId");

                    b.HasOne("CallMaster.Models.User", "Manager")
                        .WithMany()
                        .HasForeignKey("ManagerId");

                    b.Navigation("LicenceKey");

                    b.Navigation("Manager");
                });

            modelBuilder.Entity("CallMaster.Models.VoIPLoginDetails", b =>
                {
                    b.HasOne("CallMaster.Models.User", "User")
                        .WithOne("VoIPDetails")
                        .HasForeignKey("CallMaster.Models.VoIPLoginDetails", "UserId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("User");
                });

            modelBuilder.Entity("CampaignUser", b =>
                {
                    b.HasOne("CallMaster.Models.User", null)
                        .WithMany()
                        .HasForeignKey("AssignedAgentsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CallMaster.Models.CallMaster.Models.Campaign", null)
                        .WithMany()
                        .HasForeignKey("CampaignId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CallMaster.Models.CallMaster.Models.Campaign", b =>
                {
                    b.Navigation("PhoneNumbers");
                });

            modelBuilder.Entity("CallMaster.Models.LicenceKeys", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("CallMaster.Models.User", b =>
                {
                    b.Navigation("VoIPDetails")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
