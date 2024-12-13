using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public abstract class UserComponent
{
	public string ComponentName { get; set; }

	protected UserComponent(string componentName)
	{
		ComponentName = componentName;
	}

	public virtual void DisplayInfo()
	{
		Console.WriteLine($"Component: {ComponentName}");
	}
}

public class User
{
	public string Username { get; set; }
	public HealthMetrics HealthMetrics { get; set; }
	public EnvironmentalMetrics EnvironmentalMetrics { get; set; }
	public List<Badge> Badges { get; set; }

	public User(string username)
	{
		Username = username;
		HealthMetrics = new HealthMetrics();
		EnvironmentalMetrics = new EnvironmentalMetrics();
		Badges = new List<Badge>();
	}

	public int CalculateEcoScore()
	{
		return (int)EnvironmentalMetrics.PlasticReductionKg * 10 + (int)EnvironmentalMetrics.CarbonReductionKg * 20;
	}

	public int CalculateHealthScore()
	{
		return HealthMetrics.ExerciseMinutes + (HealthMetrics.WaterIntakeLiters * 10) + (HealthMetrics.SleepHours * 5);
	}

	public void DisplaySummary()
	{
		Console.WriteLine($"| {Username,-15} | {HealthMetrics.ExerciseMinutes,-18} | {HealthMetrics.WaterIntakeLiters,-18} | {HealthMetrics.SleepHours,-15} | {EnvironmentalMetrics.PlasticReductionKg,-22:F1} | {EnvironmentalMetrics.CarbonReductionKg,-22:F1} |");
	}

	public void AwardBadge(string badgeName, string description)
	{
		Badges.Add(new Badge(badgeName, description));
	}

	public override string ToString()
	{
		return $"{Username}|{HealthMetrics.ExerciseMinutes}|{HealthMetrics.WaterIntakeLiters}|{HealthMetrics.SleepHours}|{EnvironmentalMetrics.PlasticReductionKg}|{EnvironmentalMetrics.CarbonReductionKg}";
	}

	public static User FromString(string data)
	{
		var parts = data.Split('|');
		if (parts.Length < 6) throw new FormatException("Invalid data format.");

		var user = new User(parts[0])
		{
			HealthMetrics = new HealthMetrics
			{
				ExerciseMinutes = int.Parse(parts[1]),
				WaterIntakeLiters = int.Parse(parts[2]),
				SleepHours = int.Parse(parts[3])
			},
			EnvironmentalMetrics = new EnvironmentalMetrics
			{
				PlasticReductionKg = double.Parse(parts[4]),
				CarbonReductionKg = double.Parse(parts[5])
			}
		};

		return user;
	}
}

public class HealthMetrics : UserComponent
{
	public int ExerciseMinutes { get; set; }
	public int WaterIntakeLiters { get; set; }
	public int SleepHours { get; set; }

	public HealthMetrics() : base("Health Metrics")
	{
	}

	public override void DisplayInfo()
	{
		Console.WriteLine($"Health Metrics: Exercise Minutes: {ExerciseMinutes}, Water Intake: {WaterIntakeLiters}L, Sleep Hours: {SleepHours}");
	}
}

public class EnvironmentalMetrics : UserComponent
{
	public double PlasticReductionKg { get; set; }
	public double CarbonReductionKg { get; set; }

	public EnvironmentalMetrics() : base("Environmental Metrics")
	{
	}

	public override void DisplayInfo()
	{
		Console.WriteLine($"Environmental Metrics: Plastic Reduction: {PlasticReductionKg}kg, Carbon Reduction: {CarbonReductionKg}kg");
	}
}

public class Badge : UserComponent
{
	public string Description { get; set; }

	public Badge(string name, string description) : base(name)
	{
		Description = description;
	}

	public override void DisplayInfo()
	{
		Console.WriteLine($"Badge: {ComponentName}, Description: {Description}");
	}
}

class Program
{
	static List<User> users = new List<User>();
	static User currentUser;
	static string filePath = "users.txt";

	public static void Main(string[] args)
	{
		LoadUsersFromFile();

		Console.Write("Enter your name: ");
		string username = Console.ReadLine();
		currentUser = users.FirstOrDefault(u => u.Username == username) ?? new User(username);
		if (!users.Contains(currentUser)) users.Add(currentUser);

		bool exit = false;
		while (!exit)
		{
			// Clear the screen for a clean output
			Console.Clear();

			// Displaying the menu in Cyan
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine("\n--- Eco-Wellness Tracker Menu ---");
			Console.ResetColor();
			Console.WriteLine("[1] Add Another User");
			Console.WriteLine("[2] Log Health Activity");
			Console.WriteLine("[3] Log Environmental Activity");
			Console.WriteLine("[4] View All Users' Summaries");
			Console.WriteLine("[5] Identify and Award Top Performers");
			Console.WriteLine("[6] Switch Current User");
			Console.WriteLine("[7] Save & Exit");
			Console.WriteLine("[8] Delete User");
			Console.WriteLine("[9] Search User");
			Console.Write("Choose an option: ");
			string choice = Console.ReadLine();

			switch (choice)
			{
				case "1":
					AddUser();
					break;
				case "2":
					LogHealthActivity();
					break;
				case "3":
					LogEnvironmentalActivity();
					break;
				case "4":
					ViewAllUsersSummaries();
					break;
				case "5":
					IdentifyAndAwardTopPerformers();
					break;
				case "6":
					SwitchCurrentUser();
					break;
				case "7":
					SaveUsersToFile();
					exit = true;
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("Thank you for using the Eco-Wellness Tracker!");
					Console.ResetColor();
					break;
				case "8":
					DeleteUser();
					break;
				case "9":
					SearchUser();
					break;
				default:
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Invalid choice, please try again.");
					Console.ResetColor();
					break;
			}

			// Wait for the user to press a key before clearing the screen again
			Console.WriteLine("\nPress any key to return to the main menu...");
			Console.ReadKey();
		}
	}

	static void AddUser()
	{
		Console.Write("Enter new user's username: ");
		string username = Console.ReadLine();
		if (users.Any(u => u.Username == username))
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("User already exists.");
			Console.ResetColor();
			return;
		}
		users.Add(new User(username));
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine($"{username} added successfully.");
		Console.ResetColor();
	}

	static void LogHealthActivity()
	{
		Console.WriteLine("\n[1] Log Exercise");
		Console.WriteLine("[2] Log Water Intake");
		Console.WriteLine("[3] Log Sleep");
		Console.Write("Choose an option: ");
		string option = Console.ReadLine();

		try
		{
			switch (option)
			{
				case "1":
					Console.Write("Enter exercise minutes: ");
					int minutes = int.Parse(Console.ReadLine());
					currentUser.HealthMetrics.ExerciseMinutes += minutes;
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("Exercise logged successfully!");
					Console.ResetColor();
					break;
				case "2":
					Console.Write("Enter water intake in liters: ");
					int liters = int.Parse(Console.ReadLine());
					currentUser.HealthMetrics.WaterIntakeLiters += liters;
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("Water intake logged successfully!");
					Console.ResetColor();
					break;
				case "3":
					Console.Write("Enter sleep hours: ");
					int hours = int.Parse(Console.ReadLine());
					currentUser.HealthMetrics.SleepHours += hours;
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("Sleep logged successfully!");
					Console.ResetColor();
					break;
				default:
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Invalid choice.");
					Console.ResetColor();
					break;
			}
		}
		catch (FormatException)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Invalid input format. Please enter numeric values.");
			Console.ResetColor();
		}
	}

	static void LogEnvironmentalActivity()
	{
		Console.WriteLine("\n[1] Log Plastic Reduction");
		Console.WriteLine("[2] Log Carbon Reduction");
		Console.Write("Choose an option: ");
		string option = Console.ReadLine();

		try
		{
			switch (option)
			{
				case "1":
					Console.Write("Enter plastic reduction in kg: ");
					double plasticKg = double.Parse(Console.ReadLine());
					currentUser.EnvironmentalMetrics.PlasticReductionKg += plasticKg;
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("Plastic reduction logged successfully!");
					Console.ResetColor();
					break;
				case "2":
					Console.Write("Enter carbon reduction in kg: ");
					double carbonKg = double.Parse(Console.ReadLine());
					currentUser.EnvironmentalMetrics.CarbonReductionKg += carbonKg;
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("Carbon reduction logged successfully!");
					Console.ResetColor();
					break;
				default:
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Invalid choice.");
					Console.ResetColor();
					break;
			}
		}
		catch (FormatException)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Invalid input format. Please enter numeric values.");
			Console.ResetColor();
		}
	}

	static void ViewAllUsersSummaries()
	{
		Console.ForegroundColor = ConsoleColor.DarkGreen;
		Console.WriteLine("\n--- All Users' Summaries ---");
		Console.ResetColor();
		Console.WriteLine("| Username        | Exercise (mins)  | Water Intake (L) | Sleep (hrs)   | Plastic Reduction (kg) | Carbon Reduction (kg) |");
		Console.WriteLine("|-----------------|------------------|------------------|---------------|------------------------|------------------------|");

		foreach (var user in users)
		{
			user.DisplaySummary();
		}
		Console.WriteLine("|-----------------|------------------|------------------|---------------|------------------------|------------------------|");
	}
	static void IdentifyAndAwardTopPerformers()
	{
		// Retrieve the top 3 health and eco users
		var topHealthUsers = users.OrderByDescending(u => u.CalculateHealthScore()).Take(3).ToList();
		var topEcoUsers = users.OrderByDescending(u => u.CalculateEcoScore()).Take(3).ToList();

		// Display the top health performers
		if (topHealthUsers.Any())
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("\n--- Top Health Performers ---");
			Console.ResetColor();
			Console.WriteLine("| Rank | Username        | Health Score  |");
			Console.WriteLine("|------|-----------------|---------------|");
			int rank = 1;
			foreach (var user in topHealthUsers)
			{
				user.AwardBadge("Health Champion", "Awarded for excellent health habits!");
				Console.Write("| {0,-4} | ", rank++);

				// Change color of the username only
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.Write($"{user.Username,-15} ");
				Console.ResetColor();

				// Display health score
				Console.WriteLine($"| {user.CalculateHealthScore(),-13} |");
			}
			Console.WriteLine("|------|-----------------|---------------|");
		}
		else
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("No top health performers found.");
			Console.ResetColor();
		}

		// Display the top eco performers
		if (topEcoUsers.Any())
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("\n--- Top Eco Performers ---");
			Console.ResetColor();
			Console.WriteLine("| Rank | Username        | Eco Score     |");
			Console.WriteLine("|------|-----------------|---------------|");
			int rank = 1;
			foreach (var user in topEcoUsers)
			{
				user.AwardBadge("Eco Champion", "Awarded for outstanding environmental impact reduction!");
				Console.Write("| {0,-4} | ", rank++);

				// Change color of the username only
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.Write($"{user.Username,-15} ");
				Console.ResetColor();

				// Display eco score
				Console.WriteLine($"| {user.CalculateEcoScore(),-13} |");
			}
			Console.WriteLine("|------|-----------------|---------------|");
		}
		else
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("No top eco performers found.");
			Console.ResetColor();
		}
	}

	static void SwitchCurrentUser()
	{
		Console.Write("Enter username to switch to: ");
		string username = Console.ReadLine();
		currentUser = users.FirstOrDefault(u => u.Username == username);
		if (currentUser != null)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"Switched to user: {username}");
			Console.ResetColor();
		}
		else
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("User not found.");
			Console.ResetColor();
		}
	}

	static void SaveUsersToFile()
	{
		try
		{
			using (StreamWriter writer = new StreamWriter(filePath))
			{
				foreach (var user in users)
				{
					writer.WriteLine(user.ToString());
				}
			}
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("User data saved successfully.");
			Console.ResetColor();
		}
		catch (Exception ex)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"Error saving data: {ex.Message}");
			Console.ResetColor();
		}
	}

	static void LoadUsersFromFile()
	{
		try
		{
			if (File.Exists(filePath))
			{
				using (StreamReader reader = new StreamReader(filePath))
				{
					string line;
					while ((line = reader.ReadLine()) != null)
					{
						var user = User.FromString(line);
						users.Add(user);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"Error loading data: {ex.Message}");
			Console.ResetColor();
		}
	}

	static void DeleteUser()
	{
		Console.Write("Enter the username of the user to delete: ");
		string username = Console.ReadLine();
		var user = users.FirstOrDefault(u => u.Username == username);
		if (user != null)
		{
			users.Remove(user);
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"{username} has been deleted.");
			Console.ResetColor();
		}
		else
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("User not found.");
			Console.ResetColor();
		}
	}

	static void SearchUser()
	{
		Console.Write("Enter username to search: ");
		string username = Console.ReadLine();
		var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
		if (user != null)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("\n--- User Summary ---");
			Console.ResetColor();
			Console.WriteLine("| Username        | Exercise (mins)  | Water Intake (L) | Sleep (hrs)   | Plastic Reduction (kg) | Carbon Reduction (kg) | Health Score | Eco Score  |");
			Console.WriteLine("|-----------------|------------------|------------------|---------------|------------------------|------------------------|--------------|------------|");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"| {user.Username,-15} | {user.HealthMetrics.ExerciseMinutes,-16} | {user.HealthMetrics.WaterIntakeLiters,-16} | {user.HealthMetrics.SleepHours,-13} | {user.EnvironmentalMetrics.PlasticReductionKg,-22} | {user.EnvironmentalMetrics.CarbonReductionKg,-22} | {user.CalculateHealthScore(),-12} | {user.CalculateEcoScore(),-10} |");
			Console.ResetColor();
			Console.WriteLine("|-----------------|------------------|------------------|---------------|------------------------|------------------------|--------------|------------|");
		}
		else
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("User not found.");
			Console.ResetColor();
		}
	}
}