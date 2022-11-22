﻿// This file is from RainBOT (https://github.com/BujjuIsDumb/RainBOT)
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

using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using RainBOT.Core.Entities.Services;

namespace RainBOT.Modules
{
    public class Hotlines : ApplicationCommandModule
    {
        public RbService Service { private get; set; }

        [SlashCommand("hotlines", "Get a list of hotlines.")]
        public async Task HotlinesAsync(InteractionContext ctx)
        {
            var embed1 = new DiscordEmbedBuilder()
                .WithAuthor("LGBTQIA Resource Center", "https://lgbtqia.ucdavis.edu/support/hotlines", "https://lgbtqia.ucdavis.edu/sites/g/files/dgvnsk3586/files/lgbtqia-watermark.jpg")
                .WithTitle("Hotlines")
                .WithDescription("**CW ||S//c/d/|| (||uiie||)**\n\n||This is a list of suicide " +
                "prevention hotlines. Some are for LGBTQIA+ people, and some are for general " +
                "suicide-prevention. If you need to, please call one of the hotlines that apply " +
                "to your situation.||")
                .WithFooter("Whether you realize it or not, there are people who care about you. ♥")
                .WithColor(new DiscordColor(3092790));

            var embed2 = new DiscordEmbedBuilder()
                .WithTitle("LGBTQIA Support Lines")
                .WithColor(new DiscordColor(1270686))
                .AddField("Trevor Project", "||• \"The Trevor Project is the leading national " +
                "organization providing crisis intervention and suicide prevention services to " +
                "lesbian, gay, bisexual, transgender, queer & questioning(LGBTQ) young people " +
                "under 25\"\n• Text, Instant Messaging also available\n• Website: [Click Here](" +
                "https://www.thetrevorproject.org/about/)\n• Number: (866)-488-7386||")
                .AddField("LGBT National Youth Hotline (ages 23 and under)", "||• Free and " +
                "Confidential peer support for the LGBTQ and questioning community ages 25 and " +
                "younger.\n• Mondays to Fridays from 1 pm–9 pm PST\n• Saturday from 9 am–2 pm " +
                "PST\n• Website: [Click Here](https://www.glbthotline.org/talkline.html)\n• " +
                "Number: (800)-246-7743||")
                .AddField("LGBT National Hotline", "||• The LGBT National Hotline is for all " +
                "ages.\n• We provide a safe space that is anonymous and confidential where " +
                "callers can speak on many different issues and concerns including, but limited " +
                "to, coming out issues, gender and/or sexuality identities, relationship " +
                "concerns, bullying, workplace issues, HIV/AIDS anxiety, safer sex information, " +
                "suicide, and much more.\n• Monday thru Friday from 1PM to 9PM PST - Saturday " +
                "from 9am to 2pm PST\n• Website: [Click Here](https://www.glbthotline.org/" +
                "national-hotline.html)\n• Number: (888)-843-4564||")
                .AddField("True Colors United", "||• True Colors focuses on supporting homeless " +
                "youth. They have a hotline but you can also reach out to them for other " +
                "resources.\n• Website: [Click Here](https://truecolorsunited.org/)\n• Number: (" +
                "212)-461-4401||")
                .AddField("Pride Institute LGBTQ Dependency", "||​​​​​​• Substance Dependency " +
                "Organization\n• Website: [Click Here](" +
                "https://pride-institute.com/lgbtq-recovery-programs/)\n• Number: (800)-547-7433" +
                "||");

            var embed3 = new DiscordEmbedBuilder()
                .WithTitle("General Support Lines")
                .WithColor(new DiscordColor(1270686))
                .AddField("Crisis Text Line", "||• Crisis Text Line* is the free, 24/7, " +
                "confidential text message service for people in crisis.\n• Website: [Click Here" +
                "](https://www.crisistextline.org/)\n• Text HOME to 741741 in the United States." +
                "||")
                .AddField("Teen Line", "||• If you have a problem or just want to talk with " +
                "another teen who understands, then this is the right place for you!\n• Website" +
                ": [Click Here](https://teenlineonline.org/)\n• Text \"TEEN\" to 839863 between " +
                "6:00pm - 9:00pm PST to speak with one of our teens(Text STOP to opt out.|| ")
                .AddField("Community Helpline", "||• \"Community Helpline has been taking " +
                "crisis calls since 1971, helping callers work through feelings that range from " +
                "loneliness & depression, to suicide. Our goal is to provide hope and " +
                "compassion to the caller. As a confidential crisis and referral hotline, we " +
                "have served the local South Bay community and have grown to serve the entire " +
                "state of California.\"\n• Website: [Click Here](" +
                "http://chelpline.org/wordpress/about/)\n• Number: (877)-541-2525||")
                .AddField("Wellspace Health Crisis/Sui\\*\\*\\*\\* Prevention", "||• Texting " +
                "and Instant Chat Messaging also available\n• Website: [Click Here](" +
                "https://www.wellspacehealth.org/services/behavioral-health-prevention/suicide-prevention/" +
                ")\n• Number: (916)-368-3111||");

            var embed4 = new DiscordEmbedBuilder()
                .WithTitle("Topic Specific Support Lines")
                .WithColor(new DiscordColor(1270686))
                .AddField("RAINN: Rape Abuse + Incest National Network", "||• When " +
                "you call 800.656.HOPE (4673), you’ll be routed to a local RAINN affiliate " +
                "organization based on the first six digits of your phone number. Cell phone " +
                "callers have the option to enter the ZIP code of their current location to " +
                "more accurately locate the nearest sexual assault service provider.\n• Website" +
                ": [Click Here](" +
                "https://www.rainn.org/about-national-sexual-assault-telephone-hotline/)\n• " +
                "Number: (800)656-4673||")
                .AddField("WEAVE Crisis Intervention For Domestic Violence " +
                "and Sex Trafficking/Sexual Assult", "||• All of " +
                "WEAVE’s services can be accessed by calling the Support and Information Line. " +
                "WEAVE’s 24-Hour Support and Information Line offers immediate intervention and " +
                "support by trained peer counselors. Help is available in over 23 languages.\n• " +
                "Website: [Click Here](https://www.weaveinc.org/get-help)\n• Number: (916)-920-" +
                "2952||")
                .AddField("\"Friends for Survival\" Suicide Loss Helpline", "||• We are " +
                "a 501(c)(3) tax-exempt charitable, national non-profit bereavement outreach " +
                "organization available to those who are grieving a suicide death of family or " +
                "friends. We also assist professionals who work with those who are grieving a " +
                "suicide tragedy. Friends For Survival, organized by and for survivors, has " +
                "been offering suicide bereavement support services since 1983. All staff and " +
                "volunteers have been directly impacted by a suicide death.\n• Website: [Click " +
                "Here](https://friendsforsurvival.org/)\n• Number: (800)-646-7322)||");

            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddEmbeds(new DiscordEmbed[] { embed1, embed2, embed3, embed4 })
                .AsEphemeral());
        }
    }
}