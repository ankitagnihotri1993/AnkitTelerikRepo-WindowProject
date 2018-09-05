using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
namespace LogicLayer
{
    
    public class EmpDataAdapter
    {
        EmpDataAccess dataBase = new EmpDataAccess();
        
        public List<string> GetEmpList()
        {
          DataTable dt=  dataBase.GetDataTable();

            var table = dt.AsEnumerable();



            return new List<string>();

        }
    }
}
