using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Prices.Persistence.EntityFramework.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IntervalEndTimeUtcScalar",
                columns: table => new
                {
                    IntervalEndTimeUtc = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "PriceMarkets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    Abbreviation = table.Column<string>(type: "character varying(3)", unicode: false, maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceMarkets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    PriceIndexId = table.Column<int>(type: "integer", nullable: false),
                    PricingNodeId = table.Column<int>(type: "integer", nullable: false),
                    IntervalEndTimeUtc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    IntervalStartTimeUtc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    IntervalLength = table.Column<int>(type: "integer", nullable: false),
                    LmpPrice = table.Column<decimal>(type: "numeric(19,9)", nullable: false),
                    EnergyPrice = table.Column<decimal>(type: "numeric(19,9)", nullable: false),
                    CongestionPrice = table.Column<decimal>(type: "numeric(19,9)", nullable: false),
                    LossPrice = table.Column<decimal>(type: "numeric(19,9)", nullable: false),
                    PricingNodeName = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    LastModifiedAtUtc = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => new { x.PriceIndexId, x.PricingNodeId, x.IntervalEndTimeUtc });
                });

            migrationBuilder.CreateTable(
                name: "PriceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PricingNodeTypeMappings",
                columns: table => new
                {
                    PricingNodeTypeId = table.Column<int>(type: "integer", nullable: false),
                    RegionalTransmissionOperatorId = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingNodeTypeMappings", x => new { x.PricingNodeTypeId, x.RegionalTransmissionOperatorId });
                });

            migrationBuilder.CreateTable(
                name: "PricingNodeTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingNodeTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegionalTransmissionOperators",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    LegalName = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionalTransmissionOperators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PriceIndexes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    RegionalTransmissionOperatorId = table.Column<int>(type: "integer", nullable: false),
                    PriceMarketId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceIndexes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceIndexes_PriceMarkets",
                        column: x => x.PriceMarketId,
                        principalTable: "PriceMarkets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PriceIndexes_RegionalTransmissionOperators",
                        column: x => x.RegionalTransmissionOperatorId,
                        principalTable: "RegionalTransmissionOperators",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PricingNodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RegionalTransmissionOperatorId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    PricingNodeTypeId = table.Column<int>(type: "integer", nullable: false),
                    StartDateUtc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    EndDateUtc = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    CurrentPrice = table.Column<decimal>(type: "numeric(19,9)", nullable: true),
                    Change24Hour = table.Column<decimal>(type: "numeric(19,9)", nullable: true),
                    CreatedAtUtc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAtUtc = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingNodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PricingNodes_PricingNodeTypes",
                        column: x => x.PricingNodeTypeId,
                        principalTable: "PricingNodeTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PricingNodes_RegionalTransmissionOperators",
                        column: x => x.RegionalTransmissionOperatorId,
                        principalTable: "RegionalTransmissionOperators",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PricesFiles",
                columns: table => new
                {
                    BlobName = table.Column<string>(type: "character varying(200)", unicode: false, maxLength: 200, nullable: false),
                    PriceIndexId = table.Column<int>(type: "integer", nullable: false),
                    PriceTypeId = table.Column<int>(type: "integer", nullable: false),
                    FileName = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: false),
                    VirtualFolder = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: false),
                    StartDateUtc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    EndDateUtc = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    FileSourceUrl = table.Column<string>(type: "character varying(2048)", unicode: false, maxLength: 2048, nullable: false),
                    DocumentId = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAtUtc = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricesFiles", x => x.BlobName);
                    table.ForeignKey(
                        name: "FK_PricesFiles_PriceIndexes",
                        column: x => x.PriceIndexId,
                        principalTable: "PriceIndexes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PricesFiles_PriceTypes",
                        column: x => x.PriceTypeId,
                        principalTable: "PriceTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "PriceMarkets",
                columns: new[] { "Id", "Abbreviation", "Name" },
                values: new object[,]
                {
                    { 1, "DAM", "Day-Ahead" },
                    { 2, "RTM", "Real-Time" }
                });

            migrationBuilder.InsertData(
                table: "PriceTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Historical" },
                    { 2, "Daily" },
                    { 3, "Current" }
                });

            migrationBuilder.InsertData(
                table: "PricingNodeTypeMappings",
                columns: new[] { "PricingNodeTypeId", "RegionalTransmissionOperatorId", "Code" },
                values: new object[,]
                {
                    { 1, 1, "TH" },
                    { 2, 1, "DPZ" }
                });

            migrationBuilder.InsertData(
                table: "PricingNodeTypes",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 0, "Unknown", "Unknown" },
                    { 1, "Trading Hub", "Hub" },
                    { 2, "Default Load Aggregation Point", "DLAP" }
                });

            migrationBuilder.InsertData(
                table: "RegionalTransmissionOperators",
                columns: new[] { "Id", "LegalName", "Name" },
                values: new object[,]
                {
                    { 1, "California ISO", "CAISO" },
                    { 2, "Electric Reliability Council of Texas", "ERCOT" }
                });

            migrationBuilder.InsertData(
                table: "PriceIndexes",
                columns: new[] { "Id", "Name", "PriceMarketId", "RegionalTransmissionOperatorId" },
                values: new object[,]
                {
                    { 1, "CAISO - Day-Ahead", 1, 1 },
                    { 2, "CAISO - Real-Time", 2, 1 },
                    { 3, "ERCOT - Day-Ahead", 1, 2 },
                    { 4, "ERCOT - Real-Time", 2, 2 }
                });

            migrationBuilder.InsertData(
                table: "PricingNodes",
                columns: new[] { "Id", "Change24Hour", "CreatedAtUtc", "CurrentPrice", "DisplayName", "EndDateUtc", "LastModifiedAtUtc", "Name", "PricingNodeTypeId", "RegionalTransmissionOperatorId", "StartDateUtc" },
                values: new object[,]
                {
                    { 1, null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), null, "NP15", null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), "TH_NP15_GEN-APND", 1, 1, null },
                    { 2, null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), null, "SP15", null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), "TH_SP15_GEN-APND", 1, 1, null },
                    { 3, null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), null, "ZP26", null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), "TH_ZP26_GEN-APND", 1, 1, null },
                    { 4, null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), null, "PGE", null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), "DLAP_PGAE-APND", 2, 1, null },
                    { 5, null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), null, "SCE", null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), "DLAP_SCE-APND", 2, 1, null },
                    { 6, null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), null, "SDGE", null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), "DLAP_SDGE-APND", 2, 1, null },
                    { 7, null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), null, "VEA", null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), "DLAP_VEA-APND", 2, 1, null },
                    { 8, null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), null, "HB_BUSAVG", null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), "HB_BUSAVG", 1, 2, null },
                    { 9, null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), null, "HB_HOUSTON", null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), "HB_HOUSTON", 1, 2, null },
                    { 10, null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), null, "HB_HUBAVG", null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), "HB_HUBAVG", 1, 2, null },
                    { 11, null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), null, "HB_NORTH", null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), "HB_NORTH", 1, 2, null },
                    { 12, null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), null, "HB_SOUTH", null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), "HB_SOUTH", 1, 2, null },
                    { 13, null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), null, "HB_WEST", null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), "HB_WEST", 1, 2, null },
                    { 14, null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), null, "LZ_AEN", null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), "LZ_AEN", 2, 2, null },
                    { 15, null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), null, "LZ_CPS", null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), "LZ_CPS", 2, 2, null },
                    { 16, null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), null, "LZ_HOUSTON", null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), "LZ_HOUSTON", 2, 2, null },
                    { 17, null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), null, "LZ_LCRA", null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), "LZ_LCRA", 2, 2, null },
                    { 18, null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), null, "LZ_NORTH", null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), "LZ_NORTH", 2, 2, null },
                    { 19, null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), null, "LZ_RAYBN", null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), "LZ_RAYBN", 2, 2, null },
                    { 20, null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), null, "LZ_SOUTH", null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), "LZ_SOUTH", 2, 2, null },
                    { 21, null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), null, "LZ_WEST", null, NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), "LZ_WEST", 2, 2, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PriceIndexes_Name",
                table: "PriceIndexes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PriceIndexes_PriceMarketId",
                table: "PriceIndexes",
                column: "PriceMarketId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceIndexes_RegionalTransmissionOperatorId",
                table: "PriceIndexes",
                column: "RegionalTransmissionOperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceIndexes_RegionalTransmissionOperatorId_PriceMarketId",
                table: "PriceIndexes",
                columns: new[] { "RegionalTransmissionOperatorId", "PriceMarketId" });

            migrationBuilder.CreateIndex(
                name: "IX_PriceMarkets_Abbreviation",
                table: "PriceMarkets",
                column: "Abbreviation",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PriceMarkets_Name",
                table: "PriceMarkets",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prices_PriceIndexId_IntervalEndTimeUtc",
                table: "Prices",
                columns: new[] { "PriceIndexId", "IntervalEndTimeUtc" })
                .Annotation("Npgsql:IndexInclude", new[] { "PricingNodeId", "LmpPrice" });

            migrationBuilder.CreateIndex(
                name: "IX_Prices_PricingNodeId",
                table: "Prices",
                column: "PricingNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_PricesFiles_PriceIndexId",
                table: "PricesFiles",
                column: "PriceIndexId");

            migrationBuilder.CreateIndex(
                name: "IX_PricesFiles_PriceTypeId",
                table: "PricesFiles",
                column: "PriceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PricingNodes_PricingNodeTypeId",
                table: "PricingNodes",
                column: "PricingNodeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PricingNodes_RegionalTransmissionOperatorId_DisplayName",
                table: "PricingNodes",
                columns: new[] { "RegionalTransmissionOperatorId", "DisplayName" },
                unique: true,
                filter: "('DisplayName' IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_PricingNodes_RegionalTransmissionOperatorId_Name",
                table: "PricingNodes",
                columns: new[] { "RegionalTransmissionOperatorId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PricingNodeTypeMappings_RegionalTransmissionOperatorId",
                table: "PricingNodeTypeMappings",
                column: "RegionalTransmissionOperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_PricingNodeTypes_Name",
                table: "PricingNodeTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegionalTransmissionOperators_LegalName",
                table: "RegionalTransmissionOperators",
                column: "LegalName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegionalTransmissionOperators_Name",
                table: "RegionalTransmissionOperators",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntervalEndTimeUtcScalar");

            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropTable(
                name: "PricesFiles");

            migrationBuilder.DropTable(
                name: "PricingNodes");

            migrationBuilder.DropTable(
                name: "PricingNodeTypeMappings");

            migrationBuilder.DropTable(
                name: "PriceIndexes");

            migrationBuilder.DropTable(
                name: "PriceTypes");

            migrationBuilder.DropTable(
                name: "PricingNodeTypes");

            migrationBuilder.DropTable(
                name: "PriceMarkets");

            migrationBuilder.DropTable(
                name: "RegionalTransmissionOperators");
        }
    }
}
