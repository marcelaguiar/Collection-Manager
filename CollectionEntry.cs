using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsManager
{
    class CollectionEntry
    {
        public CollectionEntry(string idInput, string makerInput, string variantInput)
        {
            this.Id = idInput;
            this.Maker = makerInput;
            this.Variant = variantInput;
        }

        public string Id { get; set; }
        public string Maker { get; set; }
        public string Variant { get; set; }
    }
}
