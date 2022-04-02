using System;
using System.Collections.Generic;
using System.Text;

namespace GvasFormat.Settings
{
    public partial class GvasSettings
    {
        public static LiveryProjectionPraseMode LiveryProjectionPraseOption { get; set; } = LiveryProjectionPraseMode.Layers;

        public enum LiveryProjectionPraseMode : int 
        {
            ///<summary>Prases Projection Data in it's pure layer form, slower (the default)</summary>
            Layers,
            ///<summary>Prases Projection Data in it's raw data form, faster but less usable</summary>
            Chunks
        }
    }
}
