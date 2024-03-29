﻿// This file is from RainBOT.
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

namespace RainBOT.Core
{
    /// <summary>
    ///     The definitions for tonetags and identities.
    /// </summary>
    public class Definitions
    {
        /// <summary>
        ///     Gets the tonetag definitions for the bot.
        /// </summary>
        public static Dictionary<string, string> Tonetags { get; }
            = new Dictionary<string, string>()
            {
                ["/j"] = "Joking",
                ["/s"] = "Sarcastic",
                ["/sarc"] = "Sarcastic",
                ["/srs"] = "Serious",
                ["/hj"] = "Half joking",
                ["/1qj"] = "One-quarter joking",
                ["/qj"] = "One-quarter joking",
                ["/3qj"] = "Three-quarters joking",
                ["/ij"] = "Inside joke",
                ["/lh"] = "Light-hearted",
                ["/t"] = "Teasing",
                ["/gen"] = "Genuine",
                ["/g"] = "Genuine",
                ["/genq"] = "Genuine question",
                ["/gq"] = "Genuine question",
                ["/rhe"] = "Rhetorical question",
                ["/ref"] = "Reference",
                ["/q"] = "Quote",
                ["/lyr"] = "Lyric",
                ["/l"] = "Lyric",
                ["/para"] = "Paraphrasing",
                ["/info"] = "Informational",
                ["/mod"] = "Moderating",
                ["/c"] = "Copypasta",
                ["/tan"] = "Tangent",
                ["/ot"] = "Off-topic",
                ["/hyp"] = "Hyperbole",
                ["/li"] = "Literal",
                ["/ex"] = "Exaggeration",
                ["/m"] = "Metaphor",
                ["/meta"] = "Metaphor",
                ["/nm"] = "Not mad",
                ["/u"] = "Upset",
                ["/lu"] = "A little upset",
                ["/vu"] = "Very upset",
                ["/nbh"] = "Nobody here",
                ["/sbh"] = "Somebody here",
                ["/nbr"] = "Not being rude",
                ["/nf"] = "Not forced",
                ["/th"] = "Threat",
                ["/nay"] = "Not at you",
                ["/ay"] = "At you",
                ["/nav"] = "Not a vent",
                ["/av"] = "Venting",
                ["/v"] = "Venting",
                ["/vent"] = "Venting",
                ["/p"] = "Platonic",
                ["/plat"] = "Platonic",
                ["/r"] = "Romantic",
                ["/rom"] = "Romantic",
                ["/si"] = "Sexual",
                ["/sx"] = "Sexual",
                ["/x"] = "Sexual",
                ["/nsi"] = "Not sexual",
                ["/fam"] = "Familial",
                ["/pos"] = "Positive connotation",
                ["/pc"] = "Positive connotation",
                ["/neg"] = "Negative connotation",
                ["/nc"] = "Negative connotation",
                ["/neu"] = "Neutral connotation",
            };

        /// <summary>
        ///     Gets the identity definitions for the bot.
        /// </summary>
        public static Dictionary<string, string> Identities { get; }
            = new Dictionary<string, string>()
            {
                ["2s"] = "Two-spirit",
                ["2"] = "Part of two-spirit",
                ["s"] = "Part of two-spirit",
                ["l"] = "Lesbian",
                ["g"] = "Gay",
                ["b"] = "Bi",
                ["t"] = "Trans",
                ["q"] = "Queer",
                ["i"] = "Intersex",
                ["a"] = "Asexual",
                ["+"] = "More",
                ["lgbt"] = "Lesbian, gay, bi, and trans.",
                ["lgbt+"] = "Lesbian, gay, bi, trans, and more.",
                ["lgbtq"] = "Lesbian, gay, bi, trans, and queer.",
                ["lgbtq+"] = "Lesbian, gay, bi, trans, queer, and more.",
                ["lgbtqi"] = "Lesbian, gay, bi, trans, queer, and intersex.",
                ["lgbtqi+"] = "Lesbian, gay, bi, trans, queer, intersex, and more.",
                ["lgbtqia"] = "Lesbian, gay, bi, trans, queer, intersex, and asexual.",
                ["lgbtqia+"] = "Lesbian, gay, bi, trans, queer, intersex, asexual, and more.",
                ["2slgbtqia"] = "Lesbian, gay, bi, trans, queer, two-spirit, intersex, and asexual.",
                ["2slgbtqia+"] = "Lesbian, gay, bi, trans, queer, two-spirit, intersex, asexual, and more.",
                ["two spirit"] = "An indigenous third gender.",
                ["2 spirit"] = "An indigenous third gender.",
                ["two-spirit"] = "An indigenous third gender.",
                ["2-spirit"] = "An indigenous third gender.",
                ["lesbian"] = "A feminine-aligned person sexually and/or romantically attracted to feminine-aligned people.",
                ["gay"] = "A masculine-aligned person sexually and/or romantically attracted to masculine-aligned people. Also used as an umbrella term for queer people.",
                ["bisexual"] = "Someone sexually attracted to people of the same gender AND other genders.",
                ["bi"] = "Someone attracted to people of the same gender and other genders.",
                ["biromantic"] = "Someone romantically attracted to people of the same gender AND other genders.",
                ["transgender"] = "An umbrella term for someone whose gender and assigned sex at birth do not align.",
                ["trans"] = "An umbrella term for someone whose gender and assigned sex at birth do not align.",
                ["transsexual"] = "A transgender person who has changed their sex. (Different from transgender!)",
                ["queer"] = "An umbrella term for someone who isn't straight.",
                ["intersex"] = "A person with both male and female primary sex characteristics.",
                ["asexual"] = "An umbrella term for someone who doesn't fully experience sexual attraction. For example, a cupiosexual person.",
                ["ace"] = "An umbrella term for someone who doesn't fully experience sexual attraction. For example, a cupiosexual person.",
                ["aromantic"] = "An umbrella term for someone who doesn't fully experience romantic attraction.  For example, a cupioromantic person.",
                ["aro"] = "An umbrella term for someone who doesn't fully experience romantic attraction. For example, a cupioromantic person.",
                ["aroace"] = "Someone who is asexual and aromantic.",
                ["a-sepc"] = "An umbrella term for someone on the spectrum of asexuality and/or aromanticism. For example, a demisexual person.",
                ["aspec"] = "An umbrella term for someone on the spectrum of asexuality and/or aromanticism. For example, a demisexual person.",
                ["straight"] = "A heterosexual and heteroromantic person.",
                ["monosexual"] = "An umbrella term for someone sexually attracted to only one gender. For example, an exclusively homosexual and homoromantic woman.",
                ["monoromantic"] = "An umbrella term for someone romantically attracted to only one gender. For example, an exclusively homosexual and homoromantic woman.",
                ["cisgender"] = "Someone whose gender identity and sex assigned at birth do not align.",
                ["cis"] = "Someone whose gender identity and sex assigned at birth do not align.",
                ["cishet"] = "Someone who is not part of the LGBTQIA+ community.",
                ["cishetallo"] = "Someone who is not part of the LGBTQIA+ community.",
                ["cishetalloendo"] = "Someone who is not part of the LGBTQIA+ community.",
                ["allosexual"] = "Someone who fully experiences sexual attraction.",
                ["alloromantic"] = "Someone who fully experiences romantic attraction",
                ["endosex"] = "Someone who is not intersex; their primary sex characteristics fit into the male and female categories.",
                ["pansexual"] = "Someone sexually attracted to all genders, and to whom gender doesn't affect sexual attraction.",
                ["panromantic"] = "Someone romantically attracted to all genders, and to whom gender doesn't affect romantic attraction.",
                ["omnisexual"] = "Someone sexually attracted to all genders, but to whom some aspects of gender still affects sexual attraction.",
                ["omniromantic"] = "Someone romantically attracted to all genders, but to whom some aspects of gender still affects romantic attraction.",
                ["polysexual"] = "An umbrella term for someone who is sexually attracted to more than one gender. For example, an omnisexual person.",
                ["polyromantic"] = "An umbrella term for someone who is romantically attracted to more than one gender. For example, an omniromantic person.",
                ["demisexual"] = "Someone who only experiences sexual attraction to people that they already have a close bond to.",
                ["demiromantic"] = "Someone who only experiences romantic attraction to people that they already have a close bond to.",
                ["aceflux"] = "Someone whose ability to feel sexual attraction fluctuates.",
                ["aroflux"] = "Someone whose ability to feel romantic attraction fluctuates.",
                ["cupiosexual"] = "An asexual person who still experiences sexual desires.",
                ["cupioromantic"] = "An aromantic person who still experiences romantic desires.",
                ["aegosexual"] = "A sex-repulsed asexual person.",
                ["grayasexual"] = "Someone who still experiences sexual attraction, but not to the extent that they would consider themselves allosexual.",
                ["greyasexual"] = "Someone who still experiences sexual attraction, but not to the extent that they would consider themselves allosexual",
                ["grayaromantic"] = "Someone who still experiences romantic attraction, but not to the extent that they would consider themselves alloromantic.",
                ["greyaromantic"] = "Someone who still experiences romantic attraction, but not to the extent that they would consider themselves alloromantic.",
                ["sapphic"] = "Relating to wlw attraction or relationships.",
                ["achillean"] = "Relating to mlm attraction or relationships.",
                ["enbian"] = "Relating to nblnb attraction or relationships.",
                ["toric"] = "Relating to nblm attraction or relationships.",
                ["trixic"] = "Relating to nblw attraction or relationships.",
                ["neptunic"] = "Like sapphic but including feminine-aligned non-binary people.",
                ["vincian"] = "Like achillean but including masculine-aligned non-binary people.",
                ["wlw"] = "Women loving women",
                ["mlm"] = "Men loving men",
                ["nblw"] = "Non-binary people loving women",
                ["nblm"] = "Non-binary people loving men",
                ["nblnb"] = "Non binary people loving non-binary people",
                ["non-binary"] = "Someone whose gender identity doesn't fully fall into the man and woman categories",
                ["nonbinary"] = "Someone whose gender identity doesn't fully fall into the man and woman categories",
                ["enby"] = "Someone whose gender identity doesn't fully fall into the man and woman categories",
                ["nb"] = "Someone whose gender identity doesn't fully fall into the man and woman categories",
                ["genderqueer"] = "Someone whose gender identity doesn't fully fall into the man and woman categories",
                ["agender"] = "Someone who doesn't have a gender.",
                ["bigender"] = "Someone who has two genders.",
                ["trigender"] = "Someone who has three genders.",
                ["polygender"] = "Someone who has more than one gender.",
                ["pangender"] = "Someone who identifies with all genders.",
                ["demiboy"] = "Someone who only partially identifies as a boy.",
                ["demi-boy"] = "Someone who only partially identifies as a boy.",
                ["demi boy"] = "Someone who only partially identifies as a boy.",
                ["demigirl"] = "Someone who only partially identifies as a girl.",
                ["demi-girl"] = "Someone who only partially identifies as a girl.",
                ["demi girl"] = "Someone who only partially identifies as a girl.",
                ["deminonbinary"] = "Someone who only partially identifies as non-binary.",
                ["demi nonbinary"] = "Someone who only partially identifies as a non-binary.",
                ["demi-nonbinary"] = "Someone who only partially identifies as a non-binary.",
                ["deminon-binary"] = "Someone who only partially identifies as a non-binary.",
                ["demi non-binary"] = "Someone who only partially identifies as a non-binary.",
                ["demi-non-binary"] = "Someone who only partially identifies as a non-binary.",
                ["xenogender"] = "A gender for someone who feels like their gender is represented by something typically not associated with gender.",
                ["neopronouns"] = "Pronouns used to replace the pronouns of a language when none of those pronouns apply to someone."
            };
    }
}