using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngleSharp.Parser.Models
{
    public class CocktailListItem
    {
        public string Id { get; internal set; }
        public string Title { get; internal set; }
        public string Link { get; internal set; }
    }
}
