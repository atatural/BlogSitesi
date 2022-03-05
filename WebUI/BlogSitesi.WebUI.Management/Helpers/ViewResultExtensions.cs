using BlogSitesi.WebUI.Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace BlogSitesi.WebUI.Management.Helpers
{
    public class ViewResultExtensions
    {
        public static string ViewResult(ViewModelResult viewModelResult)
        {
            if (viewModelResult == null)
                return "";

            if (viewModelResult.Errors.Count()>0 && !viewModelResult.IsSucceed )
            {
                var return_html = new StringBuilder($"<div class='alert alert-danger'><p>{viewModelResult.Message}</p><ul>");
                foreach (var item in viewModelResult.Errors)
                {
                    return_html.Append($"<li>{item}</li>");
                }
                return_html.Append("</ul></div>");

                return return_html.ToString();
            }

            if (!viewModelResult.IsSucceed && !string.IsNullOrEmpty(viewModelResult.Message))
            {
                var return_html = new StringBuilder($"<div class='alert alert-danger'> Hata : {viewModelResult.Message} </div>");
                return return_html.ToString();
            }

            if (viewModelResult.IsSucceed && !string.IsNullOrEmpty(viewModelResult.Message))
            {
                var return_html = new StringBuilder($"<div class='alert alert-success'> Başarılı! : {viewModelResult.Message} </div>");
                return return_html.ToString();
            }

            return "";
        }
    }
}
