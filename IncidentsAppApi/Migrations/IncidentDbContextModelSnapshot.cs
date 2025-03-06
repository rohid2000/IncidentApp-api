﻿// <auto-generated />
using IncidentsAppApi.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace IncidentsAppApi.Migrations
{
    [DbContext(typeof(IncidentDbContext))]
    partial class IncidentDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("IncidentsAppApi.Models.Incident", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Priority")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Incidents");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "Poep op de stoep",
                            Priority = "Hoog",
                            Status = "In behandeling",
                            UserId = 0
                        },
                        new
                        {
                            Id = 2,
                            Description = "Omgevallen boom",
                            Priority = "Hoog",
                            Status = "In behandeling",
                            UserId = 0
                        },
                        new
                        {
                            Id = 3,
                            Description = "Graffiti op de muur in een tunnel",
                            Priority = "Laag",
                            Status = "Gemeld",
                            UserId = 0
                        });
                });

            modelBuilder.Entity("IncidentsAppApi.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("bit");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            IsAdmin = true,
                            Password = "test123",
                            Username = "rsewpersad"
                        });
                });

            modelBuilder.Entity("IncidentsAppApi.Models.Incident", b =>
                {
                    b.HasOne("IncidentsAppApi.Models.User", "User")
                        .WithMany("Incidents")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("IncidentsAppApi.Models.User", b =>
                {
                    b.Navigation("Incidents");
                });
#pragma warning restore 612, 618
        }
    }
}
