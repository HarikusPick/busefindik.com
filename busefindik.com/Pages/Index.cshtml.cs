using busefindik.com.MyHelpers;
using busefindik.com.Pages.Admin.Images;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace busefindik.com.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        
        [BindProperty, Required(ErrorMessage = "Lütfen tam isminizi girin")]
        public string FullName { get; set; } = "";


        [BindProperty, Required(ErrorMessage = "Lütfen epostanızı girin"), EmailAddress]
        public string Email { get; set; } = "";

        [BindProperty]
        public string? Phone { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "Lütfen konu başlığını doldurun")]
        public string Subject { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "Mesajınızı girin")]
        [MinLength(5, ErrorMessage = "Mesaj 5 karakterden uzun olmalı")]
        [MaxLength(1024, ErrorMessage = "Mesaj 1024 karakterden kısa olmalı")]
        public string Message { get; set; } = "";

        public string SuccessMessage { get; set; } = "";
        public string ErrorMessage { get; set; } = "";

        public List<ImageInfo> listimages = new List<ImageInfo>();

        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                // error
                ErrorMessage = "Lütfen gerekli alanları doldurun.";
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
            string emailSubject = "Mesajınız hakkında";
            string emailMessage = "Sayın " + username + ",\n" +
                "Mesajınızı aldım. Benimle iletişim kurduğunuz için teşekkür ederim. \n" +
                "En kısa süre içerisinde dönüş sağlaycağım\n" +
                "Sevgilerimle\n\n" +
                "Mesajınız :\n" + Message;

            EmailSender.SendEmail(Email, username, emailSubject, emailMessage).Wait();


            SuccessMessage = "Mesajınız iletildi.";

            FullName = "";
            Email = "";
            Phone = "";
            Subject = "";
            Message = "";

            ModelState.Clear();
        }
        public void OnGet()
        {


            try
            {
                string connectionString = "Data Source=localhost\\SQLEXPRESS01;Initial Catalog=busefindik;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM images";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ImageInfo bookInfo = new ImageInfo();
                                bookInfo.Id = reader.GetInt32(0);
                                bookInfo.Title = reader.GetString(1);
                                bookInfo.Category = reader.GetString(2);
                                bookInfo.Description = reader.GetString(3);
                                bookInfo.Slider = reader.GetString(5);
                                bookInfo.ImageFileName = reader.GetString(4);
                                bookInfo.CreatedAt = reader.GetDateTime(6).ToString("dd/MM/yyyy");
                                listimages.Add(bookInfo);

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);

            }



        }
    }


}