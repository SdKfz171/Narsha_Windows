using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Narsha_Windows.Models
{
    public class BeaconParameter
    {
        public Guid Uuid { get; set; }
        public ushort Minor { get; set; }
        public bool BackgroundFlag { get; set; }
    }
}
