using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spectre.Core.Migrations
{
    /// <inheritdoc />
    public partial class CreateOperatorTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
               name: "Operators",
               columns: table => new
               {
                   OperatorID = table.Column<int>(type: "int", nullable: false)
                       .Annotation("SqlServer:Identity", "1, 1"),
                   OperatorName = table.Column<string>(type: "nvarchar(max)", nullable: true)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_Operators", x => x.OperatorID);
               });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AboutSpectres");

            migrationBuilder.DropTable(
                name: "ActionLogOperations");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaim");

            migrationBuilder.DropTable(
                name: "AspNetUserClaim");

            migrationBuilder.DropTable(
                name: "AspNetUserLogin");

            migrationBuilder.DropTable(
                name: "AspNetUserRole");

            migrationBuilder.DropTable(
                name: "AspNetUserToken");

            migrationBuilder.DropTable(
                name: "Awards");

            migrationBuilder.DropTable(
                name: "Bands");

            migrationBuilder.DropTable(
                name: "Banners");

            migrationBuilder.DropTable(
                name: "CompanyPackageDetails");

            migrationBuilder.DropTable(
                name: "CompanyPreRegistration");

            migrationBuilder.DropTable(
                name: "ContactUs");

            migrationBuilder.DropTable(
                name: "ExceptionLogs");

            migrationBuilder.DropTable(
                name: "Features");

            migrationBuilder.DropTable(
                name: "FQA");

            migrationBuilder.DropTable(
                name: "Glossary");

            migrationBuilder.DropTable(
                name: "HelpAboutUs");

            migrationBuilder.DropTable(
                name: "HelpServices");

            migrationBuilder.DropTable(
                name: "HelpUsing");

            migrationBuilder.DropTable(
                name: "HomePageHeaders");

            migrationBuilder.DropTable(
                name: "Icons");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "LatestNews");

            migrationBuilder.DropTable(
                name: "Operators");

            migrationBuilder.DropTable(
                name: "Pages");

            migrationBuilder.DropTable(
                name: "Parameters");

            migrationBuilder.DropTable(
                name: "RegisterationRequests");

            migrationBuilder.DropTable(
                name: "ReportSnaps");

            migrationBuilder.DropTable(
                name: "Role_Permission");

            migrationBuilder.DropTable(
                name: "SavedFilters");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Sheet1$");

            migrationBuilder.DropTable(
                name: "SocioEonomics");

            migrationBuilder.DropTable(
                name: "SpectreFooters");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "TemplateFilterDetails");

            migrationBuilder.DropTable(
                name: "UserActionLogs");

            migrationBuilder.DropTable(
                name: "UserNotifications");

            migrationBuilder.DropTable(
                name: "VisualizingReports");

            migrationBuilder.DropTable(
                name: "AspNetRole");

            migrationBuilder.DropTable(
                name: "AspNetUser");

            migrationBuilder.DropTable(
                name: "CompanyPackages");

            migrationBuilder.DropTable(
                name: "PackagePagePermissions");

            migrationBuilder.DropTable(
                name: "aspnet_Permissions");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "TemplateFilters");

            migrationBuilder.DropTable(
                name: "Packages");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Lookups");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Lookups");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Lookups");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Lookups");

            migrationBuilder.DropColumn(
                name: "LookupCode",
                table: "Lookups");

            migrationBuilder.DropColumn(
                name: "ModiciationDate",
                table: "Lookups");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Lookups");

            migrationBuilder.DropColumn(
                name: "NameAr",
                table: "Lookups");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Lookups");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Lookups");

            migrationBuilder.DropColumn(
                name: "UserDefined",
                table: "Lookups");

            migrationBuilder.AlterDatabase(
                collation: "SQL_Latin1_General_CP1_CI_AS",
                oldCollation: "SQL_Latin1_General_CP1256_CI_AS");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Lookups",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);
        }
    }
}
