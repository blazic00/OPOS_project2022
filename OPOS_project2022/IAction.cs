using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPOS_project2022
{
    internal interface IAction
    {
        public static void Function(SemaphoreSlim semaphore)
        {
            for (int i = 0; i < 1000000; i++)
            {
                semaphore.Wait();
                semaphore.Release();
            }
        }
    }
}
