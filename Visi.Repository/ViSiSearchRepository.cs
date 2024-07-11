using System;
using System.Collections.Generic;
using System.Linq;
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
        return resourceType switch
        {
            nameof(Patient) => await SearchPatient(arguments, options),
            nameof(AuditEvent) => new SearchResult(new List<IResource>(), 0),
            _ => throw new NotImplementedException($"ResourceType {resourceType} is not supported.")
        };
    }

    private async Task<SearchResult> SearchPatient(IArgumentCollection arguments, SearchOptions options)
    {
        if (!arguments.HasAny(arg => arg.ArgumentName == "_sort"))
            arguments.AddArgument(new Argument(ArgumentSource.Default, "_sort", "_id"));

        var query = _queryContext.CreateQuery(new PatientQueryFactory(_visiContext), arguments, options);

        var count = await query.ExecuteCount(_visiContext);

        if (count <= 0) return new SearchResult(new List<IResource>(), query.GetPageSize(), count, query.GetSkip());

        var visiPatients = await query.Execute(_visiContext).ToListAsync();

        return new SearchResult(visiPatients.Select(child => _resourceMapper.MapPatient(child)), query.GetPageSize(),
            count, query.GetSkip());
    }
}