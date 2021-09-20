

using System;
using Livedev;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using ErpLicense;
using LivedevStudio.Models;

namespace Livedev.Controllers
{
    public class PageController : Controller
    {
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
        string connectionString = WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
        //LivedevData FtkData = new LivedevData();

        //private ErpContext db = new ErpContext();



        LivedevData FtkData = new LivedevData();
        Cookie Cookie = new Cookie();
        CookieErp CookieErp = new CookieErp();
        CacheUser CacheUser = new CacheUser();
        CookieUser CookieUser = new CookieUser();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SubModulePage()
        {

            SqlConnection connection = new SqlConnection(connectionString);
            DataSet submodule = new DataSet();
            DataTable dt = new DataTable();
            string SubModuleQuery = "SELECT  dc.CategoryId,dc.ApplicationId, dc.ModuleCategory, dc.IsSystem, dc.Sl, dm.ModuleAlis FROM Utl_Dynamic_Module_Category dc INNER JOIN Utl_Dynamic_Database_Module dm ON dc.ApplicationId = dm.DatabaseId order by dc.ApplicationId";
            SqlCommand command = new SqlCommand(SubModuleQuery, connection);

            SqlDataAdapter sda = new SqlDataAdapter(command);
            sda.Fill(submodule);
            ViewBag.SubModule = submodule;
            return View();
        }


        public ActionResult TablePagination(int? page) 
        {

		    int perpage=10;
            int startpage = Convert.ToInt32((page - 1)* perpage);
            SqlConnection connection = new SqlConnection(connectionString);
            DataSet submodule = new DataSet();
            DataTable dt = new DataTable();
            string SubModuleQuery = "SELECT  dc.CategoryId,dc.ApplicationId, dc.ModuleCategory, dc.IsSystem, dc.Sl, dm.ModuleAlis FROM Utl_Dynamic_Module_Category dc " +
                "INNER JOIN Utl_Dynamic_Database_Module dm  ON dc.ApplicationId = dm.DatabaseId    order by dc.ApplicationId OFFSET  "+ startpage + " ROWS FETCH NEXT "+10+" ROWS ONLY ";
            SqlCommand command = new SqlCommand(SubModuleQuery, connection);

            SqlDataAdapter sda = new SqlDataAdapter(command);
            sda.Fill(submodule);
            ViewBag.SubModule = submodule;


            DataSet sm = new DataSet();
          
            string smquery = "SELECT  dc.CategoryId,dc.ApplicationId, dc.ModuleCategory, dc.IsSystem, dc.Sl, dm.ModuleAlis FROM Utl_Dynamic_Module_Category dc INNER JOIN Utl_Dynamic_Database_Module dm ON dc.ApplicationId = dm.DatabaseId order by dc.ApplicationId";
            SqlCommand command1 = new SqlCommand(smquery, connection);

            SqlDataAdapter sda1 = new SqlDataAdapter(command);
            sda1.Fill(sm);
            ViewBag.SM = sm;
            return View();
        }

        public ActionResult SearchSubModule(string SubmoduleVal)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            DataSet submodule = new DataSet();
            DataTable dt = new DataTable();
            string SubModuleQuery = "SELECT dc.CategoryId,dc.ApplicationId, dc.ModuleCategory, dc.IsSystem, dc.Sl, dm.ModuleAlis FROM Utl_Dynamic_Module_Category" +
                " dc INNER JOIN  Utl_Dynamic_Database_Module dm ON dc.ApplicationId = dm.DatabaseId where dm.ModuleAlis LIKE '%" + SubmoduleVal + "%' OR dc.ModuleCategory LIKE '%" + SubmoduleVal+ "%' OR dc.ApplicationId LIKE '%" + SubmoduleVal + "%' order by dc.ApplicationId";
            SqlCommand command = new SqlCommand(SubModuleQuery, connection);

            SqlDataAdapter sda = new SqlDataAdapter(command);
            sda.Fill(submodule);
            ViewBag.SubModule = submodule;
            return View();
            
        }



        public ActionResult Pages(int? id)
        {
            ViewBag.ID = id;
            return View();
        }

        public ActionResult Table(int? id)
        {
            ViewBag.ID = id;
            return View();
        }

        public ActionResult EditSubModule(int? submoduleId, int? AppId)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            DataSet module = new DataSet();
            string moduleList = "Select DatabaseId,ModuleAlis from Utl_Dynamic_Database_Module order by DatabaseId";
            SqlCommand command1 = new SqlCommand(moduleList, connection);

            SqlDataAdapter sda1 = new SqlDataAdapter(command1);
            sda1.Fill(module);
            ViewBag.Module = module;

            DataSet EditModule = new DataSet();
           
            string SubModuleQuery = "SELECT CategoryId,ApplicationId, ModuleCategory, IsSystem, Sl  FROM Utl_Dynamic_Module_Category where CategoryId = '" + submoduleId + "' "; 
            SqlCommand command = new SqlCommand(SubModuleQuery, connection);
            SqlDataAdapter sda = new SqlDataAdapter(command);
            sda.Fill(EditModule);
            ViewBag.EditModule = EditModule;
            ViewBag.Aid = AppId;
       


            return View();
        }

        public ActionResult EditPageList(int? pageId, string SubModName)
        {
            DataSet pages = new DataSet();
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(connectionString);
            string PageQuery = "SELECT * from Utl_Dynamic_Module where CategoryId = '" + pageId + "'";

            SqlCommand command = new SqlCommand(PageQuery, connection);

            SqlDataAdapter sda = new SqlDataAdapter(command);
            sda.Fill(pages);
            ViewBag.Pages = pages;
            ViewBag.SubModName = SubModName;
            ViewBag.SubmoduleId = pageId;
            return View();
        }

        public JsonResult UpdatePageList(FormCollection collection)
        {


            SqlConnection connection = new SqlConnection(connectionString);
            int totalRow = Convert.ToInt32(collection["editpagelisthf"]);
            string UpdatePage="";
            string ms = "";
            for (int i = 0; i < totalRow; i++)
            {
                if(collection["isdrilldownreport[" + i + "]"] == null || collection["isdrilldownreport[" + i + "]"] == "")
                {
                    collection["isdrilldownreport[" + i + "]"] = "0";
                }

                UpdatePage = "";
                var ModuleId = collection["moduleid[" + i + "]"];
            
                UpdatePage = "UPDATE Utl_Dynamic_Module SET   ModuleName='" + collection["modulename[" + i + "]"] + "', ModuleCaption='" + collection["modulecaption[" + i + "]"] + "'," +
                    " ModuleType='" + collection["moduletype[" + i + "]"] + "',ReportTitle='" + collection["reporttitle[" + i + "]"] + "',IsDrilldownReport='" + collection["isdrilldownreport[" + i + "]"] + "'," +
                    " SlNo='" + collection["slno[" + i + "]"] + "' WHERE ModuleId ='" + ModuleId + "'";

                SqlCommand command = new SqlCommand(UpdatePage, connection);
                connection.Open();
                int effectfield = command.ExecuteNonQuery();

                connection.Close();
              
                if (effectfield > 0)
                {
                    ms = "Updated Successfully!";
                }
                else
                {
                    ms = "Failed to Update!";
                }

            }


            return Json(new { success = true, message = ms }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult SelectById(int? pageId, string SubModName)
        {
            DataSet pages = new DataSet();
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(connectionString);
            //string PageQuery= "SELECT  dc.CategoryId , dc.ApplicationId , dc.ModuleCategory , dc.IsSystem , dc.Sl, dm.ModuleId, dm.ApplicationId, dm.ExistingModuleId, dm.ModuleName, " +
            //                  "dm.IsNewCopy, dm.ModuleCaption, dm.ReportTitle, dm.CategoryId, dm.ModuleType, dm.FormDefinition, dm.SlNo FROM " +
            //                  "Utl_Dynamic_Module_Category AS dc INNER JOIN Utl_Dynamic_Module AS dm ON dc.CategoryId = dm.CategoryId where dm.CategoryId = '"+ pageId + "'";
            string PageQuery = "SELECT ModuleId,ModuleName,ModuleType,ModuleCaption from Utl_Dynamic_Module where CategoryId = '" + pageId + "'";

            SqlCommand command = new SqlCommand(PageQuery, connection);

            SqlDataAdapter sda = new SqlDataAdapter(command);
            sda.Fill(pages);
            ViewBag.Pages = pages;
            ViewBag.SubModName = SubModName;
            ViewBag.SubmoduleId = pageId;
            return View();
        }

        public ActionResult EditTableList(int? tableId, string pn)
        {
            DataSet table = new DataSet();
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(connectionString);
            //string TableQuery = "SELECT TableCaption, ParentTableId, TemplateType FROM Utl_Dynamic_Table  where ModuleId = '" + tableId + "'";
            string TableQuery = "SELECT c.*, p.TableCaption as ParentGrid, c.TemplateType as TemplateType  FROM Utl_Dynamic_Table c Left outer join  Utl_Dynamic_Table p on c.ParentTableId = p.TableId where c.ModuleId = '" + tableId + "' order by c.SlNo";

            SqlCommand command = new SqlCommand(TableQuery, connection);

            SqlDataAdapter sda = new SqlDataAdapter(command);
            sda.Fill(table);
            ViewBag.Table = table;
            ViewBag.PageName = pn;
            ViewBag.ModuleId = tableId;

            DataSet ParentTableList = new DataSet();
            string ParentTable = "Select TableId,TableCaption from Utl_Dynamic_Table order by TableId";
            SqlCommand command1 = new SqlCommand(ParentTable, connection);
            SqlDataAdapter sda1 = new SqlDataAdapter(command1);
            sda1.Fill(ParentTableList);
            ViewBag.ParentTableList = ParentTableList;

            return View();
        }

        public JsonResult UpdateTableList(FormCollection collection)
        {


            SqlConnection connection = new SqlConnection(connectionString);
            int totalRow = Convert.ToInt32(collection["edittablelisthf"]);
            string UpdateTable = "";
            string ms = "";
            for (int i = 0; i < totalRow; i++)
            {
                UpdateTable = "";
                var TableId = collection["tableid[" + i + "]"];

                if (collection["isreadonly[" + i + "]"] == null || collection["isreadonly[" + i + "]"] == "")
                {
                    collection["isreadonly[" + i + "]"] = "0";
                }
                if (collection["isworkfollow[" + i + "]"] == null || collection["isworkfollow[" + i + "]"] == "")
                {
                    collection["isworkfollow[" + i + "]"] = "0";
                }
                if (collection["showonreport[" + i + "]"] == null || collection["showonreport[" + i + "]"] == "")
                {
                    collection["showonreport[" + i + "]"] = "0";
                }

                UpdateTable = "UPDATE Utl_Dynamic_Table SET   ParentTableId='" + collection["parenttableid[" + i + "]"] + "', ModuleTableCaption='" + collection["moduletablecaption[" + i + "]"] + "',TableCaption='" + collection["tablecaption[" + i + "]"] + "'," +
                    " TemplateType='" + collection["templatetype[" + i + "]"] + "',ModuleWidth='" + collection["modulewidth[" + i + "]"] + "',FormWidth='" + collection["formwidth[" + i + "]"] + "'," +
                    "RowPosition='" + collection["rowposition[" + i + "]"] + "',ColPosition='" + collection["colposition[" + i + "]"] + "',IsReadOnly='" + collection["isreadonly[" + i + "]"] + "'," +
                    "IsWorkFollow='" + collection["isworkfollow[" + i + "]"] + "',ShowOnReport='" + collection["showonreport[" + i + "]"] + "',SlNo='" + collection["slno[" + i + "]"] + "' WHERE TableId ='" + TableId + "'";

                SqlCommand command = new SqlCommand(UpdateTable, connection);
                connection.Open();
                int effectfield = command.ExecuteNonQuery();

                connection.Close();

                if (effectfield > 0)
                {
                    ms = "Updated Successfully!";
                }
                else
                {
                    ms = "Failed to Update!";
                }

            }


            return Json(new { success = true, message = ms }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SelectTableById(int? pageId, string pn)
        {
            DataSet table = new DataSet();
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(connectionString);
            //string TableQuery = "SELECT TableCaption, ParentTableId, TemplateType FROM Utl_Dynamic_Table  where ModuleId = '" + tableId + "'";
            string TableQuery = "SELECT c.TableId,c.TableName,c.TableCaption ,p.TableCaption as ParentGrid, c.TemplateType  FROM Utl_Dynamic_Table c Left outer join  Utl_Dynamic_Table p on c.ParentTableId = p.TableId where c.ModuleId = '" + pageId + "' order by c.SlNo";
            
            SqlCommand command = new SqlCommand(TableQuery, connection);

            SqlDataAdapter sda = new SqlDataAdapter(command);
            sda.Fill(table);
            ViewBag.Table = table;
            ViewBag.PageName = pn;
            ViewBag.ModuleId = pageId;
           
            return View();
        }

        public ActionResult EditPageById(int? pageId)
        {

            SqlConnection connection = new SqlConnection(connectionString);
            DataSet module = new DataSet();
            string moduleList = "Select DatabaseId,ModuleAlis from Utl_Dynamic_Database_Module order by DatabaseId";
            SqlCommand command1 = new SqlCommand(moduleList, connection);

            SqlDataAdapter sda1 = new SqlDataAdapter(command1);
            sda1.Fill(module);
            ViewBag.Module = module;

            DataSet EditPage = new DataSet();

            string PageQuery = "SELECT  ModuleId, m.ApplicationId, ExistingModuleId, IsNewCopy, ModuleName, ModuleCaption, ReportTitle, IsDrilldownReport," +
                "m.CategoryId,c.ModuleCategory, ImagePath, ModuleType, FormDefinition, SlNo, IsActive FROM Utl_Dynamic_Module m inner join Utl_Dynamic_Module_Category c on m.CategoryId = c.CategoryId" +
                " where ModuleId = '" + pageId + "' order by m.SlNo ";
            SqlCommand command = new SqlCommand(PageQuery, connection);
            SqlDataAdapter sda = new SqlDataAdapter(command);
            sda.Fill(EditPage);
            ViewBag.EditPage = EditPage;

            DataSet ExitingPage = new DataSet();

            string ExistPageQuery = "SELECT  ModuleId,  ModuleCaption FROM Utl_Dynamic_Module";
            SqlCommand command2 = new SqlCommand(ExistPageQuery, connection);
            SqlDataAdapter sda2 = new SqlDataAdapter(command2);
            sda2.Fill(ExitingPage);
            ViewBag.ExitingPage = ExitingPage;

            return View();
        }
        public ActionResult EditTableById(int? tableId)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            DataSet EditTable = new DataSet();
            string EditTableList = "SELECT m.ModuleId,m.ModuleCaption, c.TableId,c.TableCaption,c.ModuleTableCaption,c.TableName,c.RefreshType,c.ParentTableId,c.ForeignTableId, c.ForeignModuleId, c.IsExternalDataShow," +
                " c.ExternalDataCaption,c.ExternalDataFilter, c.ExistingTableId, c.ModuleWidth, c.ModuleHeight, c.FormWidth, c.PositionExp, c.RowPosition, c.ColPosition, c.IsReadOnly, c.IsWorkFollow, c.IsAutoToggle," +
                " c.ProcedureName,c.ProcedureCaption, c.ProcedureValidation, c.ProcFireOnTrans,c.ReportProcedureName, c.RptProcForWorksheet, c.ProcFooterView,c.ProcFooterDml,c.Is_Active, c.ReportFileName," +
                " c.ShowOnReport, c.SqlStatement, c.ObjectType, c.TemplateType, c.FormDefinition, c.SlNo, p.TableCaption as ParentGrid, c.TemplateType FROM Utl_Dynamic_Table c " +
                "Left outer join Utl_Dynamic_Table p on c.ParentTableId = p.TableId inner join Utl_Dynamic_Module m on c.ModuleId = m.ModuleId where c.TableId = '"+ tableId + "' order by c.SlNo";
            SqlCommand command = new SqlCommand(EditTableList, connection);
            SqlDataAdapter sda = new SqlDataAdapter(command);
            sda.Fill(EditTable);
            ViewBag.EditTable = EditTable;


            DataSet DbTableLists = new DataSet();
            string TableName = "select lower(d.DataField) as DataField,d.DataText from (select TABLE_NAME AS DataField, TABLE_NAME AS DataText, TABLE_TYPE from" +
                " INFORMATION_SCHEMA.TABLES Union All select 'INFORMATION_SCHEMA.TABLES' AS DataField,'INFORMATION_SCHEMA_TABLES' AS DataText,'A' AS TABLE_TYPE ) d ORDER BY d.TABLE_TYPE,d.DataText";
            SqlCommand command2 = new SqlCommand(TableName, connection);
            SqlDataAdapter sda2 = new SqlDataAdapter(command2);
            sda2.Fill(DbTableLists);
            ViewBag.DbTableList = DbTableLists;

            DataSet DbProcedureLists = new DataSet();
            string ProcedureNames = "select lower(name) AS DataField,name AS DataText from sys.objects where type='P' and is_ms_shipped =0 ORDER BY name";
            SqlCommand command3 = new SqlCommand(ProcedureNames, connection);
            SqlDataAdapter sda3 = new SqlDataAdapter(command3);
            sda3.Fill(DbProcedureLists);
            ViewBag.DbProcedureLists = DbProcedureLists;


            DataSet ParentTableList = new DataSet();
            string ParentTable = "SELECT  [TableId],[ModuleTableCaption] ,[TableName], ModuleTableCaption+' [ '+TableName+' ]' as TableCaption FROM [dbo].[Utl_Dynamic_Table] order by TableId";
            SqlCommand command1 = new SqlCommand(ParentTable, connection);
            SqlDataAdapter sda1 = new SqlDataAdapter(command1);
            sda1.Fill(ParentTableList);
            ViewBag.ParentTableList = ParentTableList;

            return View();



        }
 
        public ActionResult EditPanelById(int? PanelId,int? TableId)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            DataSet EditPanel = new DataSet();
            string EditPanelList = "Select Panelname,PanelCaption,RowPosition,ColPosition,isnull(PnlColNo,1) as PnlColNo,isnull(IsVertically,0) as IsVertically,SlNo From Utl_Dynamic_Table_Panel where PanelId='" + PanelId + "' ";

            SqlCommand command = new SqlCommand(EditPanelList, connection);
            SqlDataAdapter sda = new SqlDataAdapter(command);
            sda.Fill(EditPanel);
            ViewBag.EditPanel = EditPanel;
            ViewBag.TableId = TableId;
            ViewBag.PanelId = PanelId;
            return View();
        }

        public JsonResult UpdatePanel(FormCollection collection)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            long PanelId = Convert.ToInt64(collection["panelid"]);

            int isvertically = collection["IsVertically"] == "" ? 0 : Convert.ToInt32(collection["IsVertically"]);
            int pnlColNo = collection["PnlColNo"] == "" ? 0 : Convert.ToInt32(collection["PnlColNo"]);
            string ms = "";
            if (pnlColNo > 0 && collection["RowPosition"] != "" && collection["ColumnPosition"] != "")
            {
                string UpdatePanel = "UPDATE Utl_Dynamic_Table_Panel SET  PanelName='" + collection["PanelName"] + "',PanelCaption='" + collection["PanelCaption"] + "', RowPosition='" +
                collection["RowPosition"] + "', ColPosition ='" + collection["ColumnPosition"] + "',PnlColNo ='" + pnlColNo + "',IsVertically =isnull('" + isvertically + "',0), SlNo= '" + collection["SlNo"] + "', Update_By ='"
                + collection["update_by"] + "', Update_Date = " + collection["update_date"] + " WHERE PanelId ='" + PanelId + "'";

                SqlCommand command = new SqlCommand(UpdatePanel, connection);
                connection.Open();
                int effectfield = command.ExecuteNonQuery();
                connection.Close();
                if (effectfield > 0)
                {
                    ms = "Updated Successfully!";
                }
                else
                {
                    ms = "Failed to update!";
                }
            }
            else
            {
                ms = "Data was not Update because =>";
                ms = pnlColNo < 1 ? ms + "'Row Column No.'must be greater than 0 (zero)." : ms;
                ms = collection["RowPosition"] == "" ? ms + "'Row Position' is not empty." : ms;
                ms = collection["ColumnPosition"] == "" ? ms + "'Column Position' is not empty." : ms;
            }
            return Json(new { success = true, message = ms }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeletePanelById(int? PanelId)
        {

            SqlConnection connection = new SqlConnection(connectionString);
           
            string DeletePanel = "Delete from Utl_Dynamic_Table_Panel   WHERE PanelId ='" + PanelId + "'";

            SqlCommand command = new SqlCommand(DeletePanel, connection);
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
        public JsonResult DeleteTriggerById(int? TriggerId)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            string Deletetrigger = "DELETE FROM Utl_Dynamic_Table_Trigger WHERE TriggerId = " + TriggerId + "";

            SqlCommand command = new SqlCommand(Deletetrigger, connection);
            connection.Open();
            int effectfield = command.ExecuteNonQuery();

            connection.Close();
            string ms = effectfield > 0 ? "Deleted Successfully!" : "Failed to Delete!";
            return Json(new { success = true, message = ms }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult EditAutocodeById(int? propertyId,int? tableid,int? ModuleId)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            DataSet EditAutocode = new DataSet();
            string EditAutocodequery = "Select * From Utl_Dynamic_Autocode_Property where PropertyId='" + propertyId + "'";

            SqlCommand command = new SqlCommand(EditAutocodequery, connection);
            SqlDataAdapter sda = new SqlDataAdapter(command);
            sda.Fill(EditAutocode);
            ViewBag.EditAutocode = EditAutocode;

            ViewBag.TableId = tableid;
            ViewBag.ModuleId = ModuleId;

            return View();
        }

        

        public JsonResult UpdateAutocode(FormCollection collection)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            int effectfield = 0;
            string ms = "";

            if (collection["formattype"] != "" && collection["format"] != "")
            {
                string UpdateAuto = "UPDATE Utl_Dynamic_Autocode_Property SET  FormatType='" + collection["formattype"] + "',Format='" + collection["format"] + "', Expression='" + collection["expression"] + "', SlNo ='" + collection["slno"] + "'," +
                " IsActive= '" + collection["isactive"] + "', Update_By ='" + collection["update_by"] + "', Update_Date =" + collection["update_date"] + "  WHERE PropertyId ='" + collection["propertyid"] + "'";

                SqlCommand command = new SqlCommand(UpdateAuto, connection);
                connection.Open();
                effectfield = command.ExecuteNonQuery();
                connection.Close();
                ms = effectfield > 0 ? "Updated Successfully!" : "Failed to Save!";
            }

            ms = collection["formattype"] == "" && collection["format"] == "" ? ms + "'Format Type' and 'Format Value' are not empty." : ms + "";
            ms = collection["formattype"] != "" && collection["format"] == "" ? ms + " 'Format Value' is not empty." : ms + "";
            ms = collection["formattype"] == "" && collection["format"] != "" ? ms + " 'Format Type' is not empty." : ms + "";

            return Json(new { success = true, message = ms }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteAutocodeById(int? PropertyId )
        {

            SqlConnection connection = new SqlConnection(connectionString);

            string DeleteAutocode = "Delete from Utl_Dynamic_Autocode_Property   WHERE PropertyId ='" + PropertyId + "'";

            SqlCommand command = new SqlCommand(DeleteAutocode, connection);
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
        public ActionResult EditColumnById(int? columnId)
        {
            return View();
        }

        public ActionResult CreateSubModule(int? submoduleId)
        {

            DataSet module = new DataSet();
            SqlConnection connection = new SqlConnection(connectionString);
            string moduleList = "Select DatabaseId,ModuleAlis from Utl_Dynamic_Database_Module";
            SqlCommand command = new SqlCommand(moduleList, connection);

            SqlDataAdapter sda = new SqlDataAdapter(command);
            sda.Fill(module);
            ViewBag.Module = module;
            ViewBag.PId = 0;
            return View();
        }

        public ActionResult CreatePage(string SubModName,int? SubModId)
        {

            SqlConnection connection = new SqlConnection(connectionString);
            DataSet module = new DataSet();
            string moduleList = "Select DatabaseId,ModuleAlis from Utl_Dynamic_Database_Module order by DatabaseId";
            SqlCommand command1 = new SqlCommand(moduleList, connection);

            SqlDataAdapter sda1 = new SqlDataAdapter(command1);
            sda1.Fill(module);
            ViewBag.Module = module;

            DataSet EditPage = new DataSet();

            //string PageQuery = "SELECT  ModuleId, m.ApplicationId, ExistingModuleId, IsNewCopy, ModuleName, ModuleCaption, ReportTitle, IsDrilldownReport," +
            //    "m.CategoryId,c.ModuleCategory, ImagePath, ModuleType, FormDefinition, SlNo, IsActive FROM Utl_Dynamic_Module m inner join Utl_Dynamic_Module_Category c on m.CategoryId = c.CategoryId" +
            //    " where ModuleId = '" + pageId + "' ";
            //SqlCommand command = new SqlCommand(PageQuery, connection);
            //SqlDataAdapter sda = new SqlDataAdapter(command);
            //sda.Fill(EditPage);
            //ViewBag.EditPage = EditPage;

            DataSet ExitingPage = new DataSet();

            string ExistPageQuery = "SELECT  ModuleId,  ModuleCaption FROM Utl_Dynamic_Module";
            SqlCommand command2 = new SqlCommand(ExistPageQuery, connection);
            SqlDataAdapter sda2 = new SqlDataAdapter(command2);
            sda2.Fill(ExitingPage);
            ViewBag.ExitingPage = ExitingPage;

            ViewBag.SubmoduleName = SubModName;
            ViewBag.SubmoduleId = SubModId;
            return View();
        }
        public ActionResult CreateTable(string pageName,int? pageId)
        {
            SqlConnection connection = new SqlConnection(connectionString);
           
            ViewBag.PageId = pageId;
            ViewBag.PageName = pageName;
            DataSet ParentTableLists = new DataSet();
            string ParentTable = "SELECT  [TableId],[ModuleTableCaption] ,[TableName], ModuleTableCaption+' [ '+TableName+' ]' as TableCaption FROM [dbo].[Utl_Dynamic_Table] order by TableId";
            SqlCommand command1 = new SqlCommand(ParentTable, connection);
            SqlDataAdapter sda1 = new SqlDataAdapter(command1);
            sda1.Fill(ParentTableLists);
            ViewBag.ParentTableLists = ParentTableLists;

            DataSet DbTableLists = new DataSet();
            string TableName = "select lower(d.DataField) as DataField,d.DataText from (select TABLE_NAME AS DataField, TABLE_NAME AS DataText, TABLE_TYPE from" +
                " INFORMATION_SCHEMA.TABLES Union All select 'INFORMATION_SCHEMA.TABLES' AS DataField,'INFORMATION_SCHEMA_TABLES' AS DataText,'A' AS TABLE_TYPE ) d ORDER BY d.TABLE_TYPE,d.DataText";
            SqlCommand command2 = new SqlCommand(TableName, connection);
            SqlDataAdapter sda2 = new SqlDataAdapter(command2);
            sda2.Fill(DbTableLists);
            ViewBag.DbTableLists = DbTableLists;
 
            DataSet DbProcedureLists = new DataSet();
            string ProcedureNames = "select lower(name) AS DataField,name AS DataText from sys.objects where type='P' and is_ms_shipped =0 ORDER BY name";
            SqlCommand command3 = new SqlCommand(ProcedureNames, connection);
            SqlDataAdapter sda3 = new SqlDataAdapter(command3);
            sda3.Fill(DbProcedureLists);
            ViewBag.DbProcedureLists = DbProcedureLists;
            return View();
        }

        public JsonResult SaveSystemField(long? TableId)
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
                SqlCommand dCmd = new SqlCommand("Tmp_Add_System_Column", connection);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@TableId", TableId));

                connection.Open();
                //int s = dCmd.ExecuteNonQuery();
                object s = dCmd.ExecuteScalar();
                connection.Close();
                ms = "Saved Successfully!";
            }
            else
            {
                ms = "Columns already exist in this table. Do you want to create new columns? First, you delete existing columns in this table.";
            }

            return Json(new { success = true, message = ms }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveAllField(long? TableId)
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
                SqlCommand dCmd = new SqlCommand("Tmp_Add_Dynamic_Column", connection);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@TableId", TableId));

                connection.Open();
                //int s = dCmd.ExecuteNonQuery();
                object s = dCmd.ExecuteScalar();
                connection.Close();
                ms = "Saved Successfully!";
            }
            else
            {
                ms = "Columns already exist in this table. Do you want to create new columns? First, you delete existing columns in this table.";
            }
            return Json(new { success = true, message = ms }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SelectColumnById(int? TableId,string TableName,int ModuleId)
        {
            DataSet column = new DataSet();
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(connectionString);
            string TableQuery = "SELECT ColumnId,ColumnName, SelectColumnName, ColumnDisplayName, ColumnProperty, ControlType FROM Utl_Dynamic_Table_Column where TableId = '" + TableId + "' and IsShow='1' order by SlNo";
            SqlCommand command = new SqlCommand(TableQuery, connection);
            SqlDataAdapter sda = new SqlDataAdapter(command);
            sda.Fill(column);
            ViewBag.Column = column;
            ViewBag.tablename = TableName;
            ViewBag.tableid = TableId;
            ViewBag.ModuleId = ModuleId;
            return View();
        }
        public ActionResult SelectPanelById(int? PanelId,int? TableId)
        {
            DataSet SavedColumnList = new DataSet();
            DataSet UnSavedColumnList = new DataSet();
            SqlConnection connection = new SqlConnection(connectionString);
            string SavedColumnQuery = "SELECT ColumnId,ColumnName,RowPosition,ColPosition, ColumnDisplayName,PanelId,PnlColSl,PnlColMerge FROM Utl_Dynamic_Table_Column where TableId = '" + TableId + "' and PanelId='" + PanelId + "' and IsShow='1' order by PnlColSl";
            SqlCommand command = new SqlCommand(SavedColumnQuery, connection);
            SqlDataAdapter sda = new SqlDataAdapter(command);
            sda.Fill(SavedColumnList);
            ViewBag.SavedColumnList = SavedColumnList;

           
            string UnSavedColumnQuery = "SELECT ColumnId,ColumnName, ColumnDisplayName,PanelId,PnlColSl,PnlColMerge FROM Utl_Dynamic_Table_Column where TableId = '" + TableId + "' and isnull(PanelId,0)=0 and IsShow='1'";
            SqlCommand command1 = new SqlCommand(UnSavedColumnQuery, connection);
            SqlDataAdapter sda1 = new SqlDataAdapter(command1);
            sda1.Fill(UnSavedColumnList);
            ViewBag.UnSavedColumnList = UnSavedColumnList;

            ViewBag.TableId = TableId;
            ViewBag.PanelId = PanelId;
            return View();
        }
        public ActionResult CreatePanel(int? cf)
        {
            return View();
        }
        public ActionResult CreateColumn(int? cf)
        {
            return View();
        }
        public ActionResult SetColumn(int? TableId,string TableName,int? ModuleId)
        {
            DataSet ds = new DataSet();


            DataSet ddlfield = new DataSet();
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(connectionString);
            ViewBag.tableId = TableId;
            ViewBag.tableName = TableName;
            ViewBag.ModuleId = ModuleId;
            string ObjectQuery = "SELECT [object_id] as ObjectId  from sys.tables where name='" + TableName + "'";
            SqlCommand com = new SqlCommand(ObjectQuery, connection);
            connection.Open();
            var ObjId = com.ExecuteScalar();
            connection.Close();
            string query = "SELECT  name AS DataField from sys.all_columns where [object_id]='"+ ObjId + "' and name not in (select ColumnName from Utl_Dynamic_Table_Column where TableId='"+TableId+"')";
           // string query = "SELECT  name AS DataField from sys.all_columns where [object_id]='550605350'";
           // string query = "SELECT  name AS DataField from sys.all_columns where object_id=(SELECT object_id from sys.all_objects WHERE name=(SELECT TableName from UTL_DYNAMIC_TABLE where TableId=448)) order by column_id";
            SqlCommand command = new SqlCommand(query, connection);
            //SELECT[TableName] FROM[Genikos_Erp].[dbo].[Utl_Dynamic_Table] where TableId = '448'
            SqlDataAdapter sda = new SqlDataAdapter(command);
            sda.Fill(ds);
            ViewBag.Data = ds;

            DataSet dbfield = new DataSet();
          
            string DbFieldQuery = "Select * from utl_dynamic_table_column where IsShow='1' and  TableId='" + TableId + "'";
            SqlCommand command2 = new SqlCommand(DbFieldQuery, connection);

            SqlDataAdapter sda2 = new SqlDataAdapter(command2);
            sda2.Fill(dbfield);
            ViewBag.Df = dbfield;

            DataSet ParentTableList = new DataSet();
            string ParentTable = "SELECT  [TableId],[ModuleTableCaption] ,[TableName], ModuleTableCaption+' [ '+TableName+' ]' as TableCaption FROM [dbo].[Utl_Dynamic_Table] order by TableId";
            SqlCommand command1 = new SqlCommand(ParentTable, connection);
            SqlDataAdapter sda1 = new SqlDataAdapter(command1);
            sda1.Fill(ParentTableList);
            ViewBag.ExsitingTableList = ParentTableList;


            return View();
        }

        public ActionResult ShowUnmapColumn(int TableId, string TableName)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string ObjectQuery = "SELECT [object_id] as ObjectId  from sys.tables where name='" + TableName + "'";
            SqlCommand com = new SqlCommand(ObjectQuery, connection);
            connection.Open();
            var ObjId = com.ExecuteScalar();
            connection.Close();

            DataSet UnmapColList = new DataSet();
            string UnmapQuery = "select ColumnName from Utl_Dynamic_Table_Column where TableId='"+ TableId + "' and ColumnName not in (SELECT  name AS DataField from sys.all_columns where object_id='"+ ObjId + "')";
            SqlCommand command = new SqlCommand(UnmapQuery, connection);
            SqlDataAdapter sda = new SqlDataAdapter(command);
            sda.Fill(UnmapColList);
            ViewBag.unmap = UnmapColList;
            ViewBag.tableid = TableId;

            DataSet mapColData = new DataSet();
            string mapColQuery = "SELECT  name AS DataField from sys.all_columns where [object_id]='" + ObjId + "' and name not in (select ColumnName from Utl_Dynamic_Table_Column where TableId='" + TableId + "')";
            SqlCommand cmd = new SqlCommand(mapColQuery, connection);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
            dataAdapter.Fill(mapColData);
            ViewBag.map = mapColData;
            return View();
        }


        public ActionResult SetPanel(int? TableId)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            DataSet PanelList = new DataSet();
            string panelquery = "Select PanelId,PanelName,PanelCaption,RowPosition,ColPosition,isnull(PnlColNo,0) as PnlColNo,isnull(IsVertically,0) as IsVertically, SlNo from Utl_Dynamic_Table_Panel where TableId='" + TableId+"'";
            SqlCommand command = new SqlCommand(panelquery, connection);
            SqlDataAdapter sda = new SqlDataAdapter(command);
            sda.Fill(PanelList);
            ViewBag.PanelList = PanelList;

            ViewBag.tableId = TableId;
            return View();
        }

     
        public ActionResult SetAutocode(int? TableId,int? ModuleId)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            DataSet AutocodeList = new DataSet();
            string autoquery = "Select ColumnId, ColumnDisplayName from Utl_Dynamic_Table_Column where ColumnProperty ='auto' and TableId='" + TableId + "' and IsShow='1' order by SlNo ";
            SqlCommand command8 = new SqlCommand(autoquery, connection);
            SqlDataAdapter sda8 = new SqlDataAdapter(command8);
            sda8.Fill(AutocodeList);
            ViewBag.AutocodeList = AutocodeList;


            long columnId;
            DataSet ColAutoList = new DataSet();
            foreach (DataRow AutoId in AutocodeList.Tables[0].Rows)
            {
                columnId = Convert.ToInt64(AutoId["ColumnId"]);
                string autoQuery = "SELECT  PropertyId,FormatType,Format, SlNo FROM Utl_Dynamic_Autocode_Property WHERE  ColumnId ='" + columnId + "' ";
                SqlCommand command10 = new SqlCommand(autoQuery, connection);
                SqlDataAdapter sda10 = new SqlDataAdapter(command10);
                sda10.Fill(ColAutoList);

            }

            ViewBag.ColumnAutoList = ColAutoList;
            ViewBag.tableId = TableId;
            ViewBag.ModuleId = ModuleId;
            return View();
        }

        public ActionResult SetImage(int? TableId)
        {
            SqlConnection connection = new SqlConnection(connectionString);
          
            DataSet ImageList = new DataSet();
            string imageQuery = " Select * From Utl_Dynamic_Table_Column where TableId='"+TableId+"' and ControlType='img' and IsShow='1' order by SlNo";
            SqlCommand command9 = new SqlCommand(imageQuery, connection);
            SqlDataAdapter sda9 = new SqlDataAdapter(command9);
            sda9.Fill(ImageList);
            ViewBag.ImageList = ImageList;
            ViewBag.tableId = TableId;
            return View();
        }

        public ActionResult SetExpression(int? TableId)
        {
            DataSet expressionList = new DataSet();
            SqlConnection connection = new SqlConnection(connectionString);
            string ExpressionQuery = "Select * from utl_dynamic_table_column where IsShow='1' and TableId='" + TableId + "' order by SlNo";
            SqlCommand command1 = new SqlCommand(ExpressionQuery, connection);

            SqlDataAdapter sda1 = new SqlDataAdapter(command1);
            sda1.Fill(expressionList);
            ViewBag.ExpressionList = expressionList;
            ViewBag.tableId = TableId;
            return View();
        }

        public ActionResult SetFunction(int? TableId, int? ModuleId)
        {
            DataSet functionList = new DataSet();
            SqlConnection connection = new SqlConnection(connectionString);
            string FunctionQuery = "Select ColumnId,ColumnName, ColumnDisplayName,Isnull(IsDataShort,0)  as IsDataShort, Isnull(IsDescending,0) as IsDescending,AggregateFunction, EvaluateValue, EffectColumn from Utl_Dynamic_Table_Column where IsShow= '1' and TableId='" + TableId + "'  order by SlNo"; /*and ControlType in ('ddl', 'txt', 'rbl', '0')*/
            SqlCommand command1 = new SqlCommand(FunctionQuery, connection);

            SqlDataAdapter sda1 = new SqlDataAdapter(command1);
            sda1.Fill(functionList);
            ViewBag.FunctionList = functionList;
            ViewBag.tableId = TableId;
            ViewBag.moduleid = ModuleId;
            return View();
        }
     
        public ActionResult AddFunctionById(int? columnid,string columnname, int? tableid,int? ModuleId)
        {
            ViewBag.ColumnId = columnid;
            ViewBag.ColumnName = columnname;
            ViewBag.TableId = tableid;
            ViewBag.moduleid = ModuleId;
            return View();
        }

        public ActionResult ShowFunctionById(int? columnid,string ColumnName ,int? tableid)
        {
            DataSet functionList = new DataSet();
            SqlConnection connection = new SqlConnection(connectionString);
            string FunctionQuery = "Select TableFunctionId, ColumnId, ColumnName, EvaluateValue, EffectColumn , SlNo, Is_Active from Utl_Dynamic_Table_Function where  TableId='" + tableid + "' and ColumnId='"+ columnid + "'  order by SlNo";
            SqlCommand command1 = new SqlCommand(FunctionQuery, connection);

            SqlDataAdapter sda1 = new SqlDataAdapter(command1);
            sda1.Fill(functionList);
            ViewBag.FunctionList = functionList;
            ViewBag.ColumnId = columnid;
            ViewBag.tableId = tableid;
            return View();
           
        }
        public ActionResult EditFunctionById(int? FunctionId, int? TableId,int? ColumnId,string ColumnName)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            DataSet EditFunction = new DataSet();
            string EditFunctionList = "Select * From Utl_Dynamic_Table_Function where TableFunctionId='" + FunctionId + "' ";

            SqlCommand command = new SqlCommand(EditFunctionList, connection);
            SqlDataAdapter sda = new SqlDataAdapter(command);
            sda.Fill(EditFunction);
            ViewBag.EditFunction = EditFunction;
            ViewBag.TableId = TableId;
            ViewBag.ColumnId = ColumnId;
            ViewBag.ColumnName = ColumnName;
            return View();
        }

        public JsonResult UpdateFunctionById(FormCollection collection)
        {
            string ms = "";
            SqlConnection connection = new SqlConnection(connectionString);
            decimal slno = collection["SlNo"] == "" ? 0 : Convert.ToDecimal(collection["SlNo"]);
            if (collection["evaluatevalue"] != "" && collection["effectcolumn"] != "")
            {
                string UpdateFunction = "UPDATE Utl_Dynamic_Table_Function SET  EvaluateValue='" + collection["evaluatevalue"] + "',EffectColumn='" + collection["effectcolumn"] + "', SlNo='" +
                slno + "', Is_Active ='" + collection["is_active"] + "',Update_By ='" + collection["update_by"] + "', Update_Date = " + collection["update_date"] + "  WHERE TableFunctionId ='" + collection["TableFunctionId"] + "'";

                SqlCommand command = new SqlCommand(UpdateFunction, connection);
                connection.Open();
                int effectfield = command.ExecuteNonQuery();
                connection.Close();
                ms = effectfield > 0 ? "Update Successfully!" : "Failed to Update!";
            }
            else
            {
                ms = collection["evaluatevalue"] == "" && collection["effectcolumn"] == "" ? "The 'Evaluate Expression' and 'Effect Field' will not be empty"
                : collection["evaluatevalue"] == "" && collection["effectcolumn"] != "" ? "The 'Evaluate Expression' will not be empty"
                : collection["evaluatevalue"] != "" && collection["effectcolumn"] == "" ? "The 'Effect Field' will not be empty" : "";
            }
       
            return Json(new { success = true, message = ms }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteFunctionById(int? FunctionId)
        {

            SqlConnection connection = new SqlConnection(connectionString);

            string DeleteFunction = "Delete from Utl_Dynamic_Table_Function   WHERE TableFunctionId ='" + FunctionId + "'";

            SqlCommand command = new SqlCommand(DeleteFunction, connection);
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


        public ActionResult SetSorting(int? TableId)
        {
            DataSet sortingList = new DataSet();
            SqlConnection connection = new SqlConnection(connectionString);
            string SortingQuery = "Select ColumnId,ColumnName, ColumnDisplayName,Isnull(IsDataShort,0)  as IsDataShort, Isnull(IsDescending,0) as IsDescending,AggregateFunction, EvaluateValue, EffectColumn from Utl_Dynamic_Table_Column where IsShow= '1' and TableId='" + TableId + "' order by SlNo";
            SqlCommand command1 = new SqlCommand(SortingQuery, connection);

            SqlDataAdapter sda1 = new SqlDataAdapter(command1);
            sda1.Fill(sortingList);
            ViewBag.SortingList = sortingList;
            ViewBag.tableId = TableId;
            return View();
        }
        public ActionResult SetDdlSource(int? TableId)
        {
            DataSet ddlList = new DataSet();
            SqlConnection connection = new SqlConnection(connectionString);
            string ddlQuery = "Select * from utl_dynamic_table_column where IsShow='1' and TableId='" + TableId + "' and ControlType='ddl' order by SlNo";
            SqlCommand command1 = new SqlCommand(ddlQuery, connection);

            SqlDataAdapter sda1 = new SqlDataAdapter(command1);
            sda1.Fill(ddlList);
            ViewBag.DdlList = ddlList;
            ViewBag.tableId = TableId;

            DataSet LovTableList = new DataSet();
            string TableList = "SELECT  [TableId],[ModuleTableCaption] ,[TableName], ModuleTableCaption+' [ '+TableName+' ]' as TableCaption FROM [dbo].[Utl_Dynamic_Table] order by TableId";

            SqlCommand command5 = new SqlCommand(TableList, connection);
            SqlDataAdapter sda5 = new SqlDataAdapter(command5);
            sda5.Fill(LovTableList);
            ViewBag.LovTableList = LovTableList;

            DataSet DbTableList = new DataSet();
            string DBTableName = "select lower(TABLE_NAME) AS DataField,TABLE_NAME AS DataText from INFORMATION_SCHEMA.TABLES ORDER BY TABLE_TYPE,TABLE_NAME";
            SqlCommand command6 = new SqlCommand(DBTableName, connection);
            SqlDataAdapter sda6 = new SqlDataAdapter(command6);
            sda6.Fill(DbTableList);
            ViewBag.DbTableList = DbTableList;


            DataSet ParentDdList = new DataSet();
            string parentcolumn = "SELECT lower(ControlType+ColumnName) as DataField,ColumnDisplayName as DataText FROM Utl_Dynamic_Table_Column where TableId ='"+TableId+"' and ControlType = 'ddl' Order By SlNo";
            SqlCommand command = new SqlCommand(parentcolumn, connection);
            SqlDataAdapter sda = new SqlDataAdapter(command);
            sda.Fill(ParentDdList);
            ViewBag.ParentDdList = ParentDdList;

            


            return View();
        }

        public ActionResult SetFieldValidation(int? TableId, string SystemField, string Sort)
        {
            DataSet validationList = new DataSet();
            SqlConnection connection = new SqlConnection(connectionString);
            string validationQuery = "Select * from utl_dynamic_table_column where IsShow='1' and TableId='" + TableId + "' " + SystemField + " order by " + Sort + "";
            SqlCommand command1 = new SqlCommand(validationQuery, connection);

            SqlDataAdapter sda1 = new SqlDataAdapter(command1);
            sda1.Fill(validationList);
            ViewBag.validationList = validationList;
            ViewBag.tableId = TableId;
            return View();
        }
        public ActionResult SetRole(int? TableId, string SystemField, string Sort)
        {
            DataSet roleList = new DataSet();
            SqlConnection connection = new SqlConnection(connectionString);
            string RoleQuery = "Select * from utl_dynamic_table_column where IsShow='1' and TableId='" + TableId + "' " + SystemField + " order by " + Sort + "";
            SqlCommand command1 = new SqlCommand(RoleQuery, connection);

            SqlDataAdapter sda1 = new SqlDataAdapter(command1);
            sda1.Fill(roleList);
            ViewBag.RoleList = roleList;
            ViewBag.tableId = TableId;

            DataSet UserRoleList = new DataSet();
            string UserRoleQuery = "Select [Object_Id], [Object_Name] from wf_Privilege_Object";
            SqlCommand command = new SqlCommand(UserRoleQuery, connection);

            SqlDataAdapter sda = new SqlDataAdapter(command);
            sda.Fill(UserRoleList);
            ViewBag.UserRoleList = UserRoleList;


            DataSet DataRoleList = new DataSet();
            string DataRoleQuery = "Select Trans_Policy_Id,Policy_Name from wf_user_trans_open_policy order by Policy_Name";
            SqlCommand command3 = new SqlCommand(DataRoleQuery, connection);

            SqlDataAdapter sda3 = new SqlDataAdapter(command3);
            sda3.Fill(DataRoleList);
            ViewBag.DataRoleList = DataRoleList;

            return View();
        }


        public ActionResult SetHyperLink(int? TableId, string SystemField, string Sort)
        {
            DataSet pagelist = new DataSet();
            DataSet linkList = new DataSet();
            SqlConnection connection = new SqlConnection(connectionString);
            string LinkQuery = "select c.*,t.TableCaption,t.TableId from utl_dynamic_table_column c left outer join Utl_Dynamic_Table t ON c.LinkTableId = t.TableId where c.TableId = '"
                + TableId + "' and c.IsShow = '1' " + SystemField + " order by " + Sort + "";
            SqlCommand command1 = new SqlCommand(LinkQuery, connection);

            SqlDataAdapter sda1 = new SqlDataAdapter(command1);
            sda1.Fill(linkList);
            ViewBag.linkList = linkList;
            ViewBag.tableId = TableId;

            string PageList = "SELECT  [ModuleId],[ModuleCaption]  FROM [Genikos_Erp].[dbo].[Utl_Dynamic_Module]";
            SqlCommand command3 = new SqlCommand(PageList, connection);

            SqlDataAdapter sda3 = new SqlDataAdapter(command3);
            sda3.Fill(pagelist);
            ViewBag.Pagelist = pagelist;

            return View();
        }

        public ActionResult SetVariable(int? TableId, string SystemField, string Sort)
        {
            DataSet variableList = new DataSet();
            SqlConnection connection = new SqlConnection(connectionString);
            string VariableQuery = "Select * from utl_dynamic_table_column where IsShow='1' and TableId='" + TableId + "' " + SystemField + " order by " + Sort + "";
            SqlCommand command1 = new SqlCommand(VariableQuery, connection);

            SqlDataAdapter sda1 = new SqlDataAdapter(command1);
            sda1.Fill(variableList);
            ViewBag.VariableList = variableList;
            ViewBag.tableId = TableId;

            DataSet GlobalVaribleList = new DataSet();
            string globalQuery = "Select ParamId, ParamName from Utl_Dynamic_Global_Param order by SlNo";
            SqlCommand command = new SqlCommand(globalQuery, connection);

            SqlDataAdapter sda = new SqlDataAdapter(command);
            sda.Fill(GlobalVaribleList);
            ViewBag.GlobalVaribleList = GlobalVaribleList;

            return View();
        }

        public ActionResult SetDML(int? TableId, string IsSyatem)
        {
            DataSet dmlList = new DataSet();
            SqlConnection connection = new SqlConnection(connectionString);
            string DmlQuery = "Select ColumnId,ColumnDisplayName,Isnull(IsFvInsert,0)  as IsFvInsert, Isnull(IsFvUpdate,0) as IsFvUpdate,Isnull(IsFvForceEdit,0) as IsFvForceEdit," +
                "Isnull(IsModuleSelect, 0) as IsModuleSelect,Isnull(IsForeignTable, 0) as IsForeignTable,AggregateFunction from Utl_Dynamic_Table_Column" +
                " where IsShow = '1' and TableId = '" + TableId + "' " + IsSyatem + " order by SlNo";
            SqlCommand command1 = new SqlCommand(DmlQuery, connection);

            SqlDataAdapter sda1 = new SqlDataAdapter(command1);
            sda1.Fill(dmlList);
            ViewBag.DmlList = dmlList;
            ViewBag.tableId = TableId;
            return View();
        }



        public ActionResult SetLayout(int? TableId, string Sort)
        {
            DataSet layout = new DataSet();
            SqlConnection connection = new SqlConnection(connectionString);
            string LayoutQuery = "Select * from utl_dynamic_table_column where IsShow = '1' and TableId = '" + TableId + "' order by " + Sort + "";
            SqlCommand command1 = new SqlCommand(LayoutQuery, connection);
            
            SqlDataAdapter sda1 = new SqlDataAdapter(command1);
            sda1.Fill(layout);
            
            DataSet PanelList = new DataSet();
            string panelquery = "Select PanelId,PanelName,PanelCaption,SlNo from Utl_Dynamic_Table_Panel where TableId='" + TableId + "'";
            SqlCommand command = new SqlCommand(panelquery, connection);
            SqlDataAdapter sda = new SqlDataAdapter(command);
            sda.Fill(PanelList);
            ViewBag.PanelList = PanelList;

            DataSet DbfieldWidth = new DataSet();
            string fieldWithQuery = "SELECT max_length as ColSize,lower(name) as ColName  from sys.all_columns where  object_id=(SELECT object_id from sys.all_objects WHERE TYPE in ('U', 'V') and name = (SELECT TableName from UTL_DYNAMIC_TABLE where TableId = " + TableId + "))";
            SqlCommand cmd = new SqlCommand(fieldWithQuery, connection);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
            dataAdapter.Fill(DbfieldWidth);
            ViewBag.dbfieldWith = DbfieldWidth;

            ViewBag.Layout = layout;
            ViewBag.tableId = TableId;
            return View();
        }

        public ActionResult Setup(int? TableId, string Filter, string Sort)
        {
            DataSet dbfield = new DataSet();
            SqlConnection connection = new SqlConnection(connectionString);
            string DbFieldQuery = "Select * from utl_dynamic_table_column where IsShow='1' and  TableId=" + TableId + " " + Filter + " order by " + Sort + "";
            SqlCommand command2 = new SqlCommand(DbFieldQuery, connection);

            SqlDataAdapter sda2 = new SqlDataAdapter(command2);
            sda2.Fill(dbfield);
            ViewBag.Df = dbfield;
            ViewBag.tableId= TableId;

          

            DataSet viewColumn = new DataSet();
      
            string viewColumnQuery = "SELECT lower(C.TableId) as DataField,C.ColumnDisplayName + ' => ' + T.TableCaption as DataText FROM" +
                " Utl_Dynamic_Table_Column c INNER JOIN UTL_DYNAMIC_TABLE T ON C.TableId = T.TableId WHERE C.ColumnProperty = 'pkt' order by C.ColumnDisplayName";
            SqlCommand command = new SqlCommand(viewColumnQuery, connection);

            SqlDataAdapter sda = new SqlDataAdapter(command);
            sda.Fill(viewColumn);
            ViewBag.ViewColumn = viewColumn;
            return View();
        }

        public ActionResult SetTrigger(int TableId, int ModuleId)
        {
            DataSet dbTrigger = new DataSet();
            SqlConnection connection = new SqlConnection(connectionString);

            string QueryString = "SELECT [TriggerId],[TableId],[ModuleId],[ApplicationId],[TriggerTypeId],[TriggerName],[TriggerCaption],[IsAutoExecute],[SlNo],[DeveloperSyncId] FROM [dbo].[Utl_Dynamic_Table_Trigger] Where TableId = " + TableId + " Order By TriggerTypeId, SlNo";
            SqlCommand command = new SqlCommand(QueryString, connection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dbTrigger);

            ViewBag.tblTrigger = dbTrigger;
            ViewBag.TableId = TableId;
            ViewBag.ModuleId = ModuleId;

            return View();
        }

        public ActionResult EditTriggerById(int TriggerId, int TableId)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            DataSet EditTrigger = new DataSet();
            string EditTriggerList = "SELECT [TriggerId],[TableId],[ApplicationId],[TriggerTypeId],[TriggerName],[TriggerCaption],[IsAutoExecute],[SlNo] FROM [dbo].[Utl_Dynamic_Table_Trigger] where TriggerId = " + TriggerId + " ";

            SqlCommand command = new SqlCommand(EditTriggerList, connection);
            SqlDataAdapter sda = new SqlDataAdapter(command);
            sda.Fill(EditTrigger);
            ViewBag.EditTrigger = EditTrigger;
            ViewBag.TableId = TableId;
            ViewBag.TriggerId = TriggerId;
            return View();
        }

        public JsonResult UpdateTrigger(FormCollection collection)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            //int tableid = Convert.ToInt32(collection["tableid"]);
            int triggerid = Convert.ToInt32(collection["triggerid"]);
            int isAutoExecute = collection["IsAutoExecute"] == "" ? 0 : Convert.ToInt32(collection["IsAutoExecute"]);
            int triggerType = Convert.ToInt32(collection["TriggerTypeId"]);
            string ms = "";
            if (triggerType > 0 && collection["TriggerName"].ToString() != "" && collection["TriggerCaption"].ToString() != "")
            {
                string UpdatePanel = "UPDATE Utl_Dynamic_Table_Trigger SET  TriggerTypeId='" + collection["TriggerTypeId"] + "',TriggerCaption='" + 
                    collection["TriggerCaption"] + "', TriggerName='" + collection["TriggerName"] + "', IsAutoExecute ='" + isAutoExecute + "',SlNo ='" + 
                    collection["SlNo"] + "', Update_By ='"+ collection["update_by"] + "', Update_Date = " + collection["update_date"] + " WHERE TriggerId ='" + triggerid + "'";

                SqlCommand command = new SqlCommand(UpdatePanel, connection);
                connection.Open();
                int effectfield = command.ExecuteNonQuery();
                connection.Close();
                ms = effectfield > 0 ? "Updated Successfully!" : "Failed to Save!";
            }
            else
            {
                ms = triggerType < 1 ? ms + "'Trigger Type' must be selecte." : ms + "";
                ms = collection["TriggerName"].ToString() == "" ? ms + "'Trigger Name' must be fill up." : ms + "";
                ms = collection["TriggerCaption"].ToString() == "" ? ms + "'Trigger Caption' must be fill up." : ms + "";
            }

            return Json(new { success = true, message = ms }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetupAutocode(int? colId, string ColumnName,int? ModuleId, int? TableId)
        {
            ViewBag.ColumnName = ColumnName;
            ViewBag.ColumnId = colId;
            ViewBag.ModuleId = ModuleId;
            ViewBag.TableId = TableId;
            return View();
        }

        public JsonResult GetGridById(int? linkPageId)
        {
            List<Table> gridList = new List<Table>();
            SqlConnection connection = new SqlConnection(connectionString);
            string GridList = "Select TableId,TableCaption from Utl_Dynamic_Table where ModuleId='"+ linkPageId + "'";
            SqlCommand command3 = new SqlCommand(GridList, connection);
            connection.Open();
            SqlDataReader reader = command3.ExecuteReader();
           
            while (reader.Read())
            {
                long TableId = Convert.ToInt32(reader["TableId"]);
                string TableCaption = reader["TableCaption"].ToString();

                Table table = new Table(TableId, TableCaption);
                gridList.Add(table);
            }
            reader.Close();
            connection.Close();
            return Json(gridList);
        }

        public JsonResult GetColumnById(int? linkTabId)
        {
            List<Column> columnList = new List<Column>();
            SqlConnection connection = new SqlConnection(connectionString);
            string GridList = "Select ColumnId, ColumnDisplayName from Utl_Dynamic_Table_Column Where TableId='"+ linkTabId + "' Order by ColumnId";
            SqlCommand command3 = new SqlCommand(GridList, connection);
            connection.Open();
            SqlDataReader reader = command3.ExecuteReader();

            while (reader.Read())
            {
                long ColumnId = Convert.ToInt32(reader["ColumnId"]);
                string ColumnDisplayName = reader["ColumnDisplayName"].ToString();



                Column column = new Column(ColumnId, ColumnDisplayName);
                columnList.Add(column);
            }
            reader.Close();
            connection.Close();
            return Json(columnList);
        }

        public JsonResult GetColValByName(string TabName)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string ObjectQuery = "SELECT [object_id] as ObjectId  from sys.tables where name='" + TabName + "'";
            SqlCommand com = new SqlCommand(ObjectQuery, connection);
            connection.Open();
            var ObjId = com.ExecuteScalar();
            connection.Close();
            string query = "SELECT  name AS ColumnName,column_id as ColumnId from sys.all_columns where [object_id]='" + ObjId + "' ";
            SqlCommand command = new SqlCommand(query, connection);
           

            List<Column> columnList = new List<Column>();
         
           
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                long ColumnId = Convert.ToInt32(reader["ColumnId"]);
                string ColumnName = reader["ColumnName"].ToString();

                Column column = new Column(ColumnId, ColumnName);
                columnList.Add(column);
            }
            reader.Close();
            connection.Close();
            return Json(columnList);
        }
        public JsonResult DeleteSubModuleById(int? submoduleId)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            string DeleteSubmodule = "Delete from Utl_Dynamic_Module_Category   WHERE CategoryId ='" + submoduleId + "'";

            SqlCommand command = new SqlCommand(DeleteSubmodule, connection);
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
        public JsonResult DeletePageById(int? ModuleId)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            string Deletemodule = "Delete from Utl_Dynamic_Module   WHERE ModuleId ='" + ModuleId + "'";

            SqlCommand command = new SqlCommand(Deletemodule, connection);
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
        public JsonResult DeleteTableById(int? TableId)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string DeleteTable = "Delete from Utl_Dynamic_Table   WHERE TableId ='" + TableId + "'";
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
        public JsonResult DeleteColumnById(int? ColumnId)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string DeleteColummn = "Delete from Utl_Dynamic_Table_Column   WHERE ColumnId ='" + ColumnId + "'";
            SqlCommand command = new SqlCommand(DeleteColummn, connection);
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

        public ActionResult CreateDbField()
        {

            return View();
        }

        public ActionResult CreateLayout()
        {

            return View();
        }
        public ActionResult CreateDml()
        {

            return View();
        }
        public ActionResult CreateVariable()
        {

            return View();
        }
        public ActionResult CreateHyperlink()
        {

            return View();
        }
        public ActionResult CreateRole()
        {

            return View();
        }
        public ActionResult CreateFieldValidation()
        {

            return View();
        }
        public ActionResult CreateDdlSource()
        {

            return View();
        }
        public ActionResult CreateFieldMapping()
        {

            return View();
        }
        public ActionResult CreateExpression()
        {

            return View();
        }
        public ActionResult CreateFunction()
        {

            return View();
        }

        

        public ActionResult SelectDmlColumn(string DmlColumnId, int? rc, int? tabId,int? Moduleid)
        {
            var tableid = tabId;
            string TableId = "3";
            
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
            string dmlcol = DmlColumnId.ToLower();

                //str = "INSERT INTO utl_dynamic_table_column(tableid, columnname, selectcolumnname, foreignkeyname, controltype, columnproperty, columndisplayname) values(";
                //str = str + "@tableid@, @columnname[" + i + "]@, @selectcolumnname[" + i + "]@, @foreignkeyname[" + i + "]@, @controltype[" + i + "]@, @columnproperty[" + i + "]@, @columndisplayname[" + i + "]@)";

                 
            string pk_value = FtkData.GetNewPkValue(TableId);
            str = "INSERT INTO utl_dynamic_table_column(columnid,tableid, ModuleId, columnname,isshow,IsSystem,IsFvUpdate, IsFvInsert, SlNo,is_approved,Is_Active,is_deleted,is_cancel,work_follow_position)" +
                  " values('" + pk_value + "','" + tableid + "','" + Moduleid + "','" + dmlcol + "','1',0,'0','0','0','0','1','0','0','1')";


            var s = FtkData.ExecuteNonQueryCommand("1", TableId, "0", "0", str);


            ViewBag.RowCount = rc;
            ViewBag.Id = DmlColumnId;
            ViewBag.TableId = tableid;
            return View();
            

        }


        public ActionResult SelectPanelColumn(string DmlColumnId, int? rc, int? tabId, int? ColumnId,int? PanelId)
        {
            var tableid = tabId;
            string TableId = "3";
            
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

            SqlConnection con = new SqlConnection(connectionString);            
            string Pnlsl = "Select max(PnlColSl) as PanelSl from Utl_Dynamic_Table_Column Where TableId = "+ tableid + "";
            SqlCommand command3 = new SqlCommand(Pnlsl, con);
            
            con.Open();           
            var PanelSlNo = command3.ExecuteScalar() == DBNull.Value ? 0 : command3.ExecuteScalar();           
            con.Close();

            string str;
           
            string pk_value = FtkData.GetNewPkValue(TableId);
            str = "Update Utl_Dynamic_Table_Column Set PanelId='"+PanelId+ "', PnlColSl = "+ (Convert.ToDecimal(PanelSlNo)+1) + " where ColumnId='" + ColumnId + "'";

            var s = FtkData.ExecuteNonQueryCommand("1", TableId, "0", "0", str);

            DataSet panelColSl = new DataSet();
            string PanelColSlno = "Select PnlColSl, PnlColMerge from Utl_Dynamic_Table_Column Where ColumnId='" + ColumnId + "' ";
            
            SqlCommand command = new SqlCommand(PanelColSlno, con);
            
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            dataAdapter.Fill(panelColSl);
            ViewBag.PanelColSl = panelColSl;

            ViewBag.RowCount = rc;
            ViewBag.Id = DmlColumnId;
            ViewBag.TableId = tableid;
            ViewBag.ColumnId = ColumnId;
            ViewBag.PanelId = PanelId;
            return View();
        }
        public ActionResult RemoveRow(string DmlColumn, int? tableId)
        {
            SqlConnection con = new SqlConnection(connectionString);
            string deleteQuery = "Delete from utl_dynamic_table_column where columnname='" + DmlColumn + "' and TableId='"+tableId+"'";
            SqlCommand com = new SqlCommand(deleteQuery, con);
            con.Open();
               com.ExecuteNonQuery();
            con.Close();
            ViewBag.DmlColumn = DmlColumn;
            ViewBag.TableId = tableId;

            return View();
        }
        public ActionResult RemovePanelRow(string DmlColumn, int? tableId,int? ColumnId,int? PanelId)
        {
            SqlConnection con = new SqlConnection(connectionString);
            string updateQuery = "Update Utl_Dynamic_Table_Column Set PanelId=0, PnlColSl = 0, PnlColMerge = 0 where ColumnId='" + ColumnId+"'";
            SqlCommand com = new SqlCommand(updateQuery, con);
            con.Open();
            com.ExecuteNonQuery();
            con.Close();
            ViewBag.DmlColumn = DmlColumn;
            ViewBag.TableId = tableId;
            ViewBag.ColumnId = ColumnId;
            ViewBag.PanelId = PanelId;
            return View();
        }

    }
}