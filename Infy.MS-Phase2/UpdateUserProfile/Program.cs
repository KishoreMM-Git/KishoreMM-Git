using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace UpdateUserProfile
{
    public class Program
    {
        public static string signatureConfiguration = string.Empty;
        public static void Main(string[] args)
        {
            bool batcjJobStatus = true;
            string batchJobLog = string.Empty;
            Guid batchJobRecordId = Guid.Empty;
            List<USer> ActiveUSer = null;
            Common objCommon = new Common();
            Dictionary<string, string> dcConfigs = null;
            string secret = string.Empty;//
            string clientId = string.Empty;
            string accessToken = string.Empty;
            string Url = string.Empty;
            //fetch Azure configurations from App.config to GetAccessToken
            string authority = ConfigurationManager.AppSettings[Constants.Authority].ToString();
            //clientId = ConfigurationManager.AppSettings[Constants.ClientId].ToString();
            //secret = ConfigurationManager.AppSettings[Constants.Secret].ToString();
            string serviceUrl = ConfigurationManager.AppSettings[Constants.ServiceUrl].ToString();
            string keyVaultUrl = ConfigurationManager.AppSettings["KeyVaultUrl"].ToString();
            string clientIdSecretName = ConfigurationManager.AppSettings["clientIdSecretName"].ToString();
            string cientSecretName = ConfigurationManager.AppSettings["ClientSecretName"].ToString();

            try
            {
                //Console.WriteLine("Azure key Vault Client keyVaultUrl Value:" + keyVaultUrl);
                //Console.WriteLine("Azure key Vault Client clientIdSecretName Value:" + clientIdSecretName);
                //Console.WriteLine("Azure key Vault Client cientSecretName Value:" + cientSecretName);

                clientId = objCommon.GetSecretValue(keyVaultUrl, clientIdSecretName).Result;
                Console.WriteLine("Azure key Vault ClientId secret Value:" + clientId);
                secret = objCommon.GetSecretValue(keyVaultUrl, cientSecretName).Result;
                Console.WriteLine("Azure key Vault Client secret Value:" + secret);
                batchJobLog += "---Batch Job Started---" + DateTime.Now + Environment.NewLine;
                batchJobLog += "<<=================================================>>" + Environment.NewLine;

                batchJobRecordId = Common.CreateBatchJobRecord(authority, clientId, secret, serviceUrl, Constants.batchJobName).Result;
                batchJobLog += "Batch Job Record Created" + Environment.NewLine;
                accessToken = objCommon.GetAccessToken(authority, clientId, secret, serviceUrl);
                //accessToken = objCommon.GetAccessToken(authority, "dbbfed03-5023-410b-b588-8db70a61c8af", "rFzxuRtO20n@BywqSIn_i7Y]5Q]nnAMQ", serviceUrl);
                if (!string.IsNullOrEmpty(accessToken))
                {
                    batchJobLog += "Access Token retrieved" + Environment.NewLine;
                    dcConfigs = objCommon.FetchConfigurations(accessToken, serviceUrl, Constants.configSetUpName);
                    if (dcConfigs != null && dcConfigs.Count > 0)
                    {
                        batchJobLog += "Configuration exist in CRM for updateuserprofile" + Environment.NewLine;
                        if (dcConfigs.ContainsKey(Constants.webserviceurl)) Url = dcConfigs[Constants.webserviceurl].ToString();
                        if (dcConfigs.ContainsKey(Constants.SignatureConfiguration)) signatureConfiguration = dcConfigs[Constants.SignatureConfiguration].ToString();
                    }
                    if (!string.IsNullOrEmpty(Url))
                    {
                        ActiveUSer = objCommon.FetchActiveUser(accessToken, serviceUrl);
                        if (ActiveUSer != null && ActiveUSer.Count > 0)
                        {
                            batchJobLog += "Activeuser Record Count :" + ActiveUSer.Count + Environment.NewLine;
                            foreach (USer user in ActiveUSer)
                            {
                                try
                                {
                                    string webserviceurl = string.Empty;
                                    string nmls = string.Empty;
                                    Guid userid = Guid.Empty;
                                    List<JObject> data = null;
                                    if (!string.IsNullOrEmpty(user.USERID))
                                    {
                                        userid = Guid.Parse(user.USERID);
                                    }
                                    if (!string.IsNullOrEmpty(user.Slug))
                                    {
                                        batchJobLog += "Slug Name: " + user.Slug + Environment.NewLine;
                                        webserviceurl = Url + (user.Slug);
                                        Console.WriteLine("webserviceurl:" + webserviceurl);
                                        data = Common.callnmlswebservice(webserviceurl).Result;

                                    }
                                    if ((data == null || data.Count == 0) && (!string.IsNullOrEmpty(user.FirstName)) && (!string.IsNullOrEmpty(user.LastName)))
                                    {
                                        batchJobLog += "Slug Name:" + " " + user.FirstName + "-" + user.LastName + Environment.NewLine;
                                        webserviceurl = Url + (user.FirstName + "-" + user.LastName);
                                        Console.WriteLine("webserviceurl:" + webserviceurl);
                                        data = Common.callnmlswebservice(webserviceurl).Result;

                                    }
                                    if ((data == null || data.Count == 0) && (!string.IsNullOrEmpty(user.DomainName)))
                                    {
                                        var domainName = user.DomainName.Split('@');
                                        var slug = domainName[0];
                                        webserviceurl = Url + (slug);
                                        batchJobLog += "Slug Name:" + " " + slug + Environment.NewLine;
                                        data = Common.callnmlswebservice(webserviceurl).Result;
                                    }
                                    
                                    if (data != null && data.Count > 0)
                                    {
                                        try
                                        {
                                            Task.WaitAll(Task.Run(async () => await updateuserdata(data, userid, serviceUrl, accessToken)));
                                            batchJobLog += ""+ user.FirstName +"-"+user.LastName +" Data Updated" + Environment.NewLine;
                                        }
                                        catch (Exception ex)
                                        {
                                            batcjJobStatus = false;
                                            Console.WriteLine("" + user.FirstName + "-" + user.LastName + " Data Updation Failed");
                                            //batchJobLog += "" + user.FirstName + "-" + user.LastName + " Data Updation Failed" + Environment.NewLine;
                                           // batchJobLog += ex.InnerException + Environment.NewLine;
                                        }
                                        foreach (JObject entity in data)
                                        {
                                            var nestedObject = entity.Value<JObject>("acf").Properties();
                                            if (nestedObject != null)
                                            {
                                                Dictionary<string, string> dict = new Dictionary<string, string>() { };
                                                var nestedObjectDict = nestedObject.ToDictionary(k => k.Name, v => v.Value.ToString());
                                                var values = nestedObjectDict.Where(pv => pv.Key.StartsWith("testimonial"));
                                                var list = values.ToList();
                                                foreach (KeyValuePair<string, string> Testimonial in list)
                                                {
                                                    try
                                                    {
                                                        if (!string.IsNullOrEmpty(Testimonial.Key))
                                                        {
                                                            if (!string.IsNullOrEmpty(Testimonial.Value))
                                                            {
                                                                string Testimonialname = Testimonial.Key;
                                                                string TestimonialnameDescprition = Testimonial.Value;
                                                                bool record = objCommon.fetchtestimonial(accessToken, serviceUrl, Testimonialname, userid);
                                                                if (record != true)
                                                                {
                                                                    Task.WaitAll(Task.Run(async () => await createRecordinTestimonial(Testimonialname, TestimonialnameDescprition, userid, serviceUrl, accessToken)));
                                                                   batchJobLog += "Testimonial Created :" + Testimonialname + Environment.NewLine;

                                                                }

                                                            }
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        batcjJobStatus = false;
                                                        batchJobLog += ex.InnerException + Environment.NewLine;
                                                    }
                                                }
                                            }
                                            nmls = null;
                                            webserviceurl = string.Empty;

                                        }
                                    }
                                    else
                                    {
                                        batcjJobStatus = false;
                                        batchJobLog += "data not retuned from webservice" + Environment.NewLine;
                                    }
                                }

                                catch (Exception ex)
                                {
                                    batcjJobStatus = false;
                                    batchJobLog += ex.InnerException + Environment.NewLine;
                                }
                            }
                        }

                    }
                    else
                    {
                        batcjJobStatus = false;
                        batchJobLog += "service url not Found in Configaration" + Environment.NewLine;

                    }
                }
            }
            catch (Exception ex)
            {
                batcjJobStatus = false;
                batchJobLog += " error @ main method" + ex.InnerException + Environment.NewLine;
            }
            finally
            {
                batchJobLog += "---Batch Job Ended---" + DateTime.Now + Environment.NewLine;
                batchJobLog += "<<=================================================>>" + Environment.NewLine;
                //Update Log record
                Task.WaitAll(Task.Run(async () => await Common.UpdateBatchJobStatus(authority, clientId, secret, serviceUrl, batchJobRecordId, batcjJobStatus, batchJobLog, Constants.batchJobName)));
            }
        }

        public static async Task createRecordinTestimonial(string Testimonialname, string TestimonialnameDescprition, Guid userid, string serviceUrl, string accessToken)
        {
            Common objcomman = new Common();
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(serviceUrl);
                    //httpClient.Timeout = new TimeSpan(0, 2, 0);
                    httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                    httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                    httpClient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessToken);

                    if (TestimonialnameDescprition.Length > 1048575)
                    {
                        TestimonialnameDescprition = TestimonialnameDescprition.Substring(0, 1048574);
                    }

                    JObject testimonial = new JObject();
                    testimonial["ims_name"] = Testimonialname;
                    testimonial["ims_testimonialdescriptions"] = TestimonialnameDescprition;
                    testimonial["ims_user@odata.bind"] = "/systemusers(" + userid + ")";
                    testimonial["ownerid@odata.bind"] = "/systemusers(" + userid + ")";
                    HttpResponseMessage createResponse =
                       await httpClient.SendAsJsonAsync(HttpMethod.Post, "api/data/v9.1/ims_testimonials", testimonial);
                    if (!createResponse.IsSuccessStatusCode)
                    {
                        throw new Exception(createResponse.ReasonPhrase + "Status Code " + objcomman.ExtractErrorMessage(createResponse) + createResponse.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while creating Record Testimonial:" + "Please check Testimonial Privilege of User security Role");
            }
        }

        public static async Task updateuserdata(List<JObject> data, Guid userid, string serviceUrl, string accessToken)
        {
            Common objcomman = new Common();
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(serviceUrl);
                    httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                    httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                    httpClient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessToken);
                    httpClient.Timeout = TimeSpan.FromMinutes(3);


                    if (data != null)
                    {
                        foreach (JObject entity in data)
                        {
                            var stateLiceneceNumber = string.Empty;
                            var link = entity["link"].ToString();
                            var fax_number = (string)entity.SelectToken("acf.fax_number");
                            var Facebookurl = (string)entity.SelectToken("acf.facebook_url");
                            var lotitle = (string)entity.SelectToken("acf.lotitle");
                            var email = (string)entity.SelectToken("acf.email_address");
                            var cellphone = (string)entity.SelectToken("acf.cell_phone_number");
                            var loanofficerimage = (string)entity.SelectToken("acf.loan_officer_image");
                            var zillowurl = (string)entity.SelectToken("acf.zillow_url");
                            var state_disclosure = (string)entity.SelectToken("acf.state_disclosure");
                            var Linkdinurl = (string)entity.SelectToken("acf.linkedin_url");
                            var branchAddress = (string)entity.SelectToken("acf.branch_address");
                            var branchCity = (string)entity.SelectToken("acf.branch_city");
                            var branchState = (string)entity.SelectToken("acf.branch_state");
                            var branchZip = (string)entity.SelectToken("acf.branch_zip");
                            var nmlsNumber = (string)entity.SelectToken("acf.nmls_id");
                            var jsonWordPress = JsonConvert.SerializeObject(data);
                            var lowebsite = (string)entity.SelectToken("acf.lo_website_url");
                            var officePhone = (string)entity.SelectToken("acf.office_phone_number");

                            if (!string.IsNullOrEmpty(state_disclosure))
                            {
                                var stateLiceneceNumbe_split = state_disclosure.Split('|');
                                stateLiceneceNumber = stateLiceneceNumbe_split[0];
                            }
                            if (state_disclosure.Length > 1048575)
                            {
                                state_disclosure = state_disclosure.Substring(0, 1048574);
                            }
                            var stateCodes = getStateCodes(stateLiceneceNumber);
                            JObject crmdataupdated = new JObject
                                {
                                     {"ims_websiteurl", link},
                                     {"address1_fax",fax_number},
                                     {"ims_facebookid",Facebookurl},
                                     {"jobtitle",lotitle},
                                    // {"internalemailaddress",email},
                                     {"mobilephone",cellphone},
                                     {"photourl",loanofficerimage},
                                     {"ims_zillowagentwebsite",zillowurl},
                                     {"ims_linkedinurl",Linkdinurl},
                                     {"ims_disclaimertext",state_disclosure},
                                     {"ims_statelicensesnumbers",stateLiceneceNumber},
                                     {"address1_line1",branchAddress},
                                     {"address1_city",branchCity},
                                     {"address1_stateorprovince",branchState},
                                     {"address1_postalcode",branchZip},
                                     {"ims_nmlsnumber",nmlsNumber},
                                     {"ims_signature", jsonWordPress},
                                     {"ims_statelicensesnames",stateCodes},
                                     {"ims_lowebsite",lowebsite},
                                     {"address1_telephone1",officePhone}

                            };
                            HttpResponseMessage updateResponse = await httpClient.SendAsJsonAsync(new HttpMethod("PATCH"), "api/data/v9.1/systemusers(" + userid + ")", crmdataupdated);
                            //JObject jObjectWordPress = (JObject)data.ToString(); 
                            //var userSignature = BuildUserSignature(entity, serviceUrl, accessToken, signatureConfiguration);
                            //Console.WriteLine(userSignature);
                            //var jsonWordPress = JsonConvert.SerializeObject(data);
                            //JObject jObjectUser = new JObject();
                            //jObjectUser["ims_signature"] = jsonWordPress;
                            //HttpResponseMessage updateResponseSignature = await httpClient.SendAsJsonAsync(new HttpMethod("PATCH"), "api/data/v9.1/systemusers(" + userid + ")", jObjectUser);
                            if (!updateResponse.IsSuccessStatusCode) //200
                            {
                                throw new Exception(updateResponse.ReasonPhrase + "Status Code " + updateResponse.StatusCode + objcomman.ExtractErrorMessage(updateResponse) + Environment.NewLine);
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while Updataing Record:" + ex.InnerException);
            }
        }

        private static string getStateCodes(string Licenses)
        {
            if(Licenses =="" || Licenses==string.Empty)
            {
                return null;
            }
            List<string> statecodes = new List<string>();
           // string stateLicenses = "NC-I-183424, SC-BFI-MLO - 1002646,GA-57858,AR-109304, AZ-0933094, CA-DOC713818, CT-LO-713818, FL-LO32517, GA-32686, IL-031.0030791, MD-20677, MN-MLO-713818, NC-I-152324, NE, NH, NJ, NM, NV-54778, NY, OH-MLO.048792.000, OK-MLO11798, OR, PA-35351, RI, SC-BFI-MLO - 713818, TN-112054, TX-SML, VA-MLO-9520VA, WA-MLO-713818, WI-713818   ";
            string[] stateLicenseList = Licenses.Split(',');


            foreach (var license in stateLicenseList)
            {
                string[] stateLicense = license.Split('-');
                statecodes.Add(stateLicense[0].Replace(" ", string.Empty));
            }

            // Console.WriteLine(string.Join(",", statecodes));
            return string.Join(",", statecodes);

        }
        public static string BuildUserSignature(JObject wordpress, string serviceUrl, string accessToken, string signatureConfig)
        {
            string[] socialMediaHolders = { "{acf.twitter_url}", "{acf.instagram_url}", "{acf.facebook_url}", "{acf.linkedin_url}", "{acf.youtube_url}", "{acf.zillow_url}" };
            string[] commaSeprator = { "{acf.branch_state}", "{acf.branch_zip}" };

            Dictionary<string, bool> socialMedia = new Dictionary<string, bool>();
            Dictionary<string, bool> commaSepratorDictionary = new Dictionary<string, bool>();

            using (HttpClient httpClientForImportMapping = new HttpClient())
            {
                httpClientForImportMapping.BaseAddress = new Uri(serviceUrl);
                httpClientForImportMapping.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                httpClientForImportMapping.DefaultRequestHeaders.Add("OData-Version", "4.0");
                httpClientForImportMapping.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClientForImportMapping.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpResponseMessage responseForImportMapping = httpClientForImportMapping.GetAsync("api/data/v9.1/ims_importdetailsmappings?$select=ims_sourcefield,ims_defaultvalue,ims_targetfield&$filter=ims_sourceentity eq 'Wordpress' and ims_name eq 'Signature HTML'", HttpCompletionOption.ResponseHeadersRead).Result;

                if (responseForImportMapping.IsSuccessStatusCode) //200
                {
                    JObject importMappingCollection = JObject.Parse(responseForImportMapping.Content.ReadAsStringAsync().Result);
                    JToken importMappingArray;
                    if (importMappingCollection.TryGetValue("value", out importMappingArray))
                    {
                        JArray importMappingentities = (JArray)importMappingArray;
                        foreach (JObject importMapping in importMappingentities)
                        {
                            if (importMapping.ContainsKey("ims_sourcefield") && importMapping.ContainsKey("ims_targetfield"))
                            {
                                string sourcefield = importMapping.GetValue("ims_sourcefield").ToString();
                                string targetfield = importMapping.GetValue("ims_targetfield").ToString();
                                string defaultValue = importMapping.GetValue("ims_defaultvalue").ToString();
                                string wordpressData = (string)wordpress.SelectToken(sourcefield);
                                if (signatureConfig.Contains(targetfield) && !string.IsNullOrEmpty(wordpressData))
                                {
                                    if (sourcefield == "acf.loan_officer_image" && (wordpressData == "false" || wordpressData == "False"))
                                    {
                                        signatureConfig = signatureConfig.Replace("{acf.loan_officer_image_display}", "none");
                                    }
                                    if (sourcefield == "acf.state_disclosure")
                                    {
                                        if (wordpressData.Length > 1048575)
                                        {
                                            wordpressData = wordpressData.Substring(0, 1048574);
                                        }
                                        int index = wordpressData.IndexOf('|', wordpressData.IndexOf('|'));
                                        signatureConfig = signatureConfig.Replace(targetfield, wordpressData.Substring(0, index - 1));
                                    }
                                    else
                                    {
                                        signatureConfig = signatureConfig.Replace(targetfield, wordpressData);
                                        signatureConfig = signatureConfig.Replace(targetfield.Replace("}","") + "_display}", defaultValue);
                                    }
                                    if (socialMediaHolders.Contains(targetfield))
                                    {
                                        socialMedia.Add(targetfield, true);
                                    }
                                    if (commaSeprator.Contains(targetfield))
                                    {
                                        commaSepratorDictionary.Add(targetfield, true);
                                    }
                                    
                                }
                                else
                                {
                                    signatureConfig = signatureConfig.Replace(targetfield, "");
                                    signatureConfig = signatureConfig.Replace(targetfield.Replace("}","") + "_display}", "none");
                                    if(socialMediaHolders.Contains(targetfield))
                                    {
                                        socialMedia.Add(targetfield, false);
                                    }
                                    if (commaSeprator.Contains(targetfield))
                                    {
                                        commaSepratorDictionary.Add(targetfield, false);
                                    }
                                }
                            }
                        }
                        signatureConfig = signatureConfig.Replace("{|}", "|");
                        //signatureConfig = signatureConfig.Replace("{,}", ",");
                        if(socialMedia.ContainsValue(true))
                        {
                            signatureConfig = signatureConfig.Replace("{findmeon}", "flex");
                        }
                        else if(!socialMedia.ContainsValue(true))
                        {
                            signatureConfig = signatureConfig.Replace("{findmeon}", "none");
                        }
                        if(commaSepratorDictionary.ContainsValue(true))
                        {
                            signatureConfig = signatureConfig.Replace("{,}", ",");
                        }
                        else if (!commaSepratorDictionary.ContainsValue(true) && commaSepratorDictionary.Count>0)
                        {
                            signatureConfig = signatureConfig.Replace("{,}", "");
                        }
                    }
                }
            }
            return signatureConfig;
        }
    }
}

namespace nmlswebservice
{
    public class Rootobject
    {
        public Class1[] Property1 { get; set; }
    }

    public class Class1
    {
        public int id { get; set; }
        public DateTime date { get; set; }
        public DateTime date_gmt { get; set; }
        public Guid guid { get; set; }
        public DateTime modified { get; set; }
        public DateTime modified_gmt { get; set; }
        public string slug { get; set; }
        public string status { get; set; }
        public string type { get; set; }
        public string link { get; set; }
        public Title title { get; set; }
        public Content content { get; set; }
        public Excerpt excerpt { get; set; }
        public int author { get; set; }
        public int featured_media { get; set; }
        public int menu_order { get; set; }
        public string template { get; set; }
        public object[] meta { get; set; }
        public Acf acf { get; set; }
        public _Links _links { get; set; }
    }

    public class Guid
    {
        public string rendered { get; set; }
    }

    public class Title
    {
        public string rendered { get; set; }
    }

    public class Content
    {
        public string rendered { get; set; }
        public bool _protected { get; set; }
    }

    public class Excerpt
    {
        public string rendered { get; set; }
        public bool _protected { get; set; }
    }

    public class Acf
    {
        public string lo_crm { get; set; }
        public string point_of_sale { get; set; }
        public string leadpops { get; set; }
        public string personal_greeting { get; set; }
        public string cover_bg { get; set; }
        public string lotitle { get; set; }
        public string email_address { get; set; }
        public object loan_officer_image { get; set; }
        public string office_phone_number { get; set; }
        public string cell_phone_number { get; set; }
        public string fax_number { get; set; }
        public string nmls_id { get; set; }
        public string lic_info { get; set; }
        public string state_disclosure { get; set; }
        public string apply_online_url_blend { get; set; }
        public string apply_video_url { get; set; }
        public string facebook_url { get; set; }
        public string linkedin_url { get; set; }
        public string twitter_url { get; set; }
        public string zillow_url { get; set; }
        public string testimonial_1 { get; set; }
        public string testimonial_1_name { get; set; }
        public string testimonial_2 { get; set; }
        public string testimonial_2_name { get; set; }
        public string testimonial_3 { get; set; }
        public string testimonial_3_name { get; set; }
        public string facebook_pixel { get; set; }
        public string personal_slogan { get; set; }
        public string custom_bio { get; set; }
        public string meta_keywords { get; set; }
        public bool fluent_in_spanish { get; set; }
        public string about_me_video_url { get; set; }
        public string about_me_bio { get; set; }
    }

    public class _Links
    {
        public Self[] self { get; set; }
        public Collection[] collection { get; set; }
        public About[] about { get; set; }
        public Author[] author { get; set; }
        public VersionHistory[] versionhistory { get; set; }
        public PredecessorVersion[] predecessorversion { get; set; }
        public WpAttachment[] wpattachment { get; set; }
        public Cury[] curies { get; set; }
    }

    public class Self
    {
        public string href { get; set; }
    }

    public class Collection
    {
        public string href { get; set; }
    }

    public class About
    {
        public string href { get; set; }
    }

    public class Author
    {
        public bool embeddable { get; set; }
        public string href { get; set; }
    }

    public class VersionHistory
    {
        public int count { get; set; }
        public string href { get; set; }
    }

    public class PredecessorVersion
    {
        public int id { get; set; }
        public string href { get; set; }
    }

    public class WpAttachment
    {
        public string href { get; set; }
    }

    public class Cury
    {
        public string name { get; set; }
        public string href { get; set; }
        public bool templated { get; set; }
    }
}


