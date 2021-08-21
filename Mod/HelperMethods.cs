using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Jp.ParahumansOfTheWormverse
{
    public static class HelperMethods
    {
        public static void ReorderTokenPool(this TokenPool[] tokenPools, string poolThatShouldBeFirst)
        {
            var temp = new List<TokenPool>(tokenPools);
            int targetIndex = temp.FindIndex(tp => string.Equals(tp.Identifier, poolThatShouldBeFirst, StringComparison.Ordinal));
            //if targetIndex == -1, no matching pool found, make no change.
            //if targetIndex == 0, matching pool already first, make no change.
            if (targetIndex > 0)
            {
                var newFirst = tokenPools[targetIndex];

                //shuffle all other indexes forward without changing the relative order
                int index = targetIndex;
                while (index > 0)
                {
                    tokenPools[index] = tokenPools[--index];
                }
                tokenPools[0] = newFirst;
            }
        }

        public static void SetPowerNumeralsArray(this ReflectionStatusEffect effect, int[] array)
        {
            var p1 = effect.GetType().GetProperty(nameof(effect.PowerNumeralsToChange));
            var p2 = p1.DeclaringType.GetProperty(nameof(effect.PowerNumeralsToChange));

            p2.SetValue(effect, array, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, null, null);
        }
    }
}
