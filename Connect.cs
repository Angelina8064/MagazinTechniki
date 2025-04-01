using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagazinTechniki
{
    static public class Connect
    {
        static public string conn =
            $@"host={Properties.Settings.Default.host}; 
            uid={Properties.Settings.Default.uid};
            pwd={Properties.Settings.Default.pwd};
            database={Properties.Settings.Default.db};";
    }
}
