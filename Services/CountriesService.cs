using ServiceContracts;
using ServiceContracts.DTO;
using Microsoft.EntityFrameworkCore;
using Entities;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace Services;

public class CountriesService : ICountriesService
{
    //private fileds
    private readonly ApplicationDbContext _db;

    //constractor
    public CountriesService(ApplicationDbContext personDbContext)
    {
        _db = personDbContext;
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
        if (await _db.Countries.AnyAsync(country => country.CountryName == countryAddRequest.CountryName))
        {
            throw new ArgumentException("Given country name is already exist.");
        }

        //change country add request to country object
        Country newCountry = countryAddRequest.ToCountry();

        //generate new countryId 
        newCountry.CountryID = Guid.NewGuid();

        //add country to country list
        _db.Countries.Add(newCountry);
        await _db.SaveChangesAsync();

        return newCountry.ToCountryResponse();
    }

    public async Task<List<CountryResponse>> GetAllCountries()
    {
        //return all country that exist in countries list as list of response type
        return await _db.Countries.Select(country => country.ToCountryResponse()).ToListAsync();
    }

    public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
    {
        if (countryID == null)
            return null;

        Country? country = await _db.Countries.FirstOrDefaultAsync(temp => temp.CountryID == countryID);

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
                    if(!_db.Countries.Where(temp => temp.CountryName == countryName).Any())
                    {
                        var country = new Country() { CountryID = Guid.NewGuid(), CountryName = countryName};
                        _db.Countries.Add(country);
                        await _db.SaveChangesAsync();

                        countInsertedCountry++;
                    }
                }
            }
        }

        return countInsertedCountry;
    }
}