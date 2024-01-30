using DSharpPlus.Entities;
using HotelLib.Commands;

namespace HotelLib.Utils;

public static class CommandBuilder
{
    public static DiscordApplicationCommand Build(this SlashCommand command)
    {
        var options = command.Options.Select(x => new DiscordApplicationCommandOption(x.Name, x.Description, x.Type, x.Required)).ToList();
        return new DiscordApplicationCommand(command.Name, command.Description, options, defaultMemberPermissions: command.Permissions);
    }
}
