using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatmateLivingBillShare
{
    public class Bill
    {
        public float Price { set; get; }
        public string Payer { set; get; }
        public string[] SharedPeople { set; get; }
        public string Item {  set; get; }
    }

    public class NameCheck
    {
        public NameCheck(string n, bool c)
        {
            Name = n;
            Check = c;
        }
        public string Name { set; get; }
        public bool Check { set; get; }
    }
}
