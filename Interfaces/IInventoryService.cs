using CoreAPI_EF.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Interfaces
{
	public interface IInventoryService
	{
		Task<Inventory> GetInventoryByUniqueIdAsync(Guid UniqueId);
		Task<List<Inventory>> GetAllInventoryByProjectAsync(string projectKey);
		Task<bool> CreateInventoryAsync(Inventory invToCreate);
		Task<Inventory> UpdateInventoryAsync(Guid UniqueId, IDictionary<string, string> updateInvDict);
		Task<bool> DeleteInventoryAsync(Guid UniqueId);
		Task<bool> ValidateProjectAccessFromToken(string projectKey, string token);
		Task<bool> RedeemInventory(string projectKey, string sku, int count);
	}
}

