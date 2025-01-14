﻿using System;
using Arcus.Security.Secrets.Core.Interfaces;
using GuardNet;

namespace Arcus.Security.Secrets.Core.Models
{
    /// <summary>
    /// Represents the secret returned from the <see cref="ISecretProvider"/> implementation.
    /// </summary>
    public class Secret
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Secret"/> class.
        /// </summary>
        /// <param name="value">The secret value.</param>
        /// <param name="version">The version of the secret.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> cannot be <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="version"/> cannot be <c>null</c>.</exception>
        public Secret(string value, string version)
        {
            Guard.NotNull(value, nameof(value));
            Guard.NotNull(version, nameof(version));

            Value = value;
            Version = version;
        }

        /// <summary>
        /// Gets the secret value.
        /// </summary>
        public string Value { get;}

        /// <summary>
        /// Gets the version of the secret.
        /// </summary>
        /// <remarks>Version is not checked for <c>null</c>.</remarks>
        public string Version { get; }
    }
}
