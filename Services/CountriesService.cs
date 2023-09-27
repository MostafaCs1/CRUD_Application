using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using RepositoryContracts;

namespace Services;

public class CountriesService : ICountriesService
{
    //private fileds
    private readonly ICountriesRepository _countriesRepository;

    //constractor
    public CountriesService(ICountriesRepository countriesRepository)
    {
        _countriesRepository = countriesRepository;
    }

    //Services
    public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
    {
        //validation : country add request can't be null
        if (countryAddRequest == null)
        {
            throw new ArgumentNullException(nameof(countryAddRequest));
        }

        //validation : country add request's name can't be null
        if (string.IsNullOrEmpty(countryAddRequest.CountryName))
        {
            throw new ArgumentException(nameof(countryAddRequest.CountryName));
        }

        //validation : country name can't be duplicate
        if (await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName) != null)
        {
            throw new ArgumentException("Given country name is already exist.");
        }

        //change country add request to country object
        Country newCountry = countryAddRequest.ToCountry();

        //generate new countryId 
        newCountry.CountryID = Guid.NewGuid();

        //add country to country list
        await _countriesRepository.AddCountry(newCountry);

        return newCountry.ToCountryResponse();
    }

    public async Task<List<CountryResponse>> GetAllCountries()
    {
        //return all country that exist in countries list as list of response type
        List<Country> countries = await _countriesRepository.GetAllCountry();
        return countries.Select(country => country.ToCountryResponse()).ToList();
    }

    public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
    {
        if (countryID == null)
            return null;

        Country? country = await _countriesRepository.GetCountryByCountryID(countryID.Value);

        if (country == null)
            return null;

        return country.ToCountryResponse();
    }

    public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
    {
        var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream);
        int countInsertedCountry = 0;

        using (var excelPackage = new ExcelPackage(memoryStream))
        {
            ExcelWorksheet countriesSheet = excelPackage.Workbook.Worksheets["Countries"];

            int totalRow = countriesSheet.Dimension.Rows;
            for (int row = 2; row <= totalRow; row++)
            {
                object? cellValue = countriesSheet.Cells[row, 1].Value;
                if(cellValue != null)
                {
                    string? countryName = cellValue.ToString();
                    if(countryName != null && await _countriesRepository.GetCountryByCountryName(countryName) == null)
                    {
                        var country = new Country() { CountryID = Guid.NewGuid(), CountryName = countryName};
                        await _countriesRepository.AddCountry(country);
                        countInsertedCountry++;
                    }
                }
            }
        }

        return countInsertedCountry;
    }
}