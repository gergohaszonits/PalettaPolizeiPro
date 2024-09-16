using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PalettaPolizeiPro.Migrations
{
    /// <inheritdoc />
    public partial class postgresdatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Palettas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Identifier = table.Column<string>(type: "text", nullable: true),
                    Loop = table.Column<int>(type: "integer", nullable: false),
                    ServiceFlag = table.Column<bool>(type: "boolean", nullable: false),
                    PalettaError = table.Column<bool>(type: "boolean", nullable: false),
                    Marked = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Palettas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IP = table.Column<string>(type: "text", nullable: false),
                    Rack = table.Column<int>(type: "integer", nullable: false),
                    Slot = table.Column<int>(type: "integer", nullable: false),
                    IsStationOn = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Loop = table.Column<int>(type: "integer", nullable: false),
                    DB = table.Column<int>(type: "integer", nullable: false),
                    StationType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    WorkerID = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PalettaProperties",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Identifier = table.Column<string>(type: "text", nullable: false),
                    PredefiniedCycle = table.Column<int>(type: "integer", nullable: false),
                    ActualCycle = table.Column<int>(type: "integer", nullable: false),
                    EngineNumber = table.Column<string>(type: "text", nullable: true),
                    ReadTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PalettaId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PalettaProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PalettaProperties_Palettas_PalettaId",
                        column: x => x.PalettaId,
                        principalTable: "Palettas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "QueryEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PalettaName = table.Column<string>(type: "text", nullable: true),
                    OperationStatus = table.Column<byte>(type: "smallint", nullable: true),
                    ControlFlag = table.Column<byte>(type: "smallint", nullable: true),
                    PalettaId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueryEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QueryEvents_Palettas_PalettaId",
                        column: x => x.PalettaId,
                        principalTable: "Palettas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    InfoText = table.Column<string>(type: "text", nullable: true),
                    ScheduledTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FinishedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StartSortTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndSortTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MaximumSort = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: true),
                    Discriminator = table.Column<string>(type: "character varying(21)", maxLength: 21, nullable: false),
                    PalettaId = table.Column<long>(type: "bigint", nullable: true),
                    QueryStateId = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Palettas_PalettaId",
                        column: x => x.PalettaId,
                        principalTable: "Palettas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_QueryEvents_QueryStateId",
                        column: x => x.QueryStateId,
                        principalTable: "QueryEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderPaletta",
                columns: table => new
                {
                    InScheduledId = table.Column<long>(type: "bigint", nullable: false),
                    ScheduledPalettasId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPaletta", x => new { x.InScheduledId, x.ScheduledPalettasId });
                    table.ForeignKey(
                        name: "FK_OrderPaletta_Orders_InScheduledId",
                        column: x => x.InScheduledId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderPaletta_Palettas_ScheduledPalettasId",
                        column: x => x.ScheduledPalettasId,
                        principalTable: "Palettas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderPaletta1",
                columns: table => new
                {
                    FinishedPalettasId = table.Column<long>(type: "bigint", nullable: false),
                    InFinishedId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPaletta1", x => new { x.FinishedPalettasId, x.InFinishedId });
                    table.ForeignKey(
                        name: "FK_OrderPaletta1_Orders_InFinishedId",
                        column: x => x.InFinishedId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderPaletta1_Palettas_FinishedPalettasId",
                        column: x => x.FinishedPalettasId,
                        principalTable: "Palettas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_PalettaId",
                table: "Notifications",
                column: "PalettaId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_QueryStateId",
                table: "Notifications",
                column: "QueryStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPaletta_ScheduledPalettasId",
                table: "OrderPaletta",
                column: "ScheduledPalettasId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPaletta1_InFinishedId",
                table: "OrderPaletta1",
                column: "InFinishedId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PalettaProperties_PalettaId",
                table: "PalettaProperties",
                column: "PalettaId");

            migrationBuilder.CreateIndex(
                name: "IX_QueryEvents_PalettaId",
                table: "QueryEvents",
                column: "PalettaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "OrderPaletta");

            migrationBuilder.DropTable(
                name: "OrderPaletta1");

            migrationBuilder.DropTable(
                name: "PalettaProperties");

            migrationBuilder.DropTable(
                name: "Stations");

            migrationBuilder.DropTable(
                name: "QueryEvents");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Palettas");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
