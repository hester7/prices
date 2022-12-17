using System.ComponentModel;

namespace Prices.Core.Domain.Enums
{
    public enum PricingNodeTypes
    {
        Unknown = 0,

        [Description("Trading Hub")]
        Hub = 1,

        /// <summary>
        /// Default Load Aggregation Point
        /// </summary>
        [Description("DLAP")]
        DLAP = 2,

        ///// <summary>
        ///// Sub Default Load Aggregation Point
        ///// </summary>
        //[Description("SLAP")]
        //SLAP = 3,

        ///// <summary>
        ///// Custom Load Aggregation Point
        ///// </summary>
        //[Description("CLAP")]
        //CLAP = 4,

        //[Description("Aggregate System Resource")]
        //ASR = 5,

        //[Description("Aggregate Generation")]
        //AG = 6,

        //[Description("Point of Delivery")]
        //POD = 7,

        //// Unknown
        //DCA = 8,
        //DEPZ = 9,
        //EIMT = 10,
        //EPZ = 11,

        //// Ahead Scheduling Process
        //ASP = 12,
        //CASP = 13,
        //DASP = 14,
    }
}
