using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatmateLivingBillShare
{
    /// <summary>
    /// Calculate the bill of each flatmate
    /// </summary>
    internal class BillCalculation
    {
        public List<Bill> Bills { get; set; } = new();

        public Dictionary<string, float> Result { get; set; } = new();

        public BillCalculation() { }

        public void Add(Bill bill)
        {
            Bills.Add(bill);
        }

        public void Remove(int index)
        {
            Bills.RemoveAt(index);
        }

        public Dictionary<string, float> Calculate()
        {
            return null;
        }
    }
}
