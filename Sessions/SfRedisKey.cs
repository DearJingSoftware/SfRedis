using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfRedis.Sessions
{
    public class SfRedisKey
    {
        String _Name;

        public string Name { get => _Name; set => _Name = value; }
    }
}
