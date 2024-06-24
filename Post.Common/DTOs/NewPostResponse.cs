using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Common.DTOs
{
    public class NewPostResponse
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
    }
}
