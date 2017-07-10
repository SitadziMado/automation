using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Rpi
{
    public class Order
    {
        public Order()
        {
            Id = -1;
            Material = "Сферический в вакууме.";
            Gas = GasType.None;
            Notes = "Заказ по умолчанию.";
        }

        public Order(
            int id,
            string material,
            GasType gas,
            string notes,
            byte[] program
        )
        {
            Id = id;
            Material = material;
            Gas = gas;
            Notes = notes;
            Prog = program;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Материал: {0}\n\r", Material);
            sb.AppendFormat("Газ: {0}\n\r", gasString[(int)Gas]);
            sb.AppendFormat("Примечание: {0}\n\r", Material);
            return sb.ToString();
        }

        static string[] gasString = new string[] { "Не определен", "Кислород", "Азот", "Н/д", "Воздух" };

        public int Id { get; set; }
        public string Material { get; set; }
        public GasType Gas { get; set; }
        public string Notes { get; set; }
        public byte[] Prog { get; set; }
    }
}
