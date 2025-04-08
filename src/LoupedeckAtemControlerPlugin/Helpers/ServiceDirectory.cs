

namespace Loupedeck.LoupedeckAtemControlerPlugin
{

    using System;

    using Loupedeck.LoupedeckAtemControlerPlugin.ATEM;
    using Loupedeck.LoupedeckAtemControlerPlugin.Helpers;

    public static class ServiceDirectory
    {


        public static readonly Type T_StillImageData = typeof(StillImageData);
        public static readonly Type T_AtemControlInterface = typeof(AtemControlInterface);
        public static readonly Type T_BlinkenLightsTimeSource = typeof(BlinkenLightsTimeSource);


        private static readonly Dictionary<Type, Object> sd = new();    // This plugin provides various serives such as Still Image select, 
                                                                        // A Wheel Adjustment which can be used for multiple functions
                                                                        // (e.g. Cross Mix fader) all those services have a common data model which
                                                                        // they share in between various commands adjustments and forther backend objects
                                                                        // instead of keeping them individually an extendible Dictunary is maintained



        public static Object Get(Type t) => sd[t];

        public static void Register(Object o) => sd[o.GetType()] = o;

    }
}

