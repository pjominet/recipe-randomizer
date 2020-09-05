using System.IO;

namespace RecipeRandomizer.Business.Interfaces
{
    public interface IFileService
    {
        public void CheckForAllowedSignature(Stream stream, string proposedExtension);
        public void DeleteExistingFile(string fileName);
        public void SaveFileToDisk(Stream sourceStream, string physicalDestination, string trustedFileName);
    }
}
