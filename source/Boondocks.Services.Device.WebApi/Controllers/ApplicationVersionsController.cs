using System;
using Boondocks.Services.DataAccess.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boondocks.Services.Device.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("v1/applicationVersions")]
    [Authorize]
    public class ApplicationVersionsController : Controller
    {
        private readonly IBlobDataAccessProvider _blobDataAccessProvider;

        public ApplicationVersionsController(IBlobDataAccessProvider blobDataAccessProvider)
        {
            _blobDataAccessProvider = blobDataAccessProvider ?? throw new ArgumentNullException(nameof(blobDataAccessProvider));
        }

        [HttpGet("{id}")]
        [Produces("application/octet-stream")]
        public IActionResult Get(Guid id)
        {
            //TODO: Determine if spinnig up a dedicated docker repository would be more efficient.

            //Get the download stream
            var stream = _blobDataAccessProvider.ApplicationVersionImages.GetDownloadStream(id);

            //https://stackoverflow.com/a/42460443/232566
            return File(
                stream, 
                "application/octet-stream",
                _blobDataAccessProvider.ApplicationVersionImages.GetFilename(id));
        }
    }
}