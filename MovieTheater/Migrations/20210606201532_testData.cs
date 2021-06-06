using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MovieTheater.Migrations
{
    public partial class testData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Actors",
                columns: new[] { "Id", "DateOfBirth", "Name", "Photo" },
                values: new object[,]
                {
                    { 5, new DateTime(1962, 1, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jim Carrey", null },
                    { 6, new DateTime(1965, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Robert Downey Jr.", null },
                    { 7, new DateTime(1981, 6, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chris Evans", null }
                });

            migrationBuilder.InsertData(
                table: "Genders",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 4, "Adventure" },
                    { 5, "Animation" },
                    { 6, "Suspense" },
                    { 7, "Romance" }
                });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "AtCinema", "Poster", "PremiereDate", "Title" },
                values: new object[,]
                {
                    { 2, true, null, new DateTime(2019, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Avengers: Endgame" },
                    { 3, false, null, new DateTime(2019, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Avengers: Infinity Wars" },
                    { 4, false, null, new DateTime(2020, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sonic the Hedgehog" },
                    { 5, false, null, new DateTime(2020, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Emma" },
                    { 6, false, null, new DateTime(2020, 8, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Wonder woman 1984" }
                });

            migrationBuilder.InsertData(
                table: "MovieActors",
                columns: new[] { "ActorId", "MovieId", "Character", "Order" },
                values: new object[,]
                {
                    { 6, 2, "Tony Stark", 1 },
                    { 7, 2, "Steve Rogers", 2 },
                    { 6, 3, "Tony Stark", 1 },
                    { 7, 3, "Steve Rogers", 2 },
                    { 5, 4, "Dr. Ivo Robotnik", 1 }
                });

            migrationBuilder.InsertData(
                table: "MovieGenders",
                columns: new[] { "GenderId", "MovieId" },
                values: new object[,]
                {
                    { 6, 2 },
                    { 4, 2 },
                    { 6, 3 },
                    { 4, 3 },
                    { 4, 4 },
                    { 6, 5 },
                    { 7, 5 },
                    { 6, 6 },
                    { 4, 6 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "MovieActors",
                keyColumns: new[] { "ActorId", "MovieId" },
                keyValues: new object[] { 5, 4 });

            migrationBuilder.DeleteData(
                table: "MovieActors",
                keyColumns: new[] { "ActorId", "MovieId" },
                keyValues: new object[] { 6, 2 });

            migrationBuilder.DeleteData(
                table: "MovieActors",
                keyColumns: new[] { "ActorId", "MovieId" },
                keyValues: new object[] { 6, 3 });

            migrationBuilder.DeleteData(
                table: "MovieActors",
                keyColumns: new[] { "ActorId", "MovieId" },
                keyValues: new object[] { 7, 2 });

            migrationBuilder.DeleteData(
                table: "MovieActors",
                keyColumns: new[] { "ActorId", "MovieId" },
                keyValues: new object[] { 7, 3 });

            migrationBuilder.DeleteData(
                table: "MovieGenders",
                keyColumns: new[] { "GenderId", "MovieId" },
                keyValues: new object[] { 4, 2 });

            migrationBuilder.DeleteData(
                table: "MovieGenders",
                keyColumns: new[] { "GenderId", "MovieId" },
                keyValues: new object[] { 4, 3 });

            migrationBuilder.DeleteData(
                table: "MovieGenders",
                keyColumns: new[] { "GenderId", "MovieId" },
                keyValues: new object[] { 4, 4 });

            migrationBuilder.DeleteData(
                table: "MovieGenders",
                keyColumns: new[] { "GenderId", "MovieId" },
                keyValues: new object[] { 4, 6 });

            migrationBuilder.DeleteData(
                table: "MovieGenders",
                keyColumns: new[] { "GenderId", "MovieId" },
                keyValues: new object[] { 6, 2 });

            migrationBuilder.DeleteData(
                table: "MovieGenders",
                keyColumns: new[] { "GenderId", "MovieId" },
                keyValues: new object[] { 6, 3 });

            migrationBuilder.DeleteData(
                table: "MovieGenders",
                keyColumns: new[] { "GenderId", "MovieId" },
                keyValues: new object[] { 6, 5 });

            migrationBuilder.DeleteData(
                table: "MovieGenders",
                keyColumns: new[] { "GenderId", "MovieId" },
                keyValues: new object[] { 6, 6 });

            migrationBuilder.DeleteData(
                table: "MovieGenders",
                keyColumns: new[] { "GenderId", "MovieId" },
                keyValues: new object[] { 7, 5 });

            migrationBuilder.DeleteData(
                table: "Actors",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Actors",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Actors",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
