using System;
using System.Data;
using Boondocks.Services.Management.Contracts;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace Boondocks.Services.Management.WebApi.Model
{
    public static class DataAccessExtensions
    {
        /// <summary>
        /// If null, NotFoundResult is returned. Otherwise OkObjectResult with the specified value is returned. This is 
        /// useful for checking if in object exists in the database.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IActionResult ObjectOrNotFound(this object value)
        {
            if (value == null)
                return new NotFoundResult();

            return new OkObjectResult(value);
        }

        /// <summary>
        /// If the value is 0, a NotFoundResult is returned. Otherwise, OkResult is returned.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IActionResult HandleUpdateResult(this int value)
        {
            if (value == 0)
                return new NotFoundResult();

            return new OkResult();
        }
    }
}