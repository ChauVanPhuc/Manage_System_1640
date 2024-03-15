namespace Manage_System.Service
{
    public class FileService : IFileService
    {
        IWebHostEnvironment env;
        public FileService(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public string SaveImage(IFormFile imgFile)
        {
            try
            {
                var wwwPath = this.env.WebRootPath;
                var path = Path.Combine(wwwPath, "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                // Check the allowed extenstions
                var ext = Path.GetExtension(imgFile.FileName);
/*                var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg" };
                if (!allowedExtensions.Contains(ext))
                {
                    string msg = string.Format("Only {0} extensions are allowed", string.Join(",", allowedExtensions));

                }*/

                string uniqueString = Guid.NewGuid().ToString();
                var newFileName = uniqueString + ext;
                var fileWithPath = Path.Combine(path, newFileName);
                var stream = new FileStream(fileWithPath, FileMode.Create);
                imgFile.CopyTo(stream);
                stream.Close();
                return newFileName;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool Delete(string imgFile)
        {
            try
            {
                var wwwPath = this.env.WebRootPath;
                var path = Path.Combine(wwwPath, "Uploads\\", imgFile);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    return true;
                }
                return false;
            }
            catch 
            {
                return false;
               
            }
        }


    }
}
