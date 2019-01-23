using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class BindableEvent
    {
        public string FROM { get; set; }

        public string TO { get; set; }

        public string DAY { get; set; }

        public string MONTH { get; set; }

        public string TIME { get; set; }

        public string EVENTNAME { get; set; }

        public string TOWN { get; set; }

        public Events Event { get; set; }
    }
}
