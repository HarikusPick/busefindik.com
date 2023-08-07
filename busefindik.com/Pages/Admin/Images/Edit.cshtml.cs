using busefindik.com.MyHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace busefindik.com.Pages.Admin.Images
{
    [RequireAuth(RequiredRole = "admin")]
    public class EditModel : PageModel
    {
        [BindProperty]
        public int Id { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "The Title is required")]
        [MaxLength(100, ErrorMessage = "The Title cannot exceed 100 characters")]
        public string Title { get; set; } = "";

        
        [BindProperty, Required]
        public string Category { get; set; } = "";

        [BindProperty]
        [MaxLength(5000, ErrorMessage = "The Description cannot exceed 5000 characters")]
        public string? Description { get; set; } = "";

        [BindProperty]
        public string ImageFileName { get; set; } = "";

        [BindProperty, Required]
        public string Slider { get; set; } = "";

        [BindProperty]
        public string? InstagramLink { get; set; } = "";

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public string errorMessage = "";
        public string successMessage = "";

        private IWebHostEnvironment webHostEnvironment;

        public EditModel(IWebHostEnvironment env)  //constractor
        {
            webHostEnvironment = env; //public folderdeki dosyalara eriþim saðlama
        }


        public void OnGet()
        {
            string requestId = Request.Query["id"];

            try
            {
                string connectionString = "Data Source=localhost\\SQLEXPRESS01;Initial Catalog=busefindik;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM images WHERE id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", requestId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Id = reader.GetInt32(0);
                                Title = reader.GetString(1);
                                Category = reader.GetString(2);
                                Description = reader.GetString(3);
                                ImageFileName = reader.GetString(4);
                                Slider = reader.GetString(5);
                                InstagramLink = reader.GetString(7);
                            }
                            else
                            {
                                Response.Redirect("/Admin/Images/Index");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Response.Redirect("/Admin/Images/Index");
            }
        }

        public void OnPost()
        {
            if (!ModelState.IsValid) //modelin doðrulama durumunu kontrol eder 
            {
                errorMessage = "Data validation failed";
                return;
            }

            // successfull data validation

            if (Description == null) Description = "";
            if (InstagramLink == null) InstagramLink = "#";

            //eðer yeni imagefile varsa eskiyi silip yenisini eklememiz lazým
            string newFileName = ImageFileName;
            if (ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(ImageFile.FileName);

                string imageFolder = webHostEnvironment.WebRootPath + "/images/";
                string imageFullPath = Path.Combine(imageFolder, newFileName);
                /*
                Bu kod, Path.Combine yöntemi kullanýlarak imageFolder ve newFileName 
                deðiþkenlerinin deðerlerini birleþtirerek tam bir dosya yolunu oluþturuyor.
               */
                Console.WriteLine("New image (Edit): " + imageFullPath); //file stream nesnesi yaratma using kullanma nedeni kod çalýþtýktan sonra otomatik dispose olmasý

                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    ImageFile.CopyTo(stream);
                }

                // delete old image
                string oldImageFullPath = Path.Combine(imageFolder, ImageFileName);
                System.IO.File.Delete(oldImageFullPath);
                Console.WriteLine("Delete Image " + oldImageFullPath);
            }

            // update the book data in the database
            try
            {
                string connectionString = "Data Source=localhost\\SQLEXPRESS01;Initial Catalog=busefindik;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE images SET title=@title, category=@category, description=@description, image_filename=@image_filename, slider=@slider, instagramlink=@instagramlink WHERE id=@id;";
                        


                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@title", Title);
                        command.Parameters.AddWithValue("@category", Category);
                        command.Parameters.AddWithValue("@description", Description);
                        command.Parameters.AddWithValue("@image_filename", newFileName);
                        command.Parameters.AddWithValue("@slider", Slider);
                        command.Parameters.AddWithValue("@id", Id);
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

            successMessage = "Data saved correctly";
            Response.Redirect("/Admin/Images/Index");
        }
    }
}
