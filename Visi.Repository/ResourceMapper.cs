using Hl7.Fhir.Model;
using Hl7.Fhir.Support;
using Visi.Repository.Models;
using Vonk.Core.Common;
using Vonk.Fhir.R4;
// using Vonk.Fhir.R3;

namespace Visi.Repository;

public class ResourceMapper
{
    public IResource MapPatient(Child source)
    {
        var patient = new Patient
        {
            Id = source.ChildId.ToString(),
            BirthDate = source.BirthDateTime
                .ToFhirDate() //Time part is not converted here, since the Birthdate is of type date
            //If you have it present in the source, and want to communicate it, you
            //need to add a birthtime extension.
        };
        patient.Identifier.Add(new Identifier("http://mycompany.org/patientnumber", source.ChildId.ToString()));
        patient.Name.Add(new HumanName().WithGiven(source.FirstName).AndFamily(source.LastName));
        patient.Gender = source.Sex switch
        {
            'm' => AdministrativeGender.Male,
            'f' => AdministrativeGender.Female,
            'u' => AdministrativeGender.Unknown,
            _ => AdministrativeGender.Other
        };
        return patient.ToIResource();
    }
}