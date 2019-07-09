using System;
using Microsoft.Azure.Cosmos.Table;

namespace Db
{
    public class Verification : TableEntity
    {
        public Verification()
        {
        }

        public Verification(string socialEmail, string ctsEmail)
        {
            PartitionKey = "Default";
            RowKey = socialEmail;
            CtsEmail = ctsEmail;
            Created = DateTime.Now;
        }

        public string CtsEmail { get; set; }
        public DateTime Created { get; set; }
    }
}