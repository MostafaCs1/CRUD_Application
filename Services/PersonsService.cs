using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Services.Helper;
using ServiceContracts.Enums;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using OfficeOpenXml;
using RepositoryContracts;

namespace Services;

public class PersonsService : IPersonsService
{
    //fields
    private readonly IPersonsRepository _personsrepository;

    //constructor
    public PersonsService(IPersonsRepository personsRepository)
    {
        _personsrepository = personsRepository;
    }


    //services
    public async Task<PersonResponse> AddPerson(PersonAddRequest? request)
    {
        // PersonsAddrequest can't be null
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        // Model validation
        ValidationHelper.ModelValidation(request);

        //generate personId
        Guid personID = Guid.NewGuid();

        //create person object
        Person newPerson = request.ToPerson();
        newPerson.PersonID = personID;

        //add person to person list
        await _personsrepository.AddPerson(newPerson);

        return newPerson.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetAllPersons()
    {
        IEnumerable<Person> persons = await _personsrepository.GetAllPersons();
        return persons.Select(person => person.ToPersonResponse()).ToList();
    }

    public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
    {
        //validation: personId can't be null
        if (personID == null)
            return null;

        Person? response = await _personsrepository.GetPersonByPersonID(personID.Value);

        //validation" personId can't be invalid
        if (response == null)
            return null;

        return response.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetFiltredPersons(string searchBy, string? searchString)
    {
        List<Person> allPersons = await _personsrepository.GetAllPersons();
        List<Person> matchingPersons = allPersons;

        if (string.IsNullOrEmpty(searchString) || string.IsNullOrEmpty(searchBy))
        {
            return matchingPersons.Select(person => person.ToPersonResponse()).ToList();
        }

        matchingPersons = (searchBy) switch
        {
            (nameof(PersonResponse.PersonName)) => await _personsrepository
            .GetFiltredPersons(person => person.PersonName.Contains(searchString)),

            (nameof(PersonResponse.Email)) => await _personsrepository
            .GetFiltredPersons(person => person.Email.Contains(searchString)),

            (nameof(PersonResponse.DateOfBirth)) => await _personsrepository
                .GetFiltredPersons(person => person.DateOfBirth.Value.ToString("yyyy MMMM dd").Contains(searchString)),

            (nameof(PersonResponse.Gender)) => await _personsrepository
            .GetFiltredPersons(person => person.Gender.Equals(searchString)),

            (nameof(PersonResponse.CountryID)) => await _personsrepository
                .GetFiltredPersons(person => person.Country.CountryName.Contains(searchString)),

            (nameof(PersonResponse.Address)) => await _personsrepository
            .GetFiltredPersons(person => person.Address.Contains(searchString)),

            _ => allPersons
        };

        return matchingPersons.Select(person => person.ToPersonResponse()).ToList();
    }

    public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
    {
        if (string.IsNullOrEmpty(sortBy))
        {
            return allPersons;
        }

        List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
        {
            (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) =>
            allPersons.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) =>
            allPersons.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.Email), SortOrderOptions.ASC) =>
            allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Email), SortOrderOptions.DESC) =>
            allPersons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.Address), SortOrderOptions.ASC) =>
            allPersons.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Address), SortOrderOptions.DESC) =>
            allPersons.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.Country), SortOrderOptions.ASC) =>
            allPersons.OrderBy(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Country), SortOrderOptions.DESC) =>
            allPersons.OrderByDescending(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.Age), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Age).ToList(),
            (nameof(PersonResponse.Age), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Age).ToList(),

            (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),
            (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),

            (nameof(PersonResponse.Gender), SortOrderOptions.ASC) =>
            allPersons.OrderBy(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Gender), SortOrderOptions.DESC) =>
            allPersons.OrderByDescending(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.ReceiveNewsLetters).ToList(),
            (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.ReceiveNewsLetters).ToList(),

            _ => allPersons
        };

        return sortedPersons;
    }

    public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdate)
    {
        //Update request can't be null
        if (personUpdate == null)
            throw new ArgumentNullException(nameof(personUpdate));

        //Model validation
        ValidationHelper.ModelValidation(personUpdate);

        Person? matchingPerson = await _personsrepository.GetPersonByPersonID(personUpdate.PersonID);
        //PersonID should be a valid person Id
        if (matchingPerson == null)
            throw new ArgumentException("Given person Id isn't exist in persons list.");

        //update person details
        await _personsrepository.UpdatePerson(personUpdate.ToPerson());

        return matchingPerson.ToPersonResponse();
    }

    public async Task<bool> DeletePerson(Guid? personID)
    {
        //PersonsID can't be null
        if (personID == null)
            throw new ArgumentNullException(nameof(personID));

        //Check that person already exist in persons list
        Person? person = await _personsrepository.GetPersonByPersonID(personID.Value);
        if (person == null)
            return false;

        //delete person from list
        bool isDeleted = await _personsrepository.DeletePersonByPersonID(personID.Value);

        return isDeleted;
    }

    public async Task<MemoryStream> GetPersonsCSV()
    {
        MemoryStream memoryStream = new MemoryStream();
        StreamWriter streamWriter = new StreamWriter(memoryStream);

        CsvConfiguration csvConfiguration = new CsvConfiguration(cultureInfo: CultureInfo.InvariantCulture);
        CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfiguration);

        //PersonName,Email,DateOfBirth,Age,Gender,Country,Address,ReceiveNewsLetters
        csvWriter.WriteField<string>(nameof(PersonResponse.PersonName));
        csvWriter.WriteField<string>(nameof(PersonResponse.Email));
        csvWriter.WriteField<string>(nameof(PersonResponse.DateOfBirth));
        csvWriter.WriteField<string>(nameof(PersonResponse.Age));
        csvWriter.WriteField<string>(nameof(PersonResponse.Gender));
        csvWriter.WriteField<string>(nameof(PersonResponse.Country));
        csvWriter.WriteField<string>(nameof(PersonResponse.Address));
        csvWriter.WriteField<string>(nameof(PersonResponse.ReceiveNewsLetters));
        csvWriter.NextRecord();
        csvWriter.Flush();

        //get all persons list
        List<PersonResponse> persons = (await _personsrepository.GetAllPersons()).Select(person => person.ToPersonResponse()).ToList();

        foreach (PersonResponse person in persons)
        {
            csvWriter.WriteField(person.PersonName);
            csvWriter.WriteField(person.Email);
            if (person.DateOfBirth != null)
                csvWriter.WriteField(person.DateOfBirth.Value.ToString("yyyy/MM/dd"));
            else
                csvWriter.WriteField(" ");
            csvWriter.WriteField(person.Age);
            csvWriter.WriteField(person.Gender);
            csvWriter.WriteField(person.Country);
            csvWriter.WriteField(person.Address);
            csvWriter.WriteField(person.ReceiveNewsLetters);
            csvWriter.NextRecord();
            csvWriter.Flush();
        }

        memoryStream.Position = 0;
        return memoryStream;
    }

    public async Task<MemoryStream> GetPersonExcel()
    {
        MemoryStream memoryStream = new MemoryStream();
        using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
        {
            ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");

            //PersonName,Email,DateOfBirth,Age,Gender,Country,Address,ReceiveNewsLetters
            workSheet.Cells["A1"].Value = "Person Name";
            workSheet.Cells["B1"].Value = "Email";
            workSheet.Cells["C1"].Value = "Date Of Birth";
            workSheet.Cells["D1"].Value = "Age";
            workSheet.Cells["E1"].Value = "Gender";
            workSheet.Cells["F1"].Value = "Country";
            workSheet.Cells["G1"].Value = "Address";
            workSheet.Cells["H1"].Value = "Receive News Letters";

            //get list of all persons
            List<PersonResponse> persons = (await _personsrepository.GetAllPersons()).Select(person => person.ToPersonResponse()).ToList();

            //write persons into excel file
            int row = 2;
            foreach (PersonResponse person in persons)
            {
                workSheet.Cells[row, 1].Value = person.PersonName;
                workSheet.Cells[row, 2].Value = person.Email;
                if (person.DateOfBirth != null)
                    workSheet.Cells[row, 3].Value = person.DateOfBirth.Value.ToString("yyyy-MM-dd");
                workSheet.Cells[row, 4].Value = person.Age;
                workSheet.Cells[row, 5].Value = person.Gender;
                workSheet.Cells[row, 6].Value = person.Country;
                workSheet.Cells[row, 7].Value = person.Address;
                workSheet.Cells[row, 8].Value = person.ReceiveNewsLetters;

                row++;
            }

            workSheet.Cells[$"A1:H{row}"].AutoFitColumns();
            await excelPackage.SaveAsync();
        }
        memoryStream.Position = 0;

        return memoryStream;
    }
}

