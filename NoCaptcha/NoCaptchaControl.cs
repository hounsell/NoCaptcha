using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gnome.NoCaptcha
{
   [ToolboxData("<{0}:NoCaptchaControl runat=server></{0}:NoCaptchaControl>")]
   public class NoCaptchaControl : BaseValidator
   {

      public NoCaptchaTheme Theme { get; set; }

      public NoCaptchaType Type { get; set; }

      public NoCaptchaSize Size { get; set; }

      public bool UseSecureToken { get; set; }

      public string ErrorCssClass { get; set; }

      [Category("Keys")]
      [Description("This is your site / public key for Google NoCAPTCHA")]
      public string SiteKey { get; set; }

      [Category("Keys")]
      [Description("This is your site / public key for Google NoCAPTCHA")]
      public string SecretKey { get; set; }

      public NoCaptchaControl()
          : base()
      {
         Theme = NoCaptchaTheme.Light;
         Type = NoCaptchaType.Image;
         Size = NoCaptchaSize.Normal;
         UseSecureToken = true;
         ErrorCssClass = "text-danger";
         ErrorMessage = "Please tick the \"I'm not a robot\" checkbox";
      }

      protected override void OnPreRender(EventArgs e)
      {
         if (!Page.ClientScript.IsClientScriptIncludeRegistered("NoCaptcha"))
         {
            Page.ClientScript.RegisterClientScriptInclude("NoCaptcha", "https://www.google.com/recaptcha/api.js");
         }

         base.OnPreRender(e);
      }

      public override void RenderControl(HtmlTextWriter writer)
      {
         NoCaptcha nc = (string.IsNullOrEmpty(SiteKey) || string.IsNullOrEmpty(SecretKey)) ?
             new NoCaptcha() :
             new NoCaptcha(SiteKey, SecretKey);

         writer.AddAttribute(HtmlTextWriterAttribute.Class, "g-recaptcha");
         writer.AddAttribute("data-sitekey", nc.SiteKey);
         if (UseSecureToken)
         {
            writer.AddAttribute("data-stoken", nc.SecureToken);
         }
         writer.AddAttribute("data-theme", Theme.ToString().ToLower());
         writer.AddAttribute("data-type", Type.ToString().ToLower());
         writer.AddAttribute("data-size", Size.ToString().ToLower());

         writer.RenderBeginTag(HtmlTextWriterTag.Div);
         writer.RenderEndTag();

         if (!IsValid && (HttpContext.Current.Handler as Page).IsPostBack)
         {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, ErrorCssClass);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.WriteEncodedText(ErrorMessage);
            writer.RenderEndTag();
         }
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

      protected override bool ControlPropertiesValid()
      {
         return true;
      }

      protected override bool EvaluateIsValid()
      {
         NoCaptcha nc = (string.IsNullOrEmpty(SiteKey) || string.IsNullOrEmpty(SecretKey)) ?
             new NoCaptcha() :
             new NoCaptcha(SiteKey, SecretKey);

         string clientResponse = GetClientResponse();
         if (string.IsNullOrEmpty(clientResponse))
         {
            return false;
         }
         var result = nc.Validate(clientResponse, GetClientIP());
         IsValid = result.Succeeded;
         if (IsValid)
         {
            return true;
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

         return false;
      }
   }
}
