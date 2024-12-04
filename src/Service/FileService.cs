namespace dating_app_backend.src.Service
{
    public class FileService
    {
        private readonly string _uploadFolder;

        public FileService()
        {
            _uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "Upload/images");
            if (!Directory.Exists(_uploadFolder))
            {
                Directory.CreateDirectory(_uploadFolder);
            }
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is invalid.");
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(_uploadFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/Upload/images/{fileName}";
        }
    }
}
