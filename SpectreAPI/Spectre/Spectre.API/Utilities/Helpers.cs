using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Spectre.API.Utilities
{
    public class Helpers
    {
        public static string GenerateRandom(int Length)
        {
            const string LO = "abcdefghijklmnopqrstuvwxyz";
            const string UPP = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string NUM = "1234567890";
            const string CHAR = ".!#&";
            string res = "";
            Random rnd = new Random();
            while (0 < Length)
            {
                res += LO[rnd.Next(LO.Length)].ToString() + UPP[rnd.Next(UPP.Length)].ToString() + NUM[rnd.Next(NUM.Length)].ToString() + CHAR[rnd.Next(CHAR.Length)].ToString();
                Length = Length - 4;
            }
            return res.ToString();
        }

        public static bool SendEmail(string To, string Body, string From, string Subject, string SMTPUserName, string SMPTPassword, string HostServer, int Port)
        {
            SmtpClient client = new SmtpClient(HostServer, Port);
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(SMTPUserName, SMPTPassword);
            //i added the line below
            client.EnableSsl = true;

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(From);
            mailMessage.To.Add(To);
            mailMessage.Body = Body;
            mailMessage.Subject = Subject;
            mailMessage.IsBodyHtml = true;


            /* POSSIBLE ALTERNATIVE SOLUTION BELOW */

            // PACKAGE NAME: https://www.nuget.org/packages/AIM

            //public static void SendMail()
            //{
            //    var mailMessage = new MimeMailMessage();
            //    mailMessage.Subject = "test mail";
            //    mailMessage.Body = "hi dude!";
            //    mailMessage.Sender = new MimeMailAddress("you@gmail.com", "your name");
            //    mailMessage.To.Add(new MimeMailAddress("yourfriend@gmail.com", "your friendd's name"));
            //    // You can add CC and BCC list using the same way
            //    mailMessage.Attachments.Add(new MimeAttachment("your file address"));

            //    //Mail Sender (Smtp Client)

            //    var emailer = new SmtpSocketClient();
            //    emailer.Host = "your mail server address";
            //    emailer.Port = 465;
            //    emailer.SslType = SslMode.Ssl;
            //    emailer.User = "mail sever user name";
            //    emailer.Password = "mail sever password";
            //    emailer.AuthenticationMode = AuthenticationType.Base64;
            //    // The authentication types depends on your server, it can be plain, base 64 or none. 
            //    //if you do not need user name and password means you are using default credentials 
            //    // In this case, your authentication type is none            
            //    emailer.MailMessage = mailMessage;
            //    emailer.OnMailSent += new SendCompletedEventHandler(OnMailSent);
            //    emailer.SendMessageAsync();
            //}

            //// A simple call back function:
            //private void OnMailSent(object sender, AsyncCompletedEventArgs asynccompletedeventargs)
            //{
            //    if (e.UserState != null)
            //        Console.Out.WriteLine(e.UserState.ToString());
            //    if (e.Error != null)
            //    {
            //        MessageBox.Show(e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //    else if (!e.Cancelled)
            //    {
            //        MessageBox.Show("Send successfull!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    }
            //}

            try
            {
                client.Send(mailMessage);
                //client.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public static string DecodeBTOA(string CipherText)
        {

            byte[] data = System.Convert.FromBase64String(CipherText);
            return System.Text.ASCIIEncoding.ASCII.GetString(data);

        }

        public static bool CreateTempTable(string TableName, DataTable dtTable, string ConnectionString)
        {
            try
            {

                using (SqlConnection Con = new SqlConnection(ConnectionString))
                {
                    Con.Open();
                    string TableColumns = "";
                    foreach (DataColumn dc in dtTable.Columns)
                        TableColumns += "[" + dc.ColumnName + "] " + SqlTypeOf(dc.DataType) + ",";
                    if (TableColumns != "") TableColumns = TableColumns.Substring(0, TableColumns.Length - 1);

                    SqlCommand CMDManipulate = new SqlCommand("CreatTmpTable", Con);
                    CMDManipulate.Parameters.AddWithValue("@TableName", TableName);
                    CMDManipulate.Parameters.AddWithValue("@Columns", TableColumns);
                    CMDManipulate.CommandType = CommandType.StoredProcedure;

                    CMDManipulate.ExecuteNonQuery();
                    Con.Close();
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        private static string SqlTypeOf(Type type)
        {
            switch (type.Name.ToString())
            {
                case "Int32":
                    return "Int";

                case "String":
                    return "varchar(MAX)";

                case "DateTime":
                    return "datetime";
                case "Decimal":
                    return "numeric(17,3)";
                case "Byte":
                    return "bit";
                case "Byte[]":
                    return "timestamp";
                case "Boolean":
                    return "bit";
                default:
                    return "varchar(MAX)";
            }

        }


        public static bool SendEmailToMany(List<string> To, List<string> CC, string Body, string From, string Subject, string SMTPUserName, string SMPTPassword, string HostServer, int Port)
        {
            SmtpClient client = new SmtpClient(HostServer, Port);
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(SMTPUserName, SMPTPassword);

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(From);
            foreach (var email in To)
            {
                mailMessage.To.Add(email);
            }
            foreach (var email in CC)
            {
                mailMessage.CC.Add(email);
            }
            mailMessage.Body = Body;
            mailMessage.Subject = Subject;
            mailMessage.IsBodyHtml = true;

            try
            {
                client.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
