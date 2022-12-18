﻿// <auto-generated />
using System;
using Apsitvarkom.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Apsitvarkom.DataAccess.Migrations
{
    [DbContext(typeof(PollutedLocationContext))]
    [Migration("20221218115321_DatabaseTriggers")]
    partial class DatabaseTriggers
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Apsitvarkom.Models.CleaningEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("IsFinalized")
                        .HasColumnType("boolean");

                    b.Property<string>("Notes")
                        .HasColumnType("text");

                    b.Property<Guid>("PollutedLocationId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("PollutedLocationId");

                    b.ToTable("CleaningEvents");
                });

            modelBuilder.Entity("Apsitvarkom.Models.PollutedLocation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Notes")
                        .HasColumnType("text");

                    b.Property<int>("Progress")
                        .HasColumnType("integer");

                    b.Property<int>("Radius")
                        .HasColumnType("integer");

                    b.Property<string>("Severity")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("Spotted")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("PollutedLocations", t =>
                        {
                            t.HasCheckConstraint("CK_PollutedLocation_Progress", "\"Progress\" >= 0 and \"Progress\" <= 100");

                            t.HasCheckConstraint("CK_PollutedLocation_Radius", "\"Radius\" >= 1");

                            t.HasCheckConstraint("CK_PollutedLocation_Severity", "\"Severity\" in ('Low', 'Moderate', 'High')");
                        });
                });

            modelBuilder.Entity("Apsitvarkom.Models.CleaningEvent", b =>
                {
                    b.HasOne("Apsitvarkom.Models.PollutedLocation", null)
                        .WithMany("Events")
                        .HasForeignKey("PollutedLocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Apsitvarkom.Models.PollutedLocation", b =>
                {
                    b.OwnsOne("Apsitvarkom.Models.Location", "Location", b1 =>
                        {
                            b1.Property<Guid>("PollutedLocationId")
                                .HasColumnType("uuid");

                            b1.HasKey("PollutedLocationId");

                            b1.ToTable("PollutedLocations");

                            b1.WithOwner()
                                .HasForeignKey("PollutedLocationId");

                            b1.OwnsOne("Apsitvarkom.Models.Coordinates", "Coordinates", b2 =>
                                {
                                    b2.Property<Guid>("LocationPollutedLocationId")
                                        .HasColumnType("uuid");

                                    b2.Property<double>("Latitude")
                                        .HasColumnType("double precision");

                                    b2.Property<double>("Longitude")
                                        .HasColumnType("double precision");

                                    b2.HasKey("LocationPollutedLocationId");

                                    b2.ToTable("PollutedLocations", t =>
                                        {
                                            t.HasCheckConstraint("CK_Coordinates_Latitude", "\"Location_Coordinates_Latitude\" >= -90 and \"Location_Coordinates_Latitude\" <= 90");

                                            t.HasCheckConstraint("CK_Coordinates_Longitude", "\"Location_Coordinates_Longitude\" >= -180 and \"Location_Coordinates_Longitude\" <= 180");
                                        });

                                    b2.WithOwner()
                                        .HasForeignKey("LocationPollutedLocationId");
                                });

                            b1.OwnsOne("Apsitvarkom.Models.Translated<string>", "Title", b2 =>
                                {
                                    b2.Property<Guid>("LocationPollutedLocationId")
                                        .HasColumnType("uuid");

                                    b2.Property<string>("English")
                                        .HasColumnType("text");

                                    b2.Property<string>("Lithuanian")
                                        .HasColumnType("text");

                                    b2.HasKey("LocationPollutedLocationId");

                                    b2.ToTable("PollutedLocations");

                                    b2.WithOwner()
                                        .HasForeignKey("LocationPollutedLocationId");
                                });

                            b1.Navigation("Coordinates")
                                .IsRequired();

                            b1.Navigation("Title")
                                .IsRequired();
                        });

                    b.Navigation("Location")
                        .IsRequired();
                });

            modelBuilder.Entity("Apsitvarkom.Models.PollutedLocation", b =>
                {
                    b.Navigation("Events");
                });
#pragma warning restore 612, 618
        }
    }
}
