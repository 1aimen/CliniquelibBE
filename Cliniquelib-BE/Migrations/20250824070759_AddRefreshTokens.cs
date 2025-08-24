using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cliniquelib_BE.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_users_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_Clinic_ClinicId",
                table: "UserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_Role_RoleId",
                table: "UserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_users_UserId",
                table: "UserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_users_Organization_organization_id",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Organization",
                table: "Organization");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Clinic",
                table: "Clinic");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole");

            migrationBuilder.DropIndex(
                name: "IX_UserRole_UserId",
                table: "UserRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Role",
                table: "Role");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserRole");

            migrationBuilder.RenameTable(
                name: "Organization",
                newName: "organization");

            migrationBuilder.RenameTable(
                name: "Clinic",
                newName: "clinic");

            migrationBuilder.RenameTable(
                name: "UserRole",
                newName: "user_roles");

            migrationBuilder.RenameTable(
                name: "Role",
                newName: "roles");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                newName: "refreshtoken");

            migrationBuilder.RenameColumn(
                name: "Timezone",
                table: "organization",
                newName: "timezone");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "organization",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Meta",
                table: "organization",
                newName: "meta");

            migrationBuilder.RenameColumn(
                name: "LegalName",
                table: "organization",
                newName: "legalname");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "organization",
                newName: "createdat");

            migrationBuilder.RenameColumn(
                name: "CountryCode",
                table: "organization",
                newName: "countrycode");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "organization",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Timezone",
                table: "clinic",
                newName: "timezone");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "clinic",
                newName: "phone");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "clinic",
                newName: "organizationid");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "clinic",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Meta",
                table: "clinic",
                newName: "meta");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "clinic",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "clinic",
                newName: "createdat");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "clinic",
                newName: "address");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "clinic",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "user_roles",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "user_roles",
                newName: "role_id");

            migrationBuilder.RenameColumn(
                name: "ClinicId",
                table: "user_roles",
                newName: "clinic_id");

            migrationBuilder.RenameIndex(
                name: "IX_UserRole_RoleId",
                table: "user_roles",
                newName: "IX_user_roles_role_id");

            migrationBuilder.RenameIndex(
                name: "IX_UserRole_ClinicId",
                table: "user_roles",
                newName: "IX_user_roles_clinic_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "roles",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "roles",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "roles",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "refreshtoken",
                newName: "userid");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "refreshtoken",
                newName: "expiresat");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "refreshtoken",
                newName: "createdat");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "refreshtoken",
                newName: "token");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_UserId",
                table: "refreshtoken",
                newName: "IX_refreshtoken_userid");

            migrationBuilder.AlterColumn<Guid>(
                name: "organization_id",
                table: "users",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "clinic_id",
                table: "user_roles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_organization",
                table: "organization",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_clinic",
                table: "clinic",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_roles",
                table: "user_roles",
                columns: new[] { "user_id", "clinic_id", "role_id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_roles",
                table: "roles",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_refreshtoken",
                table: "refreshtoken",
                column: "token");

            migrationBuilder.AddForeignKey(
                name: "FK_refreshtoken_users_userid",
                table: "refreshtoken",
                column: "userid",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_clinic_clinic_id",
                table: "user_roles",
                column: "clinic_id",
                principalTable: "clinic",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_roles_role_id",
                table: "user_roles",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_users_user_id",
                table: "user_roles",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_organization_organization_id",
                table: "users",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_refreshtoken_users_userid",
                table: "refreshtoken");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_clinic_clinic_id",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_roles_role_id",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_users_user_id",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_users_organization_organization_id",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_organization",
                table: "organization");

            migrationBuilder.DropPrimaryKey(
                name: "PK_clinic",
                table: "clinic");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_roles",
                table: "user_roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_roles",
                table: "roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_refreshtoken",
                table: "refreshtoken");

            migrationBuilder.RenameTable(
                name: "organization",
                newName: "Organization");

            migrationBuilder.RenameTable(
                name: "clinic",
                newName: "Clinic");

            migrationBuilder.RenameTable(
                name: "user_roles",
                newName: "UserRole");

            migrationBuilder.RenameTable(
                name: "roles",
                newName: "Role");

            migrationBuilder.RenameTable(
                name: "refreshtoken",
                newName: "RefreshTokens");

            migrationBuilder.RenameColumn(
                name: "timezone",
                table: "Organization",
                newName: "Timezone");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Organization",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "meta",
                table: "Organization",
                newName: "Meta");

            migrationBuilder.RenameColumn(
                name: "legalname",
                table: "Organization",
                newName: "LegalName");

            migrationBuilder.RenameColumn(
                name: "createdat",
                table: "Organization",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "countrycode",
                table: "Organization",
                newName: "CountryCode");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Organization",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "timezone",
                table: "Clinic",
                newName: "Timezone");

            migrationBuilder.RenameColumn(
                name: "phone",
                table: "Clinic",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "organizationid",
                table: "Clinic",
                newName: "OrganizationId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Clinic",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "meta",
                table: "Clinic",
                newName: "Meta");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Clinic",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "createdat",
                table: "Clinic",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "address",
                table: "Clinic",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Clinic",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "role_id",
                table: "UserRole",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "clinic_id",
                table: "UserRole",
                newName: "ClinicId");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "UserRole",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_user_roles_role_id",
                table: "UserRole",
                newName: "IX_UserRole_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_user_roles_clinic_id",
                table: "UserRole",
                newName: "IX_UserRole_ClinicId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Role",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Role",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Role",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "userid",
                table: "RefreshTokens",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "expiresat",
                table: "RefreshTokens",
                newName: "ExpiresAt");

            migrationBuilder.RenameColumn(
                name: "createdat",
                table: "RefreshTokens",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "token",
                table: "RefreshTokens",
                newName: "Token");

            migrationBuilder.RenameIndex(
                name: "IX_refreshtoken_userid",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_UserId");

            migrationBuilder.AlterColumn<Guid>(
                name: "organization_id",
                table: "users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ClinicId",
                table: "UserRole",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "UserRole",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Organization",
                table: "Organization",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Clinic",
                table: "Clinic",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Role",
                table: "Role",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_UserId",
                table: "UserRole",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_users_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_Clinic_ClinicId",
                table: "UserRole",
                column: "ClinicId",
                principalTable: "Clinic",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_Role_RoleId",
                table: "UserRole",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_users_UserId",
                table: "UserRole",
                column: "UserId",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_Organization_organization_id",
                table: "users",
                column: "organization_id",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
