using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rm_FastKnownScan
{
    public class ScanConfig
    {
        public float KnownScanTime = 2f; 
        public void ApplyModifier()
        {
            FastRedundantScan.SetKnownScanTime(KnownScanTime);
        }
    }
}
