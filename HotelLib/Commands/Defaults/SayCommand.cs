﻿using DSharpPlus;
using DSharpPlus.Entities;
using HotelLib.Logging;
using HotelLib.Utils;

namespace HotelLib.Commands.Defaults;

public class SayCommand : SlashCommand
{
    public override string Name => "say";
    public override string Description => "Sends a message in a channel.";
    public override Permissions? Permission => Permissions.ManageMessages;

    public override List<SlashOption> Options => new()
    {
        new SlashOption("content", "The message to send.", ApplicationCommandOptionType.String, true),
        new SlashOption("channel", "The channel to send the message in.", ApplicationCommandOptionType.Channel, false),
        new SlashOption("reply", "The message to reply to. (Message ID)", ApplicationCommandOptionType.String, false)
    };

    public override async Task Handle(HotelBot bot, DiscordInteraction interaction)
    {
        try
        {
            var content = interaction.GetString("content");
            var channel = interaction.GetChannel("channel") ?? interaction.Channel;
            var replyString = interaction.GetString("reply");

            if (content == null || string.IsNullOrWhiteSpace(content))
            {
                await interaction.Reply("You must provide a message to send.", true);
                return;
            }

            var msg = new DiscordMessageBuilder().WithContent(content);

            if (replyString != null)
            {
                if (!ulong.TryParse(replyString, out var reply))
                {
                    await interaction.Reply("You must provide a valid message ID to reply to.", true);
                    return;
                }

                var messageToReplyTo = await channel.GetMessageAsync(reply);

                if (messageToReplyTo == null)
                {
                    await interaction.Reply("I couldn't find that message.", true);
                    return;
                }

                msg = msg.WithReply(reply, true);
            }

            var message = await channel.SendMessageAsync(msg);
            await interaction.Reply("Message sent!", true);

            const ulong channelid = 880455708729032864;
            var loggingChannel = interaction.Guild.GetChannel(channelid);

            if (loggingChannel != null)
            {
                var embed = new DiscordEmbedBuilder()
                            .WithAuthor(interaction.User.Username, $"https://discord.com/users/{interaction.User.Id}", iconUrl: interaction.User.AvatarUrl)
                            .WithDescription($"**Message sent in {channel.Mention}**")
                            .AddField("Content", content, true)
                            .WithColor(bot.AccentColor);

                embed.AddField("Message", message.JumpLink.ToString(), true);

                await loggingChannel.SendMessageAsync(embed);
            }
            else
            {
                Logger.Log($"Logging channel {channelid} not found.");
            }
        }
        catch (Exception e)
        {
            var error = new DiscordEmbedBuilder
            {
                Title = "An error occurred while executing this command:",
                Description = e.Message,
                Color = DiscordColor.Red
            };

            if (e.StackTrace != null)
            {
                var stackTrace = e.StackTrace.Split("\n");
                var stackTraceString = stackTrace.Where((_, i) => i != 0).Aggregate("", (current, t) => current + t + "\n");

                // limit the stack trace to 1024 characters
                if (stackTraceString.Length > 1014)
                {
                    stackTraceString = stackTraceString[..1014];
                }

                error.AddField("Stack Trace", $"```cs\n{stackTraceString}```");
            }

            await interaction.ReplyEmbed(error, true);
        }
    }
}
