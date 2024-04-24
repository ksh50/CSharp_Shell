using System;
using System.Collections.Generic;

interface ICommand
{
    void Execute(string[] args);
}

class EchoCommand : ICommand
{
    public void Execute(string[] args)
    {
        Console.WriteLine(string.Join(" ", args));
    }
}

class AddCommand : ICommand
{
    public void Execute(string[] args)
    {
        if (args.Length >= 2 && int.TryParse(args[0], out int num1) && int.TryParse(args[1], out int num2))
        {
            Console.WriteLine($"Result: {num1 + num2}");
        }
        else
        {
            Console.WriteLine("Error: 'add' command requires two integer arguments.");
        }
    }
}

class CommandRegistry
{
    private Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>();

    public CommandRegistry()
    {
        // ここで各コマンドを登録
        Register("echo", new EchoCommand());
        Register("add", new AddCommand());
    }

    public void Register(string commandName, ICommand command)
    {
        commands[commandName] = command;
    }

    public ICommand? GetCommand(string commandName)
    {
        if (commands.TryGetValue(commandName.ToLower(), out ICommand? command))
        {
            return command;
        }
        return null;
    }
}

class EnhancedShell
{
    static List<string> history = new List<string>();
    static int currentHistoryIndex = -1;
    static CommandRegistry registry = new CommandRegistry();

    static void Main()
    {
        string input = "";

        while (true)
        {
            Console.Write("shell> ");
            input = ReadInput();
            if (input.ToLower() == "exit")
            {
                Console.WriteLine("Exiting shell...");
                Console.ReadKey(true);
                break;
            }

            var parts = ParseInput(input);
            if (parts.Length > 0)
            {
                ICommand? command = registry.GetCommand(parts[0]);
                if (command != null)
                {
                    command.Execute(parts[1..]);
                }
                else
                {
                    Console.WriteLine($"Unknown command: {parts[0]}");
                }
            }
            if (!string.IsNullOrWhiteSpace(input))
            {
                history.Add(input);
                currentHistoryIndex = -1;
            }
        }
    }

    static string ReadInput()
    {
        string input = "";
        ConsoleKeyInfo keyInfo;
        currentHistoryIndex = -1;  // 初期化
        while (true)
        {
            keyInfo = Console.ReadKey(true);
            if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                if (currentHistoryIndex < history.Count - 1)
                {
                    currentHistoryIndex++;
                    input = history[history.Count - 1 - currentHistoryIndex];
                    Console.CursorLeft = 7;
                    Console.Write(new string(' ', Console.WindowWidth - 7));
                    Console.CursorLeft = 7;
                    Console.Write(input);
                }
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                if (currentHistoryIndex > 0)
                {
                    currentHistoryIndex--;
                    input = history[history.Count - 1 - currentHistoryIndex];
                    Console.CursorLeft = 7;
                    Console.Write(new string(' ', Console.WindowWidth - 7));
                    Console.CursorLeft = 7;
                    Console.Write(input);
                }
                else if (currentHistoryIndex == 0)
                {
                    currentHistoryIndex--;
                    input = "";
                    Console.CursorLeft = 7;
                    Console.Write(new string(' ', Console.WindowWidth - 7));
                    Console.CursorLeft = 7;
                }
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return input;
            }
            else if (keyInfo.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input = input.Remove(input.Length - 1);
                Console.Write("\b \b");
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                input += keyInfo.KeyChar;
                Console.Write(keyInfo.KeyChar);
            }
        }
    }

    static string[] ParseInput(string input)
    {
        return input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    }
}
