using DSharpPlus;
using DSharpPlus.Entities;
using HotelLib.Utils;

namespace HotelLib.Commands;

public abstract class SlashCommandGroup : SlashCommand
{
    protected virtual int Depth => 1;
    public abstract List<SlashCommand> SubCommands { get; }

    protected SlashCommandGroup()
    {
        Options.AddRange(SubCommands.Select(x => new SlashOption(x.Name, x.Description, ApplicationCommandOptionType.SubCommand, true)));
    }

    public override void Handle(HotelBot bot, DiscordInteraction interaction)
    {
        var option = interaction.Data.Options.First();
        var subcommand = option.Name;

        for (var i = 0; i < Depth - 1; i++)
        {
            option = option.Options.First();
            subcommand = option.Name;
        }

        var command = SubCommands.FirstOrDefault(x => x.Name == subcommand);

        if (command is null)
        {
            interaction.Reply("Subcommand not found.", true);
            return;
        }

        command.Handle(bot, interaction);
    }
}
