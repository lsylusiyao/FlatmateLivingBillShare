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
        public IEnumerable<Bill> Bills { get; set; }

        public Dictionary<string, Dictionary<string, float>> Result { get; set; } = new();


        public BillCalculation(string[] names) 
        {
            // comb(n, 2)
            for (int i = 0; i < names.Length - 1; i++)
            {
                var data = new Dictionary<string, float>();
                for (int j = i + 1; j < names.Length; j++)
                {
                    data.Add(names[j], 0f);
                }
                Result.Add(names[i], data);
            }
        }

        public Dictionary<string, Dictionary<string, float>> Calculate()
        {
            ArgumentNullException.ThrowIfNull(Bills);
            // clear last result
            foreach (var (a, b) in Result.SelectMany(a => a.Value.Select(b => (a, b))))
            {
                a.Value[b.Key] = 0f;
            }
            // "shared people" need to pay the payer for the shared price
            foreach (var bill in Bills)
            {
                float amount = bill.Price / bill.SharedPeople.Length;
                foreach (var p in bill.SharedPeople)
                {
                    if (Result.TryGetValue(bill.Payer, out Dictionary<string, float>? value)
                        && value.ContainsKey(p)) // p is in the next-level dict
                    {
                        value[p] += amount;
                    }
                    else if (bill.Payer == p) { continue; }
                    else
                    {
                        Result[p][bill.Payer] -= amount;
                    }
                }
            }
            return Result;
        }

    }
}
