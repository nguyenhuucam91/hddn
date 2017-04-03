using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace HDNHD.Core.Models
{
    /// <summary>
    /// Data: html, json, xml...
    /// </summary>
    public class AjaxResult : JsonResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public bool ForceRefresh { get; set; }

        public AjaxResult()
            : base()
        {
            ForceRefresh = false;
            IsSuccess = true;
            JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            Data = new
            {
                IsSuccess = IsSuccess,
                Message = Message,
                ForceRefresh = ForceRefresh,
                Data = Data
            };

            base.ExecuteResult(context);
        }

        public static AjaxResult Fail(string message)
        {
            return new AjaxResult()
            {
                IsSuccess = false,
                Message = message
            };
        }

        public static string ToJson(object data)
        {
            return JsonConvert.SerializeObject(data);
        }

        public static string ToXml<T>(T data)
        {
            if (data == null) return string.Empty;

            var xmlSerializer = new XmlSerializer(typeof(T));

            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
                {
                    xmlSerializer.Serialize(xmlWriter, data);
                    return stringWriter.ToString();
                }
            }
        }
    }
}