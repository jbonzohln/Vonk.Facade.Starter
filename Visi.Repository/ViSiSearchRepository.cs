using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.EntityFrameworkCore;
using Visi.Repository.Models;
using Vonk.Core.Common;
using Vonk.Core.Context;
using Vonk.Core.Pluggability.ContextAware;
using Vonk.Core.Repository;
using Vonk.Core.Support;
using Vonk.Facade.Relational;

namespace Visi.Repository;

[ContextAware(InformationModels = new[] { VonkConstants.Model.FhirR4 })]
public class ViSiSearchRepository : SearchRepository
{
    private readonly ResourceMapper _resourceMapper;
    private readonly ViSiContext _visiContext;

    public ViSiSearchRepository(QueryContext queryBuilderContext, ViSiContext visiContext,
        ResourceMapper resourceMapper) : base(queryBuilderContext)
    {
        Check.NotNull(visiContext, nameof(visiContext));
        Check.NotNull(resourceMapper, nameof(resourceMapper));
        _visiContext = visiContext;
        _resourceMapper = resourceMapper;
    }

    protected override async Task<SearchResult> Search(string resourceType, IArgumentCollection arguments,
        SearchOptions options)
    {
        switch (resourceType)
        {
            case nameof(Patient):
                return await SearchPatient(arguments, options);
            default:
                throw new NotImplementedException($"ResourceType {resourceType} is not supported.");
        }
    }

    private async Task<SearchResult> SearchPatient(IArgumentCollection arguments, SearchOptions options)
    {
        if (arguments.GetArgument("_sort") == null)
        {
            arguments.AddArgument(new Argument(ArgumentSource.Default, "_sort", "_id"));
        }

        var query = _queryContext.CreateQuery(new PatientQueryFactory(_visiContext), arguments, options);

        var count = await query.ExecuteCount(_visiContext);
        var patientResources = new List<IResource>();
        if (count > 0)
        {
            var visiPatients = await query.Execute(_visiContext).ToListAsync();

            foreach (var visiPatient in visiPatients) patientResources.Add(_resourceMapper.MapPatient(visiPatient));
        }

        return new SearchResult(patientResources, query.GetPageSize(), count, query.GetSkip());
    }
}