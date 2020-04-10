using CoreAPI_EF.Data;
using CoreAPI_EF.Domain;
using CoreAPI_EF.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Services
{
	public class InventoryService : IInventoryService
	{
		private readonly DataContext _dataContext;
		private readonly ITokenHelperService _tokenHelperService;

		public InventoryService(DataContext dataContext, ITokenHelperService tokenHelperService)
		{
			_dataContext = dataContext;
			_tokenHelperService = tokenHelperService;
		}


		/*******************************************************
		* IsJwtWithValidSecurityAlgorithm
		* ****************************************************/
		public async Task<Inventory> GetInventoryByUniqueIdAsync(Guid UniqueId)
		{
			return await _dataContext.Inventory.SingleOrDefaultAsync(x => x.UniqueId == UniqueId);
		}


		/*******************************************************
		* IsJwtWithValidSecurityAlgorithm
		* ****************************************************/
		public async Task<List<Inventory>> GetAllInventoryByProjectAsync(string projectKey)
		{
			return await _dataContext.Inventory.Where(x => x.ProjectKey == projectKey).ToListAsync();
		}


		/*******************************************************
		* CreateInventoryAsync
		* ****************************************************/
		public async Task<bool> CreateInventoryAsync(Inventory invToCreate)
		{
			try
			{
				await _dataContext.Inventory.AddAsync(invToCreate);
				var created = await _dataContext.SaveChangesAsync();
				return created > 0;
			}
			catch
			{
			}

			return false;

		}


		/*******************************************************
		* UpdateInventoryAsync
		* ****************************************************/
		public async Task<Inventory> UpdateInventoryAsync(Guid UniqueId, IDictionary<string, string> updateInvDict)//, IDictionary<string, string> updateDictionary)
		{
			var inv = await GetInventoryByUniqueIdAsync(UniqueId);
			if (inv is null)
				return null;

			if (updateInvDict.ContainsKey("SKU") && updateInvDict["SKU"] != null)
				inv.SKU = updateInvDict["SKU"].Trim();

			if (updateInvDict.ContainsKey("Name") && updateInvDict["Name"] != null)
				inv.Name = updateInvDict["Name"].Trim();

			if (updateInvDict.ContainsKey("Desc1") && updateInvDict["Desc1"] != null)
				inv.Desc1 = updateInvDict["Desc1"].Trim();

			if (updateInvDict.ContainsKey("Desc2") && updateInvDict["Desc2"] != null)
				inv.Desc2 = updateInvDict["Desc2"].Trim();

			if (updateInvDict.ContainsKey("Status") && updateInvDict["Status"] != null)
				inv.Status = updateInvDict["Status"].Trim();

			if (updateInvDict.ContainsKey("DtStart") && updateInvDict["DtStart"] != null)
			{
				DateTime dateTime;
				if (DateTime.TryParse(updateInvDict["DtStart"], out dateTime))
				{
					inv.DtStart = dateTime;
				}
			}

			if (updateInvDict.ContainsKey("DtEnd") && updateInvDict["DtEnd"] != null)
			{
				DateTime dateTime;
				if (DateTime.TryParse(updateInvDict["DtEnd"], out dateTime))
				{
					inv.DtEnd = dateTime;
				}
			}

			if (updateInvDict.ContainsKey("Balance") && updateInvDict["Balance"] != null)
			{
				int number;
				if (Int32.TryParse(updateInvDict["Balance"], out number))
				{
					inv.Balance = number;
				}
			}

			if (updateInvDict.ContainsKey("Allocation") && updateInvDict["Allocation"] != null)
			{
				int number;
				if (Int32.TryParse(updateInvDict["Allocation"], out number))
				{
					inv.Allocation = number;
				}
			}

			if (updateInvDict.ContainsKey("Usage") && updateInvDict["Usage"] != null)
			{
				int number;
				if (Int32.TryParse(updateInvDict["Usage"], out number))
				{
					inv.Usage = number;
				}
			}

			_dataContext.Inventory.Update(inv);
			var updated = await _dataContext.SaveChangesAsync();

			if (updated > 0)
				return inv;
			else
				return null;
		}

		/*******************************************************
		* RedeemItemAsync
		* ****************************************************/
		public async Task<bool> RedeemInventory(string projectKey, string sku, int count)
		{
			if (count > 0)
			{
				var inv = await _dataContext.Inventory.Where(x => x.ProjectKey == projectKey && x.SKU == sku).FirstOrDefaultAsync();
				inv.Usage += count;
				inv.Balance -= count;
				_dataContext.Inventory.Update(inv);
				var updated = await _dataContext.SaveChangesAsync();
				return updated > 0;
			}

			return false;
		}

		/*******************************************************
		* DeleteInventoryAsync
		* ****************************************************/
		public async Task<bool> DeleteInventoryAsync(Guid UniqueId)
		{
			var inv = await GetInventoryByUniqueIdAsync(UniqueId);
			_dataContext.Inventory.Remove(inv);
			var deleted = await _dataContext.SaveChangesAsync();
			return deleted > 0;
		}


		/*******************************************************
		* ValidateProjectAccessFromToken
		* ****************************************************/
		public async Task<bool> ValidateProjectAccessFromToken(string projectKey, string token)
		{
			var validatedToken = _tokenHelperService.GetPrincipalFromToken(token);
			try
			{
				if (validatedToken is null)
					return false;

				var user = await _tokenHelperService.GetUserFromToken(token);
				return _dataContext.UserProjects.Where(x => x.ProjectKey == projectKey && x.UserName == user.UserName).Count() > 0;
			}
			catch
			{
				return false;
			}
		}


	}
}
