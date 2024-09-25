using Spectre.Console;

namespace DatabaseMigrator;

class Program
{
    static void Main(string[] args)
    {
        bool confirmBackedUp = AnsiConsole.Confirm("[red]IMPORTANT NOTICE: You should backup your old database in case of corruption or misuse of this application. Confirm that you have backed up your database.[/]");

        if (!confirmBackedUp)
        {
            AnsiConsole.MarkupLine("[red]Confirmation failed exiting application.[/]");
        }

        string sourcePath = AnsiConsole.Prompt(new TextPrompt<string>(
            "[yellow]Please input the path to your [b]OLD[/] database [/]"
        ));

        if (File.Exists(sourcePath))
        {
            AnsiConsole.MarkupLine("[green]Source db file found.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]Source db file not found. Check path and verify it's correct. Path provided: {Path.GetFullPath(sourcePath)}[/]");
            return;
        }
        
        string destinationPath = AnsiConsole.Prompt(new TextPrompt<string>(
            "[yellow]Please input the path to your [b]NEW[/] database file.[/]"
        ));

        if (File.Exists(destinationPath))
        {
            AnsiConsole.MarkupLine("[green]Destination db file found.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]Destination db file not found. Check path and verify it's correct. Path provided: {Path.GetFullPath(destinationPath)}[/]");
            return;
        }
        
        AnsiConsole.MarkupLine("[yellow]Attempting to create backups of both databases.[/]");

        string sourceBackupPath = sourcePath + "." + DateTime.Now.ToString("h.mm.ss-dd.MM.yyyy") + ".bak";
        string destinationBackupPath = destinationPath + "." + DateTime.Now.ToString("h.mm.ss-dd.MM.yyyy") + ".bak";
        File.Copy(sourcePath, sourceBackupPath);
        File.Copy(destinationPath, destinationBackupPath);
        
        if (File.Exists(sourceBackupPath))
        {
            AnsiConsole.MarkupLine($"[green]Created backup of source file at: {sourceBackupPath}.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Failed to create a backup of source file.[/]");
            return;
        }
        
        if (File.Exists(destinationBackupPath))
        {
            AnsiConsole.MarkupLine($"[green]Created backup of destination file at: {destinationBackupPath}.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Failed to create a backup of destination file.[/]");
            return;
        }
        
        AnsiConsole.MarkupLine($"[green]Attempting to migrate database.[/]");

        AnsiConsole.Status().Spinner(Spinner.Known.Star).Start("[red]Migrating. [b]DO NOT STOP APPLICATION[/][/]", x =>
        {
            DatabaseMigrationHelper.MigrateDatabases(sourcePath, destinationPath);
        });
    }
}