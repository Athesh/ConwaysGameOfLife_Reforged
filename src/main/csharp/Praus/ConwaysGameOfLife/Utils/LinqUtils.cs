using System;
using System.Linq;
using System.Collections.Generic;


namespace Praus.ConwaysGameOfLife.Utils {
    public static class LinqUtils {
        public static ulong Sum(this IEnumerable<ulong> ulongs) {
            ulong sum = 0;
            foreach (var ul in ulongs) {
                sum += ul;
            }
            return sum;
        }
    }
}

