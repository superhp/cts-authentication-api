using System;
using Microsoft.Azure.Cosmos.Table;

namespace Db
{
    public class Verification : TableEntity
    {
        public Verification()
        {
        }

        public Verification(Guid guid, string socialEmail, string ctsEmail)
        {
            PartitionKey = "Default";
            RowKey = guid.ToString();
            SocialEmail = socialEmail;
            CtsEmail = ctsEmail;
            Created = DateTime.Now;
        }

        public string SocialEmail { get; set; }
        public string CtsEmail { get; set; }
        public DateTime Created { get; set; }
    }
}