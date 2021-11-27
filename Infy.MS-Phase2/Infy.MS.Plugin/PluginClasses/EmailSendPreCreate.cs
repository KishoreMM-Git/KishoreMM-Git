using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xrm;
using XRMExtensions;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using System.Net;
using System.Xml.Linq;
using System.Security.Permissions;
using System.Security;
using Microsoft.Win32;
namespace Infy.MS.Plugins.PluginClasses
{
    public class EmailSendPreCreate:BasePlugin
    {
        public override void ExecutePlugin(IExtendedPluginContext context)
        {
            if (context == null)
            {
                throw new InvalidPluginExecutionException("Context not found");
            }

            if (context.MessageName.ToLower() != "create") { return; }

            if (context.PrimaryEntityName.ToLower() != Email.EntityName) { return; }

            var email = context.GetTargetEntity<Entity>();

            
                Entity template = context.Retrieve("template", new Guid("6d3721a5-2138-ea11-a813-000d3a4f6db7"), new ColumnSet(true));
                string body = GetDataFromXml(template.GetAttributeValue<string>("body"), "match"); 
                 string imgsrc = "Thanks";//CaptureUrl();
                body = body.Replace("[image here]", imgsrc);
                email[Email.Description] = body;
                context.Update(email);        

        }
        public static string CaptureUrl()
        {
            string response = string.Empty;
            PermissionSet perms = new PermissionSet(null);
            perms.AddPermission(new UIPermission(PermissionState.Unrestricted));
            perms.AddPermission(new RegistryPermission(PermissionState.None));
            perms.PermitOnly();

            try
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey("http://losst.movement.directory/lo/70961/email");
               // Console.WriteLine("Registry key: {0}", key.Name);
                using (WebClient client = new WebClient())
                {

                    byte[] responseBytes = client.DownloadData("http://losst.movement.directory/lo/70961/email");
                    response = Encoding.UTF8.GetString(responseBytes);
                }
            }
            catch (SecurityException e)
            {
                Console.WriteLine("Security Exception:\n\n{0}", e.Message);
            }
           
           
            //Uri uri = new Uri("http://losst.movement.directory/lo/70961/email");
            //WebClient client = new WebClient();
            //string downloadString = client.DownloadString(uri);
            return response;
        }
        private static string GetDataFromXml(string value, string attributeName)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            XDocument document = XDocument.Parse(value);
            // get the Element with the attribute name specified  
            XElement element = document.Descendants().Where(ele => ele.Attributes().Any(attr => attr.Name == attributeName)).FirstOrDefault();
            return element == null ? string.Empty : element.Value;
        }
        public override IEnumerable<RegisteredEvent> GetRegisteredEvents()
        {
            yield return new Xrm.RegisteredEvent(PipelineStage.PreOperation, SdkMessageProcessingStepMode.Synchronous, "Create");
        }
    }
}
