﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using OxfordParser.Data;

namespace OxfordParser.Data.Migrations
{
    [DbContext(typeof(WordsDbContext))]
    partial class WordsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.16")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("OxfordParser.Data.Entities.Word", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("PicturePath")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<string>("SoundPathUK")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<string>("SoundPathUS")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<int>("WordLevel")
                        .HasColumnType("integer");

                    b.Property<int>("WordTypeId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("WordTypeId");

                    b.ToTable("Words");
                });

            modelBuilder.Entity("OxfordParser.Data.Entities.WordCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("NameEng")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("NameRu")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("PicturePath")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.HasKey("Id");

                    b.ToTable("WordCategories");
                });

            modelBuilder.Entity("OxfordParser.Data.Entities.WordType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("NameEng")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("NameRu")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.HasKey("Id");

                    b.ToTable("WordTypes");
                });

            modelBuilder.Entity("OxfordParser.Data.Entities.WordUsage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Comment")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<int?>("WordCategoryId")
                        .HasColumnType("integer");

                    b.Property<int>("WordId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("WordCategoryId");

                    b.HasIndex("WordId");

                    b.ToTable("WordUsages");
                });

            modelBuilder.Entity("WordWordCategory", b =>
                {
                    b.Property<int>("CategoriesId")
                        .HasColumnType("integer");

                    b.Property<int>("WordsId")
                        .HasColumnType("integer");

                    b.HasKey("CategoriesId", "WordsId");

                    b.HasIndex("WordsId");

                    b.ToTable("WordAndWordCategory");
                });

            modelBuilder.Entity("OxfordParser.Data.Entities.Word", b =>
                {
                    b.HasOne("OxfordParser.Data.Entities.WordType", "WordType")
                        .WithMany()
                        .HasForeignKey("WordTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("WordType");
                });

            modelBuilder.Entity("OxfordParser.Data.Entities.WordUsage", b =>
                {
                    b.HasOne("OxfordParser.Data.Entities.WordCategory", "WordCategory")
                        .WithMany()
                        .HasForeignKey("WordCategoryId");

                    b.HasOne("OxfordParser.Data.Entities.Word", "Word")
                        .WithMany("Usages")
                        .HasForeignKey("WordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Word");

                    b.Navigation("WordCategory");
                });

            modelBuilder.Entity("WordWordCategory", b =>
                {
                    b.HasOne("OxfordParser.Data.Entities.WordCategory", null)
                        .WithMany()
                        .HasForeignKey("CategoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OxfordParser.Data.Entities.Word", null)
                        .WithMany()
                        .HasForeignKey("WordsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OxfordParser.Data.Entities.Word", b =>
                {
                    b.Navigation("Usages");
                });
#pragma warning restore 612, 618
        }
    }
}
