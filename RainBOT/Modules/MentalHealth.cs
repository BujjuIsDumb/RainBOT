// This file is from RainBOT.
// 
// Copyright(c) 2022 Bujju
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using RainBOT.Core;
using RainBOT.Core.Attributes;
using RainBOT.Core.Services;

namespace RainBOT.Modules
{
    /// <summary>
    ///     The mental health module.
    /// </summary>
    public class MentalHealth : ApplicationCommandModule
    {
        /// <summary>
        ///     Sets the database service.
        /// </summary>
        public Database Data { private get; set; }

        /// <summary>
        ///     Gets the embeds for suicide prevention hotlines.
        /// </summary>
        public DiscordEmbed[] Hotlines { get; } = new DiscordEmbed[]
        {
            new DiscordEmbedBuilder()
                .WithAuthor("LGBTQIA Resource Center", "https://lgbtqia.ucdavis.edu/support/hotlines", "https://lgbtqia.ucdavis.edu/sites/g/files/dgvnsk3586/files/lgbtqia-watermark.jpg")
                .WithTitle("Hotlines")
                .WithDescription("**CW ||S//c/d/|| (||uiie||)**\n\n||This is a list of suicide prevention hotlines. Some are for LGBTQIA+ people, and some are for general suicide-prevention. If you need to, please call one of the hotlines that apply to your situation.||")
                .WithFooter("Whether you realize it or not, there are people who care about you. ♥")
                .WithColor(new DiscordColor(3092790)),

            new DiscordEmbedBuilder()
                .WithTitle("LGBTQIA Support Lines")
                .WithColor(new DiscordColor(1270686))
                .AddField("Trevor Project", "||• \"The Trevor Project is the leading national organization providing crisis intervention and suicide prevention services to lesbian, gay, bisexual, transgender, queer & questioning(LGBTQ) young people under 25\"\n• Text, Instant Messaging also available\n• Website: [Click Here](https://www.thetrevorproject.org/about/)\n• Number: (866)-488-7386||")
                .AddField("LGBT National Youth Hotline (ages 23 and under)", "||• Free and Confidential peer support for the LGBTQ and questioning community ages 25 and younger.\n• Mondays to Fridays from 1 pm–9 pm PST\n• Saturday from 9 am–2 pm PST\n• Website: [Click Here](https://www.glbthotline.org/talkline.html)\n• Number: (800)-246-7743||")
                .AddField("LGBT National Hotline", "||• The LGBT National Hotline is for all ages.\n• We provide a safe space that is anonymous and confidential where callers can speak on many different issues and concerns including, but limited to, coming out issues, gender and/or sexuality identities, relationship concerns, bullying, workplace issues, HIV/AIDS anxiety, safer sex information, suicide, and much more.\n• Monday thru Friday from 1PM to 9PM PST - Saturday from 9am to 2pm PST\n• Website: [Click Here](https://www.glbthotline.org/national-hotline.html)\n• Number: (888)-843-4564||")
                .AddField("True Colors United", "||• True Colors focuses on supporting homeless youth. They have a hotline but you can also reach out to them for other resources.\n• Website: [Click Here](https://truecolorsunited.org/)\n• Number: (212)-461-4401||")
                .AddField("Pride Institute LGBTQ Dependency", "||​​​​​​• Substance Dependency Organization\n• Website: [Click Here](https://pride-institute.com/lgbtq-recovery-programs/)\n• Number: (800)-547-7433||"),

            new DiscordEmbedBuilder()
                .WithTitle("General Support Lines")
                .WithColor(new DiscordColor(1270686))
                .AddField("Crisis Text Line", "||• Crisis Text Line* is the free, 24/7, confidential text message service for people in crisis.\n• Website: [Click Here](https://www.crisistextline.org/)\n• Text HOME to 741741 in the United States.||")
                .AddField("Teen Line", "||• If you have a problem or just want to talk with another teen who understands, then this is the right place for you!\n• Website: [Click Here](https://teenlineonline.org/)\n• Text \"TEEN\" to 839863 between 6:00pm - 9:00pm PST to speak with one of our teens(Text STOP to opt out.|| ")
                .AddField("Community Helpline", "||• \"Community Helpline has been taking crisis calls since 1971, helping callers work through feelings that range from loneliness & depression, to suicide. Our goal is to provide hope and compassion to the caller. As a confidential crisis and referral hotline, we have served the local South Bay community and have grown to serve the entire state of California.\"\n• Website: [Click Here](http://chelpline.org/wordpress/about/)\n• Number: (877)-541-2525||")
                .AddField("Wellspace Health Crisis/Sui\\*\\*\\*\\* Prevention", "||• Texting and Instant Chat Messaging also available\n• Website: [Click Here](https://www.wellspacehealth.org/services/behavioral-health-prevention/suicide-prevention/)\n• Number: (916)-368-3111||"),

            new DiscordEmbedBuilder()
                .WithTitle("Topic Specific Support Lines")
                .WithColor(new DiscordColor(1270686))
                .AddField("RAINN: Rape Abuse + Incest National Network", "||• When you call 800.656.HOPE (4673), you’ll be routed to a local RAINN affiliate organization based on the first six digits of your phone number. Cell phone callers have the option to enter the ZIP code of their current location to more accurately locate the nearest sexual assault service provider.\n• Website: [Click Here](https://www.rainn.org/about-national-sexual-assault-telephone-hotline/)\n• Number: (800)656-4673||")
                .AddField("WEAVE Crisis Intervention For Domestic Violence and Sex Trafficking/Sexual Assult", "||• All of WEAVE’s services can be accessed by calling the Support and Information Line. WEAVE’s 24-Hour Support and Information Line offers immediate intervention and support by trained peer counselors. Help is available in over 23 languages.\n• Website: [Click Here](https://www.weaveinc.org/get-help)\n• Number: (916)-920-2952||")
                .AddField("\"Friends for Survival\" Suicide Loss Helpline", "||• We are a 501(c)(3) tax-exempt charitable, national non-profit bereavement outreach organization available to those who are grieving a suicide death of family or friends. We also assist professionals who work with those who are grieving a suicide tragedy. Friends For Survival, organized by and for survivors, has been offering suicide bereavement support services since 1983. All staff and volunteers have been directly impacted by a suicide death.\n• Website: [Click Here](https://friendsforsurvival.org/)\n• Number: (800)-646-7322)||")
        };

        /// <summary>
        ///     The /vent command.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <param name="anonymous">The anonymous option.</param>
        /// <param name="tw">The tw option.</param>
        /// <returns></returns>
        [SlashCommand("vent", "Create a vent.")]
        [GuildOnly]
        [SlashCooldown(5, 300, SlashCooldownBucketType.User)]
        [SlashRequireBotPermissions(Permissions.AccessChannels | Permissions.SendMessages | Permissions.EmbedLinks)]
        [SlashBannable]
        public async Task VentAsync(InteractionContext ctx,
            [Option("anonymous", "Whether or not the vent will show you name to non-moderators.")] bool anonymous,
            [Option("tw", "Whether or not the vent contains potentially triggering content.")] bool tw)
        {
            if (anonymous && !ctx.Guild.GetGuildData(Data).AnonymousVenting)
            {
                await ctx.CreateResponseAsync("⚠️ You are not allowed to make anonymous vents in this server.", true);
                return;
            }

            var responseCache = new List<VentResponse>();

            #region Components
            var ventModal = new DiscordInteractionResponseBuilder()
                .WithTitle("Create the vent")
                .WithCustomId($"ventModal-{DateTimeOffset.Now.ToUnixTimeSeconds()}")
                .AddComponents(new TextInputComponent(label: "Vent", customId: "vent", placeholder: "What do you want to get off your chest?", style: TextInputStyle.Paragraph, min_length: 25, max_length: 1200));

            var respondModal = new DiscordInteractionResponseBuilder()
                .WithTitle("Respond to the vent")
                .WithCustomId($"respondModal-{DateTimeOffset.Now.ToUnixTimeSeconds()}")
                .AddComponents(new TextInputComponent(label: "Response", customId: "response", style: TextInputStyle.Paragraph, min_length: 5, max_length: 1200));

            var respondButton = new DiscordButtonComponent(ButtonStyle.Primary, $"respondButton-{DateTimeOffset.Now.ToUnixTimeSeconds()}", "Respond", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("💬")));

            var hotlinesButton = new DiscordButtonComponent(ButtonStyle.Primary, $"hotlinesButton-{DateTimeOffset.Now.ToUnixTimeSeconds()}", "Send Hotlines", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("🛡")));

            var moderateButton = new DiscordButtonComponent(ButtonStyle.Secondary, $"moderateButton-{DateTimeOffset.Now.ToUnixTimeSeconds()}", "Moderate", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("⚙")));
            #endregion

            #region Event Handlers
            ctx.Client.ModalSubmitted += async (sender, args) =>
            {
                if (args.Interaction.Data.CustomId == ventModal.CustomId)
                {
                    await ctx.Channel.SendMessageAsync(new DiscordMessageBuilder()
                        .WithEmbed(new DiscordEmbedBuilder()
                        .WithAuthor(name: anonymous ? null : ctx.User.Username, iconUrl: anonymous ? null : ctx.User.AvatarUrl)
                        .WithTitle("📨 A new vent has arrived!")
                        .WithDescription((tw ? "||" : null) + args.Values["vent"] + (tw ? "||" : null))
                        .WithFooter(tw ? "This vent contains potentially triggering content!" : null)
                        .WithColor(new DiscordColor(3092790)))
                        .AddComponents(respondButton, moderateButton));

                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        .WithContent("✅ Your vent has been sent.")
                        .AsEphemeral());
                }
                else if (args.Interaction.Data.CustomId == respondModal.CustomId)
                {
                    try
                    {
                        await ctx.Member.SendMessageAsync(new DiscordEmbedBuilder()
                            .WithTitle("📨 A new vent response has arrived.")
                            .WithDescription("> " + args.Values["response"])
                            .WithColor(new DiscordColor(3092790)));

                        await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                            .WithContent("✅ Your response has been sent.")
                            .AsEphemeral());

                        responseCache.Add(new(args.Interaction.User, args.Values["response"], DateTimeOffset.Now));
                    }
                    catch (UnauthorizedException)
                    {
                        await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                            .WithContent("⚠️ The user doesn't have their DMs enabled for this server.")
                            .AsEphemeral());
                    }
                }
            };

            ctx.Client.ComponentInteractionCreated += async (sender, args) =>
            {
                if (args.Id == respondButton.CustomId)
                {
                    await args.Interaction.CreateResponseAsync(InteractionResponseType.Modal, respondModal);
                }
                else if (args.Id == hotlinesButton.CustomId)
                {
                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        .AddEmbeds(Hotlines)
                        .AsEphemeral());
                }
                else if (args.Id == moderateButton.CustomId)
                {
                    var user = args.User as DiscordMember;

                    if (ctx.Guild.GetGuildData(Data).VentModerators.Contains(user.Id) || user.Permissions.HasPermission(Permissions.Administrator) || user.IsOwner)
                    {
                        var embed = new DiscordEmbedBuilder()
                            .WithTitle($"Vent from {ctx.User.Username}")
                            .WithThumbnail(ctx.User.AvatarUrl)
                            .WithDescription($"**Anonymous:** {(anonymous ? "Enabled" : "Disabled")}\n**Trigger Warning:** {(tw ? "Enabled" : "Disabled")}\n**Responses:** {responseCache.Count}")
                            .WithColor(new DiscordColor(3092790));

                        foreach (var response in responseCache)
                            embed.AddField($"Response from {response.creator.Username}", $"> {response.message}" + $"\n\nSent <t:{response.creationTimestamp.ToUnixTimeSeconds()}:R>");

                        await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                            .AddEmbed(embed)
                            .AsEphemeral());
                    }
                    else
                    {
                        await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                            .WithContent("⚠️ You are not a vent moderator. Vent moderators can be set in server settings.")
                            .AsEphemeral());
                    }
                }
            };
            #endregion

            await ctx.CreateResponseAsync(InteractionResponseType.Modal, ventModal);
        }

        /// <summary>
        ///     The /hotlines command.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [SlashCommand("hotlines", "Get a list of hotlines.")]
        public async Task HotlinesAsync(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .AddEmbeds(Hotlines)
                .AsEphemeral());
        }

        /// <summary>
        ///     A cached response to a vent.
        /// </summary>
        /// <param name="creator">The creator of the response.</param>
        /// <param name="message">The response message that was sent.</param>
        /// <param name="creationTimestamp">When the response was created.</param>
        private record VentResponse(DiscordUser creator, string message, DateTimeOffset creationTimestamp);
    }
}