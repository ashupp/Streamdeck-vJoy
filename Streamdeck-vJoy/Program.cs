using BarRaider.SdTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streamdeck_vJoy
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            // Uncomment this line of code to allow for debugging
            //System.Diagnostics.Debugger.Launch();
#endif

            SDWrapper.Run(args);
        }
    }
}
