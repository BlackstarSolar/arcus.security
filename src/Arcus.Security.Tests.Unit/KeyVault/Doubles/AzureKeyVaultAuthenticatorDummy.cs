﻿using System;
using System.Threading.Tasks;
using Arcus.Security.Providers.AzureKeyVault.Authentication.Interfaces;
using Microsoft.Azure.KeyVault;

namespace Arcus.Security.Tests.Unit.KeyVault.Doubles
{
    internal class AzureKeyVaultAuthenticatorDummy : IKeyVaultAuthentication
    {
        /// <summary>
        ///     Authenticates with Azure Key Vault
        /// </summary>
        /// <returns>A <see cref="IKeyVaultClient" /> client to use for interaction with the vault</returns>
        public Task<IKeyVaultClient> Authenticate()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Authenticates with Azure Key Vault
        /// </summary>
        /// <returns>A <see cref="IKeyVaultClient" /> client to use for interaction with the vault</returns>
        public async Task<IKeyVaultClient> AuthenticateAsync()
        {
            throw new NotImplementedException();
        }
    }
}