using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apsitvarkom.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PollutedLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Radius = table.Column<int>(type: "integer", nullable: false),
                    Severity = table.Column<string>(type: "text", nullable: false),
                    Spotted = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Progress = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    LocationTitleEnglish = table.Column<string>(name: "Location_Title_English", type: "text", nullable: true),
                    LocationTitleLithuanian = table.Column<string>(name: "Location_Title_Lithuanian", type: "text", nullable: true),
                    LocationCoordinatesLatitude = table.Column<double>(name: "Location_Coordinates_Latitude", type: "double precision", nullable: false),
                    LocationCoordinatesLongitude = table.Column<double>(name: "Location_Coordinates_Longitude", type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollutedLocations", x => x.Id);
                    table.CheckConstraint("CK_Coordinates_Latitude", "\"Location_Coordinates_Latitude\" >= -90 and \"Location_Coordinates_Latitude\" <= 90");
                    table.CheckConstraint("CK_Coordinates_Longitude", "\"Location_Coordinates_Longitude\" >= -180 and \"Location_Coordinates_Longitude\" <= 180");
                    table.CheckConstraint("CK_PollutedLocation_Progress", "\"Progress\" >= 0 and \"Progress\" <= 100");
                    table.CheckConstraint("CK_PollutedLocation_Radius", "\"Radius\" >= 1");
                    table.CheckConstraint("CK_PollutedLocation_Severity", "\"Severity\" in ('Low', 'Moderate', 'High')");
                });

            migrationBuilder.CreateTable(
                name: "CleaningEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PollutedLocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    IsFinalized = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CleaningEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CleaningEvents_PollutedLocations_PollutedLocationId",
                        column: x => x.PollutedLocationId,
                        principalTable: "PollutedLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CleaningEvents_PollutedLocationId",
                table: "CleaningEvents",
                column: "PollutedLocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CleaningEvents");

            migrationBuilder.DropTable(
                name: "PollutedLocations");
        }
    }
}
