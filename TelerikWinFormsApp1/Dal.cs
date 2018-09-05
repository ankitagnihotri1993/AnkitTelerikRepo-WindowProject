using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TelerikWinFormsApp1
{
    public class Dal
    {
        SqlConnection con = new SqlConnection("data source=DESKTOP-FVRIAA6; database=telerikapp; integrated security=true");
        public List<Employee> GetEmployees()
        {
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter("usp_emp_get", con);
            da.Fill(dt);
      
            List<Employee> emplist = new List<Employee>();
            emplist = (from DataRow dr in dt.Rows
                       select new Employee()
                       {
                           id = Convert.ToInt32(dr["id"]),
                           empName = dr["name"].ToString(),
                           address = dr["address"].ToString(),
                           age = Convert.ToInt32(dr["age"]),
                           gender = dr["gender"].ToString(),
                           empcountry = dr["cname"].ToString(),
                           emphobbies = dr["hobbies"].ToString(),                       
                           empdob = dr["dob"].ToString(),
                           
                          empimages =   (byte[])dr["images"],
                       }).ToList();

            return emplist;
        }


        public List<country> Getcountry()
        {
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter("usp_country_get", con);
            da.Fill(dt);


            List<country> countrylist = new List<country>();
            countrylist = (from DataRow dr in dt.Rows
                           select new country()
                           {
                               cid = Convert.ToInt32(dr["cid"]),
                               cname = dr["cname"].ToString(),
                           }).ToList();
           return countrylist;
        }

    }
}
