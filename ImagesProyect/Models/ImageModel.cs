namespace ImagesProyect.Models
{
    public class ImageModel
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; } = DateTime.Now;
        public int NodeId { get; set; }  // ID del nodo donde se almacena la imagen
    }
}
