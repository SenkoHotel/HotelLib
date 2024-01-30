using DSharpPlus;
using DSharpPlus.Entities;

namespace HotelLib.Commands;

public abstract class SlashCommand
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    public virtual Permissions? Permission => null;
    public virtual List<SlashOption> Options { get; } = new();

    public abstract void Handle(HotelBot bot, DiscordInteraction interaction);
}
