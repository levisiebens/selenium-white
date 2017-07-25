using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumWhiteAutomationLibrary
{
    public class TestSessionResponse
    {
        public int InactivityTime { get; set; }

        public String InternalKey { get; set; }

        public String Msg { get; set; }

        public String ProxyId { get; set; }

        public String Session { get; set; }

        public Boolean Success { get; set; }
    }
}
