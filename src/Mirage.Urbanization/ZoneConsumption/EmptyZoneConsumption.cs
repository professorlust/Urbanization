﻿using System.Drawing;
using System.Reflection;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.ZoneConsumption
{
    public class EmptyZoneConsumption : BaseSingleZoneConsumption
    {
        public override string Name => "Clear";

        public override char KeyChar => 'd';
        public override BuildStyle BuildStyle => BuildStyle.ClickAndDrag;

        public override Color Color => DefaultColor;

        public override int Cost => 10;

        public static Color DefaultColor = System.Drawing.Color.BurlyWood;

        public override Image Tile => new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Mirage.Urbanization.Tiles.clear.png"));

        public override IGetCanOverrideWithResult GetCanOverrideWith(IAreaZoneConsumption consumption)
        {
            return new AreaZoneConsumptionOverrideInfoResult(
                resultingAreaConsumption: consumption, 
                toBeDeployedAreaConsumption: consumption
            );
        }
    }
}