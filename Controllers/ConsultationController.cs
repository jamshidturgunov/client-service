using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Serilog;
using TestClientsRequests.Entity;
using TestClientsRequests.Extensions;
using TestClientsRequests.Helpers;
using TestClientsRequests.Mapping;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource;

namespace TestClientsRequests.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ConsultationController : ControllerBase
    {
        const string SPREADSHEET_ID = "1utIACKDenN3iGys8DjDWPEqGykvvtzbGhi0DfZVx27w";
        const string SHEET_NAME = "Consult";

        private int lastId;

        SpreadsheetsResource.ValuesResource _googleSheetValues;

        private readonly IStringLocalizer _localizer;
        private readonly IStringLocalizer _anotherLocalizer;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ConsultationController> _logger;

        public ConsultationController(
            GoogleSheetsHelper googleSheetsHelper, 
            IStringLocalizerFactory factory, 
            IHttpContextAccessor httpContextAccessor,
            ILogger<ConsultationController> logger)
        {
            _googleSheetValues = googleSheetsHelper.Service.Spreadsheets.Values;

            lastId = GetLastIdFromData() ?? 0;

            _localizer = factory.Create("Region", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            _anotherLocalizer = factory.Create("TypeOfProduct", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name); ;
            _httpContextAccessor = httpContextAccessor;

            _logger = logger;
        }

        //[HttpGet(Name = "GetRegions")]
        //public List<Tuple<int, string, string>> GetRegions()
        //{
        //    var tupleList = Enum.GetValues(typeof(Region))
        //        .Cast<Region>()
        //        .Select(e => new Tuple<int, string, string>((int)e, e.ToString(), e.GetLocalizedValue(_localizer))).
        //        ToList();

        //    return tupleList;
        //}

        [HttpGet(Name = "GetRegions")]
        public List<object> GetRegions()
        {
            var regionModels = Enum.GetValues(typeof(Region))
                .Cast<Region>()
                .Select(e => new
                {
                    Id = (int)e,
                    Value = e.ToString(),
                    Name = e.GetLocalizedValue(_localizer)
                })
                .ToList<object>();

            return regionModels;
        }

        [HttpGet(Name = "GetProductTypes")]
        public List<object> GetProductTypes()
        {
            var productTypeModels = Enum.GetValues(typeof(TypeOfProduct))
                .Cast<TypeOfProduct>()
                .Select(e => new
                {
                    Id = (int)e,
                    Value = e.ToString(),
                    Name = e.GetLocalizedValue(_anotherLocalizer)
                }).
                ToList<object>();

            return productTypeModels;
        }

        //[HttpGet(Name = "GetProductTypes")]
        //public List<Tuple<int, string, string>> GetProductTypes()
        //{
        //    var tupleList = Enum.GetValues(typeof(TypeOfProduct))
        //        .Cast<TypeOfProduct>()
        //        .Select(e => new Tuple<int, string, string>((int)e, e.ToString(), e.GetLocalizedValue(_anotherLocalizer))).
        //        ToList();

        //    return tupleList;
        //}

        [HttpGet]
        public IActionResult GetAll()
        {
            var range = $"{SHEET_NAME}!A:H";
            var request = _googleSheetValues.Get(SPREADSHEET_ID, range);
            var response = request.Execute();
            var values = response.Values;
            return Ok(ConsultationDetailsMapper.MapFromRangeData(values));
        }

        [HttpGet("{rowId}")]
        public IActionResult Get(int rowId)
        {
            var range = $"{SHEET_NAME}!A{rowId + 1}:H{rowId + 1}";
            var request = _googleSheetValues.Get(SPREADSHEET_ID, range);
            var response = request.Execute();
            var values = response.Values;
            return Ok(ConsultationDetailsMapper.MapFromRangeData(values).FirstOrDefault());
        }

        [HttpPost]
        public IActionResult Create([FromForm] ConsultDetail detail)
        {
            try
            {
                detail.Id = ++lastId;

                var consult = new DetailConsult
                {
                    Id = detail.Id,
                    Region = _localizer[detail.Region.ToString()],
                    City = detail.City,
                    FullName = detail.FullName,
                    PhoneNumber = detail.PhoneNumber,
                    Organization = detail.Organization,
                    TypeOfProduct = _anotherLocalizer[detail.TypeOfProduct.ToString()],
                    FromLandingPage = detail.FromLandingPage
                };

                var range = $"{SHEET_NAME}!A:H";
                var valueRange = new ValueRange
                {
                    Values = ConsultationDetailsMapper.MapToRangeData(consult)
                };

                _logger.LogInformation("--------------- Create Consultation Log! ------------");

                var appendRequest = _googleSheetValues.Append(valueRange, SPREADSHEET_ID, range);
                appendRequest.ValueInputOption = AppendRequest.ValueInputOptionEnum.USERENTERED;
                appendRequest.Execute();
                return CreatedAtAction(nameof(Get), new { rowId = consult.Id }, consult); 
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return BadRequest($"There is something wrong! - {ex}");
            }
        }

        private int? GetLastIdFromData()
        {
            var range = $"{SHEET_NAME}!A:H";
            var request = _googleSheetValues.Get(SPREADSHEET_ID, range);
            var response = request.Execute();
            var values = response.Values;
            if (values == null || values.Count == 0)
            {
                return null;
            }
            return int.Parse(values[values.Count - 1][0].ToString());
        }
    }
}
