using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using WebinarRegistration.Models;

namespace WebinarRegistration.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private IConfiguration _configuration;
        public List<Webinar> webinarList { get; private set; } = new List<Webinar>();
        public string eventHtml { get; private set; } = @"<div class=""container webinar""> <div class=""row webinarTitleRow""> <div class=""col-sm webinarTitleText"">#TITLE#<div class=""company"">#COMPANY#</div></div> <div class=""col-sm-5 dateTimeDesktop""> <div class=""row""> <div class=""col-md webinarTime""> <div class=""webinarDateTimeHeader"">Time:</div> <div class=""webinarDateTimeText"">#TIME# Eastern</div> </div> <div class=""col-md webinarDate""> <div class=""dateContainer""> <span class=""webinarDateTimeHeader"">Date:</span><br /> <span class=""webinarDateTimeText"">#DATE#</span> </div> </div> </div> </div> </div> <div class=""row dateTimeMobile""> <div class=""col-md""> <span class=""webinarDateTimeHeader"">Date:</span><br /> <span class=""webinarDateTimeText"">#DATE#</span> <br /><br /> <div class=""webinarDateTimeHeader"">Time:</div> <div class=""webinarDateTimeText"">#TIME#</div> </div> </div> <div class=""row webinarDescRow""> <div class=""col-md""> #DESCRIPTION# </div> </div> <div class=""row webinarButtonRow""> <div class=""col-md""> <a href=""#LINK#""  target=""_blank""><input type=""button"" class=""formButton"" value=""REGISTER NOW"" /></a> </div> </div> </div>";

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async void OnGet()
        {            
            var grantType = _configuration["grant_type"];
            var tokenEndpoint = _configuration["TokenEndPoint"];
            var APIendPoint = _configuration["APIendPoint"];
            var clientId = _configuration["client_id"];
            var clientSecret = _configuration["client_secret"];
            var tenantId = _configuration["tenantId"];
            var principle = _configuration["principle"];
            var target = _configuration["target"];
            var listId = _configuration["listId"];

            var data = new[]
            {
                new KeyValuePair<string, string>("grant_type",grantType),
                new KeyValuePair<string, string>("client_id",$"{clientId}@{tenantId}"),
                new KeyValuePair<string, string>("client_secret",clientSecret),
                new KeyValuePair<string, string>("resource",$"{principle}/{target}@{tenantId}"),
            };          

            using(var client = new HttpClient())
            {
                var result = client.PostAsync(tokenEndpoint.Replace("#TenantId#", tenantId), new FormUrlEncodedContent(data)).GetAwaiter().GetResult();
                var jsonContent = await result.Content.ReadAsStringAsync();
                var accessToken = JsonConvert.DeserializeObject<Token>(jsonContent).access_token;               

                client.SetBearerToken(accessToken);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.GetAsync(APIendPoint.Replace("#ListId#", listId)).Result;
                var responseContent = await response.Content.ReadAsStringAsync();
                var webinarJsonList = JsonConvert.DeserializeObject<dynamic>(responseContent).value;                             

                for (int i = 0; i < webinarJsonList.Count; i++)
                {
                    if ((bool)webinarJsonList[i].Active)
                    {
                        Webinar webinar = new Webinar()
                        {
                            Id = webinarJsonList[i].Id,
                            Title = webinarJsonList[i].Title,
                            Description = webinarJsonList[i].Description,
                            Active = webinarJsonList[i].Active,
                            EventDate = webinarJsonList[i].Date,
                            Company = webinarJsonList[i].CompanyProduct,
                            RegistrationLink = webinarJsonList[i].RegistrationLink.Url,
                        };

                        webinarList.Add(webinar);
                    }                    
                }   
                webinarList = webinarList.OrderBy(e => e.EventDate).ThenByDescending(e => e.Id).ToList();            
            }
        }
    }
}