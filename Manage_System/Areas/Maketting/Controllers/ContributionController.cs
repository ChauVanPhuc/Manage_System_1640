using AspNetCoreHero.ToastNotification.Abstractions;
using Manage_System.models;
using Manage_System.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

namespace Manage_System.Areas.Maketting.Controllers
{

    [Area("Maketting")]
    public class ContributionController : Controller
    {
        private readonly ManageSystem1640Context _db;
        private readonly INotyfService _notyf;
        IWebHostEnvironment _env;

        public ContributionController(ManageSystem1640Context db, INotyfService notyf, IWebHostEnvironment env)
        {
            _db = db;
            _notyf = notyf;
            _env = env;
        }

        [Route("Maketting/Contributions")]
        public IActionResult Index()
        {
            var contributions = _db.Contributions
                .Include(x => x.ImgFiles)
                .Include(x => x.Comments)
                .Include(x => x.Magazine)
                .Include(x => x.User)
                .Where(x => x.Status == true)
                .ToList();

            return View(contributions);
        }

        [Route("/Maketting/Contributions/Publish/{id:}")]
        public IActionResult Publish(int id)
        {
            try
            {
                var contri = _db.Contributions.Find(id);
                if (contri == null)
                {
                    _notyf.Error("Contributions does not exist");
                    return Redirect("/Maketting/Contributions");
                }
                else
                {
                    if (contri.Publics == false)
                    {
                        contri.Publics = true;
                    }
                    else
                    {
                        contri.Publics = false;
                    }

                    _db.Contributions.Update(contri);
                    _db.SaveChanges();

                    _notyf.Success("Update Success");
                    return Redirect("/Maketting/Contributions");
                }
            }
            catch
            {

                _notyf.Error("Update Faill");
                return Redirect("/Maketting/Contributions");
            }


        }

        [Route("/Maketting/Contributions/DownloadFile/{id:}")]
        public IActionResult DownloadFile(int id)
        {
            var urlFile = _db.ImgFiles
                .Include(x => x.Contribution)
                .ThenInclude(x => x.User)
                .Where(x => x.ContributionId == id)
                .ToList();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                string userName = "";
                
                using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {   
                    foreach (var file in urlFile)
                    {
                        // Đường dẫn đầy đủ của tệp trong tệp Zip
                        var wwwPath = this._env.WebRootPath;
                        var entryPath = Path.Combine(wwwPath, "Uploads\\", file.Url);


                        var fileInfor = new FileInfo(entryPath);    

                        // Tạo entry cho tệp        
                        var entry = archive.CreateEntry(file.Url);
                            
                        
                        using (var entryStream = entry.Open())
                        using (var fileStream = new FileStream(entryPath, FileMode.Open, FileAccess.Read))
                        {
                            fileStream.CopyTo(entryStream);
                        }

                        userName = file.Contribution.User.FullName.Replace(" ", "");
                    }
                    
                }   

                // Trả về tệp Zip dưới dạng phản hồi HTTP
                return File(memoryStream.ToArray(), "application/zip", ""+userName.ToString()+ ".zip");
            }


        }
    }
}
