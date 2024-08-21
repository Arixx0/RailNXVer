using System;
using System.Collections.Generic;

namespace Utility
{
    public static class CollectionsUtility
    {
        /// <summary>Shuffles given list with fisher-yates method.</summary>
        /// <param name="list">list to shuffle.</param>
        /// <param name="random">random instance for generating random index.</param>
        public static void Shuffle<T>(this List<T> list, int shuffleCount = 1, System.Random random = null)
        {
            random ??= new Random();

            while (shuffleCount > 0)
            {
                --shuffleCount;
                
                var endIndex = list.Count;
                while (endIndex > 1)
                {
                    --endIndex;

                    var randomIndex = random.Next(endIndex + 1);
                    (list[endIndex], list[randomIndex]) = (list[randomIndex], list[endIndex]);
                }
            }
        }
    }
}