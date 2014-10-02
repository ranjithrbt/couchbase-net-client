﻿using System;
using Couchbase.Configuration.Server.Serialization;
using Couchbase.IO.Converters;

namespace Couchbase.IO.Operations
{
    internal sealed class Config : OperationBase<BucketConfig>
    {
        public Config(IByteConverter converter)
            : base(converter)
        {
        }

        public override byte[] CreateExtras()
        {
            Format = DataFormat.Json;
            Flags = new Flags
            {
                Compression = Compression.None,
                DataFormat = Format,
                TypeCode = TypeCode.Object
            };
            return new byte[0];
        }

        public override void ReadExtras(byte[] buffer)
        {
            Format = DataFormat.Json;
            Flags = new Flags
            {
                Compression = Compression.None,
                DataFormat = Format,
                TypeCode = TypeCode.Object
            };
        }

        public override OperationCode OperationCode
        {
            get { return OperationCode.GetClusterConfig; }
        }

        public override int BodyOffset
        {
            get { return 24; }
        }
    }
}

#region [ License information ]

/* ************************************************************
 *
 *    @author Couchbase <info@couchbase.com>
 *    @copyright 2014 Couchbase, Inc.
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * ************************************************************/

#endregion [ License information ]