using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExamAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    TakenAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamAttempts_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorrectAnswer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IncorrectAnswer1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IncorrectAnswer2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IncorrectAnswer3 = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Subjects",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Geography" },
                    { 2, "Kyrgyz History" },
                    { 3, "Kyrgyz Language" }
                });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "CorrectAnswer", "IncorrectAnswer1", "IncorrectAnswer2", "IncorrectAnswer3", "SubjectId", "Text" },
                values: new object[,]
                {
                    { 1, "Bishkek", "Osh", "Naryn", "Talas", 1, "What is the capital of Kyrgyzstan?" },
                    { 2, "Issyk-Kul", "Son-Kul", "Chatyr-Kul", "Sary-Chelek", 1, "Which lake is the largest in Kyrgyzstan?" },
                    { 3, "Tian Shan", "Altai", "Caucasus", "Ural", 1, "Which mountain range covers much of Kyrgyzstan?" },
                    { 4, "Kazakhstan", "Uzbekistan", "Tajikistan", "China", 1, "Which country borders Kyrgyzstan to the north?" },
                    { 5, "China", "Kazakhstan", "Russia", "Turkmenistan", 1, "Which country borders Kyrgyzstan to the east?" },
                    { 6, "Uzbekistan", "China", "Mongolia", "Afghanistan", 1, "Which country borders Kyrgyzstan to the west?" },
                    { 7, "Tajikistan", "Kazakhstan", "Russia", "Azerbaijan", 1, "Which country borders Kyrgyzstan to the south?" },
                    { 8, "Continental", "Tropical", "Equatorial", "Oceanic", 1, "What is the approximate climate of most of Kyrgyzstan?" },
                    { 9, "Osh", "Balykchy", "Karakol", "Tokmok", 1, "Which city is located in southern Kyrgyzstan?" },
                    { 10, "Fergana Valley", "Chui Glacier", "Pamir Ridge", "Karakum Plain", 1, "Which river valley is one of the most important agricultural regions in Kyrgyzstan?" },
                    { 11, "Som", "Tenge", "Ruble", "Manat", 1, "What is the currency used in Kyrgyzstan?" },
                    { 12, "Karakol", "Talas", "Jalal-Abad", "Batken", 1, "Which city is near the eastern end of Issyk-Kul Lake?" },
                    { 13, "Balykchy", "Naryn", "Osh", "Batken", 1, "Which city is located near the western side of Issyk-Kul Lake?" },
                    { 14, "Son-Kul", "Issyk-Kul", "Chatkal", "Aydarkul", 1, "What is the name of the high-altitude lake often used as summer pasture area?" },
                    { 15, "Hydropower", "Petroleum", "Natural gas", "Coal only", 1, "Which natural resource is important in Kyrgyzstan's mountainous rivers?" },
                    { 16, "Jalal-Abad", "Issyk-Kul", "Naryn", "Chui", 1, "Which region is known for walnut forests in Kyrgyzstan?" },
                    { 17, "Mountains", "Deserts", "Low coastal plains", "Rainforests", 1, "What type of relief dominates Kyrgyzstan?" },
                    { 18, "Bishkek", "Dushanbe", "Tashkent", "Astana", 1, "Which capital city lies in the Chui Valley?" },
                    { 19, "Naryn Region", "Batken Region", "Talas Region", "Chui Region", 1, "Which oblast contains the city of Naryn?" },
                    { 20, "Issyk-Kul", "Son-Kul", "Chatyr-Kul", "Kel-Suu", 1, "Which lake in Kyrgyzstan does not freeze due to its salinity and depth?" },
                    { 21, "Southwest", "North", "East", "Northeast", 1, "Which direction from Bishkek is Osh located?" },
                    { 22, "Naryn Region", "Talas Region", "Chui Region", "Batken only", 1, "Which Kyrgyz region borders China?" },
                    { 23, "Summer pasture", "Sea trade", "Rice farming", "Shipbuilding", 1, "What is a common traditional use of jailoos in Kyrgyzstan?" },
                    { 24, "Landlocked", "Island", "Peninsula", "Coastal", 1, "What kind of country is Kyrgyzstan in terms of sea access?" },
                    { 25, "Talas", "Naryn", "Tokmok", "Kyzyl-Kiya", 1, "Which city is the administrative center of Talas Region?" },
                    { 26, "Batken", "Karakol", "Balykchy", "Talas", 1, "Which city is the administrative center of Batken Region?" },
                    { 27, "China", "Kazakhstan", "Russia", "Iran", 1, "Which neighboring country lies southeast of Kyrgyzstan?" },
                    { 28, "Tourism", "Shipbuilding", "Marine fishing", "Tropical forestry", 1, "Which sector is strongly influenced by mountain scenery and Issyk-Kul?" },
                    { 29, "Valleys", "High glaciers", "Mountain peaks", "Uninhabited ridges", 1, "Which part of Kyrgyzstan is generally more densely populated?" },
                    { 30, "Too-Ashuu", "Khyber", "Salang", "Bering", 1, "Which pass is historically important for travel between north and south Kyrgyzstan?" },
                    { 31, "1991", "1989", "1990", "1993", 2, "In which year did Kyrgyzstan gain independence?" },
                    { 32, "Manas", "Semetei", "Kurmanbek", "Toktogul", 2, "What is the name of the great Kyrgyz epic hero?" },
                    { 33, "Russian Empire", "Ottoman Empire", "British Empire", "Roman Empire", 2, "Which empire incorporated the territory of modern Kyrgyzstan in the 19th century?" },
                    { 34, "Tsarist conscription decree", "Discovery of oil", "End of World War II", "Formation of NATO", 2, "Which event of 1916 caused a major uprising in Central Asia?" },
                    { 35, "Urkun", "Tulip Revolution", "Perestroika", "Jadidism", 2, "What is the common name for the tragic events and exodus of 1916 among the Kyrgyz?" },
                    { 36, "USSR", "Yugoslavia", "Czechoslovakia", "Mongolian Empire", 2, "Which Soviet republic did Kyrgyzstan belong to before independence?" },
                    { 37, "Frunze", "Verny", "Alma-Ata", "Pishpek only in Soviet period", 2, "What was the capital of Soviet Kyrgyzstan called before it became Bishkek?" },
                    { 38, "Pishpek", "Samarkand", "Taraz", "Andijan", 2, "Which city name was used historically for modern Bishkek before Frunze?" },
                    { 39, "Kurmanjan Datka", "Tomyris", "Razia Sultan", "Joan of Arc", 2, "Which famous female stateswoman is celebrated in Kyrgyz history?" },
                    { 40, "A continuation of the Manas epic tradition", "A Soviet law", "A mountain pass", "A Russian governor", 2, "What is Semetei in Kyrgyz culture?" },
                    { 41, "Soviet socialist system", "Constitutional monarchy", "Feudal sultanate", "Colonial democracy", 2, "What political system existed in Kyrgyzstan before 1991 as part of the USSR?" },
                    { 42, "2010", "2001", "1995", "2018", 2, "Which year is associated with the April events that changed power in Kyrgyzstan?" },
                    { 43, "Tulip Revolution", "Orange Revolution", "Velvet Revolution", "February Revolution", 2, "Which revolution in Kyrgyzstan took place in 2005?" },
                    { 44, "Nomadic pastoralism", "Ocean trade", "Tea plantation farming", "Industrial shipbuilding", 2, "What was the main economic way of life of many Kyrgyz historically?" },
                    { 45, "Horse", "Camel only", "Elephant", "Llama", 2, "Which animal was central to traditional Kyrgyz nomadic life?" },
                    { 46, "Yurt", "Pagoda", "Igloo", "Castle", 2, "Which structure is a symbol of traditional Kyrgyz nomadic culture?" },
                    { 47, "Mikhail Frunze", "Lenin only", "Brezhnev", "Molotov", 2, "Which Soviet leader's surname gave the name Frunze to the capital?" },
                    { 48, "It became a union republic", "It became independent", "It joined NATO", "It became part of China", 2, "What happened to Kyrgyzstan in 1936 within the USSR?" },
                    { 49, "Kurmanjan Datka", "Toktogul", "Chingiz Aitmatov", "Manaschi Sayakbay", 2, "Which historical figure is linked with preserving and leading southern Kyrgyz lands under pressure in the 19th century?" },
                    { 50, "Epic of Manas", "Hamlet", "Iliad", "Don Quixote", 2, "Which literary work is considered one of the treasures of Kyrgyz oral heritage?" },
                    { 51, "19th century", "15th century", "10th century", "21st century", 2, "Which century saw the Russian Empire expand into the territory of modern Kyrgyzstan?" },
                    { 52, "Industrialization and collectivization", "Feudal fragmentation", "Maritime colonization", "End of literacy", 2, "What was one consequence of Soviet rule in Kyrgyzstan?" },
                    { 53, "Chingiz Aitmatov", "Abai", "Tolstoy", "Navoi", 2, "Which writer from Kyrgyzstan became world famous in the 20th century?" },
                    { 54, "A reciter of the Manas epic", "A tribal tax collector", "A Soviet general", "A mountain guide only", 2, "What is a manaschi?" },
                    { 55, "Formation of a sovereign state", "Restoration of tsarist rule", "Incorporation into the Roman Empire", "Becoming a monarchy", 2, "Which period followed Kyrgyzstan's independence in 1991?" },
                    { 56, "Fergana Valley", "Crimea", "Siberia", "Baltic coast", 2, "Which neighboring region historically had strong cultural and economic links with the Kyrgyz of the south?" },
                    { 57, "Oral tradition", "Marble inscriptions only", "Ancient newspapers", "Cinema archives only", 2, "What is one major source of traditional historical memory among the Kyrgyz?" },
                    { 58, "Independence Day", "Victory Day only", "Constitution of USSR Day", "Empire Day", 2, "Which state holiday marks independence in Kyrgyzstan?" },
                    { 59, "Nomadic life", "Island life", "Mediterranean trade life", "Jungle farming life", 2, "Which historical lifestyle shaped Kyrgyz customs, clothing, and mobility?" },
                    { 60, "Clan and tribe", "Senate and consul", "Pharaoh court", "Shogunate house", 2, "What was an important traditional social unit among the Kyrgyz?" },
                    { 61, "14", "10", "8", "20", 3, "How many vowels are there in the Kyrgyz alphabet?" },
                    { 62, "Ө", "Ж", "Ч", "Ң only consonant", 3, "Which of these is a Kyrgyz vowel?" },
                    { 63, "Б", "Ү", "Ө", "Ы", 3, "Which of these is a Kyrgyz consonant?" },
                    { 64, "Ң", "Q", "W", "X", 3, "Which letter is specific to the Kyrgyz alphabet?" },
                    { 65, "китеп", "жакшы", "барды", "тез", 3, "Which word is a noun?" },
                    { 66, "кооз", "мектеп", "окуду", "келди", 3, "Which word is an adjective?" },
                    { 67, "жазды", "дептер", "узун", "көк", 3, "Which word is a verb?" },
                    { 68, "Ким?", "Эмне?", "Кайда?", "Качан?", 3, "Which question asks 'Who?' in Kyrgyz?" },
                    { 69, "Эмне?", "Ким?", "Кайсы?", "Качан?", 3, "Which question asks 'What?' in Kyrgyz?" },
                    { 70, "Кайда?", "Эмне үчүн?", "Канча?", "Ким?", 3, "Which question asks 'Where?' in Kyrgyz?" },
                    { 71, "Качан?", "Кайда?", "Кантип?", "Эмне?", 3, "Which question asks 'When?' in Kyrgyz?" },
                    { 72, "китеп", "калем", "терезе", "эшик", 3, "Which word means 'book' in Kyrgyz?" },
                    { 73, "мектеп", "шаар", "дос", "кыш", 3, "Which word means 'school' in Kyrgyz?" },
                    { 74, "дос", "үй", "тоо", "жол", 3, "Which word means 'friend' in Kyrgyz?" },
                    { 75, "үй", "суу", "бак", "күн", 3, "Which word means 'house' in Kyrgyz?" },
                    { 76, "суу", "от", "жер", "аба", 3, "Which word means 'water' in Kyrgyz?" },
                    { 77, "күн", "айыл", "түн", "жылдыз", 3, "Which word means 'sun' in Kyrgyz?" },
                    { 78, "балдар", "бала", "китеп", "шаар", 3, "Which of these is plural?" },
                    { 79, "-лар/-лер", "-мак", "-ып", "-чы", 3, "What suffix often forms plural nouns in Kyrgyz?" },
                    { 80, "окуучу", "окуучулар", "балдар", "үйлөр", 3, "Which of these words is singular?" },
                    { 81, "сулуу", "жүгүрдү", "китеп", "бүгүн", 3, "Which word means 'beautiful' in Kyrgyz?" },
                    { 82, "баруу", "келди", "дос", "узун", 3, "Which word means 'to go' in Kyrgyz infinitive form?" },
                    { 83, "окуу", "окуду", "мугалим", "тез", 3, "Which word means 'to read' in Kyrgyz infinitive form?" },
                    { 84, "мен", "жакшы", "барды", "мектеп", 3, "Which word is a pronoun?" },
                    { 85, "мен", "сен", "биз", "алар", 3, "Which word means 'I' in Kyrgyz?" },
                    { 86, "биз", "мен", "сен", "ал", 3, "Which word means 'we' in Kyrgyz?" },
                    { 87, "алар", "ал", "мен", "силер", 3, "Which word means 'they' in Kyrgyz?" },
                    { 88, "Кайсы?", "жакшы", "барды", "кыска", 3, "Which of these is a question word?" },
                    { 89, "мугалим", "окуучу", "дос", "балдар", 3, "Which word means 'teacher' in Kyrgyz?" },
                    { 90, "окуучу", "мугалим", "ата", "эне", 3, "Which word means 'student/pupil' in Kyrgyz?" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamAttempts_SubjectId",
                table: "ExamAttempts",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_SubjectId",
                table: "Questions",
                column: "SubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExamAttempts");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Subjects");
        }
    }
}
