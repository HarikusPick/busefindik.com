using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace busefindik.com.Pages.Admin.Images
{

    public class ImagesModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "kategori zorunlu")]
        public string Category { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "a��klama alan� zorunlu")]
        [MinLength(1, ErrorMessage = "a��klama 1 karakterden uzun olmal�")]
        [MaxLength(1024, ErrorMessage = "a��klama 1024 karakterden k�sa olmal�")]
        public string Description { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "ba�l�k alan� zorunlu")]
        [MinLength(1, ErrorMessage = "ba�l�k 1 karakterden uzun olmal�")]
        [MaxLength(1024, ErrorMessage = "a��klama 1024 karakterden k�sa olmal�")]
        public string Title { get; set; } = "";

        [BindProperty]
        public string? Slider { get; set; } = "";

        [BindProperty]
        public string? InstagramLink { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "foto�raf y�kleyiniz")]
        public IFormFile ImageFile { get; set; }


        public string errorMessage = "";
        public string successMessage = "";

        private IWebHostEnvironment webHostEnvironment;
        public ImagesModel(IWebHostEnvironment env)
        {
            webHostEnvironment = env;
        }

        public void OnGet()
        {

        }
        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                errorMessage = "data validation error";
                return;
            }
            if (Description == null) Description = "";
            if (InstagramLink == null) InstagramLink = "#";

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(ImageFile.FileName);

            string imageFolder = webHostEnvironment.WebRootPath + "/images/";

            string imageFullPath = Path.Combine(imageFolder, newFileName);

            using (var stream = System.IO.File.Create(imageFullPath))
            {
                ImageFile.CopyTo(stream);
            }

            try
            {
                string connectionString = "Data Source=localhost\\SQLEXPRESS01;Initial Catalog=busefindik;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO images (title, category, description, image_filename, slider, instagramlink) VALUES (@title, @category, @description, @image_filename, @slider, @instagramlink);";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@title", Title);
                        command.Parameters.AddWithValue("@category", Category);
                        command.Parameters.AddWithValue("@description", Description);
                        command.Parameters.AddWithValue("@image_filename", newFileName);
                        command.Parameters.AddWithValue("@slider", Slider);
                        command.Parameters.AddWithValue("@instagramlink", InstagramLink);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
            successMessage = "ba�ar�l�";
            Response.Redirect("/");
        }

    }
}
