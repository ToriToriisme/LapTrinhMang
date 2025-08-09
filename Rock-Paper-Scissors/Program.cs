using System;

class Program
{
    static void Main()
    {
        string[] choices = { "Rock", "Paper", "Scissors" };
        Random random = new Random();
        bool playAgain = true;

        Console.WriteLine("=== Rock Paper Scissors ===");

        while (playAgain)
        {
            Console.WriteLine("\nEnter your choice (rock, paper, scissors):");
            string playerChoice = Console.ReadLine().Trim().ToLower();

            if (playerChoice != "rock" && playerChoice != "paper" && playerChoice != "scissors")
            {
                Console.WriteLine("Invalid choice! Please choose rock, paper, or scissors.");
                continue;
            }

            string computerChoice = choices[random.Next(choices.Length)];
            Console.WriteLine($"Computer chose: {computerChoice}");

            if (playerChoice == computerChoice.ToLower())
            {
                Console.WriteLine("It's a tie!");
            }
            else if (
                (playerChoice == "rock" && computerChoice == "Scissors") ||
                (playerChoice == "paper" && computerChoice == "Rock") ||
                (playerChoice == "scissors" && computerChoice == "Paper")
            )
            {
                Console.WriteLine("You win!");
            }
            else
            {
                Console.WriteLine("You lose!");
            }

            Console.WriteLine("Play again? (y/n)");
            string answer = Console.ReadLine().Trim().ToLower();
            playAgain = (answer == "y");
        }

        Console.WriteLine("Thanks for playing!");
    }
}
