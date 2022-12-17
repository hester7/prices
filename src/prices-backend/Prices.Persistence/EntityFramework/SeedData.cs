using NodaTime;
using Prices.Core.Domain.Models;
using Prices.Core.Domain.Enums;

namespace Prices.Persistence.EntityFramework
{
    public static class SeedData
    {
        public static PriceIndex[] PriceIndexes => new[]
        {
            new PriceIndex
            {
                Id = Core.Domain.Enums.PriceIndexes.CaisoDayAhead,
                Name = "CAISO - Day-Ahead",
                RegionalTransmissionOperatorId = Rtos.CAISO,
                PriceMarketId = Core.Domain.Enums.PriceMarkets.DAM,
            },
            new PriceIndex
            {
                Id = Core.Domain.Enums.PriceIndexes.CaisoRealTime,
                Name = "CAISO - Real-Time",
                RegionalTransmissionOperatorId = Rtos.CAISO,
                PriceMarketId = Core.Domain.Enums.PriceMarkets.RTM,
            },
            new PriceIndex
            {
                Id = Core.Domain.Enums.PriceIndexes.ErcotDayAhead,
                Name = "ERCOT - Day-Ahead",
                RegionalTransmissionOperatorId = Rtos.ERCOT,
                PriceMarketId = Core.Domain.Enums.PriceMarkets.DAM,
            },
            new PriceIndex
            {
                Id = Core.Domain.Enums.PriceIndexes.ErcotRealTime,
                Name = "ERCOT - Real-Time",
                RegionalTransmissionOperatorId = Rtos.ERCOT,
                PriceMarketId = Core.Domain.Enums.PriceMarkets.RTM,
            },

            //new PriceIndex
            //{
            //    PriceIndexId = Core.Domain.Enums.PriceIndexes.CaisoFifteenMinute,
            //    Name = "CAISO - Fifteen-Minute",
            //    RegionalTransmissionOperatorId = Rtos.CAISO,
            //    PriceMarketId = Core.Domain.Enums.PriceMarkets.FMM,
            //},
            //new PriceIndex
            //{
            //    PriceIndexId = Core.Domain.Enums.PriceIndexes.ErcotFifteenMinute,
            //    Name = "ERCOT - Fifteen-Minute",
            //    RegionalTransmissionOperatorId = Rtos.ERCOT,
            //    PriceMarketId = Core.Domain.Enums.PriceMarkets.FMM,
            //},
        };

        public static PriceMarket[] PriceMarkets => new[]
        {
            new PriceMarket
            {
                Id = Core.Domain.Enums.PriceMarkets.DAM,
                Name = "Day-Ahead",
                Abbreviation = "DAM"
            },
            new PriceMarket
            {
                Id = Core.Domain.Enums.PriceMarkets.RTM,
                Name = "Real-Time",
                Abbreviation = "RTM"
            },
            //new PriceMarket
            //{
            //    PriceMarketId = Core.Domain.Enums.PriceMarkets.FMM,
            //    Name = "Fifteen-Minute",
            //    Abbreviation = "FMM"
            //}
        };

        public static PriceType[] PriceTypes => new[]
        {
            new PriceType
            {
                Id = Core.Domain.Enums.PriceTypes.Historical,
                Name = "Historical"
            },
            new PriceType
            {
                Id = Core.Domain.Enums.PriceTypes.Daily,
                Name = "Daily"
            },
            new PriceType
            {
                Id = Core.Domain.Enums.PriceTypes.Current,
                Name = "Current"
            },
        };

        public static PricingNode[] PricingNodes
        {
            get
            {
                var now = SystemClock.Instance.GetCurrentInstant();

                return new[]
                {
                    new PricingNode
                    {
                        Id = 1,
                        RegionalTransmissionOperatorId = Rtos.CAISO,
                        Name = "TH_NP15_GEN-APND",
                        DisplayName = "NP15",
                        PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.Hub,
                        CreatedAtUtc = now,
                        LastModifiedAtUtc = now
                    },
                    new PricingNode
                    {
                        Id = 2,
                        RegionalTransmissionOperatorId = Rtos.CAISO,
                        Name = "TH_SP15_GEN-APND",
                        DisplayName = "SP15",
                        PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.Hub,
                        CreatedAtUtc = now,
                        LastModifiedAtUtc = now
                    },
                    new PricingNode
                    {
                        Id = 3,
                        RegionalTransmissionOperatorId = Rtos.CAISO,
                        Name = "TH_ZP26_GEN-APND",
                        DisplayName = "ZP26",
                        PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.Hub,
                        CreatedAtUtc = now,
                        LastModifiedAtUtc = now
                    },
                    new PricingNode
                    {
                        Id = 4,
                        RegionalTransmissionOperatorId = Rtos.CAISO,
                        Name = "DLAP_PGAE-APND",
                        DisplayName = "PGE",
                        PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.DLAP,
                        CreatedAtUtc = now,
                        LastModifiedAtUtc = now
                    },
                    new PricingNode
                    {
                        Id = 5,
                        RegionalTransmissionOperatorId = Rtos.CAISO,
                        Name = "DLAP_SCE-APND",
                        DisplayName = "SCE",
                        PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.DLAP,
                        CreatedAtUtc = now,
                        LastModifiedAtUtc = now
                    },
                    new PricingNode
                    {
                        Id = 6,
                        RegionalTransmissionOperatorId = Rtos.CAISO,
                        Name = "DLAP_SDGE-APND",
                        DisplayName = "SDGE",
                        PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.DLAP,
                        CreatedAtUtc = now,
                        LastModifiedAtUtc = now
                    },
                    new PricingNode
                    {
                        Id = 7,
                        RegionalTransmissionOperatorId = Rtos.CAISO,
                        Name = "DLAP_VEA-APND",
                        DisplayName = "VEA",
                        PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.DLAP,
                        CreatedAtUtc = now,
                        LastModifiedAtUtc = now
                    },
                    new PricingNode
                    {
                        Id = 8,
                        RegionalTransmissionOperatorId = Rtos.ERCOT,
                        Name = "HB_BUSAVG",
                        DisplayName = "HB_BUSAVG",
                        PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.Hub,
                        CreatedAtUtc = now,
                        LastModifiedAtUtc = now
                    },
                    new PricingNode
                    {
                        Id = 9,
                        RegionalTransmissionOperatorId = Rtos.ERCOT,
                        Name = "HB_HOUSTON",
                        DisplayName = "HB_HOUSTON",
                        PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.Hub,
                        CreatedAtUtc = now,
                        LastModifiedAtUtc = now
                    },
                    new PricingNode
                    {
                        Id = 10,
                        RegionalTransmissionOperatorId = Rtos.ERCOT,
                        Name = "HB_HUBAVG",
                        DisplayName = "HB_HUBAVG",
                        PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.Hub,
                        CreatedAtUtc = now,
                        LastModifiedAtUtc = now
                    },
                    new PricingNode
                    {
                        Id = 11,
                        RegionalTransmissionOperatorId = Rtos.ERCOT,
                        Name = "HB_NORTH",
                        DisplayName = "HB_NORTH",
                        PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.Hub,
                        CreatedAtUtc = now,
                        LastModifiedAtUtc = now
                    },
                    new PricingNode
                    {
                        Id = 12,
                        RegionalTransmissionOperatorId = Rtos.ERCOT,
                        Name = "HB_SOUTH",
                        DisplayName = "HB_SOUTH",
                        PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.Hub,
                        CreatedAtUtc = now,
                        LastModifiedAtUtc = now
                    },
                    new PricingNode
                    {
                        Id = 13,
                        RegionalTransmissionOperatorId = Rtos.ERCOT,
                        Name = "HB_WEST",
                        DisplayName = "HB_WEST",
                        PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.Hub,
                        CreatedAtUtc = now,
                        LastModifiedAtUtc = now
                    },
                    new PricingNode
                    {
                        Id = 14,
                        RegionalTransmissionOperatorId = Rtos.ERCOT,
                        Name = "LZ_AEN",
                        DisplayName = "LZ_AEN",
                        PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.DLAP,
                        CreatedAtUtc = now,
                        LastModifiedAtUtc = now
                    },
                    new PricingNode
                    {
                        Id = 15,
                        RegionalTransmissionOperatorId = Rtos.ERCOT,
                        Name = "LZ_CPS",
                        DisplayName = "LZ_CPS",
                        PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.DLAP,
                        CreatedAtUtc = now,
                        LastModifiedAtUtc = now
                    },
                    new PricingNode
                    {
                        Id = 16,
                        RegionalTransmissionOperatorId = Rtos.ERCOT,
                        Name = "LZ_HOUSTON",
                        DisplayName = "LZ_HOUSTON",
                        PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.DLAP,
                        CreatedAtUtc = now,
                        LastModifiedAtUtc = now
                    },
                    new PricingNode
                    {
                        Id = 17,
                        RegionalTransmissionOperatorId = Rtos.ERCOT,
                        Name = "LZ_LCRA",
                        DisplayName = "LZ_LCRA",
                        PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.DLAP,
                        CreatedAtUtc = now,
                        LastModifiedAtUtc = now
                    },
                    new PricingNode
                    {
                        Id = 18,
                        RegionalTransmissionOperatorId = Rtos.ERCOT,
                        Name = "LZ_NORTH",
                        DisplayName = "LZ_NORTH",
                        PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.DLAP,
                        CreatedAtUtc = now,
                        LastModifiedAtUtc = now
                    },
                    new PricingNode
                    {
                        Id = 19,
                        RegionalTransmissionOperatorId = Rtos.ERCOT,
                        Name = "LZ_RAYBN",
                        DisplayName = "LZ_RAYBN",
                        PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.DLAP,
                        CreatedAtUtc = now,
                        LastModifiedAtUtc = now
                    },
                    new PricingNode
                    {
                        Id = 20,
                        RegionalTransmissionOperatorId = Rtos.ERCOT,
                        Name = "LZ_SOUTH",
                        DisplayName = "LZ_SOUTH",
                        PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.DLAP,
                        CreatedAtUtc = now,
                        LastModifiedAtUtc = now
                    },
                    new PricingNode
                    {
                        Id = 21,
                        RegionalTransmissionOperatorId = Rtos.ERCOT,
                        Name = "LZ_WEST",
                        DisplayName = "LZ_WEST",
                        PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.DLAP,
                        CreatedAtUtc = now,
                        LastModifiedAtUtc = now
                    },
                };
            }
        }

        public static PricingNodeType[] PricingNodeTypes => new[]
        {
            new PricingNodeType
            {
                Id = Core.Domain.Enums.PricingNodeTypes.Unknown,
                Name = "Unknown",
                Description = "Unknown"
            },
            new PricingNodeType
            {
                Id = Core.Domain.Enums.PricingNodeTypes.Hub,
                Name = "Hub",
                Description = "Trading Hub"
            },
            new PricingNodeType
            {
                Id = Core.Domain.Enums.PricingNodeTypes.DLAP,
                Name = "DLAP",
                Description = "Default Load Aggregation Point"
            },
            //new PricingNodeType
            //{
            //    PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.SLAP,
            //    Name = "SLAP",
            //    Description = "Sub Default Load Aggregation Point"
            //},
            //new PricingNodeType
            //{
            //    PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.CLAP,
            //    Name = "CLAP",
            //    Description = "Custom Load Aggregation Point"
            //},
            //new PricingNodeType
            //{
            //    PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.ASR,
            //    Name = "Aggregate System Resource",
            //    Description = "Aggregate System Resource"
            //},
            //new PricingNodeType
            //{
            //    PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.AG,
            //    Name = "Aggregate Generation",
            //    Description = "Aggregate Generation"
            //},
            //new PricingNodeType
            //{
            //    PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.POD,
            //    Name = "Point of Delivery",
            //    Description = "Point of Delivery"
            //},
            //new PricingNodeType
            //{
            //    PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.DCA,
            //    Name = "DCA",
            //    Description = "DCA"
            //},
            //new PricingNodeType
            //{
            //    PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.DEPZ,
            //    Name = "DEPZ",
            //    Description = "DEPZ"
            //},
            //new PricingNodeType
            //{
            //    PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.EIMT,
            //    Name = "EIMT",
            //    Description = "EIMT"
            //},
            //new PricingNodeType
            //{
            //    PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.EPZ,
            //    Name = "EPZ",
            //    Description = "EPZ"
            //},
            //new PricingNodeType
            //{
            //    PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.ASP,
            //    Name = "ASP",
            //    Description = "ASP"
            //},
            //new PricingNodeType
            //{
            //    PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.CASP,
            //    Name = "CASP",
            //    Description = "CASP"
            //},
            //new PricingNodeType
            //{
            //    PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.DASP,
            //    Name = "DASP",
            //    Description = "DASP"
            //},
        };

        public static PricingNodeTypeMapping[] PricingNodeTypeMappings => new[]
        {
            new PricingNodeTypeMapping
            {
                PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.Hub,
                RegionalTransmissionOperatorId = Rtos.CAISO,
                Code = "TH"
            },
            new PricingNodeTypeMapping
            {
                PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.DLAP,
                RegionalTransmissionOperatorId = Rtos.CAISO,
                Code = "DPZ"
            },
            //new PricingNodeTypeMapping
            //{
            //    PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.SLAP,
            //    RegionalTransmissionOperatorId = Rtos.CAISO,
            //    Code = "SPZ"
            //},
            //new PricingNodeTypeMapping
            //{
            //    PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.CLAP,
            //    RegionalTransmissionOperatorId = Rtos.CAISO,
            //    Code = "CPZ"
            //},
            //new PricingNodeTypeMapping
            //{
            //    PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.ASR,
            //    RegionalTransmissionOperatorId = Rtos.CAISO,
            //    Code = "ASR"
            //},
            //new PricingNodeTypeMapping
            //{
            //    PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.AG,
            //    RegionalTransmissionOperatorId = Rtos.CAISO,
            //    Code = "AG"
            //},
            //new PricingNodeTypeMapping
            //{
            //    PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.POD,
            //    RegionalTransmissionOperatorId = Rtos.CAISO,
            //    Code = "POD"
            //},
            //new PricingNodeTypeMapping
            //{
            //    PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.DCA,
            //    RegionalTransmissionOperatorId = Rtos.CAISO,
            //    Code = "DCA"
            //},
            //new PricingNodeTypeMapping
            //{
            //    PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.DEPZ,
            //    RegionalTransmissionOperatorId = Rtos.CAISO,
            //    Code = "DEPZ"
            //},
            //new PricingNodeTypeMapping
            //{
            //    PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.EIMT,
            //    RegionalTransmissionOperatorId = Rtos.CAISO,
            //    Code = "EIMT"
            //},
            //new PricingNodeTypeMapping
            //{
            //    PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.EPZ,
            //    RegionalTransmissionOperatorId = Rtos.CAISO,
            //    Code = "EPZ"
            //},

            //new PricingNodeTypeMapping
            //{
            //    PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.ASP,
            //    RegionalTransmissionOperatorId = Rtos.CAISO,
            //    Code = "ASP"
            //},
            //new PricingNodeTypeMapping
            //{
            //    PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.CASP,
            //    RegionalTransmissionOperatorId = Rtos.CAISO,
            //    Code = "CASP"
            //},
            //new PricingNodeTypeMapping
            //{
            //    PricingNodeTypeId = Core.Domain.Enums.PricingNodeTypes.DASP,
            //    RegionalTransmissionOperatorId = Rtos.CAISO,
            //    Code = "DASP"
            //},
        };

        public static RegionalTransmissionOperator[] RegionalTransmissionOperators => new[]
        {
            new RegionalTransmissionOperator
            {
                Id = Rtos.CAISO,
                Name = "CAISO",
                LegalName = "California ISO"
            },
            new RegionalTransmissionOperator
            {
                Id = Rtos.ERCOT,
                Name = "ERCOT",
                LegalName = "Electric Reliability Council of Texas"
            }
        };
    }
}
