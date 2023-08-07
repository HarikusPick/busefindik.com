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

        [BindProperty, Required(ErrorMessage = "L�tfen tam isminizi girin")]
        public string FullName { get; set; } = "";

        
        [BindProperty, Required(ErrorMessage = "L�tfen epostan�z� girin"), EmailAddress]
        public string Email { get; set; } = "";

        [BindProperty]
        public string? Phone { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "L�tfen konu ba�l���n� doldurun")]
        public string Subject { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "Mesaj�n�z� girin")]
        [MinLength(5, ErrorMessage = "Mesaj 5 karakterden uzun olmal�")]
        [MaxLength(1024, ErrorMessage = "Mesaj 1024 karakterden k�sa olmal�")]
        public string Message { get; set; } = "";

        public string SuccessMessage { get; set; } = "";
        public string ErrorMessage { get; set; } = "";

        public void OnPost ()
        {
            if (!ModelState.IsValid)
            {
                // error
                ErrorMessage = "L�tfen gerekli alanlar� doldurun.";
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
            string emailSubject = "Mesaj�n�z hakk�nda";
            string emailMessage = "Say�n " + username + ",\n" +
                "Mesaj�n�z� ald�m. Benimle ileti�im kurdu�unuz i�in te�ekk�r ederim. \n" +
                "En k�sa s�re i�erisinde d�n�� sa�layca��m\n" +
                "Sevgilerimle\n\n" +
                "Mesaj�n�z :\n" + Message;

            EmailSender.SendEmail(Email, username, emailSubject, emailMessage).Wait();


            SuccessMessage = "Mesaj�n�z iletildi.";

            FullName = "";
            Email = "";
            Phone = "";
            Subject = "";
            Message = "";

            ModelState.Clear();
        }
    }
}
