﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.Dtos.Import
{
    [XmlType("Customer")]
    public class ImportCustomersDto
    {
        [XmlElement("name")]
        public string Name { get; set; }
        [XmlElement("birthDay")]
        public DateTime BirthDate { get; set; }
        [XmlElement("isYoungDriver")]
        public bool IsYoungDriver { get; set; }
    }
}
