namespace ConsoleCalculator
{
	class Program
	{
		// Constants for game setup
		private const int BlackCount = 3; // Mafia (Black team)
		private const int RedCount = 7;   // Citizens (Red team)

		static void Main()
		{
			int simulations = GetSimulationsCount();
			Stats results = CalculateBlackWinProbability(simulations);

			Console.WriteLine($"Probability of black team winning: {results.OverallWinsPercent:P2}");

			foreach (var kvp in results.BlackLeftDistribution.OrderByDescending(kvp => kvp.Key))
			{
				Console.WriteLine($"{kvp.Key} players left: {kvp.Value:P2}");
			}
		}

		private static int GetSimulationsCount()
		{
			Console.Write("Enter the number of simulations to run: ");
			do
			{
				string input = Console.ReadLine();
				if (int.TryParse(input, out int simulations) && simulations > 0)
				{
					return simulations;
				}
				Console.Write("Please enter a valid positive number: ");
			} while (true);
		}

		private static Stats CalculateBlackWinProbability(int simulations)
		{
			var random = new Random();
			int blackWins = 0;
			var playersLeftDistribution = new Dictionary<int, int>();

			for (int i = 0; i < simulations; i++)
			{
				int blackCount = SimulateGame(random);
				if (blackCount > 0)
				{
					blackWins++;
					if (playersLeftDistribution.ContainsKey(blackCount))
						playersLeftDistribution[blackCount]++;
					else
						playersLeftDistribution[blackCount] = 1;
				}
			}

			return new Stats
			{
				OverallWinsPercent = (double)blackWins / simulations,
				BlackLeftDistribution = playersLeftDistribution.ToDictionary(kvp => kvp.Key, kvp => (double)kvp.Value / blackWins)
			};
		}

		private static int SimulateGame(Random random)
		{
			int blackCount = BlackCount;
			int redCount = RedCount;

			// Mafia eliminates 1 red player in first round
			redCount--;

			while (blackCount > 0 && redCount > blackCount)
			{
				// Players vote to eliminate 1 player randomly
				bool isBlackEliminated = random.NextDouble() < (double)blackCount / (blackCount + redCount);
				if (isBlackEliminated)
					blackCount--;
				else redCount--;

				if (redCount == blackCount)
					break;

				//Mafia eliminates 1 red player at night
				redCount--;
			}
			return blackCount > 0 ? blackCount : 0;
		}

		private class Stats
		{
			public double OverallWinsPercent { get; set; }
			public Dictionary<int, double> BlackLeftDistribution { get; set; }
		}
	}
}
