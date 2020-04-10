using AutoMapper;
using CoreAPI_EF.Data;
using CoreAPI_EF.Domain;
using CoreAPI_EF.Interfaces;
using CoreAPI_EF.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace CoreAPI_EF.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly DataContext _dataContext;
        private readonly TransactionControlSettings _transactionControlSettings;
        private readonly IMapper _mapper;
        private readonly ITokenHelperService _tokenHelperService;
        private readonly IInventoryService _inventoryService;

        struct ImageData
        {
            public Guid uniqueId { get; set; }
            public string name { get; set; }
            public string storageType { get; set; }
            public string path { get; set; }
            public bool active { get; set; }
        }

        struct FileDownloadData
        {
            public string fileName { get; set; }
            public string mimeType { get; set; }
            public MemoryStream memStream { get; set; }
        }

        public TransactionService(DataContext dataContext, TransactionControlSettings transactionControlSettings, IMapper mapper, ITokenHelperService tokenHelperService, IInventoryService inventoryService)
        {
            _dataContext = dataContext;
            _transactionControlSettings = transactionControlSettings;
            _mapper = mapper;
            _tokenHelperService = tokenHelperService;
            _inventoryService = inventoryService;
        }


        /*******************************************************
         * GetTransactionByuniqueIdAsync
         * ****************************************************/
        public async Task<Transaction> GetTransactionByUniqueIdAsync(Guid uniqueId)
        {
            return await _dataContext.Transactions.SingleOrDefaultAsync(x => x.UniqueId == uniqueId);
        }

        /*******************************************************
         * CreateTransactionAsync
         * ****************************************************/
        public async Task<List<string>> CreateTransactionAsync(Transaction tranToCreate, List<IFormFile> imagesToCreate, bool byPassSkuValidation)
        {
            List<string> errors = new List<string>();

            try
            {
                errors = await ValidateTransactionAsync(tranToCreate, errors, false, byPassSkuValidation);
                if (errors.Count == 0)
                {
                    if (imagesToCreate.Count > 0)
                        errors = ValidateImagesAsync(imagesToCreate, errors);

                    if (errors.Count > 0)
                        return errors;

                    tranToCreate.Status = "P";
                    await _dataContext.Transactions.AddAsync(tranToCreate);
                    var created = await _dataContext.SaveChangesAsync();
                    if (created > 0)
                    {
                        if (byPassSkuValidation == false)
                            await _inventoryService.RedeemInventory(tranToCreate.ProjectKey, tranToCreate.SKU, 1);
                        if (imagesToCreate.Count > 0)
                            await CreateImagesAsync(imagesToCreate, tranToCreate.Id);
                    }
                    else
                        errors.Add("Transaction not created");
                }
            }
            catch
            {
                errors.Add("Transaction not created");
            }

            return errors;
        }

        /*******************************************************
         * ValidateTransactionAsync
         * ****************************************************/
        private async Task<List<string>> ValidateTransactionAsync(Transaction tranToCreate, List<string> errors, bool isUpdate = false, bool byPassSkuValidation = false)
        {
            var project = await _dataContext.Projects.SingleOrDefaultAsync(x => x.ProjectKey == tranToCreate.ProjectKey && x.Status == "A");
            if (project is null)
            {
                errors.Add("Invalid project key");
            }
            else if (byPassSkuValidation == false)
            {

                var inv = await _dataContext.Inventory.SingleOrDefaultAsync(x => x.ProjectKey == tranToCreate.ProjectKey && x.SKU == tranToCreate.SKU && x.Status == "A");

                if (inv is null)
                    errors.Add("Invalid sku");
                else
                    tranToCreate.InventoryId = inv.Id;

                if (!isUpdate)
                {
                    if (inv.ControlType == 1 || inv.ControlType == 3)
                    {
                        if (inv.Balance <= 0 || inv.Allocation <= inv.Usage)
                            errors.Add("Inventory item is out of stock");
                    }
                    else if (inv.ControlType == 2 || inv.ControlType == 3)
                    {
                        if (tranToCreate.DtTransaction < inv.DtStart || tranToCreate.DtTransaction > inv.DtEnd)
                            errors.Add("Inventory item is expired");
                    }


                }
            }

            return errors;
        }

        /*******************************************************
         * ValidateImagesAsync
         * ****************************************************/
        private List<string> ValidateImagesAsync(IList<IFormFile> imagesToCreate, List<string> errors)
        {
            try
            {
                string fileName = string.Empty;
                HashSet<string> filenames = new HashSet<string>();
                foreach (IFormFile img in imagesToCreate)
                {
                    if (img != null && img.Length > 0)
                    {
                        fileName = img.FileName;
                        var imageExt = Path.GetExtension(fileName);
                        if (!_transactionControlSettings.AcceptedFileTypes.Contains(imageExt))
                        {
                            errors.Add($"File type for \"{fileName}\" is not accepted");
                        }

                        if (filenames.Contains(fileName))
                        {
                            errors.Add($"Image names are not unique");
                            break;
                        }
                        else
                        {
                            filenames.Add(fileName);
                        }
                    }
                }
            }
            catch
            {
                errors.Add("Error saving image");
            }

            return errors;
        }

        /*******************************************************
        * CreateImagesAsync
        * ****************************************************/
        private async Task<bool> CreateImagesAsync(List<IFormFile> imagesToCreate, int transactionId)
        {
            var previousTransactionImages = _dataContext.TransactionImages.Where(x => x.TransactionId == transactionId).ToList();
            previousTransactionImages.ForEach(x => x.Active = false);

            string today = DateTime.Now.ToString("yyyy-MM-dd");
            string rightNow = DateTime.Now.ToString("yyyyMMddHHmmssfff");

            try
            {
                if (_transactionControlSettings.StorageType == "blob")
                {
                    CloudBlobContainer blobContainer = await GetCloudBlobContainer();

                    string basePath = ($"{_transactionControlSettings.BlobBaseStoragePath}/{today}");
                    string savePath = ($"{basePath}/{rightNow}");

                    while (await BlobPathExistsAsync(blobContainer, savePath))
                    {
                        rightNow = DateTime.Now.AddMilliseconds(1).ToString("yyyyMMddHHmmssfff");
                        savePath = ($"{basePath}/{rightNow}");
                    }


                    foreach (IFormFile img in imagesToCreate)
                    {
                        CloudBlockBlob blob = blobContainer.GetBlockBlobReference($"{savePath}/{img.FileName}");
                        blob.Properties.ContentType = img.ContentType;

                        using (var memoryStream = new MemoryStream())
                        {
                            img.OpenReadStream().CopyTo(memoryStream);
                            memoryStream.ToArray();
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            await blob.UploadFromStreamAsync(memoryStream);
                        }

                        await _dataContext.TransactionImages.AddAsync(new TransactionImage
                        {
                            TransactionId = transactionId,
                            StorageType = "blob",
                            BlobContainer = _transactionControlSettings.BlobContainer,
                            BlobContainerUrl = _transactionControlSettings.BlobContainerUrl,
                            Path = savePath,
                            ImageName = img.FileName,
                            Active = true
                        });
                    }
                }
                else
                {
                    string basePath = ($"{_transactionControlSettings.FileBaseStoragePath}\\{today}");
                    string savePath = ($"{basePath}\\{rightNow}");

                    while (System.IO.Directory.Exists(savePath))
                    {
                        rightNow = DateTime.Now.AddMilliseconds(1).ToString("yyyyMMddHHmmssfff");
                        savePath = ($"{basePath}\\{rightNow}");
                    }

                    System.IO.Directory.CreateDirectory(savePath);

                    foreach (IFormFile img in imagesToCreate)
                    {
                        using (var fileStream = new FileStream(Path.Combine(savePath, img.FileName), FileMode.Create))
                        {
                            img.CopyTo(fileStream);
                        }

                        await _dataContext.TransactionImages.AddAsync(new TransactionImage
                        {
                            TransactionId = transactionId,
                            StorageType = "file",
                            Path = savePath,
                            ImageName = img.FileName,
                            Active = true
                        });
                    }
                }
            }
            catch
            {
                return false;
            }

            var created = await _dataContext.SaveChangesAsync();
            return created > 0;
        }

        /*******************************************************
        * UpdateTransactionAsync
        * ****************************************************/
        public async Task<List<string>> UpdateTransactionAsync(Transaction originalTran, Transaction tranUpdate, List<IFormFile> imagesToCreate, string token)
        {
            List<string> errors = new List<string>();
            try
            {
                errors = VerifyUpdatePermitted(originalTran, errors);
                if (errors.Count > 0)
                    return errors;

                errors = await ValidateTransactionAsync(tranUpdate, errors, true);
                if (errors.Count == 0)
                {
                    if (imagesToCreate.Count > 0)
                        errors = ValidateImagesAsync(imagesToCreate, errors);

                    if (errors.Count > 0)
                        return errors;

                    var logged = await CreateTransactionLog(originalTran, token);
                    if (!logged)
                    {
                        errors.Add("Transaction not updated");
                    }
                    else
                    {
                        tranUpdate.Status = tranUpdate.Status.ToUpper() == "R" ? tranUpdate.Status = "A" : tranUpdate.Status = "P";  // Review goes to Active, NC goes to Pending

                        _dataContext.Entry(originalTran).State = EntityState.Detached;
                        _dataContext.Transactions.Update(tranUpdate);
                        var updated = await _dataContext.SaveChangesAsync();
                        if (updated > 0)
                        {
                            if (imagesToCreate.Count > 0)
                                await CreateImagesAsync(imagesToCreate, tranUpdate.Id);
                            else
                            {
                                var previousTransactionImages = _dataContext.TransactionImages.Where(x => x.TransactionId == tranUpdate.Id).ToList();
                                previousTransactionImages.ForEach(x => x.Active = false);
                                await _dataContext.SaveChangesAsync();
                            }

                        }
                        else
                            errors.Add("Transaction not updated");
                    }

                }
            }
            catch
            {
                errors.Add("Transaction not updated");
            }

            return errors;
        }

        /*******************************************************
        * VerifyUpdatePermitted
        * ****************************************************/
        private List<string> VerifyUpdatePermitted(Transaction originalTran, List<string> errors)
        {
            try
            {
                if (!_transactionControlSettings.PermitUpdateStatus.ToUpper().Contains(originalTran.Status.ToUpper()))
                {
                    errors.Add("Transaction is not available to update");
                }
                else if (originalTran.DtPulled != null && originalTran.Status.ToUpper() == "NC")
                {
                    if (originalTran.DtPulled.Value.AddDays(_transactionControlSettings.PermitUpdateLifetimeDays) < DateTime.Now)
                        errors.Add("Transaction is closed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return errors;
        }

        /*******************************************************
        * CreateTransactionLog
        * ****************************************************/
        private async Task<bool> CreateTransactionLog(Transaction originalTran, string token)
        {
            var logTran = _mapper.Map<TransactionLog>(originalTran);
            var user = await _tokenHelperService.GetUserFromToken(token);

            if (user != null)
                logTran.LoggedByUserName = user.UserName;

            await _dataContext.TransactionLogs.AddAsync(logTran);
            var logged = await _dataContext.SaveChangesAsync();

            return logged > 0;
        }

        /*******************************************************
        * GetTransactionImagesAsync
        * ****************************************************/
        public async Task<object> GetTransactionImageInfoAsync(int transactionId)
        {
            IList<ImageData> imgDataList = new List<ImageData>();
            var images = await _dataContext.TransactionImages.Where(x => x.TransactionId == transactionId).OrderByDescending(x => x.Active).ThenByDescending(x => x.DtCreated).ToListAsync();

            foreach (var i in images)
            {
                if (i.StorageType == "blob")
                    imgDataList.Add(new ImageData() { uniqueId = i.UniqueId, name = i.ImageName, path = $"{i.BlobContainerUrl}/{i.Path}", storageType = i.StorageType, active = i.Active });
                else
                    imgDataList.Add(new ImageData() { uniqueId = i.UniqueId, name = i.ImageName, path = i.Path, storageType = i.StorageType, active = i.Active });
            }

            return imgDataList;
        }

        /*******************************************************
        * GetDownloadImageByUniqueIdAsync
        * ****************************************************/
        public async Task<TransactionImage> GetDownloadImageByUniqueIdAsync(Guid unqiqueId)
        {
            return await _dataContext.TransactionImages.FirstOrDefaultAsync(x => x.UniqueId == unqiqueId);
        }

        /*******************************************************
        * GetCloudBlobContainer
        * ****************************************************/
        public async Task<CloudBlobContainer> GetCloudBlobContainer()
        {
            var storageAcct = CloudStorageAccount.Parse(_transactionControlSettings.StorageConnectionString);

            CloudBlobContainer blobContainer = storageAcct.CreateCloudBlobClient().GetContainerReference(_transactionControlSettings.BlobContainer);
            await blobContainer.CreateIfNotExistsAsync();
            return blobContainer;
        }

        /*******************************************************
        * GetMimeTypes
        *****************************************************/
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain" },
                {".pdf", "application/pdf" },
                {".doc", "application/vnd.ms-word" },
                {".docx", "application/vnd.ms-word" },
                {".xls", "application/vnd.ms-excel" },
                {".xlsx", "application/vd.openxmlformats-officedocument.spreadsheetml.sheet" },
                {".png", "imagepng" },
                {".jpg", "image/jpeg" },
                {".jpeg", "image/jpeg" },
                {".gif", "image/gif" },
                {".csv", "text/csv" },
            };
        }

        /*******************************************************
        * BlobPathExistsAsync
        * ****************************************************/
        private static async Task<bool> BlobPathExistsAsync(CloudBlobContainer container, string prefix)
        {
            BlobContinuationToken continuationToken = null;
            bool blobDirectoryExists = false;

            try
            {
                BlobResultSegment resultSegment = await container.ListBlobsSegmentedAsync(prefix,
                            false, BlobListingDetails.Metadata, null, continuationToken, null, null);

                blobDirectoryExists = resultSegment.Results.Count() > 0;
            }
            catch
            { }

            return blobDirectoryExists;
        }

    }
}
