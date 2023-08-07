using busefindik.com.MyHelpers;
using busefindik.com.Pages.Admin.Images;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace busefindik.com.Pages
{
    public class PortfolioModel : PageModel
    {
        public List<ImageInfo> listimages = new List<ImageInfo>();
        public List<string> categories = new List<string>();

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
                    string kategorisql = "SELECT DISTINCT category FROM images;";
                    using (SqlCommand command = new SqlCommand(kategorisql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string category = reader.GetString(0);
                                categories.Add(category);
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
