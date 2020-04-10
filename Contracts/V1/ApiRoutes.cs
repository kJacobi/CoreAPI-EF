using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Contracts.V1
{
	public class ApiRoutes
	{
		public const string Root = "api";
		public const string Version = "v1";
		public const string Base = Root + "/" + Version;

		public static class Auth
		{
			public const string Login = Base + "/Login";

			public const string Register = Base + "/Register";

			public const string Refresh = Base + "/Refresh";
		}


		public static class Inventory
		{
			public const string GetAllInventoryByProject = Base + "/GetAllByProject/{ProjectKey}";

			public const string GetInventory = Base + "/GetInventory/{UniqueId}";

			public const string CreateInventory = Base + "/CreateInventory";

			public const string UpdateInventory = Base + "/UpdateInventory/{UniqueId}";

			public const string DeleteInventory = Base + "/DeleteInventory/{UniqueId}";
		}

		public static class Project
		{
			public const string GetAllProjects = Base + "/GetAllProjects";

			public const string GetProject = Base + "/GetProject/{UniqueId}";

			public const string CreateProject = Base + "/CreateProject";
		}

		public static class Transaction
		{
			public const string CreateTransaction = Base + "/CreateTransaction";

			public const string GetTransaction = Base + "/GetTransaction/{UniqueId}";

			public const string GetTransactionImageDownload = Base + "/GetTransactionImageDownload/{UniqueId}";

			public const string UpdateTransaction = Base + "/UpdateTransaction/{UniqueId}";
		}
	}
}
