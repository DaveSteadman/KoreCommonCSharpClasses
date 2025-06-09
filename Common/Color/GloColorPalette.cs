using System.Collections.Generic;

public static class GloColorPalette
{
    public static readonly Dictionary<string, GloColorRGB> Colors = new Dictionary<string, GloColorRGB>
    {
        // Primary colors (1s & 0s)
        { "Red",         new GloColorRGB(255,   0,   0) },
        { "Green",       new GloColorRGB(  0, 255,   0) },
        { "Blue",        new GloColorRGB(  0,   0, 255) },
        { "Yellow",      new GloColorRGB(255, 255,   0) },
        { "Magenta",     new GloColorRGB(255,   0, 255) },
        { "Cyan",        new GloColorRGB(  0, 255, 255) },

        // Monochrome colors
        { "Black",       new GloColorRGB(  0,   0,   0) },
        { "DarkGrey",    new GloColorRGB( 64,  64,  64) },
        { "Grey",        new GloColorRGB(128, 128, 128) },
        { "LightGrey",   new GloColorRGB(192, 192, 192) },
        { "OffWhite",    new GloColorRGB(230, 230, 230) },
        { "White",       new GloColorRGB(255, 255, 255) },

        { "Grey05pct",   new GloColorRGB( 13,  13,  13) },
        { "Grey10pct",   new GloColorRGB( 26,  26,  26) },
        { "Grey15pct",   new GloColorRGB( 38,  38,  38) },
        { "Grey20pct",   new GloColorRGB( 51,  51,  51) },
        { "Grey25pct",   new GloColorRGB( 64,  64,  64) },
        { "Grey30pct",   new GloColorRGB( 77,  77,  77) },
        { "Grey35pct",   new GloColorRGB( 89,  89,  89) },
        { "Grey40pct",   new GloColorRGB(102, 102, 102) },
        { "Grey45pct",   new GloColorRGB(115, 115, 115) },
        { "Grey50pct",   new GloColorRGB(128, 128, 128) },
        { "Grey55pct",   new GloColorRGB(140, 140, 140) },
        { "Grey60pct",   new GloColorRGB(153, 153, 153) },
        { "Grey65pct",   new GloColorRGB(166, 166, 166) },
        { "Grey70pct",   new GloColorRGB(179, 179, 179) },
        { "Grey75pct",   new GloColorRGB(192, 192, 192) },
        { "Grey80pct",   new GloColorRGB(204, 204, 204) },
        { "Grey85pct",   new GloColorRGB(217, 217, 217) },
        { "Grey90pct",   new GloColorRGB(230, 230, 230) },
        { "Grey95pct",   new GloColorRGB(242, 242, 242) },

        // Light color pastels
        { "LightRed",      new GloColorRGB(255, 179, 179) },
        { "LightGreen",    new GloColorRGB(179, 255, 179) },
        { "LightBlue",     new GloColorRGB(179, 179, 255) },
        { "LightYellow",   new GloColorRGB(255, 255, 179) },
        { "LightCyan",     new GloColorRGB(179, 255, 255) },
        { "LightMagenta",  new GloColorRGB(255, 179, 255) },
        { "LightOrange",   new GloColorRGB(255, 217, 179) },
        { "LightPurple",   new GloColorRGB(217, 179, 255) },

        // Dark versions
        { "DarkRed",     new GloColorRGB( 64,   0,   0) },
        { "DarkGreen",   new GloColorRGB(  0,  64,   0) },
        { "DarkBlue",    new GloColorRGB(  0,   0,  64) },
        { "DarkYellow",  new GloColorRGB( 64,  64,   0) },
        { "DarkCyan",    new GloColorRGB(  0,  64,  64) },
        { "DarkMagenta", new GloColorRGB( 64,   0,  64) },

        // Secondary and midtone colors
        { "Navy",        new GloColorRGB(  0,   0, 128) },
        { "MidGreen",    new GloColorRGB(  0, 128,   0) },
        { "Teal",        new GloColorRGB(  0, 128, 128) },
        { "Azure",       new GloColorRGB(  0, 128, 255) },
        { "SpringGreen", new GloColorRGB(  0, 255, 128) },
        { "Maroon",      new GloColorRGB(128,   0,   0) },
        { "Purple",      new GloColorRGB(128,   0, 128) },
        { "Violet",      new GloColorRGB(128,   0, 255) },
        { "Olive",       new GloColorRGB(128, 128,   0) },
        { "MidBlue",     new GloColorRGB(128, 128, 255) },
        { "Chartreuse",  new GloColorRGB(128, 255,   0) },
        { "MidCyan",     new GloColorRGB(128, 255, 255) },
        { "Rose",        new GloColorRGB(255,   0, 128) },
        { "Orange",      new GloColorRGB(255, 128,   0) },

        // Notable named colors
        { "Gold",        new GloColorRGB(255, 214,   0) },
        { "Silver",      new GloColorRGB(192, 192, 192) },
        { "Bronze",      new GloColorRGB(204, 133,  64) },
        { "Brown",       new GloColorRGB(153, 102,  51) },
        { "Pink",        new GloColorRGB(255, 191, 204) },
        { "Lime",        new GloColorRGB(191, 255,   0) },
        { "Indigo",      new GloColorRGB( 74,   0, 130) },
        { "Copper",      new GloColorRGB(184, 115,  51) },
        { "Coral",       new GloColorRGB(255, 128,  79) },
        { "Lavender",    new GloColorRGB(230, 230, 255) },
        { "Peach",       new GloColorRGB(255, 217, 179) },
        { "Mint",        new GloColorRGB(179, 255, 179) },

        // Off/Muted colors
        { "MutedYellow", new GloColorRGB(204, 204,  77) },
        { "MutedGreen",  new GloColorRGB( 77, 204,  77) },
        { "MutedBlue",   new GloColorRGB( 77,  77, 230) },
        { "MutedRed",    new GloColorRGB(204,  77,  77) },
        { "MutedPurple", new GloColorRGB(204,  77, 204) },
        { "MutedCyan",   new GloColorRGB( 77, 204, 204) },
        { "MutedOrange", new GloColorRGB(204, 128,  77) },
        { "DustyPink",   new GloColorRGB(204, 128, 128) },
        { "MutedBrown",  new GloColorRGB(204, 153, 102) },
        { "MutedLime",   new GloColorRGB(128, 204,  77) },
        { "MutedIndigo", new GloColorRGB( 77,   0, 128) },
        { "MutedGold",   new GloColorRGB(204, 179,   0) },
        { "MutedSilver", new GloColorRGB(153, 153, 153) },
        { "MutedBronze", new GloColorRGB(179, 128,  77) }
    };
}
