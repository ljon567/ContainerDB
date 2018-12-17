using Microsoft.EntityFrameworkCore.Migrations;

namespace ContainerDB.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocationsItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ContainerID = table.Column<string>(nullable: true),
                    Tauranga = table.Column<string>(nullable: true),
                    Lyttleton = table.Column<string>(nullable: true),
                    Timaru = table.Column<string>(nullable: true),
                    Otago = table.Column<string>(nullable: true),
                    Kiwirail = table.Column<string>(nullable: true),
                    PortConnect = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationsItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContainerItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ContainerID = table.Column<string>(nullable: true),
                    shipco = table.Column<string>(nullable: true),
                    size = table.Column<string>(nullable: true),
                    grade = table.Column<int>(nullable: true),
                    location = table.Column<int>(nullable: true),
                    full = table.Column<bool>(nullable: true),
                    status = table.Column<string>(nullable: true),
                    time = table.Column<string>(nullable: true),
                    type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerItem", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocationsItem");
            migrationBuilder.DropTable(
                name: "ContainerItem");
        }
    }
}
