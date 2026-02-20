using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HowItLooks.State;
public class RoundState
{
    public bool IsStarted { get; set; }
    public int RoundCounter { get; set; } = 1;
}
