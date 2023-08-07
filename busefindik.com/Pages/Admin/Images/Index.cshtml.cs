using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace busefindik.com.Pages.Admin.Images
{
    public class IndexModel : PageModel
    {
            public List<ImageInfo> listimages = new List<ImageInfo>();
            
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
        public class ImageInfo
        {
            public int Id { get; set; }
            public string Title { get; set; } = "";
            public string Category { get; set; } = "";
            public string Description { get; set; } = "";
            public string ImageFileName { get; set; } = "";
            public string Slider { get; set; } = "";
            public string CreatedAt { get; set; } = "";
            public string? InstagramLink { get; set; } = "";
        }
}