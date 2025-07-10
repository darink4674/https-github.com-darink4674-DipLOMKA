using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIPLOM.Pages
{
    public partial class FlightSearch : Component
    {
        public FlightSearch()
        {
            InitializeComponent();
        }

        public FlightSearch(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
