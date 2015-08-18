using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NoCaptcha
{
    [ToolboxData("<{0}:NoCaptchaControl runat=server />")]
    public class NoCaptchaControl : WebControl, IValidator
    {
        [Bindable(true)]
        [Category("Keys")]
        [Description("This is your site / public key for Google NoCAPTCHA")]
        public string SiteKey { get; set; }

        [Bindable(true)]
        [Category("Keys")]
        [Description("This is your site / public key for Google NoCAPTCHA")]
        public string SecretKey { get; set; }

        public bool IsValid { get; set; }

        public string ErrorMessage { get; set; }

        public NoCaptchaControl()
            : base(HtmlTextWriterTag.Div)
        {
        }

        protected override void OnInit(EventArgs e)
        {
            Page.Validators.Add(this);
            base.OnInit(e);
        }


        public void Validate()
        {
            NoCaptcha nc = (string.IsNullOrEmpty(SiteKey) || string.IsNullOrEmpty(SecretKey)) ?
                new NoCaptcha() :
                new NoCaptcha(SiteKey, SecretKey);

            string clientResponse = GetClientResponse();
            if (string.IsNullOrEmpty(clientResponse))
            {
                ErrorMessage = "Please tick the \"I'm not a robot\" checkbox";
                IsValid = false;
                return;
            }
            var result = nc.Validate(clientResponse, GetClientIP());
            IsValid = result.Succeeded;
            if (IsValid)
            {
                return;
            }

            List<string> errors = new List<string>();
            if (result.ErrorMessages == null)
            {
                errors.Add("Unspecified Error");
            }
            else
            {
                foreach (string error in result.ErrorMessages)
                {
                    switch (error)
                    {
                        case "missing-input-secret":
                            errors.Add("The secret parameter is missing");
                            break;
                        case "invalid-input-secret":
                            errors.Add("The secret parameter is invalid or malformed");
                            break;
                        case "missing-input-response":
                            errors.Add("The response parameter is missing");
                            break;
                        case "invalid-input-response":
                            errors.Add("The response parameter is invalid or malformed");
                            break;
                        default:
                            errors.Add(error);
                            break;
                    }
                }
            }
            ErrorMessage = string.Join(", ", errors);
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            NoCaptcha nc = (string.IsNullOrEmpty(SiteKey) || string.IsNullOrEmpty(SecretKey)) ?
                new NoCaptcha() :
                new NoCaptcha(SiteKey, SecretKey);

            output.Write(nc.GetSecureHtml());

            if (!IsValid)
            {
                output.Write($"<div class=\"text-danger\">{ErrorMessage}</div>");
            }

            output.Write("<script src=\"https://www.google.com/recaptcha/api.js\"></script>");
        }

        private string GetClientResponse()
        {
            return HttpContext.Current.Request.Form["g-recaptcha-response"];
        }

        private string GetClientIP()
        {
            string ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }
    }
}
