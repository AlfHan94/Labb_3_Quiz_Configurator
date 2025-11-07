using System;
using System.Collections.Generic; 

namespace Labb_3_Quiz_Configurator.Models
{
    public static class DifficultyEnumValues
    {
        public static IEnumerable<Difficulty> All { get; } =
            (Difficulty[])Enum.GetValues(typeof(Difficulty));
    }
}
