using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AzureTestApp.Controllers
{
    [Route("")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpPost("cpu-request")]
        public void ProcessCpuRequest(int percentage, int milliseconds)
        {
            for (int i = 0; i < 12; i++)
                (new Thread(() => ConsumeCpu(percentage, milliseconds))).Start();
        }

        private static void ConsumeCpu(int percentage, int milliseconds)
        {
            if (percentage < 0 || percentage > 100)
                throw new ArgumentException("percentage");
            var watch = new Stopwatch();
            watch.Start();
            var watchMain = System.Diagnostics.Stopwatch.StartNew();
            while (true)
            {
                if (watch.ElapsedMilliseconds <= percentage) continue;
                Thread.Sleep(100 - percentage);
                watch.Reset();
                watch.Start();

                if (watchMain.ElapsedMilliseconds >= milliseconds) break;
            }
        }

        [HttpPost("memory-request")]
        public void ProcessMemoryRequest(int mb, int milliseconds)
        {
            var ar = new byte[930000, mb];
            var rnd = new Random();

            Parallel.For(0, ar.GetLength(0), (i) =>
            {
                for (int j = 0; j < ar.GetLength(1); j++)
                {
                    ar[i, j] = (byte)rnd.Next(1, 10);
                }
            });

        }

        [HttpPost("memory-clear")]
        public void ClearMemoryRequest()
        {
            GC.Collect();
        }
    }
}
