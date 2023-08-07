using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SendGrid;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace busefindik.com.Pages.Admin.Portfolio
{
    public class CreateModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "açýklama alaný zorunlu")]
        [MinLength(1, ErrorMessage = "açýklama 1 karakterden uzun olmalý")]
        public string Description { get; set; } = "";


        public string errorMessage = "";
        public string successMessage = "";

        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                errorMessage = "data validation error";
                return;
            }
            if (Description == null) Description = "";
            try
            {
                string connectionString = "Data Source=localhost\\SQLEXPRESS01;Initial Catalog=busefindik;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO portfolio (description) VALUES (@description);";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@description", Description);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
            successMessage = "baþarýlý";
            Response.Redirect("/");
        }
    }
}


