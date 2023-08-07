using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;


namespace busefindik.com.Pages.Admin.Portfolio
{

    public class EditModel : PageModel
    {

        [BindProperty]
        public int Id { get; set; }


        [BindProperty]
        [Required(ErrorMessage = "açýklama zorunlu")]
        public string Description { get; set; } = "";

        public string errorMessage = "";
        public string successMessage = "";


        public void OnGet()
        {
            string requestId = Request.Query["id"];

            try
            {
                string connectionString = "Data Source=localhost\\SQLEXPRESS01;Initial Catalog=busefindik;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM portfolio WHERE id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", requestId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Id = reader.GetInt32(0);
                                Description = reader.GetString(1);
                            }
                            else
                            {
                                Response.Redirect("/Admin/Portfolio/Index");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Response.Redirect("/Admin/Portfolio/Index");
            }
        }

        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                errorMessage = "Data validation failed";
                return;
            }

            // update the book data in the database
            try
            {
                string connectionString = "Data Source=localhost\\SQLEXPRESS01;Initial Catalog=busefindik;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE portfolio SET description=@description WHERE id=@id;";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@description", Description);
                        command.Parameters.AddWithValue("@id", Id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            successMessage = "Data saved correctly";
            Response.Redirect("/Admin/Portfolio/Index");
        }
    }
}
