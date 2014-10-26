﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using palette = SCANsat.SCAN_UI.SCANpalette;

namespace SCANsat.Platform.Palettes
{
	static class PaletteLoader
	{
		internal static List<Palette> palettes = new List<Palette>();
		internal static Palette defaultPalette = generateDefaultPalette();

		private static Palette generateDefaultPalette()
		{
			Color32[] c;
			c = new[] { (Color32)palette.xkcd_DarkPurple, (Color32)palette.xkcd_Cerulean, (Color32)palette.xkcd_ArmyGreen, (Color32)palette.xkcd_Yellow, (Color32)palette.xkcd_Red, (Color32)palette.xkcd_Magenta, (Color32)palette.xkcd_White };
			return new Palette(c, "Default", Palette.Kind.Qualitative, (Palette.Is)2, (Palette.Is)2, (Palette.Is)2, (Palette.Is)2);
		}

		internal static void generatePalettes(Palette.Kind Kind, int Size)
		{
			palettes.Clear();
			if (Kind == Palette.Kind.Diverging)
			{
				palettes.Add(ColorBrewer.BrewerPalettes.Spectral(Size));
				palettes.Add(ColorBrewer.BrewerPalettes.RdYlGn(Size));
				palettes.Add(ColorBrewer.BrewerPalettes.RdBu(Size));
				palettes.Add(ColorBrewer.BrewerPalettes.PiYG(Size));
				palettes.Add(ColorBrewer.BrewerPalettes.PRGn(Size));
				palettes.Add(ColorBrewer.BrewerPalettes.RdYlBu(Size));
				palettes.Add(ColorBrewer.BrewerPalettes.BrBG(Size));
				palettes.Add(ColorBrewer.BrewerPalettes.RdGy(Size));
				palettes.Add(ColorBrewer.BrewerPalettes.PuOr(Size));
			}
			else if (Kind == Palette.Kind.Qualitative)
			{
				palettes.Add(defaultPalette);
				palettes.Add(ColorBrewer.BrewerPalettes.Set2(Size));
				palettes.Add(ColorBrewer.BrewerPalettes.Accent(Size));
				palettes.Add(ColorBrewer.BrewerPalettes.Set1(Size));
				palettes.Add(ColorBrewer.BrewerPalettes.Set3(Size));
				palettes.Add(ColorBrewer.BrewerPalettes.Dark2(Size));
				palettes.Add(ColorBrewer.BrewerPalettes.Paired(Size));
				palettes.Add(ColorBrewer.BrewerPalettes.Pastel2(Size));
				palettes.Add(ColorBrewer.BrewerPalettes.Pastel1(Size));
			}
			else if (Kind == Palette.Kind.Sequential)
			{
				palettes.Add(ColorBrewer.BrewerPalettes.OrRd(Size));
				palettes.Add(ColorBrewer.BrewerPalettes.BuPu(Size));
				//palettes.Add(ColorBrewer.BrewerPalettes.Oranges(Size));
				palettes.Add(ColorBrewer.BrewerPalettes.BuGn(Size));
				palettes.Add(ColorBrewer.BrewerPalettes.YlOrBr(Size));
				palettes.Add(ColorBrewer.BrewerPalettes.YlGn(Size));
				palettes.Add(ColorBrewer.BrewerPalettes.Reds(Size));
				palettes.Add(ColorBrewer.BrewerPalettes.RdPu(Size));
				//palettes.Add(ColorBrewer.BrewerPalettes.Greens(Size));
				palettes.Add(ColorBrewer.BrewerPalettes.YlGnBu(Size));
				//palettes.Add(ColorBrewer.BrewerPalettes.Purples(Size));
				//palettes.Add(ColorBrewer.BrewerPalettes.GnBu(Size));
				//palettes.Add(ColorBrewer.BrewerPalettes.Greys(Size));
				palettes.Add(ColorBrewer.BrewerPalettes.YlOrRd(Size));
				palettes.Add(ColorBrewer.BrewerPalettes.PuRd(Size));
				palettes.Add(ColorBrewer.BrewerPalettes.Blues(Size));
				palettes.Add(ColorBrewer.BrewerPalettes.PuBuGn(Size));
			}
		}
	}
}