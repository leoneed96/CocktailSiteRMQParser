using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngleSharp.Parser.Models
{
    public class CocktailDetails
    {
        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
        public List<Stuff> Stuffs { get; set; } = new List<Stuff>();
        public List<string> Receipt { get; set; } = new List<string>();
        public string About { get; set; }
        public string RelativeImageUrl { get; set; }
    }

    [DebuggerDisplay("{Title}: {Amount} {Unit}")]
    public class Ingredient
    {
        public string Title { get; set; }
        public string Unit { get; set; }
        public double Amount { get; set; }
    }

    [DebuggerDisplay("{Title}: {Amount} {Unit}")]
    public class Stuff
    {
        public string Title { get; set; }
        public string Unit { get; set; }
        public double Amount { get; set; }
    }
}
