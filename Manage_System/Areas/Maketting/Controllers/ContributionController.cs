using AspNetCoreHero.ToastNotification.Abstractions;
using Manage_System.models;
using Manage_System.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

namespace Manage_System.Areas.Maketting.Controllers
{

    [Area("Maketting")]
    [Authorize(Policy = "Maketting")]
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
                .Where(x => x.Status == "Approved" && x.SubmissionDate.Value.AddDays(14) > DateTime.Now)
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

                        userName = (file.Contribution.User.FullName + "_" + file.Contribution.Title).Replace(" ", "_");
                    }

                }

                // Trả về tệp Zip dưới dạng phản hồi HTTP
                _notyf.Success("Download File Success");
                return File(memoryStream.ToArray(), "application/zip", "" + userName.ToString() + ".zip");
                
            }


        }

        //for All File
        public async Task<MemoryStream> DownloadFileAsync(int id)
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
                        var wwwPath = this._env.WebRootPath;
                        var entryPath = Path.Combine(wwwPath, "Uploads\\", file.Url);

                        var fileInfor = new FileInfo(entryPath);

                        var entry = archive.CreateEntry(file.Url);

                        using (var entryStream = entry.Open())
                        using (var fileStream = new FileStream(entryPath, FileMode.Open, FileAccess.Read))
                        {
                            await fileStream.CopyToAsync(entryStream);
                        }

                        userName = (file.Contribution.User.FullName + "_" + file.Contribution.Title).Replace(" ", "_");
                    }
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                return memoryStream;
            }
        }


        [Route("/Maketting/Contributions/DownloadAllFile")]
        public async Task<IActionResult> DownloadAllFile()
        {
            var urlFiles = _db.Contributions
                .Include(x => x.User)
                .Include(x => x.ImgFiles)
                .Where(x => x.Status == "Approved")
                .ToList();

            var zipFiles = new List<byte[]>();

            var nameFile = new List<string>();
            foreach (var file in urlFiles)
            {
                var fileMemoryStream = await DownloadFileAsync((int)file.Id);
                zipFiles.Add(fileMemoryStream.ToArray());
                nameFile.Add((file.User.FullName + "_" + file.Title).Replace(" ", "_"));
            }

            using (var responseStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(responseStream, ZipArchiveMode.Create, true))
                {
                    for (var i = 0; i < zipFiles.Count; i++)
                    {
                        var zipFileBytes = zipFiles[i];
                        var name = nameFile[i];
                        var entry = zipArchive.CreateEntry(""+name+".zip");

                        using (var entryStream = entry.Open())
                        using (var fileMemoryStream = new MemoryStream(zipFileBytes))
                        {
                            await fileMemoryStream.CopyToAsync(entryStream);
                        }
                    }
                }

                responseStream.Seek(0, SeekOrigin.Begin);
                _notyf.Success("Download All File Success");
                return File(responseStream.ToArray(), "application/zip", "AllFiles.zip");
            }
        }
    }
}
