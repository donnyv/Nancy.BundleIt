using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nancy.BundleIt
{
    public class Bundle
    {
        public Bundle(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}
