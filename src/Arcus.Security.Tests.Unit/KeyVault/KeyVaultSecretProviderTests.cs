﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Arcus.Security.Providers.AzureKeyVault.Authentication.Interfaces;
using Arcus.Security.Providers.AzureKeyVault.Configuration;
using Arcus.Security.Secrets.AzureKeyVault;
using Arcus.Security.Secrets.Core.Models;
using Arcus.Security.Tests.Unit.KeyVault.Doubles;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Rest;
using Xunit;

namespace Arcus.Security.Tests.Unit.KeyVault
{
    public class KeyVaultSecretProviderTests
    {
        [Fact]
        public void KeyVaultSecretProvider_CreateWithEmptyUri_ShouldFailWithUriFormatException()
        {
            // Arrange
            string uri = string.Empty;

            // Act & Assert
            Assert.ThrowsAny<UriFormatException>(() => new KeyVaultSecretProvider(new AzureKeyVaultAuthenticatorDummy(), new KeyVaultConfiguration(uri)));
        }

        [Fact]
        public void KeyVaultSecretProvider_CreateWithHttpScheme_ShouldFailWithUriFormatException()
        {
            // Arrange
            string uri = $"http://{Guid.NewGuid():N}.vault.azure.net/";

            // Act & Assert
            Assert.ThrowsAny<UriFormatException>(() => new KeyVaultSecretProvider((IKeyVaultAuthentication) null, new KeyVaultConfiguration(uri)));
        }

        [Fact]
        public void KeyVaultSecretProvider_CreateWithoutUri_ShouldFailWithArgumentException()
        {
            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(
                () => new KeyVaultSecretProvider(
                    new AzureKeyVaultAuthenticatorDummy(), 
                    new KeyVaultConfiguration(rawVaultUri: null)));
        }

        [Fact]
        public void KeyVaultSecretProvider_CreateWithoutClientFactory_ShouldFailWithArgumentException()
        {
            // Arrange
            string uri = GenerateVaultUri();

            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() => new KeyVaultSecretProvider((IKeyVaultAuthentication) null, new KeyVaultConfiguration(uri)));
        }

        [Fact]
        public void KeyVaultSecretProvider_CreateWithValidArguments_ShouldSucceed()
        {
            // Arrange
            string uri = GenerateVaultUri();

            // Act & Assert
            var secretProvider = new KeyVaultSecretProvider(new AzureKeyVaultAuthenticatorDummy(), new KeyVaultConfiguration(uri));
            Assert.NotNull(secretProvider);
        }

        [Fact]
        public async Task KeyVaultSecretProvider_GetsSecretValue_AfterRetriedTooManyRequestException()
        {
            // Arrange
            string expected = $"secret-value-{Guid.NewGuid()}";
            string secretName = $"secret-name-{Guid.NewGuid()}";
            KeyVaultSecretProvider provider = CreateSecretProviderWithTooManyRequestSimulation(expected);

            // Act
            string actual = await provider.Get(secretName);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task KeyVaultSecretProvider_GetsRawSecretValue_AfterRetriedTooManyRequestException()
        {
            // Arrange
            string expected = $"secret-value-{Guid.NewGuid()}";
            string secretName = $"secret-name-{Guid.NewGuid()}";
            KeyVaultSecretProvider provider = CreateSecretProviderWithTooManyRequestSimulation(expected);

            // Act
            string actual = await provider.GetRawSecret(secretName);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task KeyVaultSecretProvider_GetsRawSecretAsync_AfterRetriedTooManyRequestException()
        {
            // Arrange
            string expected = $"secret-value-{Guid.NewGuid()}";
            string secretName = $"secret-name-{Guid.NewGuid()}";
            KeyVaultSecretProvider provider = CreateSecretProviderWithTooManyRequestSimulation(expected);

            // Act
            string actual = await provider.GetRawSecretAsync(secretName);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task KeyVaultSecretProvider_GetsSecret_AfterRetriedTooManyRequestException()
        {
            // Arrange
            string expected = $"secret-value-{Guid.NewGuid()}";
            string secretName = $"secret-name-{Guid.NewGuid()}";
            KeyVaultSecretProvider provider = CreateSecretProviderWithTooManyRequestSimulation(expected);

            // Act
            Secret actual = await provider.GetSecret(secretName);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(expected, actual.Value);
            Assert.NotNull(actual.Version);
        }

        [Fact]
        public async Task KeyVaultSecretProvider_GetsSecretAsync_AfterRetriedTooManyRequestException()
        {
            // Arrange
            string expected = $"secret-value-{Guid.NewGuid()}";
            string secretName = $"secret-name-{Guid.NewGuid()}";
            KeyVaultSecretProvider provider = CreateSecretProviderWithTooManyRequestSimulation(expected);

            // Act
            Secret actual = await provider.GetSecretAsync(secretName);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(expected, actual.Value);
            Assert.NotNull(actual.Version);
        }

        private static KeyVaultSecretProvider CreateSecretProviderWithTooManyRequestSimulation(string expected)
        {
            // Arrange
            var keyVaultClient = new SimulatedKeyVaultClient(
                () => throw new KeyVaultErrorException("Sabotage secret retrieval with TooManyRequests")
                {
                    Response = new HttpResponseMessageWrapper(
                        new HttpResponseMessage(HttpStatusCode.TooManyRequests),
                        "some HTTP response content to ignore")
                },
                () => new SecretBundle(value: expected, id: $"http://requires-3-or-4-segments/secrets/with-the-second-named-secrets-{Guid.NewGuid()}"));


            var provider = new KeyVaultSecretProvider(
                new StubKeyVaultAuthenticator(keyVaultClient),
                new KeyVaultConfiguration(GenerateVaultUri()));

            return provider;
        }

        private static string GenerateVaultUri()
        {
            return $"https://{Guid.NewGuid():N}.vault.azure.net/";
        }


    }
}
