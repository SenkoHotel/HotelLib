using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using HotelLib.Commands;
using HotelLib.Commands.Defaults;
using HotelLib.Logging;
using HotelLib.Utils;
using Newtonsoft.Json;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace HotelLib;

public class HotelBot
{
    internal static HotelBot? Instance { get; private set; }

    public DiscordColor AccentColor { get; init; } = DiscordColor.White;
    public List<SlashCommand> Commands { get; init; } = new();
    public DiscordClient Client { get; }

    public HotelBot(string token)
    {
        Instance = this;

        Client = new DiscordClient(new DiscordConfiguration
        {
            Token = token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.All,
            AutoReconnect = true,
            MinimumLogLevel = LogLevel.None
        });

        Client.Ready += onReady;
        Client.InteractionCreated += onInteract;
    }

    public static T LoadConfig<T>()
    {
        var json = File.ReadAllText("config.json");
        return JsonConvert.DeserializeObject<T>(json)!;
    }

    public async Task Start()
    {
        if (!Commands.Any(c => c is SayCommand))
            Commands.Add(new SayCommand());

        await Client.ConnectAsync();
        await Task.Delay(-1);
    }

    private Task onReady(DiscordClient sender, ReadyEventArgs args)
    {
        Logger.Log($"Logged in as {Client.CurrentUser.Username}#{Client.CurrentUser.Discriminator}");

        Commands.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));

        var registeredCommands = Commands.Select(command =>
        {
            Logger.Log($"Registering command /{command.Name}");
            return command.Build();
        }).ToList();

        Client.BulkOverwriteGlobalApplicationCommandsAsync(registeredCommands);

        return Task.CompletedTask;
    }

    private Task onInteract(DiscordClient sender, InteractionCreateEventArgs args)
    {
        var interaction = args.Interaction;
        var command = Commands.FirstOrDefault(x => x.Name == interaction.Data.Name);

        if (command == null)
        {
            Logger.Log($"Command {interaction.Data.Name} not found.", Logging.LogLevel.Warning);
            return Task.CompletedTask;
        }

        try
        {
            command.Handle(this, interaction);
        }
        catch (Exception e)
        {
            Logger.Log($"Error while handling command {interaction.Data.Name}: {e.Message}", Logging.LogLevel.Error);
        }

        return Task.CompletedTask;
    }
}
