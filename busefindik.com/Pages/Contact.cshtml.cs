using busefindik.com.MyHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Xml.Linq;
using busefindik.com.Pages.Admin.Images;

namespace busefindik.com.Pages
{
    public class ContactModel : PageModel
    {
        public List<ImageInfo> listcontactImages = new List<ImageInfo>();

        public void OnGet()
        {
            try
            {
                string connectionString = "Data Source=localhost\\SQLEXPRESS01;Initial Catalog=busefindik;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM images WHERE slider='true'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ImageInfo x = new ImageInfo();
                                x.Id = reader.GetInt32(0);
                                x.Title = reader.GetString(1);
                                x.Category = reader.GetString(2);
                                x.Description = reader.GetString(3);
                                x.Slider = reader.GetString(5);
                                x.ImageFileName = reader.GetString(4);
                                x.CreatedAt = reader.GetDateTime(6).ToString("dd/MM/yyyy");
                                x.InstagramLink = reader.GetString(7);
                                listcontactImages.Add(x);
                            }
                        }
                        ViewData["InstagramPhotoList"] = listcontactImages;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [BindProperty, Required(ErrorMessage = "Lütfen tam isminizi girin")]
        public string FullName { get; set; } = "";

        
        [BindProperty, Required(ErrorMessage = "Lütfen epostanýzý girin"), EmailAddress]
        public string Email { get; set; } = "";

        [BindProperty]
        public string? Phone { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "Lütfen konu baþlýðýný doldurun")]
        public string Subject { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "Mesajýnýzý girin")]
        [MinLength(5, ErrorMessage = "Mesaj 5 karakterden uzun olmalý")]
        [MaxLength(1024, ErrorMessage = "Mesaj 1024 karakterden kýsa olmalý")]
        public string Message { get; set; } = "";

        public string SuccessMessage { get; set; } = "";
        public string ErrorMessage { get; set; } = "";

        public void OnPost ()
        {
            if (!ModelState.IsValid)
            {
                // error
                ErrorMessage = "Lütfen gerekli alanlarý doldurun.";
                return;
            }
            if (Phone == null) Phone = "";

            try 
            {
                string connectionString = "Data Source=localhost\\SQLEXPRESS01;Initial Catalog=busefindik;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO messages " +
                        "(fullname, email, phone, subject, message) VALUES " +
                        "(@fullname, @email, @phone, @subject, @message);";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@fullname", FullName);
                        command.Parameters.AddWithValue("@email", Email);
                        command.Parameters.AddWithValue("@phone", Phone);
                        command.Parameters.AddWithValue("@subject", Subject);
                        command.Parameters.AddWithValue("@message", Message);

                        command.ExecuteNonQuery();
                    }
                }

            } 
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return;
            }

            // send confirmation email to the client
            string username = FullName;
            string emailSubject = "Mesajýnýz hakkýnda";
            string emailMessage = "Sayýn " + username + ",\n" +
                "Mesajýnýzý aldým. Benimle iletiþim kurduðunuz için teþekkür ederim. \n" +
                "En kýsa süre içerisinde dönüþ saðlaycaðým\n" +
                "Sevgilerimle\n\n" +
                "Mesajýnýz :\n" + Message;

            EmailSender.SendEmail(Email, username, emailSubject, emailMessage).Wait();


            SuccessMessage = "Mesajýnýz iletildi.";

            FullName = "";
            Email = "";
            Phone = "";
            Subject = "";
            Message = "";

            ModelState.Clear();
        }
    }
}
