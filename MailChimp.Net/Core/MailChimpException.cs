﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MailChimpException.cs" company="Brandon Seydel">
//   N/A
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace MailChimp.Net.Core
{
    /// <summary>
    /// The exception that comes back from Mail Chimp when an invalid operation has occured.
    /// </summary>
    public class MailChimpException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MailChimpException"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref>
        ///         <name>name</name>
        ///     </paramref>
        ///     is null. </exception>
        /// <exception cref="InvalidCastException">The value associated with <paramref>
        ///         <name>name</name>
        ///     </paramref>
        ///     cannot be converted to a <see cref="T:System.String" />. </exception>
        /// <exception cref="SerializationException">An element with the specified name is not found in the current instance. </exception>
        // ReSharper disable once UnusedParameter.Local
        public MailChimpException(SerializationInfo info, StreamingContext context)
        {
            var errorText = string.Empty;

            try
            {
                this.Detail = info?.GetString("detail");
                this.Title = info?.GetString("title");
                this.Type = info?.GetString("type");
                this.Status = info?.GetInt32("status") ?? 0;
                this.Instance = info?.GetString("instance");

                errorText =
                    $"Title: {this.Title + Environment.NewLine} Type: {this.Type + Environment.NewLine} Status: {this.Status + Environment.NewLine} + Detail: {this.Detail + Environment.NewLine}";
                this.Errors = (List<Error>) info?.GetValue("errors", typeof(List<Error>));
                errorText += "Errors: " + string.Join(" : ", this.Errors.Select(x => x.Field + " " + x.Message));
            }
            catch
            {
                // ignored
            }
            finally
            {
                Trace.Write(errorText);
                Console.Error.WriteAsync(errorText);
            }
		}

		public List<Error> Errors { get; set; }

		public class Error
		{
			[JsonProperty("field")]
			public string Field { get; set; }
			[JsonProperty("message")]
			public string Message { get; set; }
		}


		/// <summary>
		/// Gets or Sets a human-readable explanation specific to this occurrence of the problem. Learn more about errors.
		/// </summary>
		public string Detail { get; set; }

        /// <summary>
        /// Gets or sets a string that identifies this specific occurrence of the problem. Please provide this ID when contacting support.
        /// </summary>
        public string Instance { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code (RFC2616, Section 6) generated by the origin server for this occurrence of the problem.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets a short, human-readable summary of the problem type. It shouldn't change based on the occurrence of the problem, except for purposes of localization.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets an absolute URI that identifies the problem type. When dereferenced, it should provide human-readable documentation for the problem type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The get object data.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <exception cref="ArgumentNullException">The <paramref name="info" /> parameter is a null reference (Nothing in Visual Basic). </exception>
        /// <exception cref="SerializationException">A value has already been associated with <paramref>
        ///         <name>name</name>
        ///     </paramref>
        ///     . </exception>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("detail", this.Detail);
            info.AddValue("title", this.Title);
            info.AddValue("type", this.Type);
            info.AddValue("status", this.Status);
            info.AddValue("instance", this.Instance);
			info.AddValue("errors", this.Errors);
		}
    }
}