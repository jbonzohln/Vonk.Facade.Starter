using Hl7.Fhir.Model;
using Hl7.Fhir.Support;
using Visi.Repository.Models;
using Vonk.Core.Common;
using Vonk.Fhir.R4;

namespace Visi.Repository;

public class ResourceMapper
{
    public IResource MapPatient(Child source)
    {
        return new Patient
        {
            Id = source.ChildId.ToString(),
            BirthDate = source.BirthDateTime.ToFhirDate(),
            Gender = source.Sex switch
            {
                'M' => AdministrativeGender.Male,
                'F' => AdministrativeGender.Female,
                'U' => AdministrativeGender.Unknown,
                _ => AdministrativeGender.Other
            },
            Identifier =
            [
                new Identifier("https://kidsnet.health.ri.gov/", source.ChildId.ToString())
                {
                    Value = source.ChildId.ToString(),
                    Type = new CodeableConcept("http://terminology.hl7.org/CodeSystem/v2-0203", "SR",
                        "State Registry ID",
                        "State registry ID"),
                    Use = Identifier.IdentifierUse.Usual
                }
            ],
            Name =
            [
                new HumanName
                {
                    Given =
                    [
                        source.FirstName
                    ],
                    Family = source.LastName
                }
            ]
        }.ToIResource();
    }
}