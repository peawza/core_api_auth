using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationDB.Migrations.Application
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_Department",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Department", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tb_GroupPermission",
                columns: table => new
                {
                    GroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ScreenId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FunctionCode = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeletedFlag = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_GroupPermission", x => new { x.GroupId, x.ScreenId });
                });

            migrationBuilder.CreateTable(
                name: "tb_LoginOtp",
                columns: table => new
                {
                    OtpId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    LoginIdentifier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OtpCodeHash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Channel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ReferenceNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExpireAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    MaxAttempt = table.Column<int>(type: "int", nullable: false),
                    RequestIp = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_LoginOtp", x => x.OtpId);
                });

            migrationBuilder.CreateTable(
                name: "tb_Position",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PositionCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PositionName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Position", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tb_Role",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreateBy = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedFlag = table.Column<bool>(type: "bit", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tb_User",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstLoginFlag = table.Column<bool>(type: "bit", nullable: false),
                    ActiveFlag = table.Column<bool>(type: "bit", nullable: false),
                    SystemAdminFlag = table.Column<bool>(type: "bit", nullable: false),
                    OTPLogin = table.Column<bool>(type: "bit", nullable: false),
                    PasswordAge = table.Column<int>(type: "int", nullable: true),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdatePasswordDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InActiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tb_UserInfo",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false, comment: "User ID (Main Key)"),
                    UserNumber = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "UserName"),
                    EmployeeCode = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "Employee Code"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "First Name"),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "Last Name"),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "Remark, more note."),
                    LanguageCode = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "Language Code login"),
                    DepartmentCode = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "Department Code login"),
                    PositionCode = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "Position Code login"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Create Date"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "Create By"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Update Date"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "Update By"),
                    DeletedFlag = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_UserInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tb_RoleClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_RoleClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tb_RoleClaim_tb_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "tb_Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb_PasswordHistory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false, comment: "User ID (Main Key)"),
                    HistoryId = table.Column<int>(type: "int", nullable: false),
                    HistoryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_PasswordHistory", x => new { x.Id, x.HistoryId });
                    table.ForeignKey(
                        name: "FK_tb_PasswordHistory_tb_User_Id",
                        column: x => x.Id,
                        principalTable: "tb_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb_UserClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_UserClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tb_UserClaim_tb_User_UserId",
                        column: x => x.UserId,
                        principalTable: "tb_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb_UserLogin",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_UserLogin", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_tb_UserLogin_tb_User_UserId",
                        column: x => x.UserId,
                        principalTable: "tb_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb_UserRole",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_UserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_tb_UserRole_tb_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "tb_Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_UserRole_tb_Role_RoleId1",
                        column: x => x.RoleId,
                        principalTable: "tb_Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tb_UserRole_tb_User_UserId",
                        column: x => x.UserId,
                        principalTable: "tb_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb_UserToken",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_UserToken", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_tb_UserToken_tb_User_UserId",
                        column: x => x.UserId,
                        principalTable: "tb_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tb_Department_DepartmentCode",
                table: "tb_Department",
                column: "DepartmentCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb_Position_PositionCode",
                table: "tb_Position",
                column: "PositionCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "tb_Role",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_tb_RoleClaim_RoleId",
                table: "tb_RoleClaim",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "tb_User",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "tb_User",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_tb_UserClaim_UserId",
                table: "tb_UserClaim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_UserLogin_UserId",
                table: "tb_UserLogin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_UserRole_RoleId",
                table: "tb_UserRole",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_Department");

            migrationBuilder.DropTable(
                name: "tb_GroupPermission");

            migrationBuilder.DropTable(
                name: "tb_LoginOtp");

            migrationBuilder.DropTable(
                name: "tb_PasswordHistory");

            migrationBuilder.DropTable(
                name: "tb_Position");

            migrationBuilder.DropTable(
                name: "tb_RoleClaim");

            migrationBuilder.DropTable(
                name: "tb_UserClaim");

            migrationBuilder.DropTable(
                name: "tb_UserInfo");

            migrationBuilder.DropTable(
                name: "tb_UserLogin");

            migrationBuilder.DropTable(
                name: "tb_UserRole");

            migrationBuilder.DropTable(
                name: "tb_UserToken");

            migrationBuilder.DropTable(
                name: "tb_Role");

            migrationBuilder.DropTable(
                name: "tb_User");
        }
    }
}
