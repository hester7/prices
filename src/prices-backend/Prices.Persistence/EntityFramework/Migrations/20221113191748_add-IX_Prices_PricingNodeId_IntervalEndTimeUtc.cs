using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace Prices.Persistence.EntityFramework.Migrations
{
    public partial class addIX_Prices_PricingNodeId_IntervalEndTimeUtc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Prices_PricingNodeId",
                table: "Prices");

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16683670681382004L), NodaTime.Instant.FromUnixTimeTicks(16683670681382004L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16683670681382004L), NodaTime.Instant.FromUnixTimeTicks(16683670681382004L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16683670681382004L), NodaTime.Instant.FromUnixTimeTicks(16683670681382004L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16683670681382004L), NodaTime.Instant.FromUnixTimeTicks(16683670681382004L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16683670681382004L), NodaTime.Instant.FromUnixTimeTicks(16683670681382004L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16683670681382004L), NodaTime.Instant.FromUnixTimeTicks(16683670681382004L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16683670681382004L), NodaTime.Instant.FromUnixTimeTicks(16683670681382004L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16683670681382004L), NodaTime.Instant.FromUnixTimeTicks(16683670681382004L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16683670681382004L), NodaTime.Instant.FromUnixTimeTicks(16683670681382004L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16683670681382004L), NodaTime.Instant.FromUnixTimeTicks(16683670681382004L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16683670681382004L), NodaTime.Instant.FromUnixTimeTicks(16683670681382004L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16683670681382004L), NodaTime.Instant.FromUnixTimeTicks(16683670681382004L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16683670681382004L), NodaTime.Instant.FromUnixTimeTicks(16683670681382004L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16683670681382004L), NodaTime.Instant.FromUnixTimeTicks(16683670681382004L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16683670681382004L), NodaTime.Instant.FromUnixTimeTicks(16683670681382004L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16683670681382004L), NodaTime.Instant.FromUnixTimeTicks(16683670681382004L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16683670681382004L), NodaTime.Instant.FromUnixTimeTicks(16683670681382004L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16683670681382004L), NodaTime.Instant.FromUnixTimeTicks(16683670681382004L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16683670681382004L), NodaTime.Instant.FromUnixTimeTicks(16683670681382004L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16683670681382004L), NodaTime.Instant.FromUnixTimeTicks(16683670681382004L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16683670681382004L), NodaTime.Instant.FromUnixTimeTicks(16683670681382004L) });

            migrationBuilder.CreateIndex(
                name: "IX_Prices_PricingNodeId_IntervalEndTimeUtc",
                table: "Prices",
                columns: new[] { "PricingNodeId", "IntervalEndTimeUtc" })
                .Annotation("Npgsql:IndexInclude", new[] { "PriceIndexId", "LmpPrice" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Prices_PricingNodeId_IntervalEndTimeUtc",
                table: "Prices");

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), NodaTime.Instant.FromUnixTimeTicks(16681156760411555L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), NodaTime.Instant.FromUnixTimeTicks(16681156760411555L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), NodaTime.Instant.FromUnixTimeTicks(16681156760411555L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), NodaTime.Instant.FromUnixTimeTicks(16681156760411555L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), NodaTime.Instant.FromUnixTimeTicks(16681156760411555L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), NodaTime.Instant.FromUnixTimeTicks(16681156760411555L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), NodaTime.Instant.FromUnixTimeTicks(16681156760411555L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), NodaTime.Instant.FromUnixTimeTicks(16681156760411555L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), NodaTime.Instant.FromUnixTimeTicks(16681156760411555L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), NodaTime.Instant.FromUnixTimeTicks(16681156760411555L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), NodaTime.Instant.FromUnixTimeTicks(16681156760411555L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), NodaTime.Instant.FromUnixTimeTicks(16681156760411555L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), NodaTime.Instant.FromUnixTimeTicks(16681156760411555L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), NodaTime.Instant.FromUnixTimeTicks(16681156760411555L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), NodaTime.Instant.FromUnixTimeTicks(16681156760411555L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), NodaTime.Instant.FromUnixTimeTicks(16681156760411555L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), NodaTime.Instant.FromUnixTimeTicks(16681156760411555L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), NodaTime.Instant.FromUnixTimeTicks(16681156760411555L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), NodaTime.Instant.FromUnixTimeTicks(16681156760411555L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), NodaTime.Instant.FromUnixTimeTicks(16681156760411555L) });

            migrationBuilder.UpdateData(
                table: "PricingNodes",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "CreatedAtUtc", "LastModifiedAtUtc" },
                values: new object[] { NodaTime.Instant.FromUnixTimeTicks(16681156760411555L), NodaTime.Instant.FromUnixTimeTicks(16681156760411555L) });

            migrationBuilder.CreateIndex(
                name: "IX_Prices_PricingNodeId",
                table: "Prices",
                column: "PricingNodeId");
        }
    }
}
