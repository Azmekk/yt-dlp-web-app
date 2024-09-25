using System.Data.SQLite;
using Spectre.Console;

namespace DatabaseMigrator;

public static class DatabaseMigrationHelper
{
    public static void MigrateDatabases(string sourcePath, string targetPath)
    {
        SQLiteConnection sourceConnection;
        SQLiteConnection destinationConnection;
        try
        {
            sourceConnection = new SQLiteConnection(GetConnectionString(sourcePath));
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine("[red]Error: Failed to establish connection to source database. Please point to a correct file.[/]");
            throw new Exception("Failed to establish connection to source database", e); 
        }

        try
        {
            destinationConnection = new SQLiteConnection(GetConnectionString(targetPath));
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine("[red]Error: Failed to establish connection to destination database. Please point to a correct file.[/]");
            throw new Exception("Failed to establish connection to source database", e); 
        }
        
        sourceConnection.Open();
        destinationConnection.Open();
        
        string dateFormat = "yyyy-MM-dd HH:mm:ss.fffffffK";
        
        using var transaction = destinationConnection.BeginTransaction();
        var insertCommand = new SQLiteCommand(
            "INSERT INTO Videos (CreatedAt, UpdatedAt, DeletedAt, FileName, ThumbnailName, Size, Url, Downloaded) " +
            "VALUES (@CreatedAt, @UpdatedAt, @DeletedAt, @FileName, @ThumbnailName, @Size, @Url, @Downloaded)",
            destinationConnection);

        try
        {
                insertCommand.Parameters.Add(new SQLiteParameter("@CreatedAt"));
                insertCommand.Parameters.Add(new SQLiteParameter("@UpdatedAt"));
                insertCommand.Parameters.Add(new SQLiteParameter("@DeletedAt"));
                insertCommand.Parameters.Add(new SQLiteParameter("@FileName"));
                insertCommand.Parameters.Add(new SQLiteParameter("@ThumbnailName"));
                insertCommand.Parameters.Add(new SQLiteParameter("@Size"));
                insertCommand.Parameters.Add(new SQLiteParameter("@Url"));
                insertCommand.Parameters.Add(new SQLiteParameter("@Downloaded"));
                
                var selectCommand = new SQLiteCommand("SELECT created_at, updated_at, deleted_at, file_name, thumnail_name, size, url, downloaded FROM videos", sourceConnection);
                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        insertCommand.Parameters["@CreatedAt"].Value = reader["created_at"];;
                        insertCommand.Parameters["@UpdatedAt"].Value = reader["updated_at"];
                        insertCommand.Parameters["@DeletedAt"].Value = reader["deleted_at"];
                        insertCommand.Parameters["@FileName"].Value = reader["file_name"];
                        insertCommand.Parameters["@ThumbnailName"].Value = reader["thumnail_name"];
                        insertCommand.Parameters["@Size"].Value = reader["size"];
                        insertCommand.Parameters["@Url"].Value = reader["url"];
                        insertCommand.Parameters["@Downloaded"].Value = reader["downloaded"];
                        
                        insertCommand.ExecuteNonQuery();
                    }
                }
                
                transaction.Commit();
                Console.WriteLine("Data migration completed successfully.");
        }
        catch (Exception e)
        {
            transaction.Rollback();
            AnsiConsole.MarkupLine($"[red]Error: Something went wrong during migration: {e.Message}[/]");
            throw new Exception("Something went wrong during migration", e);
        }
        finally
        {
            sourceConnection.Dispose();
            destinationConnection.Dispose();
        }
    }
    
    private static string GetConnectionString(string dbPath)
    {
        return $"Data Source={dbPath};Version=3;new=False;datetimeformat=CurrentCulture";
    }
}