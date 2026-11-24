using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Readex.Infraestrutura.Migrations
{
    public partial class PersistirCatalogo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Ano",
                table: "Obras",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Formato",
                table: "Obras",
                type: "TEXT",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sinopse",
                table: "Obras",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Obras",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Obras_Slug",
                table: "Obras",
                column: "Slug",
                unique: true);

            migrationBuilder.Sql("""
                INSERT INTO "Obras"
                    ("Id", "Titulo", "Autor", "Genero", "Tipo", "CapaUrl", "Destaque", "Slug", "Ano", "Formato", "Sinopse")
                VALUES
                    ('CD385EA4-B45C-58AB-B2B2-C54E750E6C0D', 'Piranesi', 'Susanna Clarke', 'Fantasy, Mystery, Literary', 0, '/covers/piranesi.jpg', 0, 'piranesi', 2020, '272 pages', 'Piranesi lives in the House — a labyrinth of endless halls, thousands of statues, and an ocean imprisoned within. He keeps meticulous journals of its tides, until messages begin to appear that suggest the House is not all there is.'),
                    ('0C54325E-BD01-5E4D-98EA-FACAC7AD3EBB', 'Vagabond', 'Takehiko Inoue', 'Historical, Seinen, Martial Arts', 2, '/covers/vagabond.jpg', 0, 'vagabond', 1998, '37 volumes', 'A sweeping, ink-washed retelling of the life of Miyamoto Musashi, the wandering swordsman who sought invincibility under the heavens — and found something quieter instead.'),
                    ('595F62F1-6A43-5BCA-A691-8A6CF2C84AB7', 'Saga', 'Brian K. Vaughan & Fiona Staples', 'Space Opera, Romance, Fantasy', 1, '/covers/saga.jpg', 0, 'saga', 2012, '11 volumes', 'Two soldiers from opposite sides of a never-ending galactic war fall in love and risk everything to raise their newborn daughter in a universe that wants them dead.'),
                    ('D257504B-03BC-5473-9E5D-44DA9993D9A7', 'The Left Hand of Darkness', 'Ursula K. Le Guin', 'Science Fiction, Classic', 0, '/covers/the-left-hand-of-darkness.jpg', 0, 'the-left-hand-of-darkness', 1969, '304 pages', 'An envoy is sent to the ice planet Gethen to persuade its ambisexual people to join an interstellar alliance, and finds himself crossing a frozen continent with the one person he cannot understand.'),
                    ('31E5F885-9698-5EA1-93A4-9D7E1B31D0A5', 'Monster', 'Naoki Urasawa', 'Thriller, Psychological, Seinen', 2, '/covers/monster.jpg', 0, 'monster', 1994, '18 volumes', 'Brilliant surgeon Kenzo Tenma saves the life of a young boy over the town mayor. Years later, he discovers the boy has grown into a charming, terrifying killer — and Tenma must end what he started.'),
                    ('1CE446D2-9E69-5524-8C9F-5BB94E52F91E', 'Watchmen', 'Alan Moore & Dave Gibbons', 'Superhero, Mystery, Classic', 1, '/covers/watchmen.jpg', 0, 'watchmen', 1986, '416 pages', 'In an alternate 1985 where costumed heroes are outlawed, the murder of a former vigilante uncovers a conspiracy that could push the world to nuclear midnight.'),
                    ('6352B027-5431-5210-8073-F4CD412BA5F4', 'Tomorrow, and Tomorrow, and Tomorrow', 'Gabrielle Zevin', 'Literary, Contemporary', 0, '/covers/tomorrow-and-tomorrow.jpg', 0, 'tomorrow-and-tomorrow', 2022, '416 pages', 'Two friends meet in a hospital game room as children and, years later, build video games together — a thirty-year story of creative partnership, love that is not romance, and the worlds we make to survive.'),
                    ('FB400D40-109E-5F03-BDF7-C21BCC18E414', 'Frieren: Beyond Journey''s End', 'Kanehito Yamada & Tsukasa Abe', 'Fantasy, Slice of Life, Shōnen', 2, '/covers/frieren.jpg', 0, 'frieren', 2020, '13 volumes', 'After the hero party defeats the Demon King, the elf mage Frieren — who will outlive them all — sets out to understand the humans she spent a mere decade beside.'),
                    ('D322DAF5-D9F1-5042-A440-D17C49B9679C', 'The Sandman', 'Neil Gaiman', 'Dark Fantasy, Horror, Mythology', 1, '/covers/sandman.jpg', 0, 'sandman', 1989, '10 volumes', 'Dream of the Endless escapes a century of imprisonment and sets out to rebuild his kingdom, reckoning with gods, mortals, and the changing nature of stories themselves.'),
                    ('8D7C25CD-5797-5BD0-82EE-C2E47F92ABF2', 'The Remains of the Day', 'Kazuo Ishiguro', 'Literary, Historical', 0, '/covers/the-remains-of-the-day.jpg', 0, 'the-remains-of-the-day', 1989, '258 pages', 'Stevens, an aging English butler, takes a motoring trip through the West Country and reflects on decades of devoted service — and on the life he quietly chose not to live.'),
                    ('33333333-3333-3333-3333-333333333333', 'Berserk', 'Kentaro Miura', 'Dark Fantasy, Seinen, Action', 2, '/covers/berserk.jpg', 1, 'berserk', 1989, '42 volumes', 'Guts, a lone mercenary branded for death, wages a relentless war against demons and fate itself in one of the most ambitious dark fantasy epics ever drawn.'),
                    ('561FE77D-51F6-524B-8AFD-2ECCF3A8A60A', 'Maus', 'Art Spiegelman', 'Memoir, History, Classic', 1, '/covers/maus.jpg', 0, 'maus', 1991, '296 pages', 'Spiegelman interviews his father about surviving the Holocaust, rendering Jews as mice and Nazis as cats — a landmark that redefined what comics could hold.')
                ON CONFLICT("Id") DO NOTHING;
                
                UPDATE "Obras"
                SET "Slug" = 'berserk',
                    "Ano" = 1989,
                    "Formato" = '42 volumes',
                    "Sinopse" = 'Guts, a lone mercenary branded for death, wages a relentless war against demons and fate itself in one of the most ambitious dark fantasy epics ever drawn.',
                    "CapaUrl" = COALESCE(NULLIF(TRIM("CapaUrl"), ''), '/covers/berserk.jpg'),
                    "Genero" = CASE WHEN "Genero" = 'Seinen' THEN 'Dark Fantasy, Seinen, Action' ELSE "Genero" END
                WHERE "Id" = '33333333-3333-3333-3333-333333333333';
                
                DELETE FROM "Obras"
                WHERE ("Id" = '11111111-1111-1111-1111-111111111111' AND "Titulo" = 'Neuromancer')
                   OR ("Id" = '22222222-2222-2222-2222-222222222222' AND "Titulo" = 'Batman Ano Um');
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM "Obras"
                WHERE ("Id", "Slug") IN (
                    ('CD385EA4-B45C-58AB-B2B2-C54E750E6C0D', 'piranesi'),
                    ('0C54325E-BD01-5E4D-98EA-FACAC7AD3EBB', 'vagabond'),
                    ('595F62F1-6A43-5BCA-A691-8A6CF2C84AB7', 'saga'),
                    ('D257504B-03BC-5473-9E5D-44DA9993D9A7', 'the-left-hand-of-darkness'),
                    ('31E5F885-9698-5EA1-93A4-9D7E1B31D0A5', 'monster'),
                    ('1CE446D2-9E69-5524-8C9F-5BB94E52F91E', 'watchmen'),
                    ('6352B027-5431-5210-8073-F4CD412BA5F4', 'tomorrow-and-tomorrow'),
                    ('FB400D40-109E-5F03-BDF7-C21BCC18E414', 'frieren'),
                    ('D322DAF5-D9F1-5042-A440-D17C49B9679C', 'sandman'),
                    ('8D7C25CD-5797-5BD0-82EE-C2E47F92ABF2', 'the-remains-of-the-day'),
                    ('561FE77D-51F6-524B-8AFD-2ECCF3A8A60A', 'maus')
                );
                
                INSERT INTO "Obras" ("Id", "Titulo", "Autor", "Genero", "Tipo", "CapaUrl", "Destaque")
                VALUES
                    ('11111111-1111-1111-1111-111111111111', 'Neuromancer', 'William Gibson', 'Ficção Científica', 0, NULL, 1),
                    ('22222222-2222-2222-2222-222222222222', 'Batman Ano Um', 'Frank Miller', 'Crime', 1, NULL, 0)
                ON CONFLICT("Id") DO NOTHING;
                
                UPDATE "Obras"
                SET "CapaUrl" = CASE WHEN "CapaUrl" = '/covers/berserk.jpg' THEN NULL ELSE "CapaUrl" END,
                    "Genero" = CASE WHEN "Genero" = 'Dark Fantasy, Seinen, Action' THEN 'Seinen' ELSE "Genero" END
                WHERE "Id" = '33333333-3333-3333-3333-333333333333' AND "Slug" = 'berserk';
                """);

            migrationBuilder.DropIndex(name: "IX_Obras_Slug", table: "Obras");
            migrationBuilder.DropColumn(name: "Ano", table: "Obras");
            migrationBuilder.DropColumn(name: "Formato", table: "Obras");
            migrationBuilder.DropColumn(name: "Sinopse", table: "Obras");
            migrationBuilder.DropColumn(name: "Slug", table: "Obras");
        }
    }
}
