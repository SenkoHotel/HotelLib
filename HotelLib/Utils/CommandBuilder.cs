using DSharpPlus;
using DSharpPlus.Entities;
using HotelLib.Commands;

namespace HotelLib.Utils;

public static class CommandBuilder
{
    public static DiscordApplicationCommand Build(this SlashCommand command)
    {
        DiscordApplicationCommand appCommand;

        switch (command)
        {
            case SlashCommandGroup group:
                appCommand = new DiscordApplicationCommand(
                    command.Name,
                    command.Description,
                    group.SubCommands.Select(buildSubcommand),
                    defaultMemberPermissions: command.Permission);
                break;

            default:
                appCommand = new DiscordApplicationCommand(
                    command.Name,
                    command.Description,
                    command.Options.Select(buildOption),
                    defaultMemberPermissions: command.Permission);
                break;
        }

        return appCommand;
    }

    private static DiscordApplicationCommandOption buildOption(SlashOption option)
    {
        return new DiscordApplicationCommandOption(
            option.Name,
            option.Description,
            option.Type,
            option.Required
            // option.Choices.Select(x => new DiscordApplicationCommandOptionChoice(x.Name, x.Value))
        );
    }

    private static DiscordApplicationCommandOption buildSubcommand(SlashCommand subCommand, int depth = 0)
    {
        DiscordApplicationCommandOption builder;

        switch (subCommand)
        {
            case SlashCommandGroup group:
                builder = new DiscordApplicationCommandOption(
                    subCommand.Name,
                    subCommand.Description,
                    ApplicationCommandOptionType.SubCommandGroup,
                    null,
                    Array.Empty<DiscordApplicationCommandOptionChoice>(),
                    group.SubCommands.Select(s => buildSubcommand(s, depth + 1)));
                break;

            default:
                builder = new DiscordApplicationCommandOption(
                    subCommand.Name,
                    subCommand.Description,
                    ApplicationCommandOptionType.SubCommand,
                    null,
                    Array.Empty<DiscordApplicationCommandOptionChoice>(),
                    subCommand.Options.Select(buildOption));

                break;
        }

        return builder;
    }
}
