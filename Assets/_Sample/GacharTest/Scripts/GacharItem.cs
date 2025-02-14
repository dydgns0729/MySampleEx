using System;
using System.Collections.Generic;

namespace MySampleEx
{
    [Serializable]
    public class GacharItem
    {
        public int number { get; set; }
        public string name { get; set; }
        public int rate { get; set; }
    }

    [Serializable]
    public class GacharItems
    {
        public List<GacharItem> gacharItems { get; set; }
    }
}