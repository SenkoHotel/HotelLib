using DSharpPlus;

namespace HotelLib.Commands;

public class SlashOption
{
    public string Name { get; }
    public string Description { get; }
    public ApplicationCommandOptionType Type { get; }
    public bool Required { get; }

    public SlashOption(string name, string description, ApplicationCommandOptionType type, bool required)
    {
        Name = name;
        Description = description;
        Type = type;
        Required = required;
    }
}
