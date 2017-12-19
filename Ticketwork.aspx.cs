using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Web.Mail;
using System.Globalization;

namespace VMS_SUPPORT
{
    public partial class Ticketwork : System.Web.UI.Page
    {

        public static string requesttype, category;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string n = Request.QueryString["name"];
                SqlConnection conn = VMS_SUPPORT.Connection.GetConnection();
                conn.Open();
                SqlDataReader rdr, rdrtyp, Reader, rdrstat, rdrVis;
                txtcat.Items.Clear();
                String qrystat = "select * from Status";
                SqlCommand cmdstat = new SqlCommand(qrystat, conn);
                cmdstat.CommandText = qrystat;
                rdrstat = cmdstat.ExecuteReader();
                txtstatus.DataSource = rdrstat;
                txtstatus.DataValueField = "Status_ID";
                txtstatus.DataTextField = "Status";
                txtstatus.DataBind();
                rdrstat.Close();
                Int32 Ticket = Convert.ToInt32(n);
                String qry = "select tblticket.Ticket_id,tblticket.Created,tblticket.Req_name,tblticket.Req_netid,tblticket.Req_email,tblticket.Req_ext,tblticket.Req_dept,tblticket.Req_approved,tblticket.Req_category,tblticket.Req_type,tblticket.Req_comments,tblticket.ETA_date,tblticket.Piority,us.Name[Representative],usb.Name[backupRepresentative],tblticket.Status,tblticket.Follow_up_date,tblticket.Representative[repid],tblticket.BackupRepresentative[bkrepid] from tblticket Left join users us on us.Uid=tblticket.Representative  Left join users usb on usb.Uid = tblticket.BackupRepresentative where Ticket_id=@Ticketno";
                SqlCommand SLCOM = new SqlCommand(qry, conn);
                SLCOM.Parameters.Add("@Ticketno", SqlDbType.Int).Value = Ticket;
                try
                {

                    Reader = SLCOM.ExecuteReader();
                    while (Reader.Read())
                    {
                        txtticket.Text = Reader["Ticket_id"].ToString();
                        Txtcreated.Text = Reader["Created"].ToString();
                        txtReq_name.Text = Reader["Req_name"].ToString();
                        txtReqnetid.Text = Reader["Req_netid"].ToString();
                        txtReq_email.Text = Reader["Req_email"].ToString();
                        txtReq_ext.Text = Reader["Req_ext"].ToString();
                        txtReq_dept.Text = Reader["Req_dept"].ToString();
                        txtReq_approved.Text = Reader["Req_approved"].ToString();
                        category = Reader["Req_category"].ToString();
                        requesttype = Reader["Req_type"].ToString();
                        txtReq_comments.Text = Reader["Req_comments"].ToString();
                        lbletadate.Text = Reader["ETA_date"].ToString();
                        txtpiror.SelectedValue = Reader["Piority"].ToString();
                        txtRepresentative.Text = Reader["Representative"].ToString();
                        txtbkRepresentative.Text = Reader["BackupRepresentative"].ToString();
                        txtstatus.SelectedValue = Reader["Status"].ToString();
                        txtDate.Text = Reader["Follow_up_date"].ToString();
                        rep.Text = Reader["repid"].ToString();
                        bkrep.Text = Reader["bkrepid"].ToString();
                    }

                    if (txtstatus.SelectedValue == "7")
                    {
                        txtcat.Enabled = false;
                        txtReq_approved.Enabled = false;
                        txttyp.Enabled = false;
                        txtstatus.Enabled = false;
                        Calenbt.Enabled = false;
                        btnsubmit0.Enabled = false;
                    }
                    Reader.Close();
                    txtcat.Items.Clear();
                    String qryCat = "select * from Category";
                    SqlCommand cmdc = new SqlCommand(qryCat, conn);
                    cmdc.CommandText = qryCat;
                    rdr = cmdc.ExecuteReader();
                    txtcat.DataSource = rdr;
                    txtcat.DataValueField = "Category_id";
                    txtcat.DataTextField = "Category_Name";
                    txtcat.DataBind();
                    txtcat.SelectedValue = category;
                    rdr.Close();

                    String id = txtcat.SelectedValue;
                    txttyp.Items.Clear();
                    String qury = "select * from Request1 where Request_category = @Req_category";
                    SqlCommand comd = new SqlCommand(qury, conn);
                    comd.Parameters.Add("@Req_category", SqlDbType.Int).Value = id;
                    comd.CommandText = qury;
                    rdrtyp = comd.ExecuteReader();
                    txttyp.DataSource = rdrtyp;
                    txttyp.DataValueField = "Request_id";
                    txttyp.DataTextField = "Request_type";
                    txttyp.DataBind();
                    rdrtyp.Close();

                    Visble.Items.Clear();
                    String Vis = "select * from Visible";
                    SqlCommand Viscomd = new SqlCommand(Vis, conn);
                    Viscomd.Parameters.Add("@Req_category", SqlDbType.Int).Value = id;
                    Viscomd.CommandText = Vis;
                    rdrVis = Viscomd.ExecuteReader();
                    Visble.DataSource = rdrVis;
                    Visble.DataValueField = "Visible_id";
                    Visble.DataTextField = "Visible_Name";
                    Visble.DataBind();
                    rdrVis.Close();

                    txttyp.SelectedValue = requesttype;
                    GridView1.Visible = true;
                    String query = "Select File_id,File_Name from Files where Ticket_id=@Ticketno order by File_id desc";
                    SqlDataAdapter da = new SqlDataAdapter();
                    DataSet ds = new DataSet();
                    SqlCommand CMD = new SqlCommand(query, conn);
                    CMD.Parameters.Add("@Ticketno", SqlDbType.Int).Value = Ticket;
                    da.SelectCommand = CMD;
                    da.Fill(ds, "Files");
                    GridView1.DataSource = ds.Tables[0];
                    GridView1.DataBind();
                    GridView2.Visible = true;
                    String Comment = "Select Comment,Update_by,Comment_date,Visible.Visible_Name from Comment join Visible on Visible.Visible_id= Comment.Visible_id where Ticket_id=@Ticketno order by 1 desc";
                    SqlDataAdapter Commentda = new SqlDataAdapter();
                    DataSet Commentds = new DataSet();
                    SqlCommand CommentCMD = new SqlCommand(Comment, conn);
                    CommentCMD.Parameters.Add("@Ticketno", SqlDbType.Int).Value = Ticket;
                    Commentda.SelectCommand = CommentCMD;
                    Commentda.Fill(Commentds, "Comment");
                    GridView2.DataSource = Commentds.Tables[0];
                    GridView2.DataBind();
                }
                finally
                {

                    // close the connection
                    if (conn != null)
                    {
                        conn.Close();
                    }
                }

            }
        }

        protected void GridView1_SelectedIndexChanged1(object sender, EventArgs e)
        {
            SqlConnection conn = VMS_SUPPORT.Connection.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("select File_Name,File_type,data from Files where File_id=@id", conn);
            cmd.Parameters.AddWithValue("id", GridView1.SelectedRow.Cells[1].Text);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                string path = Server.MapPath("~/Files/") + dr["data"].ToString(); //get file object as FileInfo  
                Response.Clear();
                System.IO.FileInfo file = new System.IO.FileInfo(path); //-- if the file exists on the server  
                // to open file prompt Box open or Save file
                Response.AddHeader("Content-Disposition", "attachment; filename=" + dr["File_Name"].ToString());
                Response.AddHeader("Content-Length", file.Length.ToString());
                Response.ContentType = "application/vnd.ms-excel";
                Response.WriteFile(path);//File open
                Response.Flush();
                //context.Response.Close();               
                Response.End();
            }


        }

        protected void btnsubmit_Click(object sender, EventArgs e)
        {
            if (comments.Text == "")
            {

                string myStringVariable = "Please enter comments to insert";
                ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('" + myStringVariable + "');", true);
            }
            else
            {
                  SqlConnection conn = VMS_SUPPORT.Connection.GetConnection();
                    
                    if (Visble.SelectedValue == "1")
                    {
                        SqlDataReader rdr = null;
                        String eqry = "SELECT tblticket.Ticket_id, us.uemail[Repemail],usb.uemail[bkRepemail],tblticket.Req_email FROM [tblticket](nolock) Left join users us (nolock) on us.Uid=tblticket.Representative  Left join users usb (nolock) on usb.Uid = tblticket.BackupRepresentative  WHERE ([Ticket_id] = @Ticket_id)";
                        SqlCommand cmd = new SqlCommand(eqry, conn);
                        cmd.Parameters.Add("@Ticket_id", SqlDbType.Int).Value = Request.QueryString["name"];
                        cmd.CommandText = eqry;
                        try
                        {                            
                            conn.Open();
                            rdr = cmd.ExecuteReader();
                            while (rdr.Read())
                            {
                                String Ticketno = rdr["Ticket_id"].ToString();                                
                                String email = rdr["Req_email"].ToString();
                                String Repemail = rdr["Repemail"].ToString();
                                String bkRepemail = rdr["bkRepemail"].ToString();                               
                                MailMessage Msg = new MailMessage();
                                Msg.From = "vms-support@altisource.com";
                                Msg.Cc = bkRepemail + ";" + Repemail;
                                Msg.To = email;
                                Msg.Subject = "VMS Change Request/Incident :: Ticket # " + Ticketno;
                                Msg.BodyFormat = MailFormat.Html;
                                Msg.Body = "Good day, <br /><br /> Your ticket has been reviewed and updated by VMS Support Representative. To know more on the status of your ticket, please click on the below URL." +
                                            "<br /><br /> <a href= http://vmssupport:50/ticketview.aspx?str=Search&name=" + Ticketno + " >Check Status</a>" + 
                                            "<br /><br /> Thank You,<br /><br />VMS Support"; 
                                SmtpMail.SmtpServer = System.Configuration.ConfigurationManager.AppSettings["SMTPServer"].ToString();
                                SmtpMail.Send(Msg);
                                Msg = null;
                            }

                        }
                        finally
                        {
                            if (rdr != null)
                            {
                                rdr.Close();// close the reader
                            }

                            if (conn != null)
                            {
                                conn.Close();// close the connection
                            }
                        }
                    }
               
                string s = HttpContext.Current.User.Identity.Name;
                string username = s.Substring(s.IndexOf(@"\") + 1);
                DateTime now = DateTime.Now;
                String n = txtticket.Text;
                Int32 Ticket = Convert.ToInt32(n);                
                string commentin = " INSERT INTO Comment (Comment,Ticket_id,Update_by,Comment_date,Visible_id) VALUES (@Comment,@Ticket_id,@username,@commdate,@Visible_id) ";
                SqlCommand com = new SqlCommand(commentin, conn);
                com.CommandText = commentin;
                com.Parameters.Add("@Comment", SqlDbType.NVarChar).Value = comments.Text;
                com.Parameters.Add("@Ticket_id", SqlDbType.Int).Value = Ticket;
                com.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
                com.Parameters.Add("@commdate", SqlDbType.DateTime).Value = now;
                com.Parameters.Add("@Visible_id", SqlDbType.Int).Value = Visble.SelectedValue;
                conn.Open();
                com.ExecuteNonQuery();//excute query
                conn.Close();
                GridView2.Visible = true;
                String Comment = "Select Comment,Update_by,Comment_date,Visible.Visible_Name from Comment join Visible on Visible.Visible_id= Comment.Visible_id where Ticket_id=@Ticketno order by 1 desc";
                SqlDataAdapter Commentda = new SqlDataAdapter();
                DataSet Commentds = new DataSet();
                SqlCommand CommentCMD = new SqlCommand(Comment, conn);
                CommentCMD.Parameters.Add("@Ticketno", SqlDbType.Int).Value = Ticket;
                Commentda.SelectCommand = CommentCMD;
                Commentda.Fill(Commentds, "Comment");
                GridView2.DataSource = Commentds.Tables[0];
                GridView2.DataBind();
                GridView2.Visible = true;
                comments.Text = "";
                fileerr.Text = "Comment Save Successfull";
            }
        }

        protected void btnsubmit0_Click(object sender, EventArgs e)
        {
            //Save Button action 
            string s = HttpContext.Current.User.Identity.Name;
            string username = s.Substring(s.IndexOf(@"\") + 1);
            HttpFileCollection hfc = HttpContext.Current.Request.Files;
            HttpPostedFile hpf;
            if (txttyp.SelectedItem.Text == "Select" || comments.Text == "") //check if request type is selected or if comment is provided for save..
            {
                string myStringVariable = "Please select request type and enter comments to save";
                ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('" + myStringVariable + "');", true);
            }
            else
            {
                string idn = txtticket.Text;
                int id = Convert.ToInt32(idn);//Ticket#
                String catid = txtcat.SelectedValue;
                int cateid = Convert.ToInt32(catid);//category
                String reid = txttyp.SelectedValue;
                int Typid = Convert.ToInt32(reid);//Type
                String stat = txtstatus.SelectedValue;
                int statid = Convert.ToInt32(stat);//Status
                String approved = txtReq_approved.Text;
                SqlConnection conn = VMS_SUPPORT.Connection.GetConnection();
                SqlCommand upd = conn.CreateCommand();
                if (txtstatus.SelectedValue == "7")
                {
                    txtcat.Enabled = false;
                    txtReq_approved.Enabled = false;
                    txttyp.Enabled = false;
                    txtstatus.Enabled = false;
                    Calenbt.Enabled = false;
                    btnsubmit0.Enabled = false;
                    upd.CommandText = "Update tblticket set Req_category=@Req_category,Req_type=@Req_type,Req_approved=@Req_approved,Follow_up_date=@date,Status=@Status,ETA_date=@ETA_date,Representative=@Representative ,BackupRepresentative=@BackupRepresentative,Closed=@Closed,Last_updated_by=@Last_updated_by where Ticket_id=@ticket_id"; //Update query
                    upd.Parameters.AddWithValue("@Closed",  DateTime.Now);
                }
                else
                {
                    upd.CommandText = "Update tblticket set Req_category=@Req_category,Req_type=@Req_type,Req_approved=@Req_approved,Follow_up_date=@date,Status=@Status,ETA_date=@ETA_date,Representative=@Representative ,BackupRepresentative=@BackupRepresentative, Last_updated_by=@Last_updated_by where Ticket_id=@ticket_id"; //Update query
                }
                upd.Parameters.AddWithValue("@Ticket_id", id);//Ticket_id
                upd.Parameters.AddWithValue("@Req_category", cateid);//category
                upd.Parameters.AddWithValue("@Req_type", Typid);//Type
                upd.Parameters.AddWithValue("@Req_approved", approved);
                upd.Parameters.Add("@date", SqlDbType.NVarChar).Value = txtDate.Text;//Follow up date
                upd.Parameters.AddWithValue("@Status", statid);//Status
                upd.Parameters.Add("@ETA_date", SqlDbType.DateTime).Value = lbletadate.Text;
                upd.Parameters.Add("@Representative", SqlDbType.Int).Value = rep.Text;
                upd.Parameters.AddWithValue("@BackupRepresentative", SqlDbType.Int).Value = bkrep.Text;
                upd.Parameters.Add("@Last_updated_by", SqlDbType.NVarChar).Value = username;//Follow up date
                conn.Open();
                upd.ExecuteNonQuery();//excute query
                conn.Close();
                for (int i = 0; i < hfc.Count; i++)
                {
                    hpf = hfc[i];
                    if (hpf.ContentLength > 0)
                    {                       
                        string filePath = hpf.FileName;
                        string filename = System.IO.Path.GetFileName(filePath);
                        string ext = System.IO.Path.GetExtension(filename);
                        String path = id + DateTime.Now.ToString("_ddMMyyHHmmssfff_") + filename;
                        hpf.SaveAs(Server.MapPath("~/Files/") + path);                        
                        SqlCommand cone = conn.CreateCommand();
                        cone.CommandText = "INSERT INTO Files (File_Name,File_type,data,Ticket_id) VALUES (@Name, @type, @Data,@Ticket_id)"; //insert query
                        cone.Parameters.AddWithValue("@Name", filename);//name of the file
                        cone.Parameters.AddWithValue("@type", ext);//file extion
                        cone.Parameters.AddWithValue("@Data", path);//file Path
                        cone.Parameters.AddWithValue("@Ticket_id", id);//Ticket_id
                        conn.Open();
                        cone.ExecuteNonQuery();//excute query
                        conn.Close();
                        fileerr.ForeColor = System.Drawing.Color.Green;
                        fileerr.Text = "File Uploaded Successfully"; //if insert was success full.                        

                    }
                    fileerr.Text = "Save successfull";
                    GridView1.Visible = true;
                    String query = "Select File_id,File_Name from Files where Ticket_id=@Ticketno order by File_id desc";
                    SqlDataAdapter da = new SqlDataAdapter();
                    DataSet ds = new DataSet();
                    SqlCommand CMD = new SqlCommand(query, conn);
                    CMD.Parameters.Add("@Ticketno", SqlDbType.Int).Value = id;
                    da.SelectCommand = CMD;
                    da.Fill(ds, "Files");
                    GridView1.DataSource = ds.Tables[0];
                    GridView1.DataBind();                    
                }
              
                DateTime now = DateTime.Now;
                String n = txtticket.Text;
                Int32 Ticket = Convert.ToInt32(n);
                string commentin = " INSERT INTO Comment (Comment,Ticket_id,Update_by,Comment_date,Visible_id) VALUES (@Comment,@Ticket_id,@username,@commdate,@Visible_id) ";
                SqlCommand com = new SqlCommand(commentin, conn);
                com.CommandText = commentin;
                com.Parameters.Add("@Comment", SqlDbType.NVarChar).Value = comments.Text;
                com.Parameters.Add("@Ticket_id", SqlDbType.Int).Value = Ticket;
                com.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
                com.Parameters.Add("@commdate", SqlDbType.DateTime).Value = now;
                com.Parameters.Add("@Visible_id", SqlDbType.Int).Value = Visble.SelectedValue;
                conn.Open();
                com.ExecuteNonQuery();//excute query
                conn.Close();
                GridView2.Visible = true;
                String Comment = "Select Comment,Update_by,Comment_date,Visible.Visible_Name from Comment join Visible on Visible.Visible_id= Comment.Visible_id where Ticket_id=@Ticketno order by 1 desc";
                SqlDataAdapter Commentda = new SqlDataAdapter();
                DataSet Commentds = new DataSet();
                SqlCommand CommentCMD = new SqlCommand(Comment, conn);
                CommentCMD.Parameters.Add("@Ticketno", SqlDbType.Int).Value = Ticket;
                Commentda.SelectCommand = CommentCMD;
                Commentda.Fill(Commentds, "Comment");
                GridView2.DataSource = Commentds.Tables[0];
                GridView2.DataBind();
                GridView2.Visible = true;
                comments.Text = "";
               
                if (Visble.SelectedValue == "1")
                {
                    SqlDataReader rdr = null;
                    String eqry = "SELECT tblticket.Ticket_id, us.uemail[Repemail],usb.uemail[bkRepemail],tblticket.Req_email FROM [tblticket](nolock) Left join users us (nolock) on us.Uid=tblticket.Representative  Left join users usb (nolock) on usb.Uid = tblticket.BackupRepresentative  WHERE ([Ticket_id] = @Ticket_id)";
                    SqlCommand cmd = new SqlCommand(eqry, conn);
                    cmd.Parameters.Add("@Ticket_id", SqlDbType.Int).Value = Request.QueryString["name"];
                    cmd.CommandText = eqry;
                    try
                    {
                        conn.Open();
                        rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            String Ticketno = rdr["Ticket_id"].ToString();
                            String email = rdr["Req_email"].ToString();
                            String Repemail = rdr["Repemail"].ToString();
                            String bkRepemail = rdr["bkRepemail"].ToString();
                            MailMessage Msg = new MailMessage();
                            Msg.From = "vms-support@altisource.com";
                            Msg.Cc = bkRepemail + ";" + Repemail;
                            Msg.To = email;
                            Msg.Subject = "VMS Change Request/Incident :: Ticket # " + Ticketno;
                            Msg.BodyFormat = MailFormat.Html;
                            Msg.Body = "Good day, <br /><br /> Your ticket has been reviewed and updated by VMS Support Representative. To know more on the status of your ticket, please click on the below URL." +
                                        "<br /><br /> <a href= http://vmssupport:50/ticketview.aspx?str=Search&name=" + Ticketno + " >Check Status</a>" +
                                        "<br /><br /> Thank You,<br /><br />VMS Support";
                            SmtpMail.SmtpServer = System.Configuration.ConfigurationManager.AppSettings["SMTPServer"].ToString();
                            SmtpMail.Send(Msg);
                            Msg = null;
                        }

                    }
                    finally
                    {
                        if (rdr != null)
                        {
                            rdr.Close();// close the reader
                        }

                        if (conn != null)
                        {
                            conn.Close();// close the connection
                        }
                    }
                }

            }
        }

        protected void Back_Click(object sender, EventArgs e)
        {
            Response.Redirect("Workflow.aspx?");
        }

        protected void txtcat_SelectedIndexChanged(object sender, EventArgs e)
        {
            SqlConnection conn = VMS_SUPPORT.Connection.GetConnection();
            String id = txtcat.SelectedValue;            
            SqlDataReader reader;
            String qury = "select * from Request1 where Request_category = @Req_category";
            SqlCommand comd = new SqlCommand(qury, conn);
            comd.Parameters.Add("@Req_category", SqlDbType.Int).Value = id;
            comd.CommandText = qury;
            conn.Open();
            reader = comd.ExecuteReader();
            txttyp.Items.Clear();
            txttyp.Items.Insert(0, "Select");
            txttyp.DataSource = reader;
            txttyp.DataValueField = "Request_id";
            txttyp.DataTextField = "Request_type";
            txttyp.DataBind();
            conn.Close();
        }

        protected void Calenbt_Click(object sender, ImageClickEventArgs e)
        {
            DateTime date = new DateTime();
            //Flip the visibility attribute
            this.cal1.Visible = !(this.cal1.Visible);
            //If the calendar is visible try assigning the date from the textbox
            if (this.cal1.Visible)
            {
                //If the Conversion was successfull assign the textbox's date
                if (DateTime.TryParse(txtDate.Text, out date))
                {
                    cal1.SelectedDate = date;
                }
                this.cal1.Attributes.Add("style", "POSITION: absolute");
            }

        }

        protected void cal1_SelectionChanged(object sender, EventArgs e)
        {
            txtDate.Text = this.cal1.SelectedDate.Date.ToString();
            this.cal1.Visible = false;
        }

        protected void txttyp_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (txttyp.SelectedItem.Text == "Select")
            {

            }
            else
            {
                SqlDataReader rdr = null;
                String ReportName = txttyp.SelectedValue;
                SqlConnection conn = VMS_SUPPORT.Connection.GetConnection();
                SqlCommand SLCOM = new SqlCommand("[dbo].[SLA]", conn);
                SLCOM.CommandType = System.Data.CommandType.StoredProcedure;
                SLCOM.Parameters.Add("@category", SqlDbType.Int).Value = txttyp.SelectedValue;
                conn.Open();
                rdr = SLCOM.ExecuteReader();
                while (rdr.Read())
                {
                    string SLAVA = rdr["SLA"].ToString();
                    int SLAVAL = Convert.ToInt32(SLAVA);
                    rep.Text = rdr["Representative"].ToString();
                    bkrep.Text = rdr["BackUpRepresentative"].ToString();
                    DateTime fromDate = Convert.ToDateTime(Txtcreated.Text);
                    DateTime toDate = fromDate.AddDays(SLAVAL);
                    var weekendDayCount = 0;

                    while (fromDate < toDate)
                    {
                        fromDate = fromDate.AddDays(1);
                        if (fromDate.DayOfWeek == DayOfWeek.Saturday || fromDate.DayOfWeek == DayOfWeek.Sunday)
                        {
                            ++weekendDayCount;
                        }
                    }

                    DateTime ETA = toDate.AddDays(weekendDayCount);

                    if (ETA.DayOfWeek == DayOfWeek.Saturday)
                    {
                        ETA = ETA.AddDays(2);
                    }
                    else
                        if (ETA.DayOfWeek == DayOfWeek.Sunday)
                        {
                            ETA = ETA.AddDays(1);
                        }

                    lbletadate.Text = ETA.ToString();
                }
            }
            
        }
    }
}