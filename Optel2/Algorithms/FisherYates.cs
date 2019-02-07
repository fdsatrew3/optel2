using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    public static class FisherYates
    {
        private static Random _Local;

        private static Random _ThisThreadsRandom
        {
            get
            {
                return _Local ?? (_Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId)));
            }
        }

        /// <summary>
        /// Использует алгоритм Фишера-Йетса для случайного перемешивания списка <paramref name="list"/>
        /// </summary>
        /// <typeparam name="T">Тип списка <paramref name="list"/></typeparam>
        /// <param name="list">Список для перемешивания</param>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _ThisThreadsRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
