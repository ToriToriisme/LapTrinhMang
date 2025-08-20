using System;

class Program
{
    static readonly string[] choices = { "rock", "paper", "scissors" };
    
    static void Main(string[] args)
    {
        // Dispatch mode by first argument to avoid confusion with multiple StartupObject values.
        // Usage:
        //  - dotnet run -- server  → run network server (Player 1)
        //  - dotnet run -- client  → run network client (Player 2)
        //  - dotnet run            → run local two-players-on-one-keyboard mode
        if (args.Length > 0)
        {
            string mode = args[0].Trim().ToLowerInvariant();
            if (mode == "server")
            {
                Server.Run();
                return;
            }
            if (mode == "client")
            {
                Client.Run();
                return;
            }

            Console.WriteLine("Unknown mode. Use one of: 'server', 'client', or no argument for local mode.");
            return;
        }

        bool playAgain = true;
        int p1Wins = 0, p2Wins = 0, draws = 0;
        Console.WriteLine("=== Rock Paper Scissors - Two Players (Local) ===");

        // Get player names
        Console.WriteLine("\nPlayer 1, enter your name:");
        string player1Name = GetPlayerName(1);
        Console.WriteLine("\nPlayer 2, enter your name:");
        string player2Name = GetPlayerName(2);

        while (playAgain)
        {
            Console.Clear(); // Clear screen for privacy
            string player1Choice = GetValidPlayerChoice(player1Name);
            
            Console.Clear(); // Clear screen so the second player can't see first player's choice
            string player2Choice = GetValidPlayerChoice(player2Name);
            
            Console.Clear();
            RevealChoices(player1Name, player1Choice, player2Name, player2Choice);
            if (player1Choice == player2Choice)
            {
                Console.WriteLine("\nIt's a tie!");
                draws++;
            }
            else
            {
                bool player1Wins = (player1Choice == "rock" && player2Choice == "scissors") ||
                                   (player1Choice == "paper" && player2Choice == "rock") ||
                                   (player1Choice == "scissors" && player2Choice == "paper");
                if (player1Wins)
                {
                    Console.WriteLine($"\n{player1Name} wins!");
                    p1Wins++;
                }
                else
                {
                    Console.WriteLine($"\n{player2Name} wins!");
                    p2Wins++;
                }
            }

            Console.WriteLine($"Score: {player1Name} {p1Wins} - {p2Wins} {player2Name} (Draws: {draws})");

            // Rematch handshake for local mode: require both to agree
            bool p1Agrees = AskYesNo($"{player1Name}, play again? (y/n) ");
            bool p2Agrees = AskYesNo($"{player2Name}, play again? (y/n) ");
            playAgain = p1Agrees && p2Agrees;
        }

        Console.WriteLine("Thanks for playing!");
    }

    static string GetPlayerName(int playerNumber)
    {
        while (true)
        {
            string? name = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(name))
            {
                return name;
            }
            Console.WriteLine($"Please enter a valid name for Player {playerNumber}:");
        }
    }

    static string GetValidPlayerChoice(string playerName)
    {
        while (true)
        {
            Console.WriteLine($"\n{playerName}, enter your choice (rock, paper, scissors):");
            string? playerInput = Console.ReadLine()?.Trim().ToLower();
            
            if (string.IsNullOrWhiteSpace(playerInput))
            {
                Console.WriteLine("Invalid choice! Please choose rock, paper, or scissors.");
                continue;
            }

            if (Array.IndexOf(choices, playerInput) != -1)
            {
                return playerInput;
            }

            Console.WriteLine("Invalid choice! Please choose rock, paper, or scissors.");
        }
    }

    static void RevealChoices(string player1Name, string player1Choice, string player2Name, string player2Choice)
    {
        Console.WriteLine("\n=== Choices Revealed ===");
        Console.WriteLine($"{player1Name} chose: {player1Choice}");
        Console.WriteLine($"{player2Name} chose: {player2Choice}");
    }

    static void DetermineWinner(string player1Name, string player1Choice, string player2Name, string player2Choice)
    {
        if (player1Choice == player2Choice)
        {
            Console.WriteLine("\nIt's a tie!");
            return;
        }

        bool player1Wins = (player1Choice == "rock" && player2Choice == "scissors") ||
                          (player1Choice == "paper" && player2Choice == "rock") ||
                          (player1Choice == "scissors" && player2Choice == "paper");

        if (player1Wins)
        {
            Console.WriteLine($"\n{player1Name} wins!");
        }
        else
        {
            Console.WriteLine($"\n{player2Name} wins!");
        }
    }

    static bool AskToPlayAgain()
    {
        while (true)
        {
            Console.WriteLine("\nPlay again? (y/n)");
            string? answer = Console.ReadLine()?.Trim().ToLower();
            
            if (!string.IsNullOrWhiteSpace(answer))
            {
                if (answer == "y") return true;
                if (answer == "n") return false;
            }
            
            Console.WriteLine("Please enter 'y' for yes or 'n' for no.");
        }
    }

    static bool AskYesNo(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? ans = Console.ReadLine()?.Trim().ToLowerInvariant();
            if (ans == "y" || ans == "yes") return true;
            if (ans == "n" || ans == "no") return false;
            Console.WriteLine("Please enter y/n.");
        }
    }
}

