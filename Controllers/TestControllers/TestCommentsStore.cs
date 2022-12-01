using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raime.Shop.Api.Controllers.TestControllers
{
    public class TestCommentsStore
    {
        public List<TestComment> All;
        public TestCommentsStore()
        {
            All = new List<TestComment>()
            {
                new TestComment
                {
                    Id = 1,
                    Content = "Sup bro",
                    ParentId = 0,
                },
                new TestComment
                {
                    Id=2,
                    Content="Wasup guys",
                    ParentId=0,                    
                }
            };
        }
    }

    public class TestComment
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public int ParentId { get; set; }
        public TestComment Parent { get; set; }
        public IList<TestComment> Replies { get; set; } = new List<TestComment>();
    }
}
