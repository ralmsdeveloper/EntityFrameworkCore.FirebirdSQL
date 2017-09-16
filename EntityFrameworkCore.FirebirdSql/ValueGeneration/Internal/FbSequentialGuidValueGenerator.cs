/*
 *          Copyright (c) 2017 Rafael Almeida (ralms@ralms.net)
 *
 *                    EntityFrameworkCore.FirebirdSql
 *
 * THIS MATERIAL IS PROVIDED AS IS, WITH ABSOLUTELY NO WARRANTY EXPRESSED
 * OR IMPLIED.  ANY USE IS AT YOUR OWN RISK.
 * 
 * Permission is hereby granted to use or copy this program
 * for any purpose,  provided the above notices are retained on all copies.
 * Permission to modify the code and to distribute modified code is granted,
 * provided the above notices are retained, and a notice that the code was
 * modified is included with the above copyright notice.
 *
 */

using System;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace Microsoft.EntityFrameworkCore.ValueGeneration.Internal
{
    public class FbSequentialGuidValueGenerator  : ValueGenerator<Guid>
    {

        private readonly IFbOptions _options; 
        public FbSequentialGuidValueGenerator(IFbOptions options)
       =>  _options = options;
        

        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

        /// <summary>
        ///     Gets a value to be assigned to a property.
        ///     Creates a GUID where the first 8 bytes are the current UTC date/time (in ticks)
        ///     and the last 8 bytes are cryptographically random.  This allows for better performance
        ///     in clustered index scenarios.
        /// </summary>
        /// <para>The change tracking entry of the entity for which the value is being generated.</para>
        /// <returns> The value to be assigned to a property. </returns>
        public override Guid Next(EntityEntry entry)
        {
            var randomBytes = new byte[8];
            Rng.GetBytes(randomBytes);
            var ticks = (ulong)DateTime.UtcNow.Ticks*2;

            var guidBytes = new byte[16];
            var tickBytes = BitConverter.GetBytes(ticks);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(tickBytes);

            Buffer.BlockCopy(tickBytes, 0, guidBytes, 0, 8);
            Buffer.BlockCopy(randomBytes, 0, guidBytes, 8, 8); 
            return new Guid(guidBytes);

        }

        /// <summary>
        ///     Gets a value indicating whether the values generated are temporary or permanent. This implementation
        ///     always returns false, meaning the generated values will be saved to the database.
        /// </summary>
        public override bool GeneratesTemporaryValues => false;

    }
}
