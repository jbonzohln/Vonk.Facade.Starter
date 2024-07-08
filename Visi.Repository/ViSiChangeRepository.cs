using System;
using System.Threading.Tasks;
using Visi.Repository.Models;
using Vonk.Core.Common;
using Vonk.Core.Pluggability.ContextAware;
using Vonk.Core.Repository;
using Vonk.Core.Support;

namespace Visi.Repository;

[ContextAware(InformationModels = new[] { VonkConstants.Model.FhirR4 })]
public class ViSiChangeRepository : IResourceChangeRepository
{
    private readonly ResourceMapper _resourceMapper;
    private readonly ViSiContext _visiContext;

    public ViSiChangeRepository(ViSiContext visiContext, ResourceMapper resourceMapper)
    {
        Check.NotNull(visiContext, nameof(visiContext));
        Check.NotNull(resourceMapper, nameof(resourceMapper));
        _visiContext = visiContext;
        _resourceMapper = resourceMapper;
    }

    public Task<IResource> Create(IResource input)
    {
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
        throw new NotImplementedException();
    }

    public string NewVersion(string resourceType, string resourceId)
    {
        throw new NotImplementedException();
    }
}