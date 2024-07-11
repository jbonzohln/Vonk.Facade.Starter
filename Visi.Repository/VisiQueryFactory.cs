using System;
using Microsoft.EntityFrameworkCore;
using Vonk.Core.Common;
using Vonk.Facade.Relational;

namespace Visi.Repository;

public abstract class VisiQueryFactory<E, Q>(string forResourceType, DbContext onContext)
    : RelationalQueryFactory<E, Q>(forResourceType, onContext)
    where E : class
    where Q : RelationalQuery<E>, new()
{
    public override Q EntryInformationModel(string informationModel)
    {
        if (!VonkConstants.Model.FhirR4.Equals(informationModel))
            throw new NotSupportedException(
                $"This facade only supports {VonkConstants.Model.FhirR4}, not {informationModel}");
        return default; //Since we only support 1 FHIR version, there is no need to provide an actual filter.
    }
}