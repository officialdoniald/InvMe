using BLL.Xamarin.FileStoreAndLoad;

[assembly: Xamarin.Forms.Dependency(typeof(FileStoringWithDependency.Droid.FileStoreAndLoad.FileStoreAndLoad))]
namespace FileStoringWithDependency.Droid.FileStoreAndLoad
{
    public class FileStoreAndLoad : IFileStoreAndLoad
    {
        public void SaveText(string filename, string text)
        {
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = System.IO.Path.Combine(documentsPath, filename);
            System.IO.File.WriteAllText(filePath, text);
        }
        public string LoadText(string filename)
        {
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = System.IO.Path.Combine(documentsPath, filename);
            return System.IO.File.ReadAllText(filePath);
        }
    }
}