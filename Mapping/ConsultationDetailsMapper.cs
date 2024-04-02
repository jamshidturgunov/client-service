using TestClientsRequests.Entity;

namespace TestClientsRequests.Mapping
{
    public static class ConsultationDetailsMapper
    {
        public static List<DetailConsult> MapFromRangeData(IList<IList<object>> values)
        {
            var details = new List<DetailConsult>();

            foreach (var value in values)
            {
                if (value[0].ToString() == "№")
                    continue;

                DetailConsult detail = new()
                {
                    Id = int.Parse(value[0].ToString()),
                    Region = value[1].ToString(),
                    City = value[2].ToString(),
                    FullName = value[3].ToString(),
                    PhoneNumber = value[4].ToString(),
                    Organization = value[5].ToString(),
                    TypeOfProduct = value[6].ToString(),
                    FromLandingPage = value[7].ToString()

                };

                details.Add(detail);
            }

            return details;
        }

        public static IList<IList<object>> MapToRangeData(DetailConsult consult)
        {
            var objectList = new List<object>() 
            { 
                consult.Id,
                EnumToObject(consult.Region), 
                consult.City, 
                consult.FullName, 
                consult.PhoneNumber, 
                consult.Organization,
                EnumToObject(consult.TypeOfProduct),
                consult.FromLandingPage 
            };
            var rangeData = new List<IList<object>> { objectList };
            return rangeData;
        }

        private static object EnumToObject<T>(T enumValue)
        {
            return enumValue.ToString();
        }

    }
}
