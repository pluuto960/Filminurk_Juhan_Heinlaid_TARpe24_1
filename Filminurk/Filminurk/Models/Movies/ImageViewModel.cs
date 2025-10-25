namespace Filminurk.Views.Movies
{
    public class ImageViewModel
    {
        public Guid ImageID { get; set; }
        public string? FilePath { get; set; }
        public Guid? MovieID { get; set; }
        public bool? IsPoster { get; set; } //näitab kas pilt on poster või mitte
    }
}
