using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace DevDay2016SmartGallery.Services
{
    public class AlbumStorageService
    {
        private CloudStorageAccount _storageAccount;
        private CloudBlobClient _blobClient; 

        public AlbumStorageService()
        {
            _storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.AppSettings["StorageConnectionString"]);

            _blobClient = _storageAccount.CreateCloudBlobClient(); 
        }

        private async Task<CloudBlobContainer> GetContainer(string containerName)
        {
            var container = _blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();
            await container.SetPermissionsAsync(
                new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Container });

            return container;
        }

        public async Task<string> SavePicture(string album, string name, Stream picture)
        {
            CloudBlobContainer container = await GetContainer(album);
            CloudBlockBlob block = container.GetBlockBlobReference(name);
            await block.UploadFromStreamAsync(picture); 
            return block.Uri.ToString();
        }

        public async Task<bool> DeletePicture(string album, string name)
        {
            CloudBlobContainer container = await GetContainer(album);
            CloudBlockBlob block = container.GetBlockBlobReference(name);
            return await block.DeleteIfExistsAsync(); 
        }
    }
}