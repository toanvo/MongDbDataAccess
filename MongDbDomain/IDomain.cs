using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongDbDomain
{
    public interface IDomain<IdT> 
    {
        IdT Id { get; set; }
    }
}
