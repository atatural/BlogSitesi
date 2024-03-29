﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogSitesi.WebUI.Infrastructure.Rules
{
    public class RedirectToHttpsWwwNonWwwRule : IRule
    {
        #region Variables
        public int status_code { get; set; }
        public bool redirect_to_www { get; set; }
        public bool redirect_to_non_www { get; set; }
        public bool redirect_to_https { get; set; }
        public bool append_slash { get; set; }
        public string[] hosts_to_ignore { get; set; }
        #endregion
        #region Constructors
        /// <summary>
        /// Create a new rule
        /// </summary>
        public RedirectToHttpsWwwNonWwwRule()
        {
            // Set values for instance variables
            this.status_code = 301;
            this.redirect_to_www = false;
            this.redirect_to_non_www = false;
            this.redirect_to_https = false;
            this.append_slash = false;
            this.hosts_to_ignore = new string[] { "localhost" };
        } // End of the constructor
        #endregion
        #region methods
        /// <summary>
        /// Apply the rule
        /// </summary>
        public virtual void ApplyRule(RewriteContext context)
        {
            // Get the request
            HttpRequest req = context.HttpContext.Request;
            if (req.Path.Value.EndsWith(".jpg")
                || req.Path.Value.EndsWith(".webp")
                || req.Path.Value.EndsWith(".png")
                || req.Path.Value.EndsWith(".css")
                || req.Path.Value.EndsWith(".js")
                || req.Path.Value.EndsWith(".svg")
                || req.Path.Value.EndsWith(".woff")
                || req.Path.Value.EndsWith(".woff2")
                || req.Path.Value.EndsWith(".txt")
                || req.Path.Value.EndsWith(".pdf")
                )
            {
                context.Result = RuleResult.ContinueRules;
                return;
            }

            // Get the host
            string host = req.Host.Host;
            // Create an uri builder with the current request
            UriBuilder uriBuilder = new UriBuilder(host + req.Path + req.QueryString);
            uriBuilder.Scheme = req.Scheme;
            uriBuilder.Port = req.IsHttps == true ? 443 : 80;
            // Check hosts to ignore
            for (int i = 0; i < hosts_to_ignore.Length; i++)
            {
                if (host.Equals(hosts_to_ignore[i], StringComparison.OrdinalIgnoreCase))
                {
                    context.Result = RuleResult.ContinueRules;
                    return;
                }
            }

            // Check if we should do a https redirect
            if (this.redirect_to_https == true && req.IsHttps == false)
            {
                // Add https scheme and port
                uriBuilder.Scheme = "https";
                uriBuilder.Port = 443;

                // Check if we should do a www redirect
                if (this.redirect_to_www == true && this.redirect_to_non_www == false && host.StartsWith("www") == false)
                {
                    uriBuilder.Host = "www." + uriBuilder.Host;
                }
                else if (this.redirect_to_non_www == true && this.redirect_to_www == false && host.StartsWith("www") == true)
                {
                    uriBuilder.Host = uriBuilder.Host.Replace("www.", "");
                }

                if (this.append_slash == true && req.Path.HasValue && !req.Path.Value.EndsWith('/'))
                    uriBuilder.Path = uriBuilder.Path + "/";

                // Do a redirect
                HttpResponse response = context.HttpContext.Response;
                response.StatusCode = this.status_code;
                response.Headers[HeaderNames.Location] = uriBuilder.Uri.AbsoluteUri;
                context.Result = RuleResult.EndResponse;
                return;
            }
            else if (this.redirect_to_www == true && this.redirect_to_non_www == false && host.StartsWith("www.") == false)
            {
                // Modify the host
                uriBuilder.Host = "www." + uriBuilder.Host;

                if (this.append_slash == true && req.Path.HasValue && !req.Path.Value.EndsWith('/'))
                    uriBuilder.Path = uriBuilder.Path + "/";

                // Do a redirect
                HttpResponse response = context.HttpContext.Response;
                response.StatusCode = this.status_code;
                response.Headers[HeaderNames.Location] = uriBuilder.Uri.AbsoluteUri;
                context.Result = RuleResult.EndResponse;
                return;
            }
            else if (this.redirect_to_non_www == true && this.redirect_to_www == false && host.StartsWith("www.") == true)
            {
                // Modify the url
                uriBuilder.Host = uriBuilder.Host.Replace("www.", "");

                if (this.append_slash == true && req.Path.HasValue && !req.Path.Value.EndsWith('/'))
                    uriBuilder.Path = uriBuilder.Path + "/";

                // Do a redirect
                HttpResponse response = context.HttpContext.Response;
                response.StatusCode = this.status_code;
                response.Headers[HeaderNames.Location] = uriBuilder.Uri.AbsoluteUri;
                context.Result = RuleResult.EndResponse;
                return;
            }
            else
            {
                if (this.append_slash == true && req.Path.HasValue && !req.Path.Value.EndsWith('/'))
                {
                    uriBuilder.Path = uriBuilder.Path + "/";
                    // Do a redirect
                    HttpResponse response = context.HttpContext.Response;
                    response.StatusCode = this.status_code;
                    response.Headers[HeaderNames.Location] = uriBuilder.Uri.AbsoluteUri;
                    context.Result = RuleResult.EndResponse;
                    return;
                }

                context.Result = RuleResult.ContinueRules;
                return;
            }
        } // End of the ApplyRule method
        #endregion
    }
}
