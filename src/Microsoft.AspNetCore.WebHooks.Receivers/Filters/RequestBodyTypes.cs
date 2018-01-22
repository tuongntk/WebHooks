// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Microsoft.AspNetCore.WebHooks.Filters
{
    internal static class RequestBodyTypes
    {
        private static readonly MediaTypeHeaderValue ApplicationJsonMediaType
            = new MediaTypeHeaderValue("application/json");
        private static readonly MediaTypeHeaderValue ApplicationXmlMediaType
            = new MediaTypeHeaderValue("application/xml");
        private static readonly MediaTypeHeaderValue TextJsonMediaType = new MediaTypeHeaderValue("text/json");
        private static readonly MediaTypeHeaderValue TextXmlMediaType = new MediaTypeHeaderValue("text/xml");

        /// <summary>
        /// Determines whether the specified request contains JSON as indicated by a content type of
        /// <c>application/json</c>, <c>text/json</c> or <c>application/xyz+json</c>. The term <c>xyz</c> can for
        /// example be <c>hal</c> or some other JSON-derived media type.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest"/> to check.</param>
        /// <returns>
        /// <see langword="true"/> if the specified request contains JSON content; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsJson(HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var contentType = request.GetTypedHeaders().ContentType;
            if (contentType == null)
            {
                return false;
            }

            if (contentType.IsSubsetOf(ApplicationJsonMediaType) || contentType.IsSubsetOf(TextJsonMediaType))
            {
                return true;
            }

            // MVC's JsonInputFormatter does not support text/*+json by default. RFC 3023 and 6839 allow */*+json but
            // https://www.iana.org/assignments/media-types/media-types.xhtml shows all +json registrations except
            // model/gltf+json match application/*+json.
            return contentType.Type.Equals("application", StringComparison.OrdinalIgnoreCase) &&
                contentType.SubType.EndsWith("+json", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the specified request contains XML as indicated by a content type of
        /// <c>application/xml</c>, <c>text/xml</c> or <c>application/xyz+xml</c>. The term <c>xyz</c> can for example
        /// be <c>rdf</c> or some other XML-derived media type.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest"/> to check.</param>
        /// <returns>
        /// <see langword="true"/> if the specified request contains XML content; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsXml(HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var contentType = request.GetTypedHeaders().ContentType;
            if (contentType == null)
            {
                return false;
            }

            if (contentType.IsSubsetOf(ApplicationXmlMediaType) || contentType.IsSubsetOf(TextXmlMediaType))
            {
                return true;
            }

            // MVC's XML input formatters do not support text/*+xml by default. RFC 3023 and 6839 allow */*+xml but
            // https://www.iana.org/assignments/media-types/media-types.xhtml shows almost all +xml registrations
            // match application/*+xml and none match text/*+xml.
            return contentType.Type.Equals("application", StringComparison.OrdinalIgnoreCase) &&
                contentType.SubType.EndsWith("+xml", StringComparison.OrdinalIgnoreCase);
        }
    }
}
