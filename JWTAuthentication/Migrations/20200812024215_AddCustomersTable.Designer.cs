﻿// <auto-generated />
using System;
using JWTAuthentication.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace JWTAuthentication.Migrations
{
    [DbContext(typeof(JWTAuthenticationContext))]
    [Migration("20200812024215_AddCustomersTable")]
    partial class AddCustomersTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("JWTAuthentication.Model.Game", b =>
                {
                    b.Property<long>("GameId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("ImagePath")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastPlayed")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("NextGameDateTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("OwnerId")
                        .HasColumnType("bigint");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.HasKey("GameId");

                    b.HasIndex("OwnerId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("JWTAuthentication.Model.PlayerToGame", b =>
                {
                    b.Property<long>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long>("GameId")
                        .HasColumnType("bigint");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("id");

                    b.HasIndex("GameId");

                    b.HasIndex("UserId");

                    b.ToTable("PlayerToGames");
                });

            modelBuilder.Entity("JWTAuthentication.Model.Token", b =>
                {
                    b.Property<string>("token")
                        .HasColumnType("text");

                    b.HasKey("token");

                    b.ToTable("Tokens");
                });

            modelBuilder.Entity("JWTAuthentication.Model.UserAccount", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("EmailAddress")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("UserAccounts");
                });

            modelBuilder.Entity("JWTAuthentication.Model.UserData", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<string>("HoursPlayed")
                        .HasColumnType("text");

                    b.Property<string>("ImagePath")
                        .HasColumnType("text");

                    b.Property<string>("MemberSince")
                        .HasColumnType("text");

                    b.Property<string>("Subscription")
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.ToTable("UserDatas");
                });

            modelBuilder.Entity("JWTAuthentication.Model.Game", b =>
                {
                    b.HasOne("JWTAuthentication.Model.UserAccount", "UserAccount")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("JWTAuthentication.Model.PlayerToGame", b =>
                {
                    b.HasOne("JWTAuthentication.Model.Game", "Game")
                        .WithMany()
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("JWTAuthentication.Model.UserAccount", "UserAccount")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("JWTAuthentication.Model.UserData", b =>
                {
                    b.HasOne("JWTAuthentication.Model.UserAccount", "UserAccount")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
