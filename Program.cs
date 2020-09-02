private static void Main(string[] args)
{
	Console.Title = "Quazide - By iLinked";
	Console.WriteLine("Loading config...");
	Bot.Config = Config.Load();
	Console.WriteLine("Config loaded successfully! logging into account...");
	Bot.Login();
	Console.WriteLine("Logged in successfully!");
	if (Bot.Config.UseCommand)
	{
		Console.WriteLine("Command mode activated, type " + Bot.Config.Prefix + Bot.Config.Command + " to nuke! :)");
		Thread.Sleep(-1);
		return;
	}
	for (;;)
	{
		Console.Clear();
		Console.WriteLine("Please specify an action");
		Console.WriteLine("1 - Nuke all servers");
		Console.WriteLine("2 - Nuke specific server");
		Console.Write("Action: ");
		int num = int.Parse(Console.ReadLine());
		if (num != 1)
		{
			if (num != 2)
			{
				Console.WriteLine("Unknown mode.");
			}
			else
			{
				Console.Write("Server ID: ");
				ulong guildId = ulong.Parse(Console.ReadLine());
				string dm = "";
				if (Bot.BotAccount)
				{
					Console.Write("DM message (keep empty for none): ");
					dm = Console.ReadLine();
				}
				Bot.NukeGuild(guildId, dm);
			}
		}
		else
		{
			string dm = "";
			if (Bot.BotAccount)
			{
				Console.Write("DM message (keep empty for none): ");
				dm = Console.ReadLine();
			}
			Bot.NukeGuild(dm);
		}
	}
}
