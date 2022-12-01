using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raime.Shop.Api.Controllers.TestControllers
{
    public class TestUsersStore
    {
        public List<TestUser> _users;
        public TestUsersStore()
        {
            _users = new List<TestUser>
            { 
               new TestUser
               { 
                   Id = 1,
                   Name = "Raime",
                   Comments =new List<TestComment> 
                   {
                       new TestComment { Id = 1, Content = "sup bro", }
                   }
               },
               new TestUser { Id = 2, Name = "Domest" },
               new TestUser { Id = 3, Name = "Maveldos" },
            };
        }
    }

    public class TestUser
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IList<TestComment> Comments { get; set; } = new List<TestComment>();
    }
}
