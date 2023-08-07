using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace busefindik.com.Pages.Admin.Contact
{
    public class DetailsModel : PageModel
    {
        public MessageInfo messageInfo = new MessageInfo();
        public void OnGet()
        {
            string requestId = Request.Query["id"];

            try
            {
                string connectionString = "Data Source=localhost\\SQLEXPRESS01;Initial Catalog=busefindik;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM messages WHERE id=@id";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", requestId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                messageInfo.Id = reader.GetInt32(0);
                                messageInfo.FullName = reader.GetString(1);
                                messageInfo.Email = reader.GetString(2);
                                messageInfo.Phone = reader.GetString(3);
                                messageInfo.Subject = reader.GetString(4);
                                messageInfo.Message = reader.GetString(5);
                                messageInfo.CreatedAt = reader.GetDateTime(6).ToString("MM/dd/yyyy");
                            }
                            else
                            {
                                Response.Redirect("/Admin/Contact/Index");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Response.Redirect("/Admin/Contact/Index");
            }
        }
    }
}
