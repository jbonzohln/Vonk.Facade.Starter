using System;
using System.Threading.Tasks;
using Firely.Server.Search.Shared.Indexing;
using Hl7.Fhir.Model;
using Hl7.Fhir.Specification;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Serilog;
using Serilog.Core;
using Visi.Repository.Models;
using Vonk.Core.Common;
using Vonk.Core.ElementModel;
using Vonk.Core.Pluggability.ContextAware;
using Vonk.Core.Repository;
using Vonk.Repository.MongoDb;
using Vonk.Repository.MongoDb.Db;
using Task = System.Threading.Tasks.Task;

namespace Visi.Repository;

[ContextAware(InformationModels = new[] { VonkConstants.Model.FhirR4 })]
public class ViSiChangeRepository(
    ViSiContext visiContext,
    IStructureDefinitionSummaryProvider schemaProvider,
    IOptions<MongoDbOptions> mongoDbOptions,
    MongoResourceMapper resourceMapper,
    SearchParamRegistryFactory searchParamRegistryFactory)
    : IResourceChangeRepository
{
    private readonly SearchParamRegistry searchParamRegistry = searchParamRegistryFactory(_ => true, "ViSiChangeRepository");
    
    public async Task<IResource> Create(IResource input)
    {
        if (input.Type == nameof(AuditEvent))
        {
            Console.WriteLine(mongoDbOptions.Value.ConnectionString);
            
            var client = new MongoClient(mongoDbOptions.Value.ConnectionString);
            var collection = client.GetDatabase("fhir-audit").GetCollection<Entry>("AuditEvent");

            input = input.EnsureMeta();
            var resourceIndex = ResourceIndexer.Index(input.ToTypedElement(schemaProvider).Cache(),
                new IndexOptions(searchParamRegistry));
            foreach (var entry in resourceMapper.MapToEntries(resourceIndex,
                         input.GetChangeIndicator(), input.InformationModel))
                await collection.InsertOneAsync(entry);
            return input;
        }

        throw new NotImplementedException();
    }

    public Task<IResource> Delete(ResourceKey toDelete, string informationModel)
    {
        throw new NotImplementedException();
    }

    public Task<IResource> Update(ResourceKey original, IResource update)
    {
        throw new NotImplementedException();
    }

    public string NewId(string resourceType)
    {
        if (resourceType == nameof(AuditEvent))
        {
            return Uuid.Generate().ToString();
        }
        throw new NotImplementedException();
    }

    public string NewVersion(string resourceType, string resourceId)
    {
        if (resourceType == nameof(AuditEvent))
        {
            return "1";
        }
        throw new NotImplementedException();
    }
}