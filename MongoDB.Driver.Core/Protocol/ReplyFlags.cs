﻿/* Copyright 2010-2013 10gen Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;

namespace MongoDB.Driver.Core.Protocol
{
    /// <summary>
    /// Flags sent with an <see cref="OpCode.Reply"/>.
    /// </summary>
    [Flags]
    public enum ReplyFlags
    {
        /// <summary>
        /// The cursor was not found.
        /// </summary>
        CursorNotFound = 1,

        /// <summary>
        /// The query failed.
        /// </summary>
        QueryFailure = 2,

        /// <summary>
        /// The server is await capable.
        /// </summary>
        AwaitCapable = 8
    }
}

