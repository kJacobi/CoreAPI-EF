using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using CoreAPI_EF.Contracts.V1;
using CoreAPI_EF.Contracts.V1.Requests;
using CoreAPI_EF.Contracts.V1.Responses;
using CoreAPI_EF.Domain;
using CoreAPI_EF.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CoreAPI_EF.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Super, Admin, Manager")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IMapper _mapper;

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
                {".png", "image/png" },
                {".jpg", "image/jpeg" },
                {".jpeg", "image/jpeg" },
                {".gif", "image/gif" },
                {".csv", "text/csv" },
            };
        }

        public TransactionController(ITransactionService transactionService, IMapper mapper)
        {
            _transactionService = transactionService;
            _mapper = mapper;
        }

        /*******************************************************
        * CreateTransaction
        * ****************************************************/
        /// <remarks>
        /// NOTE:  "dtBirth" is a required field.   Send "1900-01-01" as a default value if you do not plan to use.
        /// </remarks>
        [HttpPost(ApiRoutes.Transaction.CreateTransaction)]
        public async Task<IActionResult> CreateTransaction([FromForm] Req_Transaction request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Res_Common
                {
                    Success = false,
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
            if (request.Amount == null)
            {
                request.Amount = 0;
            }

            var tranToCreate = _mapper.Map<Transaction>(request);
            if (tranToCreate.DtTransaction is null)
                tranToCreate.DtTransaction = DateTime.Now;

            List<IFormFile> imagesToCreate = new List<IFormFile>();
            PropertyInfo[] props = request.GetType().GetProperties();
            foreach (var p in props.Where(x => x.PropertyType == typeof(IFormFile) && x.GetValue(request, null) != null))
            {
                IFormFile f = (IFormFile)p.GetValue(request, null);
                if (f != null) imagesToCreate.Add(f);
            }



            IList<string> Errors = await _transactionService.CreateTransactionAsync(tranToCreate, imagesToCreate, request.ByPassSkuValidation);

            var response = _mapper.Map<Res_Transaction>(tranToCreate);
            if (Errors.Count > 0)
            {
                response.Success = false;
                response.Errors = Errors;
                return BadRequest(response);
            }

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + ApiRoutes.Transaction.GetTransaction.Replace("{UniqueId}", tranToCreate.UniqueId.ToString());

            response.Success = true;
            return Created(locationUri, response);

        }

        /*******************************************************
        * GetTransaction
        * ****************************************************/
        [HttpGet(ApiRoutes.Transaction.GetTransaction)]
        public async Task<IActionResult> GetTransaction([FromRoute] Guid UniqueId)
        {
            var tran = await _transactionService.GetTransactionByUniqueIdAsync(UniqueId);
            if (tran is null)
                return NotFound();

            var response = _mapper.Map<Res_GetTransaction>(tran);
            response.ImageData = await _transactionService.GetTransactionImageInfoAsync(tran.Id);

            return Ok(response);
        }

        /*******************************************************
        * GetTransactionImageDownload
        * ****************************************************/
        [HttpGet(ApiRoutes.Transaction.GetTransactionImageDownload)]
        public async Task<IActionResult> GetTransactionImageDownload([FromRoute] Guid UniqueId)
        {
            var img = await _transactionService.GetDownloadImageByUniqueIdAsync(UniqueId);
            if (img is null)
                return NotFound();

            try
            {
                if (img.StorageType == "blob")
                {
                    CloudBlobContainer container = await _transactionService.GetCloudBlobContainer();
                    CloudBlockBlob blob = container.GetBlockBlobReference($"{img.Path}/{img.ImageName}");
                    Stream blobStream = await blob.OpenReadAsync();

                    return File(blobStream, blob.Properties.ContentType, img.ImageName);
                }
                else
                {
                    var path = $"{img.Path}\\{img.ImageName}";
                    var ext = Path.GetExtension(path).ToLowerInvariant();
                    var memory = new MemoryStream();

                    using (var stream = new FileStream(path, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;

                    return File(memory, GetMimeTypes()[ext], Path.GetFileName(path));
                }
            }
            catch
            {
                return NotFound();
            }
        }

        /*******************************************************
        * UpdateTransaction
        * ****************************************************/
        [HttpPost(ApiRoutes.Transaction.UpdateTransaction)]
        public async Task<IActionResult> UpdateTransaction([FromRoute] Guid UniqueId, [FromForm] Req_Transaction request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Res_Common
                {
                    Success = false,
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }

            var tranOriginal = await _transactionService.GetTransactionByUniqueIdAsync(UniqueId);
            if (tranOriginal is null)
                return NotFound();

            var auth = Request.Headers["Authorization"];
            var token = auth.First().Remove(0, "Bearer ".Length).Trim();

            List<IFormFile> imagesToCreate = new List<IFormFile>();
            PropertyInfo[] props = request.GetType().GetProperties();
            foreach (var p in props.Where(x => x.PropertyType == typeof(IFormFile) && x.GetValue(request, null) != null))
            {
                IFormFile f = (IFormFile)p.GetValue(request, null);
                if (f != null) imagesToCreate.Add(f);
            }

            var tranUpdate = _mapper.Map<Transaction>(tranOriginal);
            _mapper.Map<Req_Transaction, Transaction>(request, tranUpdate);
            IList<string> Errors = await _transactionService.UpdateTransactionAsync(tranOriginal, tranUpdate, imagesToCreate, token);

            var response = _mapper.Map<Res_Transaction>(tranUpdate);
            if (Errors.Count > 0)
            {
                response.Success = false;
                response.Errors = Errors;
                return BadRequest(response);
            }

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + ApiRoutes.Transaction.GetTransaction.Replace("{UniqueId}", tranUpdate.UniqueId.ToString());

            response.Success = true;
            return Created(locationUri, response);
        }

    }
}