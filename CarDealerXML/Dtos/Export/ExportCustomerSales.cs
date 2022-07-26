using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using CarDealer.Models;

namespace CarDealer.Dtos.Export
{
    [XmlType("customer")]
    public class ExportCustomerSales
    {
        [XmlAttribute("full-name")]
        public string FullName { get; set; }
        [XmlAttribute("bought-cars")]
        public int BoughtCars { get; set; }
        [XmlAttribute("spent-money")]
        public decimal SpentMoney { get; set; }
        [XmlIgnore]
        public Car Car { get; set; }
    }
}
