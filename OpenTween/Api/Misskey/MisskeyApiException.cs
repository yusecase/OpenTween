// OpenTween - Client of Twitter
// Copyright (c) 2024 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
// All rights reserved.
//
// This file is part of OpenTween.
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

#nullable enable

using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;

namespace OpenTween.Api.Misskey
{
    public class MisskeyApiException : WebApiException
    {
        public HttpStatusCode StatusCode { get; }

        public MisskeyError? ErrorResponse { get; }

        public MisskeyApiException()
        {
        }

        public MisskeyApiException(string message)
            : base(message)
        {
        }

        public MisskeyApiException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public MisskeyApiException(HttpStatusCode statusCode, string responseText)
            : base(statusCode.ToString(), responseText)
        {
            this.StatusCode = statusCode;
        }

        public MisskeyApiException(HttpStatusCode statusCode, MisskeyError error, string responseText)
            : base(error.Error.Message, responseText)
        {
            this.StatusCode = statusCode;
            this.ErrorResponse = error;
        }

        protected MisskeyApiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.StatusCode = (HttpStatusCode)info.GetValue("StatusCode", typeof(HttpStatusCode));
            this.ErrorResponse = (MisskeyError?)info.GetValue("ErrorResponse", typeof(MisskeyError));
        }

        private MisskeyApiException(string message, string responseText, Exception innerException)
            : base(message, responseText, innerException)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("StatusCode", this.StatusCode);
            info.AddValue("ErrorResponse", this.ErrorResponse);
        }

        public static MisskeyApiException CreateFromException(HttpRequestException ex)
            => new(ex.InnerException?.Message ?? ex.Message, ex);

        public static MisskeyApiException CreateFromException(OperationCanceledException ex)
            => new("Timeout", ex);

        public static MisskeyApiException CreateFromException(SerializationException ex, string responseText)
            => new("Invalid JSON", responseText, ex);
    }
}
