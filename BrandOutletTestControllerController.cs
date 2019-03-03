using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BrandOutlet.Models;
using BrandOutlet.Models.EntityModel;
using BrandApi.Models.Custom_Models;

namespace BrandOutlet.Controllers
{
    public class BrandOutletTestControllerController : Controller
    {
        private readonly InfoDBEntities _entities = new InfoDBEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult highChart()
        {
            return View();
        }
        public bool InsertData(List<tblInfoDetail> dataInfo)
        {
            //foreach (var data in dataInfo)
            //{
            //    _entities.tblInfoDetails.Add(data);
            //    _entities.SaveChanges(); 
            //}
            try
            {
                var consString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
                var con = new SqlConnection(consString);

                //var result = new Result();

                con.Open();
                var dto = new DataTable();

                dto.Columns.AddRange(new DataColumn[4]
                {
                    new DataColumn("Name", typeof (string)),
                    new DataColumn("Details", typeof (string)),
                    new DataColumn("Address", typeof (string)),
                    new DataColumn("Number", typeof (string))

                });

                foreach (var itm in dataInfo)
                {


                    var row = dto.NewRow();
                    row["Name"] = itm.Name;
                    row["Address"] = itm.Address;
                    row["Details"] = itm.Details;
                    row["Number"] = itm.Number;

                    dto.Rows.Add(row);

                }
                using (var sqlTransaction = con.BeginTransaction())
                {
                    using (
                        var sqlBulkCopy = new SqlBulkCopy(con, SqlBulkCopyOptions.Default,
                            sqlTransaction))
                    {
                        //Set the database table name  tblShadowIMEIRecord
                        sqlBulkCopy.DestinationTableName = "dbo.tblSqlInfo"; //"dbo.tblIMEIRecord";

                        //[OPTIONAL]: Map the DataTable columns with that of the database table
                        sqlBulkCopy.ColumnMappings.Add("Name", "Name");
                        sqlBulkCopy.ColumnMappings.Add("Address", "Address");
                        sqlBulkCopy.ColumnMappings.Add("Details", "Details");
                        sqlBulkCopy.ColumnMappings.Add("Number", "Number");


                        try
                        {
                            sqlBulkCopy.WriteToServer(dto);
                            sqlTransaction.Commit();
                            con.Close();

                            return true;

                        }
                        catch (Exception exception)
                        {
                            sqlTransaction.Rollback();
                            con.Close();
                            return false;
                        }
                    }
                }


              

            }
            catch
            {
            }
            return true;
        }

    }
}