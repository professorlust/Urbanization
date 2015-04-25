using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.Tilesets
{
    internal class BitmapSelector
    {
        private readonly Bitmap[] _bitmaps;

        internal BitmapSelector(params Bitmap[] bitmaps)
        {
            _bitmaps = bitmaps;
        }

        public Bitmap SelectOneWithId(int id)
        {
            return _bitmaps[id % _bitmaps.Length];
        }
    }

    internal class GrowthZonePredicateAndBitmapSelector<T>
        where T : IAreaObjectWithSeed
    {
        private readonly BitmapSelector _bitmapSelector;
        private readonly Func<T, bool> _predicate;

        public Func<T, bool> Predicate { get { return _predicate; } }
        internal BitmapSelector BitmapSelector { get { return _bitmapSelector; } }

        public GrowthZonePredicateAndBitmapSelector(Func<T, bool> predicate, BitmapSelector bitmapSelector)
        {
            _predicate = predicate;
            _bitmapSelector = bitmapSelector;
        }
    }

    internal class GrowthZoneBitmapSelectorCollection<T>
        where T : IAreaObjectWithSeed
    {
        private readonly IList<GrowthZonePredicateAndBitmapSelector<T>> _predicateAndBitmapSelectors;

        public GrowthZoneBitmapSelectorCollection(params GrowthZonePredicateAndBitmapSelector<T>[] growthZoneAndBitmapSelectors)
        {
            _predicateAndBitmapSelectors = growthZoneAndBitmapSelectors.ToList();
        }

        public Bitmap GetBitmapFor(T cluster)
        {
            return _predicateAndBitmapSelectors
                .Single(x => x.Predicate(cluster))
                .BitmapSelector
                .SelectOneWithId(cluster.Id);
        }
    }

    internal static class BitmapSelectorCollections
    {
        internal static readonly GrowthZoneBitmapSelectorCollection<BaseGrowthZoneClusterConsumption> CommercialCollection = new GrowthZoneBitmapSelectorCollection<BaseGrowthZoneClusterConsumption>(
            new GrowthZonePredicateAndBitmapSelector<BaseGrowthZoneClusterConsumption>(x => x.PopulationDensity == 0,
                new BitmapSelector(BitmapAccessor.GetImage("GrowthZones.Commercial.empty.png"))
            ),
            new GrowthZonePredicateAndBitmapSelector<BaseGrowthZoneClusterConsumption>(x => x.PopulationDensity > 0 && x.PopulationDensity < 25,
                new BitmapSelector(
                    BitmapAccessor.GetImage("GrowthZones.Commercial.d1q1n1.png"),
                    BitmapAccessor.GetImage("GrowthZones.Commercial.d1q1n2.png")
                )
            ),
            new GrowthZonePredicateAndBitmapSelector<BaseGrowthZoneClusterConsumption>(x => x.PopulationDensity >= 25,
                new BitmapSelector(
                    BitmapAccessor.GetImage("GrowthZones.Commercial.d2q1n1.png"),
                    BitmapAccessor.GetImage("GrowthZones.Commercial.d2q1n2.png"),
                    BitmapAccessor.GetImage("GrowthZones.Commercial.d2q1n3.png")
                )
            )
        );

        internal static readonly GrowthZoneBitmapSelectorCollection<BaseGrowthZoneClusterConsumption> IndustrialCollection = new GrowthZoneBitmapSelectorCollection<BaseGrowthZoneClusterConsumption>(
            new GrowthZonePredicateAndBitmapSelector<BaseGrowthZoneClusterConsumption>(x => x.PopulationDensity == 0,
                new BitmapSelector(BitmapAccessor.GetImage("GrowthZones.Industrial.empty.png"))
            ),
            new GrowthZonePredicateAndBitmapSelector<BaseGrowthZoneClusterConsumption>(x => x.PopulationDensity > 0 && x.PopulationDensity < 25,
                new BitmapSelector(
                    BitmapAccessor.GetImage("GrowthZones.Industrial.d1q1n1.png")
                )
            ),
            new GrowthZonePredicateAndBitmapSelector<BaseGrowthZoneClusterConsumption>(x => x.PopulationDensity >= 25,
                new BitmapSelector(
                    BitmapAccessor.GetImage("GrowthZones.Industrial.d2q1n1.png")
                )
            )
        );

        internal static readonly GrowthZoneBitmapSelectorCollection<BaseGrowthZoneClusterConsumption> ResidentialCollection = new GrowthZoneBitmapSelectorCollection<BaseGrowthZoneClusterConsumption>(
            new GrowthZonePredicateAndBitmapSelector<BaseGrowthZoneClusterConsumption>(x => x.PopulationDensity <= 8,
                new BitmapSelector(BitmapAccessor.GetImage("GrowthZones.Residential.empty.png"))
            ),
            new GrowthZonePredicateAndBitmapSelector<BaseGrowthZoneClusterConsumption>(x => x.PopulationDensity > 8 && x.PopulationDensity < 25,
                new BitmapSelector(
                    BitmapAccessor.GetImage("GrowthZones.Residential.d1q1n1.png")
                )
            ),
            new GrowthZonePredicateAndBitmapSelector<BaseGrowthZoneClusterConsumption>(x => x.PopulationDensity >= 25,
                new BitmapSelector(
                    BitmapAccessor.GetImage("GrowthZones.Residential.d2q1n1.png"),
                    BitmapAccessor.GetImage("GrowthZones.Residential.d2q1n2.png"), 
                    BitmapAccessor.GetImage("GrowthZones.Residential.d2q1n3.png")
                )
            )
        );

        internal static readonly GrowthZoneBitmapSelectorCollection<ZoneClusterMemberConsumption> ResidentialHouseCollection = new GrowthZoneBitmapSelectorCollection<ZoneClusterMemberConsumption>(
            new GrowthZonePredicateAndBitmapSelector<ZoneClusterMemberConsumption>(x =>
            {
                var growthZone = x.ParentBaseZoneClusterConsumption as BaseGrowthZoneClusterConsumption;
                if (growthZone == null) throw new InvalidOperationException();
                return growthZone.PopulationDensity <= 8;
            },
                new BitmapSelector(
                    BitmapAccessor.GetImage("GrowthZones.Residential.Houses.d1q1n1.png"),
                    BitmapAccessor.GetImage("GrowthZones.Residential.Houses.d1q1n2.png"),
                    BitmapAccessor.GetImage("GrowthZones.Residential.Houses.d1q1n3.png"),
                    BitmapAccessor.GetImage("GrowthZones.Residential.Houses.d1q1n4.png")
                )
            )
        );

    }

    internal static class BitmapAccessor
    {
        public static readonly AnimatedBitmap PowerPlant = new AnimatedBitmap(250, GetImage("coal1.png"), GetImage("coal2.png"), GetImage("coal3.png"), GetImage("coal4.png"));

        public static readonly Bitmap TrainStation = GetImage("trainstation.png");

        public static readonly Bitmap Police = GetImage("police.png");

        public static class GrowthZones
        {
            public static readonly Bitmap NoElectricity = GetImage("GrowthZones.noelectricity.png");
        }
        public static readonly Bitmap Rubbish = GetImage("rubbish.png");

        public static class NetworkZones
        {
            public static readonly Bitmap RailNorthSouthRoadEastWest = GetImage("NetworkZones.railnsroadew.png");
            public static readonly Lazy<Bitmap> RoadNorthSouthRailEastWest = new Lazy<Bitmap>(() =>
                RailNorthSouthRoadEastWest.Get90DegreesRotatedClone()
            );

            public static readonly Bitmap WaterNorthSouthRoadEastWest = GetImage("NetworkZones.waternsroadew.png");
            public static readonly Lazy<Bitmap> RoadNorthSouthWaterEastWest = new Lazy<Bitmap>(() =>
                WaterNorthSouthRoadEastWest.Get90DegreesRotatedClone()
            );

            public static readonly Bitmap PowerNorthSouthRoadEastWest = GetImage("NetworkZones.powernsroadew.png");
            public static readonly Lazy<Bitmap> RoadNorthSouthPowerEastWest = new Lazy<Bitmap>(() =>
                PowerNorthSouthRoadEastWest.Get90DegreesRotatedClone()
            );

            public static readonly Bitmap RailNorthSouthWaterEastWest = GetImage("NetworkZones.railnswaterew.png");
            public static readonly Lazy<Bitmap> WaterNorthSouthRailEastWest = new Lazy<Bitmap>(() =>
                RailNorthSouthWaterEastWest.Get90DegreesRotatedClone());

            public static readonly Bitmap PowerNorthSouthWaterEastWest = GetImage("NetworkZones.powernswaterew.png");
            public static readonly Lazy<Bitmap> WaterNorthSouthPowerEastWest = new Lazy<Bitmap>(() =>
                PowerNorthSouthWaterEastWest.Get90DegreesRotatedClone()); 


            public static readonly Bitmap RailNorthSouthPowerEastWest = GetImage("NetworkZones.railnspowerew.png");
            public static readonly Lazy<Bitmap> PowerNorthSouthRailEastWest = new Lazy<Bitmap>(() =>
                RailNorthSouthPowerEastWest.Get90DegreesRotatedClone()
            );

            private static NetworkZoneTileset GenerateNetworkZoneTileSet(string formatter)
            {
                return new NetworkZoneTileset(
                    bitmapEast: GetImage(String.Format(formatter, "e")),
                    bitmapEastWest: GetImage(String.Format(formatter, "ew")),
                    bitmapNorthWest: GetImage(String.Format(formatter, "nw")),
                    bitmapWestNorthEast: GetImage(String.Format(formatter, "nwe")),
                    bitmapNorthEastSouthWest: GetImage(String.Format(formatter, "nwes")),
                    bitmapNoDirection: GetImage(String.Format(formatter, string.Empty))
                );
            }

            public static class Road
            {
                public static readonly NetworkZoneTileset RoadZoneTileSet =
                    GenerateNetworkZoneTileSet("NetworkZones.Road.road{0}.png");

                public static Bitmap GetBitmapFor(RoadZoneConsumption consumption)
                {
                    switch (consumption.GetTrafficDensity())
                    {
                        case TrafficDensity.Low:
                            return TrafficAnim.Low.GetBitmapFor(consumption);
                        case TrafficDensity.High:
                            return TrafficAnim.High.GetBitmapFor(consumption);
                        case TrafficDensity.None:
                            return RoadZoneTileSet.GetBitmapFor(consumption);
                        default:
                            throw new NotImplementedException();
                    }
                }

                public static Bitmap GetBitmapFor(IIntersectingZoneConsumption intersection)
                {
                    if (intersection.EastWestZoneConsumption is RoadZoneConsumption ^
                        intersection.NorthSouthZoneConsumption is RoadZoneConsumption)
                    {
                        switch (((intersection.EastWestZoneConsumption as RoadZoneConsumption) ??
                                       (intersection.NorthSouthZoneConsumption as RoadZoneConsumption)).GetTrafficDensity())
                        {
                            case TrafficDensity.None:

                                if (intersection.EastWestZoneConsumption is RailRoadZoneConsumption)
                                {
                                    return RoadNorthSouthRailEastWest.Value;
                                }
                                else if (intersection.NorthSouthZoneConsumption is RailRoadZoneConsumption)
                                {
                                    return RailNorthSouthRoadEastWest;
                                }
                                else if (intersection.EastWestZoneConsumption is PowerLineConsumption)
                                {
                                    return RoadNorthSouthPowerEastWest.Value;
                                }
                                else if (intersection.NorthSouthZoneConsumption is PowerLineConsumption)
                                {
                                    return PowerNorthSouthRoadEastWest;
                                }
                                else if (intersection.NorthSouthZoneConsumption is WaterZoneConsumption)
                                {
                                    return WaterNorthSouthRoadEastWest;
                                }
                                else if (intersection.EastWestZoneConsumption is WaterZoneConsumption)
                                {
                                    return RoadNorthSouthWaterEastWest.Value;
                                }
                                else
                                {
                                    throw new InvalidOperationException();
                                }

                            case TrafficDensity.Low:
                                return TrafficAnim.Low.GetBitmapFor(intersection);
                            case TrafficDensity.High:
                                return TrafficAnim.High.GetBitmapFor(intersection);
                            default:
                                throw new NotImplementedException();
                        }
                    }
                    else throw new ArgumentException("Invalid intersection was specified.", "intersection");
                }

                public static class TrafficAnim
                {
                    private static readonly InfiniteEnumeratorCycler Cycler = new InfiniteEnumeratorCycler();

                    public static readonly AnimatedRoadNetworkZoneTileset Low = new AnimatedRoadNetworkZoneTileset(
                        frameTileSets: EnumerateLowFrames()
                            .Select(frameName => GenerateNetworkZoneTileSet(frameName + ".road{0}.png"))
                            .ToArray(), 
                        powerNorthSouthRoadEastWestFrames: EnumerateLowFrames()
                            .Select(frameName => GetImage(frameName + ".powernsroadew.png"))
                            .ToArray(), 
                        railNorthSouthRoadEastWestFrames: EnumerateLowFrames()
                            .Select(frameName => GetImage(frameName + ".railnsroadew.png"))
                            .ToArray(),
                        waterNorthSouthRoadEastWestFrames: EnumerateLowFrames()
                            .Select(frameName => GetImage(frameName + ".waternsroadew.png"))
                            .ToArray(),
                        cycler: Cycler
                    );
                    public static readonly AnimatedRoadNetworkZoneTileset High = new AnimatedRoadNetworkZoneTileset(
                        frameTileSets: EnumerateHighFrames()
                            .Select(frameName => GenerateNetworkZoneTileSet(frameName + ".road{0}.png"))
                            .ToArray(),
                        powerNorthSouthRoadEastWestFrames: EnumerateHighFrames()
                            .Select(frameName => GetImage(frameName + ".powernsroadew.png"))
                            .ToArray(),
                        railNorthSouthRoadEastWestFrames: EnumerateHighFrames()
                            .Select(frameName => GetImage(frameName + ".railnsroadew.png"))
                            .ToArray(),
                        waterNorthSouthRoadEastWestFrames: EnumerateHighFrames()
                            .Select(frameName => GetImage(frameName + ".waternsroadew.png"))
                            .ToArray(), 
                        cycler: Cycler
                    );

                    private static IEnumerable<string> EnumerateLowFrames()
                    {
                        return
                            Enumerable.Range(1, 5)
                                .Select(iteration => "NetworkZones.Road.TrafficAnim.Low.Frame" + iteration);
                    }

                    private static IEnumerable<string> EnumerateHighFrames()
                    {
                        return
                            Enumerable.Range(1, 5)
                                .Select(iteration => "NetworkZones.Road.TrafficAnim.High.Frame" + iteration);
                    }
                }
            }
            public static class Rail
            {
                public static readonly Bitmap RailNoDirection = GetImage("NetworkZones.Rail.rail.png");
                public static readonly Bitmap RailEast = GetImage("NetworkZones.Rail.raile.png");
                public static readonly Bitmap RailEastWest = GetImage("NetworkZones.Rail.railew.png");
                public static readonly Bitmap RailNorthWest = GetImage("NetworkZones.Rail.railnw.png");
                public static readonly Bitmap RailWestNorthEast = GetImage("NetworkZones.Rail.railnwe.png");
                public static readonly Bitmap RailNorthWestEastSouth = GetImage("NetworkZones.Rail.railnwes.png");
            }
            public static class Power
            {
                public static readonly Bitmap PowerNoDirection = GetImage("NetworkZones.Power.power.png");
                public static readonly Bitmap PowerEast = GetImage("NetworkZones.Power.powere.png");
                public static readonly Bitmap PowerEastWest = GetImage("NetworkZones.Power.powerew.png");
                public static readonly Bitmap PowerNorthWest = GetImage("NetworkZones.Power.powernw.png");
                public static readonly Bitmap PowerWestNorthEast = GetImage("NetworkZones.Power.powernwe.png");
                public static readonly Bitmap PowerNorthWestEastSouth = GetImage("NetworkZones.Power.powernwes.png");
            }
            public static class Wood
            {
                public static readonly Bitmap WoodNoDirection = GetImage("NetworkZones.Wood.wood.png");
                public static readonly Bitmap WoodEast = GetImage("NetworkZones.Wood.woode.png");
                public static readonly Bitmap WoodEastWest = GetImage("NetworkZones.Wood.woodew.png");
                public static readonly Bitmap WoodNorthWest = GetImage("NetworkZones.Wood.woodnw.png");
                public static readonly Bitmap WoodWestNorthEast = GetImage("NetworkZones.Wood.woodnwe.png");
                public static readonly Bitmap WoodNorthWestEastSouth = GetImage("NetworkZones.Wood.woodnwes.png");
            }
            public static class Water
            {
                public static readonly Bitmap WaterNoDirection = GetImage("NetworkZones.Water.water.png");
                public static readonly Bitmap WaterEast = GetImage("NetworkZones.Water.watere.png");
                public static readonly Bitmap WaterEastWest = GetImage("NetworkZones.Water.waterew.png");
                public static readonly Bitmap WaterNorthWest = GetImage("NetworkZones.Water.waternw.png");
                public static readonly Bitmap WaterWestNorthEast = GetImage("NetworkZones.Water.waternwe.png");
                public static readonly Bitmap WaterNorthWestEastSouth = GetImage("NetworkZones.Water.waternwes.png");
            }
        }

        public static Bitmap GetImage(string name)
        {
            Stream file = Assembly.GetExecutingAssembly().GetManifestResourceStream("Mirage.Urbanization.Tilesets." + name);

            if (file == null)
                throw new ArgumentException("Could not extract resource stream for file: " + name, "name");
            return new Bitmap(Image.FromStream(file));
        }
    }

    public static class BitmapExtensions
    {
        public static Bitmap Get90DegreesRotatedClone(this Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");
            var clone = bitmap.Clone() as Bitmap;
            if (clone == null)
                throw new ArgumentException(
                    String.Format("Could not clone 'bitmap' into a new {0} instance.", typeof(Bitmap).Name), "bitmap");

            clone.RotateFlip(RotateFlipType.Rotate90FlipNone);
            return clone;
        }
    }
}