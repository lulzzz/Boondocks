using Boondocks.Auth.Api.Models;
using Boondocks.Auth.Domain.Entities;
using NetFusion.Base.Validation;
using NetFusion.Messaging.Types;
using System;

namespace Boondocks.Auth.Api.Commands
{
    /// <summary>
    /// Command issued to authenticate the caller for a requesting service-API using
    /// a given set of credentials.
    /// </summary>
    public class AuthenticateCaller : Command<AuthResult>,
        IValidatableType
    {
        /// <summary>
        /// Contains information about the authentication request.
        /// </summary>
        public AuthContext Context { get; }

        /// <summary>
        /// Command constructor.
        /// </summary>
        /// <param name="credentialModel">Model containing information about the service API requesting
        /// authentication and a set of key value pairs identifying the caller.</param>
        /// <param name="resourceModel">Model containing information about the resource for which access
        /// is being requested.</param>
        public AuthenticateCaller(
            AuthCredentialModel credentialModel,
            AuthResourceModel resourceModel)
        {
            Context = AuthContext.FromService(credentialModel.Api, credentialModel.Credentials);
            if (resourceModel.IsResourceAccessRequest)
            {
                Context = Context.ForResource(resourceModel.Service, resourceModel.Scope);
            }
        }

        public void Validate(IObjectValidator validator)
        {
            validator.Verify(!String.IsNullOrWhiteSpace(Context.RequestingApi), 
                "Service requesting validation not set.");
            
            validator.Verify(Context.Credentials != null, 
                "Credentials not specified.");

            validator.Verify(Context.IsValidResourceScope,
                "Specified resource context is not valid.");
        }
    }
}
