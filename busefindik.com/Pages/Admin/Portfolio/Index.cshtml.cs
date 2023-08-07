using busefindik.com.Pages.Admin.Images;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace busefindik.com.Pages.Admin.Portfolio
{
    public class IndexModel : PageModel
    {
        public List<PortfolioInfo> listyazi = new List<PortfolioInfo>();
        public void OnGet()
        {
            try
            {
                string connectionString = "Data Source=localhost\\SQLEXPRESS01;Initial Catalog=busefindik;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM portfolio";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
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
    public class PortfolioInfo
    {
        public int Id { get; set; }
        public string Description { get; set; } = "";
        public string CreatedAt { get; set; } = "";
    }
}
