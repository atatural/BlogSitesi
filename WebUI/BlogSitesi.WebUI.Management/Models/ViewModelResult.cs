using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogSitesi.WebUI.Management.Models
{
    public class ViewModelResult
    {
        public ViewModelResult()
        {
            IsSucceed = true;
            Errors = new List<string>();

        }
        public ViewModelResult(bool _isSucceed, string _message)
        {
            IsSucceed = _isSucceed;
            Message = _message;
            Errors = new List<string>();
        }

        public ViewModelResult(bool _isSucceed, string _message, string _errors)
        {
            IsSucceed = _isSucceed;
            Message = _message;
            Errors = string.IsNullOrEmpty(_errors) ? new List<string>() : new List<string>() { _errors };
        }
        public ViewModelResult(bool _isSucceed, string _message, List<String> _errors)
        {
            IsSucceed = _isSucceed;
            Message = _message;
            Errors = _errors;
        }

        public bool IsSucceed { get; set; }
        public string Message { get; set; }
        public List<String> Errors { get; set; }


    }
}
