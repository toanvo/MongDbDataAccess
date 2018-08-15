using MongoDB.Bson;
using System;

namespace MongDbDomain
{
    public class Employee
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public double Salary { get; set; }
    }
}
