
using System;

using System.Collections.Generic;
using System.Data;
using Livedev;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using LivedevStudio.Models;
using ErpLicense;
using System.Configuration;
using System.Data.SqlClient;

namespace Livedev.Controllers
{

    

    public class SubmoduleController : Controller
    {

       
        LivedevData FtkData = new LivedevData();
        Cookie Cookie = new Cookie();
        CookieErp CookieErp = new CookieErp();
        CacheUser CacheUser = new CacheUser();
        CookieUser CookieUser = new CookieUser();

        string connectionString = WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;


        public string ConnectionString()
        {
            string ServerName = "";
            string DBName = "";
            string UserId = "";
            string Password = "";

            //HttpCookie reqCookies = Request.Cookies["SVConnect"];
            //ServerName = reqCookies["serverName"].ToString();
            //DBName = reqCookies["databaseName"].ToString();
            //UserId = reqCookies["userName"].ToString();
            //Password = reqCookies["password"].ToString();
            ServerName = "59.152.60.149";
            DBName = "Genikos_Erp_edu";
            UserId = "erp_user";
            Password = "erp123";
            string conString = WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
            //connectionString = "Data Source=@Server@;Initial Catalog=@Schema@;Integrated Security=false;User ID=@User@;Password=@Password@;"
            conString = conString.Replace("@Server@", ServerName);
            conString = conString.Replace("@Schema@", DBName);
            conString = conString.Replace("@User@", UserId);
            conString = conString.Replace("@Password@", Password);
            return conString;
        }
        
        
        

        public JsonResult ServerConnection(FormCollection collection)
        {
            string serverName = collection["ServerName"].ToString();
            string databaseName = collection["Database"].ToString();
            string userName = collection["Userid"].ToString();
            string password = collection["Password"].ToString();

            //create a cookie
            HttpCookie ServerConnect = new HttpCookie("SVConnect");
            ServerConnect["serverName"] = serverName;
            ServerConnect["databaseName"] = databaseName;
            ServerConnect["userName"] = userName;
            ServerConnect["password"] = password;
            ServerConnect.Expires.AddDays(30);
            Response.Cookies.Add(ServerConnect);


            //retrieve data from cookie
            //HttpCookie reqCookies = Request.Cookies["SVConnect"];
            //string ServerName = reqCookies["serverName"].ToString();
            //string DBName = reqCookies["databaseName"].ToString();
            //string UserId = reqCookies["userName"].ToString();
            //string Password = reqCookies["password"].ToString();
            

            string ms = "";
            try
            { 
                ms = "Your Connection String has been successfull!";
            }
            catch (Exception ex)
            {
                ms = "Failed to Save! " + ex;
            }
            return Json(new { success = true, message = ms }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Create(FormCollection collection)
        {
            string TableId = collection["TableIds"];
            string PkId = collection["hfpkid"];
            string FkId = collection["hffkid"];
            
            string strQuery = "", dmlFVtype = "", dmltype = "";

            dmlFVtype = "cmd_fv_insert";

            dmltype = "insert";

           
           
            string CompanyId = "1";
            Cookie.SetCookie("CompanyId", CompanyId);
            
            DataRow[] Company = License.CompanyList().Select("CompanyId =" + CompanyId);
            string DataBase = Company[0]["DbName"].ToString();
            string CompanyAlias = Company[0]["CompanyAlias"].ToString();

            CompanyAlias = CompanyAlias + "Live";

            CookieErp.DatabaseName = DataBase;
            CookieErp.CompanyAlias = CompanyAlias;
            CookieErp.CompanyName = "Logic Software Limited";
            


            Cookie.SetCookie("UserId", "1");
            if (CacheUser.Current.GlobalParamList == null || CacheUser.Current.GlobalParamList.Count == 0)
            {
                FtkData.SetGlobalParam();
                ConnectionString();

            }
            CacheUser.Current.GlobalParamList[4] = "1";
            CacheUser.Current.GlobalParamList[5] = "system";
            CacheUser.Current.GlobalParamList[6] = "Md. Omar Faruk";
            CacheUser.Current.GlobalParamList[10] = "1";

            //INSERT INTO utl_dynamic_module_category(categoryid, applicationid, modulecategory, issystem, sl, entry_user_id, entry_by, entry_date, is_active, is_approved, is_deleted, work_follow_position) values(@@categoryid@@, @applicationid@, @modulecategory@, @issystem@, @sl@, @entry_user_id@, @entry_by@, @entry_date@, @is_active@, @is_approved@, @is_deleted@, @work_follow_position@)

            CookieUser.UserAuthenticate = true;
            CookieUser.Password = "iaas!2017!";
           
            strQuery = FtkData.GetSqlCmd(TableId, PkId, FkId, dmlFVtype);
            DataTable tbl = new DataTable();
            tbl = FtkData.GetDynamicTable(TableId, PkId, FkId, dmltype);

            foreach (DataRow col in tbl.Rows)
            {
                string colName = col.ItemArray.GetValue(0).ToString();
                string colProperty= col.ItemArray.GetValue(1).ToString();
                string inputType = col.ItemArray.GetValue(2).ToString();
                string colval = collection[colName];
                string GlobalParamId = col.ItemArray.GetValue(7).ToString();

               if(inputType != "txt" && colval == null)
                {
                    colval ="0";
                }
               if(colName=="entry_date" || colName=="update_date" )
                {
                    colval = "GETDATE()";
                }

               //radio check

               
                if (colProperty == "pk")
                {
                    string pk_value = FtkData.GetNewPkValue(TableId);
                    strQuery = strQuery.Replace("@@" + colName + "@@", "'" + pk_value + "'");
                   
                }
                else if(colName == "entry_date")
                {
                    strQuery = strQuery.Replace("@entry_date@", "GETDATE()");
                }
                else 
                {
                    strQuery = strQuery.Replace("@" + colName + "@", "'" + colval + "'");

                }
              
            }

           
             var s=FtkData.ExecuteNonQueryCommand("1", TableId, FkId, "1", strQuery);

            string ms = "";
            if (s)
            {
                ms = "Saved Successfully!";
            }
            else
            {
                ms = "Failed to Save!";
            }
            return Json(new { success = true, message = ms }, JsonRequestBehavior.AllowGet);
         
           
        }


        


        public ActionResult UpdateSubmodule(FormCollection collection)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            if (collection["issystem"] == null || collection["issystem"]=="")
            {
                collection["issystem"] = "0";
            }

            string UpdateSubmodule = "UPDATE utl_dynamic_module_category SET  applicationid = '" + collection["applicationid"] + "', " +

                "modulecategory = '" + collection["modulecategory"] + "', issystem = '" + collection["issystem"] + "', sl = '" + collection["sl"] + "'," +
                " update_by = '" + collection["update_by"] + "', update_date = " + collection["update_date"] + ", is_active = '" + collection["is_active"] + "' WHERE categoryid = '" + collection["categoryid"] + "'";
            SqlCommand command = new SqlCommand(UpdateSubmodule, connection);
            connection.Open();
            int effectfield = command.ExecuteNonQuery();

            connection.Close();
            string ms = "";
            if (effectfield > 0)
            {
                ms = "Updated Successfully!";
            }
            else
            {
                ms = "Failed to Update!";
            }


            return Json(new { success = true, message = ms }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateTable(FormCollection collection)
        {
          
            collection["isreadonly"] = collection["isreadonly"] == null || collection["isreadonly"] == ""? "0": collection["isreadonly"];
            collection["isworkfollow"] = collection["isworkfollow"] == null || collection["isworkfollow"] == ""? "0" : collection["isworkfollow"];
            collection["objecttype"] = collection["objecttype"] == null || collection["objecttype"] == "" ? "0" : collection["objecttype"];
            collection["isautotoggle"] = collection["isautotoggle"] == null || collection["isautotoggle"] == "" ? "0" : collection["isautotoggle"];  
            collection["rptprocforworksheet"] = collection["rptprocforworksheet"] == null || collection["rptprocforworksheet"] == "" ? "0" : collection["rptprocforworksheet"];
            collection["showonreport"] = collection["showonreport"] == null || collection["showonreport"] == "" ? "0" : collection["showonreport"];
            

            SqlConnection connection = new SqlConnection(connectionString);
            string UpdateTable = "UPDATE utl_dynamic_table SET moduleid = '" + collection["moduleid"] + "', parenttableid = '" + collection["parenttableid"] + "'," +
                  " moduletablecaption = '" + collection["moduletablecaption"] + "', tablecaption = '" + collection["tablecaption"] + "', templatetype = '" + collection["templatetype"] + "'," +
                  " tablename = '" + collection["tablename"] + "', existingtableid = '" + collection["existingtableid"] + "', foreigntableid = '" + collection["foreigntableid"] + "', " +
                  "externaldatafilter = '" + collection["externaldatafilter"] + "', externaldatacaption = '" + collection["externaldatacaption"] + "', isexternaldatashow = '" + collection["isexternaldatashow"] + "'," +
                  " modulewidth = '" + collection["modulewidth"] + "',moduleheight = '" + collection["moduleheight"] + "', positionexp = '" + collection["positionexp"] + "', rowposition = '" + collection["rowposition"] + "'," +
                  " colposition = '" + collection["colposition"] + "', isreadonly = '" + collection["isreadonly"] + "', isworkfollow = '" + collection["isworkfollow"] + "'," +
                  " formwidth = '" + collection["formwidth"] + "', procedurename = '" + collection["procedurename"] + "', objecttype = '" + collection["objecttype"] + "'," +
                  " procfireontrans = '" + collection["procfireontrans"] + "', procedurecaption = '" + collection["procedurecaption"] + "'," +
                  " refreshtype = '" + collection["refreshtype"] + "', isautotoggle = '" + collection["isautotoggle"] + "'," +
                  " reportprocedurename = '" + collection["reportprocedurename"] + "', rptprocforworksheet = '" + collection["rptprocforworksheet"] + "', reportfilename = '" + collection["reportfilename"] + "'," +
                  " procedurevalidation = '" + collection["procedurevalidation"] + "', procfooterview = '" + collection["procfooterview"] + "', procfooterdml = '" + collection["procfooterdml"] + "'," +
                  " showonreport = '" + collection["showonreport"] + "', slno = '" + collection["slno"] + "', update_by = '" + collection["update_by"] + "', update_date = " + collection["update_date"] + "," +
                  " is_active = '" + collection["is_active"] + "' WHERE tableid = '" + collection["tableid"] + "'";
            SqlCommand command = new SqlCommand(UpdateTable, connection);
            connection.Open();
            int effectfield = command.ExecuteNonQuery();

            connection.Close();
            string ms = "";
            if (effectfield > 0)
            {
                ms = "Updated Successfully!";
            }
            else
            {
                ms = "Failed to Update!";
            }

           
            return Json(new { success = true, message = ms }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdatePage(FormCollection collection)
        {
            SqlConnection connection = new SqlConnection(connectionString);

           
            collection["isdrilldownreport"] = collection["isdrilldownreport"] == null || collection["isdrilldownreport"] == ""? "0" : collection["isdrilldownreport"];
            collection["isactive"] = collection["isactive"] == null || collection["isactive"] == ""? "0": collection["isactive"];    
            collection["isnewcopy"] = collection["isnewcopy"] == null || collection["isnewcopy"] == ""? "0": collection["isnewcopy"];
            

            string UpdatePage = "UPDATE utl_dynamic_module SET categoryid = '"+collection["categoryid"]+"', applicationid = '"+collection["applicationid"]+"', " +
                "moduletype = '"+collection["moduletype"]+"', modulename = '"+collection["modulename"]+"', modulecaption = '"+collection["modulecaption"]+"', " +
                "reporttitle = '"+collection["reporttitle"]+ "', IsDrilldownReport = '" + collection["isdrilldownreport"]+"', existingmoduleid = '"+collection["existingmoduleid"]+"'," +
                " isnewcopy = '"+collection["isnewcopy"]+"', imagepath = '"+collection["imagepath"]+"', slno = '"+collection["slno"]+"', isactive = '"+collection["isactive"]+"'," +
                " update_by = '"+collection["update_by"]+"', update_date = "+collection["update_date"]+", is_active = '"+collection["is_active"]+"' WHERE moduleid = '"+collection["moduleid"]+"'";
            SqlCommand command = new SqlCommand(UpdatePage, connection);
            connection.Open();
            int effectfield = command.ExecuteNonQuery();

            connection.Close();
            string ms = "";
            if (effectfield > 0)
            {
                ms = "Updated Successfully!";
            }
            else
            {
                ms = "Failed to Update!";
            }


            return Json(new { success = true, message = ms }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CopyExsitingTable(int TableId, int ExsitionTableId)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            string ColumnExist = "select top 1 1 as Exist from UTL_DYNAMIC_TABLE_COLUMN where TableId= " + TableId + "";
            SqlCommand com = new SqlCommand(ColumnExist, connection);
            connection.Open();
            var IsColumnExist = com.ExecuteScalar() == null ? 0 : com.ExecuteScalar();
            connection.Close();

            string ms = "";
            if (Convert.ToInt32(IsColumnExist) == 0)
            {
                SqlCommand dCmd = new SqlCommand("Tmp_Copy_Existing_Table", connection);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@TableId", TableId));
                dCmd.Parameters.Add(new SqlParameter("@ExtingTableId", ExsitionTableId));

                connection.Open();
                int s = dCmd.ExecuteNonQuery();
                //object s = dCmd.ExecuteScalar();
                connection.Close();
                ms = "Column saved successfully!";
            }
            else
            {
                ms = "Columns already exist in this table. Do you want to create new columns? First, you delete existing columns in this table.";
            }

            return Json(new { success = true, message = ms }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteExistingCol(int TableId)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string DeleteTable = "Delete from Utl_Dynamic_Table_Column where TableId = " + TableId + "";
            SqlCommand command = new SqlCommand(DeleteTable, connection);
            connection.Open();
            int effectfield = command.ExecuteNonQuery();

            connection.Close();
            string ms = "";
            if (effectfield > 0)
            {
                ms = "Deleted Successfully!";
            }
            else
            {
                ms = "Failed to Delete!";
            }
            return Json(new { success = true, message = ms }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreatePanel(FormCollection collection)
        {
            var tableid = collection["tableid"];
            

            string TableId = "21";

            string PkId = "0", FkId = "0";
            //int totalRow = Convert.ToInt32(collection["dbfieldhf"]);

            string CompanyId = "1";
            Cookie.SetCookie("CompanyId", CompanyId);

            DataRow[] Company = License.CompanyList().Select("CompanyId =" + CompanyId);
            string DataBase = Company[0]["DbName"].ToString();
            string CompanyAlias = Company[0]["CompanyAlias"].ToString();

            CompanyAlias = CompanyAlias + "Live";

            CookieErp.DatabaseName = DataBase;
            CookieErp.CompanyAlias = CompanyAlias;
            CookieErp.CompanyName = "Logic Software Limited";


            Cookie.SetCookie("UserId", "1");
            if (CacheUser.Current.GlobalParamList == null || CacheUser.Current.GlobalParamList.Count == 0)
            {
                FtkData.SetGlobalParam();

            }
            CacheUser.Current.GlobalParamList[4] = "1";
            CacheUser.Current.GlobalParamList[5] = "system";
            CacheUser.Current.GlobalParamList[6] = "Md. Omar Faruk";
            CacheUser.Current.GlobalParamList[10] = "1";

            //INSERT INTO utl_dynamic_module_category(categoryid, applicationid, modulecategory, issystem, sl, entry_user_id, entry_by, entry_date, is_active, is_approved, is_deleted, work_follow_position) values(@@categoryid@@, @applicationid@, @modulecategory@, @issystem@, @sl@, @entry_user_id@, @entry_by@, @entry_date@, @is_active@, @is_approved@, @is_deleted@, @work_follow_position@)

            CookieUser.UserAuthenticate = true;
            CookieUser.Password = "iaas!2017!";

            string str;
            
            int isvertically = collection["IsVertically"] == ""?0: Convert.ToInt32(collection["IsVertically"]);
            int pnlColNo = collection["PnlColNo"] == ""? 0: Convert.ToInt32(collection["PnlColNo"]);

            string pk_value = FtkData.GetNewPkValue(TableId);
            string ms = "";
            if (pnlColNo > 0 && collection["RowPosition"].ToString() != "" && collection["ColumnPosition"].ToString() != "")
            {
                str = "INSERT INTO Utl_Dynamic_Table_Panel(PanelId, TableId, PanelName,PanelCaption,RowPosition, ColPosition,PnlColNo,IsVertically, SlNo, Entry_User_Id,Entry_By,Entry_Date," +
                "Is_Approved,Is_Deleted,Is_Cancel,Work_Follow_Position) values('" + pk_value + "','" + tableid + "','" + collection["PanelName"] + "','" + collection["PanelCaption"] + "'," +
                "'" + collection["RowPosition"] + "','" + collection["ColumnPosition"] + "', '" + pnlColNo + "','" + isvertically + "','" + collection["SlNo"] + "','" + collection["entry_user_id"] + "'," +
                "'" + collection["entry_by"] + "', " + collection["entry_date"] + ", '" + collection["is_approved"] + "', '"
                + collection["is_deleted"] + "', '" + collection["is_cancel"] + "', '" + collection["work_follow_position"] + "')";

                var s = FtkData.ExecuteNonQueryCommand("1", TableId, "0", "0", str);
                if (s)
                {
                    ms = "Saved Successfully!";
                }
                else
                {
                    ms = "Failed to Save!";
                }
            }
            else
            {
                ms = "Data was not Save because,";
                ms = pnlColNo < 1 ? ms + " 'Row Column No.'must be greater than 0 (zero)." : ms;
                ms = collection["RowPosition"] == "" ? ms + " 'Row Position' is not empty." : ms;
                ms = collection["ColumnPosition"] == "" ? ms + " 'Column Position' is not empty." : ms;
            }  
            return Json(new { success = true, message = ms }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult CreateTrigger(FormCollection collection)
        {
            var tableid = collection["tableid"];
            //string TableId = "20";
            string PkId = "0", FkId = "0";
            
            string CompanyId = "1";
            Cookie.SetCookie("CompanyId", CompanyId);

            DataRow[] Company = License.CompanyList().Select("CompanyId =" + CompanyId);
            string DataBase = Company[0]["DbName"].ToString();
            string CompanyAlias = Company[0]["CompanyAlias"].ToString();

            CompanyAlias = CompanyAlias + "Live";

            CookieErp.DatabaseName = DataBase;
            CookieErp.CompanyAlias = CompanyAlias;
            CookieErp.CompanyName = "Logic Software Limited";


            Cookie.SetCookie("UserId", "1");
            if (CacheUser.Current.GlobalParamList == null || CacheUser.Current.GlobalParamList.Count == 0)
            {
                FtkData.SetGlobalParam();

            }
            CacheUser.Current.GlobalParamList[4] = "1";
            CacheUser.Current.GlobalParamList[5] = "system";
            CacheUser.Current.GlobalParamList[6] = "Md. Omar Faruk";
            CacheUser.Current.GlobalParamList[10] = "1";

            
            CookieUser.UserAuthenticate = true;
            CookieUser.Password = "iaas!2017!";

            SqlConnection connection = new SqlConnection(connectionString);
            string trgStr = "SELECT max(TriggerId) as TriggerId FROM Utl_Dynamic_Table_Trigger";
            SqlCommand command = new SqlCommand(trgStr, connection);
            connection.Open();
            int triggerId = command.ExecuteScalar() == DBNull.Value ? 0 : Convert.ToInt32(command.ExecuteScalar());
            connection.Close();

            //string pk_value = FtkData.GetNewPkValue(TableId);
            string ms = ""; string str;           

            int isAutoExecute = collection["IsAutoExecute"] == "" ? 0 : Convert.ToInt32(collection["IsAutoExecute"]);
            int triggerType = Convert.ToInt32(collection["TriggerTypeId"]);

            if (triggerType > 0 && collection["TriggerName"].ToString() != "" && collection["TriggerCaption"].ToString() != "")
            {
                str = "INSERT INTO Utl_Dynamic_Table_Trigger(TriggerId, TableId, ModuleId,TriggerTypeId,TriggerName, TriggerCaption,IsAutoExecute,SlNo, Entry_User_Id,Entry_By,Entry_Date," +
                "Is_Approved,Is_Deleted,Is_Cancel,Work_Follow_Position) values('" + (triggerId+1) + "','" + tableid + "','" + collection["moduleid"] + "','" + triggerType + "'," +
                "'" + collection["TriggerName"] + "','" + collection["TriggerCaption"] + "', '" + isAutoExecute + "','" + collection["SlNo"] + "','" + collection["entry_user_id"] + "'," +
                "'" + collection["entry_by"] + "', " + collection["entry_date"] + ", '" + collection["is_approved"] + "', '"
                + collection["is_deleted"] + "', '" + collection["is_cancel"] + "', '" + collection["work_follow_position"] + "')";

                var s = FtkData.ExecuteNonQueryCommand("1", "0", "0", "0", str);
                ms=s==true? "Saved Successfully!" : "Failed to Save!";
            }
            else
            {
                ms = "Data was not Save because,";
                ms = triggerType < 1 ? ms + "  You must select 'Trigger Type'." : ms;
                ms = collection["TriggerName"] == "" ? ms + " 'Source' cannot be empty." : ms;
                ms = collection["TriggerCaption"] == "" ? ms + " 'Trigger Caption' cannot be empty." : ms;
            }

            return Json(new { success = true, message = ms }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateFunction(FormCollection collection)
        {
            var tableid = collection["tableid"];


            string TableId = "6";

            string PkId = "0", FkId = "0";
            //int totalRow = Convert.ToInt32(collection["dbfieldhf"]);

            string CompanyId = "1";
            Cookie.SetCookie("CompanyId", CompanyId);

            DataRow[] Company = License.CompanyList().Select("CompanyId =" + CompanyId);
            string DataBase = Company[0]["DbName"].ToString();
            string CompanyAlias = Company[0]["CompanyAlias"].ToString();

            CompanyAlias = CompanyAlias + "Live";

            CookieErp.DatabaseName = DataBase;
            CookieErp.CompanyAlias = CompanyAlias;
            CookieErp.CompanyName = "Logic Software Limited";


            Cookie.SetCookie("UserId", "1");
            if (CacheUser.Current.GlobalParamList == null || CacheUser.Current.GlobalParamList.Count == 0)
            {
                FtkData.SetGlobalParam();

            }
            CacheUser.Current.GlobalParamList[4] = "1";
            CacheUser.Current.GlobalParamList[5] = "system";
            CacheUser.Current.GlobalParamList[6] = "Md. Omar Faruk";
            CacheUser.Current.GlobalParamList[10] = "1";

            //INSERT INTO utl_dynamic_module_category(categoryid, applicationid, modulecategory, issystem, sl, entry_user_id, entry_by, entry_date, is_active, is_approved, is_deleted, work_follow_position) values(@@categoryid@@, @applicationid@, @modulecategory@, @issystem@, @sl@, @entry_user_id@, @entry_by@, @entry_date@, @is_active@, @is_approved@, @is_deleted@, @work_follow_position@)

            CookieUser.UserAuthenticate = true;
            CookieUser.Password = "iaas!2017!";

            string str;
            string ms = "";

            string pk_value = FtkData.GetNewPkValue(TableId);
            decimal slno = collection["SlNo"] == "" ? 0 : Convert.ToDecimal(collection["SlNo"]);

            if (collection["evaluatevalue"] != "" && collection["effectcolumn"] != "")
            {
                str = "INSERT INTO Utl_Dynamic_Table_Function(TableFunctionId,ColumnId, TableId,ModuleId,ColumnName, EvaluateValue,EffectColumn, SlNo, Is_Active, " +
                "Entry_User_Id,Entry_By,Entry_Date,Is_Approved,Is_Deleted,Is_Cancel,Work_Follow_Position) values(" + pk_value + "," + collection["columnid"] +
                "," + tableid + ","+ collection["moduleid"] + ",'" + collection["columnname"] + "','" + collection["evaluatevalue"] + "','" + collection["effectcolumn"] + "'," +
                slno + ",'" + collection["is_active"] + "','" + collection["entry_user_id"] + "','" + collection["entry_by"] + "', " + collection["entry_date"] +
                ", '" + collection["is_approved"] + "', '"+ collection["is_deleted"] + "', '" + collection["is_cancel"] + "', '" + collection["work_follow_position"] + "')";

                var s = FtkData.ExecuteNonQueryCommand("1", TableId, "0", "0", str);
                ms = s == true ? "Save Successfully!":"Failed to Save!";
            }
            else
            {
                ms = collection["evaluatevalue"] == "" && collection["effectcolumn"] == "" ? "The 'Evaluate Expression' and 'Effect Field' will not be empty"
                : collection["evaluatevalue"] == "" && collection["effectcolumn"] != "" ? "The 'Evaluate Expression' will not be empty"
                : collection["evaluatevalue"] != "" && collection["effectcolumn"] == "" ? "The 'Effect Field' will not be empty" : "";
            }
            

            return Json(new { success = true, message = ms }, JsonRequestBehavior.AllowGet);

        }




        public JsonResult SaveSetColumn(FormCollection collection)
        {
            var tableid = collection["dbtableid"];
            string TableId = "3";
            //string PkId = collection["hfpkid"];
            //string FkId = collection["hffkid"];
            string PkId = "0", FkId = "0";
            int totalRow = Convert.ToInt32(collection["dbfieldhf"]);
            string str = "", dmlFVtype = "", dmltype = "";

            //dmlFVtype = "cmd_fv_insert";
            dmlFVtype = "cmd_fv_update";

            dmltype = "insert";

            //if (PkId != "0")
            //{
            //    dmlFVtype = "cmd_fv_update";
            //    //dmltype = "update";
            //}


            string CompanyId = "1";
            Cookie.SetCookie("CompanyId", CompanyId);

            DataRow[] Company = License.CompanyList().Select("CompanyId =" + CompanyId);
            string DataBase = Company[0]["DbName"].ToString();
            string CompanyAlias = Company[0]["CompanyAlias"].ToString();

            CompanyAlias = CompanyAlias + "Live";

            CookieErp.DatabaseName = DataBase;
            CookieErp.CompanyAlias = CompanyAlias;
            CookieErp.CompanyName = "Logic Software Limited";


            Cookie.SetCookie("UserId", "1");
            if (CacheUser.Current.GlobalParamList == null || CacheUser.Current.GlobalParamList.Count == 0)
            {
                FtkData.SetGlobalParam();

            }
            CacheUser.Current.GlobalParamList[4] = "1";
            CacheUser.Current.GlobalParamList[5] = "system";
            CacheUser.Current.GlobalParamList[6] = "Md. Omar Faruk";
            CacheUser.Current.GlobalParamList[10] = "1";

            //INSERT INTO utl_dynamic_module_category(categoryid, applicationid, modulecategory, issystem, sl, entry_user_id, entry_by, entry_date, is_active, is_approved, is_deleted, work_follow_position) values(@@categoryid@@, @applicationid@, @modulecategory@, @issystem@, @sl@, @entry_user_id@, @entry_by@, @entry_date@, @is_active@, @is_approved@, @is_deleted@, @work_follow_position@)

            CookieUser.UserAuthenticate = true;
            CookieUser.Password = "iaas!2017!";

            string ms = "";
            bool s = false;
            for (int i = 0; i < totalRow; i++)
            {
                var ColId = collection["columnid[" + i + "]"];
                str = "";

                //var colna = collection["columnname[" + i + "]"];
                //var seecoln = collection["selectcolumnname[" + i + "]"];

                if (collection["columnname[" + i + "]"] == collection["selectcolumnname[" + i + "]"].ToLower() && collection["columndisplayname[" + i + "]"] != "" && collection["listcation[" + i + "]"] != "")
                {
                    collection["isshow[" + i + "]"] = collection["isshow[" + i + "]"] == null || collection["isshow[" + i + "]"] == "" ? "0" : collection["isshow[" + i + "]"];
                    collection["is_active[" + i + "]"] = collection["is_active[" + i + "]"] == null || collection["is_active[" + i + "]"] == "" ? "0" : collection["is_active[" + i + "]"];

                    str = "UPDATE utl_dynamic_table_column SET  tableid='" + tableid + "', ColumnName= '" + collection["columnname[" + i + "]"] + "', selectcolumnname= LOWER('"
                        + collection["selectcolumnname[" + i + "]"] + "')," + " DataType = '" + collection["datatype[" + i + "]"] + "', foreignkeyname='"
                        + collection["foreignkeyname[" + i + "]"] + "', controltype= '" + collection["controltype[" + i + "]"] + "', columnproperty= '"
                        + collection["columnproperty[" + i + "]"] + "', columndisplayname= '" + collection["columndisplayname[" + i + "]"] + "',Is_Active= '"
                        + collection["is_active[" + i + "]"] + "', ListCaption = '" + collection["listcation[" + i + "]"] + "'   WHERE columnid ='" + ColId + "'";

                    s = FtkData.ExecuteNonQueryCommand("1", TableId, "0", "1", str);
                    if (s)
                    {
                        ms = ms + "Saved Successfully!";
                    }
                    else
                    {
                        ms = ms + "Failed to Save! ";
                    }
                }
                if(collection["columnname[" + i + "]"] != collection["selectcolumnname[" + i + "]"].ToLower())
                {                   
                    ms = ms+" 'DML Column("+ collection["columnname[" + i + "]"] + ")' and 'View Column("+ collection["selectcolumnname[" + i + "]"] + ")' are not same.";                
                }               
                if (collection["columndisplayname[" + i + "]"] == "")
                {
                    ms = ms + " 'Form Caption(" + collection["columnname[" + i + "]"] + ")' is empty.";
                }
                if(collection["listcation[" + i + "]"] == "")
                {
                    ms = ms + " 'View Caption(" + collection["columnname[" + i + "]"] + ")' is empty.";
                }
  
            }

            string msg = ms.Replace("Saved Successfully!", "");
            msg = msg == "" ? "Saved Successfully!" : msg;

            return Json(new { success = true, message = msg }, JsonRequestBehavior.AllowGet);
        }
      
         public JsonResult UpdatePanelSl(FormCollection collection)
         {
            
            string TableId = "3";
            
            string FkId = "0";
            string PkId = "0";
            string str = "", dmlFVtype = "", dmltype = "";

            int totalRow = Convert.ToInt32(collection["panelrowhf"]);

            string CompanyId = "1";
            Cookie.SetCookie("CompanyId", CompanyId);

            DataRow[] Company = License.CompanyList().Select("CompanyId =" + CompanyId);
            string DataBase = Company[0]["DbName"].ToString();
            string CompanyAlias = Company[0]["CompanyAlias"].ToString();

            CompanyAlias = CompanyAlias + "Live";

            CookieErp.DatabaseName = DataBase;
            CookieErp.CompanyAlias = CompanyAlias;
            CookieErp.CompanyName = "Logic Software Limited";


            Cookie.SetCookie("UserId", "1");
            if (CacheUser.Current.GlobalParamList == null || CacheUser.Current.GlobalParamList.Count == 0)
            {
                FtkData.SetGlobalParam();

            }
            CacheUser.Current.GlobalParamList[4] = "1";
            CacheUser.Current.GlobalParamList[5] = "system";
            CacheUser.Current.GlobalParamList[6] = "Md. Omar Faruk";
            CacheUser.Current.GlobalParamList[10] = "1";


            CookieUser.UserAuthenticate = true;
            CookieUser.Password = "iaas!2017!";
            bool s = false;
            string tblid = "0";
            string panelid = "0";

            for (int i = 0; i < totalRow; i++)
            {
                var ColId = collection["columnid[" + i + "]"];
                tblid = collection["tableid[" + i + "]"]; ;
                panelid = collection["panelid[" + i + "]"];

                str = "";
                str = "UPDATE utl_dynamic_table_column SET PanelId = '" + collection["panelid[" + i + "]"] + "', PnlColSl='" + collection["PnlColSl[" + i + "]"] + "', PnlColMerge='" + collection["PnlColMerge[" + i + "]"] + "' WHERE columnid ='" + ColId + "'";


                s = FtkData.ExecuteNonQueryCommand("1", TableId, FkId, "1", str);

            }


            List<PanelColSlno> columnSlnos = new List<PanelColSlno>();
            SqlConnection connection = new SqlConnection(connectionString);
            string TableColumn = "Select ColumnId, ROW_NUMBER() over(order by PnlColSl) as RowIndex, Isnull(PnlColMerge,0) as PnlColMerge from Utl_Dynamic_Table_Column where TableId = "+ tblid + " and PanelId = "+ panelid + " order by PnlColSl ";
            SqlCommand command3 = new SqlCommand(TableColumn, connection);
            connection.Open();
            SqlDataReader reader = command3.ExecuteReader();

            while (reader.Read())
            {
                long columnId = Convert.ToInt64(reader["ColumnId"]);
                decimal pnlColsln = Convert.ToDecimal(reader["RowIndex"].ToString());
                int pnlColMerge = Convert.ToInt32(reader["PnlColMerge"]);

                PanelColSlno columnsl = new PanelColSlno(columnId, pnlColsln, pnlColMerge);
                columnSlnos.Add(columnsl);
            }
            reader.Close();
            connection.Close();

            var pnlSlno = columnSlnos.Select(c => c.PnlColsln).ToList();
            for (int i = 1; i <= pnlSlno.Count(); i++)
            {
                long columnId = columnSlnos.Where(sl => sl.PnlColsln == i).Select(ci => ci.ColumnId).FirstOrDefault();
                int pnlColMg = columnSlnos.Where(sl => sl.PnlColsln == i).Select(ci => ci.PnlColMerge).FirstOrDefault();
                decimal pnlcolsl = columnSlnos.Where(sl => sl.PnlColsln == i).Select(p => p.PnlColsln).FirstOrDefault();
                
                string columnSlUpdate = "update Utl_Dynamic_Table_Column set PanelId = "+ panelid + ", PnlColSl = " + pnlcolsl + ", PnlColMerge = " + pnlColMg + " where ColumnId = " + columnId;
                s = FtkData.ExecuteNonQueryCommand("1", TableId, FkId, "1", columnSlUpdate);

            }


            string ms = "";
            if (s)
            {
                ms = "Saved Successfully!";
            }
            else
            {
                ms = "Failed to Save!";
            }
            return Json(new { success = true, message = ms }, JsonRequestBehavior.AllowGet);


         }
        public JsonResult UpdateLayouts(FormCollection collection)
        {
            string LayoutTableid = collection["tableid"];
            string TableId = "3";
            
            int totalRow = Convert.ToInt32(collection["layouthf"]);
            string FkId = "0";
            string PkId = "0";
            string str = "", dmlFVtype = "", dmltype = "";

            string CompanyId = "1";
            Cookie.SetCookie("CompanyId", CompanyId);

            DataRow[] Company = License.CompanyList().Select("CompanyId =" + CompanyId);
            string DataBase = Company[0]["DbName"].ToString();
            string CompanyAlias = Company[0]["CompanyAlias"].ToString();

            CompanyAlias = CompanyAlias + "Live";

            CookieErp.DatabaseName = DataBase;
            CookieErp.CompanyAlias = CompanyAlias;
            CookieErp.CompanyName = "Logic Software Limited";


            Cookie.SetCookie("UserId", "1");
            if (CacheUser.Current.GlobalParamList == null || CacheUser.Current.GlobalParamList.Count == 0)
            {
                FtkData.SetGlobalParam();

            }
            CacheUser.Current.GlobalParamList[4] = "1";
            CacheUser.Current.GlobalParamList[5] = "system";
            CacheUser.Current.GlobalParamList[6] = "Md. Omar Faruk";
            CacheUser.Current.GlobalParamList[10] = "1";

            //INSERT INTO utl_dynamic_module_category(categoryid, applicationid, modulecategory, issystem, sl, entry_user_id, entry_by, entry_date, is_active, is_approved, is_deleted, work_follow_position) values(@@categoryid@@, @applicationid@, @modulecategory@, @issystem@, @sl@, @entry_user_id@, @entry_by@, @entry_date@, @is_active@, @is_approved@, @is_deleted@, @work_follow_position@)

            CookieUser.UserAuthenticate = true;
            CookieUser.Password = "iaas!2017!";

            bool s = false;
            string ms = "";

            for (int i = 0; i < totalRow; i++)
            {
                var ColId = collection["columnid[" + i + "]"];

                int colWidth = collection["columnwidth[" + i + "]"] == "" ? 10 : Convert.ToInt32(collection["columnwidth[" + i + "]"]);
                if(colWidth > 9 )
                {
                    str = "";
                    str = "UPDATE utl_dynamic_table_column SET ColumnDisplayName = '" + collection["columndisplayname[" + i + "]"] + "', panelid='" + 
                        collection["panelid[" + i + "]"] + "', rowposition='" + collection["rowposition[" + i + "]"] + "', colposition='" + 
                        collection["colposition[" + i + "]"] + "', columnwidth='" + colWidth + "', slno= '" + collection["slno[" + i + "]"] + "', reportcolumn= '" + 
                        collection["reportcolumn[" + i + "]"] + "'   WHERE columnid ='" + ColId + "'";

                    s = FtkData.ExecuteNonQueryCommand("1", TableId, FkId, "1", str);

                    ms = s == true ? ms + "Saved Successfully!" : ms + " Failed to Save!";
                }
                if(colWidth <= 9)
                {
                    ms = ms + " 'Field Width(" + collection["columndisplayname[" + i + "]"] + ")' must be greater than 9.";
                }

            }
            List<ColumnSlno> columnSlnos = new List<ColumnSlno>();
            SqlConnection connection = new SqlConnection(connectionString);
            string TableColumn = "select ColumnId, ROW_NUMBER() over(order by SlNo) as RowIndex, lower(ColumnName) as colName,50 as ColSize, isnull(ReportColumn,0) as ReportColumn from Utl_Dynamic_Table_Column where TableId = " + LayoutTableid + " order by SlNo";
            SqlCommand command3 = new SqlCommand(TableColumn, connection);
            connection.Open();
            SqlDataReader reader = command3.ExecuteReader();

            while (reader.Read())
            {
                long columnId = Convert.ToInt64(reader["ColumnId"]);
                decimal slNo = Convert.ToDecimal( reader["RowIndex"].ToString());
                string colName = reader["colName"].ToString();
                int colSize = Convert.ToInt32(reader["ColSize"]);
                int reportSlno = Convert.ToInt32(reader["ReportColumn"]);

                ColumnSlno columnsl = new ColumnSlno(columnId, slNo, colName, colSize, reportSlno);
                columnSlnos.Add(columnsl);
            }
            reader.Close();
            connection.Close();

            var slno = columnSlnos.Select(c => c.SlNo).ToList();
            for(int i = 1; i <= slno.Count(); i++)
            {
                long columnId = columnSlnos.Where(sl => sl.SlNo == i).Select(ci => ci.ColumnId).FirstOrDefault();
                int reportSl = columnSlnos.Where(sl => sl.SlNo == i).Select(ci => ci.ReportSlno).FirstOrDefault();
                reportSl = reportSl == 0 ? 0 : i;
                string columnSlUpdate = "update Utl_Dynamic_Table_Column set SlNo = " + i + ", ReportColumn = "+ reportSl +" where ColumnId = " + columnId;
                s = FtkData.ExecuteNonQueryCommand("1", TableId, FkId, "1", columnSlUpdate);
                
            }


            string msg = ms.Replace("Saved Successfully!", "");
            msg = msg == "" ? "Saved Successfully!" : msg;

            return Json(new { success = true, message = msg }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult UpdateVariable(FormCollection collection)
        {
            //string TableId = collection["TableIds"];
            string TableId = "3";
            //string PkId = collection["hfpkid"];
            //string FkId = collection["hffkid"];
            string FkId = "0";
            string PkId = "0";
            string str = "", dmlFVtype = "", dmltype = "";

            //dmlFVtype = "cmd_fv_insert";
            dmlFVtype = "cmd_fv_update";
            int totalRow = Convert.ToInt32(collection["variablehf"]);
            dmltype = "update";

            //if (PkId != "0")
            //{
            //    dmlFVtype = "cmd_fv_update";
            //    //dmltype = "update";
            //}


            string CompanyId = "1";
            Cookie.SetCookie("CompanyId", CompanyId);

            DataRow[] Company = License.CompanyList().Select("CompanyId =" + CompanyId);
            string DataBase = Company[0]["DbName"].ToString();
            string CompanyAlias = Company[0]["CompanyAlias"].ToString();

            CompanyAlias = CompanyAlias + "Live";

            CookieErp.DatabaseName = DataBase;
            CookieErp.CompanyAlias = CompanyAlias;
            CookieErp.CompanyName = "Logic Software Limited";


            Cookie.SetCookie("UserId", "1");
            if (CacheUser.Current.GlobalParamList == null || CacheUser.Current.GlobalParamList.Count == 0)
            {
                FtkData.SetGlobalParam();

            }
            CacheUser.Current.GlobalParamList[4] = "1";
            CacheUser.Current.GlobalParamList[5] = "system";
            CacheUser.Current.GlobalParamList[6] = "Md. Omar Faruk";
            CacheUser.Current.GlobalParamList[10] = "1";

            CookieUser.UserAuthenticate = true;
            CookieUser.Password = "iaas!2017!";

            bool s = false;
            string ms = "";

            for (int i = 0; i < totalRow; i++)
            {
                var ColId = collection["columnid[" + i + "]"];

                int globalParamId = Convert.ToInt32(collection["globalparamid[" + i + "]"]);
                int globalParamValue = Convert.ToInt32(collection["globalparamvalue[" + i + "]"]);


                if ((globalParamId == 0 && globalParamValue == 0) || (globalParamId > 0 && globalParamValue > 0))
                {
                    str = "";
                    str = "UPDATE utl_dynamic_table_column SET ColumnDisplayName = '" + collection["columndisplayname[" + i + "]"] + "', globalparamid='" +
                        globalParamId + "', globalparamvalue='" + globalParamValue + "', defaultvalue='" +
                        collection["defaultvalue[" + i + "]"] + "', filterby='" + collection["filterby[" + i + "]"] + "'  WHERE columnid ='" + ColId + "'";

                    s = FtkData.ExecuteNonQueryCommand("1", TableId, FkId, "1", str);
                    ms = s == true ? ms + "Saved Successfully!" : ms + " Failed to Save!";
                }
                if(globalParamId > 0 && globalParamValue <= 0)
                {
                    ms = ms + " When 'Global Variable("+ collection["columndisplayname[" + i + "]"] + ")' is set. Then you must set the 'Variable Method'.";
                }
                if(globalParamId <= 0 && globalParamValue > 0)
                {
                    ms = ms + " When 'Global Variable(" + collection["columndisplayname[" + i + "]"] + ")' is not set. Then you do not need to set the 'Variable Method'.";
                }

                
            }
            string msg = ms.Replace("Saved Successfully!", "");
            msg = msg == "" ? "Saved Successfully!" : msg;

            return Json(new { success = true, message = msg }, JsonRequestBehavior.AllowGet);
        }



        public JsonResult UpdateDML(FormCollection collection)
        {
            //string TableId = collection["TableIds"];
            string TableId = "3";
            string PkId = collection["hfpkid"];
            string FkId = collection["hffkid"];
            int totalRow = Convert.ToInt32(collection["dmlhf"]);
            string str = "", dmlFVtype = "", dmltype = "";

            //dmlFVtype = "cmd_fv_insert";
            dmlFVtype = "cmd_fv_update";

            dmltype = "update";

            //if (PkId != "0")
            //{
            //    dmlFVtype = "cmd_fv_update";
            //    //dmltype = "update";
            //}


            string CompanyId = "1";
            Cookie.SetCookie("CompanyId", CompanyId);

            DataRow[] Company = License.CompanyList().Select("CompanyId =" + CompanyId);
            string DataBase = Company[0]["DbName"].ToString();
            string CompanyAlias = Company[0]["CompanyAlias"].ToString();

            CompanyAlias = CompanyAlias + "Live";

            CookieErp.DatabaseName = DataBase;
            CookieErp.CompanyAlias = CompanyAlias;
            CookieErp.CompanyName = "Logic Software Limited";


            Cookie.SetCookie("UserId", "1");
            if (CacheUser.Current.GlobalParamList == null || CacheUser.Current.GlobalParamList.Count == 0)
            {
                FtkData.SetGlobalParam();

            }
            CacheUser.Current.GlobalParamList[4] = "1";
            CacheUser.Current.GlobalParamList[5] = "system";
            CacheUser.Current.GlobalParamList[6] = "Md. Omar Faruk";
            CacheUser.Current.GlobalParamList[10] = "1";

            //INSERT INTO utl_dynamic_module_category(categoryid, applicationid, modulecategory, issystem, sl, entry_user_id, entry_by, entry_date, is_active, is_approved, is_deleted, work_follow_position) values(@@categoryid@@, @applicationid@, @modulecategory@, @issystem@, @sl@, @entry_user_id@, @entry_by@, @entry_date@, @is_active@, @is_approved@, @is_deleted@, @work_follow_position@)

            CookieUser.UserAuthenticate = true;
            CookieUser.Password = "iaas!2017!";

            

            bool s = false; 
            for (int i = 0; i < totalRow; i++)
            {

                if (collection["isfvinsert[" + i + "]"] == null || collection["isfvinsert[" + i + "]"] == "")
                {
                    collection["isfvinsert[" + i + "]"] = "0";
                }
                if (collection["isfvupdate[" + i + "]"] == null || collection["isfvupdate[" + i + "]"] == "")
                {
                    collection["isfvupdate[" + i + "]"] = "0";
                }
                if (collection["isfvforceedit[" + i + "]"] == null || collection["isfvforceedit[" + i + "]"] == "")
                {
                    collection["isfvforceedit[" + i + "]"] = "0";
                }
                if (collection["ismoduleselect[" + i + "]"] == null || collection["ismoduleselect[" + i + "]"] == "")
                {
                    collection["ismoduleselect[" + i + "]"] = "0";
                }
                if (collection["isforeigntable[" + i + "]"] == null || collection["isforeigntable[" + i + "]"] == "")
                {
                    collection["isforeigntable[" + i + "]"] = "0";
                }

                str = "";
                var ColId = collection["columnid[" + i + "]"];
                str = "UPDATE utl_dynamic_table_column SET ColumnDisplayName = '" + collection["columndisplayname[" + i + "]"] + "', isfvinsert='" + collection["isfvinsert[" + i + "]"] + "', isfvupdate='" + collection["isfvupdate[" + i + "]"] + "'," +
                    " isfvforceedit='" + collection["isfvforceedit[" + i + "]"] + "', ismoduleselect='" + collection["ismoduleselect[" + i + "]"] + "'," +
                    " isforeigntable= '" + collection["isforeigntable[" + i + "]"] + "'   WHERE columnid ='"+ ColId + "'";


                 s = FtkData.ExecuteNonQueryCommand("1", TableId, FkId, "1", str);

            }
            string ms = "";
            if (s)
            {
                ms = "Saved Successfully!";
            }
            else
            {
                ms = "Failed to Save!";
            }
            return Json(new { success = true, message = ms }, JsonRequestBehavior.AllowGet);
          

        }

        public JsonResult UpdateHyperlink(FormCollection collection)
        {
            //string TableId = collection["TableIds"];
            string TableId = "3";
            //string PkId = collection["hfpkid"];
            //string FkId = collection["hffkid"];
            string FkId = "0";
            string PkId = "0";
            string str = "", dmlFVtype = "", dmltype = "";
            int totalRow = Convert.ToInt32(collection["hyperlinkhf"]);
            //dmlFVtype = "cmd_fv_insert";
            dmlFVtype = "cmd_fv_update";

            dmltype = "update";

            //if (PkId != "0")
            //{
            //    dmlFVtype = "cmd_fv_update";
            //    //dmltype = "update";
            //}


            string CompanyId = "1";
            Cookie.SetCookie("CompanyId", CompanyId);

            DataRow[] Company = License.CompanyList().Select("CompanyId =" + CompanyId);
            string DataBase = Company[0]["DbName"].ToString();
            string CompanyAlias = Company[0]["CompanyAlias"].ToString();

            CompanyAlias = CompanyAlias + "Live";

            CookieErp.DatabaseName = DataBase;
            CookieErp.CompanyAlias = CompanyAlias;
            CookieErp.CompanyName = "Logic Software Limited";


            Cookie.SetCookie("UserId", "1");
            if (CacheUser.Current.GlobalParamList == null || CacheUser.Current.GlobalParamList.Count == 0)
            {
                FtkData.SetGlobalParam();

            }
            CacheUser.Current.GlobalParamList[4] = "1";
            CacheUser.Current.GlobalParamList[5] = "system";
            CacheUser.Current.GlobalParamList[6] = "Md. Omar Faruk";
            CacheUser.Current.GlobalParamList[10] = "1";

            //INSERT INTO utl_dynamic_module_category(categoryid, applicationid, modulecategory, issystem, sl, entry_user_id, entry_by, entry_date, is_active, is_approved, is_deleted, work_follow_position) values(@@categoryid@@, @applicationid@, @modulecategory@, @issystem@, @sl@, @entry_user_id@, @entry_by@, @entry_date@, @is_active@, @is_approved@, @is_deleted@, @work_follow_position@)

            CookieUser.UserAuthenticate = true;
            CookieUser.Password = "iaas!2017!";
            string ms = "";
            bool s = false;
            for (int i = 0; i < totalRow; i++)
            {
                var ColId = collection["columnid[" + i + "]"];

                int linktype = Convert.ToInt32(collection["linktype[" + i + "]"]);
                int linkpageid = Convert.ToInt32(collection["linkpageid[" + i + "]"]);

                if ((linktype == 0 && linkpageid == 0) || (linktype > 0 && linkpageid > 0))
                {
                    str = "";
                    str = "UPDATE utl_dynamic_table_column SET ColumnDisplayName = '" + collection["columndisplayname[" + i + "]"] + "', linktype='" +
                        linktype + "', linkpageid='" + linkpageid + "', linktableid='" +
                        collection["linktableid[" + i + "]"] + "', linkcolumnname='" + collection["linkcolumnname[" + i + "]"] + "'  WHERE columnid ='" + ColId + "'";

                    s = FtkData.ExecuteNonQueryCommand("1", TableId, FkId, "1", str);
                    ms = s == true ? ms + "Saved Successfully!" : ms + " Failed to Save!";
                }
                if (linktype > 0 && linkpageid <= 0)
                {
                    ms = ms + " When 'Hyperlink Roll(" + collection["columndisplayname[" + i + "]"] + ")' is set. Then you must set the 'Link Page'.";
                }
                if (linktype <= 0 && linkpageid > 0)
                {
                    ms = ms + " When 'Hyperlink Roll(" + collection["columndisplayname[" + i + "]"] + ")' is not set. Then you do not need to set the 'Link Page'.";
                }
            }

            string msg = ms.Replace("Saved Successfully!", "");
            msg = msg == "" ? "Saved Successfully!" : msg;

            return Json(new { success = true, message = msg }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateRole(FormCollection collection)
        {
            
            string TableId = "3";
            string FkId = "0";
            string PkId = "0";
            string str = "";
            int totalRow = Convert.ToInt32(collection["rolehf"]);
            string CompanyId = "1";
            Cookie.SetCookie("CompanyId", CompanyId);

            DataRow[] Company = License.CompanyList().Select("CompanyId =" + CompanyId);
            string DataBase = Company[0]["DbName"].ToString();
            string CompanyAlias = Company[0]["CompanyAlias"].ToString();

            CompanyAlias = CompanyAlias + "Live";

            CookieErp.DatabaseName = DataBase;
            CookieErp.CompanyAlias = CompanyAlias;
            CookieErp.CompanyName = "Logic Software Limited";


            Cookie.SetCookie("UserId", "1");
            if (CacheUser.Current.GlobalParamList == null || CacheUser.Current.GlobalParamList.Count == 0)
            {
                FtkData.SetGlobalParam();

            }
            CacheUser.Current.GlobalParamList[4] = "1";
            CacheUser.Current.GlobalParamList[5] = "system";
            CacheUser.Current.GlobalParamList[6] = "Md. Omar Faruk";
            CacheUser.Current.GlobalParamList[10] = "1";

            //INSERT INTO utl_dynamic_module_category(categoryid, applicationid, modulecategory, issystem, sl, entry_user_id, entry_by, entry_date, is_active, is_approved, is_deleted, work_follow_position) values(@@categoryid@@, @applicationid@, @modulecategory@, @issystem@, @sl@, @entry_user_id@, @entry_by@, @entry_date@, @is_active@, @is_approved@, @is_deleted@, @work_follow_position@)

            CookieUser.UserAuthenticate = true;
            CookieUser.Password = "iaas!2017!";

            bool s = false;
            for (int i = 0; i <totalRow; i++)
            {
                var ColId = collection["columnid[" + i + "]"];
                str = "";
                str = "UPDATE utl_dynamic_table_column SET ColumnDisplayName = '" + collection["columndisplayname[" + i + "]"] + "', user_privilege = '" + 
                    collection["user_privilege[" + i + "]"] + "', transpolicyid='" + collection["transpolicyid[" + i + "]"] + "'  WHERE columnid ='"+ ColId + "'";


               s = FtkData.ExecuteNonQueryCommand("1", TableId, FkId, "1", str);

            }

            string ms = "";
            if (s)
            {
                ms = "Saved Successfully!";
            }
            else
            {
                ms = "Failed to Save!";
            }
            return Json(new { success = true, message = ms }, JsonRequestBehavior.AllowGet);


        }

        public JsonResult UpdateFieldValidation(FormCollection collection)
        {
            //string TableId = collection["TableIds"];
            string TableId = "3";
            string PkId = collection["hfpkid"];
            string FkId = collection["hffkid"];
            int totalRow = Convert.ToInt32(collection["fieldvalidationhf"]);
            string str = "", dmlFVtype = "", dmltype = "";

            //dmlFVtype = "cmd_fv_insert";
            dmlFVtype = "cmd_fv_update";

            dmltype = "update";

            //if (PkId != "0")
            //{
            //    dmlFVtype = "cmd_fv_update";
            //    //dmltype = "update";
            //}


            string CompanyId = "1";
            Cookie.SetCookie("CompanyId", CompanyId);

            DataRow[] Company = License.CompanyList().Select("CompanyId =" + CompanyId);
            string DataBase = Company[0]["DbName"].ToString();
            string CompanyAlias = Company[0]["CompanyAlias"].ToString();

            CompanyAlias = CompanyAlias + "Live";

            CookieErp.DatabaseName = DataBase;
            CookieErp.CompanyAlias = CompanyAlias;
            CookieErp.CompanyName = "Logic Software Limited";


            Cookie.SetCookie("UserId", "1");
            if (CacheUser.Current.GlobalParamList == null || CacheUser.Current.GlobalParamList.Count == 0)
            {
                FtkData.SetGlobalParam();

            }
            CacheUser.Current.GlobalParamList[4] = "1";
            CacheUser.Current.GlobalParamList[5] = "system";
            CacheUser.Current.GlobalParamList[6] = "Md. Omar Faruk";
            CacheUser.Current.GlobalParamList[10] = "1";

            //INSERT INTO utl_dynamic_module_category(categoryid, applicationid, modulecategory, issystem, sl, entry_user_id, entry_by, entry_date, is_active, is_approved, is_deleted, work_follow_position) values(@@categoryid@@, @applicationid@, @modulecategory@, @issystem@, @sl@, @entry_user_id@, @entry_by@, @entry_date@, @is_active@, @is_approved@, @is_deleted@, @work_follow_position@)

            CookieUser.UserAuthenticate = true;
            CookieUser.Password = "iaas!2017!";
            string ms = "";
            bool s = false;

            for (int i = 0; i < totalRow; i++)
            {
                str = "";
                var ColId = collection["columnid[" + i + "]"];
                int opertorid = Convert.ToInt32(collection["operatorid[" + i + "]"]);
                string valueExpression = collection["valueexpression[" + i + "]"].ToString();

                if ((opertorid == 0 && valueExpression == "") || (opertorid > 0 && valueExpression != ""))
                {
                    str = "UPDATE utl_dynamic_table_column SET ColumnDisplayName = '" + collection["columndisplayname[" + i + "]"] + "', operatorid='" +
                    opertorid + "', valueexpression='" + valueExpression + "', errormessage='" + collection["errormessage[" + i + "]"] + "' WHERE columnid ='" + ColId + "'";

                    s = FtkData.ExecuteNonQueryCommand("1", TableId, FkId, "1", str);
                    ms = s == true ? ms + "Saved Successfully!" : ms + " Failed to Save!";
                }
                if (opertorid > 0 && valueExpression == "")
                {
                    ms = ms + " When 'Operator(" + collection["columndisplayname[" + i + "]"] + ")' is set. Then you must set the 'Compare to Value'.";
                }
                if (opertorid <= 0 && valueExpression != "")
                {
                    ms = ms + " When 'Operator(" + collection["columndisplayname[" + i + "]"] + ")' is not set. Then you do not need to set the 'Compare to Value'.";
                }
            }

            string msg = ms.Replace("Saved Successfully!", "");
            msg = msg == "" ? "Saved Successfully!" : msg;

            return Json(new { success = true, message = msg }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateDdlSource(FormCollection collection)
        {
            //string TableId = collection["TableIds"];
            string TableId = "3";
            string PkId = collection["hfpkid"];
            string FkId = collection["hffkid"];
            int totalRow = Convert.ToInt32(collection["ddlhf"]);
            string str = "", dmlFVtype = "", dmltype = "";

            //dmlFVtype = "cmd_fv_insert";
            dmlFVtype = "cmd_fv_update";

            dmltype = "update";

            //if (PkId != "0")
            //{
            //    dmlFVtype = "cmd_fv_update";
            //    //dmltype = "update";
            //}


            string CompanyId = "1";
            Cookie.SetCookie("CompanyId", CompanyId);

            DataRow[] Company = License.CompanyList().Select("CompanyId =" + CompanyId);
            string DataBase = Company[0]["DbName"].ToString();
            string CompanyAlias = Company[0]["CompanyAlias"].ToString();

            CompanyAlias = CompanyAlias + "Live";

            CookieErp.DatabaseName = DataBase;
            CookieErp.CompanyAlias = CompanyAlias;
            CookieErp.CompanyName = "Logic Software Limited";


            Cookie.SetCookie("UserId", "1");
            if (CacheUser.Current.GlobalParamList == null || CacheUser.Current.GlobalParamList.Count == 0)
            {
                FtkData.SetGlobalParam();

            }
            CacheUser.Current.GlobalParamList[4] = "1";
            CacheUser.Current.GlobalParamList[5] = "system";
            CacheUser.Current.GlobalParamList[6] = "Md. Omar Faruk";
            CacheUser.Current.GlobalParamList[10] = "1";

            //INSERT INTO utl_dynamic_module_category(categoryid, applicationid, modulecategory, issystem, sl, entry_user_id, entry_by, entry_date, is_active, is_approved, is_deleted, work_follow_position) values(@@categoryid@@, @applicationid@, @modulecategory@, @issystem@, @sl@, @entry_user_id@, @entry_by@, @entry_date@, @is_active@, @is_approved@, @is_deleted@, @work_follow_position@)

            CookieUser.UserAuthenticate = true;
            CookieUser.Password = "iaas!2017!";

            string ms = "";
            bool s = false;
            for (int i = 0; i < totalRow; i++)
            {
                str = "";
                int ddlDataSource = Convert.ToInt32(collection["ddldatasource[" + i + "]"]);
                int lovtableid = Convert.ToInt32(collection["lovtableid[" + i + "]"]);
                var ColId = collection["columnid[" + i + "]"];

                if(ddlDataSource == 1 && collection["tablename[" + i + "]"] != "" && collection["columnvalue[" + i + "]"] != "" && collection["columntext[" + i + "]"] != "")
                {
                    str = "UPDATE utl_dynamic_table_column SET ColumnDisplayName = '" + collection["columndisplayname[" + i + "]"] + "', parentcolumn='" + collection["parentcolumn[" + i + "]"] + "', ddldatasource='" + collection["ddldatasource[" + i + "]"] + "'," +
                    " lovtableid='" + collection["lovtableid[" + i + "]"] + "',tablename='" + collection["tablename[" + i + "]"] + "',columnvalue='" + collection["columnvalue[" + i + "]"] + "'," +
                    "columntext='" + collection["columntext[" + i + "]"] + "',columntextexp='" + collection["columntextexp[" + i + "]"] + "',whereexpression='" + collection["whereexpression[" + i + "]"] + "'," +
                    "orderbyexpression='" + collection["whereexpression[" + i + "]"] + "'   WHERE columnid ='" + ColId + "'";

                    s = FtkData.ExecuteNonQueryCommand("1", TableId, FkId, "1", str);
                    ms = s == true ? ms + "Saved Successfully!" : ms + "Failed to Save!";
                }
                else if (ddlDataSource == 2 && lovtableid > 0 && collection["tablename[" + i + "]"] != "" && collection["columnvalue[" + i + "]"] != "" && collection["columntext[" + i + "]"] != "")
                {
                    str = "UPDATE utl_dynamic_table_column SET ColumnDisplayName = '" + collection["columndisplayname[" + i + "]"] + "', parentcolumn='" + collection["parentcolumn[" + i + "]"] + "', ddldatasource='" + collection["ddldatasource[" + i + "]"] + "'," +
                    " lovtableid='" + collection["lovtableid[" + i + "]"] + "',tablename='" + collection["tablename[" + i + "]"] + "',columnvalue='" + collection["columnvalue[" + i + "]"] + "'," +
                    "columntext='" + collection["columntext[" + i + "]"] + "',columntextexp='" + collection["columntextexp[" + i + "]"] + "',whereexpression='" + collection["whereexpression[" + i + "]"] + "'," +
                    "orderbyexpression='" + collection["whereexpression[" + i + "]"] + "'   WHERE columnid ='" + ColId + "'";

                    s = FtkData.ExecuteNonQueryCommand("1", TableId, FkId, "1", str);
                    ms = s == true ? ms + "Saved Successfully!" : ms + "Failed to Save!";
                }

                ms = (ddlDataSource == 1 || ddlDataSource == 2) && collection["tablename[" + i + "]"] == "" ? ms + "You must select a 'Dropdown Table("+ collection["columndisplayname[" + i + "]"] + ")'. " : ms + "";
                ms = (ddlDataSource == 1 || ddlDataSource == 2) && collection["tablename[" + i + "]"] != "" && collection["columnvalue[" + i + "]"] == "" && collection["columntext[" + i + "]"] != "" ? ms + "You must select a 'Value Column("+ collection["columndisplayname[" + i + "]"] + ")'. " : ms + "";
                ms = (ddlDataSource == 1 || ddlDataSource == 2) && collection["tablename[" + i + "]"] != "" && collection["columnvalue[" + i + "]"] != "" && collection["columntext[" + i + "]"] == "" ? ms + "You must select a 'Text Column(" + collection["columndisplayname[" + i + "]"] + ")'. " : ms + "";
                ms = (ddlDataSource == 1 || ddlDataSource == 2) && collection["tablename[" + i + "]"] != "" && collection["columnvalue[" + i + "]"] == "" && collection["columntext[" + i + "]"] == "" ? ms + "You must select a 'Value Column(" + collection["columndisplayname[" + i + "]"] + ")' and 'Text Column(" + collection["columndisplayname[" + i + "]"] + ")'. " : ms + "";
                ms = ddlDataSource == 2 && lovtableid <= 0 ? ms + "When you select Data Source 'LOV Table' Then you must select a 'Popup LOV Table(" + collection["columndisplayname[" + i + "]"] + ")'. " : ms + "";


            }

            string msg = ms.Replace("Saved Successfully!", "");
            msg = msg == "" ? "Saved Successfully!" : msg;

            return Json(new { success = true, message = msg }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateFunction(FormCollection collection)
        {
            //string TableId = collection["TableIds"];
            string TableId = "3";
            string PkId = collection["hfpkid"];
            string FkId = collection["hffkid"];
            int totalRow = Convert.ToInt32(collection["functionhf"]);
            string str = "", dmlFVtype = "", dmltype = "";

            //dmlFVtype = "cmd_fv_insert";
            dmlFVtype = "cmd_fv_update";

            dmltype = "update";

            //if (PkId != "0")
            //{
            //    dmlFVtype = "cmd_fv_update";
            //    //dmltype = "update";
            //}


            string CompanyId = "1";
            Cookie.SetCookie("CompanyId", CompanyId);

            DataRow[] Company = License.CompanyList().Select("CompanyId =" + CompanyId);
            string DataBase = Company[0]["DbName"].ToString();
            string CompanyAlias = Company[0]["CompanyAlias"].ToString();

            CompanyAlias = CompanyAlias + "Live";

            CookieErp.DatabaseName = DataBase;
            CookieErp.CompanyAlias = CompanyAlias;
            CookieErp.CompanyName = "Logic Software Limited";


            Cookie.SetCookie("UserId", "1");
            if (CacheUser.Current.GlobalParamList == null || CacheUser.Current.GlobalParamList.Count == 0)
            {
                FtkData.SetGlobalParam();

            }
            CacheUser.Current.GlobalParamList[4] = "1";
            CacheUser.Current.GlobalParamList[5] = "system";
            CacheUser.Current.GlobalParamList[6] = "Md. Omar Faruk";
            CacheUser.Current.GlobalParamList[10] = "1";

            //INSERT INTO utl_dynamic_module_category(categoryid, applicationid, modulecategory, issystem, sl, entry_user_id, entry_by, entry_date, is_active, is_approved, is_deleted, work_follow_position) values(@@categoryid@@, @applicationid@, @modulecategory@, @issystem@, @sl@, @entry_user_id@, @entry_by@, @entry_date@, @is_active@, @is_approved@, @is_deleted@, @work_follow_position@)

            CookieUser.UserAuthenticate = true;
            CookieUser.Password = "iaas!2017!";
            bool s = false;
            for (int i = 0; i < totalRow; i++)
            {

                if (collection["isdatashort[" + i + "]"] ==null || collection["isdatashort[" + i + "]"]=="")
                {
                    collection["isdatashort[" + i + "]"] = "0";
                }
                if (collection["isdescending[" + i + "]"] == null || collection["isdescending[" + i + "]"] == "")
                {
                    collection["isdescending[" + i + "]"] = "0";
                }

                str = "";
                var ColId = collection["columnid[" + i + "]"];
                str = "UPDATE utl_dynamic_table_column SET ColumnDisplayName = '" + collection["columndisplayname[" + i + "]"] + "',  EvaluateValue='"+ collection["evaluatevalue[" + i + "]"] + "', EffectColumn ='" + collection["effectcolumn[" + i + "]"] + "' WHERE columnid ='" + ColId + "'";


                 s = FtkData.ExecuteNonQueryCommand("1", TableId, FkId, "1", str);

            }

            string ms = "";
            if (s)
            {
                ms = "Saved Successfully!";
            }
            else
            {
                ms = "Failed to Save!";
            }
            return Json(new { success = true, message = ms }, JsonRequestBehavior.AllowGet);


        }
        public JsonResult UpdateSorting(FormCollection collection)
        {
            //string TableId = collection["TableIds"];
            string TableId = "3";
            string PkId = collection["hfpkid"];
            string FkId = collection["hffkid"];
            int totalRow = Convert.ToInt32(collection["sortinghf"]);
            string str = "", dmlFVtype = "", dmltype = "";

            //dmlFVtype = "cmd_fv_insert";
            dmlFVtype = "cmd_fv_update";

            dmltype = "update";

            //if (PkId != "0")
            //{
            //    dmlFVtype = "cmd_fv_update";
            //    //dmltype = "update";
            //}


            string CompanyId = "1";
            Cookie.SetCookie("CompanyId", CompanyId);

            DataRow[] Company = License.CompanyList().Select("CompanyId =" + CompanyId);
            string DataBase = Company[0]["DbName"].ToString();
            string CompanyAlias = Company[0]["CompanyAlias"].ToString();

            CompanyAlias = CompanyAlias + "Live";

            CookieErp.DatabaseName = DataBase;
            CookieErp.CompanyAlias = CompanyAlias;
            CookieErp.CompanyName = "Logic Software Limited";


            Cookie.SetCookie("UserId", "1");
            if (CacheUser.Current.GlobalParamList == null || CacheUser.Current.GlobalParamList.Count == 0)
            {
                FtkData.SetGlobalParam();

            }
            CacheUser.Current.GlobalParamList[4] = "1";
            CacheUser.Current.GlobalParamList[5] = "system";
            CacheUser.Current.GlobalParamList[6] = "Md. Omar Faruk";
            CacheUser.Current.GlobalParamList[10] = "1";

            //INSERT INTO utl_dynamic_module_category(categoryid, applicationid, modulecategory, issystem, sl, entry_user_id, entry_by, entry_date, is_active, is_approved, is_deleted, work_follow_position) values(@@categoryid@@, @applicationid@, @modulecategory@, @issystem@, @sl@, @entry_user_id@, @entry_by@, @entry_date@, @is_active@, @is_approved@, @is_deleted@, @work_follow_position@)

            CookieUser.UserAuthenticate = true;
            CookieUser.Password = "iaas!2017!";
            bool s = false;
            for (int i = 0; i < totalRow; i++)
            {
                str = "";
                var ColId = collection["columnid[" + i + "]"];
                str = "UPDATE utl_dynamic_table_column SET ColumnDisplayName = '" + collection["columndisplayname[" + i + "]"] + "', IsDataShort='" + collection["isdatashort[" + i + "]"] + "', isdescending='" +
                    collection["isdescending[" + i + "]"] + "', aggregatefunction='" + collection["aggregatefunction[" + i + "]"] + "'  WHERE columnid ='" + ColId + "'";


                s = FtkData.ExecuteNonQueryCommand("1", TableId, FkId, "1", str);

            }

            string ms = "";
            if (s)
            {
                ms = "Saved Successfully!";
            }
            else
            {
                ms = "Failed to Save!";
            }
            return Json(new { success = true, message = ms }, JsonRequestBehavior.AllowGet);


        }
        public JsonResult UpdateExpression(FormCollection collection)
        {
            //string TableId = collection["TableIds"];
            string TableId = "3";
            string PkId = collection["hfpkid"];
            string FkId = collection["hffkid"];
            int totalRow = Convert.ToInt32(collection["expressionhf"]);
            string str = "", dmlFVtype = "", dmltype = "";

            //dmlFVtype = "cmd_fv_insert";
            dmlFVtype = "cmd_fv_update";

            dmltype = "update";

            //if (PkId != "0")
            //{
            //    dmlFVtype = "cmd_fv_update";
            //    //dmltype = "update";
            //}


            string CompanyId = "1";
            Cookie.SetCookie("CompanyId", CompanyId);

            DataRow[] Company = License.CompanyList().Select("CompanyId =" + CompanyId);
            string DataBase = Company[0]["DbName"].ToString();
            string CompanyAlias = Company[0]["CompanyAlias"].ToString();

            CompanyAlias = CompanyAlias + "Live";

            CookieErp.DatabaseName = DataBase;
            CookieErp.CompanyAlias = CompanyAlias;
            CookieErp.CompanyName = "Logic Software Limited";


            Cookie.SetCookie("UserId", "1");
            if (CacheUser.Current.GlobalParamList == null || CacheUser.Current.GlobalParamList.Count == 0)
            {
                FtkData.SetGlobalParam();

            }
            CacheUser.Current.GlobalParamList[4] = "1";
            CacheUser.Current.GlobalParamList[5] = "system";
            CacheUser.Current.GlobalParamList[6] = "Md. Omar Faruk";
            CacheUser.Current.GlobalParamList[10] = "1";

            //INSERT INTO utl_dynamic_module_category(categoryid, applicationid, modulecategory, issystem, sl, entry_user_id, entry_by, entry_date, is_active, is_approved, is_deleted, work_follow_position) values(@@categoryid@@, @applicationid@, @modulecategory@, @issystem@, @sl@, @entry_user_id@, @entry_by@, @entry_date@, @is_active@, @is_approved@, @is_deleted@, @work_follow_position@)

            CookieUser.UserAuthenticate = true;
            CookieUser.Password = "iaas!2017!";

            bool s = false;
            for (int i = 0; i < totalRow; i++)
            {
                var ColId = collection["columnid[" + i + "]"];
                str = "";
                str = "UPDATE utl_dynamic_table_column SET ColumnDisplayName = '" + collection["columndisplayname[" + i + "]"] + "', coldescription='" + 
                    collection["coldescription[" + i + "]"] + "', sqlexpression='" + collection["sqlexpression[" + i + "]"] + "' WHERE columnid ='"+ColId+"'";


                 s = FtkData.ExecuteNonQueryCommand("1", TableId, FkId, "1", str);

            }

            string ms = "";
            if (s)
            {
                ms = "Saved Successfully!";
            }
            else
            {
                ms = "Failed to Save!";
            }
            return Json(new { success = true, message = ms }, JsonRequestBehavior.AllowGet);


        }

        public JsonResult Creatennn(FormCollection collection)
        {

            var tableid = collection["tableid"];


            string TableId = "9";

            string PkId = "0", FkId = "0";
            //int totalRow = Convert.ToInt32(collection["dbfieldhf"]);

            string CompanyId = "1";
            Cookie.SetCookie("CompanyId", CompanyId);

            DataRow[] Company = License.CompanyList().Select("CompanyId =" + CompanyId);
            string DataBase = Company[0]["DbName"].ToString();
            string CompanyAlias = Company[0]["CompanyAlias"].ToString();

            CompanyAlias = CompanyAlias + "Live";

            CookieErp.DatabaseName = DataBase;
            CookieErp.CompanyAlias = CompanyAlias;
            CookieErp.CompanyName = "Logic Software Limited";


            Cookie.SetCookie("UserId", "1");
            if (CacheUser.Current.GlobalParamList == null || CacheUser.Current.GlobalParamList.Count == 0)
            {
                FtkData.SetGlobalParam();

            }
            CacheUser.Current.GlobalParamList[4] = "1";
            CacheUser.Current.GlobalParamList[5] = "system";
            CacheUser.Current.GlobalParamList[6] = "Md. Omar Faruk";
            CacheUser.Current.GlobalParamList[10] = "1";

            //INSERT INTO utl_dynamic_module_category(categoryid, applicationid, modulecategory, issystem, sl, entry_user_id, entry_by, entry_date, is_active, is_approved, is_deleted, work_follow_position) values(@@categoryid@@, @applicationid@, @modulecategory@, @issystem@, @sl@, @entry_user_id@, @entry_by@, @entry_date@, @is_active@, @is_approved@, @is_deleted@, @work_follow_position@)

            CookieUser.UserAuthenticate = true;
            CookieUser.Password = "iaas!2017!";

            string str;
            string ms = "";

            string pk_value = FtkData.GetNewPkValue(TableId);

            
            if(collection["formattype"] != ""&& collection["format"] != "")
            {
                str = "insert into Utl_Dynamic_Autocode_Property(PropertyId,ColumnId,TableId,ModuleId,FormatType,[Format],Expression,ColumnName,SlNo,IsActive," +
                "Entry_User_Id,Entry_By,Entry_Date,Is_Approved,Is_Deleted,Is_Cancel,Work_Follow_Position) values('" + pk_value + "','" + collection["columnid"] + "','" + tableid + "','" + collection["moduleid"] + "','" +
                collection["formattype"] + "','" + collection["format"] + "','" + collection["expression"] + "','" + collection["columnname"] + "'," +
                "'" + collection["slno"] + "','" + collection["isactive"] + "','" + collection["entry_user_id"] + "'," +
                "'" + collection["entry_by"] + "', " + collection["entry_date"] + ", '" + collection["is_approved"] + "', '"
                + collection["is_deleted"] + "', '" + collection["is_cancel"] + "', '" + collection["work_follow_position"] + "')";

                var s = FtkData.ExecuteNonQueryCommand("1", TableId, "0", "0", str);
                ms = s == true ? "Saved Successfully!" : "Failed to Save!";
            }
            ms = collection["formattype"] == "" && collection["format"] == "" ? ms+"'Format Type' and 'Format Value' are not empty." : ms+"";
            ms = collection["formattype"] != "" && collection["format"] == "" ? ms+" 'Format Value' is not empty." : ms+"";
            ms = collection["formattype"] == "" && collection["format"] != "" ? ms+" 'Format Type' is not empty." : ms+"";

            return Json(new { success = true, message = ms }, JsonRequestBehavior.AllowGet);          
        }

        public JsonResult UpdateAutocode(FormCollection collection)
        {
            //string TableId = collection["TableIds"];
            string TableId = "3";
            string PkId = collection["hfpkid"];
            string FkId = collection["hffkid"];

            string str = "", dmlFVtype = "", dmltype = "";

            //dmlFVtype = "cmd_fv_insert";
            dmlFVtype = "cmd_fv_update";

            dmltype = "update";

            //if (PkId != "0")
            //{
            //    dmlFVtype = "cmd_fv_update";
            //    //dmltype = "update";
            //}


            string CompanyId = "1";
            Cookie.SetCookie("CompanyId", CompanyId);

            DataRow[] Company = License.CompanyList().Select("CompanyId =" + CompanyId);
            string DataBase = Company[0]["DbName"].ToString();
            string CompanyAlias = Company[0]["CompanyAlias"].ToString();

            CompanyAlias = CompanyAlias + "Live";

            CookieErp.DatabaseName = DataBase;
            CookieErp.CompanyAlias = CompanyAlias;
            CookieErp.CompanyName = "Logic Software Limited";


            Cookie.SetCookie("UserId", "1");
            if (CacheUser.Current.GlobalParamList == null || CacheUser.Current.GlobalParamList.Count == 0)
            {
                FtkData.SetGlobalParam();

            }
            CacheUser.Current.GlobalParamList[4] = "1";
            CacheUser.Current.GlobalParamList[5] = "system";
            CacheUser.Current.GlobalParamList[6] = "Md. Omar Faruk";
            CacheUser.Current.GlobalParamList[10] = "1";

            //INSERT INTO utl_dynamic_module_category(categoryid, applicationid, modulecategory, issystem, sl, entry_user_id, entry_by, entry_date, is_active, is_approved, is_deleted, work_follow_position) values(@@categoryid@@, @applicationid@, @modulecategory@, @issystem@, @sl@, @entry_user_id@, @entry_by@, @entry_date@, @is_active@, @is_approved@, @is_deleted@, @work_follow_position@)

            CookieUser.UserAuthenticate = true;
            CookieUser.Password = "iaas!2017!";

            bool s = false;
            for (int i = 0; i < 1; i++)
            {
                var ColId = collection["columnid[" + i + "]"];
                str = "";
                str = "UPDATE utl_dynamic_table_column SET  coldescription='" + collection["coldescription[" + i + "]"] + "', sqlexpression='" + collection["sqlexpression[" + i + "]"] + "' WHERE columnid ='"+ ColId + "'";


                 s = FtkData.ExecuteNonQueryCommand("1", TableId, FkId, "1", str);

            }

            string ms = "";
            if (s)
            {
                ms = "Saved Successfully!";
            }
            else
            {
                ms = "Failed to Save!";
            }
            return Json(new { success = true, message = ms }, JsonRequestBehavior.AllowGet);


        }
        
        public JsonResult UpdateImage(FormCollection collection)
        {
            //string TableId = collection["TableIds"];
            string TableId = "3";
            string PkId = collection["hfpkid"];
            string FkId = collection["hffkid"];

            int totalRow = Convert.ToInt32(collection["imagehf"]);
            string str = "", dmlFVtype = "", dmltype = "";

            //dmlFVtype = "cmd_fv_insert";
            dmlFVtype = "cmd_fv_update";

            dmltype = "update";

            //if (PkId != "0")
            //{
            //    dmlFVtype = "cmd_fv_update";
            //    //dmltype = "update";
            //}


            string CompanyId = "1";
            Cookie.SetCookie("CompanyId", CompanyId);

            DataRow[] Company = License.CompanyList().Select("CompanyId =" + CompanyId);
            string DataBase = Company[0]["DbName"].ToString();
            string CompanyAlias = Company[0]["CompanyAlias"].ToString();

            CompanyAlias = CompanyAlias + "Live";

            CookieErp.DatabaseName = DataBase;
            CookieErp.CompanyAlias = CompanyAlias;
            CookieErp.CompanyName = "Logic Software Limited";


            Cookie.SetCookie("UserId", "1");
            if (CacheUser.Current.GlobalParamList == null || CacheUser.Current.GlobalParamList.Count == 0)
            {
                FtkData.SetGlobalParam();

            }
            CacheUser.Current.GlobalParamList[4] = "1";
            CacheUser.Current.GlobalParamList[5] = "system";
            CacheUser.Current.GlobalParamList[6] = "Md. Omar Faruk";
            CacheUser.Current.GlobalParamList[10] = "1";

            //INSERT INTO utl_dynamic_module_category(categoryid, applicationid, modulecategory, issystem, sl, entry_user_id, entry_by, entry_date, is_active, is_approved, is_deleted, work_follow_position) values(@@categoryid@@, @applicationid@, @modulecategory@, @issystem@, @sl@, @entry_user_id@, @entry_by@, @entry_date@, @is_active@, @is_approved@, @is_deleted@, @work_follow_position@)

            CookieUser.UserAuthenticate = true;
            CookieUser.Password = "iaas!2017!";

            string ms = "";
            bool s = false;
            for (int i = 0; i < totalRow; i++)
            {
                str = "";
                var ColId = collection["columnid[" + i + "]"];
                int formHeight = collection["formheight[" + i + "]"] == "" ? 30 : Convert.ToInt32(collection["formheight[" + i + "]"]);
                int formWidth = collection["formwidth[" + i + "]"] == "" ? 30 : Convert.ToInt32(collection["formwidth[" + i + "]"]);
                int gridHeight = collection["gridheight[" + i + "]"] == "" ? 30 : Convert.ToInt32(collection["gridheight[" + i + "]"]);
                int gridWidth = collection["gridwidth[" + i + "]"] == "" ? 30 : Convert.ToInt32(collection["gridwidth[" + i + "]"]);

                if(formHeight >= 30 && formWidth >= 30 && gridHeight >= 30 && gridWidth >= 30)
                {
                    str = "UPDATE utl_dynamic_table_column SET  ImgFormHeight='" + formHeight + "', ImgFormWidth='" + formWidth +
                    "',ImgGridHeight='" + gridHeight + "',ImgGridWidth='" + gridWidth + "'  WHERE columnid ='" + ColId + "'";

                    s = FtkData.ExecuteNonQueryCommand("1", TableId, FkId, "1", str);
                    ms = s == true ? ms + "Saved Successfully!" : ms + " Failed to Save!";
                }
                ms = formHeight < 30 ? ms + "'Form Height(" + collection["columndisplayname[" + i + "]"] + ")' must be greater than equal 30." : ms + "";
                ms = formWidth < 30 ? ms + "'Form Width(" + collection["columndisplayname[" + i + "]"] + ")' must be greater than equal 30." : ms + "";
                ms = gridHeight < 30 ? ms + "'Grid Height(" + collection["columndisplayname[" + i + "]"] + ")' must be greater than equal 30." : ms + "";
                ms = gridWidth < 30 ? ms + "'Form Width(" + collection["columndisplayname[" + i + "]"] + ")' must be greater than equal 30." : ms + "";
            }
            string msg = ms.Replace("Saved Successfully!", "");
            msg = msg == "" ? "Saved Successfully!" : msg;

            return Json(new { success = true, message = ms }, JsonRequestBehavior.AllowGet);

        }


    }
}