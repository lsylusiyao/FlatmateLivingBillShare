using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public Dictionary<string, float> EachMateCost { get; set; } = new();

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
            foreach (var name in names) { EachMateCost.Add(name, 0f); }
        }

        public Dictionary<string, Dictionary<string, float>> Calculate()
        {
            ArgumentNullException.ThrowIfNull(Bills);
            // clear last result
            foreach (var (a, b) in Result.SelectMany(a => a.Value.Select(b => (a, b))))
            {
                a.Value[b.Key] = 0f;
            }
            foreach (var a in EachMateCost) { EachMateCost[a.Key] = 0f; }
            // "shared people" need to pay the payer for the shared price
            foreach (var bill in Bills)
            {
                float amount = bill.Price / bill.SharedPeople.Length;
                foreach (var p in bill.SharedPeople)
                {
                    EachMateCost[p] += amount; // calc the amount for every flatmate
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

        public void Export()
        {
            ArgumentNullException.ThrowIfNull(Bills);
            var recordBills = new List<object>();
            foreach (var bill in Bills)
            {
                recordBills.Add(new
                {
                    bill.Price,
                    bill.Payer,
                    SharedPeople = string.Join("; ", bill.SharedPeople),
                    bill.Item,
                });
            }
            var summary = new List<BillResult>();
            foreach (var a in Result)
            {
                foreach (var b in a.Value)
                {
                    if (b.Value >= 0) summary.Add(new(b.Key, a.Key, b.Value));
                    else summary.Add(new(a.Key, b.Key, -b.Value));
                }
            }
            var eachSummary = new List<object>();
            eachSummary.AddRange(EachMateCost.Select(x => new
            {
                Name = x.Key,
                Amount = x.Value,
            }));

            var sheets = new Dictionary<string, object>
            {
                {"Record", recordBills },
                {"Summary", summary },
                {"EachPersonSum", eachSummary }
            };
            MiniExcel.SaveAs($"Flatmate_bill_{DateTime.Now:yyyy_MM_dd}.xlsx", sheets, excelType:ExcelType.XLSX, overwriteFile:true);
        }

    }
}
