using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PalettaPolizeiPro.Migrations
{
    /// <inheritdoc />
    public partial class postgresdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Eks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KeyId = table.Column<string>(type: "text", nullable: true),
                    WorkerId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eks", x => x.Id);
                });

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
                    Marked = table.Column<bool>(type: "boolean", nullable: false),
                    IsOut = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Palettas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServerNotifications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ip = table.Column<string>(type: "text", nullable: false),
                    Rack = table.Column<int>(type: "integer", nullable: false),
                    Slot = table.Column<int>(type: "integer", nullable: false),
                    IsStationOn = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Loop = table.Column<int>(type: "integer", nullable: false),
                    DB = table.Column<int>(type: "integer", nullable: false),
                    StationPcIp = table.Column<string>(type: "text", nullable: true),
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
                    Role = table.Column<int>(type: "integer", nullable: false),
                    EksId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Eks_EksId",
                        column: x => x.EksId,
                        principalTable: "Eks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PalettaProperties",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Identifier = table.Column<string>(type: "text", nullable: false),
                    PalettaId = table.Column<long>(type: "bigint", nullable: false),
                    PredefiniedCycle = table.Column<int>(type: "integer", nullable: false),
                    ActualCycle = table.Column<int>(type: "integer", nullable: false),
                    EngineNumber = table.Column<string>(type: "text", nullable: true),
                    ReadTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PalettaProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PalettaProperties_Palettas_PalettaId",
                        column: x => x.PalettaId,
                        principalTable: "Palettas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QueryStates",
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
                    table.PrimaryKey("PK_QueryStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QueryStates_Palettas_PalettaId",
                        column: x => x.PalettaId,
                        principalTable: "Palettas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EksEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    State = table.Column<int>(type: "integer", nullable: false),
                    EksWorkerId = table.Column<string>(type: "text", nullable: false),
                    EksKeyId = table.Column<string>(type: "text", nullable: true),
                    StationId = table.Column<long>(type: "bigint", nullable: false),
                    Time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EksEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EksEvents_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    Time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    InfoText = table.Column<string>(type: "text", nullable: true),
                    ScheduledTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FinishedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    StartSortTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EndSortTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
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
                name: "CheckEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StationId = table.Column<long>(type: "bigint", nullable: false),
                    PropertyId = table.Column<long>(type: "bigint", nullable: false),
                    Time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckEvents_PalettaProperties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "PalettaProperties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CheckEvents_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QueryEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StationId = table.Column<long>(type: "bigint", nullable: false),
                    StateId = table.Column<long>(type: "bigint", nullable: false),
                    Time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueryEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QueryEvents_QueryStates_StateId",
                        column: x => x.StateId,
                        principalTable: "QueryStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QueryEvents_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderPalettaFinishes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PalettaId = table.Column<long>(type: "bigint", nullable: false),
                    OrderId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPalettaFinishes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderPalettaFinishes_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderPalettaFinishes_Palettas_PalettaId",
                        column: x => x.PalettaId,
                        principalTable: "Palettas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderPalettaSchedules",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PalettaId = table.Column<long>(type: "bigint", nullable: false),
                    OrderId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPalettaSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderPalettaSchedules_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderPalettaSchedules_Palettas_PalettaId",
                        column: x => x.PalettaId,
                        principalTable: "Palettas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CheckEvents_PropertyId",
                table: "CheckEvents",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckEvents_StationId",
                table: "CheckEvents",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_EksEvents_StationId",
                table: "EksEvents",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_UserId",
                table: "Feedbacks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPalettaFinishes_OrderId",
                table: "OrderPalettaFinishes",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPalettaFinishes_PalettaId",
                table: "OrderPalettaFinishes",
                column: "PalettaId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPalettaSchedules_OrderId",
                table: "OrderPalettaSchedules",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPalettaSchedules_PalettaId",
                table: "OrderPalettaSchedules",
                column: "PalettaId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PalettaProperties_PalettaId",
                table: "PalettaProperties",
                column: "PalettaId");

            migrationBuilder.CreateIndex(
                name: "IX_QueryEvents_StateId",
                table: "QueryEvents",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_QueryEvents_StationId",
                table: "QueryEvents",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_QueryStates_PalettaId",
                table: "QueryStates",
                column: "PalettaId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_EksId",
                table: "Users",
                column: "EksId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CheckEvents");

            migrationBuilder.DropTable(
                name: "EksEvents");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "OrderPalettaFinishes");

            migrationBuilder.DropTable(
                name: "OrderPalettaSchedules");

            migrationBuilder.DropTable(
                name: "QueryEvents");

            migrationBuilder.DropTable(
                name: "ServerNotifications");

            migrationBuilder.DropTable(
                name: "PalettaProperties");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "QueryStates");

            migrationBuilder.DropTable(
                name: "Stations");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Palettas");

            migrationBuilder.DropTable(
                name: "Eks");
        }
    }
}
