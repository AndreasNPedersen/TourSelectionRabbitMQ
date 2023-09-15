using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourSelectionRabbitMQ
{
    public class Tour
    {
        public Tour() { }
        public string Name { get; set; }
        public string Email { get; set; }
        public TourType Type { get; set; }
        public bool Book { get; set; }
        public bool Cancel { get; set;}
    }

    public enum TourType
    {
        kedelig,fed
    }
}
