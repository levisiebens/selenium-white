using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using TestStack.White.UIItems.WindowItems;

namespace SeleniumWhiteLibrary
{
    public class SwitchWindowModel
    {
        public Window Window { get; set; }

        public String Title { get; set; }

        public List<SwitchWindowModel> ChildWindows { get; set; } 
    }
}
