using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeServer.Formula1Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BroadcastName = table.Column<string>(type: "TEXT", nullable: true),
                    DriverNumber = table.Column<int>(type: "INTEGER", nullable: true),
                    DriverName = table.Column<string>(type: "TEXT", nullable: true),
                    NameAcronym = table.Column<string>(type: "TEXT", nullable: true),
                    HeadshotUrl = table.Column<string>(type: "TEXT", nullable: true),
                    TeamName = table.Column<string>(type: "TEXT", nullable: true),
                    TeamColor = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Laps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DriverId = table.Column<int>(type: "INTEGER", nullable: false),
                    SessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    LapNumber = table.Column<int>(type: "INTEGER", nullable: true),
                    TireCompound = table.Column<string>(type: "TEXT", nullable: false),
                    LapTime = table.Column<float>(type: "REAL", nullable: true),
                    Sector1 = table.Column<float>(type: "REAL", nullable: true),
                    Sector2 = table.Column<float>(type: "REAL", nullable: true),
                    Sector3 = table.Column<float>(type: "REAL", nullable: true),
                    MiniSectors1 = table.Column<string>(type: "TEXT", nullable: false),
                    MiniSectors2 = table.Column<string>(type: "TEXT", nullable: false),
                    MiniSectors3 = table.Column<string>(type: "TEXT", nullable: false),
                    SpeedTrap = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Laps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Meetings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Year = table.Column<int>(type: "INTEGER", nullable: true),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    MeetingName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meetings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SessionResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    MeetingId = table.Column<int>(type: "INTEGER", nullable: false),
                    MeetingKey = table.Column<int>(type: "INTEGER", nullable: false),
                    SessionKey = table.Column<int>(type: "INTEGER", nullable: false),
                    DNF = table.Column<bool>(type: "INTEGER", nullable: true),
                    DNS = table.Column<bool>(type: "INTEGER", nullable: true),
                    DSQ = table.Column<bool>(type: "INTEGER", nullable: true),
                    DriverNumber = table.Column<int>(type: "INTEGER", nullable: true),
                    FinishingPosition = table.Column<int>(type: "INTEGER", nullable: true),
                    GapToLeader = table.Column<float>(type: "REAL", nullable: false),
                    NumberOfPitstops = table.Column<int>(type: "INTEGER", nullable: true),
                    Tires = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MeetingId = table.Column<int>(type: "INTEGER", nullable: false),
                    MeetingKey = table.Column<int>(type: "INTEGER", nullable: false),
                    CircuitKey = table.Column<int>(type: "INTEGER", nullable: true),
                    SessionName = table.Column<string>(type: "TEXT", nullable: true),
                    SessionType = table.Column<string>(type: "TEXT", nullable: true),
                    TrackTemp = table.Column<int>(type: "INTEGER", nullable: true),
                    Rainfall = table.Column<bool>(type: "INTEGER", nullable: false),
                    AirPressure = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_Id",
                table: "Drivers",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Laps_Id",
                table: "Laps",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_Id",
                table: "Meetings",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SessionResults_Id",
                table: "SessionResults",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_Id",
                table: "Sessions",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropTable(
                name: "Laps");

            migrationBuilder.DropTable(
                name: "Meetings");

            migrationBuilder.DropTable(
                name: "SessionResults");

            migrationBuilder.DropTable(
                name: "Sessions");
        }
    }
}
