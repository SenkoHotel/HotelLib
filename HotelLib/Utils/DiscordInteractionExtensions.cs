﻿using DSharpPlus;
using DSharpPlus.Entities;

namespace HotelLib.Utils;

public static class DiscordInteractionExtensions
{
    #region Reply

    public static void ReplyEmbed(this DiscordInteraction interaction, DiscordEmbedBuilder embed, bool ephemeral = false)
    {
        interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder
        {
            IsEphemeral = ephemeral
        }.AddEmbed(embed.Build()));
    }

    public static void Reply(this DiscordInteraction interaction, string content, bool ephemeral = false)
    {
        interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder
        {
            IsEphemeral = ephemeral,
            Content = content
        });
    }

    public static void ReplyAutoComplete(this DiscordInteraction interaction, List<DiscordAutoCompleteChoice> choices)
    {
        var response = new DiscordInteractionResponseBuilder();
        response.AddAutoCompleteChoices(choices);
        interaction.CreateResponseAsync(InteractionResponseType.AutoCompleteResult, response);
    }

    #endregion

    #region Acknowledge

    public static async Task Acknowledge(this DiscordInteraction interaction)
    {
        await interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
    }

    public static async Task AcknowledgeEphemeral(this DiscordInteraction interaction)
    {
        await interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder
        {
            IsEphemeral = true
        });
    }

    #endregion

    #region Followup

    public static void FollowupEmbed(this DiscordInteraction interaction, DiscordEmbedBuilder embed, bool ephemeral = false)
    {
        interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder
        {
            IsEphemeral = ephemeral
        }.AddEmbed(embed.Build()));
    }

    public static void Followup(this DiscordInteraction interaction, string content, bool ephemeral = false)
    {
        interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder
        {
            IsEphemeral = ephemeral,
            Content = content
        });
    }

    #endregion

    #region Options

    public static string? GetString(this DiscordInteraction interaction, string name)
    {
        return interaction.getOptions()?.Where(option => option.Name == name).Select(option => option.Value).FirstOrDefault() as string;
    }

    public static int? GetInt(this DiscordInteraction interaction, string name)
    {
        var value = interaction.getOptions()?.Where(option => option.Name == name).Select(option => option.Value).FirstOrDefault();
        return value is not long number ? null : (int)number;
    }

    public static bool? GetBool(this DiscordInteraction interaction, string name)
    {
        return interaction.Data?.Options?.Where(option => option.Name == name).Select(option => option.Value).FirstOrDefault() as bool?;
    }

    public static async Task<DiscordUser?> GetUser(this DiscordInteraction interaction, string name)
    {
        var value = interaction.getOptions()?.Where(option => option.Name == name).Select(option => option.Value).FirstOrDefault();
        if (value is not ulong id) return null;

        var client = HotelBot.Instance?.Client;

        if (client == null)
            return null;

        return await client.GetUserAsync(id);
    }

    public static async Task<DiscordMember?> GetMember(this DiscordInteraction interaction, string name)
    {
        var value = interaction.getOptions()?.Where(option => option.Name == name).Select(option => option.Value).FirstOrDefault();

        if (value is not ulong id)
            return null;

        return await interaction.Guild.GetMemberAsync(id);
    }

    public static DiscordChannel? GetChannel(this DiscordInteraction interaction, string name)
    {
        var value = interaction.getOptions()?.Where(option => option.Name == name).Select(option => option.Value).FirstOrDefault();
        return value is not ulong id ? null : interaction.Guild.GetChannel(id);
    }

    public static DiscordRole? GetRole(this DiscordInteraction interaction, string name)
    {
        var value = interaction.getOptions()?.Where(option => option.Name == name).Select(option => option.Value).FirstOrDefault();
        return value is not ulong id ? null : interaction.Guild.GetRole(id);
    }

    public static DiscordAttachment? GetAttachment(this DiscordInteraction interaction, string name)
    {
        var id = (ulong)(interaction.getOptions()?.Where(option => option.Name == name).Select(o => o.Value).FirstOrDefault() ?? 0);
        return id == 0 ? null : interaction.Data.Resolved?.Attachments?.Where(a => a.Key == id).Select(b => b.Value).FirstOrDefault();
    }

    private static IEnumerable<DiscordInteractionDataOption>? getOptions(this DiscordInteraction interaction)
    {
        var options = interaction.Data?.Options?.ToList();
        if (options == null || !options.Any()) return null;

        while (options.FirstOrDefault()?.Options?.Any() ?? false)
        {
            options = options.First().Options.ToList();
        }

        return options;
    }

    #endregion
}
