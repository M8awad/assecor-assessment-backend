using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonsManager.Data.Entities
{
    public class Person
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Color { get; set; }
    }
}
