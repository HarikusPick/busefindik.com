using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using busefindik.com.Pages.Admin.Images;
using System.Data.SqlClient;
using busefindik.com.Pages.Admin.Portfolio;

namespace busefindik.com.Pages
{
    public class AboutModel : PageModel
    {
        public List<ImageInfo> listcontactImages = new List<ImageInfo>();
        public List<PortfolioInfo> listyazi = new List<PortfolioInfo>();
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

                    string sql2 = "SELECT * FROM portfolio";
                    using (SqlCommand command = new SqlCommand(sql2, connection))
                    {
                        using(SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PortfolioInfo yaziInfo = new PortfolioInfo();
                                yaziInfo.Id = reader.GetInt32(0);
                                yaziInfo.Description = reader.GetString(1);
                                yaziInfo.CreatedAt = reader.GetDateTime(2).ToString("dd/MM/yyyy");
                                listyazi.Add(yaziInfo);
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
