using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationDB.Migrations.System
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ts_ImportLog",
                columns: table => new
                {
                    JobLogId = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    InterfaceCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    InterfaceName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FtpServername = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FileFolder = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    InterfaceFilename = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    JobStatus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProcessBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartDatetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinishDatetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalRecord = table.Column<int>(type: "int", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ts_ImportLog", x => x.JobLogId);
                });

            migrationBuilder.CreateTable(
                name: "ts_ImportLogDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobLogId = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    RowNo = table.Column<int>(type: "int", nullable: false),
                    ErrorDetail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ts_ImportLogDetail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ts_LocalizedMessages",
                columns: table => new
                {
                    MessageCode = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    MessageType = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    MessageNameEN = table.Column<string>(type: "VARCHAR(500)", nullable: true),
                    MessageNameTH = table.Column<string>(type: "VARCHAR(500)", nullable: true),
                    Remark = table.Column<string>(type: "VARCHAR(256)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateBy = table.Column<string>(type: "VARCHAR(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ts_LocalizedMessages", x => new { x.MessageCode, x.MessageType });
                });

            migrationBuilder.CreateTable(
                name: "ts_LocalizedResources",
                columns: table => new
                {
                    ScreenCode = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    ObjectID = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    ResourcesEN = table.Column<string>(type: "VARCHAR(256)", nullable: true),
                    ResourcesTH = table.Column<string>(type: "VARCHAR(256)", nullable: true),
                    Remark = table.Column<string>(type: "VARCHAR(256)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateBy = table.Column<string>(type: "VARCHAR(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ts_LocalizedResources", x => new { x.ScreenCode, x.ObjectID });
                });

            migrationBuilder.CreateTable(
                name: "ts_Module",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModuleNameEN = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModuleNameTH = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Seq = table.Column<int>(type: "int", nullable: false),
                    IconClass = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ts_Module", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ts_Screen",
                columns: table => new
                {
                    ScreenId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameEN = table.Column<string>(type: "varchar(100)", nullable: false),
                    NameTH = table.Column<string>(type: "varchar(100)", nullable: false),
                    SupportDeviceType = table.Column<string>(type: "varchar(100)", nullable: true),
                    FunctionCode = table.Column<int>(type: "int", nullable: false),
                    ModuleCode = table.Column<string>(type: "varchar(50)", nullable: false),
                    SubModuleCode = table.Column<string>(type: "varchar(50)", nullable: true),
                    IconClass = table.Column<string>(type: "varchar(50)", nullable: true),
                    MainMenuFlag = table.Column<bool>(type: "bit", nullable: false),
                    PermissionFlag = table.Column<bool>(type: "bit", nullable: false),
                    Seq = table.Column<int>(type: "int", nullable: false),
                    PageTitleNameEN = table.Column<string>(type: "varchar(100)", nullable: true),
                    PageTitleNameTH = table.Column<string>(type: "varchar(100)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ts_Screen", x => x.ScreenId);
                });

            migrationBuilder.CreateTable(
                name: "ts_ScreenFunction",
                columns: table => new
                {
                    FunctionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FunctionCode = table.Column<int>(type: "int", nullable: false),
                    FunctionName = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ts_ScreenFunction", x => x.FunctionId);
                });

            migrationBuilder.CreateTable(
                name: "ts_SubModule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubModuleCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SubModuleNameEN = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SubModuleNameTH = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Seq = table.Column<int>(type: "int", nullable: false),
                    IconClass = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ts_SubModule", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ts_SystemConfig",
                columns: table => new
                {
                    ConfigCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ConfigDesc = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ValueVarchar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValueVarchar2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValueVarchar3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValueInt = table.Column<int>(type: "int", nullable: true),
                    ValueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValueDecimal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ts_SystemConfig", x => x.ConfigCode);
                });

            migrationBuilder.CreateTable(
                name: "ts_URLConfig",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    URL = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UpdateUserID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ts_URLConfig", x => x.Code);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ts_ImportLog");

            migrationBuilder.DropTable(
                name: "ts_ImportLogDetail");

            migrationBuilder.DropTable(
                name: "ts_LocalizedMessages");

            migrationBuilder.DropTable(
                name: "ts_LocalizedResources");

            migrationBuilder.DropTable(
                name: "ts_Module");

            migrationBuilder.DropTable(
                name: "ts_Screen");

            migrationBuilder.DropTable(
                name: "ts_ScreenFunction");

            migrationBuilder.DropTable(
                name: "ts_SubModule");

            migrationBuilder.DropTable(
                name: "ts_SystemConfig");

            migrationBuilder.DropTable(
                name: "ts_URLConfig");
        }
    }
}
