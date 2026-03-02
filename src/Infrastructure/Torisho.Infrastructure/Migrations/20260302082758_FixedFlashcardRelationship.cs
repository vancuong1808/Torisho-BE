using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Torisho.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixedFlashcardRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chapter_Level_LevelId",
                table: "Chapter");

            migrationBuilder.DropForeignKey(
                name: "FK_DailyActivities_User_UserId",
                table: "DailyActivities");

            migrationBuilder.DropForeignKey(
                name: "FK_FlashCard_DictionaryEntry_DictionaryEntryId",
                table: "FlashCard");

            migrationBuilder.DropForeignKey(
                name: "FK_FlashCard_User_UserId",
                table: "FlashCard");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningContent_DictionaryEntry_DictionaryEntryId",
                table: "LearningContent");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningContent_DictionaryEntry_Vocabulary_DictionaryEntryId",
                table: "LearningContent");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningContent_LearningContent_VideoLessonId",
                table: "LearningContent");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningContent_Level_LevelId",
                table: "LearningContent");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningProgress_Level_LevelId",
                table: "LearningProgress");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningProgress_User_UserId",
                table: "LearningProgress");

            migrationBuilder.DropForeignKey(
                name: "FK_Lesson_Chapter_ChapterId",
                table: "Lesson");

            migrationBuilder.DropForeignKey(
                name: "FK_Lesson_LearningContent_ContentId",
                table: "Lesson");

            migrationBuilder.DropForeignKey(
                name: "FK_Lesson_Quiz_QuizId",
                table: "Lesson");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_User_UserId",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Permission_Role_RoleId",
                table: "Permission");

            migrationBuilder.DropForeignKey(
                name: "FK_Question_Quiz_QuizId",
                table: "Question");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionOption_Question_QuestionId",
                table: "QuestionOption");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAnswer_QuestionOption_SelectedOptionId",
                table: "QuizAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAnswer_Question_QuestionId",
                table: "QuizAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAnswer_QuizAttempt_AttemptId",
                table: "QuizAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAttempt_Quiz_QuizId",
                table: "QuizAttempt");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAttempt_User_UserId",
                table: "QuizAttempt");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_User_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Role_User_UserId",
                table: "Role");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomMessage_Room_RoomId",
                table: "RoomMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomMessage_User_SenderId",
                table: "RoomMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomParticipant_Room_RoomId",
                table: "RoomParticipant");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomParticipant_User_UserId",
                table: "RoomParticipant");

            migrationBuilder.DropForeignKey(
                name: "FK_Subtitle_LearningContent_VideoLessonId",
                table: "Subtitle");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoProgress_LearningContent_VideoLessonId",
                table: "VideoProgress");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoProgress_User_UserId",
                table: "VideoProgress");

            migrationBuilder.DropIndex(
                name: "IX_LearningContent_DictionaryEntryId",
                table: "LearningContent");

            migrationBuilder.DropIndex(
                name: "IX_LearningContent_VideoLessonId",
                table: "LearningContent");

            migrationBuilder.DropIndex(
                name: "IX_LearningContent_Vocabulary_DictionaryEntryId",
                table: "LearningContent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VideoProgress",
                table: "VideoProgress");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subtitle",
                table: "Subtitle");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomParticipant",
                table: "RoomParticipant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Room",
                table: "Room");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Role",
                table: "Role");

            migrationBuilder.DropIndex(
                name: "IX_Role_UserId",
                table: "Role");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuizAttempt",
                table: "QuizAttempt");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuizAnswer",
                table: "QuizAnswer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Quiz",
                table: "Quiz");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionOption",
                table: "QuestionOption");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Question",
                table: "Question");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Permission",
                table: "Permission");

            migrationBuilder.DropIndex(
                name: "IX_Permission_RoleId",
                table: "Permission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Level",
                table: "Level");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lesson",
                table: "Lesson");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LearningProgress",
                table: "LearningProgress");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DictionaryEntry",
                table: "DictionaryEntry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Chapter",
                table: "Chapter");

            migrationBuilder.DropColumn(
                name: "Character",
                table: "LearningContent");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "LearningContent");

            migrationBuilder.DropColumn(
                name: "DictionaryEntryId",
                table: "LearningContent");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "LearningContent");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "LearningContent");

            migrationBuilder.DropColumn(
                name: "KunYomi",
                table: "LearningContent");

            migrationBuilder.DropColumn(
                name: "Meaning",
                table: "LearningContent");

            migrationBuilder.DropColumn(
                name: "OnYomi",
                table: "LearningContent");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "LearningContent");

            migrationBuilder.DropColumn(
                name: "StrokeCount",
                table: "LearningContent");

            migrationBuilder.DropColumn(
                name: "StrokeOrderGifUrl",
                table: "LearningContent");

            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "LearningContent");

            migrationBuilder.DropColumn(
                name: "VideoLessonId",
                table: "LearningContent");

            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "LearningContent");

            migrationBuilder.DropColumn(
                name: "Vocabulary_DictionaryEntryId",
                table: "LearningContent");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Permission");

            migrationBuilder.RenameTable(
                name: "VideoProgress",
                newName: "VideoProgresses");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "Subtitle",
                newName: "Subtitles");

            migrationBuilder.RenameTable(
                name: "RoomParticipant",
                newName: "RoomParticipants");

            migrationBuilder.RenameTable(
                name: "Room",
                newName: "Rooms");

            migrationBuilder.RenameTable(
                name: "Role",
                newName: "Roles");

            migrationBuilder.RenameTable(
                name: "QuizAttempt",
                newName: "QuizAttempts");

            migrationBuilder.RenameTable(
                name: "QuizAnswer",
                newName: "QuizAnswers");

            migrationBuilder.RenameTable(
                name: "Quiz",
                newName: "Quizzes");

            migrationBuilder.RenameTable(
                name: "QuestionOption",
                newName: "QuestionOptions");

            migrationBuilder.RenameTable(
                name: "Question",
                newName: "Questions");

            migrationBuilder.RenameTable(
                name: "Permission",
                newName: "Permissions");

            migrationBuilder.RenameTable(
                name: "Level",
                newName: "Levels");

            migrationBuilder.RenameTable(
                name: "Lesson",
                newName: "Lessons");

            migrationBuilder.RenameTable(
                name: "LearningProgress",
                newName: "LearningProgresses");

            migrationBuilder.RenameTable(
                name: "DictionaryEntry",
                newName: "DictionaryEntries");

            migrationBuilder.RenameTable(
                name: "Chapter",
                newName: "Chapters");

            migrationBuilder.RenameIndex(
                name: "IX_VideoProgress_VideoLessonId",
                table: "VideoProgresses",
                newName: "IX_VideoProgresses_VideoLessonId");

            migrationBuilder.RenameIndex(
                name: "IX_VideoProgress_UserId",
                table: "VideoProgresses",
                newName: "IX_VideoProgresses_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Subtitle_VideoLessonId",
                table: "Subtitles",
                newName: "IX_Subtitles_VideoLessonId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomParticipant_UserId",
                table: "RoomParticipants",
                newName: "IX_RoomParticipants_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomParticipant_RoomId",
                table: "RoomParticipants",
                newName: "IX_RoomParticipants_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizAttempt_UserId",
                table: "QuizAttempts",
                newName: "IX_QuizAttempts_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizAttempt_QuizId",
                table: "QuizAttempts",
                newName: "IX_QuizAttempts_QuizId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizAnswer_SelectedOptionId",
                table: "QuizAnswers",
                newName: "IX_QuizAnswers_SelectedOptionId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizAnswer_QuestionId",
                table: "QuizAnswers",
                newName: "IX_QuizAnswers_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizAnswer_AttemptId",
                table: "QuizAnswers",
                newName: "IX_QuizAnswers_AttemptId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionOption_QuestionId",
                table: "QuestionOptions",
                newName: "IX_QuestionOptions_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_Question_QuizId",
                table: "Questions",
                newName: "IX_Questions_QuizId");

            migrationBuilder.RenameIndex(
                name: "IX_Lesson_QuizId",
                table: "Lessons",
                newName: "IX_Lessons_QuizId");

            migrationBuilder.RenameIndex(
                name: "IX_Lesson_ContentId",
                table: "Lessons",
                newName: "IX_Lessons_ContentId");

            migrationBuilder.RenameIndex(
                name: "IX_Lesson_ChapterId",
                table: "Lessons",
                newName: "IX_Lessons_ChapterId");

            migrationBuilder.RenameIndex(
                name: "IX_LearningProgress_UserId",
                table: "LearningProgresses",
                newName: "IX_LearningProgresses_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_LearningProgress_LevelId",
                table: "LearningProgresses",
                newName: "IX_LearningProgresses_LevelId");

            migrationBuilder.RenameIndex(
                name: "IX_Chapter_LevelId",
                table: "Chapters",
                newName: "IX_Chapters_LevelId");

            migrationBuilder.AddColumn<Guid>(
                name: "RoomId1",
                table: "RoomMessage",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Notification",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "LearningContent",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "VocabularyCount",
                table: "DailyActivities",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TotalPoints",
                table: "DailyActivities",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TotalMinutes",
                table: "DailyActivities",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "RoomCount",
                table: "DailyActivities",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ReadingCount",
                table: "DailyActivities",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "QuizCount",
                table: "DailyActivities",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ListeningCount",
                table: "DailyActivities",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "GrammarCount",
                table: "DailyActivities",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FlashcardCount",
                table: "DailyActivities",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "DailyChallengeCompleted",
                table: "DailyActivities",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<string>(
                name: "ActivityDetailsJson",
                table: "DailyActivities",
                type: "json",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "WatchedDuration",
                table: "VideoProgresses",
                type: "float",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "float");

            migrationBuilder.AlterColumn<float>(
                name: "LastWatchedPosition",
                table: "VideoProgresses",
                type: "float",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "float");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCompleted",
                table: "VideoProgresses",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<float>(
                name: "CompletionPercent",
                table: "VideoProgresses",
                type: "float",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "varchar(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Users",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "AvatarUrl",
                table: "Users",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TextVi",
                table: "Subtitles",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TextJp",
                table: "Subtitles",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Transcript",
                table: "Rooms",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Rooms",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "RoomType",
                table: "Rooms",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Roles",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Roles",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "Score",
                table: "QuizAttempts",
                type: "float",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "float");

            migrationBuilder.AddColumn<Guid>(
                name: "QuizAttemptId",
                table: "QuizAnswers",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Quizzes",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "OptionText",
                table: "QuestionOptions",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Questions",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Permissions",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Permissions",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ThumbnailUrl",
                table: "Levels",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "RequiredProgressPercent",
                table: "Levels",
                type: "float",
                nullable: false,
                defaultValue: 100f,
                oldClrType: typeof(float),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Levels",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Levels",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Levels",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Lessons",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Lessons",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Lessons",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "VocabularyProgress",
                table: "LearningProgresses",
                type: "float",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "float");

            migrationBuilder.AlterColumn<float>(
                name: "TotalProgress",
                table: "LearningProgresses",
                type: "float",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "float");

            migrationBuilder.AlterColumn<float>(
                name: "ReadingProgress",
                table: "LearningProgresses",
                type: "float",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "float");

            migrationBuilder.AlterColumn<float>(
                name: "ListeningProgress",
                table: "LearningProgresses",
                type: "float",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "float");

            migrationBuilder.AlterColumn<float>(
                name: "GrammarProgress",
                table: "LearningProgresses",
                type: "float",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "Reading",
                table: "DictionaryEntries",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "MeaningsJson",
                table: "DictionaryEntries",
                type: "json",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Keyword",
                table: "DictionaryEntries",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Jlpt",
                table: "DictionaryEntries",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ExamplesJson",
                table: "DictionaryEntries",
                type: "json",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Chapters",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ThumbnailUrl",
                table: "Chapters",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "RequiredProgressPercent",
                table: "Chapters",
                type: "float",
                nullable: false,
                defaultValue: 100f,
                oldClrType: typeof(float),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Chapters",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideoProgresses",
                table: "VideoProgresses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subtitles",
                table: "Subtitles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomParticipants",
                table: "RoomParticipants",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuizAttempts",
                table: "QuizAttempts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuizAnswers",
                table: "QuizAnswers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Quizzes",
                table: "Quizzes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionOptions",
                table: "QuestionOptions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Questions",
                table: "Questions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Permissions",
                table: "Permissions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Levels",
                table: "Levels",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lessons",
                table: "Lessons",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LearningProgresses",
                table: "LearningProgresses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DictionaryEntries",
                table: "DictionaryEntries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Chapters",
                table: "Chapters",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ChapterProgresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ChapterId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    LevelId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsUnlocked = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    CompletedLessonCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TotalLessonCount = table.Column<int>(type: "int", nullable: false),
                    CompletionPercent = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    UnlockedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChapterProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChapterProgresses_Chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "Chapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChapterProgresses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Kanji",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Character = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OnYomi = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    KunYomi = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Meaning = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StrokeCount = table.Column<int>(type: "int", nullable: false),
                    StrokeOrderGifUrl = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DictionaryEntryId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kanji", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kanji_DictionaryEntries_DictionaryEntryId",
                        column: x => x.DictionaryEntryId,
                        principalTable: "DictionaryEntries",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PermissionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RoleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VideoLessons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ThumbnailUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VideoUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoLessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoLessons_LearningContent_Id",
                        column: x => x.Id,
                        principalTable: "LearningContent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Vocabulary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DictionaryEntryId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vocabulary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vocabulary_DictionaryEntries_DictionaryEntryId",
                        column: x => x.DictionaryEntryId,
                        principalTable: "DictionaryEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VideoLessonVocabularies",
                columns: table => new
                {
                    VideoLessonId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    VocabulariesId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoLessonVocabularies", x => new { x.VideoLessonId, x.VocabulariesId });
                    table.ForeignKey(
                        name: "FK_VideoLessonVocabularies_VideoLessons_VideoLessonId",
                        column: x => x.VideoLessonId,
                        principalTable: "VideoLessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoLessonVocabularies_Vocabulary_VocabulariesId",
                        column: x => x.VocabulariesId,
                        principalTable: "Vocabulary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_RoomMessage_RoomId1",
                table: "RoomMessage",
                column: "RoomId1");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_UserId1",
                table: "Notification",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_DailyActivities_ActivityDate",
                table: "DailyActivities",
                column: "ActivityDate");

            migrationBuilder.CreateIndex(
                name: "IX_DailyActivities_UserId_ActivityDate",
                table: "DailyActivities",
                columns: new[] { "UserId", "ActivityDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VideoProgresses_UserId_LastWatchedAt",
                table: "VideoProgresses",
                columns: new[] { "UserId", "LastWatchedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_VideoProgresses_UserId_VideoLessonId",
                table: "VideoProgresses",
                columns: new[] { "UserId", "VideoLessonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Status",
                table: "Users",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Status_CreatedAt",
                table: "Users",
                columns: new[] { "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subtitles_VideoLessonId_StartTime",
                table: "Subtitles",
                columns: new[] { "VideoLessonId", "StartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_RoomParticipants_RoomId_LeftAt",
                table: "RoomParticipants",
                columns: new[] { "RoomId", "LeftAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_RoomType",
                table: "Rooms",
                column: "RoomType");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_ScheduledAt",
                table: "Rooms",
                column: "ScheduledAt");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_Status",
                table: "Rooms",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_Status_ScheduledAt",
                table: "Rooms",
                columns: new[] { "Status", "ScheduledAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_QuizId_Score",
                table: "QuizAttempts",
                columns: new[] { "QuizId", "Score" });

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_UserId_QuizId_StartedAt",
                table: "QuizAttempts",
                columns: new[] { "UserId", "QuizId", "StartedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_QuizAnswers_QuizAttemptId",
                table: "QuizAnswers",
                column: "QuizAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_TargetContentId",
                table: "Quizzes",
                column: "TargetContentId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_Type",
                table: "Quizzes",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_QuestionId_IsCorrect",
                table: "QuestionOptions",
                columns: new[] { "QuestionId", "IsCorrect" });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuizId_Order",
                table: "Questions",
                columns: new[] { "QuizId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Code",
                table: "Permissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Levels_Code",
                table: "Levels",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Levels_Order",
                table: "Levels",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_ChapterId_Order",
                table: "Lessons",
                columns: new[] { "ChapterId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_LearningProgresses_UserId_LevelId",
                table: "LearningProgresses",
                columns: new[] { "UserId", "LevelId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryEntries_Jlpt",
                table: "DictionaryEntries",
                column: "Jlpt");

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryEntries_Jlpt_Keyword",
                table: "DictionaryEntries",
                columns: new[] { "Jlpt", "Keyword" });

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryEntries_Keyword",
                table: "DictionaryEntries",
                column: "Keyword");

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryEntries_Reading",
                table: "DictionaryEntries",
                column: "Reading");

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_LevelId_Order",
                table: "Chapters",
                columns: new[] { "LevelId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_ChapterProgresses_ChapterId",
                table: "ChapterProgresses",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_ChapterProgresses_UserId",
                table: "ChapterProgresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChapterProgresses_UserId_ChapterId",
                table: "ChapterProgresses",
                columns: new[] { "UserId", "ChapterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChapterProgresses_UserId_IsUnlocked",
                table: "ChapterProgresses",
                columns: new[] { "UserId", "IsUnlocked" });

            migrationBuilder.CreateIndex(
                name: "IX_ChapterProgresses_UserId_LevelId",
                table: "ChapterProgresses",
                columns: new[] { "UserId", "LevelId" });

            migrationBuilder.CreateIndex(
                name: "IX_Kanji_DictionaryEntryId",
                table: "Kanji",
                column: "DictionaryEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoLessons_Order",
                table: "VideoLessons",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_VideoLessonVocabularies_VocabulariesId",
                table: "VideoLessonVocabularies",
                column: "VocabulariesId");

            migrationBuilder.CreateIndex(
                name: "IX_Vocabulary_DictionaryEntryId",
                table: "Vocabulary",
                column: "DictionaryEntryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_Levels_LevelId",
                table: "Chapters",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DailyActivities_Users_UserId",
                table: "DailyActivities",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FlashCard_DictionaryEntries_DictionaryEntryId",
                table: "FlashCard",
                column: "DictionaryEntryId",
                principalTable: "DictionaryEntries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FlashCard_Users_UserId",
                table: "FlashCard",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LearningContent_Levels_LevelId",
                table: "LearningContent",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LearningProgresses_Levels_LevelId",
                table: "LearningProgresses",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LearningProgresses_Users_UserId",
                table: "LearningProgresses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Chapters_ChapterId",
                table: "Lessons",
                column: "ChapterId",
                principalTable: "Chapters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_LearningContent_ContentId",
                table: "Lessons",
                column: "ContentId",
                principalTable: "LearningContent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Quizzes_QuizId",
                table: "Lessons",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Users_UserId",
                table: "Notification",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Users_UserId1",
                table: "Notification",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionOptions_Questions_QuestionId",
                table: "QuestionOptions",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Quizzes_QuizId",
                table: "Questions",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAnswers_QuestionOptions_SelectedOptionId",
                table: "QuizAnswers",
                column: "SelectedOptionId",
                principalTable: "QuestionOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAnswers_Questions_QuestionId",
                table: "QuizAnswers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAnswers_QuizAttempts_AttemptId",
                table: "QuizAnswers",
                column: "AttemptId",
                principalTable: "QuizAttempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAnswers_QuizAttempts_QuizAttemptId",
                table: "QuizAnswers",
                column: "QuizAttemptId",
                principalTable: "QuizAttempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAttempts_Quizzes_QuizId",
                table: "QuizAttempts",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAttempts_Users_UserId",
                table: "QuizAttempts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomMessage_Rooms_RoomId",
                table: "RoomMessage",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomMessage_Rooms_RoomId1",
                table: "RoomMessage",
                column: "RoomId1",
                principalTable: "Rooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomMessage_Users_SenderId",
                table: "RoomMessage",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomParticipants_Rooms_RoomId",
                table: "RoomParticipants",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomParticipants_Users_UserId",
                table: "RoomParticipants",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subtitles_VideoLessons_VideoLessonId",
                table: "Subtitles",
                column: "VideoLessonId",
                principalTable: "VideoLessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoProgresses_Users_UserId",
                table: "VideoProgresses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoProgresses_VideoLessons_VideoLessonId",
                table: "VideoProgresses",
                column: "VideoLessonId",
                principalTable: "VideoLessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_Levels_LevelId",
                table: "Chapters");

            migrationBuilder.DropForeignKey(
                name: "FK_DailyActivities_Users_UserId",
                table: "DailyActivities");

            migrationBuilder.DropForeignKey(
                name: "FK_FlashCard_DictionaryEntries_DictionaryEntryId",
                table: "FlashCard");

            migrationBuilder.DropForeignKey(
                name: "FK_FlashCard_Users_UserId",
                table: "FlashCard");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningContent_Levels_LevelId",
                table: "LearningContent");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningProgresses_Levels_LevelId",
                table: "LearningProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningProgresses_Users_UserId",
                table: "LearningProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Chapters_ChapterId",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_LearningContent_ContentId",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Quizzes_QuizId",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Users_UserId",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Users_UserId1",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionOptions_Questions_QuestionId",
                table: "QuestionOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Quizzes_QuizId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAnswers_QuestionOptions_SelectedOptionId",
                table: "QuizAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAnswers_Questions_QuestionId",
                table: "QuizAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAnswers_QuizAttempts_AttemptId",
                table: "QuizAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAnswers_QuizAttempts_QuizAttemptId",
                table: "QuizAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAttempts_Quizzes_QuizId",
                table: "QuizAttempts");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAttempts_Users_UserId",
                table: "QuizAttempts");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomMessage_Rooms_RoomId",
                table: "RoomMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomMessage_Rooms_RoomId1",
                table: "RoomMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomMessage_Users_SenderId",
                table: "RoomMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomParticipants_Rooms_RoomId",
                table: "RoomParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomParticipants_Users_UserId",
                table: "RoomParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_Subtitles_VideoLessons_VideoLessonId",
                table: "Subtitles");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoProgresses_Users_UserId",
                table: "VideoProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoProgresses_VideoLessons_VideoLessonId",
                table: "VideoProgresses");

            migrationBuilder.DropTable(
                name: "ChapterProgresses");

            migrationBuilder.DropTable(
                name: "Kanji");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "VideoLessonVocabularies");

            migrationBuilder.DropTable(
                name: "VideoLessons");

            migrationBuilder.DropTable(
                name: "Vocabulary");

            migrationBuilder.DropIndex(
                name: "IX_RoomMessage_RoomId1",
                table: "RoomMessage");

            migrationBuilder.DropIndex(
                name: "IX_Notification_UserId1",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_DailyActivities_ActivityDate",
                table: "DailyActivities");

            migrationBuilder.DropIndex(
                name: "IX_DailyActivities_UserId_ActivityDate",
                table: "DailyActivities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VideoProgresses",
                table: "VideoProgresses");

            migrationBuilder.DropIndex(
                name: "IX_VideoProgresses_UserId_LastWatchedAt",
                table: "VideoProgresses");

            migrationBuilder.DropIndex(
                name: "IX_VideoProgresses_UserId_VideoLessonId",
                table: "VideoProgresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Status",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Status_CreatedAt",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subtitles",
                table: "Subtitles");

            migrationBuilder.DropIndex(
                name: "IX_Subtitles_VideoLessonId_StartTime",
                table: "Subtitles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_RoomType",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_ScheduledAt",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_Status",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_Status_ScheduledAt",
                table: "Rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomParticipants",
                table: "RoomParticipants");

            migrationBuilder.DropIndex(
                name: "IX_RoomParticipants_RoomId_LeftAt",
                table: "RoomParticipants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_Name",
                table: "Roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Quizzes",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_TargetContentId",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_Type",
                table: "Quizzes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuizAttempts",
                table: "QuizAttempts");

            migrationBuilder.DropIndex(
                name: "IX_QuizAttempts_QuizId_Score",
                table: "QuizAttempts");

            migrationBuilder.DropIndex(
                name: "IX_QuizAttempts_UserId_QuizId_StartedAt",
                table: "QuizAttempts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuizAnswers",
                table: "QuizAnswers");

            migrationBuilder.DropIndex(
                name: "IX_QuizAnswers_QuizAttemptId",
                table: "QuizAnswers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Questions",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_QuizId_Order",
                table: "Questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionOptions",
                table: "QuestionOptions");

            migrationBuilder.DropIndex(
                name: "IX_QuestionOptions_QuestionId_IsCorrect",
                table: "QuestionOptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Permissions",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_Code",
                table: "Permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Levels",
                table: "Levels");

            migrationBuilder.DropIndex(
                name: "IX_Levels_Code",
                table: "Levels");

            migrationBuilder.DropIndex(
                name: "IX_Levels_Order",
                table: "Levels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lessons",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_ChapterId_Order",
                table: "Lessons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LearningProgresses",
                table: "LearningProgresses");

            migrationBuilder.DropIndex(
                name: "IX_LearningProgresses_UserId_LevelId",
                table: "LearningProgresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DictionaryEntries",
                table: "DictionaryEntries");

            migrationBuilder.DropIndex(
                name: "IX_DictionaryEntries_Jlpt",
                table: "DictionaryEntries");

            migrationBuilder.DropIndex(
                name: "IX_DictionaryEntries_Jlpt_Keyword",
                table: "DictionaryEntries");

            migrationBuilder.DropIndex(
                name: "IX_DictionaryEntries_Keyword",
                table: "DictionaryEntries");

            migrationBuilder.DropIndex(
                name: "IX_DictionaryEntries_Reading",
                table: "DictionaryEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Chapters",
                table: "Chapters");

            migrationBuilder.DropIndex(
                name: "IX_Chapters_LevelId_Order",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "RoomId1",
                table: "RoomMessage");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "QuizAttemptId",
                table: "QuizAnswers");

            migrationBuilder.RenameTable(
                name: "VideoProgresses",
                newName: "VideoProgress");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.RenameTable(
                name: "Subtitles",
                newName: "Subtitle");

            migrationBuilder.RenameTable(
                name: "Rooms",
                newName: "Room");

            migrationBuilder.RenameTable(
                name: "RoomParticipants",
                newName: "RoomParticipant");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "Role");

            migrationBuilder.RenameTable(
                name: "Quizzes",
                newName: "Quiz");

            migrationBuilder.RenameTable(
                name: "QuizAttempts",
                newName: "QuizAttempt");

            migrationBuilder.RenameTable(
                name: "QuizAnswers",
                newName: "QuizAnswer");

            migrationBuilder.RenameTable(
                name: "Questions",
                newName: "Question");

            migrationBuilder.RenameTable(
                name: "QuestionOptions",
                newName: "QuestionOption");

            migrationBuilder.RenameTable(
                name: "Permissions",
                newName: "Permission");

            migrationBuilder.RenameTable(
                name: "Levels",
                newName: "Level");

            migrationBuilder.RenameTable(
                name: "Lessons",
                newName: "Lesson");

            migrationBuilder.RenameTable(
                name: "LearningProgresses",
                newName: "LearningProgress");

            migrationBuilder.RenameTable(
                name: "DictionaryEntries",
                newName: "DictionaryEntry");

            migrationBuilder.RenameTable(
                name: "Chapters",
                newName: "Chapter");

            migrationBuilder.RenameIndex(
                name: "IX_VideoProgresses_VideoLessonId",
                table: "VideoProgress",
                newName: "IX_VideoProgress_VideoLessonId");

            migrationBuilder.RenameIndex(
                name: "IX_VideoProgresses_UserId",
                table: "VideoProgress",
                newName: "IX_VideoProgress_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Subtitles_VideoLessonId",
                table: "Subtitle",
                newName: "IX_Subtitle_VideoLessonId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomParticipants_UserId",
                table: "RoomParticipant",
                newName: "IX_RoomParticipant_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomParticipants_RoomId",
                table: "RoomParticipant",
                newName: "IX_RoomParticipant_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizAttempts_UserId",
                table: "QuizAttempt",
                newName: "IX_QuizAttempt_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizAttempts_QuizId",
                table: "QuizAttempt",
                newName: "IX_QuizAttempt_QuizId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizAnswers_SelectedOptionId",
                table: "QuizAnswer",
                newName: "IX_QuizAnswer_SelectedOptionId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizAnswers_QuestionId",
                table: "QuizAnswer",
                newName: "IX_QuizAnswer_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizAnswers_AttemptId",
                table: "QuizAnswer",
                newName: "IX_QuizAnswer_AttemptId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_QuizId",
                table: "Question",
                newName: "IX_Question_QuizId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionOptions_QuestionId",
                table: "QuestionOption",
                newName: "IX_QuestionOption_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_QuizId",
                table: "Lesson",
                newName: "IX_Lesson_QuizId");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_ContentId",
                table: "Lesson",
                newName: "IX_Lesson_ContentId");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_ChapterId",
                table: "Lesson",
                newName: "IX_Lesson_ChapterId");

            migrationBuilder.RenameIndex(
                name: "IX_LearningProgresses_UserId",
                table: "LearningProgress",
                newName: "IX_LearningProgress_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_LearningProgresses_LevelId",
                table: "LearningProgress",
                newName: "IX_LearningProgress_LevelId");

            migrationBuilder.RenameIndex(
                name: "IX_Chapters_LevelId",
                table: "Chapter",
                newName: "IX_Chapter_LevelId");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "LearningContent",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Character",
                table: "LearningContent",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "LearningContent",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "DictionaryEntryId",
                table: "LearningContent",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "LearningContent",
                type: "varchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "LearningContent",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KunYomi",
                table: "LearningContent",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Meaning",
                table: "LearningContent",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "OnYomi",
                table: "LearningContent",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "LearningContent",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StrokeCount",
                table: "LearningContent",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StrokeOrderGifUrl",
                table: "LearningContent",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "LearningContent",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "VideoLessonId",
                table: "LearningContent",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "LearningContent",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "Vocabulary_DictionaryEntryId",
                table: "LearningContent",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AlterColumn<int>(
                name: "VocabularyCount",
                table: "DailyActivities",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "TotalPoints",
                table: "DailyActivities",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "TotalMinutes",
                table: "DailyActivities",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "RoomCount",
                table: "DailyActivities",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ReadingCount",
                table: "DailyActivities",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "QuizCount",
                table: "DailyActivities",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ListeningCount",
                table: "DailyActivities",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "GrammarCount",
                table: "DailyActivities",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "FlashcardCount",
                table: "DailyActivities",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<bool>(
                name: "DailyChallengeCompleted",
                table: "DailyActivities",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "ActivityDetailsJson",
                table: "DailyActivities",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "json",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "WatchedDuration",
                table: "VideoProgress",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float",
                oldDefaultValue: 0f);

            migrationBuilder.AlterColumn<float>(
                name: "LastWatchedPosition",
                table: "VideoProgress",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float",
                oldDefaultValue: 0f);

            migrationBuilder.AlterColumn<bool>(
                name: "IsCompleted",
                table: "VideoProgress",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<float>(
                name: "CompletionPercent",
                table: "VideoProgress",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float",
                oldDefaultValue: 0f);

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "User",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "User",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "User",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(512)",
                oldMaxLength: 512)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "User",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "User",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "User",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "AvatarUrl",
                table: "User",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TextVi",
                table: "Subtitle",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TextJp",
                table: "Subtitle",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Transcript",
                table: "Room",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Room",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "RoomType",
                table: "Room",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Role",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Role",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Role",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Quiz",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "Score",
                table: "QuizAttempt",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float",
                oldDefaultValue: 0f);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Question",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "OptionText",
                table: "QuestionOption",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Permission",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Permission",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "Permission",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "ThumbnailUrl",
                table: "Level",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "RequiredProgressPercent",
                table: "Level",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float",
                oldDefaultValue: 100f);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Level",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Level",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Code",
                table: "Level",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldMaxLength: 10)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Lesson",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Lesson",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Lesson",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "VocabularyProgress",
                table: "LearningProgress",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float",
                oldDefaultValue: 0f);

            migrationBuilder.AlterColumn<float>(
                name: "TotalProgress",
                table: "LearningProgress",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float",
                oldDefaultValue: 0f);

            migrationBuilder.AlterColumn<float>(
                name: "ReadingProgress",
                table: "LearningProgress",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float",
                oldDefaultValue: 0f);

            migrationBuilder.AlterColumn<float>(
                name: "ListeningProgress",
                table: "LearningProgress",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float",
                oldDefaultValue: 0f);

            migrationBuilder.AlterColumn<float>(
                name: "GrammarProgress",
                table: "LearningProgress",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float",
                oldDefaultValue: 0f);

            migrationBuilder.AlterColumn<string>(
                name: "Reading",
                table: "DictionaryEntry",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "MeaningsJson",
                table: "DictionaryEntry",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "json",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Keyword",
                table: "DictionaryEntry",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Jlpt",
                table: "DictionaryEntry",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldMaxLength: 10)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ExamplesJson",
                table: "DictionaryEntry",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "json",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Chapter",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ThumbnailUrl",
                table: "Chapter",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<float>(
                name: "RequiredProgressPercent",
                table: "Chapter",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float",
                oldDefaultValue: 100f);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Chapter",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideoProgress",
                table: "VideoProgress",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subtitle",
                table: "Subtitle",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Room",
                table: "Room",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomParticipant",
                table: "RoomParticipant",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Role",
                table: "Role",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Quiz",
                table: "Quiz",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuizAttempt",
                table: "QuizAttempt",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuizAnswer",
                table: "QuizAnswer",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Question",
                table: "Question",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionOption",
                table: "QuestionOption",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Permission",
                table: "Permission",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Level",
                table: "Level",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lesson",
                table: "Lesson",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LearningProgress",
                table: "LearningProgress",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DictionaryEntry",
                table: "DictionaryEntry",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Chapter",
                table: "Chapter",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_LearningContent_DictionaryEntryId",
                table: "LearningContent",
                column: "DictionaryEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningContent_VideoLessonId",
                table: "LearningContent",
                column: "VideoLessonId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningContent_Vocabulary_DictionaryEntryId",
                table: "LearningContent",
                column: "Vocabulary_DictionaryEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_UserId",
                table: "Role",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_RoleId",
                table: "Permission",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chapter_Level_LevelId",
                table: "Chapter",
                column: "LevelId",
                principalTable: "Level",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DailyActivities_User_UserId",
                table: "DailyActivities",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FlashCard_DictionaryEntry_DictionaryEntryId",
                table: "FlashCard",
                column: "DictionaryEntryId",
                principalTable: "DictionaryEntry",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FlashCard_User_UserId",
                table: "FlashCard",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LearningContent_DictionaryEntry_DictionaryEntryId",
                table: "LearningContent",
                column: "DictionaryEntryId",
                principalTable: "DictionaryEntry",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningContent_DictionaryEntry_Vocabulary_DictionaryEntryId",
                table: "LearningContent",
                column: "Vocabulary_DictionaryEntryId",
                principalTable: "DictionaryEntry",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LearningContent_LearningContent_VideoLessonId",
                table: "LearningContent",
                column: "VideoLessonId",
                principalTable: "LearningContent",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningContent_Level_LevelId",
                table: "LearningContent",
                column: "LevelId",
                principalTable: "Level",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LearningProgress_Level_LevelId",
                table: "LearningProgress",
                column: "LevelId",
                principalTable: "Level",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LearningProgress_User_UserId",
                table: "LearningProgress",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lesson_Chapter_ChapterId",
                table: "Lesson",
                column: "ChapterId",
                principalTable: "Chapter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lesson_LearningContent_ContentId",
                table: "Lesson",
                column: "ContentId",
                principalTable: "LearningContent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lesson_Quiz_QuizId",
                table: "Lesson",
                column: "QuizId",
                principalTable: "Quiz",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_User_UserId",
                table: "Notification",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Permission_Role_RoleId",
                table: "Permission",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Question_Quiz_QuizId",
                table: "Question",
                column: "QuizId",
                principalTable: "Quiz",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionOption_Question_QuestionId",
                table: "QuestionOption",
                column: "QuestionId",
                principalTable: "Question",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAnswer_QuestionOption_SelectedOptionId",
                table: "QuizAnswer",
                column: "SelectedOptionId",
                principalTable: "QuestionOption",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAnswer_Question_QuestionId",
                table: "QuizAnswer",
                column: "QuestionId",
                principalTable: "Question",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAnswer_QuizAttempt_AttemptId",
                table: "QuizAnswer",
                column: "AttemptId",
                principalTable: "QuizAttempt",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAttempt_Quiz_QuizId",
                table: "QuizAttempt",
                column: "QuizId",
                principalTable: "Quiz",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAttempt_User_UserId",
                table: "QuizAttempt",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_User_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Role_User_UserId",
                table: "Role",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomMessage_Room_RoomId",
                table: "RoomMessage",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomMessage_User_SenderId",
                table: "RoomMessage",
                column: "SenderId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomParticipant_Room_RoomId",
                table: "RoomParticipant",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomParticipant_User_UserId",
                table: "RoomParticipant",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subtitle_LearningContent_VideoLessonId",
                table: "Subtitle",
                column: "VideoLessonId",
                principalTable: "LearningContent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoProgress_LearningContent_VideoLessonId",
                table: "VideoProgress",
                column: "VideoLessonId",
                principalTable: "LearningContent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoProgress_User_UserId",
                table: "VideoProgress",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
