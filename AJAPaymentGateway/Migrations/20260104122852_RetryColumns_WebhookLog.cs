using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AJAPaymentGateway.Migrations
{
    /// <inheritdoc />
    public partial class RetryColumns_WebhookLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSuccess",
                table: "WebhookLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastRetryAt",
                table: "WebhookLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                table: "WebhookLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSuccess",
                table: "WebhookLogs");

            migrationBuilder.DropColumn(
                name: "LastRetryAt",
                table: "WebhookLogs");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "WebhookLogs");
        }
    }
}
