using MongDbDomain;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbAccess;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMongoDbAccess
{
    class Program
    {
        static void Main(string[] args)
        {
            var mongoDbConnection = ConfigurationManager.ConnectionStrings["TestMongoDB"].ConnectionString;
            var mongoContextFactory = new MongoDbContextFactory(mongoDbConnection);

            var mongoContext = mongoContextFactory.GetMongoContext();
            var employees = mongoContext.GetCollection<Employee>();

            var pulseObjects = mongoContext.GetCollection<PulseDomain>();
            

            var id = ObjectId.GenerateNewId();
            employees.InsertOne(new Employee()
            {
                Id = id,
                Name = "Test",
                Salary = 5000d,
                Birthday = new DateTime(1982, 12, 30)
            });
            
            var employeeList = new List<Employee>();
            for (int i = 0; i < 10; i++)
            {
                employeeList.Add(new Employee()
                {
                    Id = ObjectId.GenerateNewId(),
                    Name = "test " + i,
                    Salary = 1000 * i,
                    Birthday = new DateTime(1983, 1 + i, 10 + i)
                });
            }

            employees.InsertMany(employeeList);
            var employee = employees.Find(Builders<Employee>.Filter.Where(x => x.Id == id)).FirstOrDefault();
            var employeeListNew = employees.Find(Builders<Employee>.Filter.Where(x => x.Name.Contains("test"))).ToList();
            Console.ReadLine();
        }
    }
}
