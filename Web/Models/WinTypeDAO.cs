using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace NHL.Models
{
    public class WinTypeDAO
    {
        public List<WinType> winTypes;

        public WinTypeDAO() 
        {
            winTypes = Broker.Instance().GetConnection().Query<WinType>("SELECT * FROM [dbo].[Win_Types]").ToList();
        }
    }
}
