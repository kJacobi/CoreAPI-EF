using CoreAPI_EF.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Interfaces
{
	public interface ITransactionService
	{
		Task<Transaction> GetTransactionByUniqueIdAsync(Guid uniqueId);
		Task<List<string>> CreateTransactionAsync(Transaction tranToCreate, List<IFormFile> imagesToCreate, bool byPassSkuValidation);
		Task<List<string>> UpdateTransactionAsync(Transaction tranToUpdate, Transaction originalTran, List<IFormFile> imagesToUpdate, string token);
		Task<object> GetTransactionImageInfoAsync(int transactionId);
		Task<TransactionImage> GetDownloadImageByUniqueIdAsync(Guid unqiqueId);
		Task<CloudBlobContainer> GetCloudBlobContainer();
	}
}
