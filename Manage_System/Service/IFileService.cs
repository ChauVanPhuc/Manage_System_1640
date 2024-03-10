namespace Manage_System.Service
{
    public interface IFileService
    {
        public string SaveImage(IFormFile imgFile);
        public bool Delete(string imgFile);

    }
}
