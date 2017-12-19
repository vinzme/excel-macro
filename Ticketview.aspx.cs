using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Web.Mail;



namespace VMS_SUPPORT
{
    public partial class ticketview : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                if (Request.QueryString["str"] == "Search")
                {
                    GridView1.Visible = true;

                }
                else
                {
                    SqlDataReader rdr = null;
                    SqlConnection conn = VMS_SUPPORT.Connection.GetConnection();
                    String qry = "SELECT tblticket.Ticket_id, tblticket.Req_comments,us.Name[Representative],usb.Name[BackupRepresentative],us.uemail[Repemail],usb.uemail[bkRepemail],Status.Status, tblticket.Req_name,tblticket.Created,tblticket.Req_comments,tblticket.ETA_date,ref_piority.vmspiority[Piority],tblticket.Req_email,Request1.Request_type FROM [tblticket](nolock) Left join Status (nolock) on Status.Status_ID=tblticket.Status Left join users us (nolock) on us.Uid=tblticket.Representative Left Join ref_piority (nolock) on ref_piority.piority_id=tblticket.Piority Left join users usb (nolock) on usb.Uid = tblticket.BackupRepresentative left join Request1 on Request1.Request_id=tblticket.Req_category WHERE ([Ticket_id] = @Ticket_id)";
                    SqlCommand cmd = new SqlCommand(qry, conn);
                    cmd.Parameters.Add("@Ticket_id", SqlDbType.Int).Value = Request.QueryString["name"];
                    cmd.CommandText = qry;
                    try
                    {
                        conn.Open();
                        rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            String Ticketno = rdr["Ticket_id"].ToString();
                            String Description = rdr["Req_comments"].ToString();
                            String Representative = rdr["Representative"].ToString();
                            String BackupRepresentative = rdr["BackupRepresentative"].ToString();
                            String Status = rdr["Status"].ToString();
                            String Requestor = rdr["Req_name"].ToString();
                            String created = rdr["Created"].ToString();
                            String Comment = rdr["Req_comments"].ToString();
                            String eta = rdr["ETA_date"].ToString();
                            String Priority = rdr["Piority"].ToString();
                            String email = rdr["Req_email"].ToString();
                            String Repemail = rdr["Repemail"].ToString();
                            String bkRepemail = rdr["bkRepemail"].ToString();
                            String category = rdr["Request_type"].ToString();
                            MailMessage Msg = new MailMessage();
                            Msg.From = "vms-support@altisource.com";
                            Msg.Bcc = Repemail + ";" + bkRepemail;
                            Msg.To = email;
                            Msg.Subject = "VMS Change Request/Incident :: Ticket # " + Ticketno;
                            Msg.BodyFormat = MailFormat.Html;
                            Msg.Body = "Greetings <br />This is an automated response, please don’t reply back.<br />We are in receipt of your request. If you need to speak with VMS Support Representative, please call 297375 or contact VMS-Support@altisource.com for any queries." +
                                        "<br /><br />Ticket number              :   " + Ticketno +
                                        "<br />      Description                :   " + Comment +
                                        "<br />      VMS Support Representative :   " + Representative +
                                        " <br />     Ticket Status              :   " + Status +
                                        "<br />      Requestor Name             :   " + Requestor +
                                        "<br />      Received Date              :   " + created +
                                        "<br />      Estimated Completion Date  :   " + eta +
                                        "<br />      Priority                   :   " + Priority +
                                        "<br /><br />Warm Regards,<br />VMS Support Team ";

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

        protected void gridView_PageIndexChanging(object sender, EventArgs e)
        {
            
                GridView1.Visible = true;
                GridView1.DataSourceID = "Comment";
                GridView1.DataBind();
          
            
        }
    }
}

